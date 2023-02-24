using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float delay = 2f;            //Delay before enemy actually spawns
    private float delayTimer = 0f;
    public GameObject enemy;           //The spawned enemy

    // Start is called before the first frame update
    void Start()
    {
        delayTimer = delay;
    }

    // Update is called once per frame
    void Update()
    {
        delayTimer -= Time.deltaTime;

        if(delayTimer <= 0)
        {
            Instantiate(enemy, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    public void SetSpawn(GameObject enemy, float delay)
    {
        this.enemy = enemy;
        this.delay = delay;
    }

}
