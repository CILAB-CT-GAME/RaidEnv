using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UIAgentGroup : MonoBehaviour
{
    public static UIAgentGroup Instance { get; private set; }
    public GameObject agentInGroup;

    private Vector3 startPos = new Vector3(4, 0, 0);
    private Vector3 intervalY = new Vector3(0, 100, 0);
    private Vector3 strideY = new Vector3(0, 4, 0);

    private int numOfAgent;
    private Dictionary<int, GameObject> frameDict;
    private GameObject _platformInstance;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Update()
    {
        var agents = Common.FindChildWithTag(_platformInstance.transform, "agent");
        UpdatAgents(agents);
    }

    public void SetPlatform(GameObject platform)
    {
        _platformInstance = platform;
    }



    public void OnAwake()
    {
        numOfAgent = Common.FindChildWithTag(_platformInstance.transform, "agent").Count;
        frameDict = new Dictionary<int, GameObject>();
        for (int i = 0; i < numOfAgent; i++)
        {
            var go = Instantiate(agentInGroup, agentInGroup.transform.parent);
            go.transform.localPosition = startPos - (intervalY * i);
            go.name = "UI Agent " + i.ToString() + " in Group";
            frameDict[i] = go;
            go.SetActive(true);
        }
    }

    void UpdatAgents(List<GameObject> agents)
    {
        for (int i = 0; i < agents.Count; i++)
        {
            var frame = frameDict[i].gameObject;
            var className = agents[i].GetComponent<AbstractAgent>()._class.classInfo.className;
            var status = agents[i].GetComponent<AbstractAgent>()._status;
            var skills = agents[i].GetComponent<AbstractAgent>()._skillList;

            UpdatePortrait(frame, className);
            UpdateStatus(frame, status);
            UpdateSkills(frame, skills);
        }
    }

    void UpdatePortrait(GameObject frame, string className)
    {
        var portrait_image = Common.FindChildWithName(frame.transform, "Portrait").GetComponent<Image>();
        var sprite = Resources.Load<Sprite>("AgentsPortrait/" + className);

        if (sprite != null)
            portrait_image.sprite = sprite;
    }

    void UpdateStatus(GameObject frame, AbstractStatus status)
    {
        var hpbar = Common.FindChildWithName(frame.transform, "HP").GetComponent<Image>();
        var mpbar = Common.FindChildWithName(frame.transform, "MP").GetComponent<Image>();

        // portrait_image = 
        hpbar.fillAmount = (float)status.health.current / status.health.max;
        mpbar.fillAmount = (float)status.mana.current / status.mana.max;
    }

    void UpdateSkills(GameObject frame, List<AbstractSkill> skills)
    {
        for (int i = 0; i < skills.Count; i++)
        {
            var skill_i_image = Common.FindChildWithName(frame.transform, "Skill " + (i + 1).ToString()).GetComponent<Image>();
            if (skill_i_image != null)
            {
                if (Resources.Load<Sprite>("SkillIcon/" + skills[i].info.name) == null || skills[i].info.name == "BlackHole")
                {
                    continue;
                }
                else
                {
                    skill_i_image.sprite = Resources.Load<Sprite>("SkillIcon/" + skills[i].info.name);
                    skill_i_image.fillAmount = 1 - (skills[i].condition.cooltimeLeft / skills[i].condition.cooltime);
                }
            }
        }
    }
}
