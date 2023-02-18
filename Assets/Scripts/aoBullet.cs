using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aoBullet : MonoBehaviour
{
    public enum type{
        level1,
        level2,
        level3
    }
    public type bulletLevel;

    public float speed = 10f;
    public int damage = 10;
    public float lifeTime = 3f;
    private float lifeTimeTimer = 0f;

    private Vector2 position;
    private Vector2 direction;
    public float lv2Acceleration = 0.01f;
    private float lv2Speed = 0;

    [Header("Laser Properties")]
    private LineRenderer lr;
    private BoxCollider2D boxCollider;
    private Vector3[] laserPositions = new Vector3[2];
    public float laserSize = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        lifeTimeTimer = lifeTime;

        position = GetComponent<Transform>().position;

        direction = FindObjectOfType<AOMovement>().getDirection();
        if (FindObjectOfType<AOMovement>().offset.x > 0) direction.x = 1;
        else direction.x = -1;

        if (bulletLevel == type.level3) { 
            lr = GetComponent<LineRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();
            laserPositions[0] = new Vector3(0, 0);
            laserPositions[1] = new Vector3(0, 0);
            lr.startWidth = 0;
            lr.SetPositions(laserPositions);
        }

        //Debug.Log(direction);
    }

    // Update is called once per frame
    void Update()
    {
        //Bullet moves at a fast, constant speed
        if (bulletLevel == type.level1) {
            if (direction.y > 0) { GetComponent<Transform>().position = new Vector2(position.x, transform.position.y + (direction.y * speed * Time.deltaTime)); }    //Fires up
            else GetComponent<Transform>().position = new Vector2(transform.position.x + (direction.x * speed * Time.deltaTime), position.y);                       //Fires left/right
        }
        //Bullet starts slow, but speeds up
        else if (bulletLevel == type.level2) {
            if (direction.y > 0) { GetComponent<Transform>().position = new Vector2(position.x, transform.position.y + (direction.y * (speed * Time.deltaTime + lv2Speed))); }
            else { GetComponent<Transform>().position = new Vector2(transform.position.x + (direction.x * (speed * Time.deltaTime + lv2Speed)), position.y); }
            lv2Speed += lv2Acceleration;
        }
        //Shoots a piercing laser
        else {
            //reset rotation
            transform.rotation = Quaternion.identity;
            //Draw ray to wall or edge of screen
            RaycastHit2D hit;
            Vector2 laserDirection;
            float laserLength = 20f;
            if (direction.y > 0) { laserDirection = new Vector2(0, direction.y); }    //Fires up
            else { laserDirection = new Vector2(direction.x, 0); }                    //Fires left/right
            hit = Physics2D.Raycast(transform.position, laserDirection, laserLength, LayerMask.GetMask("Ground"));

            //Draw laser
            laserPositions[0] = GetComponent<Transform>().position;
            if (hit.point != Vector2.zero) {    //laser hits wall
                laserPositions[1] = hit.point;
            }
            else {  //laser hits nothing (infinite length)
                laserPositions[1] = laserPositions[0] + new Vector3(laserDirection.x * laserLength, laserDirection.y * laserLength);
            }

            lr.SetPositions(laserPositions);

            lr.startWidth = (1 - Mathf.Pow(1 - lifeTimeTimer, 5)) * laserSize;
            SetCollider(lr);
        }
        
        //Destroy after a certain amount of time
        if(lifeTimeTimer > 0){
            lifeTimeTimer -= Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    private void SetCollider(LineRenderer lr)
    {
        if(direction.y > 0) //aim up
        {
            boxCollider.offset = new Vector2(0, (lr.GetPosition(1).y - lr.GetPosition(0).y) / 2); // centered between the 2 endpoints
            boxCollider.size = new Vector2(laserSize, Mathf.Abs(lr.GetPosition(1).y - lr.GetPosition(0).y));
        }
        else // aim left/right
        {
            boxCollider.offset = new Vector2((lr.GetPosition(1).x - lr.GetPosition(0).x) / 2, 0); // centered between the 2 endpoints
            boxCollider.size = new Vector2(Mathf.Abs(lr.GetPosition(1).x - lr.GetPosition(0).x), laserSize);
        }


    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //Hits wall
        if(bulletLevel != type.level3 && col.CompareTag("Ground"))
        {
            Destroy(this.gameObject);
        }
        //Hits Enemy
        if(bulletLevel != type.level3 && col.CompareTag("Enemy"))
        {
            Destroy(this.gameObject);
        }
    }
}
