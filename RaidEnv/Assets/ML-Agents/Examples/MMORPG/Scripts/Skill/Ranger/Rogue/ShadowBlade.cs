using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBlade : AbstractSkill
{
    public ShadowBlade()
    {
        info.name = "ShadowBlade";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Shadow;
        info.hitType = HitType.Attack;
        info.targetType = TargetType.Target;
        info.projectileSpeed = 10;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 5.0f;
        condition.cooltime = 3.0f;
        condition.casttime = 0.0f;
        condition.cost = 5;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 0.9f;

        projectileFX.type = ProjectileType.Slash;
        projectileFX.size = ProjectileSize.Tiny;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}