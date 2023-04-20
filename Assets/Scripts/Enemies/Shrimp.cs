using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrimp : Enemy
{
    [Header("Enemy Specific Variables")]
    public float actionRate = 1f;
    float actionTimer = 0f;
    float attackStep = 0;
    public float jumpForce;
    int direction;

    private Rigidbody2D rb;
    private GameObject player;
    public GameObject bullet;

    new void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        direction = (FindObjectOfType<PlayerMovement>().transform.position.x - transform.position.x > 0) ? 1 : -1;
        currentState = enemyState.idle;
    }

    new void Update()
    {
        base.Update();

        if(currentState == enemyState.idle)
        {
            direction = (FindObjectOfType<PlayerMovement>().transform.position.x - transform.position.x > 0) ? 1 : -1;

            if (grounded)
            {
                actionTimer -= Time.deltaTime;
                rb.velocity = Vector2.zero;
            }
            
            if(actionTimer <= 0)
            {
                switch (Random.Range(0, 2))
                {
                    //Jump
                    case 0:
                        currentState = enemyState.jumping;
                        break;
                    //Attack
                    case 1:
                        attackStep = 0;
                        currentState = enemyState.attacking;
                        break;
                }
            }
        }
        if (currentState == enemyState.jumping)
        {
            //Jump
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2((100 + Random.Range(0, 100f)) * direction, jumpForce + Random.Range(-100f, 100f)));

            currentState = enemyState.idle;
            actionTimer = Random.Range(actionRate, actionRate + 0.5f);
        }
        if (currentState == enemyState.attacking)
        {
            if(attackStep == 0)
            {
                //Jump towards player
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2((100 + Random.Range(0, 100f)) * direction, jumpForce + Random.Range(-100f, 100f)));

                attackStep = 1;
            }
            if(attackStep == 1)
            {
                if(!grounded && rb.velocity.y < -2f)
                {
                    //Jump backwards a bit and fire bullet at player
                    direction = (FindObjectOfType<PlayerMovement>().transform.position.x - transform.position.x > 0) ? 1 : -1;
                    rb.velocity = Vector2.zero;
                    rb.AddForce(new Vector2((100 + Random.Range(0, 50f)) * -direction, (jumpForce + Random.Range(-100f, 100f))/2));

                    GameObject bullet = Instantiate(this.bullet, transform.position, Quaternion.identity);
                    bullet.GetComponent<NormalBulletNoFollow>().follow = true;

                    currentState = enemyState.idle;
                    actionTimer = Random.Range(actionRate, actionRate + 0.5f);
                }
            }
        }
        //-----DYING STATE-----
        if (currentState == enemyState.dying)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = Vector2.zero;
            direction = (FindObjectOfType<PlayerMovement>().transform.position.x - transform.position.x > 0) ? 1 : -1;
            rb.AddForce(new Vector2(Random.Range(200f, 400f) * -direction, Random.Range(200f, 400f)));
            GetComponent<Collider2D>().enabled = false;
            rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
            currentState = enemyState.dead;
        }
        //-----DEAD STATE------
        if (currentState == enemyState.dead)
        {
            tag = "Untagged";
            Destroy(this.gameObject, 2f);
        }
    }
}
