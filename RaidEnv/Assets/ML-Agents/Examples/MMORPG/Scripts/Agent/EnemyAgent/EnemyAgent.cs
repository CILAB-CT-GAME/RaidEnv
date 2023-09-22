using System.Collections;

using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Linq;


public class EnemyAgent : AbstractAgent
{
    public enum EnemyModel { Dummy, Dragon };
    public EnemyModel enemyModel;

    public enum Type { A, B, C };
    public Type enemyType;
    public int MovementRecordInterval = 1;
    
    protected override void Awake()
    {
        switch (enemyModel)
        {
            case EnemyModel.Dummy:
                _class = new Dummy();
                _status = new DummyStatus();
                break;
            case EnemyModel.Dragon:
                _class = new Dragon();
                _status = new DragonStatus();
                break;
        }

        base.Awake();
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        // enemyType = Type.C;
        ChangeTarget();
    }

    void Chase()
    {
        if (_target != null) // 타겟이 있을 때만
        {
            Vector3 direction = new Vector3();
            direction = (_target.transform.position - this.transform.position).normalized;

            if (Vector3.Distance(_target.transform.position, this.transform.position) >= 1.6f)  // 일정 거리 이상일때만 (완전 근접하면 위로 엎어치기함)
            {
                if (isCasting == false) // 캐스팅 안하고 있을때만
                {
                    if (enemyType == Type.C)
                    {
                        this.gameObject.transform.position += Vector3.Scale(direction, new Vector3(_status.etc.moveSpeed, 0, _status.etc.moveSpeed)) * Time.fixedDeltaTime; // 거리 좁히는거
                    }
                    else
                    {
                        this.gameObject.transform.position += Vector3.Scale(direction, new Vector3(10, 0, 10)) * Time.fixedDeltaTime; // 거리 좁히는거

                    }

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
            // if (objectPool.CanUse(this, _target,_skill))
            if (_skill.CanUse(this, _target))
            {
                Execute(_skill);
            }
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(true);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        CheckAgentDead();

        if (m_GameController.GetEpisodeStep() % MovementRecordInterval == 0)
        {
            m_GameController.OnMovementLogReceived(this, CreateMovementLog());
        }
        Chase();
        Attack();
    }

    void MoveToAgent()
    {

    }
}