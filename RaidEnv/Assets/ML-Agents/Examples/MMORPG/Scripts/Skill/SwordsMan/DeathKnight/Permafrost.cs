using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Permafrost : AbstractSkill
{
    public Permafrost()
    {
        info.name = "Permafrost";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Frost;
        info.hitType = HitType.Spell;
        info.targetType = TargetType.Target;
        info.projectileSpeed = 10;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 6.0f;
        condition.cooltime = 6.0f;
        condition.casttime = 1.0f;
        condition.cost = 20;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 1.5f;

        projectileFX.type = ProjectileType.PillarBlast;
        projectileFX.size = ProjectileSize.Mega;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}