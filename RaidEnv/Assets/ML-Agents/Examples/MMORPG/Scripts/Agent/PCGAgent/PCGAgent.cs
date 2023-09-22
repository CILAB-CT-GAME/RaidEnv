using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEditor;

public class PCGAgent : Agent
{
    protected List<float> _lastGeneratedSkill = null;
    // private List<List<float>> _allActions = new List<List<float>>();
    public int simulationLimit = 100;
    public PCGTargetAgentType _target;
    public PCGGenerateType _type;
    public int _targetAgentNumber;
    public int _targetSkillNumber;
    public bool changeParameterDirectly = false;
    public float _adjustmentValue = 0.05f;
    public float _lowValue = 1f;
    public float _highValue = 2f;
    public float rangeLimitMin = 10f;
    public float rangeLimitMax = 100f;
    public float casttimeLimitMin = 1f;
    public float casttimeLimitMax = 10f;
    public float costLimitMin = 0f;
    public float costLimitMax = 10f;
    public float valueLimitMin = 0.5f;
    public float valueLimitMax = 2f;
    

    protected LogManager logmanager;
    protected OverallGenerator overallGenerator;
    protected List<MMORPGEnvController> AllEnvs = new List<MMORPGEnvController>();


    public string LogPath = null;

    protected Hashtable RewardTargetValues = new Hashtable();
    protected Hashtable RewardWeights = new Hashtable();

    [Header("CSV Logging Options")]
    public bool SaveCreatedSkill = false;
    public string CreatedSkillLogPrefix = "CreatedSkillLog";
    public string CreatedSkillLogName = "Default";
    public int SaveEpisodeLimit = 0;
    public bool StrictEpisodeLength = false;

    [Header("Random Agent Options")]
    public bool RandomAgent = false;

    [Header("Debug")]
    public bool PrintDebug = false;
    public bool RunHeuristic = false;
    protected ActionSegment<int> _heuristicAction;

    // UUID4 for this game instance
    protected string m_uuid = System.Guid.NewGuid().ToString();

    protected float _previousTime;

    [Header("Balancing Options")]
    public bool UseMAProxy = false;
    public bool UseManualMaxStep = false;
    public int ManualMaxStep = 40;
    public int ManualCurrentStep = 0;

    public override void Initialize()
    {

        // Get Environment Parameters from UnityML.
        RewardTargetValues.Add("pcg_win_rate", Academy.Instance.EnvironmentParameters.GetWithDefault("pcg_target_winRate", 0.5f));
        RewardWeights.Add("pcg_win_rate", Academy.Instance.EnvironmentParameters.GetWithDefault("pcg_weight_winRate", 0.5f));

        // Casting the string to bool type

        GameObject[] platforms = GameObject.FindGameObjectsWithTag("platform");
        
        foreach (GameObject mmoenv in platforms)
        {
            AllEnvs.Add(mmoenv.GetComponent<MMORPGEnvController>());
        }

        logmanager = GameObject.Find("LogManager").GetComponent<LogManager>();  
        overallGenerator = gameObject.GetComponent<OverallGenerator>();
        
        SaveCreatedSkill = ParameterManagerSingleton.GetInstance().HasParam("pcgSaveCreatedSkill") || SaveCreatedSkill;
        if(ParameterManagerSingleton.GetInstance().HasParam("pcgHeuristic"))
        {
            RunHeuristic = Convert.ToBoolean(ParameterManagerSingleton.GetInstance().GetParam("pcgHeuristic"));
        }
        if(ParameterManagerSingleton.GetInstance().HasParam("pcgRandom"))
        {
            RandomAgent = Convert.ToBoolean(ParameterManagerSingleton.GetInstance().GetParam("pcgRandom"));
        }
        if(ParameterManagerSingleton.GetInstance().HasParam("pcgSaveEpisodeLimit"))
        {
            SaveEpisodeLimit = Convert.ToInt32(ParameterManagerSingleton.GetInstance().GetParam("pcgSaveEpisodeLimit"));
        }
        if(ParameterManagerSingleton.GetInstance().HasParam("pcgSimulationLimit"))
        {
            simulationLimit = Convert.ToInt32(ParameterManagerSingleton.GetInstance().GetParam("pcgSimulationLimit"));
        }
        if(ParameterManagerSingleton.GetInstance().HasParam("pcgStrictEpisodeLength"))
        {
            StrictEpisodeLength = Convert.ToBoolean(ParameterManagerSingleton.GetInstance().GetParam("pcgStrictEpisodeLength"));
        }

        Academy.Instance.AutomaticSteppingEnabled = false;
        _previousTime = Time.time;
        previousWinrate = null;
        
        SetRandomSkill();
        logmanager.Reset();
    }

