using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinShot : AbstractSkill
{
    public KinShot()
    {
        info.name = "KinShot";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Life;
        info.hitType = HitType.Attack;
        info.targetType = TargetType.Target;
        info.projectileSpeed = 20;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 50.0f;
        condition.cooltime = 1.0f;
        condition.casttime = 0f;
        condition.cost = 1;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 0.5f;

        projectileFX.type = ProjectileType.Missile;
        projectileFX.size = ProjectileSize.Tiny;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}