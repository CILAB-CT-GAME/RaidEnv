using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstShot : AbstractSkill
{
    public BurstShot()
    {
        info.name = "BurstShot";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Life;
        info.hitType = HitType.Attack;
        info.targetType = TargetType.Target;
        info.projectileSpeed = 20;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 60.0f;
        condition.cooltime = 10.0f;
        condition.casttime = 1.0f;
        condition.cost = 5;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = false;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 2.0f;

        projectileFX.type = ProjectileType.PillarBlast;
        projectileFX.size = ProjectileSize.Mega;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}