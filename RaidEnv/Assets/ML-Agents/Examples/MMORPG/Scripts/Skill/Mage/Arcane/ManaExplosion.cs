using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaExplosion : AbstractSkill
{
    public ManaExplosion()
    {
        info.name = "ManaExplosion";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Arcane;
        info.hitType = HitType.Spell;
        info.targetType = TargetType.Target;
        info.projectileSpeed = 30;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 30.0f;
        condition.cooltime = 10.1f;
        condition.casttime = 3.0f;
        condition.cost = 1;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 2.0f;

        projectileFX.type = ProjectileType.PillarBlast;
        projectileFX.size = ProjectileSize.Normal;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}