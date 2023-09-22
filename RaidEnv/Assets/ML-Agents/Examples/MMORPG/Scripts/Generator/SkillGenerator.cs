using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEngine;

[System.Serializable]
public class GeneratedSkill{
    public string name;
    public int triggerType;
    public int magicSchool;
    public int hitType;
    public int targetType;
    public int projectileSpeed;
    public bool affectOnAlly;
    public bool affectOnEnemy;
    public float range;
    public float cooltime;
    public float casttime;
    public int cost;
    public int nowCharged;
    public int maximumCharge;
    public bool canCastWhileCasting;
    public bool canCastWhileChanneling;
    public float value;
    public int type;
    public int size;
    public int hitCount;
}

public class SkillGenerator : MonoBehaviour
{
    public class MinMaxThreshold
    {
        public int[] triggerType = {0, System.Enum.GetValues(typeof(TriggerType)).Length};
        public int[] magicSchool = {0, System.Enum.GetValues(typeof(MagicSchool)).Length};
        public int[] hitType = {0, System.Enum.GetValues(typeof(HitType)).Length};
        public int[] targetType = {0, System.Enum.GetValues(typeof(TargetType)).Length};
        public int[] projectileSpeed = {10, 10};
        public float[] range = {1.0f, 20.0f};
        public float[] cooltime = {0.5f, 0.6f};
        public float[] casttime = {0.5f, 1.5f};
        // public int[] cost = {1, 10};
        public int[] cost = {1, 1};
        public int[] nowCharged = {1, 1};
        public int[] maximumCharge = {1, 1};
        public float[] value = {0.5f, 1.0f};
        public int[] projectileType = {0, System.Enum.GetValues(typeof(ProjectileType)).Length};
        public int[] hitCount = {1, 1};
    }


    [Header("Skill Value Threshold")]
    public MinMaxThreshold minMaxThreshold = new MinMaxThreshold();
    [Header("Information")]
    public List<AbstractSkill> skillList = new List<AbstractSkill>();
    public List<GeneratedSkill> currentlyGeneratedSkills = new List<GeneratedSkill>();
    public string skillName;
    public TriggerType triggerType;
    public MagicSchool magicSchool;
    public HitType hitType;
    public TargetType targetType;
    public ProjectileType projectileType;
    public int projectileSpeed;
    public bool canUseToAlly;
    public bool canUseToEnemy;
    public PCGTargetAgentType targetAgent;
    public int SwitchThisSkillWithGeneratedSkill = -1;

    [Header("Condition")]
    public int cost;
    public float range;
    public float cooltime;
    public float casttime;
    public int nowCharged; 
    public int maximumCharge;
    public bool canCastWhileMoving;
    public bool canCastWhileCasting;
    public bool canCastWhileChanneling;

    [Header("Effect")]
    public float value;
    public float interval;
    public float duration;

    internal int GetRandomEnumComponent(System.Type type)
    {
        return Random.Range(0, System.Enum.GetValues(type).Length);
    }

