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
    public bool geyser = false;
    public GameObject laser;

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
        //Hits wall
        if (col.CompareTag("Ground") && geyser && transform.position.y <= -1.5f)
        {
            GameObject indicator = Instantiate(this.laser, transform.position, Quaternion.identity);
            indicator.GetComponent<GuraLaser>().indicator = true;
            indicator.GetComponent<GuraLaser>().lifeTime = 1f;
            indicator.GetComponent<GuraLaser>().SetPositions(Vector2.down * 2f, Vector2.up * 20f);

            GameObject laser = Instantiate(this.laser, transform.position, Quaternion.identity);
            laser.GetComponent<GuraLaser>().delay = 1f;
            laser.GetComponent<GuraLaser>().lifeTime = 0.5f;
            laser.GetComponent<GuraLaser>().SetPositions(Vector2.down * 2f, Vector2.up * 20f);

            Destroy(this.gameObject);
        }
    }

    public void SetDirection(float x, float y)
    {
        //Debug.Log("Set Direction: (" + x + ", " + y + ")");
        direction = new Vector2(x, y).normalized;
    }

    public void SetForce(float f)
    {
        force = f;
    }

    public void SetGravity(float gravity)
    {
        rb.gravityScale = gravity;
    }

}
