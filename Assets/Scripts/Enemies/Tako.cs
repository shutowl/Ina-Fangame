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
        currentState = enemyState.moving;
    }

    new void Update()
    {
        base.Update();

        if (!stunned)
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
            //-----IDLE STATE------
            else if (currentState == enemyState.idle)
            {
                if (grounded)
                    rb.velocity = Vector2.zero;
            }
            //-----DYING STATE-----
            else if (currentState == enemyState.dying)
            {
                rb.velocity = Vector2.zero;
                direction = (FindObjectOfType<PlayerMovement>().transform.position.x - transform.position.x > 0) ? 1 : -1;
                rb.AddForce(new Vector2(200f * -direction, 200f));
                GetComponent<BoxCollider2D>().enabled = false;
                currentState = enemyState.dead;
                rngCounter = 999f;
            }
            //-----DEAD STATE------
            else
            {
                tag = "Untagged";
                Destroy(this.gameObject, 2f);
            }

            if (rngCounter > 0)
            {
                rngCounter -= Time.deltaTime;
            }
            else
            {
                rngCounter = Random.Range(1, actionTimer);
                currentState = (enemyState)(int)Random.Range(0, 2); //0-1
                direction = (FindObjectOfType<PlayerMovement>().transform.position.x - transform.position.x > 0) ? 1 : -1;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
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
