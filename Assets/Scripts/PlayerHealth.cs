using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    private int currentHealth;
    private PlayerMovement player;

    public TextMeshProUGUI healthText;
    public Slider HPSlider;
    public Slider delaySlider;
    public float delayDuration = 2f;
    private float delayTimer = 0f;
    private float timer = 0f;

    private ComboMeter comboMeter;

    void Start()
    {
        player = gameObject.GetComponent<PlayerMovement>();
        comboMeter = FindObjectOfType<ComboMeter>();
        currentHealth = maxHealth;
        healthText.text = currentHealth + "/" + maxHealth;
        SetMaxHealth(maxHealth);
        FullHeal();
    }

    void Update()
    {

        if (Keyboard.current.digit1Key.wasPressedThisFrame)    //Debug: Press 1 to take damage
        {
            Damage(20, 0.5f);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)         //Debug: Press 2 to go back to max health
        {
            FullHeal();
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)         //Debug: Press 3 to add 20 HP
        {
            SetMaxHealth(maxHealth + 20);
        }


        //Delay slider
        if (delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
            float t = timer / delayDuration;
            t = Mathf.Sin((t * Mathf.PI) / 2);                                      //https://easings.net/#easeOutSine
            delaySlider.value = Mathf.Lerp(delaySlider.value, currentHealth, t);
        }
    }

    public void Damage(int damage, float hitstun)
    {
        if(player.currentState != PlayerMovement.playerState.hitstun) currentHealth -= Mathf.Clamp(damage, 0, maxHealth);
        SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            player.currentState = PlayerMovement.playerState.dead;
        }
        else
        {
            player.setDamageState(hitstun);
            comboMeter.ResetCombo();
        }

        delayTimer = delayDuration;


    }

    public void FullHeal()
    {
        currentHealth = maxHealth;
        HPSlider.value = maxHealth;
        delaySlider.value = maxHealth;
        healthText.text = currentHealth + "/" + maxHealth;
    }

    public void SetHealth(int health)
    {
        currentHealth = health;
        HPSlider.value = health;
        healthText.text = currentHealth + "/" + maxHealth;

        if(health > delaySlider.value)
        {
            delaySlider.value = health;
        }
    }

    //Sets a new max health
    public void SetMaxHealth(int health)
    {
        maxHealth = health;
        HPSlider.maxValue = health;
        healthText.text = currentHealth + "/" + maxHealth;

        delaySlider.maxValue = health;
    }
}
