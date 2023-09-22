using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Policies;

public enum PCGGenerateType {Stat, Skill, Item, Agent}
public enum PCGTargetAgentType {Agent, Enemy, All, AllAgent, AllEnemy}
public class OverallGenerator : MonoBehaviour
{
    private List<AbstractAgent> AgentsList = new List<AbstractAgent>();
    private List<AbstractAgent> EnemiesList = new List<AbstractAgent>();
    
    public SkillGenerator skillGenerator;

    private int numberOfAgentsInSingleEnv = 0;
    private int numberOfEnemiesInSingleEnv = 0;

    // Start is called before the first frame update
    void Start()
    {
        numberOfAgentsInSingleEnv = GameObject.FindGameObjectsWithTag("platform")[0].GetComponent<MMORPGEnvController>().AgentsList.Count;
        numberOfEnemiesInSingleEnv = GameObject.FindGameObjectsWithTag("platform")[0].GetComponent<MMORPGEnvController>().EnemiesList.Count;

        foreach (GameObject MMOEnv in GameObject.FindGameObjectsWithTag("platform"))
        {
            for (int i = 0; i < numberOfAgentsInSingleEnv;i++)
            {
                AgentsList.Add(MMOEnv.GetComponent<MMORPGEnvController>().AgentsList[i].Agent);
            }

            for (int i = 0; i < numberOfEnemiesInSingleEnv; i++)
            {
                EnemiesList.Add(MMOEnv.GetComponent<MMORPGEnvController>().EnemiesList[i].Agent);
            }

        }
    }

    bool isItActiveObject(GameObject target)
    {
        bool yesOrNo = false;
        if(target.activeSelf == true)
        {
            yesOrNo = true;
        }
        return yesOrNo;
    }

    // Update is called once per frame
    public void GenerateSomethingWithParameter(PCGTargetAgentType num, PCGGenerateType type, int agentNumber, List<float> source){
        switch (num){
            case PCGTargetAgentType.Agent:
                for(int i = 0; i < ((int)Mathf.Floor(AgentsList.Count/numberOfAgentsInSingleEnv)); i++)
                {
                    SetStatSkillItemAgent(type, AgentsList[i * numberOfAgentsInSingleEnv + agentNumber], source);
                }
            
            break;
            
            case PCGTargetAgentType.Enemy:
                for(int i = 0; i < ((int)Mathf.Floor(EnemiesList.Count/numberOfEnemiesInSingleEnv)); i++)
                {
                    SetStatSkillItemAgent(type, EnemiesList[i * numberOfEnemiesInSingleEnv + agentNumber], source);
                }
            break;
            
            case PCGTargetAgentType.All:
                foreach(AbstractAgent tmpAgent in AgentsList)
                {
                    SetStatSkillItemAgent(type, tmpAgent, source);
                }

                foreach(AbstractAgent tmpAgent in EnemiesList)
                {
                    SetStatSkillItemAgent(type, tmpAgent, source);
                }
            break;

            case PCGTargetAgentType.AllAgent:
                foreach(AbstractAgent tmpAgent in AgentsList)
                {
                    SetStatSkillItemAgent(type, tmpAgent, source);
                }
            break;

            case PCGTargetAgentType.AllEnemy:
                foreach(AbstractAgent tmpAgent in EnemiesList)
                {
                    SetStatSkillItemAgent(type, tmpAgent, source);
                }
            
            break;
        }
    }

    public List<float> RandomlyGenerateList(PCGGenerateType type)
    {
    
        List <float> randomValues = new List<float>();
        switch (type){
            case PCGGenerateType.Stat:
                //Todo : code here
            break;
            
            case PCGGenerateType.Skill:
                randomValues = skillGenerator.GetRandomSkill();
            break;
            
            case PCGGenerateType.Item:
                //Todo : code here
            break;

            case PCGGenerateType.Agent:
                //Todo : code here
            break;
        }
        return randomValues;
    }

