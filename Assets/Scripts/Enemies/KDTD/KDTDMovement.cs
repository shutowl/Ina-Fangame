using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KDTDMovement : Enemy
{
    public float speed = 10f;
    public float jumpForce = 300f;
    public float minActionRate = 0.5f;
    public float maxActionRate = 2f;   //Performs an action every [1 to actionRate] seconds
    private int direction;
    private int movesLeft;
    private float rngCounter = 0f;
    private float moveTimer = 0f;
    private Vector2 centerPos = Vector2.zero;
    private float attackTimer;          // Used for attack timings (such as delays or charges)
    private int attackNum = 1;          // Determines which attack is used
    private int attackStep = 1;         // Current step of the current attack

    public float attack3Rate = 0.2f;    // Fire rate of shake attack (attack 3)
    private float attack3RateTimer = 0f;

    private Rigidbody2D rb;
    public GameObject player;
    public GameObject[] bullets;
    public GameObject tako;

    void Start()
    {
        direction = -1; //start facing left;
        movesLeft = Random.Range(1,4); //starts with 1-3 movement options
        rb = GetComponent<Rigidbody2D>();
        enabled = false;
        currentState = enemyState.idle;
        centerPos = Camera.main.transform.position;
    }

    void Update()
    {
        //KDTD will move or jump 2 or 3 times before attacking player

        //-----MOVING STATE--------
        if (currentState == enemyState.moving)
        {
            rb.velocity = new Vector2(Mathf.Lerp(0, speed, 1 - Mathf.Pow(2, -10 * (moveTimer / rngCounter))) * direction, rb.velocity.y);
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
                            for (int i = 0; i < 20; i++)
                            {
                                GameObject bullet = Instantiate(bullets[0], transform.position, Quaternion.identity);
                                bullet.GetComponent<PhysicsBullet>().setDirection(Random.Range(-1f, 1f), Random.Range(2f, 3f));
                                bullet.GetComponent<PhysicsBullet>().setForce(Random.Range(600f, 900f));
                            }
                            Debug.Log("KDTD Leap attack finished (attack 1)");
                            currentState = enemyState.idle;
                        }
                    }
                    break;
                //Attack 2: Summons about 3 takos around itself (maybe falls from the sky)
                case 2:
                    if(attackStep == 1)         //For future attacks, step 1 can be used to setup charge values like this:
                    {
                        attackTimer = 3f;
                        attackStep = 2;
                    }
                    if(attackStep == 2)         //Summon takos after a short delay
                    {
                        attackTimer -= Time.deltaTime;
                        //Enable summoning animation

                        if(attackTimer <= 0)
                        {
                            //Spawn up to 3 takos
                            Instantiate(tako, transform.position + new Vector3(0, 2), Quaternion.identity);
                            Instantiate(tako, transform.position + new Vector3(0.7f, 1), Quaternion.identity);
                            Instantiate(tako, transform.position + new Vector3(-0.7f, 1), Quaternion.identity);
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
                        rb.AddForce(new Vector2(20 * direction, 40));                       //Jump towards center
                        attackStep = 2;
                    }
                    if(attackStep == 2)
                    {
                        if (grounded)
                        {
                            rb.velocity = Vector2.zero;
                            if (transform.position.x < centerPos.x + 1 && transform.position.x > centerPos.x - 1)    //Check if near enough to center
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
                        if (attack3RateTimer < 0)
                        {
                            GameObject bullet = Instantiate(bullets[0], transform.position, Quaternion.identity);
                            bullet.GetComponent<PhysicsBullet>().setDirection(Random.Range(-1f, 1f), Random.Range(2f, 3f));
                            bullet.GetComponent<PhysicsBullet>().setForce(Random.Range(700f, 900f));
                            attack3RateTimer = attack3Rate;
                        }


                        if (attackTimer <= 0)
                        {
                            Debug.Log("KDTD Shake attack finished (attack 3)");
                            currentState = enemyState.idle;
                        }
                    }
                    break;
            }


            //Overdrive Attack 1: Constantly makes quick leaping attacks towards the player and sprays bullets upon landing. Stops briefly after landing.
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

        //Random Counters and Timers
        if(grounded && rngCounter <= 0){
            rngCounter = Random.Range(minActionRate, maxActionRate);  //from 1 to [actionTimer] seconds
            if(movesLeft > 0){
                currentState = (enemyState)(int)(Random.Range(0,2)); //Can still move, so either move or jump
                moveTimer = rngCounter;
            }
            else{
                currentState = enemyState.attacking;
                attackTimer = 1f;
                //weighted RNG for attacks
                float RNG = Random.Range(0, 100);
                switch (RNG)
                {
                    case < 50:  //50%
                        attackNum = 1;
                        break;
                    case < 70:  //20%
                        attackNum = 2;
                        break;
                    default:    //30%
                        attackNum = 3;
                        break;
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

