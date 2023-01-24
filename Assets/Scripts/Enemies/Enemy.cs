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
        idle
    }
    public enemyState currentState;

    public bool grounded;
    public int maxHealth = 30;
    private int currentHealth;

}
