using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordMotion : MonoBehaviour
{   
    public List<GameObject> FoundObjects;
    public Animator anim;
    public float AttackDistance;
    // Start is called before the first frame update
    void Start()
    {
        anim.SetBool("Click", false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetMouseButtonDown(0)){
            anim.Play("rotate");
            FoundObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("enemy"));
            foreach (GameObject found in FoundObjects){
                float Distance = Vector3.Distance(gameObject.transform.position, found.transform.position);
                if(Distance <= AttackDistance){
                    Debug.Log("근접 공격");
                }
            }
        }
        
        
    }

}
