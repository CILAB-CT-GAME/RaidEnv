using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanKnives : AbstractSkill
{
    public FanKnives()
    {
        info.name = "FanKnives";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Shadow;
        info.hitType = HitType.Attack;
        info.targetType = TargetType.Target;
        info.projectileSpeed = 20;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 10.0f;
        condition.cooltime = 1.0f;
        condition.casttime = 0.0f;
        condition.cost = 5;
        condition.nowCharged = 2;
        condition.maximumCharge = 2;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = true;
        condition.canCastWhileChanneling = true;

        coefficient.value = 0.2f;

        projectileFX.type = ProjectileType.Missile;
        projectileFX.size = ProjectileSize.Tiny;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}