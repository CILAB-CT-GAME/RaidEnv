using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class StatGenerator : MonoBehaviour
{
    
    public static StatGenerator _instance;
    public StatGenerator instance
    {
        get
        {
            if (_instance != null) return _instance;
            return _instance = new StatGenerator();
        }
    }
    [Header("Target")]
    public PCGTargetAgentType target;
    public int targetNumber = 0;

    [Header("Health")]
    public int h_current;
    public int h_max;
    public float h_regen;

    [Header("Mana")]
    public int m_current;
    public int m_max;
    public float m_regen;

    [Header("Primary")]
    public int strength;
    public int dexterity;
    public int intelligence;

    [Header("Secondary")]
    public int critical;
    public int haste;
    public int versatility;
    public int mastery;

    [Header("Attack")]
    public int power;
    public int range;
    public float speed;

    [Header("Spell")]
    public int s_power;

    [Header("Defensive")]
    public int armor;
    public int evasion;

    [Header("Etc")]
    public float moveSpeed;

    internal int GetRandomEnumComponent(System.Type type)
    {
        
        return Random.Range(0, System.Enum.GetValues(type).Length);
    }

    internal int GetRandomArrayComponent(int[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    internal float GetRandomArrayComponent(float[] array)
    {
        return array[Random.Range(0, array.Length)];
    }


    internal void SetValues()
    {
        
        // h_current=Random.Range(100,200);
        // h_max = 0;
        // while (h_max < h_current)
        // {
        //     h_max = Random.Range(101, 201);
        // }
        
        // h_regen = Random.Range(0.0f, 1.0f);

        // m_current = Random.Range(100, 200);
        // m_max = 0;
        // while (m_max < m_current)
        // {
        //     m_max = Random.Range(101, 201);
        // }

        // m_regen = Random.Range(0.0f, 1.0f);

        // strength = Random.Range(100, 200);
        // dexterity = Random.Range(100, 200);
        // intelligence = Random.Range(100, 200);

        // critical = Random.Range(100, 200); 
        // haste= Random.Range(100, 200);
        // versatility= Random.Range(100, 200);
        // mastery= Random.Range(100, 200);

        // power= Random.Range(1, 10);
        // range = Random.Range(1, 10);
        // speed = Random.Range(1.0f,5.0f); 

        // s_power = Random.Range(100, 200);

        // armor = Random.Range(100, 200);
        // evasion = Random.Range(100, 200);

        // speed =  Random.Range(1.0f, 5.0f);
    }

    public void Generate()
    {
        // SetValues();
        switch (target){
            case PCGTargetAgentType.Agent:
                GameObject.Find("RaidPlayer").GetComponent<RaidPlayerAgent>()._status.health.current = h_max;
            break;
            case PCGTargetAgentType.Enemy:
                GameObject.Find("Enemy").GetComponent<EnemyAgent>()._status.health.current = h_max;
            break;
            case PCGTargetAgentType.All:
                GameObject.Find("Enemy").GetComponent<EnemyAgent>()._status.health.current = h_max;
                GameObject.Find("RaidPlayer").GetComponent<RaidPlayerAgent>()._status.health.current = h_max;
                GameObject.Find("RaidPlayer (1)").GetComponent<RaidPlayerAgent>()._status.health.current = h_max;
                GameObject.Find("RaidPlayer (2)").GetComponent<RaidPlayerAgent>()._status.health.current = h_max;

            break;
        }

    }
}