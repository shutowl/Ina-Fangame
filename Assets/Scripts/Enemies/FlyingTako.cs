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
    public GameObject[] bullets;            //bullets[0] = normal, bullets[1] = no follow
    public bool follow = true;
    public float x = 0, y = -1;

    private float maxDistance = 8;          //Max distance enemy can fly from center of stage
    private Vector2 centerPos;              //Camera position
    private int direction = 1;              //1 = right, -1 = left
    private float timer = 0f;
    private float xPos;

    public float lifeTime = 100f;
    private float lifeTimeTimer = 0f;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        centerPos = Camera.main.transform.position;
        rb = GetComponent<Rigidbody2D>();

        direction = (transform.position.x > centerPos.x) ? -1 : 1;
        maxDistance = Mathf.Abs(centerPos.x - transform.position.x);
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
            if (follow)
            {
                Instantiate(bullets[0], transform.position, Quaternion.identity);
            }
            else
            {
                GameObject bullet = Instantiate(bullets[1], transform.position, Quaternion.identity);
                bullet.GetComponent<NormalBulletNoFollow>().SetDirection(x, y);
            }

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
        lifeTimeTimer += Time.deltaTime;

        if(lifeTimeTimer > lifeTime)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetFireRate(float fireRate)
    {
        this.fireRate = fireRate;
    }

    public void SetDuration(float duration)
    {
        this.duration = duration;
    }

    public void SetFollow(bool follow)
    {
        this.follow = follow;
    }

    public void SetDirection(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetLifeTime(float lifeTime)
    {
        this.lifeTime = lifeTime;
    }

    public void ResetValues()
    {
        lifeTime = 100f;
        follow = true;
        fireRate = 1f;
        duration = 5f;
        x = 0;
        y = -1;
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
