using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMap
{
    public Dictionary<string, KeyCode> keySettings { get; }

    public KeyMap()
    {
        keySettings  = new Dictionary<string, KeyCode>();
        keySettings.Add("MOVE_UP",      KeyCode.W);
        keySettings.Add("MOVE_DOWN",    KeyCode.S);
        keySettings.Add("MOVE_LEFT",    KeyCode.Q);
        keySettings.Add("MOVE_RIGHT",   KeyCode.E);
        keySettings.Add("TURN_LEFT",    KeyCode.A);
        keySettings.Add("TURN_RIGHT",   KeyCode.D);
        
        keySettings.Add("SKILL_1",      KeyCode.Alpha1);
        keySettings.Add("SKILL_2",      KeyCode.Alpha2);
        keySettings.Add("SKILL_3",      KeyCode.Alpha3);
        keySettings.Add("SKILL_4",      KeyCode.Alpha4);
        keySettings.Add("SKILL_5",      KeyCode.Alpha5);
        
        keySettings.Add("ITEM_1",       KeyCode.Alpha9);
        keySettings.Add("ITEM_2",       KeyCode.Alpha0);

        keySettings.Add("PING_HELP",    KeyCode.G);

        keySettings.Add("SET_TARGET",   KeyCode.Tab);
        keySettings.Add("RESET",        KeyCode.R);
        keySettings.Add("KILL",         KeyCode.K);
    }
}
