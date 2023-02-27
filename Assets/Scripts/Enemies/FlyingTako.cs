using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple flying enemy that flies in a straight horizontal line across the screen
//and shoots a single small bullet every so often
public class FlyingTako : Enemy
{
    [Header("Enemy Specific Variables")]
    public float duration = 10f;
    public float fireRate = 1f;
    private float fireRateTimer = 0f;
    public GameObject bullet;

    private int maxDistance = 8;            //Max distance enemy can fly from center of stage
    private Vector2 centerPos;              //Camera position
    private int direction = 1;              //1 = right, -1 = left
    private float timer = 0f;
    private float xPos;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        centerPos = Camera.main.transform.position;
        rb = GetComponent<Rigidbody2D>();

        direction = (transform.position.x > centerPos.x) ? -1 : 1;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        //-----MOVING STATE-----
        if(currentState == enemyState.moving)
        {
            if (timer < duration)
            {
                float t = timer / duration;
                t = t * t * (3f - 2f * t);
                xPos = Mathf.Lerp(centerPos.x - (maxDistance * direction), centerPos.x + (maxDistance * direction), t);
                transform.position = new Vector2(xPos, transform.position.y);
            }
            else
            {
                if (direction == 1)
                {
                    transform.position = new Vector2(centerPos.x + maxDistance, transform.position.y);
                    direction = -1;
                }
                else if (direction == -1)
                {
                    transform.position = new Vector2(centerPos.x - maxDistance, transform.position.y);
                    direction = 1;
                }

                timer = 0;
            }

            if(fireRateTimer > fireRate)
            {
                currentState = enemyState.attacking;
            }
        }
        //-----ATTACKING STATE-----
        else if(currentState == enemyState.attacking)
        {
            //Shoot bullet towards player
            Instantiate(bullet, transform.position, Quaternion.identity);
            fireRateTimer = 0f;
            currentState = enemyState.moving;
        }

        //-----DYING STATE-----
        else if (currentState == enemyState.dying)
        {
            rb.velocity = Vector2.zero;
            direction = (FindObjectOfType<PlayerMovement>().transform.position.x - transform.position.x > 0) ? 1 : -1;
            rb.AddForce(new Vector2(Random.Range(200f, 400f) * -direction, Random.Range(200f, 400f)));
            GetComponent<Collider2D>().enabled = false;
            rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
            currentState = enemyState.dead;
        }
        //-----DEAD STATE------
        else
        {
            tag = "Untagged";
            Destroy(this.gameObject, 2f);
        }


        fireRateTimer += Time.deltaTime;
        timer += Time.deltaTime;
    }

    public void SetFireRate(float fireRate)
    {
        this.fireRate = fireRate;
    }

    public void SetDuration(float duration)
    {
        this.duration = duration;
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
