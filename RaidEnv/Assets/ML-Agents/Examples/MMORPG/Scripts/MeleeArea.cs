using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeArea : MonoBehaviour
{

    public int damage = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
       
         if (other.gameObject.tag == "agent")
        {
            EnemyAgent agentScript = other.gameObject.GetComponent<EnemyAgent>();
            if (agentScript != null)
            {
                //print("Attacked!");
                //agentScript.OnAttacked(this);
            }

            //Destroy(gameObject); ���� ���� ����

        }
    }
}
