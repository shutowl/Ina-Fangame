using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A bullet fired at the player with no gravity
public class NormalBullet : MonoBehaviour
{
    public float speed = 10f;           //Projectile speed of
    public float lifeTime = 3f;         //Time before bullet destroys itself
    private float lifeTimeTimer = 0f;

    private GameObject player;
    private Rigidbody2D rb;
    private Vector3 playerPos;

    void Start()
    {
        lifeTimeTimer = lifeTime;

        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        playerPos = player.transform.position;
        Vector3 direction = playerPos - transform.position;
        Vector3 rotation = transform.position - playerPos;
        rb.velocity = new Vector2(direction.x, direction.y).normalized * speed;
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
}
