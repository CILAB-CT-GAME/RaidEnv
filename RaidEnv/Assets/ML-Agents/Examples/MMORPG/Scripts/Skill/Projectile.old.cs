// using System.Collections;
// using System.Collections.Generic;

// using UnityEngine;
// using Unity.MLAgents;
// using Unity.MLAgents.Policies;
// using Unity.MLAgents.Sensors;
// using Unity.MLAgents.Actuators;


// public class Projectile : MonoBehaviour
// {
//     const string SKILL_FX_DIR = "SkillEffects";

//     public ProjectileType projectileType;
//     public GameObject projectileParticle;
//     public GameObject impactParticle;
//     public GameObject muzzleParticle;
//     public GameObject[] trailParticles;
//     public Vector3 impactNormal; //Used to rotate impactparticle.

//     private AbstractSkill _skill;
//     private AbstractSkill.TerminalCondition _terminalCondition;
//     private Vector3 _castPos;
//     private Vector3 _castFowardDirection;
//     private float[] destroyingTime = { 0, 0, 0.25f, 0.4f };

//     private GameObject _source;
//     private GameObject _target;
//     public Dictionary<string, Object> presets;
//     Rigidbody m_Rigidbody;
//     SphereCollider m_collider;

//     void Awake()
//     {
//         presets = new Dictionary<string, Object>();

//         foreach (GameObject go in Resources.LoadAll(SKILL_FX_DIR, typeof(GameObject)))
//         {
//             presets.Add(go.name, go);
//         }
//         m_collider = GetComponent<SphereCollider>();
//         m_Rigidbody = GetComponent<Rigidbody>();
//         m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
//     }

//     // Start is called before the first frame update
//     void Start()
//     {
//         GenerateProjetile();
//     }

//     void FixedUpdate()
//     {
//         TravelTo(_target);
//         // m_Rigidbody.AddForce(_target.transform.position * 100);
//     }

//     void OnTriggerEnter(Collider col)
//     {
//         switch (col.gameObject.tag)
//         {
//             case "ground":
//                 break;
//             case "wall":
//                 if(_skill.projectileFX.type == ProjectileType.Missile)
//                     DestroyProjectile();
//                 break;
//             case "agent":
//                 if (_skill.OnHit(_source, col.gameObject, this)) _terminalCondition.hitCount--;
//                 break;
//             case "enemy":
//                 if (_skill.OnHit(_source, col.gameObject, this)) _terminalCondition.hitCount--;
//                 break;
//             case "untagged":
//                 break;
//             default:
//                 break;
//         }
//         if (CheckTerminal()) DestroyProjectile();
//     }

//     public void SetProjectileInfo(AbstractSkill skill, GameObject source, GameObject target = null)
//     {
//         _skill = skill;
//         _castFowardDirection = source.transform.forward;
//         _terminalCondition = _skill.terminalCondition;
//         _source = source;
//         _target = target;
//         SetProjectileFX();
//         presets = null; // to memory free
//     }

//     public void SetProjectileFX()
//     {
//         projectileParticle = presets[_skill.projectileFX.projectileParticleName] as GameObject;
//         if (_skill.projectileFX.impactParticleName != null)
//             impactParticle = presets[_skill.projectileFX.impactParticleName] as GameObject;
//         if (_skill.projectileFX.muzzleParticleName != null)
//             muzzleParticle = presets[_skill.projectileFX.muzzleParticleName] as GameObject;
//     }

//     private void GenerateProjetile()
//     {
//         switch (_skill.projectileFX.type)
//         {
//             case ProjectileType.Missile:
//                 m_collider.radius = 0.5f;
//                 projectileParticle = Instantiate(projectileParticle, transform.position, transform.rotation) as GameObject;
//                 projectileParticle.transform.parent = transform.parent;
//                 projectileParticle.transform.parent = transform;
//                 if (muzzleParticle)
//                 {
//                     muzzleParticle = Instantiate(muzzleParticle, transform.position, transform.rotation) as GameObject;
//                     muzzleParticle.transform.parent = transform.parent;
//                     muzzleParticle.transform.parent = transform;
//                     Destroy(muzzleParticle, 1.5f); // Lifetime of muzzle effect.
//                 }
//                 break;

