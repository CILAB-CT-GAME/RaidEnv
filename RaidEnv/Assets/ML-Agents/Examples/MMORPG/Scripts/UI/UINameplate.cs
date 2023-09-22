using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;


public class UINameplate : MonoBehaviour
{
    public Image HPBar;
    public Image MPBar;
    public Image CastBar;
    public Image CastBarBG;
    private AbstractAgent agent;
    private AbstractStatus status;
    private Camera mainCamera;

    void Start()
    {
        agent = gameObject.transform.parent.GetComponent<AbstractAgent>();
        status = agent._status;

        mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();
    }

    void Update()
    {
        LookAtCamera();
        locateAbove();
        UpdateBar();
    }

    void LookAtCamera()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.back, mainCamera.transform.rotation * Vector3.up);
    }

    void locateAbove()
    {
        Vector3 pos6 = Camera.main.WorldToViewportPoint(transform.parent.gameObject.transform.position);
        Vector3 pos7 = Camera.main.ViewportToWorldPoint(pos6 + new Vector3(0, 0.07f, 0));
        transform.position = pos7;
    }

    void UpdateBar()
    {
        HPBar.fillAmount = (float)status.health.current / status.health.max;
        MPBar.fillAmount = (float)status.mana.current / status.mana.max;

        if (agent.isCasting)
        {
            CastBar.gameObject.SetActive(true);
            CastBarBG.gameObject.SetActive(true);
            CastBar.fillAmount = agent.remainingCastTimeRatio;
        }
        else
        {
            CastBar.gameObject.SetActive(false);
            CastBarBG.gameObject.SetActive(false);
        }
    }
}