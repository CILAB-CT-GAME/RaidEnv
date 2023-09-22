using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEditor;

public class RangeOnlyPCGAgent : PCGAgent
{
    public RangeOnlyPCGAgent() : base()
    {
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
    }

    private float? previousWinrate = null;

    private void FixedUpdate()
    {
        if (UseManualMaxStep && ManualCurrentStep > ManualMaxStep)
        {
            EndEpisode();
        }

        if(logmanager.GetEpisodeLogCount() >= simulationLimit)
        {
            RequestDecision();
            Academy.Instance.EnvironmentStep();
            ManualCurrentStep += 1;
        } 

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        SkillGenerator.MinMaxThreshold thresHold = new SkillGenerator.MinMaxThreshold();
        
        List<float> skill = _lastGeneratedSkill;

        // Normalize each value with its min and max threshold
        sensor.AddObservation(GetMinMaxScaledValue(skill[7], thresHold.range[0], thresHold.range[1]));
        // sensor.AddObservation(GetMinMaxScaledValue(skill[8], thresHold.cooltime[0], thresHold.cooltime[1]));
        // sensor.AddObservation(GetMinMaxScaledValue(skill[9], thresHold.casttime[0], thresHold.casttime[1]));
        // sensor.AddObservation(GetMinMaxScaledValue(skill[15], thresHold.value[0], thresHold.value[1]));
    }

    public override void OnActionReceived(ActionBuffers _actionBuffers)
    {
        ActionBuffers actionBuffers = _actionBuffers;

        if (RunHeuristic || RandomAgent)
        {
            actionBuffers = new ActionBuffers(
                new ActionSegment<float>(new float[_actionBuffers.ContinuousActions.Length]), 
                new ActionSegment<int>(new int[_actionBuffers.DiscreteActions.Length]));
            Heuristic(in actionBuffers);
        }

        float winRate = logmanager.GetWinRate();

            
        float _winRateReward;
        if(previousWinrate != null) {
            _winRateReward = CalculateReward(winRate, (float)previousWinrate, (float)RewardTargetValues["pcg_win_rate"]) * 100;
        } else {
            _winRateReward = 0.0f;
        }

        // End of the reward calculation
        float _shapedReward = _winRateReward * ((float)RewardWeights["pcg_win_rate"]);
  
        AddReward(_shapedReward);

        // Record the generated skill and the game results
        if(SaveCreatedSkill && _lastGeneratedSkill != null && StepCount > 0) 
        {
            // Make a simulation result dictionary
            Hashtable simResult = new Hashtable();
            simResult.Add("winRate", winRate);

            RecordContentAndResult(_lastGeneratedSkill, simResult);
        }

        if (!float.IsNaN(winRate))
        {
            previousWinrate = winRate;
        }
        Academy.Instance.StatsRecorder.Add("PCG/WinRate", winRate);
        Academy.Instance.StatsRecorder.Add("PCG/SimulationEpisode", (int)logmanager.GetEpisodeLogCount());
        Academy.Instance.StatsRecorder.Add("PCG/TargetWinRate", ((float)RewardTargetValues["pcg_win_rate"]));
        Academy.Instance.StatsRecorder.Add("PCG/EpisodeNumber", ((float)CompletedEpisodes));


        // WinRateError with absolute 
        float winRateError = Mathf.Abs(((float)RewardTargetValues["pcg_win_rate"]) - winRate);
        Academy.Instance.StatsRecorder.Add("PCG/WinRateError", winRateError);

        // Get time from previous time
        float time = Time.time - _previousTime;
        Academy.Instance.StatsRecorder.Add("PCG/SimulationTime", time);
        _previousTime = Time.time;
        
        GenerateRangeSkill(actionBuffers.DiscreteActions);
        

        if(PrintDebug == true)  {
            // Print the 8, 9, and 15 value in the generated skill
            // 8 is cooltime, 9 is casttime, and 15 is value
            // Print it with label
            Debug.Log("WinRate: " + winRate + " WinRateReward: " + _winRateReward + " ShapedReward: " + _shapedReward + " WinRateError: " + winRateError + " Time: " + time + "");
            Debug.Log("Cooltime: " + _lastGeneratedSkill[8] + " Casttime: " + _lastGeneratedSkill[9] + " Value: " + _lastGeneratedSkill[15]);
            Debug.Log("ShapeReward: " + _shapedReward);
        }

        foreach(MMORPGEnvController env in AllEnvs)
        {
            env.BaddieTouchedBlock();
        }
        logmanager.Reset();

        // Check if the win rate is in the target range
        float _winRateLowerBound = ((float)RewardTargetValues["pcg_win_rate"]) - 0.02f;
        float _winRateUpperBound = ((float)RewardTargetValues["pcg_win_rate"]) + 0.02f;
        if(!StrictEpisodeLength && (_winRateLowerBound <= winRate && winRate <= _winRateUpperBound))
        {
            EndEpisode();
        }

        
    }

