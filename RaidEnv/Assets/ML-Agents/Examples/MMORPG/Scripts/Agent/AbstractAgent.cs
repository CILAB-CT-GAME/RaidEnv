using System.Collections;
using System.Collections.Generic;
using System.Linq; //도움핑 시연용
using System.Text;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public abstract class AbstractAgent : Agent
{
    protected MMORPGEnvController m_GameController;
    public Rigidbody m_Rigidbody { get; protected set; }
    public AbstractClass _class { get; protected set; }
    public List<AbstractSkill> _skillList { get; protected set; }
    public AbstractStatus _status { get; protected set; }

    private readonly Dictionary<Aura, AbstractAura> _bufflist =
        new Dictionary<Aura, AbstractAura>();
    private readonly Dictionary<Aura, AbstractAura> _debufflist =
        new Dictionary<Aura, AbstractAura>();

    public GameObject _target { get; protected set; }

    // TODO: private or protected or getter setter
    [HideInInspector]
    public bool isCasting = false;
    // protected Coroutine currentCasting;
    protected IEnumerator currentCasting = null;

    [HideInInspector]
    public float remainingCastTimeRatio; //UI용 임시추가

    [HideInInspector]
    public bool isMoving = false;

    [HideInInspector]
    public bool isChanneling = false;
    private ItemGenerator m_ItemManager;
    private SkillGenerator m_SkillManager;
    public GameObject m_StatManager;
    //MA_CODE//
    public GameObject PingSignal;
    //MA_CODE//
    public GameObject HelpSignal;
    //MA_CODE//
    public bool UseHelpSignalAction = false;

    // public ObjectPool objectPool;
    public int player_id; // 몇 번째 플레이어인지
    //MA_CODE//
    public bool helpRequested; // 도움핑 시연용
    //MA_CODE//
    public AbstractAgent helpMe; // 도움핑 시연용
    public GameObject projectiles;
    private Dictionary<string, Queue<GameObject>> skillsQueue;
    public bool ruleBased; // 룰베이스
    //MA_CODE//
    public bool RandomAgent;
    public ProjectileManager m_ProjectileManager;
    public Dictionary<string, Queue<GameObject>> _skillQueue { get; private set; }

    protected virtual void Awake()
    {
        // TODO : Instantiate, update using prepab
        // ItemManager = Instantiate(ItemManager, this.transform.position, Quaternion.identity);
        m_GameController = GetComponentInParent<MMORPGEnvController>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_SkillManager = gameObject.AddComponent<SkillGenerator>();
        
        _class.Initialize();
        _status.Initialize();

        // objectPool = gameObject.GetComponent<ObjectPool>();

        if (_skillList == null)
        {
            //Todo : remove this line after being able to input all parameter to simulator
            m_SkillManager.InitializeSkillSet(this.gameObject.tag);
            _skillList = m_SkillManager.skillList;
            
            var projMgrObj = Common.FindChildWithName(transform.parent, "ProjectileManager");
            m_ProjectileManager = projMgrObj.GetComponent<ProjectileManager>();
            m_ProjectileManager.Initialize(gameObject);
            _skillQueue = m_ProjectileManager.agentQueue[gameObject.name];
        }
    }

    public override void OnEpisodeBegin()
    {
        _class.Initialize();
        _status.Initialize();

        _target = null;

        if (currentCasting != null)
        {
            StopCoroutine(currentCasting);
            currentCasting = null;
        }

        foreach (AbstractSkill skill in _skillList)
        {
            skill.condition.nowCharged = skill.condition.maximumCharge;
            skill.condition.isCooldown = false;
            skill.condition.cooltimeLeft = skill.condition.cooltime;
        }

        isCasting = false;
    }

    void FixedUpdate()
    {
        CheckAgentDead(); // check agent is dead

        foreach (var buff in _bufflist.Values.ToList())
        {
            buff.Tick(Time.fixedDeltaTime);
            if (buff.isFinished)
            {
                _bufflist.Remove(buff.aura);
            }
        }
        foreach (var debuff in _debufflist.Values.ToList())
        {
            debuff.Tick(Time.fixedDeltaTime);
            if (debuff.isFinished)
            {
                _debufflist.Remove(debuff.aura);
            }
        }
    }

    public void AddBuff(AbstractAura buff)
    {
        if (_bufflist.ContainsKey(buff.aura))
        {
            _bufflist[buff.aura].Activate();
        }
        else
        {
            _bufflist.Add(buff.aura, buff);
            buff.Activate();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(IHaveAKey);
    }

    //MA_CODE//
    protected void EmitPingSignal()
    {
        GameObject obj = Instantiate(
            this.PingSignal,
            this.transform.position + new Vector3(0, 2.5f, 0),
            Quaternion.identity
        );
        obj.transform.SetParent(this.transform);

        PingSignal signal = obj.GetComponent<PingSignal>();
        signal.RemainedStep = 20;
        signal.Owner = this;
        signal.Type = PingSignalType.Help;

        m_GameController.OnPlayerEmitPingSignal(this, signal);
    }

    protected IEnumerator Cast(AbstractSkill skill)
    {
        var startTime = Time.fixedTime;
        isCasting = true;

        for (
            float remainingCastTime = skill.condition.casttime;
            remainingCastTime >= 0;
            remainingCastTime -= Time.fixedDeltaTime
        )
        {
            remainingCastTimeRatio = remainingCastTime / skill.condition.casttime; //UI용 추가

            if (isMoving)
            {
                Debug.Log("Cast canceled due to moving");
                isMoving = false;
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }


        ChangeTarget();
        // objectPool.GetObject(this.gameObject, _target, skill);

        skill.Activate(gameObject, _target);
        currentCasting = null;
        
        var finishTime = Time.fixedTime;

        if (!skill.condition.isCooldown)
            StartCoroutine(skill.Cooldown());

        isCasting = false;
        remainingCastTimeRatio = 0; //UI용 추가
    }

    protected bool Execute(AbstractSkill skill)
    {
        if (!gameObject.activeSelf) return false;
        // if (objectPool.CanUse(this, _target, skill))
        if (skill.CanUse(this, _target))
        {
            currentCasting = Cast(skill);
            StartCoroutine(currentCasting);
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void UsePortion(int whichPortion, AbstractAgent target)
    {
        m_ItemManager.UseItem(whichPortion, this);
    }

    protected bool CheckAgentDead()
    {
        bool isDead = false;
        if (_status.health.current <= 0)
        {
            switch (this.gameObject.tag)
            {
                case "agent":
                    m_GameController.OnPlayerKilled(this, null);
                    isDead = true;
                    break;
                case "enemy":
                    m_GameController.OnEnemyKilled(this, null);
                    isDead = true;
                    break;
                default:
                    Debug.Log("error in Abstract Agent - CheckAgentDead");
                    break;
            }
        }
        return isDead;
    }

    protected virtual void ChangeTarget()
    {
        List<AbstractAgent> tmpAgentList = new List<AbstractAgent>();
        List<AbstractAgent> tmpEnemyList = new List<AbstractAgent>();
        for (
            int i = 0;
            i < this.gameObject.GetComponentInParent<MMORPGEnvController>().AgentsList.Count;
            i++
        )
        {
            if (
                this.gameObject.GetComponentInParent<MMORPGEnvController>().AgentsList[i]
                    .Agent
                    .gameObject
                    .activeSelf == true
            )
            {
                tmpAgentList.Add(
                    this.gameObject.GetComponentInParent<MMORPGEnvController>().AgentsList[i].Agent
                );
            }
        }

        for (
            int i = 0;
            i < this.gameObject.GetComponentInParent<MMORPGEnvController>().EnemiesList.Count;
            i++
        )
        {
            if (
                this.gameObject.GetComponentInParent<MMORPGEnvController>().EnemiesList[i]
                    .Agent
                    .gameObject
                    .activeSelf == true
            )
            {
                tmpEnemyList.Add(
                    this.gameObject.GetComponentInParent<MMORPGEnvController>().EnemiesList[i].Agent
                );
            }
        }

        switch (this.gameObject.tag)
        {
            case "agent":
                _target = GetClosestEnemy(tmpEnemyList);
                break;
            case "enemy":
                _target = GetClosestEnemy(tmpAgentList);

                break;
        }
    }

    protected GameObject GetClosestEnemy(List<AbstractAgent> enemies)
    {
        GameObject targetNearest = null;

        float minDist = Mathf.Infinity;
        Vector3 curPos = transform.position;
        foreach (AbstractAgent enemy in enemies)
        {
            float dist = Vector3.Distance(enemy.gameObject.transform.position, curPos);
            if (dist < minDist)
            {
                targetNearest = enemy.gameObject;
                minDist = dist;
            }
        }

        return targetNearest;
    }

    public MMORPGEnvController GetGameController()
    {
        return this.m_GameController;
    }

    public void SetConfig(Hashtable config)
    {
        _class.config = config;

        _class.Initialize();
        _status.Initialize();

        _skillList = _class.skillList;
    }

    //MA_CODE//
    public void HelpRequested(AbstractAgent source)
    { // 도움핑 시연용
        if (this != source)
        {
            this.helpRequested = true;
            helpMe = source;

            if (this.HelpSignal)
            {
                GameObject obj = Instantiate(
                    this.HelpSignal,
                    this.transform.position + new Vector3(0, 2.5f, 0),
                    Quaternion.identity
                );
                obj.transform.SetParent(this.transform);
            }
        }
    }

    //MA_CODE//
    public void RequestHelp()
    { // 도움핑 시연용
        //가장 피 많은 agent 찾기
        List<MMORPGEnvController.PlayerInfo> teamList = this.m_GameController.AgentsList;
        AbstractAgent agentWithMaxHp;
        int maxHP;
        List<int> teamHP = (
            from info in teamList
            where info.Agent != this && info.Agent.gameObject.activeInHierarchy
            select info.Agent._status.health.current
        ).ToList();
        if (teamHP.Any())
            maxHP = teamHP.Max();
        else
            return;
        agentWithMaxHp = teamList.Find(info => info.Agent._status.health.current == maxHP).Agent;
        agentWithMaxHp.HelpRequested(this);
    }

    //MA_CODE//
    public Vector3 GiveHelp(AbstractAgent source)
    { // 도움핑 시연용
        var enemy = this.m_GameController.EnemiesList[0];
        Vector3 enemyPosition = enemy.Agent.transform.localPosition;
        Vector3 dirToGo = (transform.parent.rotation * (enemyPosition - transform.localPosition));
        transform.rotation = Quaternion.LookRotation(dirToGo);

        Vector3 sourceToEnemy = enemyPosition - source.transform.localPosition;
        if (dirToGo.magnitude < 0.9 * sourceToEnemy.magnitude)
        {
            this.helpRequested = false;
            helpMe = null;
        }
        return dirToGo.normalized;
    }

    /// <summary>
    ///
    ///
    /// </summary>
    /// <returns>Relation position from the area</returns>
    
    //MA_CODE//
    public Vector3 GetPosition()
    {
        Vector3 anchorPos = this.m_GameController.gameObject.transform.position;
        Vector3 relativePos = this.gameObject.transform.position - anchorPos;
        return relativePos;
    }

    /// <summary>
    ///
    ///
    /// </summary>
    /// <returns></returns>
    public MovementLog CreateMovementLog()
    {
        return new MovementLog(this, GetPosition(), this.m_GameController.GetEpisodeStep());
    }
}
