using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Projectile : MonoBehaviour
{
    private AbstractSkill _skill;

    private Vector3 _direction;
    private Vector3 _fowardDirection;

    // private float[] destroyingTime = { 0, 0, 0.25f, 0.4f };

    private GameObject _source;
    private GameObject _target;

    private int hitCount;
    private int elapseTick;
    private const int ForceDestroyTick = 5;

    void OnEnable()
    {
        hitCount = 0;
        elapseTick = 0;
        switch (_skill.projectileFX.type)
        {
            case ProjectileType.Missile:
                {
                    break;
                }
            case ProjectileType.Slash:
                {
                    transform.position = new Vector3(transform.position.x / 2, 0, transform.position.z / 2) + new Vector3(_target.transform.position.x / 2, 0, _target.transform.position.z / 2) + new Vector3(0, _source.transform.position.y, 0);
                    transform.rotation = Quaternion.LookRotation(_source.transform.forward);
                    break;
                }
            case ProjectileType.Beam:
                {
                    break;
                }
            case ProjectileType.PillarBlast:
                {
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    void FixedUpdate()
    {
        switch (_skill.projectileFX.type)
        {
            case ProjectileType.Missile:
                TravelTo(_target);
                break;
            case ProjectileType.Slash:
                elapseTick += 1;
                break;
            default:
                break;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        switch (col.gameObject.tag)
        {
            case "ground":
                if (_skill.projectileFX.type == ProjectileType.Slash)
                    Deactivate();
                break;
            case "wall":
                // Debug.Log("IsNull"+_skill);
                if (_skill.projectileFX.type == ProjectileType.Missile)
                    Deactivate();
                break;
            case "agent":
                if (_skill.OnHit(_source, col.gameObject, this))
                    Deactivate();
                    hitCount++;
                break;
            case "enemy":
                if (_skill.OnHit(_source, col.gameObject, this))
                    Deactivate();
                    hitCount++;
                break;
            case "untagged":
                break;
            default:
                break;
        }
    }

    bool CheckTerminal()
    {
        if (_skill.projectileFX.type == ProjectileType.Slash && ForceDestroyTick < elapseTick) return true;
        if (_skill.terminalCondition.hitCount <= hitCount) return true;

        return false;
    }

    public void SetSkill(AbstractSkill skill)
    {
        _skill = skill;
    }

    public void SetActors(GameObject source, GameObject target)
    {
        // _castTime = Time.fixedTime;
        _source = source;
        _target = target;

        _fowardDirection = new Vector3(_source.transform.forward.x, _source.transform.forward.y, _source.transform.forward.z);
    }

    public void TravelTo(GameObject target)
    {
        _direction = new Vector3();
        switch (_skill.info.targetType)
        {
            case TargetType.Target:
                {
                    _direction = (target.transform.position - gameObject.transform.position).normalized;
                    break;
                }
            case TargetType.NonTarget:
                {
                    _direction = _fowardDirection;
                    break;
                }
            case TargetType.Region:
                {
                    break;
                }
            default:
                {
                    break;
                }
        }

        switch (_skill.projectileFX.type)
        {
            case ProjectileType.Missile:
                {
                    transform.position += Vector3.Scale(_direction, new Vector3(_skill.info.projectileSpeed, 1, _skill.info.projectileSpeed)) * Time.fixedDeltaTime;
                    transform.rotation = Quaternion.Inverse(transform.parent.rotation);
                    break;
                }
            case ProjectileType.Slash:
                {
                    break;
                }
            case ProjectileType.Beam:
                {
                    break;
                }
            case ProjectileType.PillarBlast:
                {
                    gameObject.transform.position = target.transform.position;
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    // IEnumerator WaitFor(int numFixedUpdate = 0)
    // {
    //     for (int i = 0; i < numFixedUpdate; i++)
    //     {
    //         yield return new WaitForFixedUpdate();
    //     }
    // }

    public void Deactivate(int numFixedUpdate = 0)
    {
        // StartCoroutine(WaitFor(numFixedUpdate));
        var projectileManager = _source.GetComponent<AbstractAgent>().m_ProjectileManager;
        projectileManager.Retrieve(_source.name, _skill.info.name, gameObject);
    }
}
