using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack_2 : AbstractSkill
{
    public BossAttack_2()
    {
        info.name = "BossAttack_2";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Fire;
        info.hitType = HitType.Spell;
        info.targetType = TargetType.NonTarget;
        info.projectileSpeed = 5;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 6.0f;
        condition.cooltime = 7.0f;
        condition.casttime = 4.0f;
        condition.cost = 20;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = false;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 1.2f;

        projectileFX.type = ProjectileType.Slash;
        projectileFX.size = ProjectileSize.Mega;

        terminalCondition.hitCount = 3;

        InitializeSkillValues();
    }
}