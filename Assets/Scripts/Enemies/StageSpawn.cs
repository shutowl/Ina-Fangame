using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSpawn : MonoBehaviour
{
    public enum StageState
    {
        waiting,        //Waiting for enemies on screen
        spawning,       //State for spawning enemies
        boss
    }
    public StageState currentState;

    //Room is 20 x 11, Origin fixed on middle of stage
    public int roomLength;
    public int roomHeight;
    public Transform contoller;

    public GameObject[] enemies;
    public GameObject[] bosses;
    public GameObject spawner;
    private bool noEnemies = false;

    public int minWaveLength;
    public int maxWaveLength;
    private int currentStage = 1;
    private int wavesLeft;              //How mnay waves until the boss (0 = BossWave)
    private float waveDuration;         //Max time a wave lasts for


    void Start()
    {
        wavesLeft = Random.Range(minWaveLength, maxWaveLength);
        waveDuration = 2f;
        currentState = StageState.waiting;
    }

    void Update()
    {
        if(currentState == StageState.waiting)
        {
            if (waveDuration <= 0)
            {
                currentState = StageState.spawning;
            }
        }
        else if (currentState == StageState.spawning)
        {
            //Boss wave
            if (wavesLeft == 0)
            {
                switch (currentStage)
                {
                    //Stage 1 Boss: KDTD
                    case 1:
                        StartCoroutine(SpawnStage1Boss());
                        currentState = StageState.boss;
                        break;
                }
            }
            //Normal wave
            else
            {
                switch (currentStage)
                {
                    //Stage 1 Enemies: Takos, others
                    case 1:
                        int wave = Random.Range(1, 2);  //1
                        switch (wave)
                        {
                            //Wave 1
                            case 1:
                                StartCoroutine(SpawnStage1Wave1());
                                waveDuration = 5f;
                                break;
                            //Wave 2
                            case 2:

                                break;
                        }
                        break;
                    //Stage 2 Enemies
                    case 2:
                        break;
                }

                wavesLeft--;
                currentState = StageState.waiting;
            }
        }
        else if(currentState == StageState.boss)
        {
            if (noEnemies)
            {
                wavesLeft = Random.Range(minWaveLength, maxWaveLength);
                waveDuration = 5f;
                currentState = StageState.waiting;
            }
        }

        if(waveDuration > 0)
            waveDuration -= Time.deltaTime;

        noEnemies = (GameObject.FindGameObjectsWithTag("Enemy").Length == 0) ? true : false;
        Debug.Log(GameObject.FindGameObjectsWithTag("Enemy").Length);
    }


    IEnumerator SpawnStage1Wave1()
    {
        spawner.GetComponent<Spawner>().SetSpawn(enemies[0], 1f);
        Instantiate(spawner, contoller.position + new Vector3(4, -4), Quaternion.identity);

        yield return new WaitForSeconds(0.2f);
        spawner.GetComponent<Spawner>().SetSpawn(enemies[0], 1f);
        Instantiate(spawner, contoller.position + new Vector3(5, -4), Quaternion.identity);

        yield return new WaitForSeconds(0.2f);
        spawner.GetComponent<Spawner>().SetSpawn(enemies[0], 1f);
        Instantiate(spawner, contoller.position + new Vector3(6, -4), Quaternion.identity);
    }

    IEnumerator SpawnStage1Boss()
    {
        spawner.GetComponent<Spawner>().SetSpawn(bosses[0], 4f);
        Instantiate(spawner, contoller.position + new Vector3(5, 1), Quaternion.identity);
        yield return null;
    }
}
