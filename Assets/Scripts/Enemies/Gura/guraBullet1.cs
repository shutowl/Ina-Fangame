using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class guraBullet1 : MonoBehaviour
{

    public float speed = 10f;
    public float lifeTime = 3f;
    private float lifeTimeTimer = 0f;

    private float x = 0, y = -1;
    private Vector2 direction;

    // Start is called before the first frame update
    void Awake()
    {
        direction = new Vector2(x, y).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Transform>().position = new Vector2(GetComponent<Transform>().position.x + (speed * Time.deltaTime * direction.x), GetComponent<Transform>().position.y + (speed * Time.deltaTime * direction.y));

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
    }

    public void setDirection(float x, float y){
        //Debug.Log("Set Direction: (" + x + ", " + y + ")");
        direction = new Vector2(x, y).normalized;
    }
}
