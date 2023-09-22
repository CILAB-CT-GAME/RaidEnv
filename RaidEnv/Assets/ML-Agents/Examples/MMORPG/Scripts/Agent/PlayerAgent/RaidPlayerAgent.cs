using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RaidPlayerAgent : AbstractAgent
{
    public ClassSpecialization _classSpecialization;
    public BufferSensorComponent m_OtherAgentsBuffer;
    public int MovementRecordInterval = 1;
    private float m_LocationNormalizationFactor = 20.0f;
    private float m_VelocityNormalizationFactor = 20.0f;
    private KeyMap m_KeyMap = new KeyMap();

    [HideInInspector] public float properDistance = 0;

    [HideInInspector] public bool isClockwise = true;

    public bool ManualStepping = false;

    protected override void Awake()
    {
        switch (_classSpecialization)
        {
            case ClassSpecialization.Mage:
                _class = new MageClass();
                _status = new MageStatus();
                break;
            case ClassSpecialization.Ranger:
                _class = new RangerClass();
                _status = new RangerStatus();
                break;
            case ClassSpecialization.SwordsMan:
                _class = new SwordsManClass();
                _status = new SwordsManStatus();
                break;
        }

        base.Awake();
    }

    public override void Initialize()
    {
        base.Initialize();
        var bufferSensors = GetComponentsInChildren<BufferSensorComponent>();
        m_OtherAgentsBuffer = bufferSensors[0];
    }

    void Update()
    {


    }

    int frameCount = 0;
    private void FixedUpdate()
    {
        CheckAgentDead();

        if (m_GameController.GetEpisodeStep() % MovementRecordInterval == 0)
        {
            m_GameController.OnMovementLogReceived(this, CreateMovementLog());
        }


        if (ruleBased)
        {
            CalcDistance();
            Chase();
            Attack();
        }

        if (ManualStepping)
        {
            frameCount += 1;
            if (frameCount % 5 == 0)
                Academy.Instance.EnvironmentStep();
        }

    }

    public override void OnEpisodeBegin()
    {
        if (Random.Range(0, 1000) % 2 == 0)
            isClockwise = true;
        else
            isClockwise = false;

        base.OnEpisodeBegin();
        ChangeTarget();
    }

    void Chase()
    {
        if (_target != null) // 타겟이 있을 때만
        {
            Vector3 direction = new Vector3();
            Vector3 circle = new Vector3();
            direction = (_target.transform.position - this.transform.position).normalized;
            if (isCasting == false)
            {
                //스킬 최대 사정거리 유지
                if (Vector3.Distance(_target.transform.position, this.transform.position) > properDistance)
                {
                    this.transform.position += Vector3.Scale(direction, new Vector3(_status.etc.moveSpeed, 0, _status.etc.moveSpeed)) * Time.fixedDeltaTime * 5.0f;
                }
                else
                {
                    //보스 주변을 원형 이동
                    if (isClockwise)
                    {
                        circle = new Vector3(
                        Mathf.Cos(Time.timeSinceLevelLoad * _status.etc.moveSpeed),
                        0,
                        Mathf.Sin(Time.timeSinceLevelLoad * _status.etc.moveSpeed))
                        * properDistance;
                    }
                    else
                    {
                        circle = new Vector3(
                        Mathf.Sin(Time.timeSinceLevelLoad * _status.etc.moveSpeed),
                        0,
                        Mathf.Cos(Time.timeSinceLevelLoad * _status.etc.moveSpeed))
                        * properDistance;
                    }
                    this.transform.position = Vector3.Lerp(this.transform.position, _target.transform.position - circle, Time.fixedDeltaTime);
                    this.transform.position -= Vector3.Scale(direction, new Vector3(_status.etc.moveSpeed, 0, _status.etc.moveSpeed)) * Time.fixedDeltaTime * 5.0f;
                }
            }
            this.gameObject.transform.rotation = Quaternion.LookRotation(direction); // 바라보게 하는거 (얼굴은 캐스팅중이라도 항상 돌리도록)
        }
        else
        {
            ChangeTarget();
        }
    }

    void Attack()
    {
        foreach (var _skill in _skillList)
        {
            if (_skill.info.name == "Auto Attack") continue;
            // if (objectPool.CanUse(this, _target, _skill))
            if (_skill.CanUse(this, _target))
            {
                Execute(_skill);
            }
        }
    }

    void CalcDistance()
    {
        properDistance = 0;

        foreach (var _skill in _skillList)
        {
            if (_skill.info.name == "Auto Attack") continue;
            if (_skill.condition.nowCharged > 0)
            {
                if (_skill.condition.range > properDistance)
                {
                    properDistance = _skill.condition.range;
                }
            }
        }

        if (properDistance == 0) properDistance = 10;
        //Debug.Log("Proper Distance is " + properDistance);
    }

    //MA_CODE//
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position.x / m_LocationNormalizationFactor);
        sensor.AddObservation(transform.position.z / m_LocationNormalizationFactor);
        sensor.AddObservation(Vector3.Dot(this.m_Rigidbody.velocity, this.m_Rigidbody.transform.forward) / m_VelocityNormalizationFactor);
        sensor.AddObservation(Vector3.Dot(this.m_Rigidbody.velocity, this.m_Rigidbody.transform.right) / m_VelocityNormalizationFactor);
        foreach (var enemy in this.m_GameController.EnemiesList)
        {
            sensor.AddObservation(enemy.Agent.transform.position.x / m_LocationNormalizationFactor);
            sensor.AddObservation(enemy.Agent.transform.position.z / m_LocationNormalizationFactor);
            sensor.AddObservation(enemy.Agent.transform.forward.x);
            sensor.AddObservation(enemy.Agent.transform.forward.z);
            sensor.AddObservation(enemy.Agent._skillList[0].condition.cooltimeLeft / 5.0f);
            sensor.AddObservation(enemy.Agent._skillList[1].condition.cooltimeLeft / 5.0f);
        }

        List<MMORPGEnvController.PlayerInfo> teamList = this.m_GameController.AgentsList;
        foreach (var info in teamList)
        {
            if (info.Agent != this)
            {
                if (info.Agent.gameObject.activeInHierarchy)
                {
                    this.m_OtherAgentsBuffer.AppendObservation(GetOtherAgentData(info));
                }
                else
                {
                    this.m_OtherAgentsBuffer.AppendObservation(new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 });
                }
            }
        }
        sensor.AddObservation(_status.health.current / 100.0f);
        sensor.AddObservation(_skillList[0].condition.cooltimeLeft/ 5.0f);
        sensor.AddObservation(_skillList[0].condition.range/ 20.0f);
        sensor.AddObservation(_skillList[0].coefficient.value * _status.spell.power / 100.0f);
        sensor.AddObservation(_skillList[0].condition.casttime/ 5.0f);
    }

    //MA_CODE//
    private float[] GetOtherAgentData(MMORPGEnvController.PlayerInfo info)
    {
        var otherAgentdata = new float[9];
        var relativePosition = info.Agent.transform.position;
        otherAgentdata[0] = relativePosition.x / m_LocationNormalizationFactor;
        otherAgentdata[1] = relativePosition.z / m_LocationNormalizationFactor;
        var relativeVelocity = info.Agent.m_Rigidbody.velocity;
        otherAgentdata[2] = relativeVelocity.x / m_VelocityNormalizationFactor;
        otherAgentdata[3] = relativeVelocity.z / m_VelocityNormalizationFactor;
        otherAgentdata[4] = info.Agent._status.health.current/ 100.0f;
        otherAgentdata[5] = info.Agent._skillList[0].condition.cooltimeLeft/ 5.0f;
        otherAgentdata[6] = info.Agent._skillList[0].condition.range/ 20.0f;
        otherAgentdata[7] = info.Agent._skillList[0].coefficient.value * info.Agent._status.spell.power/ 100.0f;
        otherAgentdata[8] = info.Agent._skillList[0].condition.casttime/ 5.0f;
        return otherAgentdata;
    }

    /// <summary>
    /// Moves the agent according to the selected action.
    /// </summary>

    //MA_CODE//
    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];
        if (helpRequested)
        { //도움핑 시연용
            dirToGo = this.GiveHelp(helpMe);
        }
        else
        {
            switch (action)
            {
                case 0:
                    // no action
                    break;
                case 1:
                    dirToGo = transform.forward * 1f;
                    break;
                case 2:
                    dirToGo = transform.forward * -1f;
                    break;
                case 3:
                    rotateDir = transform.up * 1f;
                    break;
                case 4:
                    rotateDir = transform.up * -1f;
                    break;
                case 5:
                    dirToGo = transform.right * -0.75f;
                    break;
                case 6:
                    dirToGo = transform.right * 0.75f;
                    break;
                case 7:
                    this.Execute(_skillList[0]); //skill1
                    break;
                    // case 8:
                    //     this.Execute(_skillList[1]); //skill2
                    //     break;
                    // case 9:
                    //     this.Execute(_skillList[2]); //skill3
                    //     break;
                    // case 10:
                    //     this.EmitPingSignal(); //signal
                    //     RequestHelp();
                    //     break;
                    // case 11:
                    //     this.UsePortion(0, this);
                    //     print("use_heal_portion"); //item1 (Health)
                    //     break;
                    // case 12:
                    //     this.UsePortion(1, this);
                    //     print("use_mana_portion"); //item2 (Mana)
                    //     break;
            }
        }
        transform.Rotate(rotateDir, Time.deltaTime * 200f);
        m_Rigidbody.AddForce(dirToGo * _status.etc.moveSpeed, ForceMode.VelocityChange);
    }

    /// <summary>
    /// Called every step of the engine. Here the agent takes an action.
    /// </summary>

    //MA_CODE//
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Move the agent using the action.
        MoveAgent(actionBuffers.DiscreteActions);
    }

    //MA_CODE//
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        if(!RandomAgent){

            if (Input.GetKey(m_KeyMap.keySettings["TURN_RIGHT"]))
            {
                discreteActionsOut[0] = 3;
            }
            else if (Input.GetKey(m_KeyMap.keySettings["MOVE_UP"]))
            {
                discreteActionsOut[0] = 1;
            }
            else if (Input.GetKey(m_KeyMap.keySettings["TURN_LEFT"]))
            {
                discreteActionsOut[0] = 4;
            }
            else if (Input.GetKey(m_KeyMap.keySettings["MOVE_DOWN"]))
            {
                discreteActionsOut[0] = 2;
            }
            else if (Input.GetKey(m_KeyMap.keySettings["SKILL_1"]))
            {
                this.Execute(_skillList[0]);
            }
            else if (Input.GetKey(m_KeyMap.keySettings["SKILL_2"]))
            {
                this.Execute(_skillList[1]);
            }
            else if (Input.GetKey(m_KeyMap.keySettings["SKILL_3"]))
            {
                this.Execute(_skillList[2]);
            }
            else if (Input.GetKey(m_KeyMap.keySettings["ITEM_1"]))
            {
                this.UsePortion(0, this);
            }
            else if (Input.GetKey(m_KeyMap.keySettings["ITEM_2"]))
            {
                this.UsePortion(1, this);
            }
            else if (Input.GetKey(m_KeyMap.keySettings["PING_HELP"]))
            {
                this.EmitPingSignal();
                RequestHelp();
            }
            else if (Input.GetKey(m_KeyMap.keySettings["SET_TARGET"]))
            {
                this.ChangeTarget();
            }
            else if (Input.GetKey(m_KeyMap.keySettings["RESET"]))
            {
                Debug.Log("Reset Episode!");
                this.m_GameController.EndGame(GameResult.Draw);
            }
            else if (Input.GetKey(m_KeyMap.keySettings["KILL"]))
            {
                Debug.Log("Kill Agent!");
                this._status.health.current = 0;
            }
        }
        else
        {
            // Put random value in the actions
            int number_random = UnityEngine.Random.Range(0, 8);
            discreteActionsOut[0] = number_random;   
        }
    }

}
