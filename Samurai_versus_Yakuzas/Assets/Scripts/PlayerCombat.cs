using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;

    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayers;

    public int attackDamage = 1;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Attack"))
        {
            Attack();
        }
    }

    void Attack()
    {
        // Play attack animation
        animator.SetTrigger("Attack");

        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // if (hitEnemies.Length > 0)
        // {
        //     Debug.Log("Enemies detected: " + hitEnemies.Length);
        // }
        // else
        // {
        //     Debug.Log("No enemies detected");
        // }

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Player attack hit: " + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
            // Here you can add code to deal damage to the enemy
            // e.g. enemy.GetComponent<Enemy>().TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}