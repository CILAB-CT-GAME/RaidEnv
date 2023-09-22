using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Health
{
    public int current;
    public int max;
    public float regen;
}

public struct Mana
{
    public int current;
    public int max;
    public float regen;
}
public struct Class
{
    public int num;
}
public struct Resource
{
    public int current;
    public int max;
    public int regen;
}

public struct Attribute
{
    public struct Primary
    {
        public int strength;
        public int agility;
        public int intelligence;
    }
    public struct Secondary
    {
        public int critical;
        public int haste;
        public int versatility;
        public int mastery;

        public int criticalChance;
        public int criticalMultiplier;
        public float backAttackMultiplier;
    }

    public Primary primary;
    public Secondary secondary;
}

public struct Attack
{
    public int power;
    public int range;
    public float speed;
}

public struct Spell
{
    public int power;
}

public struct Defensive
{
    public int armor;
    public int evasion;
}

public struct Etc
{
    public float moveSpeed;
}

[System.Serializable]
public class AbstractStatus
{
    public Class classnum;
    public Health health;
    public Mana mana;
    public Resource resource;
    public Attribute attribute;
    public Attack attack;
    public Spell spell;
    public Defensive defensive;
    public Etc etc;

    public virtual void Initialize()
    {

    }
}
