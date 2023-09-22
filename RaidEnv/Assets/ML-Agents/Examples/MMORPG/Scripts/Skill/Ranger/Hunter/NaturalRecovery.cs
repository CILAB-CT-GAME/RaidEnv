using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturalRecovery : AbstractSkill
{
    public NaturalRecovery()
    {
        info.name = "NaturalRecovery";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Life;
        info.hitType = HitType.Spell;
        info.targetType = TargetType.Region;
        info.projectileSpeed = 10;
        info.affectOnAlly = true;
        info.affectOnEnemy = false;

        condition.range = 50.0f;
        condition.cooltime = 5.0f;
        condition.casttime = 1.0f;
        condition.cost = 3;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 0.5f;

        projectileFX.type = ProjectileType.Missile;
        projectileFX.size = ProjectileSize.Normal;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}