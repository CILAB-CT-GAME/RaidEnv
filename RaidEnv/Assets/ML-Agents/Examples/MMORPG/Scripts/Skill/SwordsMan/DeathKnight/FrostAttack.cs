using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostAttack : AbstractSkill
{
    public FrostAttack()
    {
        info.name = "FrostAttack";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Frost;
        info.hitType = HitType.Attack;
        info.targetType = TargetType.Target;
        info.projectileSpeed = 10;
        info.affectOnAlly = true;
        info.affectOnEnemy = true;

        condition.range = 4.0f;
        condition.cooltime = 6.0f;
        condition.casttime = 0.1f;
        condition.cost = 10;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 1.0f;

        projectileFX.type = ProjectileType.Slash;
        projectileFX.size = ProjectileSize.Mega;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}