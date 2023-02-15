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
    private float hitstunTimer = 0f;
    public bool stunned = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Update()
    {
        if(hitstunTimer > 0)
        {
            GetComponent<Hazard>().enabled = false;
            hitstunTimer -= Time.deltaTime;
        }
        else
        {
            stunned = false;
            GetComponent<Hazard>().enabled = true;
        }
    }

    //damage taken with no knockback
    public void TakeDamage(int damage, float hitstun)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            currentState = enemyState.dying;
        }
        else
        {
            hitstunTimer = hitstun;
            stunned = true;
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
            TakeDamage(damage, 0f);
        }
    }
}

