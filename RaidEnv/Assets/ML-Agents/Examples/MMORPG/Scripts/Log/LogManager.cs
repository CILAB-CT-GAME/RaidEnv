using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

using Unity.MLAgents;

public class LogManager : MonoBehaviour
{
    [Header("LOG SETTING")]
    public int MaxEpisodeLogCount = 100;
    public bool SaveMovementLog = false;
    public bool SaveCombatLog = false;
    public bool SaveGameResultLog = false;

    [Header("EPISODE DISPLAY")]
    public Text m_EpisodeDisplayLabel;

    [Header("DAMAGE DISPLAY")]
    public bool RealTimeTracking = false;
    public MMORPGEnvController TrackingTarget;
    public List<MMORPGEnvController> PlatformList;

    public Text m_DetailDisplayLabel;

    [HideInInspector]
    public List<EpisodeLog> EpisodeLogList;
    // public List<List<EpisodeLog>> EpisodeLogList_2;
    public GameObject hand;

    private int m_logCount = 0;

    StatsRecorder statsRecorder;

    void Awake()
    {
        // Register the platforms
        PlatformList = new List<MMORPGEnvController>();

        EpisodeLogList = new List<EpisodeLog>();
        // EpisodeLogList_2 = new List<List<EpisodeLog>>();

        GameObject[] platforms = GameObject.FindGameObjectsWithTag("platform");

        foreach (GameObject obj in platforms)
        {
            PlatformList.Add(obj.GetComponent<MMORPGEnvController>());
            // EpisodeLogList_2.Add(new List<EpisodeLog>());
        }   

        statsRecorder = Academy.Instance.StatsRecorder;


        Reset();
    }

    public void Reset()
    {

        EpisodeLogList.Clear();
        // foreach (List<EpisodeLog> logList in EpisodeLogList_2)
        // {
        //     logList.Clear();
        // }

        m_logCount = 0;
    }

    public void OnEpisodeLogReceived(EpisodeLog log, MMORPGEnvController envCtrl) //endgame  mmorpgenv
    {
        if (GetEpisodeLogCount() >= MaxEpisodeLogCount)
        {
            return;
        }

        m_logCount++;

        int platformId = PlatformList.FindIndex(x => x == envCtrl);
        if (platformId != -1)
        {
            // EpisodeLogList_2[platformId].Add(log);
        }
        EpisodeLogList.Add(log);

        // 1. 보스 체력정보
        foreach (int health in log.BossMaxHealth) // 게임 시작당시 체력
        {
            // Debug.Log(health);
            statsRecorder.Add("boss/health/initial", health);
            // Debug.Log(statsRecorder);
        }

        foreach (int health in log.BossCurrentHealth)  // 게임 종료당시 체력
        {
            // Debug.Log(health);
            statsRecorder.Add("boss/health/terminal", health);
        }

        // 2. 플레이어 스탯 관련 정보
        foreach (AbstractStatus status in log.EnvSetting.PlayerStats)
        {
            // Debug.Log(status.health.max);
            statsRecorder.Add("agent/health/terminal", status.health.current);
        }

        // 3. 스킬 파라미터 관련 자료
        foreach (List<AbstractSkill> skillList in log.EnvSetting.SkillParameters)
        {
            foreach (AbstractSkill skill in skillList)
            {
                // Debug.Log(skill.info.projectileSpeed);
                statsRecorder.Add("agent/skill/coefficient", skill.coefficient.value);
            }
        }

        // 4. 플레이어 무브먼트 관련 로그
        if (SaveMovementLog)
        {
            string currentPath = Directory.GetCurrentDirectory();
            string m_Directorypath = string.Format("{0}/Assets/Log", currentPath);
            if(!Directory.Exists(m_Directorypath)){
                Directory.CreateDirectory(m_Directorypath);
            }
            string csvPath = string.Format("{0}/movement_log.csv", m_Directorypath);

            //Print the text from the file
            using (StreamWriter writer = new StreamWriter(csvPath, true))
            {

                for(int i = 0; i < log.PlayerMovements.Count; i++)
                {

                    List<MovementLog> mvLog = log.PlayerMovements[i];

                    foreach (MovementLog mv in mvLog)
                    {
                        writer.WriteLine(GetEpisodeLogCount().ToString() + "," + i.ToString() + "," + mv.Time + "," + mv.Position.x + "," + mv.Position.y + "," + mv.Position.z);
                    }

                }

                for(int i=0; i< log.EnemyMovements.Count; i++)
                {
                    List<MovementLog> mvLog_enemy = log.EnemyMovements[i];

                    foreach (MovementLog mv in mvLog_enemy)
                    {
                        writer.WriteLine(GetEpisodeLogCount().ToString() + "," + "Boss" + "," + mv.Time + "," + mv.Position.x + "," + mv.Position.y + "," + mv.Position.z);
                    }
                }
            }
        }

        // 5. CombatLog
        if (SaveCombatLog)
        {
            string currentPath = Directory.GetCurrentDirectory();
            string m_Directorypath = string.Format("{0}/Assets/Log", currentPath);
            if(!Directory.Exists(m_Directorypath)){
                Directory.CreateDirectory(m_Directorypath);
            }
            string csvPath = string.Format("{0}/combat_log.csv", m_Directorypath);

            //Print the text from the file
            using (StreamWriter writer = new StreamWriter(csvPath, true))
            {

                foreach (CombatLog cl in log.PlayerCombatLogs)
                {
                    writer.WriteLine(cl.source + "," + cl.target + "," + cl.skill + "," + cl.value + "," + cl.isCritical + "," + cl.isBackAttack);
                }

                foreach (CombatLog cl in log.EnemyCombatLogs)
                {
                    writer.WriteLine(cl.source + "," + cl.target + "," + cl.skill + "," + cl.value + "," + cl.isCritical + "," + cl.isBackAttack);
                }
            }
        }

        // 6. GameResult
        if (SaveGameResultLog)
        {
            string currentPath = Directory.GetCurrentDirectory();
            string m_Directorypath = string.Format("{0}/Assets/Log", currentPath);
            if(!Directory.Exists(m_Directorypath)){
                Directory.CreateDirectory(m_Directorypath);
            }
            string csvPath = string.Format("{0}/gameresult_log.csv", m_Directorypath);

            //Print the text from the file
            using (StreamWriter writer = new StreamWriter(csvPath, true))
            {
                writer.WriteLine(log.EpisodeLength + "," + log.Result + "," + envCtrl.GetEpisodeLength());
            }
        }
    }

