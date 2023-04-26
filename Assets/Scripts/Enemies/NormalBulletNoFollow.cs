using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A variation of NormalBullet that fires in a set direction
public class NormalBulletNoFollow : MonoBehaviour
{
    public float speed = 10f;           //Projectile speed of
    public float lifeTime = 3f;         //Time before bullet destroys itself
    public bool follow = false;
    private float lifeTimeTimer = 0f;

    private Rigidbody2D rb;
    public float x = 0, y = -1;
    private Vector2 direction;
    private float followOffset = 0;

    private void Awake()
    {
        SetDirection(x, y);
    }

    void Start()
    {
        lifeTimeTimer = lifeTime;
        rb = GetComponent<Rigidbody2D>();

        if (!follow)
        {
            Vector3 rotation = -direction;
            rb.velocity = direction * speed;
            float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rot + 90);
        }
        else
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            rb = GetComponent<Rigidbody2D>();
            Vector3 playerPos = player.transform.position;

            float radius = (playerPos - transform.position).magnitude;
            float angle = Mathf.Atan2((playerPos - transform.position).y, (playerPos - transform.position).x) * Mathf.Rad2Deg;

            Vector3 direction = radius * new Vector3(Mathf.Cos((angle + followOffset) * Mathf.Deg2Rad), Mathf.Sin((angle + followOffset) * Mathf.Deg2Rad));

            Vector3 rotation = transform.position - playerPos;
            rb.velocity = new Vector2(direction.x, direction.y).normalized * speed;
            float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rot + 90);
        }

    }

    void Update()
    {
        lifeTimeTimer -= Time.deltaTime;
        if (lifeTimeTimer <= 0)
        {
            Destroy(this.gameObject);
        }
    }
/*
    private void OnTriggerEnter2D(Collider2D col)
    {
        //Hits wall or exceeds lifetime
        if (col.CompareTag("Ground"))
        {
            Destroy(this.gameObject);
        }
    }
*/
    public void SetDirection(float x, float y)
    {
        direction = new Vector2(x, y).normalized;
    }

    public void SetOffset(float degrees)
    {
        followOffset = degrees;
    }
}
