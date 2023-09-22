using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class MageStatus : AbstractStatus
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
        attribute.secondary.backAttackMultiplier = 1.2f;
        attribute.secondary.haste = 100;
        attribute.secondary.versatility = 100;
        attribute.secondary.mastery = 100;

        attack.power = 1;
        attack.range = 5;
        attack.speed = 1.5f;

        spell.power = 100;

        defensive.armor = 100;
        defensive.evasion = 100;

        etc.moveSpeed = 1.0f;
    }
}

public class MageClass : AbstractClass
{
    public enum Specialization { Arcane, Fire, Frost }
    public Specialization spec = Specialization.Fire;

    public MageClass() : base()
    {
        classInfo.className = "Mage";
        classInfo.role = Role.DPS;
        classInfo.primaryType = PrimaryType.Intelligence;

        Initialize();
    }

    public override void InitializeSkill()
    {
        _skillList = new List<AbstractSkill>();

        if (this.config != null) AddSkillsByCfg();
        else
        {
            switch (spec)
            {
                case Specialization.Arcane:
                    _skillList.Add(new MagicMissile());
                    _skillList.Add(new MagicArmor());
                    _skillList.Add(new ManaExplosion());
                    break;
                case Specialization.Fire:
                    _skillList.Add(new Fireball());
                    _skillList.Add(new FireBlast());
                    _skillList.Add(new Pyroblast());
                    break;
                case Specialization.Frost:
                    _skillList.Add(new Flurry());
                    _skillList.Add(new IceLance());
                    _skillList.Add(new Snowstorm());
                    break;
            }
        }

    }

    public void AddSkillsByCfg()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        System.Type skill1 = assembly.GetType((string)this.config["skill1"]);
        object AddSkill1 = System.Activator.CreateInstance(skill1);
        System.Type skill2 = assembly.GetType((string)this.config["skill2"]);
        object AddSkill2 = System.Activator.CreateInstance(skill2);
        System.Type skill3 = assembly.GetType((string)this.config["skill3"]);
        object AddSkill3 = System.Activator.CreateInstance(skill3);

        _skillList.Add((AbstractSkill)AddSkill1);
        _skillList.Add((AbstractSkill)AddSkill2);
        _skillList.Add((AbstractSkill)AddSkill3);
    }
}