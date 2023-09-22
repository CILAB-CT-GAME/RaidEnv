using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Role { Tanker, DPS, Healer }
public enum PrimaryType { Strength, Dexterity, Intelligence }
public enum ClassSpecialization { Mage, Ranger, SwordsMan }

public struct ClassInfo
{
    public string className;
    public Role role;
    public PrimaryType primaryType;
}

public abstract class AbstractClass
{
    public ClassInfo classInfo;
    public Sprite portrait;
    protected AbstractStatus _status;
    public AbstractStatus status
    {
        get { return _status; }
        set { _status = value; }
    }
    public Hashtable config {get; set;}
    protected List<AbstractSkill> _skillList;
    public List<AbstractSkill> skillList
    {
        get { return _skillList; }
        set { _skillList = value; }
    }
    public virtual void Initialize()
    {
        InitializeSkill();
    }
    public abstract void InitializeSkill();
}
