// reference: https://www.jonathanyu.xyz/2016/12/30/buff-system-with-scriptable-objects-for-unity/

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.MLAgents;

public abstract class Aura : ScriptableObject
{
    public float duration;
    public bool isDurationStacked;
    public bool isEffectStacked;
    public abstract AbstractAura InitializeAura(GameObject target);
}

public abstract class AbstractAura
{
    protected float duration;
    protected int effectStacks;
    public Aura aura { get; }
    protected GameObject target;
    public bool isFinished;

    public AbstractAura(Aura aura, GameObject target)
    {
        this.aura = aura;
        this.target = target;
    }

    public void Tick(float delta)
    {
        duration -= delta;
        if (duration <= 0)
        {
            End();
            isFinished = true;
        }
    }

    public void Activate()
    {
        if (aura.isEffectStacked || duration <= 0)
        {
            ApplyEffect();
            effectStacks++;
        }

        if (aura.isDurationStacked || duration <= 0)
        {
            duration += aura.duration;
        }
    }
    protected abstract void ApplyEffect();
    public abstract void End();
}