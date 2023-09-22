using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obliterate : AbstractSkill
{
    public Obliterate()
    {
        info.name = "Obliterate";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Frost;
        info.hitType = HitType.Attack;
        info.targetType = TargetType.Target;
        info.projectileSpeed = 10;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 4.0f;
        condition.cooltime = 1.0f;
        condition.casttime = 0.1f;
        condition.cost = 1;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 0.7f;

        projectileFX.type = ProjectileType.Slash;
        projectileFX.size = ProjectileSize.Normal;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}