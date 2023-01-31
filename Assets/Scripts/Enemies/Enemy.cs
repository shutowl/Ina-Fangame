using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum enemyState
    {
        moving,
        jumping,
        attacking,
        hurt,
        dying,  //for death animation
        dead,
        idle
    }
    public enemyState currentState;

    public bool grounded;
    public int maxHealth = 30;
    [SerializeField] private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    //damage take with no knockback/flinch
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            currentState = enemyState.dying;
        }

    }

    public int getCurrentHealth()
    {
        return currentHealth;
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player Bullet") && currentState != enemyState.dead)
        {
            int damage = col.GetComponent<aoBullet>().damage;
            TakeDamage(damage);
        }
    }
}

