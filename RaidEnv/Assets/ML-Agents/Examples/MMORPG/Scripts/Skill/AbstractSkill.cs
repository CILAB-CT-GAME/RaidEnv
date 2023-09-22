using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.MLAgents;

public enum TriggerType
{
    Active,
    Passive
}

public enum MagicSchool
{
    Arcane,
    Earth,
    Fire,
    Frost,
    Life,
    Light,
    Lightning,
    Shadow,
    Storm,
    Water
}

public enum HitType
{
    Attack,
    Spell
}

public enum TargetType
{
    NonTarget,
    Target,
    Region
}

public enum ProjectileType
{
    Missile,
    Beam,
    Slash,
    PillarBlast
}

public enum ProjectileSize
{
    Tiny,
    Small,
    Normal,
    Mega
}

public class AbstractSkill
{
    public struct Info
    {
        public string uuid;
        public string name;
        public TriggerType triggerType;
        public MagicSchool magicSchool;
        public HitType hitType;
        public TargetType targetType;
        public int projectileSpeed; // 0: means use skill insantly on target
        public bool affectOnAlly;
        public bool affectOnEnemy;
    }

    public struct Condition
    {
        public float cooltime;
        public float cooltimeLeft; //UI용 추가
        public float casttime;
        public int cost;
        public float range;
        public bool canCastWhileMoving;
        public bool canCastWhileCasting;
        public bool canCastWhileChanneling;
        public int nowCharged;
        public int maximumCharge;
        public bool isCooldown;
    }

    public struct TerminalCondition
    {
        public int hitCount;
    }

    public struct Coefficient
    {
        public float value;
        public float criticalChance;
        public float criticalMultiplier;
    }

    public struct ProjectileFX
    {
        public struct Particle
        {
            public string projectile;
            public string impact;
            public string muzzle;
        }

        public ProjectileType type;
        public ProjectileSize size;
        public Particle particle;
        // public string projectileParticleName;
        // public string impactParticleName;
        // public string muzzleParticleName;
    }

    public Info info;
    public Condition condition;
    public Coefficient coefficient;
    public ProjectileFX projectileFX;
    public TerminalCondition terminalCondition;

    public void InitializeSkillValues()
    {
        info.uuid = Guid.NewGuid().ToString();

        projectileFX.particle.projectile = null;
        projectileFX.particle.impact = null;
        projectileFX.particle.muzzle = null;

        switch (projectileFX.type)
        {
            case ProjectileType.Missile:
                projectileFX.particle.projectile =
                    info.magicSchool.ToString()
                    + projectileFX.type.ToString()
                    + projectileFX.size.ToString();
                projectileFX.particle.impact =
                    info.magicSchool.ToString() + "Explosion" + projectileFX.size.ToString();
                projectileFX.particle.muzzle = info.magicSchool.ToString() + "MuzzleBig";
                break;
            default:
                projectileFX.particle.projectile =
                    info.magicSchool.ToString() + projectileFX.type.ToString();
                break;
        }
    }

    public void SetValues(
        string sourceName,
        int sourceTrigerType,
        int sourceMagicSchool,
        int sourceHitType,
        int sourceTargetType,
        int sourceProjectileSpeed,
        bool sourceAffectOnAlly,
        bool sourceAffectOnEnemy,
        float sourceRange,
        float sourceCoolTime,
        float sourceCastTime,
        int sourceCost,
        int sourceNowCharge,
        int sourceMaximumCharge,
        bool sourceCanCastWhileCasting,
        bool sourceCanCastWhileChanneling,
        float sourceValue,
        int sourceProjectileType,
        int sourceProjectileSize,
        int sourceHitCount
    )
    {
        info.name = sourceName;
        info.triggerType = (TriggerType)sourceTrigerType;
        info.magicSchool = (MagicSchool)sourceMagicSchool;
        info.hitType = (HitType)sourceHitType;
        info.targetType = (TargetType)sourceTargetType;
        info.projectileSpeed = sourceProjectileSpeed;
        info.affectOnAlly = sourceAffectOnAlly;
        info.affectOnEnemy = sourceAffectOnEnemy;

        condition.range = sourceRange;
        condition.cooltime = sourceCoolTime;
        condition.casttime = sourceCastTime;
        condition.cost = sourceCost;

        condition.nowCharged = sourceNowCharge;
        condition.maximumCharge = sourceMaximumCharge;
        condition.canCastWhileCasting = sourceCanCastWhileCasting;
        condition.canCastWhileChanneling = sourceCanCastWhileChanneling;

        coefficient.value = sourceValue;

        projectileFX.type = (ProjectileType)sourceProjectileType;
        projectileFX.size = (ProjectileSize)sourceProjectileSize;

        terminalCondition.hitCount = sourceHitCount;

        this.InitializeSkillValues();
    }

