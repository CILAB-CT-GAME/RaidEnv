using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowstorm : AbstractSkill
{
    public Snowstorm()
    {
        info.name = "Snowstorm";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Frost;
        info.hitType = HitType.Spell;
        info.targetType = TargetType.Target;
        info.projectileSpeed = 30;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 30.0f;
        condition.cooltime = 10.0f;
        condition.casttime = 3.0f;
        condition.cost = 10;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = false;
        condition.canCastWhileCasting = false;  
        condition.canCastWhileChanneling = false;

        coefficient.value = 2.0f;

        projectileFX.type = ProjectileType.PillarBlast;
        projectileFX.size = ProjectileSize.Mega;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}