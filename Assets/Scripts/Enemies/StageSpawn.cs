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
                        int wave = Random.Range(3, 7); //1-6
                        StartCoroutine(SpawnStage1Wave(wave));
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
        //Debug.Log(GameObject.FindGameObjectsWithTag("Enemy").Length);
    }

    public int GetCurrentWave()
    {
        return wavesLeft;
    }

    public int GetCurrentStage()
    {
        return currentStage;
    }

    public float GetTimeLeft()
    {
        return waveDuration;
    }

    IEnumerator SpawnStage1Wave(int wave)
    {
        switch (wave)
        {
            case 1:
                waveDuration = 5f;

                spawner.GetComponent<Spawner>().SetSpawn(enemies[0], 1f);
                Instantiate(spawner, contoller.position + new Vector3(4, -4), Quaternion.identity);

                yield return new WaitForSeconds(0.2f);
                spawner.GetComponent<Spawner>().SetSpawn(enemies[0], 1f);
                Instantiate(spawner, contoller.position + new Vector3(5, -4), Quaternion.identity);

                yield return new WaitForSeconds(0.2f);
                spawner.GetComponent<Spawner>().SetSpawn(enemies[0], 1f);
                Instantiate(spawner, contoller.position + new Vector3(6, -4), Quaternion.identity);
                break;
            case 2:
                waveDuration = 5f;

                spawner.GetComponent<Spawner>().SetSpawn(enemies[0], 1f);
                Instantiate(spawner, contoller.position + new Vector3(-4, -4), Quaternion.identity);

                yield return new WaitForSeconds(0.2f);
                spawner.GetComponent<Spawner>().SetSpawn(enemies[0], 1f);
                Instantiate(spawner, contoller.position + new Vector3(-5, -4), Quaternion.identity);

                yield return new WaitForSeconds(0.2f);
                spawner.GetComponent<Spawner>().SetSpawn(enemies[0], 1f);
                Instantiate(spawner, contoller.position + new Vector3(-6, -4), Quaternion.identity);
                break;
            case 3:
                waveDuration = 7f;

                enemies[1].GetComponent<FlyingTako>().ResetValues();

                spawner.GetComponent<Spawner>().SetSpawn(enemies[1], 1f);
                Instantiate(spawner, contoller.position + new Vector3(-4, 4), Quaternion.identity);

                yield return new WaitForSeconds(0.5f);
                spawner.GetComponent<Spawner>().SetSpawn(enemies[1], 1f);
                Instantiate(spawner, contoller.position + new Vector3(-5, 3), Quaternion.identity);

                yield return new WaitForSeconds(0.5f);
                spawner.GetComponent<Spawner>().SetSpawn(enemies[1], 1f);
                Instantiate(spawner, contoller.position + new Vector3(-6, 2), Quaternion.identity);
                break;

            case 4:
                waveDuration = 10f;

                enemies[1].GetComponent<FlyingTako>().ResetValues();
                enemies[1].GetComponent<FlyingTako>().SetDuration(7f);
                enemies[1].GetComponent<FlyingTako>().SetFollow(false);
                enemies[1].GetComponent<FlyingTako>().SetDirection(0, -1);

                spawner.GetComponent<Spawner>().SetSpawn(enemies[1], 0.5f);
                Instantiate(spawner, contoller.position + new Vector3(-8, 4), Quaternion.identity);

                spawner.GetComponent<Spawner>().SetSpawn(enemies[1], 0.5f);
                Instantiate(spawner, contoller.position + new Vector3(8, 4), Quaternion.identity);

                yield return new WaitForSeconds(0.2f);

                spawner.GetComponent<Spawner>().SetSpawn(enemies[1], 0.5f);
                Instantiate(spawner, contoller.position + new Vector3(-8, 3.8f), Quaternion.identity);

                spawner.GetComponent<Spawner>().SetSpawn(enemies[1], 0.5f);
                Instantiate(spawner, contoller.position + new Vector3(8, 3.8f), Quaternion.identity);

                yield return new WaitForSeconds(0.2f);

                spawner.GetComponent<Spawner>().SetSpawn(enemies[1], 0.5f);
                Instantiate(spawner, contoller.position + new Vector3(-8, 3.6f), Quaternion.identity);

                spawner.GetComponent<Spawner>().SetSpawn(enemies[1], 0.5f);
                Instantiate(spawner, contoller.position + new Vector3(8, 3.6f), Quaternion.identity);

                yield return new WaitForSeconds(0.2f);

                spawner.GetComponent<Spawner>().SetSpawn(enemies[1], 0.5f);
                Instantiate(spawner, contoller.position + new Vector3(-8, 3.4f), Quaternion.identity);

                spawner.GetComponent<Spawner>().SetSpawn(enemies[1], 0.5f);
                Instantiate(spawner, contoller.position + new Vector3(8, 3.4f), Quaternion.identity);
                break;
            case 5:
                waveDuration = 5f;

                enemies[1].GetComponent<FlyingTako>().ResetValues();
                enemies[1].GetComponent<FlyingTako>().SetFireRate(0.05f);
                enemies[1].GetComponent<FlyingTako>().SetDuration(1f);
                enemies[1].GetComponent<FlyingTako>().SetFollow(false);
                enemies[1].GetComponent<FlyingTako>().SetDirection(0, -1);

                spawner.GetComponent<Spawner>().SetSpawn(enemies[1], 0.5f);
                Instantiate(spawner, contoller.position + new Vector3(-8, 4), Quaternion.identity);
                break;
            case 6:
                waveDuration = 12f;

                enemies[1].GetComponent<FlyingTako>().ResetValues();
                enemies[1].GetComponent<FlyingTako>().SetFireRate(0.1f);
                enemies[1].GetComponent<FlyingTako>().SetDuration(5f);
                enemies[1].GetComponent<FlyingTako>().SetFollow(false);
                enemies[1].GetComponent<FlyingTako>().SetDirection(0, -1);

                enemies[1].GetComponent<FlyingTako>().SetLifeTime(4f);
                spawner.GetComponent<Spawner>().SetSpawn(enemies[1], 0.5f);
                Instantiate(spawner, contoller.position + new Vector3(9, 4), Quaternion.identity);
                yield return new WaitForSeconds(0.2f);
                Instantiate(spawner, contoller.position + new Vector3(9, 4), Quaternion.identity);
                yield return new WaitForSeconds(0.2f);
                Instantiate(spawner, contoller.position + new Vector3(9, 4), Quaternion.identity);
                yield return new WaitForSeconds(0.2f);
                Instantiate(spawner, contoller.position + new Vector3(9, 4), Quaternion.identity);
                yield return new WaitForSeconds(0.2f);
                Instantiate(spawner, contoller.position + new Vector3(9, 4), Quaternion.identity);

                yield return new WaitForSeconds(5);
                enemies[1].GetComponent<FlyingTako>().SetLifeTime(1f);
                enemies[1].GetComponent<FlyingTako>().SetDirection(-1, 0);
                Instantiate(spawner, contoller.position + new Vector3(8, -5.5f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(8, -5f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(8, -4.5f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(8, -4f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(8, -3.5f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(8, -3f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(8, 5.5f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(8, 5f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(8, 4.5f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(8, 4f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(8, 3.5f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(8, 3f), Quaternion.identity);

                yield return new WaitForSeconds(2);
                enemies[1].GetComponent<FlyingTako>().SetLifeTime(3f);
                enemies[1].GetComponent<FlyingTako>().SetDuration(20f);
                enemies[1].GetComponent<FlyingTako>().SetDirection(0, -1);
                Instantiate(spawner, contoller.position + new Vector3(-8, 5f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(-6, 5f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(-4, 5f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(-2, 5f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(0, 5f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(2, 5f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(4, 5f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(6, 5f), Quaternion.identity);
                Instantiate(spawner, contoller.position + new Vector3(8, 5f), Quaternion.identity);

                break;
        }
    }

    IEnumerator SpawnStage1Boss()
    {
        spawner.GetComponent<Spawner>().SetSpawn(bosses[0], 4f);
        Instantiate(spawner, contoller.position + new Vector3(5, 1), Quaternion.identity);
        yield return null;
    }
}
