using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float hitPoints = 100f;
    private float maxHealth;
    [SerializeField] Animator animator;
    [SerializeField] EnemyAI enemyAI;

    private bool isAlive = true;
    [SerializeField] bool isBoss = false;
    BossSecondStage bossSecondStage;
    EndingTrigger endingTrigger;

    private void Start()
    {
        if (isBoss)
        {
            maxHealth = hitPoints;
            bossSecondStage = GetComponent<BossSecondStage>();
            endingTrigger = FindObjectOfType<EndingTrigger>();
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;

        hitPoints -= damage;
        if (isBoss)
        {
            if (hitPoints <= maxHealth / 2)
            {
                bossSecondStage.ActivateSecondStage();
            }
        }
        BroadcastMessage("OnDamageTaken");

        if (hitPoints <= 0)
        {
            if (isBoss)
            {
                endingTrigger.BossIsDead();
            }

            isAlive = false;
            enemyAI.OnEnemyDeath();
            animator.SetTrigger("Killed");
        }
    }
}
