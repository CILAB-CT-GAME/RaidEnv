using System.Collections.Generic;
using UnityEngine;

static class Common
{
    public static GameObject FindChildWithName(Transform parent, string _name)
    {
        Transform child = parent.Find(_name);
        if (child != null)
            return child.gameObject;
        return null;
    }

    public static List<GameObject> FindChildWithTag(Transform parent, string _tag)
    {
        List<GameObject> result = new List<GameObject>();
        for (int i = 0; i < parent.childCount; ++i)
        {
            var child = parent.GetChild(i);
            if (child.tag == _tag)
            {
                result.Add(child.gameObject);
            }
        }
        return (result.Count > 0) ? result : null;
    }
}