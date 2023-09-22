using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private Dictionary<int, GameObject> instanceMap;
    private int firstInstanceID = int.MaxValue;
    private int currentInstanceID = int.MaxValue;
    private int previousInstanceID = int.MaxValue;

    public GameObject agentStatusUI;
    public GameObject agentGroupUI;
    public GameObject enemyStatusUI;

    private UIAgent m_UIAgent;
    private UIAgentGroup m_UIAgentGroup;
    private UIEnemy m_UIEnemy;


    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        instanceMap = new Dictionary<int, GameObject>();
        foreach (var go in GameObject.FindGameObjectsWithTag("platform"))
        {
            if (firstInstanceID == int.MaxValue)
                previousInstanceID = currentInstanceID = firstInstanceID = go.GetInstanceID();
            instanceMap[go.GetInstanceID()] = go;
        }

        m_UIAgent = agentStatusUI.GetComponent<UIAgent>();
        m_UIAgentGroup = agentGroupUI.GetComponent<UIAgentGroup>();
        m_UIEnemy = enemyStatusUI.GetComponent<UIEnemy>();

        m_UIAgent.SetPlatform(instanceMap[currentInstanceID]);
        m_UIAgentGroup.SetPlatform(instanceMap[currentInstanceID]);
        m_UIEnemy.SetPlatform(instanceMap[currentInstanceID]);

        m_UIAgent.OnAwake();
        m_UIAgentGroup.OnAwake();
        m_UIEnemy.OnAwake();
    }

    void Update()
    {
        EventListener();

        if (currentInstanceID != previousInstanceID)
        {
            m_UIAgent.SetPlatform(instanceMap[currentInstanceID]);
            m_UIAgentGroup.SetPlatform(instanceMap[currentInstanceID]);
            m_UIEnemy.SetPlatform(instanceMap[currentInstanceID]);
        }
    }

    // TODO
    // 1. 현재 선택한 에이전트로 변경
    // 2. 현재 선택한 에이전트가 속한 플랫폼 인스턴스로 변경
    void EventListener()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                previousInstanceID = currentInstanceID;
                currentInstanceID = hit.transform.parent.gameObject.GetInstanceID();
            }
        }
    }
}
