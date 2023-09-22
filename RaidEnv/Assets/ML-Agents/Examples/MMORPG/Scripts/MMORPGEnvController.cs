using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Policies;

public class MMORPGEnvController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerInfo
    {
        public AbstractAgent Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
        [HideInInspector]
        public Collider Col;
        // [HideInInspector]
        public PingSignal Signal;
    }

    [System.Serializable]
    public class DeadPlayerInfo
    {
        public AbstractAgent Agent;
        // public GameObject Tombstone;
        public int RemainedRespawnSteps;
    }

    [System.Serializable]
    public class EnemyInfo
    {
        public AbstractAgent Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
        [HideInInspector]
        public Collider Col;
        [HideInInspector]
        public Transform T;
        public bool IsDead;
    }

    /// <summary>
    /// Max Academy steps before this platform resets
    /// </summary>
    /// <returns></returns>
    [Header("Max Environment Steps")] public int MaxEnvironmentSteps = 10000;
    [Header("HUMAN PLAYER")] public GameObject PlayerGameObject;
    private int m_ResetTimer;

    /// <summary>
    /// The area bounds.
    /// </summary>
    [HideInInspector]
    public Bounds areaBounds;
    /// <summary>
    /// The ground. The bounds are used to spawn the elements.
    /// </summary>
    public GameObject ground;

    Material m_GroundMaterial; //cached on Awake()

    /// <summary>
    /// We will be changing the ground material based on success/failue
    /// </summary>
    Renderer m_GroundRenderer;

    public List<PlayerInfo> AgentsList = new List<PlayerInfo>();
    public List<DeadPlayerInfo> DeadAgentsList = new List<DeadPlayerInfo>();
    public List<EnemyInfo> EnemiesList = new List<EnemyInfo>();
    private Dictionary<AbstractAgent, PlayerInfo> m_PlayerDict = new Dictionary<AbstractAgent, PlayerInfo>();

    // Battle
    public bool UseRandomAgentRotation = true;
    public bool UseRandomAgentPosition = true;
    public bool RespawnAgents = false;
    public int RespawnSteps = 50;


    private int m_NumberOfRemainingPlayers;
    private int m_NumberOfRemainingEnemies;
    // public GameObject Tombstone;

    private SimpleMultiAgentGroup m_AgentGroup;
    private SimpleMultiAgentGroup m_EnemyGroup;

    public LogManager m_LogMgr;
    private EpisodeLog m_EpisodeLog;

    //MA_CODE//
    [Header("Overall Proxy")]
    public bool useMAProxy;
    private int n_episode;

    void Start()
    {
        // Get the ground's bounds
        areaBounds = ground.GetComponent<Collider>().bounds;
        // Get the ground renderer so we can change the material when a goal is scored
        m_GroundRenderer = ground.GetComponent<Renderer>();
        // Starting material
        m_GroundMaterial = m_GroundRenderer.material;
        //Reset Players Remaining
        m_NumberOfRemainingPlayers = AgentsList.Count;
        m_NumberOfRemainingEnemies = EnemiesList.Count;
        // Initialize TeamManager
        m_AgentGroup = new SimpleMultiAgentGroup();
        foreach (var item in AgentsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
            item.Col = item.Agent.GetComponent<Collider>();

            // Add to team manager
            m_AgentGroup.RegisterAgent(item.Agent);
        }

        m_EnemyGroup = new SimpleMultiAgentGroup();
        foreach (var item in EnemiesList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.T = item.Agent.transform;
            item.Col = item.Agent.GetComponent<Collider>();

            // Add to team manager
            m_EnemyGroup.RegisterAgent(item.Agent);
        }

        this.InitLogManager();
        ResetScene();
    }

    private void InitLogManager()
    {
        var logMgrObj = GameObject.Find("LogManager");

        if (logMgrObj)
        {
            m_LogMgr = logMgrObj.GetComponent<LogManager>();
        }
    }

    void Update()
    {
        // Compare the equality of the reference on first and second agent's first skill
    }

    void FixedUpdate()
    {
        
        //RespawnAgent();
        DecreasePingStep();

        m_ResetTimer += 1;

        if ((m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0) || (m_NumberOfRemainingPlayers == 0 && RespawnAgents == false))
        {
            m_AgentGroup.GroupEpisodeInterrupted();
            m_EnemyGroup.GroupEpisodeInterrupted();
            EndGame(GameResult.Draw);
        }
    }

    //MA_CODE//
    private void DecreasePingStep()
    {
        foreach (var agentInfo in AgentsList)
        {
            if (agentInfo.Signal == null) continue;

            agentInfo.Signal.RemainedStep -= 1;

            if (agentInfo.Signal.RemainedStep <= 0)
            {
                agentInfo.Signal.OnDestroy();
                agentInfo.Signal = null;
            }
        }
    }

    //MA_CODE//
    public bool OnPlayerEmitPingSignal(AbstractAgent agent, PingSignal signal)
    {

        int index = AgentsList.FindIndex(item => item.Agent == agent);

        if (index == -1) return false;

        if (AgentsList[index].Signal != null)
        {
            AgentsList[index].Signal.OnDestroy();
        }

        AgentsList[index].Signal = signal;
        return true;
    }

    public void RespawnAgent()
    {
        if (DeadAgentsList.Count == 3)
        {
            foreach (var body in DeadAgentsList)
            {
                body.RemainedRespawnSteps = body.RemainedRespawnSteps - 1;
            }
            for (int i = 0; i < DeadAgentsList.Count; i++)
            {
                if (DeadAgentsList[i].RemainedRespawnSteps == 0)
                {
                    // DeadAgentsList[i].Tombstone.gameObject.SetActive(false);
                    DeadAgentsList[i].Agent.gameObject.SetActive(true);
                    DeadAgentsList.RemoveAt(i);
                    i--;
                }
            }
            foreach (var body in DeadAgentsList)
            {
                if (body.RemainedRespawnSteps == 0)
                {
                    // body.Tombstone.gameObject.SetActive(false);
                    body.Agent.gameObject.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Use the ground's bounds to pick a random spawn position.
    /// </summary>
    public Vector3 GetRandomSpawnPos()
    {
        // var foundNewSpawnLocation = false;
        var randomSpawnPos = Vector3.zero;
        // while (foundNewSpawnLocation == false)
        // {
        //     var randomPosX = Random.Range(-areaBounds.extents.x * m_PushBlockSettings.spawnAreaMarginMultiplier,
        //         areaBounds.extents.x * m_PushBlockSettings.spawnAreaMarginMultiplier);

        //     var randomPosZ = Random.Range(-areaBounds.extents.z * m_PushBlockSettings.spawnAreaMarginMultiplier,
        //         areaBounds.extents.z * m_PushBlockSettings.spawnAreaMarginMultiplier);
        //     randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 1f, randomPosZ);
        //     if (Physics.CheckBox(randomSpawnPos, new Vector3(2.5f, 0.01f, 2.5f)) == false)
        //     {
        //         foundNewSpawnLocation = true;
        //     }
        // }
        return randomSpawnPos;
    }

    /// <summary>
    /// Swap ground material, wait time seconds, then swap back to the regular material.
    /// </summary>
    IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time)
    {
        m_GroundRenderer.material = mat;
        yield return new WaitForSeconds(time); // Wait for 2 sec
        m_GroundRenderer.material = m_GroundMaterial;
    }

    public void BaddieTouchedBlock()
    {
        if (m_AgentGroup != null) m_AgentGroup.EndGroupEpisode();
        if (m_EnemyGroup != null) m_EnemyGroup.EndGroupEpisode();


        // Swap ground material for a bit to indicate we scored.
        // StartCoroutine(GoalScoredSwapGroundMaterial(m_PushBlockSettings.failMaterial, 0.5f));
        ResetScene();
    }

    Quaternion GetRandomRot()
    {
        return Quaternion.Euler(0, UnityEngine.Random.Range(0.0f, 360.0f), 0);
    }

    private void ResetEpisodeLog()
    {
        m_EpisodeLog = new EpisodeLog(AgentsList, EnemiesList);

        m_EpisodeLog.EnvSetting = new EnvironmentSetting(3, 1);

        for (int i = 0; i < AgentsList.Count; i++)
        {
            AbstractStatus _status = AgentsList[i].Agent._status;
            List<AbstractSkill> _skillList = AgentsList[i].Agent._skillList;
            m_EpisodeLog.EnvSetting.PlayerStats[i] = _status;
            m_EpisodeLog.EnvSetting.SkillParameters[i] = _skillList;
        }
    }

    public void OnCombatLogReceived(CombatLog log)
    {
        m_EpisodeLog.AddCombatLog(log);
    }

    public void OnMovementLogReceived(AbstractAgent agent, MovementLog log)
    {
        if (agent.GetType() == typeof(RaidPlayerAgent))
        {
            int agentIdx = AgentsList.FindIndex(info => info.Agent == agent);
            if (agentIdx != -1)
            {
                m_EpisodeLog.PlayerMovements[agentIdx].Add(log);
            }
        }
        else if (agent.GetType() == typeof(EnemyAgent))
        {
            int agentIdx = EnemiesList.FindIndex(info => info.Agent == agent);

            if (agentIdx != -1)
            {
                m_EpisodeLog.EnemyMovements[agentIdx].Add(log);
            }

        }
        else
        {
            Debug.Log("Unknown Agent Type: " + agent.GetType());
        }
    }


    public void OnSkillTrigLogReceived(CombatLog log)
    {
        m_EpisodeLog.AddSkillTrigLog(log);
    }


    public EpisodeLog GetEpisodeLog()
    {
        return m_EpisodeLog;
    }

    void ResetScene()
    {
        //Reset counter
        m_ResetTimer = 0;

        //Reset Players Remaining
        m_NumberOfRemainingPlayers = AgentsList.Count;
        m_NumberOfRemainingEnemies = EnemiesList.Count;

        //Reset Agents
        foreach (var item in AgentsList)
        {
            var pos = UseRandomAgentPosition ? GetRandomSpawnPos() : item.StartingPos;
            var rot = UseRandomAgentRotation ? GetRandomRot() : item.StartingRot;

            item.Agent.transform.SetPositionAndRotation(pos, rot);
            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
            item.Agent.gameObject.SetActive(true);

            m_AgentGroup.RegisterAgent(item.Agent);
            // item.Agent.m_ItemManager.resetAllUsedItem();

            if (item.Signal)
            {
                item.Signal.OnDestroy();
                item.Signal = null;
            }
        }

        //Reset Tombstone
        // Tombstone.SetActive(false);
        DeadAgentsList.Clear();

        //Clear all projectiles
        foreach (var projectile in gameObject.GetComponentsInChildren<Projectile>())
        {
            projectile.Deactivate();
        }

        //End Episode
        foreach (var item in EnemiesList)
        {
            if (!item.Agent)
            {
                return;
            }
            item.Agent.transform.SetPositionAndRotation(item.StartingPos, item.StartingRot);
            item.Agent.gameObject.SetActive(true);

            m_EnemyGroup.RegisterAgent(item.Agent);
        }

        // adjust skill parameter by samping
        if (useMAProxy && (n_episode++ % 2 == 0))
        {
            List<AbstractAgent> agents = new List<AbstractAgent>();
            foreach (PlayerInfo agent in AgentsList)
            {
                agents.Add(agent.Agent);
            }
            foreach (EnemyInfo enemy in EnemiesList)
            {
                agents.Add(enemy.Agent);
            }

            OverallProxy.Instance.RequestMAConfig(agents);
        }

        ResetEpisodeLog();
    }

    public void OnPlayerKilled(AbstractAgent agent, GameObject source)
    {
        DeadPlayerInfo Deadman = new DeadPlayerInfo();

        m_NumberOfRemainingPlayers--;
        agent.gameObject.SetActive(false);

        //Spawn Tombstone
        // Tombstone.transform.SetPositionAndRotation(agent.transform.position, agent.transform.rotation);
        // Tombstone.SetActive(true);

        //add dead agent to the respawnlist
        Deadman.Agent = agent;
        // Deadman.Tombstone = Tombstone;
        Deadman.RemainedRespawnSteps = RespawnSteps;

        if (RespawnAgents)
        {
            DeadAgentsList.Add(Deadman);
        }
        if (m_NumberOfRemainingPlayers == 0)
        {
            this.EndGame(GameResult.EnemyWin);
        }
    }

    public void OnEnemyKilled(AbstractAgent enemy, GameObject objSource)
    {
        m_NumberOfRemainingEnemies--;
        //MA_CODE//
        m_AgentGroup.AddGroupReward(1.0f);
        this.EndGame(GameResult.PlayerWin);

    }

    public void EndGame(GameResult result)
    {
        m_AgentGroup.GroupEpisodeInterrupted();
        m_EnemyGroup.GroupEpisodeInterrupted();

        for (int i = 0; i < EnemiesList.Count; i++)
        {
            AbstractStatus _status = EnemiesList[i].Agent._status;
            m_EpisodeLog.BossCurrentHealth[i] = _status.health.current;
        }

        if (m_LogMgr)
        {
            m_EpisodeLog.Result = result;

            m_EpisodeLog.EpisodeLength = GetEpisodeStep();
            m_LogMgr.OnEpisodeLogReceived(m_EpisodeLog, this);
        }

        this.ResetScene();
    }

    private int m_lastFrameCount;
    public int GetEpisodeStep()
    {
        return (int)Time.frameCount - m_lastFrameCount;
    }

    public int GetEpisodeLength()
    {
        return m_ResetTimer;
    }
}