    void SetStatSkillItemAgent(PCGGenerateType type, AbstractAgent target, List<float> source){
        switch (type){
            case PCGGenerateType.Stat:
                SetAgentStat(target, source);
            break;
            
            case PCGGenerateType.Skill:
                SetAgentSkill(target, source);
            break;
            
            case PCGGenerateType.Item:
                SetAgentItem(target, source);
            break;

            case PCGGenerateType.Agent:
                AddNewAgent(source);
            break;
        }
    }

    void SetAgentStat(AbstractAgent target, List<float> source){
        //Todo : code here
    }

    void SetAgentSkill(AbstractAgent target, List<float> source){
        int trigerType = (int)source[0];
        int magicSchool = (int)source[1];
        int hitType = (int)source[2];
        int targetType = (int)source[3];
        int projectileSpeed = (int)source[4];
        bool affectOnAlly = source[5] > 0.5f ? true : false;
        bool affectOnEnemy = source[6] > 0.5f ? true : false;
        float range = source[7];
        float cooltime = source[8];
        float castTime = source[9];
        int cost = (int)source[10];
        int nowCharge = (int)source[11];
        int maximumCharge = (int)source[12];
        bool canCastWhileCasting = source[13] > 0.5f ? true : false;
        bool canCastWhileChanneling = source[14] > 0.5f ? true : false;
        float value = source[15];
        int projectileType = (int)source[16];
        int projectileSize = (int)source[17];
        int hitCount = (int)source[18];
        
        int skillNumber = (int)source[19];
        
        if (skillNumber == -1)
        {
            target._skillList.Add(skillGenerator.GenerateSkillWithParameter(
            trigerType, magicSchool, hitType, targetType, projectileSpeed, affectOnAlly, affectOnEnemy,
            range, cooltime, castTime, cost, nowCharge, maximumCharge, canCastWhileCasting, canCastWhileChanneling, value,
            projectileType, projectileSize, hitCount
            ));
        }
        else
        {
            target._skillList[skillNumber] = skillGenerator.GenerateSkillWithParameter(
            trigerType, magicSchool, hitType, targetType, projectileSpeed, affectOnAlly, affectOnEnemy,
            range, cooltime, castTime, cost, nowCharge, maximumCharge, canCastWhileCasting, canCastWhileChanneling, value,
            projectileType, projectileSize, hitCount
            );
        }
    }

    void SetAgentItem(AbstractAgent target, List<float> source){
        int type = (int)source[0];
        int effectedStatus = (int)source[1];
        float grants = source[2];
        float coolDown = source[3];
        int amount = (int)source[4];
        
        // target.m_ItemManager.GenerateItemWithParameters(type, effectedStatus, grants, coolDown, amount);
    }

    void AddNewAgent(List<float> source){
        //Todo : code here
    }
    
    void DeleteMultipleTimeChecker(PCGTargetAgentType num, PCGGenerateType type, int agentNumber){
        //Todo : code here
    }

    void DeleteSomeThing(PCGGenerateType type, AbstractAgent target){
        switch (type){
            case PCGGenerateType.Skill:
            //Todo : code here
            break;
            
            case PCGGenerateType.Item:
            //Todo : code here
            break;

            case PCGGenerateType.Agent:
            //Todo : code here
            break;
        }
    }

    public List<float> GetSkillParameter(string target)
    {
        return this.skillGenerator.GetSkillParameter(target);
    }

    public List<float> GetSkillParameter(PCGTargetAgentType target, int targetAgentNumber, int targetSkillNumber)
    {
        AbstractSkill foundSkill = new AbstractSkill(); 
        if(target == PCGTargetAgentType.Agent || target == PCGTargetAgentType.AllAgent || target == PCGTargetAgentType.All)
        {
            foundSkill = AgentsList[targetAgentNumber]._skillList[targetSkillNumber];   
        }
        else
        {
            foundSkill = EnemiesList[targetAgentNumber]._skillList[targetSkillNumber];
        }

        return this.skillGenerator.GetSkillParameter(foundSkill);
    }
}
