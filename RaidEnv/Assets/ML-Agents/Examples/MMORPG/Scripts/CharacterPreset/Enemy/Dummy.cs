using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyStatus : AbstractStatus
{
    public override void Initialize()
    {
        health.current = 1000;
        health.max = 1000;

        mana.current = 0;
        mana.max = 0;

        attack.power = 0;
        attack.range = 0;
        attack.speed = 0f;
    }
}

public class Dummy : AbstractClass
{
    public Dummy()
    {
        classInfo.className = "Dummy";

        Initialize();
    }

    public override void InitializeSkill()
    {
        _skillList = new List<AbstractSkill>();
        // _skillList.Add(new AutoAttack(attack));
        // _skillList.Add(new Fireball());
    }
}
