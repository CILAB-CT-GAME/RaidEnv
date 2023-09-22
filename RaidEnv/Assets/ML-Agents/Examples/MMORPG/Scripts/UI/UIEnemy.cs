using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIEnemy : MonoBehaviour
{
    public static UIEnemy Instance { get; private set; }

    public Image portrait;
    public Image HPBar;
    public TextMeshProUGUI HPText;

    public List<GameObject> enemyList;
    private GameObject _platformInstance;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Update()
    {
        enemyList = Common.FindChildWithTag(_platformInstance.transform, "enemy");

        UpdateStatus();
    }

    public void OnAwake()
    {
        enemyList = Common.FindChildWithTag(_platformInstance.transform, "enemy");
    }

    public void SetPlatform(GameObject platform)
    {
        _platformInstance = platform;
    }

    void UpdateStatus()
    {
        var enemy = enemyList[0];
        var status = enemy.GetComponent<AbstractAgent>()._status;

        HPBar.fillAmount = (float)status.health.current / status.health.max;
        HPText.text = (status.health.current).ToString() + " / " + (status.health.max).ToString();
    }
}
