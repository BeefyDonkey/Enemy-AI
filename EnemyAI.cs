using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    Transform target;
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float turnSpeed = 5f;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    [SerializeField] CapsuleCollider enemyCollider;
    [SerializeField] BoxCollider boxCollider;

    float distanceToTarget = Mathf.Infinity;

    bool isProvoked = false;
    bool isDead = false;

    [SerializeField] bool isPatrolling;
    private PatrolAI patrolAI;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] zombieSounds;
    [SerializeField] float zombieSoundWaitTime = 5f;
    bool isPlayingSounds = false;
    bool hasFirstSoundPlayed = false;

    [SerializeField] bool isBoss = false;

    void Start()
    {
        target = FindObjectOfType<PlayerHealth>().gameObject.transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (isPatrolling)
        {
            patrolAI = GetComponent<PatrolAI>();
        }
    }

    void Update()
    {
        if (isDead) return;

        distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (isProvoked)
        {
            EngageTarget();
        }
        else if (distanceToTarget <= chaseRange)
        {
            isProvoked = true;
        }
        else if (isPatrolling)
        {
            patrolAI.ManagePatrolAI();
        }
    }

    private void OnDamageTaken()
    {
        isProvoked = true;
    }

    public void OnEnemyDeath()
    {
        isDead = true;
        navMeshAgent.SetDestination(gameObject.transform.position);
        enemyCollider.enabled = false;
        boxCollider.enabled = false;
        StopAllCoroutines();
    }

    private void EngageTarget()
    {
        if (isDead) return;

        isPatrolling = false;

        FaceTarget();

        if (distanceToTarget >= navMeshAgent.stoppingDistance)
        {
            if (isBoss)
            {
                BossChaseTarget();
            }
            else
            {
                ChaseTarget();
            }
        }

        if (distanceToTarget <= navMeshAgent.stoppingDistance)
        {
            if (isBoss)
            {
                BossAttackTarget();
            }
            else
            {
                AttackTarget();
            }            
        }
    }

    private void ChaseTarget()
    {
        zombieSoundWaitTime = 5f;
        ManageZombieSounds();
        animator.SetBool("Attack", false);
        animator.SetTrigger("Move");
        navMeshAgent.SetDestination(target.position);
    }

    private void BossChaseTarget()
    {
        zombieSoundWaitTime = 5f;
        ManageZombieSounds();
        DisableBossAttackAnimations();
        animator.SetTrigger("Move");
        navMeshAgent.SetDestination(target.position);
    }
    
    private void DisableBossAttackAnimations()
    {
        animator.SetBool("Attack1", false);
        animator.SetBool("Attack2", false);
        animator.SetBool("Attack3", false);
    }

    private void AttackTarget()
    {
        zombieSoundWaitTime = 2f;
        ManageZombieSounds();
        animator.SetBool("Attack", true);
    }

    private void BossAttackTarget()
    {
        DisableBossAttackAnimations();
        zombieSoundWaitTime = 2f;
        ManageZombieSounds();

        int randomIndex = Random.Range(1, 4);
        string attackName = "Attack" + randomIndex;
        animator.SetBool(attackName, true);
    }

    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    private void ManageZombieSounds()
    {
        if (!hasFirstSoundPlayed)
        {
            int randomIndex = Random.Range(0, zombieSounds.Length);
            audioSource.PlayOneShot(zombieSounds[randomIndex]);
            hasFirstSoundPlayed = true;
        }

        if (!audioSource.isPlaying && !isPlayingSounds)
        {
            StartCoroutine(PlayRandomZombieSound());
            isPlayingSounds = true;
        }
    }

    IEnumerator PlayRandomZombieSound()
    {
        yield return new WaitForSeconds(zombieSoundWaitTime);
        int randomIndex = Random.Range(0, zombieSounds.Length);
        audioSource.PlayOneShot(zombieSounds[randomIndex]);
        isPlayingSounds = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