    /// <summary>
    /// > It returns the number of EpisodeLogs in the EpisodeLogList_2 list
    /// </summary>
    /// <returns>
    /// The number of EpisodeLogs in the EpisodeLogList_2 list.
    /// </returns>
    public int GetEpisodeLogCount()
    {
        return m_logCount;
    }

    /// <summary>
    /// /// > It returns a list of all the episode logs
    /// </summary>
    /// <returns>
    /// A list of EpisodeLogs
    /// </returns>
    public List<EpisodeLog> GetEpisodeLog()
    {
        return EpisodeLogList;
    }


    public int GetLogBufferSize(List<EpisodeLog> tmp_list)
    {
        return tmp_list.Count;
    }

    public int GetWinCount(List<EpisodeLog> tmp_list, GameResult result)
    {

        return tmp_list.Where(item => item.Result == result).Count();
    }

    public float GetResultRatio(List<EpisodeLog> tmp_list, GameResult result)
    {
        int logSize = GetLogBufferSize(tmp_list);

        if (logSize == 0)
        {
            return 0.0f;
        }
        else
        {
            return (float)GetWinCount(tmp_list, result) / (float)logSize;
        }
    }

    public float GetWinRate()
    {
        return this.GetEpisodeLog().Where(item => item.Result == GameResult.PlayerWin).Count() / (float)GetEpisodeLogCount();
    }

    public EpisodeLog GetLatestEpisodeLog(List<EpisodeLog> tmp_list) {
        if(GetLogBufferSize(tmp_list) > 0) {

            return tmp_list.Last();
        }
        else
        {
            return null;
        }
    }

    public EpisodeLog GetTrackingEpisodeLog()
    {
        // Not implemented
        if (TrackingTarget != null)
        {
            return TrackingTarget.GetEpisodeLog();
        }
        return null;
    }

    public EpisodeLog GetDisplayEpisodeLog(List<EpisodeLog> tmp_list)
    {
        if (RealTimeTracking)
        {
            return GetTrackingEpisodeLog();
        }
        else
        {
            return GetLatestEpisodeLog(tmp_list);
        }
    }

    public float[] GetSkillDealtDamage()
    {
        /**
        에피소드에 발생한 스킬의 데미지 평균 양을 측정함
        **/
        const int NUM_SKILLS = 4;
        float[] meanDealtDamage = new float[NUM_SKILLS];  // 4 is the number of skills;

        foreach (EpisodeLog el in EpisodeLogList)
        {
            float[] _dealtDamage = el.GetSkillDealtDamage();
            for (int i = 0; i < NUM_SKILLS; i++)
            {
                meanDealtDamage[i] += _dealtDamage[i];
            }
        }


        if (EpisodeLogList.Count >= 1) // Exists
        {
            for (int i = 0; i < NUM_SKILLS; i++)
            {
                meanDealtDamage[i] /= EpisodeLogList.Count;
            }
        }

        return meanDealtDamage;

    }


