using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolAI : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    [SerializeField] Transform patrol_1;
    [SerializeField] Transform patrol_2;
    private bool isPatrolOneReached = false;
    private bool isPatrolTwoReached = false;

    bool isMoving = false;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    public void ManagePatrolAI()
    {
        if (!isPatrolOneReached)
        {
            PatrolToTarget(patrol_1, ref isPatrolOneReached);
        }
        else if (!isPatrolTwoReached)
        {
            isMoving = false;
            PatrolToTarget(patrol_2, ref isPatrolTwoReached);
        }
        else
        {
            isPatrolOneReached = false;
            isPatrolTwoReached = false;
            isMoving = false;
        }
    }

    private void PatrolToTarget(Transform target, ref bool isPatrolReached)
    {
        float distanceToTraget = Vector3.Distance(transform.position, target.position);
        navMeshAgent.SetDestination(target.position);

        if (!isMoving)
        {
            animator.SetTrigger("Move");
            isMoving = true;
        }

        if (distanceToTraget <= 3)
        {
            navMeshAgent.SetDestination(transform.position);
            animator.SetTrigger("Idle");
            isPatrolReached = true;
        }
    }
}
