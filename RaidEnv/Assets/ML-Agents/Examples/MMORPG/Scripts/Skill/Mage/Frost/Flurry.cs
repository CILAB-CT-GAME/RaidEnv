using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flurry : AbstractSkill
{
    public Flurry()
    {
        info.name = "Flurry"; //진눈깨비
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Frost;
        info.hitType = HitType.Spell;
        info.targetType = TargetType.NonTarget;
        info.projectileSpeed = 10;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 10.0f;
        condition.cooltime = 1.0f;
        condition.casttime = 0f;
        condition.cost = 1;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;  
        condition.canCastWhileChanneling = false;

        coefficient.value = 0.3f;

        projectileFX.type = ProjectileType.Missile;
        projectileFX.size = ProjectileSize.Tiny;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}