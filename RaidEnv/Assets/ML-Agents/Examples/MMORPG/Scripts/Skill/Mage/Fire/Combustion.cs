using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combustion : AbstractSkill
{
    public class Effect : Aura
    {
        public int criticalIncrease = 100;
        public Effect()
        {
            duration = 10;
            isDurationStacked = false;
            isEffectStacked = false;
        }

        public override AbstractAura InitializeAura(GameObject target)
        {
            return new CombustionAura(this, target);
        }
    }
    public class CombustionAura : AbstractAura
    {
        public CombustionAura(Aura aura, GameObject target) : base(aura, target)
        {
            this.target = target;
        }

        protected override void ApplyEffect()
        {
            Effect effect = (Effect)aura;
            target.GetComponent<AbstractAgent>()._status.attribute.secondary.critical += effect.criticalIncrease;
        }

        public override void End()
        {
            Effect effect = (Effect)aura;
            target.GetComponent<AbstractAgent>()._status.attribute.secondary.critical -= effect.criticalIncrease * effectStacks;
            effectStacks = 0;
        }
    }

    public Combustion()
    {
        info.name = "Combustion";
        info.triggerType = TriggerType.Active;
        info.targetType = TargetType.Target;
        info.affectOnAlly = false;
        info.affectOnEnemy = false;

        condition.cooltime = 30.0f;
        condition.casttime = 0f;
        condition.cost = 1;
        condition.nowCharged = 1;
        condition.maximumCharge = 1;
        condition.canCastWhileMoving = true;
        condition.canCastWhileCasting = true;
        condition.canCastWhileChanneling = true;
    }

    // public override bool CanUse(AbstractAgent source, GameObject target)
    // {
    //     if (condition.nowCharged == 0) return false;
    //     if (!condition.canCastWhileMoving && source.isMoving) return false;
    //     if (!condition.canCastWhileCasting && source.isCasting) return false;
    //     if (!condition.canCastWhileChanneling && source.isChanneling) return false;

    //     return true;
    // }

    // public override void Activate(GameObject source, GameObject target)
    // {
    //     condition.nowCharged -= 1;

    //     target = source;
    //     Debug.Log("Target: " + target.name);
    //     CombustionAura combustionAura = new CombustionAura(new Effect(), target);
    //     target.GetComponent<AbstractAgent>().AddBuff(combustionAura);
    // }
}