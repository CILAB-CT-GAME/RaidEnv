using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bang : AbstractSkill
{
    public Bang()
    {
        info.name = "Bang";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Fire;
        info.hitType = HitType.Attack;
        info.targetType = TargetType.Target;
        info.projectileSpeed = 30;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 10.0f;
        condition.cooltime = 0.5f;
        condition.casttime = 0.0f;
        condition.cost = 3;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 0.2f;

        projectileFX.type = ProjectileType.Missile;
        projectileFX.size = ProjectileSize.Tiny;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}