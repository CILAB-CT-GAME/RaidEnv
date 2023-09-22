using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicArmor : AbstractSkill
{
    public MagicArmor()
    {
        info.name = "MagicArmor";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Arcane;
        info.hitType = HitType.Spell;
        info.targetType = TargetType.Region;
        info.projectileSpeed = 10;
        info.affectOnAlly = true;
        info.affectOnEnemy = false;

        condition.range = 50.0f;
        condition.cooltime = 3.0f;
        condition.casttime = 1.0f;
        condition.cost = 3;
        condition.nowCharged = 1;
        condition.maximumCharge = 2;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 1.0f;

        projectileFX.type = ProjectileType.Missile;
        projectileFX.size = ProjectileSize.Mega;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}