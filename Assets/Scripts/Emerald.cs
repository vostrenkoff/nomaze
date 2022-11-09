using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Emerald : MonoBehaviour
{
    
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider target)
    {

        if (target.tag == "Player")
        {
            Movement.num += 1;
            
            Destroy(gameObject);
        }
    }
}
