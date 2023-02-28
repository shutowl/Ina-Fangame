using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A variation of NormalBullet that fires in a set direction
public class NormalBulletNoFollow : MonoBehaviour
{
    public float speed = 10f;           //Projectile speed of
    public float lifeTime = 3f;         //Time before bullet destroys itself
    private float lifeTimeTimer = 0f;

    private Rigidbody2D rb;
    public float x = 0, y = -1;
    private Vector2 direction;

    private void Awake()
    {
        SetDirection(x, y);
    }

    void Start()
    {
        lifeTimeTimer = lifeTime;

        rb = GetComponent<Rigidbody2D>();
        Vector3 rotation = -direction;
        rb.velocity = direction * speed;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90);
    }

    void Update()
    {
        lifeTimeTimer -= Time.deltaTime;
        if (lifeTimeTimer <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //Hits wall or exceeds lifetime
        if (col.CompareTag("Ground"))
        {
            Destroy(this.gameObject);
        }
    }

    public void SetDirection(float x, float y)
    {
        direction = new Vector2(x, y).normalized;
    }
}
