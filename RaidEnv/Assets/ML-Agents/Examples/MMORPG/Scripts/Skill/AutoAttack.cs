using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;

public class AutoAttack : AbstractSkill
{
    public AutoAttack()
    {
        info.name = "Auto Attack";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Arcane;
        info.hitType = HitType.Attack;
        info.targetType = TargetType.NonTarget;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 5.0f;
        condition.cooltime = 1.0f;
        condition.casttime = 0;
        condition.cost = 0;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;  
        condition.canCastWhileChanneling = false;

        coefficient.value = 1.00f;

        projectileFX.type = ProjectileType.Slash;
        projectileFX.size = ProjectileSize.Mega;

        terminalCondition.hitCount = 1;

        base.InitializeSkillValues();
    }
}