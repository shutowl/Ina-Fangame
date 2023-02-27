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
    [SerializeField] private float hitstunTimer = 0f;
    public float ghostDuration = 1f;                //Determines how long the enemy contact hitbox is disabled for (ex: the Hazard script)
    private float ghostTimer = 0f;                  //Named "ghost" since the player should be able to pass through enemies
    public bool stunned = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Update()
    {
        if(hitstunTimer > 0)
        {
            hitstunTimer -= Time.deltaTime;
            stunned = true;
        }
        else
        {
            stunned = false;
        }

        if(ghostTimer > 0)
        {
            ghostTimer -= Time.deltaTime;
            GetComponent<Hazard>().setActive(false);
        }
        else
        {
            GetComponent<Hazard>().setActive(true);
        }
    }

    //damage taken (includes hitstun)
    public void TakeDamage(int damage, float hitstun)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            GetComponent<Hazard>().setActive(false);
            currentState = enemyState.dying;
        }
        else
        {
            hitstunTimer = hitstun;
            ghostTimer = ghostDuration;
        }
    }

    //damage taken (no hitstun)
    public void TakeDamageNoStun(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
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

            if((int)col.GetComponent<aoBullet>().bulletLevel == 2) //laser
            {
                TakeDamage(damage, 0.5f);
            }
            else
            {
                TakeDamageNoStun(damage);
            }
            
        }
    }
}

