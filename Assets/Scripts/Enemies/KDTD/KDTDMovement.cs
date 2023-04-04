using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KDTDMovement : Enemy
{
    [Header("Enemy Specific Variables")]
    public float speed = 10f;
    public float jumpForce = 300f;
    public float minActionRate = 0.5f;
    public float maxActionRate = 2f;    // Performs an action every [1 to actionRate] seconds
    private int direction;
    private int movesLeft;              // Number of movement options left before making an attack
    private float rngCounter = 0f;
    private float moveTimer = 0f;
    private Vector2 centerPos = Vector2.zero;
    private float attackTimer;          // Used for attack timings (such as delays or charges)
    private int attackNum = 1;          // Determines which attack is used
    private int attackStep = 1;         // Current step of the current attack

    public float attack3Rate = 0.2f;    // Fire rate of shake attack (attack 3)
    private float attack3RateTimer = 0f;

    public int difficulty = 1;

    private Rigidbody2D rb;
    private GameObject player;
    public GameObject[] bullets;
    public GameObject tako;
    public GameObject flyingTako;
    public GameObject healthBar;
    public GameObject spawner;

    private bool overdrive = false;

    void Start()
    {
        direction = -1; //start facing left;
        movesLeft = Random.Range(1,4); //starts with 1-3 movement options
        rb = GetComponent<Rigidbody2D>();
        enabled = false;
        currentState = enemyState.idle;
        centerPos = Camera.main.transform.position;
        player = GameObject.FindGameObjectWithTag("Player");

        bossHealthBar.SetBoss(this.gameObject, maxHealth, enemyName);
        bossHealthBar.SetBarColor(new Color(0.588f, 0.287f, 0.811f),
                                  new Color(1.000f, 0.600f, 0.994f),
                                  new Color(0.519f, 0.174f, 0.498f));
    }

    new void Update()
    {
        base.Update();

        //KDTD will move or jump 2 or 3 times before attacking player

        //-----MOVING STATE--------
        if (currentState == enemyState.moving)
        {
            rb.velocity = new Vector2(Mathf.Lerp(0, speed * 1/(2 * rngCounter), 1 - Mathf.Pow(2, -10 * (moveTimer / rngCounter))) * direction, rb.velocity.y);
            moveTimer -= Time.deltaTime;
            if(Mathf.Abs(rb.velocity.x) < 0.5f){
                movesLeft--;
                currentState = enemyState.idle;
            }
        }

        //-----JUMPING STATE------
        //Jumps will vary between 3 types (low, medium and high)
        else if (currentState == enemyState.jumping)
        {
            int jumpType = Random.Range(0, 3) + 1; //1 to 3
            switch(jumpType){
                case 1:
                    rb.AddForce(new Vector2(jumpForce / 4 * direction, jumpForce - 400));
                    break;
                case 2:
                    rb.AddForce(new Vector2(jumpForce / 4 * direction, jumpForce - 200));
                    break;
                case 3:
                    rb.AddForce(new Vector2(jumpForce / 4 * direction, jumpForce));
                    break;
            }
            movesLeft--;
            currentState = enemyState.idle;
        }

        //-----ATTACKING STATE------
        else if(currentState == enemyState.attacking)
        {
            //Add attack patterns here
            switch (attackNum)
            {
                //Attack 1: Makes a big jump towards the player. Upon landing, create a fountain of bullets spraying everywhere
                case 1:
                    if(attackStep == 1)  //Charge jump (Anticipation)
                    {
                        attackTimer -= Time.deltaTime;

                        if (attackTimer <= 0)
                        {
                            rb.AddForce(new Vector2((Mathf.Abs(transform.position.x - player.transform.position.x)) * 30 * direction, jumpForce));   //distance based on distance to player
                            attackStep = 2;
                        }
                    }
                    if(attackStep == 2)   //Getting in position
                    {
                        if(!grounded && rb.velocity.y < 0.1f)
                        {
                            attackTimer = 1f;
                            attackStep = 3;
                        }
                    }
                    if(attackStep == 3)   //hover in air for a bit before slamming down
                    {
                        attackTimer -= Time.deltaTime;
                        rb.velocity = Vector2.zero;

                        if (attackTimer <= 0)
                        {
                            rb.AddForce(new Vector2(0, -jumpForce*2));  //slam down
                            attackStep = 4;
                        }
                    }
                    if(attackStep == 4)   //create bullets upon landing
                    {
                        if (grounded)
                        {
                            //create bullets
                            for (int i = 0; i < 20 + difficulty; i++)
                            {
                                GameObject bullet = Instantiate(bullets[0], transform.position, Quaternion.identity);
                                bullet.GetComponent<PhysicsBullet>().SetDirection(Random.Range(-1f, 1f), Random.Range(2f, 3f));
                                bullet.GetComponent<PhysicsBullet>().SetForce(Random.Range(600f, 900f));
                            }
                            Debug.Log("KDTD Leap attack finished (attack 1)");
                            currentState = enemyState.idle;
                        }
                    }
                    break;
                //Attack 2: Summons about 3 takos around itself
                case 2:
                    if(attackStep == 1)         //For future attacks, step 1 can be used to setup charge values like this:
                    {
                        attackTimer = 3f;

                        //Spawn up to 3 takos
                        spawner.GetComponent<Spawner>().SetSpawn(tako, 3);
                        for (int i = 0; i < 3 + difficulty / 5; i++)
                        {
                            Instantiate(spawner, transform.position + new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(0f, 2f)), Quaternion.identity);
                        }

                        attackStep = 2;
                    }
                    if(attackStep == 2)         //Summon takos after a short delay
                    {
                        attackTimer -= Time.deltaTime;
                        //Enable summoning animation

                       if(attackTimer <= 0)
                       {
                            Debug.Log("KDTD Summon Tako finished (attack 2)");
                            currentState = enemyState.idle;
                       }
                    }
                    break;
                //Attack 3: Goes to center of stage and shakes itself, releasing projectiles
                case 3:
                    if(attackStep == 1)
                    {
                        direction = (transform.position.x < centerPos.x) ? 1 : -1;          //Faces camera (center)
                        rb.AddForce(new Vector2((20 + 3*Mathf.Abs(transform.position.x - centerPos.x)) * direction, 80));       //Jump towards center
                        attackStep = 2;
                    }
                    if(attackStep == 2)
                    {
                        if (grounded)
                        {
                            rb.velocity = Vector2.zero;
                            if (transform.position.x < centerPos.x + 1.5f && transform.position.x > centerPos.x - 1.5f)    //Check if near enough to center
                            {
                                attackTimer = 1f;   //duration of delay
                                attackStep = 3;
                            }
                            else
                            {
                                attackStep = 1; //if not, jump again
                            }
                        }
                    }
                    if(attackStep == 3) //Charge up attack
                    {
                        attackTimer -= Time.deltaTime;
                        rb.velocity = Vector2.zero; //prevents moving during charge

                        if (attackTimer <= 0)
                        {
                            attackTimer = 4f;       //duration of attack
                            attackStep = 4;
                            attack3RateTimer = attack3Rate;
                        }

                    }
                    if(attackStep == 4) //Shake attack and spawn projectiles
                    {
                        attackTimer -= Time.deltaTime;
                        attack3RateTimer -= Time.deltaTime;

                        //Spawn bullets at a random angle (above a certain point) every [rate] seconds at varying speeds
                        if (attack3RateTimer < 0 + (difficulty/400f))    //difficulty 4 = 0.01f
                        {
                            GameObject bullet = Instantiate(bullets[0], transform.position, Quaternion.identity);
                            bullet.GetComponent<PhysicsBullet>().SetDirection(Random.Range(-1f, 1f), Random.Range(2f, 3f));
                            bullet.GetComponent<PhysicsBullet>().SetForce(Random.Range(800f, 1000f));
                            attack3RateTimer = attack3Rate;
                        }


                        if (attackTimer <= 0)
                        {
                            Debug.Log("KDTD Shake attack finished (attack 3)");
                            currentState = enemyState.idle;
                        }
                    }
                    break;
                //Overdrive Attack 1: Make (3) consecutive jumps towards the player
                case 4:
                    if (attackStep == 1)  //Charge jump (Anticipation)
                    {
                        attackTimer -= Time.deltaTime;

                        if (attackTimer <= 0)
                        {
                            rb.AddForce(new Vector2((Mathf.Abs(transform.position.x - player.transform.position.x)) * 30 * direction, jumpForce));   //distance based on distance to player
                            attackStep = 2;
                        }
                    }
                    if (attackStep == 2)   //Getting in position
                    {
                        if (!grounded && rb.velocity.y < 0.1f)
                        {
                            attackTimer = 0.5f;
                            attackStep = 3;
                        }
                    }
                    if (attackStep == 3)   //hover in air for a bit before slamming down
                    {
                        attackTimer -= Time.deltaTime;
                        rb.velocity = Vector2.zero;

                        if (attackTimer <= 0)
                        {
                            rb.AddForce(new Vector2(0, -jumpForce * 2));  //slam down
                            attackStep = 4;
                        }
                    }
                    if (attackStep == 4)   //create bullets upon landing
                    {
                        if (grounded)
                        {
                            //create bullets
                            for (int i = 0; i < 20 + difficulty; i++)
                            {
                                GameObject bullet = Instantiate(bullets[0], transform.position, Quaternion.identity);
                                bullet.GetComponent<PhysicsBullet>().SetDirection(Random.Range(-1f, 1f), Random.Range(2f, 3f));
                                bullet.GetComponent<PhysicsBullet>().SetForce(Random.Range(600f, 900f));
                            }
                            attackStep = 5;
                            attackTimer = 0.5f;
                            direction = (player.transform.position.x - transform.position.x > 0) ? 1 : -1;
                        }
                    }
                    if (attackStep == 5)  //Charge jump (Anticipation)
                    {
                        attackTimer -= Time.deltaTime;

                        if (attackTimer <= 0)
                        {
                            rb.AddForce(new Vector2((Mathf.Abs(transform.position.x - player.transform.position.x)) * 30 * direction, jumpForce));   //distance based on distance to player
                            attackStep = 6;
                        }
                    } 
                    if (attackStep == 6)   //Getting in position
                    {
                        if (!grounded && rb.velocity.y < 0.1f)
                        {
                            attackTimer = 0.5f;
                            attackStep = 7;
                        }
                    }
                    if (attackStep == 7)   //hover in air for a bit before slamming down
                    {
                        attackTimer -= Time.deltaTime;
                        rb.velocity = Vector2.zero;

                        if (attackTimer <= 0)
                        {
                            rb.AddForce(new Vector2(0, -jumpForce * 2));  //slam down
                            attackStep = 8;
                        }
                    }
                    if (attackStep == 8)   //create bullets upon landing
                    {
                        if (grounded)
                        {
                            //create bullets
                            for (int i = 0; i < 20 + difficulty; i++)
                            {
                                GameObject bullet = Instantiate(bullets[0], transform.position, Quaternion.identity);
                                bullet.GetComponent<PhysicsBullet>().SetDirection(Random.Range(-1f, 1f), Random.Range(2f, 3f));
                                bullet.GetComponent<PhysicsBullet>().SetForce(Random.Range(600f, 900f));
                            }
                            attackStep = 9;
                            attackTimer = 0.5f;
                            direction = (player.transform.position.x - transform.position.x > 0) ? 1 : -1;
                        }
                    }
                    if (attackStep == 9)  //Charge jump (Anticipation)
                    {
                        attackTimer -= Time.deltaTime;

                        if (attackTimer <= 0)
                        {
                            rb.AddForce(new Vector2((Mathf.Abs(transform.position.x - player.transform.position.x)) * 30 * direction, jumpForce));   //distance based on distance to player
                            attackStep = 10;
                        }
                    }
                    if (attackStep == 10)   //Getting in position
                    {
                        if (!grounded && rb.velocity.y < 0.1f)
                        {
                            attackTimer = 0.5f;
                            attackStep = 11;
                        }
                    }
                    if (attackStep == 11)   //hover in air for a bit before slamming down
                    {
                        attackTimer -= Time.deltaTime;
                        rb.velocity = Vector2.zero;

                        if (attackTimer <= 0)
                        {
                            rb.AddForce(new Vector2(0, -jumpForce * 2));  //slam down
                            attackStep = 12;
                        }
                    }
                    if (attackStep == 12)   //create bullets upon landing
                    {
                        if (grounded)
                        {
                            //create bullets
                            for (int i = 0; i < 20 + difficulty; i++)
                            {
                                GameObject bullet = Instantiate(bullets[0], transform.position, Quaternion.identity);
                                bullet.GetComponent<PhysicsBullet>().SetDirection(Random.Range(-1f, 1f), Random.Range(2f, 3f));
                                bullet.GetComponent<PhysicsBullet>().SetForce(Random.Range(600f, 900f));
                            }
                            Debug.Log("KDTD Overdrive leap attack finished (attack 4)");
                            currentState = enemyState.idle;
                        }
                    }
                    break;
            }


            //Overdrive Attack 1: Constantly makes quick leaping attacks towards the player and sprays bullets upon landing. Stops briefly after landing.
            //Basically 3-4 Attack1 in a row
            //Activates upon reaching a certain HP

        }

        //-----HURT STATE------
        else if (currentState == enemyState.hurt)
        {

        }
        //------IDLE STATE------
        else if (currentState == enemyState.idle)
        {
            if (grounded){
                rb.velocity = Vector2.zero;

                if (rngCounter > 0){
                    rngCounter -= Time.deltaTime;
                }
            }       
        }
        //-----DYING STATE-----
        else if (currentState == enemyState.dying)
        {
            rb.velocity = Vector2.zero;
            direction = (FindObjectOfType<PlayerMovement>().transform.position.x - transform.position.x > 0) ? 1 : -1;
            rb.AddForce(new Vector2(Random.Range(200f, 400f) * -direction, Random.Range(200f, 400f)));
            GetComponent<BoxCollider2D>().enabled = false;
            currentState = enemyState.dead;
        }
        //-----DEAD STATE------
        else
        {
            tag = "Untagged";
            Destroy(this.gameObject, 3f);
        }

        //Random Counters and Timers
        if (grounded && rngCounter <= 0 && !stunned){
            rngCounter = Mathf.Clamp(Random.Range(minActionRate - (difficulty/100f), maxActionRate - (difficulty/100f)), 0.2f, 10f);  //from 1 to [actionTimer] seconds
            if(movesLeft > 0){
                currentState = (enemyState)(int)(Random.Range(0,2)); //Can still move, so either move or jump
                moveTimer = rngCounter;
            }
            else{
                currentState = enemyState.attacking;
                attackTimer = 1f;
                //weighted RNG for attacks
                float RNG = Random.Range(0, 1f);
                if (GetCurrentHealth() > maxHealth * 0.5)    //above 50% HP
                {
                    switch (RNG)
                    {
                        case < 0.5f:  //50%
                            attackNum = 1;
                            break;
                        case < 0.7f:  //20%
                            attackNum = 2;
                            break;
                        default:    //30%
                            attackNum = 3;
                            break;
                    }
                }
                else                                        //below 50% hp
                {
                    switch (RNG)
                    {
                        case < 0.6f:  //60%
                            attackNum = 4;
                            break;
                        case < 0.8f:  //20%
                            attackNum = 2;
                            break;
                        default:    //20%
                            attackNum = 3;
                            break;
                    }
                    if (!overdrive)
                    {
                        overdrive = true;
                        difficulty += 5;
                    }
                }

                //attackNum = Random.Range(0,3) + 1;  //1 to 3
                attackStep = 1;                     //Reset attack step to 1

                movesLeft = Random.Range(0,3) + 1; //1 to 3
            }
            direction = (player.transform.position.x - transform.position.x > 0) ? 1 : -1;  //if player is right of enemy, face right on next action, else do opposite.
            //currentState = (enemyState)(int)Random.Range(0, 2);
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

