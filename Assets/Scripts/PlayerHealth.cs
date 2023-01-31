using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    private int curHealth;
    private PlayerMovement player;

    public TextMeshProUGUI healthText;

    // Start is called before the first frame update
    void Start()
    {
        player = gameObject.GetComponent<PlayerMovement>();
        curHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = "Health: " + curHealth;

        if(curHealth <= 0)
        {
            healthText.text = "Health: Passed out";
            player.currentState = PlayerMovement.playerState.dead;
        }
        else if (Keyboard.current.digit1Key.wasPressedThisFrame)    //Debug: Press 1 to take damage
        {
            damage(10);
        }
        
        if (Keyboard.current.digit2Key.wasPressedThisFrame)         //Debug: Press 2 to go back to max health
        {
            curHealth = maxHealth;
        }
    }

    public void damage(int damage)
    {
        curHealth -= damage;
        player.setDamageState();
    }
}
