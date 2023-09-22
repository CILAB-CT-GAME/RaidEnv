using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack_1 : AbstractSkill
{
    public BossAttack_1()
    {
        info.name = "BossAttack_1";
        info.triggerType = TriggerType.Active;
        info.magicSchool = MagicSchool.Arcane;
        info.hitType = HitType.Spell;
        info.targetType = TargetType.NonTarget;
        info.projectileSpeed = 20;
        info.affectOnAlly = false;
        info.affectOnEnemy = true;

        condition.cooltime = 0.5f;
        condition.casttime = 0.1f;
        condition.cost = 0;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileCasting = false;
        condition.canCastWhileChanneling = false;
        condition.range = 12.0f;


        coefficient.value = 1.1f;

        projectileFX.type = ProjectileType.Missile;
        projectileFX.size = ProjectileSize.Tiny;

        terminalCondition.hitCount = 1;

        InitializeSkillValues();
    }
}



// public class BossAttack_1 : AbstractSkill
// {
//     public BossAttack_1()
//     {
//         info.name = "BossAttack_1";
//         info.triggerType = TriggerType.Active;
//         info.magicSchool = MagicSchool.Arcane;
//         info.hitType = HitType.Spell;
//         info.targetType = TargetType.NonTarget;
//         info.projectileSpeed = 50;
//         info.affectOnAlly = false;
//         info.affectOnEnemy = true;

//         condition.cooltime = 0.5f;
//         condition.casttime = 0.2f;
//         condition.cost = 0;
//         condition.nowCharged = 1;
//         condition.maximumCharge = 1;
//         condition.canCastWhileCasting = false;
//         condition.canCastWhileChanneling = false;
//         condition.range = 12.0f;


//         coefficient.value = 0.6f;

//         projectileFX.type = ProjectileType.Missile;
//         projectileFX.size = ProjectileSize.Tiny;

//         terminalCondition.hitCount = 1;

//         InitializeSkillValues();
//     }
// }
