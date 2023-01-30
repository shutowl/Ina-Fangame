using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tako : Enemy
{
    public float speed = 10;
    public float jumpForce = 500;
    public float actionTimer = 2;   //Actions will alternate every 1 to actionTimer seconds
    private float rngCounter = 0;
    private int direction = 1;

    private Rigidbody2D rb;

    void Start()
    {
        direction = -1; //start facing left;
        rb = GetComponent<Rigidbody2D>();
        enabled = false;
    }

    void Update()
    {
        //-----MOVING STATE--------
        if (currentState == enemyState.moving)
        {
            rb.velocity = new Vector2(Mathf.Lerp(0, speed, 1 - Mathf.Pow(1 - (rngCounter / actionTimer), 3)) * direction, rb.velocity.y);
        }

        //-----JUMPING STATE------
        else if (currentState == enemyState.jumping)
        {
            rb.AddForce(new Vector2(jumpForce / 4 * direction, jumpForce));
            currentState = enemyState.idle;
        }

        //-----DAMAGED STATE------
        else if (currentState == enemyState.hurt)
        {

        }

        else if (currentState == enemyState.idle)
        {
            if (grounded)
                rb.velocity = Vector2.zero;
        }


        if (rngCounter > 0)
        {
            rngCounter -= Time.deltaTime;
        }
        else
        {
            rngCounter = Random.Range(1, actionTimer);
            currentState = (enemyState)(int)Random.Range(0, 2);
            direction = (FindObjectOfType<PlayerMovement>().transform.position.x - transform.position.x > 0) ? 1 : -1;
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
