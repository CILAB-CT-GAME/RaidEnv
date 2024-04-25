using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public List<string> presetNames;
    public int numOfObject = 7;
    public GameObject defaultProjectile;
    public Dictionary<string, Dictionary<string, Queue<GameObject>>> agentQueue { get; private set; }

    void Awake()
    {
        agentQueue = new Dictionary<string, Dictionary<string, Queue<GameObject>>>();
    }

    public void Initialize(GameObject agent)
    {
        if (Common.FindChildWithName(transform, agent.name) != null) return;
        RegisterGroup(agent);
        CreateSkillObjects(agent);
    }

    public void RegisterGroup(GameObject agent)
    {
        var go = new GameObject(agent.name);
        go.transform.parent = transform;
    }

    void CreateSkillObjects(GameObject agent)
    {
        var _agentQueue = new Dictionary<string, Queue<GameObject>>();
        foreach (var skill in agent.GetComponent<AbstractAgent>()._skillList)
        {
            var objQueue = new Queue<GameObject>();
            for (int i = 0; i < numOfObject; i++)
            {
                if (skill.info.name == "BlackHole") continue;
                var go = CreateObject(skill);
                go.name = skill.info.name + "_" + i.ToString();
                go.tag = "projectile";
                go.layer = LayerMask.NameToLayer("Projectile");
                go.transform.parent = Common.FindChildWithName(transform, agent.name).transform;
                objQueue.Enqueue(go);
            }
            _agentQueue[skill.info.name] = objQueue;
        }
        agentQueue[agent.name] = _agentQueue;
    }

    GameObject CreateObject(AbstractSkill skill)
    {
        GameObject go;
        switch (skill.projectileFX.type)
        {
            case ProjectileType.Beam:
                {
                    go = null;
                    break;
                }
            case ProjectileType.PillarBlast:
                {
                    go = Instantiate(defaultProjectile, transform.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f))) as GameObject;
                    var sc = go.AddComponent<SphereCollider>();
                    sc.isTrigger = true;
                    sc.radius = 3.0f;
                    break;
                }
            case ProjectileType.Slash:
                {
                    go = Instantiate(defaultProjectile, transform.position, Quaternion.LookRotation(transform.parent.transform.forward)) as GameObject;
                    var sc = go.AddComponent<SphereCollider>();
                    sc.isTrigger = true;
                    sc.radius = skill.condition.range / 2.5f;
                    break;
                }
            case ProjectileType.Missile:
                {
                    go = Instantiate(defaultProjectile, transform.position, transform.rotation) as GameObject;
                    var sc = go.AddComponent<SphereCollider>();
                    sc.isTrigger = true;
                    sc.radius = 0.2f;
                    break;
                }
            default:
                {
                    go = Instantiate(defaultProjectile, transform.position, transform.rotation);
                    var sc = go.AddComponent<SphereCollider>();
                    sc.isTrigger = true;
                    sc.radius = 0.5f;
                    break;
                }
        }

        var rb = go.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        go.SetActive(false);
        // go.AddComponent<Projectile>();

        return go;
    }

    public void Retrieve(string agentName, string skillName, GameObject go)
    {
        go.SetActive(false);
        agentQueue[agentName][skillName].Enqueue(go);
    }
}
