using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerStatus : AbstractStatus
{
    public override void Initialize()
    {
        health.current = 100;
        health.max = 100;
        health.regen = 1.0f;

        mana.current = 100;
        mana.max = 100;
        mana.regen = 1.0f;

        attribute.primary.intelligence = 100;

        attribute.secondary.critical = 100;
        attribute.secondary.haste = 100;
        attribute.secondary.versatility = 100;
        attribute.secondary.mastery = 100;

        attack.power = 1;
        attack.range = 5;
        attack.speed = 1.5f;

        spell.power = 100;

        defensive.armor = 100;
        defensive.evasion = 100;

        etc.moveSpeed = 2.0f;
    }
}

public class RangerClass : AbstractClass
{
    public enum Specialization { Hunter, Rogue, Gunner }
    public Specialization spec = Specialization.Rogue;

    public RangerClass() : base()
    {
        classInfo.className = "Ranger";
        classInfo.role = Role.DPS;
        classInfo.primaryType = PrimaryType.Dexterity;

        Initialize();
    }

    public override void InitializeSkill()
    {
        _skillList = new List<AbstractSkill>();
        _skillList.Add(new AutoAttack());

        switch (spec)
        {
            case Specialization.Hunter:
                _skillList.Add(new KinShot());
                _skillList.Add(new NaturalRecovery());
                _skillList.Add(new BurstShot());
                break;
            case Specialization.Rogue:
                _skillList.Add(new FanKnives());
                _skillList.Add(new ShadowBlade());
                _skillList.Add(new BlackPowder());
                break;
            case Specialization.Gunner:
                _skillList.Add(new Bang());
                _skillList.Add(new Bomb());
                _skillList.Add(new Snipe());
                break;
        }
    }
}
