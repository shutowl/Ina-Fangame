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

    private float y;
    private float direction;
    public float lv2Acceleration = 0.01f;
    private float lv2Speed = 0;

    // Start is called before the first frame update
    void Start()
    {
        y = GetComponent<Transform>().position.y;

        if(FindObjectOfType<AOMovement>().offset.x > 0) direction = 1;
        else direction = -1;
    }

    // Update is called once per frame
    void Update()
    {
        //Bullet moves at a fast, constant speed
        if(bulletLevel == type.level1){
            GetComponent<Transform>().position = new Vector2(GetComponent<Transform>().position.x + (direction * speed * Time.deltaTime), y);
        }
        //Bullet starts slow, but speeds up
        else if(bulletLevel == type.level2){
            GetComponent<Transform>().position = new Vector2(GetComponent<Transform>().position.x + (direction * (speed * Time.deltaTime + lv2Speed)), y);
            lv2Speed += lv2Acceleration;
        }
        //Shoots a piercing laser
        else{
            //Draw ray or line to wall or edge of screen
        }
        
        //Destroy after a certain amount of time
        lifeTimeTimer += Time.deltaTime;
        if(lifeTimeTimer > lifeTime){
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //Hits wall or exceeds lifetime
        if(col.tag == "Ground")
        {
            Destroy(this.gameObject);
        }
        //Hits Enemy
        if(col.tag == "Enemy")
        {
            //Enemy gets Damaged
            Destroy(this.gameObject);
        }
    }
}
