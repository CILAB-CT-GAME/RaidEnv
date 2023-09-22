using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.MLAgents;

[Serializable]
public class Skill
{
    public string name;
    public string triggerType;
    public string magicSchool;
    public string hitType;
    public string targetType;
    public int projectileSpeed;
    public bool affectOnAlly;
    public bool affectOnEnemy;

    public float cooltime;
    public float casttime;
    public int cost;
    public float range;
    public bool canCastWhileMoving;
    public bool canCastWhileCasting;
    public bool canCastWhileChanneling;
    public int nowCharged;
    public int maximumCharge;

    public float interval;
    public float duration;
    public float value;
    
    public string type;
    public string size;

    public int hitCount;
}

[Serializable]
public class SkillData
{
    public List<Skill> skills = new List<Skill>();
}

public class DataManager
{
    SkillData data;
    TextAsset textAssets;
    public void init()
    {
        textAssets = Resources.Load<TextAsset>("SkillData/Skill");
        data = JsonUtility.FromJson<SkillData>(textAssets.text);
    }

    public List<AbstractSkill> GetSkillList()
    {
        List<AbstractSkill> skillList = new List<AbstractSkill>();
        skillList.Add(new AutoAttack());

        return skillList;
    }

    public AbstractSkill UnwrapSkill(Skill skill_wrapped)
    {
        AbstractSkill tempSkill = new AbstractSkill();
        tempSkill.info.name = skill_wrapped.name;
        switch (skill_wrapped.triggerType)
        {
            case "Active":
                tempSkill.info.triggerType = TriggerType.Active;
                break;
            case "Passive":
                tempSkill.info.triggerType = TriggerType.Passive;
                break;
        }
        switch (skill_wrapped.magicSchool)
        {
            case "Arcane":
                tempSkill.info.magicSchool = MagicSchool.Arcane;
                break;
            case "Earth":
                tempSkill.info.magicSchool = MagicSchool.Earth;
                break;
            case "Fire":
                tempSkill.info.magicSchool = MagicSchool.Fire;
                break;
            case "Frost":
                tempSkill.info.magicSchool = MagicSchool.Frost;
                break;
            case "Life":
                tempSkill.info.magicSchool = MagicSchool.Life;
                break;
            case "Light":
                tempSkill.info.magicSchool = MagicSchool.Light;
                break;
            case "Lightning":
                tempSkill.info.magicSchool = MagicSchool.Lightning;
                break;
            case "Shadow":
                tempSkill.info.magicSchool = MagicSchool.Shadow;
                break;
            case "Storm":
                tempSkill.info.magicSchool = MagicSchool.Storm;
                break;
            case "Water":
                tempSkill.info.magicSchool = MagicSchool.Water;
                break;
        }

        return tempSkill;
    }
}