using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsManStatus : AbstractStatus
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

        etc.moveSpeed = 1.5f;
    }
}

public class SwordsManClass : AbstractClass
{
    public enum Specialization { Warrior, Paladin, DeathKnight }
    public Specialization spec = Specialization.Warrior;

    public SwordsManClass() : base()
    {
        classInfo.className = "SwordsMan";
        classInfo.role = Role.DPS;
        classInfo.primaryType = PrimaryType.Strength;

        Initialize();
    }

    public override void InitializeSkill()
    {
        _skillList = new List<AbstractSkill>();
        _skillList.Add(new AutoAttack());

        switch (spec)
        {
            case Specialization.Warrior:
                _skillList.Add(new FurySlash());
                _skillList.Add(new FuryRoar());
                _skillList.Add(new FinalAttack());
                break;
            case Specialization.Paladin:
                _skillList.Add(new HolySlash());
                _skillList.Add(new HolyLight());
                _skillList.Add(new DivineImpact());
                break;
            case Specialization.DeathKnight:
                _skillList.Add(new Obliterate());
                _skillList.Add(new FrostAttack());
                _skillList.Add(new Permafrost());
                break;
        }
    }
}