    public string GetEpisodeStatisticsLog(List<EpisodeLog> tmp_list, int list_num)
    {
        int logSize = GetLogBufferSize(tmp_list);
        string output = "";
        output += "[Episode Result]_" + list_num.ToString() + "\n";
        output += string.Format("Player Team {0}/{1} ({2:F2}%)\n",
                                GetWinCount(tmp_list, GameResult.PlayerWin), logSize, GetResultRatio(tmp_list, GameResult.PlayerWin) * 100);
        output += string.Format("Enemy Team {0}/{1} ({2:F2}%)\n",
                                GetWinCount(tmp_list, GameResult.EnemyWin), logSize, GetResultRatio(tmp_list, GameResult.EnemyWin) * 100);
        output += string.Format("Draw {0}/{1} ({2:F2}%)\n",
                                GetWinCount(tmp_list, GameResult.Draw), logSize, GetResultRatio(tmp_list, GameResult.Draw) * 100);
        return output;

    }

    // Update is called once per frame

    void Update()
    {
        if (m_EpisodeDisplayLabel)
        {
            m_EpisodeDisplayLabel.text = "";

            m_EpisodeDisplayLabel.text += GetEpisodeStatisticsLog(GetEpisodeLog(), 0);
        }
        // if (m_DetailDisplayLabel)
        // {
        //     EpisodeLog el = GetDisplayEpisodeLog(EpisodeLogList);
        //     if (el != null)
        //     {
        //         m_DetailDisplayLabel.text = "";
        //         for (int i = 0; i < PlatformList.Count; i++)
        //         {
        //             m_DetailDisplayLabel.text += "[Platform_num]_" + i.ToString() + "\n";
        //             m_DetailDisplayLabel.text += el.ToString();
        //         }

        //     }
        //     else
        //     {
        //         m_DetailDisplayLabel.text = "No Data";
        //     }
        // }
    }
    void Clear()
    {

    }
}



public class EnvironmentSetting
{


    public List<AbstractStatus> PlayerStats;
    public List<List<AbstractSkill>> SkillParameters;

    public EnvironmentSetting()
    {

        this.PlayerStats = new List<AbstractStatus>();
        this.SkillParameters = new List<List<AbstractSkill>>();
    }


    public EnvironmentSetting(int playerCnt, int enemyCnt) : this()
    {

        // Put placeholder
        for (int i = 0; i < playerCnt; i++)
        {
            PlayerStats.Add(null);
            SkillParameters.Add(null);
        }

    }



}

public class MovementLog
{
    public MovementLog(AbstractAgent agent, Vector3 pos, int time)
    {
        Agent = agent;
        Position = pos;
        Time = time;
    }

    // Important: Put relative position to this variable.
    //            Wrong position could be saved in the variable if there is multiple MMORPGEnv platform.
    public AbstractAgent Agent;
    public Vector3 Position;
    public int Time;
}


[Serializable]
public class EpisodeLog
{
    public List<CombatLog> PlayerCombatLogs;
    public List<CombatLog> EnemyCombatLogs;

    public List<AgentLog> PlayerStatistics;
    public List<AgentLog> EnemyStatistics;

    public List<List<MovementLog>> PlayerMovements;
    public List<List<MovementLog>> EnemyMovements;

    public int EpisodeLength;
    public GameResult Result;

    private List<MMORPGEnvController.PlayerInfo> m_AgentsList;
    private List<MMORPGEnvController.EnemyInfo> m_EnemyList;

    public EnvironmentSetting EnvSetting;

    public List<int> BossMaxHealth;
    public List<int> BossCurrentHealth;

    // TODO 정적 정보 (현재 스킬 파라미터, 보스 체력, 플레이어 Stat)

    // TODO  각 에피소드에서 플레이어가 어떻게 움직였는지?

    // TODO Step 단위의 Agent의 현재 리워드


    public class SkillLog
    {
        public int Frame;

        public AbstractAgent Agent;
        public AbstractSkill Skill;  // Pointer of the skill
        public int SkillNo;  // Number of the skill
    }

    public class AgentLog
    {


        public int[] SkillTrigCounts;
        public float[] DealtDamage; // Dealt damages per skills
        public float ReceivedDamage; // Received damages totally

        public int AgentId;
        private int NumSkills;

        private List<CombatLog> SkillTrigLogs;


        public AgentLog(int agentId, int numSkills) : base()
        {
            AgentId = agentId;
            NumSkills = numSkills;

            DealtDamage = new float[numSkills];
            SkillTrigCounts = new int[numSkills];
            ReceivedDamage = 0.0f;

            SkillTrigLogs = new List<CombatLog>();
        }

