using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackPowder : AbstractSkill
{
    public BlackPowder()
    {
        info.name = "BlackPowder";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Shadow;
        info.hitType = HitType.Attack;
        info.targetType = TargetType.Target;
        info.projectileSpeed = 10;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 5.0f;
        condition.cooltime = 5.0f;
        condition.casttime = 0.0f;
        condition.cost = 5;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 1.0f;

        projectileFX.type = ProjectileType.PillarBlast;
        projectileFX.size = ProjectileSize.Mega;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}