    public virtual IEnumerator Cooldown()
    {
        while (true)
        {
            if (!(condition.nowCharged < condition.maximumCharge))
            {
                condition.isCooldown = false;
                break;
            }
            for (
                float remainingCoolTime = condition.cooltime;
                remainingCoolTime >= 0;
                remainingCoolTime -= Time.fixedDeltaTime
            )
            {
                condition.cooltimeLeft = remainingCoolTime; //UI용 추가
                condition.isCooldown = true;
                yield return new WaitForFixedUpdate();
            }
            condition.cooltimeLeft = 0; //UI용 추가
            condition.nowCharged += 1;
        }
    }

    public virtual bool CanUse(AbstractAgent source, GameObject target)
    {
        if (target == null)
            return false;
        // if (info.targetType == TargetType.Target && target == null) return false;
        if (condition.nowCharged <= 0)
            return false;
        // 스킬을 쓰는 에이전트의 정면 각에 target 이 들어와 있는지 없는지.
        if (
            condition.range < Vector3.Distance(source.transform.position, target.transform.position)
        )
            return false;
        if (!condition.canCastWhileMoving && source.isMoving)
            return false;
        if (!condition.canCastWhileCasting && source.isCasting)
            return false;
        if (!condition.canCastWhileChanneling && source.isChanneling)
            return false;

        return true;
    }

    public virtual void ActivateInstant(GameObject source, GameObject target)
    {
        var skillQueue = source.GetComponent<AbstractAgent>()._skillQueue[info.name];
        var curObj = skillQueue.Dequeue();
        var projectileNew = curObj.GetComponent<Projectile>(); // TODO

        projectileNew.SetActors(source, target);
        projectileNew.SetSkill(this);

        curObj.transform.position = source.transform.position;
        curObj.SetActive(true);
    }

    public virtual void ShootProjectile(GameObject source, GameObject target)
    {
        var skillQueue = source.GetComponent<AbstractAgent>()._skillQueue[info.name];
        var curObj = skillQueue.Dequeue();
        var proj = curObj.GetComponent<Projectile>();

        proj.SetActors(source, target);
        proj.SetSkill(this);

        curObj.transform.position = source.transform.position;

        curObj.SetActive(true);
    }

    public virtual void ChargeCost(GameObject source)
    {
        source.GetComponent<AbstractAgent>()._status.mana.current -= condition.cost;
    }

    public virtual bool IsBackAttack(GameObject target, Projectile projectile)
    {
        bool backAttack = false;
        Vector3 projPos = projectile.transform.position;
        Vector3 targetPos = target.transform.position;
        Vector3 projDir = targetPos - projPos;
        Vector3 angleVector = target.transform.forward - projDir;
        float angle = Mathf.Atan2(angleVector.x, angleVector.z) * Mathf.Rad2Deg;
        if (angle > -60 && angle < 60)
            backAttack = true;
        return backAttack;
    }

    public virtual (bool, bool, int) CalculateEffectValue(
        GameObject source,
        GameObject target,
        HitType hitType,
        Projectile projectile
    )
    {
        AbstractStatus sourceStatus = source.GetComponent<AbstractAgent>()._status;
        float effectValue = 0.0f;
        float criticalChance = sourceStatus.attribute.secondary.criticalChance;
        float criticalMultiplier = sourceStatus.attribute.secondary.criticalMultiplier;
        float backAttackMultiplier = sourceStatus.attribute.secondary.backAttackMultiplier;

        bool isCritical = false;
        bool isBackAttack = false;

        if (hitType == HitType.Attack)
            effectValue = coefficient.value * sourceStatus.attack.power;
        else if (hitType == HitType.Spell)
            effectValue = coefficient.value * sourceStatus.spell.power;

        double dice = UnityEngine.Random.value;

        if (dice * 100 <= criticalChance)
        {
            isCritical = true;
            effectValue *= criticalMultiplier;
        }
        if (!(source.tag == "enemy") && (isBackAttack = IsBackAttack(target, projectile)))
        {
            effectValue *= backAttackMultiplier;
        }

        return (isCritical, isBackAttack, (int)effectValue);
    }

