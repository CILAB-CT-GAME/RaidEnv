using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuryRoar : AbstractSkill
{
    public FuryRoar()
    {
        info.name = "FuryRoar";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Fire;
        info.hitType = HitType.Spell;
        info.targetType = TargetType.Region;
        info.projectileSpeed = 0;
        info.affectOnAlly = true;
        info.affectOnEnemy = true;

        condition.range = 5.0f;
        condition.cooltime = 3.0f;
        condition.casttime = 0.0f;
        condition.cost = 5;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 0.5f;

        projectileFX.type = ProjectileType.Missile;
        projectileFX.size = ProjectileSize.Mega;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}