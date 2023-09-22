using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class OverallProxy : MonoBehaviour
{
    public static OverallProxy Instance { get; private set; }
    public List<MMORPGEnvController> PlatformList;

    private myskillArray myskill;
    private adjustment adjustment;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        PlatformList = new List<MMORPGEnvController>();
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("platform");

        foreach (GameObject obj in platforms)
        {
            PlatformList.Add(obj.GetComponent<MMORPGEnvController>());
        }

        // Set default skill
        string skillString;
        if (ParameterManagerSingleton.GetInstance().HasParam("skillPath"))
        {
            string skillPath = Convert.ToString(ParameterManagerSingleton.GetInstance().GetParam("skillPath"));
            skillString = File.ReadAllText(skillPath);
        }
        else
        {
            skillString = Resources.Load<TextAsset>("JSON/skill").ToString();
        }
        myskill = JsonUtility.FromJson<myskillArray>(skillString);
    }

    void Start()
    {
        foreach (MMORPGEnvController platform in PlatformList)
        {

            int i_player = 0;
            foreach (var agentInfo in platform.AgentsList)
            {
                AbstractAgent agent = agentInfo.Agent as AbstractAgent;

                Hashtable config = new Hashtable();
                if (agent.ruleBased)
                {
                    config.Add("skill1", "MagicMissile");
                    config.Add("skill2", "BlackHole");
                    config.Add("skill3", "BlackHole");
                }
                else
                {
                    config.Add("skill1", myskill.player[i_player].skill1);
                    config.Add("skill2", myskill.player[i_player].skill2);
                    config.Add("skill3", myskill.player[i_player].skill3);
                }
                agent.SetConfig(config);

            }


            i_player += 1;

            List<AbstractAgent> agentList = new List<AbstractAgent>();
            foreach (var agentInfo in platform.AgentsList)
            {
                agentList.Add(agentInfo.Agent);
            }
            SetSkillConfig(agentList);
        }
    }

    public void RequestMAConfig(List<AbstractAgent> agents)
    {
        SetSkillConfig(agents);
    }

    public void ApplySkill(float range)
    {   
        foreach (MMORPGEnvController platform in PlatformList)
        {
            foreach (var agentInfo in platform.AgentsList)
            {
                RaidPlayerAgent agent = agentInfo.Agent as RaidPlayerAgent;

                if (myskill.adjustment.range.Length > 0)
                {
                    agent._skillList[0].condition.range = range;
                }
            }
        }
    }

    void SetSkillConfig(List<AbstractAgent> agents)
    {
        // adjust skill parameter
        int randomIdxCooltime = UnityEngine.Random.Range(0, myskill.adjustment.cooltime.Length);
        int randomIdxRange = UnityEngine.Random.Range(0, myskill.adjustment.range.Length);
        int randomIdxCasttime = UnityEngine.Random.Range(0, myskill.adjustment.casttime.Length);
        int randomIdxValue = UnityEngine.Random.Range(0, myskill.adjustment.value.Length);

        string str = "";
        for (int i = 0; i < agents.Count; i++)
        {
            AbstractAgent agent = agents[i];

            if (agent is RaidPlayerAgent)
            {
                // condition.cooltime
                if (myskill.adjustment.cooltime.Length > 0)
                {
                    // DEFAULT: condition.cooltime = 2.0f;
                    agent._skillList[0].condition.cooltime = myskill.adjustment.cooltime[randomIdxCooltime];
                    str += agent._skillList[0].condition.cooltime + ",";
                }
                // condition.range
                if (myskill.adjustment.range.Length > 0)
                {
                    // DEFAULT: condition.range = 40.0f;
                    agent._skillList[0].condition.range = myskill.adjustment.range[randomIdxRange];
                    str += agent._skillList[0].condition.range + ",";
                }
                // condition.casttime
                if (myskill.adjustment.casttime.Length > 0)
                {
                    // DEFAULT: condition.casttime = 0.0f;
                    agent._skillList[0].condition.casttime = myskill.adjustment.casttime[randomIdxCasttime];
                    str += agent._skillList[0].condition.casttime + ",";
                }
                // coefficient.value
                if (myskill.adjustment.value.Length > 0)
                {
                    // DEFAULT: coefficient.value = 0.5f;
                    agent._skillList[0].coefficient.value = myskill.adjustment.value[randomIdxValue];
                    str += agent._skillList[0].coefficient.value + ",";
                }
            }
        }
    }

    public List<float> GetSkillParameterArray()
    {
        AbstractSkill source = PlatformList[0].AgentsList[0].Agent._skillList[0];
        List<float> safeSkillParameter = new List<float>();

        safeSkillParameter.Add((float)(int)source.info.triggerType);
        safeSkillParameter.Add((float)(int)source.info.magicSchool);
        safeSkillParameter.Add((float)(int)source.info.hitType);
        safeSkillParameter.Add((float)(int)source.info.targetType);
        safeSkillParameter.Add((float)source.info.projectileSpeed);
        safeSkillParameter.Add((source.info.affectOnAlly == true ? 1.0f : 0.0f));
        safeSkillParameter.Add((source.info.affectOnEnemy == true ? 1.0f : 0.0f));
        
        safeSkillParameter.Add(source.condition.range);
        safeSkillParameter.Add(source.condition.cooltime);
        safeSkillParameter.Add(source.condition.casttime);
        safeSkillParameter.Add((float)source.condition.cost);
        safeSkillParameter.Add((float)source.condition.nowCharged);
        safeSkillParameter.Add((float)source.condition.maximumCharge);
        safeSkillParameter.Add((source.condition.canCastWhileCasting == true ? 1.0f : 0.0f));
        safeSkillParameter.Add((source.condition.canCastWhileChanneling == true ? 1.0f : 0.0f));

        safeSkillParameter.Add(source.coefficient.value);

        safeSkillParameter.Add((float)(int)source.projectileFX.type);
        safeSkillParameter.Add((float)(int)source.projectileFX.size);
        
        safeSkillParameter.Add(source.terminalCondition.hitCount);
        
        return safeSkillParameter;

    }
}

[System.Serializable]
public class agent
{
    public int tag;
    public string skill1, skill2, skill3;
}
[System.Serializable]
public class adjustment
{
    public float[] cooltime;
    public float[] range;
    public float[] casttime;
    public float[] value;

    
}
[System.Serializable]
public class myskillArray
{
    public agent[] enemy, player;
    public int[] skillCount;
    public adjustment adjustment;
}
