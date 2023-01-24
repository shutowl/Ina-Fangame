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

    private Rigidbody2D rb;
    public GameObject player;

    void Start()
    {
        direction = -1; //start facing left;
        movesLeft = Random.Range(1,4); //starts with 1-3 movement options
        rb = GetComponent<Rigidbody2D>();
        enabled = false;
        currentState = enemyState.idle;
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
                currentState = enemyState.idle;
            }
        }

        //-----JUMPING STATE------
        //Jumps will vary between 3 types (low, medium and high)
        else if (currentState == enemyState.jumping)
        {
            int jumpType = Random.Range(0, 3) + 1; //1, 2, or 3
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
            currentState = enemyState.idle;
        }

        //-----ATTACKING STATE------
        else if(currentState == enemyState.attacking){
            currentState = enemyState.idle;

            int attackNum = Random.Range(0,3) + 1;
            //Add attack patterns here

            //Attack 1: Makes a big jump towards the center of the stage. Upon landing, create a fountain of bullets spraying everywhere

            //Attack 2: Summons about 3 takos around itself (maybe falls from the sky)

            //Attack 3: Goes to one side of the stage and fires 

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
                movesLeft = Random.Range(0, 3) + 1;
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
