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
        idle
    }
    public enemyState currentState;

    public bool grounded;
    public int maxHealth = 30;
    private int currentHealth;

}