    public void GenerateRangeSkill(ActionSegment<int> _act)
    {

        // _lastGeneratedSkill = overallGenerator.GetSkillParameter(_target, _targetAgentNumber, _targetSkillNumber);

        // (0, 0, 0, 0)
        // (-1, -1, -1, -1) = 아무값도 안 바꾸는거
        // (-1, -1, 0 ~ 1, -1)
        // (-1, -1, 0.4, -1) --> 값 -
        // (-1, -1, 0.9, -1) --> 값 +
        // 0 ~ 0.4 사이값 = 내리는거
        // 0.6 ~ 1.0 사이값 = 값 올리는거

        // Index Description
        // 7: range
        // 8: casttime
        // 9: cool-time
        // 15: value

        // Convert action to float
        float[] act = new float[_act.Length];
        for(int i = 0; i < _act.Length; i++)
        {
            switch(_act[i])
            {
                case 0:
                    act[i] = -1.0f;
                    break;
                case 1:
                    act[i] = -0.5f;
                    break;
                case 2:
                    act[i] = 0;
                    break;
                case 3:
                    act[i] = +0.5f;
                    break;
                case 4:
                    act[i] = +1.0f;
                    break;
            
            }
        }

        SkillGenerator.MinMaxThreshold thresHold = new SkillGenerator.MinMaxThreshold();

        if(changeParameterDirectly == false)
        {
            float tickScale = 6;

            // Get range value from MineMaxThreshold in SkillGenerator
            float rangeTick = (thresHold.range[1] - thresHold.range[0]) / tickScale;

            _lastGeneratedSkill[7] += rangeTick * act[0];
            _lastGeneratedSkill[7] = ActionLimiting(_lastGeneratedSkill[7], thresHold.range[0], thresHold.range[1]);
        }
        else
        {
            _lastGeneratedSkill[7] = ActionClipping(act[0], thresHold.range[0], thresHold.range[1]);
        }

        overallGenerator.GenerateSomethingWithParameter(_target,_type,_targetAgentNumber, _lastGeneratedSkill);

        // Apply range to proxy
        if (UseMAProxy)
        {
            OverallProxy.Instance.ApplySkill(_lastGeneratedSkill[7]);
        }


    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.DiscreteActions;

        if(logmanager.GetEpisodeLogCount() == 0)
        {
            return;
        }

        float currentWinrate = logmanager.GetWinRate();
        float targetWinrate = (float)RewardTargetValues["pcg_win_rate"];
 

        // Sample a sample value 0-3 and make a switch
        // Index Description
        // 0: range
        // 2: cooltime
        // 1: casttime
        // 3: value
        int actSelection = UnityEngine.Random.Range(0, 4);
        
        if (!RandomAgent) {
            
            if (Math.Abs(currentWinrate - targetWinrate) < 0.02f)
            {
                // Do nothing
                for (int i = 0; i < actions.Length; i++)
                {
                    actions[i] = 2;
                }
                return;
            }
            else
            {
                if (currentWinrate < targetWinrate)
                {
                    // Make the game easier   
                    switch (actSelection)
                    {
                        case 0:
                            actions[0] = 3;
                            // actions[1] = 2;
                            // actions[2] = 2;
                            // actions[3] = 2;
                            break;
                        case 1:
                            // actions[1] = 1;
                            actions[0] = 2;
                            // actions[2] = 2;
                            // actions[3] = 2;
                            break;
                        case 2:
                            // actions[2] = 1;
                            actions[0] = 2;
                            // actions[1] = 2;
                            // actions[3] = 2;
                            break;
                        case 3:
                            // actions[3] = 3;
                            actions[0] = 2;
                            // actions[1] = 2;
                            // actions[2] = 2;
                            break;
                    }
                }
                else
                {
                    // Make the game harder
                    switch (actSelection)
                    {
                        case 0:
                            actions[0] = 1;
                            // actions[1] = 2;
                            // actions[2] = 2;
                            // actions[3] = 2;
                            break;
                        case 1:
                            // actions[1] = 3;
                            actions[0] = 2;
                            // actions[2] = 2;
                            // actions[3] = 2;
                            break;
                        case 2:
                            // actions[2] = 3;
                            actions[0] = 2;
                            // actions[1] = 2;
                            // actions[3] = 2;
                            break;
                        case 3:
                            // actions[3] = 1;
                            actions[0] = 2;
                            // actions[1] = 2;
                            // actions[2] = 2;
                            break;
                    }

                }
            }
        } 
        else
        {
            // Put random value in the actions
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i] = UnityEngine.Random.Range(0, 5);
            }
        }

        _heuristicAction = actions;
    }

    
}