    internal int GetRandomArrayComponent(int[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    internal float GetRandomArrayComponent(float[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    internal string GetRandomSkillName(int length)
    {
        string letterPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        string result = "";
        while (result.Length < length)
        {
            result += letterPool[Random.Range(0, letterPool.Length)];
        }
        return "SKILL_" + result;
    }

    public void InitializeSkillSet(string type)
    {
        this.skillList.Clear();
        switch (type)
        {
            case "enemy":
                this.skillList.Add(new BossAttack_1());
                this.skillList.Add(new BossAttack_2());
                break;
            case "agent":
                string test = "MagicMissile";
                Assembly assembly = Assembly.GetExecutingAssembly();
                System.Type t = assembly.GetType(test);
                object tmp = System.Activator.CreateInstance(t);
                this.skillList.Add((AbstractSkill)tmp);
                this.skillList.Add(new BlackHole());
                this.skillList.Add(new BlackHole());
                break;
        }
    }

    public List<float> GetRandomSkill()
    {
        List<float> generatedSkill = new List<float>();
        // Information
        // skillName = GetRandomSkillName(8);
        triggerType = TriggerType.Active;
        magicSchool = (MagicSchool)GetRandomEnumComponent(typeof(MagicSchool));
        hitType = HitType.Spell;
        
        // 1
        // targetType = (TargetType)GetRandomArrayComponent(new int[] { 0, 1 });
        targetType = (TargetType)(1);
        
        projectileSpeed = Random.Range(minMaxThreshold.projectileSpeed[0], minMaxThreshold.projectileSpeed[1]); // 2
        // canUseToAlly = Random.Range(0.0f, 1.0f) > 0.5f ? true : false;
        // canUseToEnemy = Random.Range(0.0f, 1.0f) > 0.5f ? true : false
        canUseToAlly = false;
        canUseToEnemy = true;

        // Condition
        cost = Random.Range(minMaxThreshold.cost[0], minMaxThreshold.cost[1]);
        range = Random.Range(minMaxThreshold.range[0], minMaxThreshold.range[1]);
        cooltime = Random.Range(minMaxThreshold.cooltime[0], minMaxThreshold.cooltime[1]); // 1
        casttime = Random.Range(minMaxThreshold.casttime[0], minMaxThreshold.casttime[1]);
        // nowCharged = Random.Range(minMaxThreshold.nowCharged[0], minMaxThreshold.nowCharged[1]);

        maximumCharge = Random.Range(minMaxThreshold.maximumCharge[0], minMaxThreshold.maximumCharge[1]); //
        // canCastWhileMoving = Random.Range(0.0f, 1.0f) > 0.5f ? true : false;
        canCastWhileMoving = true;
        // canCastWhileCasting = Random.Range(0.0f, 1.0f) > 0.5f ? true : false;
        // canCastWhileChanneling = Random.Range(0.0f, 1.0f) > 0.5f ? true : false;
        canCastWhileCasting = false;
        canCastWhileChanneling = false;

        // Effect
        value = Random.Range(minMaxThreshold.value[0], minMaxThreshold.value[1]);
        interval = Random.Range(0.0f, 1.0f);
        duration = Random.Range(0.0f, 60.0f);

        generatedSkill.Add((float)(int)triggerType); // 1
        generatedSkill.Add((float)(int)magicSchool);  // 2
        generatedSkill.Add((float)(int)hitType);  // 3
        generatedSkill.Add((float)(int)targetType);  // 4
        generatedSkill.Add((float)projectileSpeed);  // 5
        generatedSkill.Add((canUseToAlly == true ? 1.0f : 0.0f));   // 6
        generatedSkill.Add((canUseToEnemy == true ? 1.0f : 0.0f));  // 7
        generatedSkill.Add(range);  // 8
        generatedSkill.Add(cooltime);  // 9
        generatedSkill.Add(casttime);   // 10
        generatedSkill.Add((float)cost);    // 11
        generatedSkill.Add((float)maximumCharge); // Do not touch this  // 12
        generatedSkill.Add((float)maximumCharge);  // 13
        generatedSkill.Add((canCastWhileCasting == true ? 1.0f : 0.0f));  // 14
        generatedSkill.Add((canCastWhileChanneling == true ? 1.0f : 0.0f));  // 15
        generatedSkill.Add(value);  // 16
        generatedSkill.Add(0f); // projectileFX.type // 17
        generatedSkill.Add(0f); // projectileFX.size  // 18
        generatedSkill.Add(1f); // hitCount  // 19

        return generatedSkill;
    }

    public void GenerateManually()
    {
        AbstractSkill ManuallyGeneratedSkill = new AbstractSkill();

        // ManuallyGeneratedSkill.SetValues(
        //     skillName, (int)triggerType, (int)magicSchool, (int)hitType, (int)targetType, projectileSpeed, 
        //     canUseToAlly, canUseToEnemy, range, cooltime, casttime, cost, 1, maximumCharge, 
        //     canCastWhileCasting, canCastWhileChanneling, value, (int)projectileType, (int)ProjectileSize.Mega, 1
        // );

        this.SaveSkillParameterOnTheArray(
            skillName, (int)triggerType, (int)magicSchool, (int)hitType, (int)targetType,
            projectileSpeed, canUseToAlly, canUseToEnemy, range, cooltime, casttime, cost, 1, maximumCharge, 
            canCastWhileCasting, canCastWhileChanneling, value, (int)projectileType, (int)ProjectileSize.Mega, 1
        );

    }
    
    public AbstractSkill GenerateSkillWithParameter(int sourceTrigerType, int sourceMagicSchool,
        int sourceHitType, int sourceTargetType, int sourceProjectileSpeed,
        bool sourceAffectOnAlly, bool sourceAffectOnEnemy,
        float sourceRange, float sourceCoolTime, float sourceCastTime, int sourceCost,
        int sourceNowCharge, int sourceMaximumCharge,
        bool sourceCanCastWhileCasting, bool sourceCanCastWhileChanneling,
        float sourceValue, int sourceProjectileType, int sourceProjectileSize,
        int sourceHitCount
        )
    {
        AbstractSkill generatedSkillWithParameters = new AbstractSkill();
        // string sourceName = GetRandomSkillName(8);
        string sourceName = "MagicMissile";

        sourceTrigerType = Mathf.Min(minMaxThreshold.triggerType[1], sourceTrigerType);
        sourceTrigerType = Mathf.Max(minMaxThreshold.triggerType[0], sourceTrigerType);

        sourceMagicSchool = Mathf.Min(minMaxThreshold.magicSchool[1], sourceMagicSchool);
        sourceMagicSchool = Mathf.Max(minMaxThreshold.magicSchool[0], sourceMagicSchool);

        sourceHitType = Mathf.Min(minMaxThreshold.hitType[1], sourceHitType);
        sourceHitType = Mathf.Max(minMaxThreshold.hitType[0], sourceHitType);

        sourceTargetType = Mathf.Min(minMaxThreshold.targetType[1], sourceTargetType);
        sourceTargetType = Mathf.Max(minMaxThreshold.targetType[0], sourceTargetType);

        sourceProjectileSpeed = Mathf.Min(minMaxThreshold.projectileSpeed[1], sourceProjectileSpeed);
        sourceProjectileSpeed = Mathf.Max(minMaxThreshold.projectileSpeed[0], sourceProjectileSpeed);

        sourceCastTime = Mathf.Min(minMaxThreshold.casttime[1], sourceCastTime);
        sourceCastTime = Mathf.Max(minMaxThreshold.casttime[0], sourceCastTime);

        sourceCost = Mathf.Min(minMaxThreshold.cost[1], sourceCost);
        sourceCost = Mathf.Max(minMaxThreshold.cost[0], sourceCost);

        sourceRange = Mathf.Min(minMaxThreshold.range[1], sourceRange);
        sourceRange = Mathf.Max(minMaxThreshold.range[0], sourceRange);

        sourceCoolTime = Mathf.Min(minMaxThreshold.cooltime[1], sourceCoolTime);
        sourceCoolTime = Mathf.Min(minMaxThreshold.cooltime[1], sourceCoolTime);

        sourceNowCharge = Mathf.Min(minMaxThreshold.nowCharged[1], sourceNowCharge);
        sourceNowCharge = Mathf.Max(minMaxThreshold.nowCharged[0], sourceNowCharge);

        sourceMaximumCharge = Mathf.Min(minMaxThreshold.maximumCharge[1], sourceMaximumCharge);
        sourceMaximumCharge = Mathf.Max(minMaxThreshold.maximumCharge[0], sourceMaximumCharge);

        sourceValue = Mathf.Min(minMaxThreshold.value[1], sourceValue);
        sourceValue = Mathf.Max(minMaxThreshold.value[0], sourceValue);

        sourceProjectileType = Mathf.Min(minMaxThreshold.projectileType[1], sourceProjectileType);
        sourceProjectileType = Mathf.Max(minMaxThreshold.projectileType[0], sourceProjectileType);
        
        sourceHitCount = Mathf.Min(minMaxThreshold.hitCount[1], sourceHitCount);
        sourceHitCount = Mathf.Max(minMaxThreshold.hitCount[0], sourceHitCount);

        generatedSkillWithParameters.SetValues(
            sourceName, sourceTrigerType, sourceMagicSchool, sourceHitType, sourceTargetType, sourceProjectileSpeed,
            sourceAffectOnAlly, sourceAffectOnEnemy, sourceRange, sourceCoolTime, sourceCastTime, sourceCost, sourceNowCharge, sourceMaximumCharge,
            sourceCanCastWhileCasting, sourceCanCastWhileChanneling, sourceValue, sourceProjectileType, sourceProjectileSize, sourceHitCount
        );
        // if (switchSkillNow == true)
        // {
        //     SwitchCurrentSkilltoSomeSkill(generatedSkillWithParameters,switchThis);
        // }
        // if(automaticSaveToList == true)
        // {
        //     this.SaveSkillParameterOnTheArray(
        //     sourceName, sourceTrigerType, sourceMagicSchool, sourceHitType, sourceTargetType, sourceProjectileSpeed,
        //     sourceAffectOnAlly, sourceAffectOnEnemy, sourceCastTime, sourceCost, sourceRange, 1, sourceMaximumCharge,
        //     sourceCanCastWhileCasting, sourceCanCastWhileChanneling, sourceValue, sourceProjectileType, (int)ProjectileSize.Mega, 1
        // );
        // }
        return generatedSkillWithParameters;
    }

    AbstractSkill GenerateSkillFromExistParameter(int target)
    {
        AbstractSkill generatedSkillWithParameters = new AbstractSkill();
        generatedSkillWithParameters = GenerateSkillWithParameter
        (
            this.currentlyGeneratedSkills[target].triggerType,
            this.currentlyGeneratedSkills[target].magicSchool,
            this.currentlyGeneratedSkills[target].hitType,
            this.currentlyGeneratedSkills[target].targetType,
            this.currentlyGeneratedSkills[target].projectileSpeed,
            this.currentlyGeneratedSkills[target].affectOnAlly,
            this.currentlyGeneratedSkills[target].affectOnEnemy,
            this.currentlyGeneratedSkills[target].range,
            this.currentlyGeneratedSkills[target].cooltime,
            this.currentlyGeneratedSkills[target].casttime,
            this.currentlyGeneratedSkills[target].cost,
            this.currentlyGeneratedSkills[target].nowCharged,
            this.currentlyGeneratedSkills[target].maximumCharge,
            this.currentlyGeneratedSkills[target].canCastWhileCasting,
            this.currentlyGeneratedSkills[target].canCastWhileChanneling,
            this.currentlyGeneratedSkills[target].value,
            this.currentlyGeneratedSkills[target].size,
            this.currentlyGeneratedSkills[target].type,
            this.currentlyGeneratedSkills[target].hitCount
        );
        return generatedSkillWithParameters;
    }

    void SaveSkillParameterOnTheArray(
        string sourceName, int sourceTrigerType, int sourceMagicSchool,
        int sourceHitType, int sourceTargetType, int sourceProjectileSpeed,
        bool sourceAffectOnAlly, bool sourceAffectOnEnemy,
        float sourceRange, float sourceCoolTime, float sourceCastTime, int sourceCost,
        int sourceNowCharge, int sourceMaximumCharge,
        bool sourceCanCastWhileCasting, bool sourceCanCastWhileChanneling,
        float sourceValue, int sourceProjectileType, int sourceProjectileSize,
        int sourceHitCount
    )
    {
        GeneratedSkill tmpGenedSkill = new GeneratedSkill();

        tmpGenedSkill.name = sourceName;
        tmpGenedSkill.triggerType = sourceTrigerType;
        tmpGenedSkill.magicSchool = sourceMagicSchool;
        tmpGenedSkill.hitType = sourceHitType;
        tmpGenedSkill.targetType = sourceTargetType;
        tmpGenedSkill.projectileSpeed = sourceProjectileSpeed;
        tmpGenedSkill.affectOnAlly = sourceAffectOnAlly;
        tmpGenedSkill.affectOnEnemy = sourceAffectOnEnemy;

        tmpGenedSkill.range = sourceRange;
        tmpGenedSkill.cooltime = sourceCoolTime;
        tmpGenedSkill.casttime = sourceCastTime;
        tmpGenedSkill.cost = sourceCost;
        tmpGenedSkill.nowCharged = sourceNowCharge;
        tmpGenedSkill.maximumCharge = sourceMaximumCharge;
        tmpGenedSkill.canCastWhileCasting = sourceCanCastWhileCasting;
        tmpGenedSkill.canCastWhileChanneling = sourceCanCastWhileChanneling;

        tmpGenedSkill.value = sourceValue;

        tmpGenedSkill.type = sourceProjectileType;
        tmpGenedSkill.size = sourceProjectileSize;

        tmpGenedSkill.hitCount = sourceHitCount;

        currentlyGeneratedSkills.Add(tmpGenedSkill);
    }
    public List<float> GetSkillParameter(string presetName){
        List<float> safeSkillParameter = new List<float>();

        Assembly assembly = Assembly.GetExecutingAssembly();
        System.Type t = assembly.GetType(presetName);
        object source = System.Activator.CreateInstance(t);

        safeSkillParameter = GetSkillParameter((AbstractSkill)source);
        return safeSkillParameter;
    }
    public List<float> GetSkillParameter(AbstractSkill source){
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

