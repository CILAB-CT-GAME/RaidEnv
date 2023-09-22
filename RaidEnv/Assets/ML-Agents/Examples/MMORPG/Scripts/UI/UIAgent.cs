using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;


// TODO
// 가진 스킬 수에 대응하도록
public class UIAgent : MonoBehaviour
{
    public static UIAgent Instance { get; private set; }
    public Image portraitImage;
    public Image HPBar;
    public Image MPBar;

    [SerializeField]
    private List<Image> skillImages;

    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI attackSpeedText;

    private List<GameObject> agents;
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
        agents = Common.FindChildWithTag(_platformInstance.transform, "agent");

        UpdatePortrait();
        UpdateStatus();
        UpdateSkills();
    }

    public void OnAwake()
    {
        agents = Common.FindChildWithTag(_platformInstance.transform, "agent");

        if (agents == null) return;
    }

    public void SetPlatform(GameObject platform)
    {
        _platformInstance = platform;
    }

    void UpdatePortrait()
    {
        var _class = agents[0].GetComponent<AbstractAgent>()._class;
        var sprite = Resources.Load<Sprite>("AgentsPortrait/" + _class.classInfo.className);

        if (sprite != null)
            portraitImage.sprite = sprite;
    }

    void UpdateStatus()
    {
        var agent = agents[0];
        var status = agent.GetComponent<AbstractAgent>()._status;

        HPBar.fillAmount = (float)status.health.current / status.health.max;
        MPBar.fillAmount = (float)status.mana.current / status.mana.max;
        attackText.text = status.attack.power.ToString();
        defenseText.text = status.defensive.armor.ToString();
        rangeText.text = status.attack.range.ToString();
        attackSpeedText.text = status.attack.speed.ToString();
    }

    void UpdateSkills()
    {
        var agent = agents[0];
        var skills = agent.GetComponent<AbstractAgent>()._skillList;
        for (int i = 0; i < skills.Count; i++)
        {
            if (skillImages[i] != null)
            {
                if (Resources.Load<Sprite>("SkillIcon/" + skills[i].info.name) == null || skills[i].info.name == "BlackHole")
                {
                    continue;
                }
                else
                {
                    skillImages[i].sprite = Resources.Load<Sprite>("SkillIcon/" + skills[i].info.name);
                    skillImages[i].fillAmount = 1 - (skills[i].condition.cooltimeLeft / skills[i].condition.cooltime);
                }
            }
        }
    }
}
