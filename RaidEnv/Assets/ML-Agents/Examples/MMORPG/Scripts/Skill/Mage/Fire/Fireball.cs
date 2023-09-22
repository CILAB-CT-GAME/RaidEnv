using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : AbstractSkill
{
    public Fireball()
    {
        info.name = "Fireball";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Fire;
        info.hitType = HitType.Spell;
        info.targetType = TargetType.NonTarget;
        info.projectileSpeed = 10;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 40.0f;
        condition.cooltime = 2.0f;
        condition.casttime = 1.0f;
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

        // base.Awake();
        InitializeSkillValues();
    }
}