    public override void OnEpisodeBegin()
    {
        
        SetRandomSkill();
        if(previousWinrate != null)
        {
            Academy.Instance.StatsRecorder.Add("PCG/LastWinRate", (float)previousWinrate);
            float winRateError = Mathf.Abs(((float)RewardTargetValues["pcg_win_rate"]) - (float)previousWinrate);
            Academy.Instance.StatsRecorder.Add("PCG/LastWinRateError", winRateError);
        }

        if(SaveEpisodeLimit != 0 && CompletedEpisodes >= SaveEpisodeLimit)
        {
            # if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            # else
                Application.Quit();
            #endif
        }

        foreach(MMORPGEnvController env in AllEnvs)
        {
            env.BaddieTouchedBlock();
        }
        
        previousWinrate = null;
        
        ManualCurrentStep = 0;
    }

    private void SetRandomSkill() 
    {
        List<float> tmpGenedSkill;
        if (!UseMAProxy)
        {
            tmpGenedSkill = overallGenerator.RandomlyGenerateList(_type);
        }
        else
        {
            tmpGenedSkill = OverallProxy.Instance.GetSkillParameterArray();
        }
        
        tmpGenedSkill.Add(_targetSkillNumber);

        _lastGeneratedSkill = tmpGenedSkill;
        overallGenerator.GenerateSomethingWithParameter(_target, _type, _targetAgentNumber, tmpGenedSkill);

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
        sensor.AddObservation(GetMinMaxScaledValue(skill[8], thresHold.cooltime[0], thresHold.cooltime[1]));
        sensor.AddObservation(GetMinMaxScaledValue(skill[9], thresHold.casttime[0], thresHold.casttime[1]));
        sensor.AddObservation(GetMinMaxScaledValue(skill[15], thresHold.value[0], thresHold.value[1]));
    }

    protected float GetMinMaxScaledValue(float value, float min, float max)
    {
        return (value - min) / (max - min);
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
        
        GenerateSkill(actionBuffers.DiscreteActions);
        

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

    public void GenerateSkill(ActionSegment<int> _act)
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
            float coolTick = (thresHold.cooltime[1] - thresHold.cooltime[0]) / tickScale;
            float castTimeTick = (thresHold.casttime[1] - thresHold.casttime[0]) / tickScale;
            float valueTick = (thresHold.value[1] - thresHold.value[0]) /tickScale;

            _lastGeneratedSkill[7] += rangeTick * act[0];
            _lastGeneratedSkill[8] += coolTick * act[1];
            _lastGeneratedSkill[9] += castTimeTick * act[2];
            _lastGeneratedSkill[15] += valueTick * act[3];

            _lastGeneratedSkill[7] = ActionLimiting(_lastGeneratedSkill[7], thresHold.range[0], thresHold.range[1]);
            _lastGeneratedSkill[8] = ActionLimiting(_lastGeneratedSkill[8], thresHold.cooltime[0], thresHold.cooltime[1]);
            _lastGeneratedSkill[9] = ActionLimiting(_lastGeneratedSkill[9], thresHold.casttime[0], thresHold.casttime[1]);
            _lastGeneratedSkill[15] = ActionLimiting(_lastGeneratedSkill[15], thresHold.value[0], thresHold.value[1]);
        }
        else
        {
            _lastGeneratedSkill[7] = ActionClipping(act[0], thresHold.range[0], thresHold.range[1]);
            _lastGeneratedSkill[8] = ActionClipping(act[1], thresHold.cooltime[0], thresHold.cooltime[1]);
            _lastGeneratedSkill[9] = ActionClipping(act[2], thresHold.casttime[0], thresHold.casttime[1]);
            _lastGeneratedSkill[15] = ActionClipping(act[3], thresHold.value[0], thresHold.value[1]);

        }

