using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalAttack : AbstractSkill
{
    public FinalAttack()
    {
        info.name = "FinalAttack";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Fire;
        info.hitType = HitType.Attack;
        info.targetType = TargetType.NonTarget;
        info.projectileSpeed = 10;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 3.0f;
        condition.cooltime = 10.0f;
        condition.casttime = 0.5f;
        condition.cost = 20;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 2.0f;

        projectileFX.type = ProjectileType.Slash;
        projectileFX.size = ProjectileSize.Mega;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}