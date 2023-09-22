using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBlast : AbstractSkill
{
    public FireBlast()
    {
        info.name = "FireBlast";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Fire;
        info.hitType = HitType.Spell;
        info.targetType = TargetType.Target;
        info.projectileSpeed = 10;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.range = 40.0f;
        condition.cooltime = 10.0f;
        condition.casttime = 3.0f;
        condition.cost = 3;
        condition.nowCharged = 2;
        condition.maximumCharge = 2;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;

        coefficient.value = 1.0f;

        projectileFX.type = ProjectileType.PillarBlast;
        projectileFX.size = ProjectileSize.Tiny;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}