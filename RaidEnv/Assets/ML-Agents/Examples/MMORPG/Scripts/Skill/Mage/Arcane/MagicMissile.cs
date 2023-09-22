using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissile : AbstractSkill
{
    public MagicMissile()
    {
        info.name = "MagicMissile";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Arcane;
        info.hitType = HitType.Spell;
        info.targetType = TargetType.Target;
        info.projectileSpeed = 15;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 50.0f;
        condition.cooltime = 1.0f;
        condition.casttime = 0.5f;
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