        public override string ToString()
        {
            string output = "";
            string[] skillStrs = new string[NumSkills];

            for (int i = 0; i < NumSkills; i++)
            {
                skillStrs[i] = string.Format("Skill {0}: {1}", i + 1, DealtDamage[i]);
            }

            string concatedStr = string.Join(", ", skillStrs);

            output += string.Format("Id: {0}, {1}, Received Damage: {2}", AgentId, concatedStr, ReceivedDamage);

            return output;
        }

        public void AddSkillLog(CombatLog log)
        {
            SkillTrigLogs.Add(log);
        }


    }


    public EpisodeLog()
    {


    }

    public EpisodeLog(List<MMORPGEnvController.PlayerInfo> AgentsList,
                        List<MMORPGEnvController.EnemyInfo> EnemyList) : this()
    {
        m_AgentsList = AgentsList;
        m_EnemyList = EnemyList;

        PlayerMovements = new List<List<MovementLog>>();
        EnemyMovements = new List<List<MovementLog>>();

        PlayerCombatLogs = new List<CombatLog>();
        EnemyCombatLogs = new List<CombatLog>();

        PlayerStatistics = new List<AgentLog>();
        EnemyStatistics = new List<AgentLog>();

        int idx = 0;
    
        foreach (var info in AgentsList)
        {
            idx += 1;
            PlayerStatistics.Add(new AgentLog(idx, info.Agent._skillList.Count));
            PlayerMovements.Add(new List<MovementLog>());
        }

        idx = 0;
        foreach (var info in EnemyList)
        {
            idx += 1;
            EnemyStatistics.Add(new AgentLog(idx, info.Agent._skillList.Count));
            EnemyMovements.Add(new List<MovementLog>());
        }

        BossMaxHealth = new List<int>();
        BossCurrentHealth = new List<int>();

        for (int i = 0; i < m_EnemyList.Count; i++)
        {
            BossMaxHealth.Add(new int());
            BossCurrentHealth.Add(new int());
        }



    }

    public void AddCombatLog(CombatLog log)
    {
        if (log.source.GetType() == typeof(RaidPlayerAgent))
        {
            PlayerCombatLogs.Add(log);
        }
        else if (log.source.GetType() == typeof(EnemyAgent))
        {
            EnemyCombatLogs.Add(log);
        }
    }

    public void AddSkillTrigLog(CombatLog log)
    {
        int srcAgentIdx;
        int srcSkillIdx;

        if (log.source.GetType() == typeof(RaidPlayerAgent))
        {

            srcAgentIdx = m_AgentsList.FindIndex(info => info.Agent == log.source);
            srcSkillIdx = m_AgentsList[srcAgentIdx].Agent._skillList.FindIndex(item => item.info.uuid == log.skill.info.uuid);

            if (srcAgentIdx != -1 && srcSkillIdx != -1)
            {
                PlayerStatistics[srcAgentIdx].SkillTrigCounts[srcSkillIdx] += 1;
                PlayerStatistics[srcAgentIdx].AddSkillLog(log);
            }

        }
        else if (log.source.GetType() == typeof(EnemyAgent))
        {

            srcAgentIdx = m_EnemyList.FindIndex(info => info.Agent == log.source);
            srcSkillIdx = m_EnemyList[srcAgentIdx].Agent._skillList.FindIndex(item => item.info.uuid == log.skill.info.uuid);

            if (srcAgentIdx != -1 && srcSkillIdx != -1)
            {
                EnemyStatistics[srcAgentIdx].SkillTrigCounts[srcSkillIdx] += 1;
                EnemyStatistics[srcAgentIdx].AddSkillLog(log);
            }
        }

    }



    public float[] GetSkillDealtDamage()
    {
        /**
        에피소드에 발생한 스킬의 데미지 총 양을 측정함
        **/
        const int NUM_SKILLS = 4;
        float[] meanDealtDamage = new float[NUM_SKILLS];  // 4 is the number of skills;

        foreach (AgentLog al in PlayerStatistics)
        {
            for (int i = 0; i < NUM_SKILLS; i++)
            {
                meanDealtDamage[i] += al.DealtDamage[i];
            }
        }

        // Episode길이로 Normalize
        for (int i = 0; i < NUM_SKILLS; i++)
        {
            meanDealtDamage[i] /= PlayerStatistics.Count;
            meanDealtDamage[i] /= EpisodeLength;
        }

        return meanDealtDamage;
    }


    public override string ToString()
    {
        string output = "";

        output += "\n";
        output += "[Player Team]\n";
        foreach (var al in PlayerStatistics)
        {
            output += al.ToString() + "\n"; ;
        }

        output += "\n";
        output += "[Enemy Team]\n";
        foreach (var al in EnemyStatistics)
        {
            output += al.ToString() + "\n";
        }

        return output;
    }



}

public enum GameResult : ushort
{
    Draw = 0,
    PlayerWin = 1,
    EnemyWin = 2,

    TimeLimit = 10
}