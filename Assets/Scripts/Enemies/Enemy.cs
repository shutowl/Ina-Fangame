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
    public bool isBoss;
    public int maxHealth = 30;
    private float scale = 1f;
    protected int direction;
    [SerializeField] private int currentHealth;
    private float hitstunTimer = 0f;
    public float ghostDuration = 1f;                //Determines how long the enemy contact hitbox is disabled for (ex: the Hazard script)
    private float ghostTimer = 0f;                  //Named "ghost" since the player should be able to pass through enemies
    public bool stunned = false;
    public string enemyName;
    public int difficulty = 1;
    protected ComboMeter comboMeter;
    private Rigidbody2D rbBase;
    protected BossHealthBar bossHealthBar;
    public GameObject hitNumber;

    public void Start()
    {
        scale = transform.localScale.x;
        direction = -1; //start facing left;
        currentHealth = maxHealth;
        comboMeter = FindObjectOfType<ComboMeter>();
        bossHealthBar = FindObjectOfType<BossHealthBar>();
        rbBase = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        if(direction == -1)
        {
            transform.localScale = new Vector2(-scale, scale);
        }
        else
        {
            transform.localScale = new Vector2(scale, scale);
        }

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
            GetComponent<Hazard>().SetActive(false);
        }
        else
        {
            GetComponent<Hazard>().SetActive(true);
        }
    }

    //damage taken (includes hitstun)
    public void TakeDamage(int damage, float hitstun)
    {
        int totalDamage = damage + (int)(damage * comboMeter.GetBonusDMG() / 100);
        currentHealth -= totalDamage;

        if (currentHealth <= 0)
        {
            GetComponent<Hazard>().SetActive(false);
            rbBase.constraints = RigidbodyConstraints2D.FreezeRotation;
            currentState = enemyState.dying;
        }
        else
        {
            hitstunTimer = hitstun;
            ghostTimer = ghostDuration;
        }

        comboMeter.AddCombo();
        if (isBoss) bossHealthBar.SetHP(currentHealth);

        if(PlayerPrefs.GetInt("hitNumbersOn") == 1)
        {
            GameObject text = Instantiate(hitNumber, transform.position, Quaternion.identity);
            text.GetComponent<HitNumber>().SetText("" + totalDamage);
            text.GetComponent<HitNumber>().SetSize(totalDamage);
        }
    }

    //damage taken (no hitstun)
    public void TakeDamageNoStun(int damage)
    {
        int totalDamage = damage + (int)(damage * comboMeter.GetBonusDMG() / 100);
        currentHealth -= totalDamage;

        if (currentHealth <= 0)
        {
            GetComponent<Hazard>().SetActive(false);
            rbBase.constraints = RigidbodyConstraints2D.FreezeRotation;
            currentState = enemyState.dying;
        }

        comboMeter.AddCombo();
        if (isBoss) bossHealthBar.SetHP(currentHealth);

        if (PlayerPrefs.GetInt("hitNumbersOn") == 1)
        {
            GameObject text = Instantiate(hitNumber, transform.position, Quaternion.identity);
            text.GetComponent<HitNumber>().SetText("" + totalDamage);
            text.GetComponent<HitNumber>().SetSize(totalDamage);
        }
    }

    public int GetCurrentHealth()
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
                Destroy(col.gameObject);
            }
        }
    }

    private void OnEnable()
    {
        //Debug.Log(PlayerPrefs.GetInt("difficulty"));
        switch (PlayerPrefs.GetInt("difficulty"))
        {
            case 0: //Casual
                difficulty = 1;
                break;
            case 1: //Normal
                difficulty = 30;
                break;
            case 2: //Hard
                difficulty = 80;
                break;
            case 3: //Impossible
                difficulty = 200;
                break;
        }
    }
}

