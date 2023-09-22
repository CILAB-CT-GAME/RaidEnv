using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonStatus : AbstractStatus
{
    public override void Initialize()
    {
        health.current = 1000;
        health.max = 1000;

        mana.current = 100;
        mana.max = 100;

        attack.power = 1;
        attack.range = 5;
        attack.speed = 1.5f;

        etc.moveSpeed = 6;

        spell.power = 50; // temp for enemy's skills
    }
}

public class Dragon : AbstractClass
{
    public Dragon()
    {
        classInfo.className = "Dragon";

        Initialize();
    }

    public override void InitializeSkill()
    {
        // _skillList = new List<AbstractSkill>(); 스킬제너레이터에서 초기화 중
        // _skillList.Add(new AutoAttack());
        // _skillList.Add(new BossAttack_1());
        // _skillList.Add(new BossAttack_2()); 
    }
}
