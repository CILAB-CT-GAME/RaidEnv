using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolySlash : AbstractSkill
{
    public HolySlash()
    {
        info.name = "HolySlash";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Light;
        info.hitType = HitType.Attack;
        info.targetType = TargetType.Target;
        info.projectileSpeed = 10;
        info.affectOnAlly = true;
        info.affectOnEnemy = true;

        condition.range = 30.0f;
        condition.cooltime = 1.0f;
        condition.casttime = 0.0f;
        condition.cost = 1;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 0.5f;

        projectileFX.type = ProjectileType.Slash;
        projectileFX.size = ProjectileSize.Tiny;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}