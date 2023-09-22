using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceLance : AbstractSkill
{
    public IceLance()
    {
        info.name = "IceLance";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Frost;
        info.hitType = HitType.Spell;
        info.targetType = TargetType.Target;
        info.projectileSpeed = 20;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 50.0f;
        condition.cooltime = 1.0f;
        condition.casttime = 1.0f;
        condition.cost = 3;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;  
        condition.canCastWhileChanneling = false;

        coefficient.value = 0.8f;

        projectileFX.type = ProjectileType.Missile;
        projectileFX.size = ProjectileSize.Normal;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}