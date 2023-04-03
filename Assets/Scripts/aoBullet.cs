using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

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
    private VolumetricLineBehavior line;
    private EdgeCollider2D hurtbox;
    public Vector3 startPos;
    public Vector3 endPos;
    public float width = 2f;

    void Start()
    {
        lifeTimeTimer = lifeTime;

        position = GetComponent<Transform>().position;

        direction = FindObjectOfType<AOMovement>().getDirection();
        if (FindObjectOfType<AOMovement>().offset.x > 0) direction.x = 1;
        else direction.x = -1;

        if(bulletLevel == type.level3)
        {
            //Laser properties
            line = GetComponent<VolumetricLineBehavior>();
            hurtbox = GetComponent<EdgeCollider2D>();
            line.SetStartAndEndPoints(startPos, endPos);
            hurtbox.SetPoints(new List<Vector2>() { startPos, endPos });
        }
    }

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
            //Draw laser
            line.LineWidth = (1 - Mathf.Pow(1 - lifeTimeTimer / lifeTime, 5)) * width;
            hurtbox.edgeRadius = (1 - Mathf.Pow(1 - lifeTimeTimer / lifeTime, 5)) * width * 0.15f;
            line.SetStartAndEndPoints(startPos, endPos);
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

    public void SetPositions(Vector3 start, Vector3 end)
    {
        startPos = start;
        endPos = end;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //Hits wall
        if(bulletLevel != type.level3 && col.CompareTag("Ground"))
        {
            Destroy(this.gameObject);
        }
    }
}
