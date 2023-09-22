using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class StatAgent : AbstractAgent
{
    List<float> statList = new List<float>(20);
    float auxiliary_input = Random.Range(-1, 1);

    public override void Initialize()
    {
        _status = new AbstractStatus();
        _status.classnum.num = Random.Range(0, 2);
        _status.health.current = 100;
        _status.health.max = 100;
        _status.health.regen = 1.0f;

        _status.mana.current = 100;
        _status.mana.max = 100;
        _status.mana.regen = 1.0f;

        _status.attribute.primary.strength = 100;
        _status.attribute.primary.agility = 100;
        _status.attribute.primary.intelligence = 100;

        _status.attribute.secondary.critical = 100;
        _status.attribute.secondary.haste = 100;
        _status.attribute.secondary.versatility = 100;
        _status.attribute.secondary.mastery = 100;

        _status.attack.power = 1;
        _status.attack.range = 5;
        _status.attack.speed = 1.5f;

        _status.spell.power = 100;

        _status.defensive.armor = 100;
        _status.defensive.evasion = 100;

        _status.etc.moveSpeed = 1.0f;
    }
 
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_status.classnum.num);
        sensor.AddObservation(_status.health.current);
        sensor.AddObservation(_status.health.max);
        sensor.AddObservation(_status.health.regen);
        sensor.AddObservation(_status.mana.current);
        sensor.AddObservation(_status.mana.max);
        sensor.AddObservation(_status.mana.regen);
        switch (_status.classnum.num)
        {
            case 0:
                sensor.AddObservation(_status.attribute.primary.intelligence);
                break;
            case 1:
                sensor.AddObservation(_status.attribute.primary.agility);
                break;
            case 2:
                sensor.AddObservation(_status.attribute.primary.strength);
                break;
        }
        sensor.AddObservation(_status.attribute.secondary.critical);
        sensor.AddObservation(_status.attribute.secondary.haste);
        sensor.AddObservation(_status.attribute.secondary.versatility);
        sensor.AddObservation(_status.attribute.secondary.mastery);
        sensor.AddObservation(_status.attack.power);
        sensor.AddObservation(_status.attack.range);
        sensor.AddObservation(_status.attack.speed);
        sensor.AddObservation(_status.spell.power);
        sensor.AddObservation(_status.defensive.armor);
        sensor.AddObservation(_status.defensive.evasion);
        sensor.AddObservation(_status.etc.moveSpeed);
        sensor.AddObservation(auxiliary_input);

    }


    //public void GenerateStat(ActionSegment<int> act)
    //{

    //    _status.health.current = act[0];
    //    _status.health.max = act[1];
    //    _status.health.regen = act[2];

    //    _status.mana.current = act[3];
    //    _status.mana.max = act[4];
    //    _status.mana.regen = act[5];
    //    switch (_status.classnum.num)
    //    {
    //        case 0:
    //            _status.primary.strength = act[6];
    //            break;
    //        case 1:
    //            _status.primary.dexterity = act[7];
    //            break;
    //        case 2:
    //            _status.primary.intelligence = act[8];
    //            break;
    //    }

    //    _status.secondary.critical = act[9];
    //    _status.secondary.haste = act[10];
    //    _status.secondary.versatility = act[11];
    //    _status.secondary.mastery = act[12];

    //    _status.attack.power = act[13];
    //    _status.attack.range = act[14];
    //    _status.attack.speed = act[15];

    //    _status.spell.power = act[16];

    //    _status.defensive.armor = act[17];
    //    _status.defensive.evasion = act[18];

    //    _status.etc.moveSpeed = act[19];
    //}


    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
      
        //GenerateStat(actionBuffers.ContinuousActions);
    }
}

