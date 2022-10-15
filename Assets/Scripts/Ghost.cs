using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ghost : MonoBehaviour
{
    public NavMeshAgent agent;

    public Vector3 target;

    private void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform.position;

        agent.SetDestination(target);
    }
}
