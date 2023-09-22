using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyroblast : AbstractSkill
{
    public Pyroblast()
    {
        info.name = "Pyroblast";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Fire;
        info.hitType = HitType.Spell;
        info.targetType = TargetType.Target;
        info.projectileSpeed = 10;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 40.0f;
        condition.cooltime = 15.0f;
        condition.casttime = 4.5f;
        condition.cost = 5;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 1.5f;

        projectileFX.type = ProjectileType.Missile;
        projectileFX.size = ProjectileSize.Normal;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}