//             case ProjectileType.Beam:

//                 break;

//             case ProjectileType.PillarBlast:
//                 m_collider.radius = 3.0f;
//                 projectileParticle = Instantiate(projectileParticle, transform.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f))) as GameObject;
//                 projectileParticle.transform.parent = transform.parent;
//                 projectileParticle.transform.parent = transform;
//                 Destroy(projectileParticle, destroyingTime[(int)_skill.projectileFX.type]);
//                 Destroy(gameObject, destroyingTime[(int)_skill.projectileFX.type]);
//                 break;

//             default:
//                 m_collider.radius = _skill.condition.range/2.5f;
//                 this.transform.position = new Vector3(transform.position.x/2, 0, transform.position.z/2) + new Vector3(_target.transform.position.x/2, 0, _target.transform.position.z/2) + new Vector3(0,_source.transform.position.y,0);
//                 projectileParticle = Instantiate(projectileParticle, transform.position, Quaternion.LookRotation(_source.transform.forward)) as GameObject;
//                 projectileParticle.transform.parent = transform.parent;
//                 projectileParticle.transform.parent = transform;
//                 Destroy(projectileParticle, destroyingTime[(int)_skill.projectileFX.type]);
//                 Destroy(gameObject, destroyingTime[(int)_skill.projectileFX.type]);
//                 break;
//         }
//     }

//     public void TravelTo(GameObject target)
//     {
//         Vector3 direction = new Vector3();
//         switch (_skill.info.targetType)
//         {
//             case TargetType.Target:
//                 direction = (target.transform.position - this.transform.position).normalized;
//                 break;
//             case TargetType.NonTarget:
//                 direction = _castFowardDirection;
//                 break;
//             case TargetType.Region:
//                 break;
//             default:
//                 break;
//         }

//         switch (_skill.projectileFX.type)
//         {
//             case ProjectileType.Missile:
//                 this.gameObject.transform.position += Vector3.Scale(direction, new Vector3(_skill.info.projectileSpeed, 1, _skill.info.projectileSpeed)) * Time.deltaTime;
//                 break;
//             case ProjectileType.Slash:
//                 // this.gameObject.transform.position = Vector3.Scale(direction, new Vector3(_skill.condition.range*10, 1, _skill.condition.range*10)) * 0.6f;
//                 break;
//             case ProjectileType.PillarBlast:
//                 this.gameObject.transform.position = target.transform.position;
//                 break;
//         }        
//     }

//     private bool CheckTerminal()
//     {
//         if (_terminalCondition.hitCount > 0) return false;
//         return true;
//     }

//     public void DestroyProjectile()
//     {
//         switch (_skill.projectileFX.type)
//         {
//             case ProjectileType.Missile:
//                 impactParticle = Instantiate(impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal)) as GameObject;
//                 impactParticle.transform.parent = transform.parent;

//                 // foreach (GameObject trail in trailParticles)
//                 // {
//                 //     GameObject curTrail = transform.Find(projectileParticle.name + "/" + trail.name).gameObject;
//                 //     curTrail.transform.parent = null;
//                 //     Destroy(curTrail, 3f);
//                 // }
//                 Destroy(projectileParticle, 3f);
//                 Destroy(impactParticle, 2.5f);
//                 Destroy(muzzleParticle, 1.5f); // Lifetime of muzzle effect.
//                 Destroy(gameObject);
//                 break;
//             default:
//                 Destroy(projectileParticle, destroyingTime[(int)_skill.projectileFX.type]);
//                 Destroy(gameObject, destroyingTime[(int)_skill.projectileFX.type]);
//                 break;
//         }
//     }
// }
