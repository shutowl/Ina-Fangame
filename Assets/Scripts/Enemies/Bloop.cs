using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloop : Enemy
{
    [Header("Enemy Specific Variables")]
    public float lifeTime = 20f;
    float lifeTimeTimer = 0f;
    public float fireRate = 1f;
    float fireRateTimer = 0f;
    private float bulletOffset = 0f;
    float offsetRate = 0.5f;
    public int density = 8;
    public float speed = 5f;
    float x, y;                 //Position of x and y positions
    int xDir;                   //x direction
    int yDir;                   //y direction;
    float startYPos;
    public float amplitude;     //height of wave motion
    public float duration;      //period of wave motion
    float timer = 0f;

    private Rigidbody2D rb;
    private GameObject player;
    public GameObject bullet;

    new void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        xDir = (FindObjectOfType<PlayerMovement>().transform.position.x - transform.position.x > 0) ? 1 : -1;
        yDir = 1;
        x = transform.position.x;
        y = transform.position.y;
        startYPos = y;
        lifeTimeTimer = lifeTime;
        fireRateTimer = fireRate - Mathf.Clamp((difficulty / 200), 0, 1f);
    }

    new void Update()
    {
        base.Update();

        lifeTimeTimer -= Time.deltaTime;
        if(lifeTimeTimer <= 0)
        {
            Destroy(this.gameObject);
        }

        timer += Time.deltaTime;
        fireRateTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        //-----MOVING STATE-----
        if (currentState == enemyState.moving)
        {
            //x moves at constant rate
            x -= speed * -xDir * Time.deltaTime;
            //y moves in a wave motion
            if (timer < duration)
            {
                float t = timer / duration;
                t = t * t * (3 - (2 * t));
                y = Mathf.LerpUnclamped(startYPos - (amplitude * yDir), startYPos + (amplitude * yDir), t);
                transform.position = new Vector2(x, y);
            }
            else
            {
                if (yDir == 1)
                {
                    transform.position = new Vector2(transform.position.x, startYPos + amplitude);
                    yDir = -1;
                }
                else if (yDir == -1)
                {
                    transform.position = new Vector2(transform.position.x, startYPos - amplitude);
                    yDir = 1;
                }
                timer = 0;
            }

            if (fireRateTimer <= 0 && (x > -5f && x < 19f))
            {
                currentState = enemyState.attacking;
            }
        }
        //-----ATTACKING STATE-----
        if (currentState == enemyState.attacking)
        {
            //Spiral attack
            for (int i = 0; i < density + Mathf.Clamp((int)(Mathf.Log10(Mathf.Abs(difficulty)) * 2), 0, 15); i++)
            {
                float angle = i * Mathf.PI * 2 / (density + Mathf.Clamp((int)(Mathf.Log10(Mathf.Abs(difficulty)) * 2), 0, 15)) + bulletOffset;
                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);
                Vector2 pos = (Vector2)transform.position + new Vector2(x, y);
                float angleDegrees = -angle * Mathf.Rad2Deg;
                Quaternion rot = Quaternion.Euler(0, 0, angleDegrees);

                GameObject bullet = Instantiate(this.bullet, pos, rot);
                bullet.GetComponent<NormalBulletNoFollow>().SetDirection(x, y);
                bullet.GetComponent<NormalBulletNoFollow>().lifeTime = 8f;
            }
            bulletOffset += offsetRate;

            fireRateTimer = fireRate - Mathf.Clamp((difficulty / 150f), 0, 1.3f);
            currentState = enemyState.moving;
        }
        //-----DYING STATE-----
        if (currentState == enemyState.dying)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = Vector2.zero;
            xDir = (FindObjectOfType<PlayerMovement>().transform.position.x - transform.position.x > 0) ? 1 : -1;
            rb.AddForce(new Vector2(Random.Range(200f, 400f) * xDir, Random.Range(200f, 400f)));
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
