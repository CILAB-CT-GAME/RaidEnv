using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snipe : AbstractSkill
{
    public Snipe()
    {
        info.name = "Snipe";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Fire;
        info.hitType = HitType.Attack;
        info.targetType = TargetType.Target;
        info.projectileSpeed = 60;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 20.0f;
        condition.cooltime = 10.0f;
        condition.casttime = 2.0f;
        condition.cost = 10;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = false;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 1.0f;

        projectileFX.type = ProjectileType.Missile;
        projectileFX.size = ProjectileSize.Mega;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}