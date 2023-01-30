using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBullet : MonoBehaviour
{
    private float force = 100f;
    public float lifeTime = 3f;
    private float lifeTimeTimer = 0f;

    private float x = -1, y = 1;
    private Vector2 direction;
    private Rigidbody2D rb;

    bool launched = false;

    void Awake()
    {
        direction = new Vector2(x, y).normalized;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!launched)
        {
            rb.AddForce(direction * force);
            launched = true;
        }

        //Destroy after a certain amount of time
        lifeTimeTimer += Time.deltaTime;
        if (lifeTimeTimer > lifeTime)
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

    public void setDirection(float x, float y)
    {
        //Debug.Log("Set Direction: (" + x + ", " + y + ")");
        direction = new Vector2(x, y).normalized;
    }

    public void setForce(float f)
    {
        force = f;
    }
}