    public virtual void AffectOnAlly(GameObject source, GameObject target, Projectile projectile)
    {
        AbstractStatus sourceStatus = source.GetComponent<AbstractAgent>()._status;
        AbstractStatus targetStatus = target.GetComponent<AbstractAgent>()._status;

        bool isCritical,
            isBackAttack;
        int effectValue;
        (isCritical, isBackAttack, effectValue) = CalculateEffectValue(
            source,
            target,
            info.hitType,
            projectile
        );

        AbstractAgent _src = source.GetComponent<AbstractAgent>();
        AbstractAgent _trg = target.GetComponent<AbstractAgent>();
        CombatLog cl = new CombatLog();
        cl.hitTime = _src.GetGameController().GetEpisodeStep();
        cl.source = _src;
        cl.target = _trg;
        cl.skill = this;
        cl.value = effectValue;
        cl.isCritical = isCritical;
        cl.isBackAttack = isBackAttack;
        _src.GetGameController().OnSkillTrigLogReceived(cl);

        targetStatus.health.current = Mathf.Min(
            targetStatus.health.max,
            targetStatus.health.current + effectValue
        );
    }

    public virtual void AffectOnEnemy(
        GameObject source,
        GameObject target,
        Projectile projectile
    )
    {
        AbstractStatus sourceStatus = source.GetComponent<AbstractAgent>()._status;
        AbstractStatus targetStatus = target.GetComponent<AbstractAgent>()._status;

        bool isCritical,
            isBackAttack;
        int effectValue;
        (isCritical, isBackAttack, effectValue) = CalculateEffectValue(
            source,
            target,
            info.hitType,
            projectile
        );

        AbstractAgent _src = source.GetComponent<AbstractAgent>();
        AbstractAgent _trg = target.GetComponent<AbstractAgent>();
        CombatLog cl = new CombatLog();
        cl.hitTime = _src.GetGameController().GetEpisodeStep();
        cl.source = _src;
        cl.target = _trg;
        cl.skill = this;
        cl.value = effectValue;
        cl.isCritical = isCritical;
        cl.isBackAttack = isBackAttack;
        _src.GetGameController().OnCombatLogReceived(cl);

        targetStatus.health.current = Mathf.Max(0, targetStatus.health.current - effectValue);

        float hit_reward = (float)effectValue * 0.001f * 10;
        source.GetComponent<AbstractAgent>().AddReward(hit_reward);
    }

    public virtual bool IsTerminal()
    {
        if (terminalCondition.hitCount > 0)
            return false;
        return true;
    }

    public virtual bool OnHit(GameObject source, GameObject target, Projectile projectile)
    {
        bool hitByAlly = (source.tag == target.tag);

        if (hitByAlly && info.affectOnAlly)
        {
            AffectOnAlly(source, target, projectile);
            return true;
        }
        if (!hitByAlly && info.affectOnEnemy)
        {
            AffectOnEnemy(source, target, projectile);
            return true;
        }
        return false;
    }

    //
    // 스킬 활성화
    // 1. charge cost, 스킬 활성화가 실패하던 아니던 우선 cost 지불
    // 2. projectileSpeed에 따라 (0 -> instant cast on target) active instant 또는 projectile shoot
    //   2-1. skill projectile 생성 (또는 aura류의 스킬이라면 activate나 activate instant를 override해서 구현)
    //   2-2. 생성된 projectile이 피격되었을 때, Call OnHit
    //     2-2-1. 스킬의 종류에 따라 effectvalue 계산
    //     2-2-2. affect ally|enemy에 따라 health 가감
    // 3. skill의 current charge -1
    public virtual void Activate(GameObject source, GameObject target = null)
    {
        ChargeCost(source);

        switch (projectileFX.type)
        {
            case ProjectileType.Beam:
            {
                break;
            }
            case ProjectileType.Missile:
            {
                ShootProjectile(source, target);
                break;
            }
            case ProjectileType.PillarBlast:
            {
                break;
            }
            case ProjectileType.Slash:
            {
                ActivateInstant(source, target);
                break;
            }
        }

        if (condition.cooltime > 0.0f)
            condition.nowCharged -= 1;

        AbstractAgent _aa = source.GetComponent<AbstractAgent>();
        if (_aa)
        {
            // Append to the AgentLog class
            CombatLog cl = new CombatLog();
            cl.trigTime = _aa.GetGameController().GetEpisodeStep();
            cl.source = _aa;
            cl.skill = this;
            _aa.GetGameController().OnSkillTrigLogReceived(cl);
        }
    }
}
