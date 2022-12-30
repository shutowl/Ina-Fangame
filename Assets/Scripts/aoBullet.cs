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
    private EdgeCollider2D edgeCollider;
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
            edgeCollider = GetComponent<EdgeCollider2D>();
            laserPositions[0] = new Vector3(0, 0);
            laserPositions[1] = new Vector3(0, 0);
            lr.startWidth = 0;
            lr.SetPositions(laserPositions);
        }
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
            //Draw ray or line to wall or edge of screen
            RaycastHit2D hit;
            if (direction.y > 0) { hit = Physics2D.Raycast(transform.position, new Vector2(0, direction.y), 100f, LayerMask.GetMask("Ground")); }
            else { hit = Physics2D.Raycast(transform.position, new Vector2(direction.x, 0), 100f, LayerMask.GetMask("Ground")); }
            //Draw laser
            laserPositions[0] = GetComponent<Transform>().position;
            laserPositions[1] = hit.point;
            lr.SetPositions(laserPositions);

            lr.startWidth = (1 - Mathf.Pow(1 - lifeTimeTimer, 5)) * laserSize;
            SetEdgeCollider(lr);
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

    private void SetEdgeCollider(LineRenderer lr)
    {
        List<Vector2> edges = new();

        edges.Add(Vector2.zero);
        if(direction.y > 0)
        {
            edges.Add(new Vector2(0, (lr.GetPosition(1).y - lr.GetPosition(0).y) * direction.y));
        }
        else
        {
            edges.Add(new Vector2(0, (lr.GetPosition(1).x - lr.GetPosition(0).x) * direction.x));
        }
        //Debug.Log(lr.GetPosition(0));
        //Debug.Log(lr.GetPosition(1));

        edgeCollider.SetPoints(edges);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //Hits wall or exceeds lifetime
        if(col.tag == "Ground")
        {
            Destroy(this.gameObject);
        }
        //Hits Enemy
        if(bulletLevel != type.level3 && col.tag == "Enemy")
        {
            //Enemy gets Damaged
            Destroy(this.gameObject);
        }
    }
}
