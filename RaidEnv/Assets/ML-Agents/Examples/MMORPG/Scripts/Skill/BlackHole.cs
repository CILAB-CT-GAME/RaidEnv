using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : AbstractSkill
{
    public BlackHole()
    {
        info.name = "BlackHole";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Fire;
        info.hitType = HitType.Spell;
        info.targetType = TargetType.NonTarget;
        info.projectileSpeed = 0;
        info.affectOnAlly = false;
        info.affectOnEnemy = false;

        condition.range = 0.0f;
        condition.cooltime = 99999.0f;
        condition.casttime = 0.0f;
        condition.cost = 0;
        condition.nowCharged = 0;
        condition.maximumCharge = 0;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;  
        condition.canCastWhileChanneling = false;

        coefficient.value = 0.0f;

        projectileFX.type = ProjectileType.Missile;
        projectileFX.size = ProjectileSize.Tiny;

        terminalCondition.hitCount = 0;

        // base.Awake();
        InitializeSkillValues();
    }
}