using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyLight : AbstractSkill
{
    public HolyLight()
    {
        info.name = "HolyLight";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Light;
        info.hitType = HitType.Spell;
        info.targetType = TargetType.Region;
        info.projectileSpeed = 10;
        info.affectOnAlly = true;
        info.affectOnEnemy = true;

        condition.range = 40.0f;
        condition.cooltime = 7.0f;
        condition.casttime = 1.0f;
        condition.cost = 5;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 1.0f;

        projectileFX.type = ProjectileType.PillarBlast;
        projectileFX.size = ProjectileSize.Normal;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}