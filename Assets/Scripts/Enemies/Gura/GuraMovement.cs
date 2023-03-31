using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuraMovement : Enemy
{
    [Header("Enemy Specific Variables")]
    public float speed = 10f;
    public float jumpForce = 300f;
    public float minActionRate = 0.5f;
    public float maxActionRate = 2f;    // Performs an action every [1 to actionRate] seconds
    private int direction;
    private float rngCounter = 0f;
    private Vector2 centerPos = Vector2.zero;
    private float attackTimer;          // Used for attack timings (such as delays or charges)
    private int attackNum = 1;          // Determines which attack is used
    private int attackStep = 1;         // Current step of the current attack
    public int difficulty = 1;
    private bool overdrive = false;

    private Rigidbody2D rb;
    private GameObject player;
    public GameObject[] bullets;
    public GameObject healthBar;

    private bool bulletRainOn = false;
    public float bulletRainRate = 0.2f;
    private float bulletRainTimer = 0f;
    private Vector2 lastPosition;
    private float fireRateTimer = 0f;   // Used for spiral attack
    private float bulletOffset = 0f;    // Used for spiral attack

    void Start()
    {
        direction = -1; //start facing left;
        rb = GetComponent<Rigidbody2D>();
        enabled = false;
        currentState = enemyState.idle;
        centerPos = Camera.main.transform.position;
        player = GameObject.FindGameObjectWithTag("Player");

        bossHealthBar.SetBoss(this.gameObject, maxHealth, enemyName);
        bossHealthBar.SetBarColor(new Color(0.286f, 0.313f, 0.812f),
                                  new Color(0.600f, 0.907f, 1.000f),
                                  new Color(0.134f, 0.204f, 0.481f));
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        //-----IDLE STATE-----
        if(currentState == enemyState.idle)
        {
            if (grounded)
            {
                rb.velocity = Vector2.zero;

                if (rngCounter > 0)
                {
                    rngCounter -= Time.deltaTime;
                }
            }
        }

        // I'll try applying movement within the attacks this time
        /*//-----MOVING STATE-----
        else if(currentState == enemyState.moving)
        {

        }

        //-----JUMPING STATE-----
        else if (currentState == enemyState.jumping)
        {

        }*/

        //-----ATTACKING STATE-----
        else if (currentState == enemyState.attacking)
        {
            switch (attackNum)
            {
                //Attack 1: Jump past the player while shooting bullets downwards
                case 1:
                    if(attackStep == 1)
                    {
                        attackTimer = 1f;
                        attackStep = 2;
                    }
                    if(attackStep == 2)
                    {
                        attackTimer -= Time.deltaTime;

                        if(attackTimer <= 0)
                        {
                            rb.AddForce(new Vector2((Mathf.Abs(transform.position.x - player.transform.position.x)) * 60 * direction, jumpForce));
                            attackStep = 3;
                        }
                    }
                    if(attackStep == 3)
                    {
                        if(!grounded && rb.velocity.y < 0f)
                        {
                            rb.velocity = Vector2.zero;
                            rb.AddForce(new Vector2(100 * direction, 300));

                            GameObject bullet = Instantiate(bullets[1], transform.position, Quaternion.identity);
                            bullet.GetComponent<NormalBulletNoFollow>().speed = 7f;
                            bullet.GetComponent<NormalBulletNoFollow>().SetDirection(-1, -2);

                            bullet = Instantiate(bullets[1], transform.position, Quaternion.identity);
                            bullet.GetComponent<NormalBulletNoFollow>().speed = 7f;
                            bullet.GetComponent<NormalBulletNoFollow>().SetDirection(0, -2);

                            bullet = Instantiate(bullets[1], transform.position, Quaternion.identity);
                            bullet.GetComponent<NormalBulletNoFollow>().speed = 7f;
                            bullet.GetComponent<NormalBulletNoFollow>().SetDirection(1, -2);

                            currentState = enemyState.idle;
                        }
                    }
                    
                    break;

                //Attack 2: Lob a few projectiles at the player, when projectiles land, they create a damaging geyser from the floor
                case 2:
                    if(attackStep == 1)
                    {
                        attackTimer = 0.5f;
                        attackStep = 2;
                    }
                    if(attackStep == 2)
                    {
                        attackTimer -= Time.deltaTime;

                        if(attackTimer <= 0)
                        {
                            //Fire a few bullets towards player (Replace with geyser bullets later)
                            GameObject bullet = Instantiate(bullets[0], transform.position, Quaternion.identity);
                            bullet.GetComponent<PhysicsBullet>().SetForce(900f);
                            bullet.GetComponent<PhysicsBullet>().SetDirection(player.transform.position.x - transform.position.x, 20);

                            bullet = Instantiate(bullets[0], transform.position, Quaternion.identity);
                            bullet.GetComponent<PhysicsBullet>().SetForce(900f);
                            bullet.GetComponent<PhysicsBullet>().SetDirection(player.transform.position.x - transform.position.x, 30);

                            bullet = Instantiate(bullets[0], transform.position, Quaternion.identity);
                            bullet.GetComponent<PhysicsBullet>().SetForce(900f);
                            bullet.GetComponent<PhysicsBullet>().SetDirection(player.transform.position.x - transform.position.x, 40);

                            currentState = enemyState.idle;
                        }
                    }
                    break;

                //Attack 3: Floats in the air and shoots bullets in a sprial
                case 3:
                    if(attackStep == 1)
                    {
                        attackTimer = 0f;
                        attackStep = 2;
                        //bulletOffset = 0f;
                        lastPosition = transform.position;
                        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                    }
                    if(attackStep == 2)
                    {
                        attackTimer += Time.deltaTime;
                        float t = attackTimer / 2f;
                        t = -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
                        transform.position = Vector2.Lerp(lastPosition, centerPos + new Vector2(0, 3), t);

                        if(attackTimer >= 2)
                        {
                            attackStep = 3;
                            attackTimer = 5f;
                        }
                    }
                    if(attackStep == 3)
                    {
                        attackTimer -= Time.deltaTime;

                        //Shoot spiral bullets
                        float fireRate = 0.3f;
                        int density = 5;
                        float offsetRate = 0.5f;

                        if (fireRateTimer > 0) fireRateTimer -= Time.deltaTime;
                        else
                        {
                            for (int i = 0; i < density; i++)
                            {
                                float angle = (i * Mathf.PI * 2 / density) + bulletOffset;
                                float x = Mathf.Cos(angle);
                                float y = Mathf.Sin(angle);
                                Vector2 pos = (Vector2)transform.position + new Vector2(x, y);
                                float angleDegrees = -angle * Mathf.Rad2Deg;
                                Quaternion rot = Quaternion.Euler(0, 0, angleDegrees);

                                GameObject bullet = Instantiate(bullets[1], pos, rot);
                                bullet.GetComponent<NormalBulletNoFollow>().SetDirection(x, y);
                                fireRateTimer = fireRate;
                            }
                            bulletOffset += offsetRate;
                        }

                        if (attackTimer <= 0)
                        {
                            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                            currentState = enemyState.idle;
                        }
                    }
                    break;

                //Attack 4: Shoots 3 lasers in a row at the player, the last laser creates bullets on impact
                case 4:

                    break;

                //Attack 5: Causes waterfalls (vertical bullets) to fall from the ceiling
                case 5:

                    break;

                //[Overdrive] Attack 6: Combines waterfall attack and laser attack
                case 6:

                    break;

                //[Overdrive] Attack 7 (one time only): Causes the stage to rain bullets from the ceiling at random spots until boss is defeated
                case 7:
                    if(attackStep == 1)
                    {
                        attackTimer = 3f;
                        attackStep = 2;
                    }
                    if(attackStep == 2)
                    {
                        attackTimer -= Time.deltaTime;

                        if(attackTimer <= 0)
                        {
                            bulletRainOn = true;
                            currentState = enemyState.idle;
                        }
                    }
                    break;
            }
        }

        //-----DYING STATE------
        else if (currentState == enemyState.dying)
        {
            rb.velocity = Vector2.zero;
            direction = (FindObjectOfType<PlayerMovement>().transform.position.x - transform.position.x > 0) ? 1 : -1;
            rb.AddForce(new Vector2(Random.Range(200f, 400f) * -direction, Random.Range(200f, 400f)));
            GetComponent<BoxCollider2D>().enabled = false;
            currentState = enemyState.dead;
        }

        //-----DEAD STATE-----
        else if (currentState == enemyState.dead)
        {
            tag = "Untagged";
            Destroy(this.gameObject, 3f);
        }


        //Counters and Timers
        //Random Counters and Timers
        if (grounded && rngCounter <= 0 && !stunned)
        {
            rngCounter = Mathf.Clamp(Random.Range(minActionRate - (difficulty / 100f), maxActionRate - (difficulty / 100f)), 0.2f, 10f);  //from 1 to [actionTimer] seconds
            currentState = enemyState.attacking;
            attackTimer = 1f;

            //weighted RNG for attacks
            float RNG = Random.Range(0, 1f);
            if (getCurrentHealth() > maxHealth * 0.5)    //above 50% HP
            {
                switch (RNG)
                {
                    case <= 0.5f:
                        attackNum = 1;
                        break;
                    case <= 0.8f:
                        attackNum = 2;
                        break;
                    default: 
                        attackNum = 3;
                        break;
                }
            }
            else                                        //below 50% hp (overdrive)
            {
                switch (RNG)
                {
                    case <= 0.5f:
                        attackNum = 1;
                        break;
                    case <= 0.8f:
                        attackNum = 2;
                        break;
                    default:
                        attackNum = 3;
                        break;
                }
                if (!overdrive)
                {
                    overdrive = true;
                    difficulty += 5;
                }
                if (!bulletRainOn)
                {
                    attackNum = 7;
                }
            }

            attackStep = 1;                     //Reset attack step to 1

            direction = (player.transform.position.x - transform.position.x > 0) ? 1 : -1;  //if player is right of enemy, face right on next action, else do opposite.
        }

        //Bullet Rain
        if (bulletRainOn)
        {
            bulletRainTimer -= Time.deltaTime;

            if(bulletRainTimer <= 0)
            {
                GameObject bullet = Instantiate(bullets[0], new Vector2(Random.Range(centerPos.x-11, centerPos.x+11), centerPos.y+6.5f), Quaternion.identity);
                bullet.GetComponent<PhysicsBullet>().SetDirection(0, -1);
                bullet.GetComponent<PhysicsBullet>().SetGravity(0.5f);
                bulletRainTimer = bulletRainRate;
            }
        }
    }

    private void OnBecameVisible()
    {
        enabled = true;
    }
    private void OnBecameInvisible()
    {
        enabled = false;
    }
}
