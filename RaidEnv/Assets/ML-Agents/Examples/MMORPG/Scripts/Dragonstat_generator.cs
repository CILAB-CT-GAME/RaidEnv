using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Dragonstat_generator : MonoBehaviour
{
    // Start is called before the first frame update
    public static SkillGenerator _instance;
    public SkillGenerator instance
    {
        get
        {
            if (_instance != null) return _instance;
            return _instance = new SkillGenerator();
        }
    }

}
