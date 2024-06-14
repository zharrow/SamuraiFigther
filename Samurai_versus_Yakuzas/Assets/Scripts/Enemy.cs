using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 10;
    
    int _currentHealth;

    public Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = maxHealth;
        DetectPlayer();
    }

    void DetectPlayer()
    {
        
    }
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
        
        animator.SetTrigger("Hurt");
        
        if (_currentHealth <= 0)
        {
            Die();
        }

        Invoke("Recover", 10);
    }

    void Die()
    {
        Debug.Log("Enemy Died!");
        animator.SetBool("Death", true);

        GetComponent<BoxCollider2D>().enabled = false;
        this.enabled = false;
    }

    void Recover()
    {
        animator.SetTrigger("Recover");
        _currentHealth = maxHealth;
        Debug.Log("Enemy Health Restored");
    }
}
