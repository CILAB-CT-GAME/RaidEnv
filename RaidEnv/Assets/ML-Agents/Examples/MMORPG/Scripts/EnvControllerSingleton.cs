using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class EnvControllerSingleton : MonoBehaviour
{
    // Start is called before the first frame update

    public List<MMORPGEnvController> PlatformList;
    int frameCount = 0;
    void Start()
    {
        PlatformList = new List<MMORPGEnvController>();
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("platform");

        foreach (GameObject obj in platforms)
        {
            PlatformList.Add(obj.GetComponent<MMORPGEnvController>());
        }   

    }

    void FixedUpdate()
    {
        frameCount += 1;

        if (frameCount % 5 == 0)
        {
            Academy.Instance.EnvironmentStep();
        }
    }    
}
