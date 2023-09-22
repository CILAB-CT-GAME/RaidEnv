using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurySlash : AbstractSkill
{
    public FurySlash()
    {
        info.name = "FurySlash";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Fire;
        info.hitType = HitType.Attack;
        info.targetType = TargetType.NonTarget;
        info.projectileSpeed = 10;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 3.0f;
        condition.cooltime = 0.5f;
        condition.casttime = 0.0f;
        condition.cost = 1;
        condition.nowCharged = 2;
        condition.maximumCharge = 2;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 0.7f;

        projectileFX.type = ProjectileType.Slash;
        projectileFX.size = ProjectileSize.Normal;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}