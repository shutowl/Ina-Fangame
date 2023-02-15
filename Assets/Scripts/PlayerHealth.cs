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
        healthText.text = "Health: " + Mathf.Clamp(curHealth, 0, maxHealth);
    }

    // Update is called once per frame
    void Update()
    {

        if (Keyboard.current.digit1Key.wasPressedThisFrame)    //Debug: Press 1 to take damage
        {
            damage(20, 0.5f);
        }
        
        if (Keyboard.current.digit2Key.wasPressedThisFrame)         //Debug: Press 2 to go back to max health
        {
            fullHeal();
        }
    }

    public void damage(int damage, float hitstun)
    {
        curHealth -= damage;

        if (curHealth <= 0)
        {
            healthText.text = "Health: Passed out";
            player.currentState = PlayerMovement.playerState.dead;
        }
        else
        {
            healthText.text = "Health: " + Mathf.Clamp(curHealth, 0, maxHealth);
            player.setDamageState(hitstun);
        }
    }

    public void fullHeal()
    {
        curHealth = maxHealth;
        healthText.text = "Health: " + Mathf.Clamp(curHealth, 0, maxHealth);
    }
}
