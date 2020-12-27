using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    public NavMeshAgent agent;

    public Vector3 vTarget;

    private void Update()
    {
        if (Manager.manager.isPause)
        {
            return;
        }

        if (vTarget == Vector3.zero)
        {
            return;
        }

        if (Vector3.Distance(transform.position, vTarget) > 3)
        {
            agent.SetDestination(vTarget);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
