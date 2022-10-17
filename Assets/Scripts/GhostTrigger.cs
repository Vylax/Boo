using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            GetComponentInParent<Ghost>().Attack();
        }else if (other.gameObject.tag == "ShockWave")
        {
            if(GetComponentInParent<Ghost>().spawnTime < other.gameObject.GetComponent<ShockWave>().spawnTime)
            {
                GetComponentInParent<Ghost>().Die();
            }
        }
    }
}
