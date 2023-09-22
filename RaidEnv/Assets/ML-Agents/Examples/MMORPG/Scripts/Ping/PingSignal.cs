using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PingSignalType
{
    Help = 1
}

[System.Serializable]
public class PingSignal : MonoBehaviour
{
    public int RemainedStep = 20;

    public AbstractAgent Owner = null;

    public PingSignalType Type = PingSignalType.Help;

    void Start()
    {
    }

    void Update()
    {
    }


    public void OnDestroy()
    {
        Destroy(this.gameObject);
    }


}
