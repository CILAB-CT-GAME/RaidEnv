using UnityEditor;
using UnityEngine;

#if UnityEditor
[CustomEditor(typeof(ItemGenerator))]
public class ItemInspectator : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ItemGenerator item = (ItemGenerator)target;
        
        

        if(GUILayout.Button("Generate"))
        {
            item.GenerateItem(-1, (int)item.type, (int)item.effectedStatus, item.grants, item.coolDown, item.amount);
        }
        if(GUILayout.Button("Check this item already generated"))
        {
            Debug.Log(item.IsThisItemGeneratedEarlier((int)item.type, (int)item.effectedStatus, item.grants, item.coolDown, item.amount));
            
        }
    }
}
[CustomEditor(typeof(SkillGenerator))]
internal class SkillInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SkillGenerator skill = (SkillGenerator)target;
        if (GUILayout.Button("Generate"))
        {
            skill.GenerateManually();
        }
    }
}
[CustomEditor(typeof(StatGenerator))]
    internal class StatInspector : Editor
    {
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            StatGenerator stat = (StatGenerator)target;
            if (GUILayout.Button("Generate"))
            {
                stat.Generate();
            }
        }
    }
#endif