        overallGenerator.GenerateSomethingWithParameter(_target,_type,_targetAgentNumber, _lastGeneratedSkill);

        // Apply range to proxy
        if (UseMAProxy)
        {
            OverallProxy.Instance.ApplySkill(_lastGeneratedSkill[7]);
        }


    }

    protected float ActionClipping(float target, float min, float max)
    {
        return ((max - min) * target) + min;
    }

    protected float ActionLimiting(float target, float min, float max)
    {
        target = Mathf.Min(max, target);
        target = Mathf.Max(min, target);
        return target;
    }

    protected float CalculateReward(float newValue, float oldValue, float targetValue)
    {
        float oldError = Mathf.Abs(targetValue - oldValue);
        float newError = Mathf.Abs(targetValue - newValue);

        // Return the difference between the new error and the old error
        return oldError - newError;
    }

    public void RecordContentAndResult(List<float> content, Hashtable result)
    {
        // Make a new PCG Log file with the parameters
        PCGStepLog log = new PCGStepLog();
        log.EpisodeCount = CompletedEpisodes + 1;
        log.StepCount = StepCount;
        log.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        log.Content = content;
        log.Result = result;
        log.InstanceUUID = m_uuid;

        FlushLog(GetSkillLogPath(), log);
    }

    private string GetSkillLogPath()
    {
        return ParameterManagerSingleton.GetInstance().GetParam("logPath") + 
        "SkillLog_" + ParameterManagerSingleton.GetInstance().GetParam("runId") + "_" + m_uuid + ".csv";
    }


    public void FlushLog(string filePath, PCGStepLog log)
    {
        if (!File.Exists(filePath))
        {
            // Print whether the file exists or not
            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.Write("EpisodeCount,StepCount,Time,InstanceUUID,");

                // Write the CSV header with the content and result index which are named Content[0] with repeated number of times
                for (int i = 0; i < log.Content.Count; i++)
                {
                    sw.Write("Content[" + i + "],");
                }

                //Also write the result index
                for (int i = 0; i < log.Result.Count; i++)
                {
                    // Write the result index with comma, but not last item
                    if (i == log.Result.Count - 1)
                    {
                        sw.Write("Result[" + i + "]");
                    } else {
                        sw.Write("Result[" + i + "],");
                    }
                }
                sw.WriteLine();
            }
        }

        // Append a file to write to csv file 
        using (StreamWriter sw = File.AppendText(filePath))
        {
            sw.WriteLine(log.ToCSVRoW());
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
                            actions[1] = 2;
                            actions[2] = 2;
                            actions[3] = 2;
                            break;
                        case 1:
                            actions[1] = 1;
                            actions[0] = 2;
                            actions[2] = 2;
                            actions[3] = 2;
                            break;
                        case 2:
                            actions[2] = 1;
                            actions[0] = 2;
                            actions[1] = 2;
                            actions[3] = 2;
                            break;
                        case 3:
                            actions[3] = 3;
                            actions[0] = 2;
                            actions[1] = 2;
                            actions[2] = 2;
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
                            actions[1] = 2;
                            actions[2] = 2;
                            actions[3] = 2;
                            break;
                        case 1:
                            actions[1] = 3;
                            actions[0] = 2;
                            actions[2] = 2;
                            actions[3] = 2;
                            break;
                        case 2:
                            actions[2] = 3;
                            actions[0] = 2;
                            actions[1] = 2;
                            actions[3] = 2;
                            break;
                        case 3:
                            actions[3] = 1;
                            actions[0] = 2;
                            actions[1] = 2;
                            actions[2] = 2;
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


public class PCGStepLog
{
    public int EpisodeCount;
    public int StepCount;
    public string Time;
    public string InstanceUUID;
    public List<float> Content;
    public Hashtable Result;

    public string ToCSVRoW()
    {
        string row = "";
        row += EpisodeCount + ",";
        row += StepCount + ",";
        row += Time + ",";
        row += InstanceUUID + ",";
        foreach (float value in Content)
        {
            row += value + ",";
        }
        foreach (DictionaryEntry entry in Result)
        {
            row += entry.Value + ",";
        }
    
        // Remove the last comma
        row = row.Substring(0, row.Length - 1);

        return row;
    }
}