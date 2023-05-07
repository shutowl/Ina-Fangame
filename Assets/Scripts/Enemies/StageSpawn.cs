using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageSpawn : MonoBehaviour
{
    public enum StageState
    {
        waiting,        //Waiting for enemies on screen
        spawning,       //State for spawning enemies
        boss
    }
    public StageState currentState;
    public bool inOrder = false;

    //Room is 20 x 11, Origin fixed on middle of stage
    public int roomLength;
    public int roomHeight;
    public Transform contoller;
    public TextMeshProUGUI mainStageText;
    public TextMeshProUGUI subStageText;
    public GameObject StageUI;

    public GameObject[] enemies;
    public GameObject[] bosses;
    public GameObject spawner;
    private bool noEnemies = false;

    public int minWaveLength;
    public int maxWaveLength;
    private int currentStage = 1;
    private int stageRow = 0;           //Determines generation of stage (Ex: Myth)
    private int stageCol = 0;           //Determines main talent of generation (Ex: Gura)
    bool collabStage = false;
    int wave;
    private int wavesLeft;              //How mnay waves until the boss (0 = BossWave)
    private float waveDuration;         //Max time a wave lasts for

    private float bossTimer = 0f;       //Used to prevent other waves from spawning during boss wave

    ComboMeter comboMeter;


    void Start()
    {
        wavesLeft = Random.Range(minWaveLength, maxWaveLength);
        waveDuration = 5f;
        currentState = StageState.waiting;

        comboMeter = FindObjectOfType<ComboMeter>();
        StageUI.SetActive(false);
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
                bossTimer = 10f;
                switch (stageRow)
                {
                    //Myth Gen
                    case 0:
                        switch (stageCol)
                        {
                            case 0: //Myth

                                break;
                            case 1: //Gura
                                StartCoroutine(SpawnGuraBoss());
                                currentState = StageState.boss;
                                break;
                            case 2: //Calli

                                break;
                            case 3: //Ame

                                break;
                            case 4: //Kiara

                                break;
                        }
                        break;
                    //Council Gen
                    case 1:
                        switch (stageCol)
                        {
                            case 0: //Council

                                break;
                            case 1: //Bae

                                break;
                            case 2: //Mumei

                                break;
                            case 3: //Fauna

                                break;
                            case 4: //Kronii

                                break;
                            case 5: //Sana

                                break;
                        }
                        break;
                }
            }
            //Normal wave
            else
            {
                switch (stageRow)
                {
                    //Myth Gen
                    case 0:
                        switch (stageCol)
                        {
                            case 0: //Myth

                                break;
                            case 1: //Gura
                                if (inOrder)
                                {
                                    wave = Mathf.Clamp((wave + 1) % 3, 1, 2);   //Clamp((wave + 1) % numOfAttacks+1, 1, numOfAttacks)
                                }
                                else
                                {
                                    wave = Random.Range(1, 3); //1-2
                                }
                                StartCoroutine(SpawnGuraWave(wave));
                                break;
                            case 2: //Calli

                                break;
                            case 3: //Ame

                                break;
                            case 4: //Kiara

                                break;
                        }
                        break;
                    //Council Gen
                    case 1:
                        switch (stageCol)
                        {
                            case 0: //Council

                                break;
                            case 1: //Bae

                                break;
                            case 2: //Mumei

                                break;
                            case 3: //Fauna

                                break;
                            case 4: //Kronii

                                break;
                            case 5: //Sana

                                break;
                        }
                        break;
                }

                wavesLeft--;
                currentState = StageState.waiting;
            }
        }
        else if(currentState == StageState.boss)
        {
            if (noEnemies && bossTimer <= 0)
            {
                //Check if single or collab stage
                //If single, finish level
                if (!collabStage)
                {
                    CompleteStage();
                }
                //Otherwise, continue to next stage
                else
                {
                    wavesLeft = Random.Range(minWaveLength, maxWaveLength);
                    waveDuration = 5f;
                    currentState = StageState.waiting;
                }
            }
            bossTimer -= Time.deltaTime;
        }

        if(waveDuration > 0)
            waveDuration -= Time.deltaTime;

        noEnemies = (GameObject.FindGameObjectsWithTag("Enemy").Length == 0);
        comboMeter.SetStop(noEnemies);
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

    //The player finishes the level and statistics are shown
    //Menu prompt will also show to either retry or go back
    void CompleteStage()
    {
        StartCoroutine(FindObjectOfType<PlayerMovement>().PausePlayer(true));
        StageUI.SetActive(true);
        Debug.Log("Stage Complete");
    }

    IEnumerator SpawnTutorialWave(int wave)
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

    IEnumerator SpawnTutorialBoss()
    {
        spawner.GetComponent<Spawner>().SetSpawn(bosses[0], 4f);
        Instantiate(spawner, contoller.position + new Vector3(5, 1), Quaternion.identity);

        yield return null;
    }

    IEnumerator SpawnGuraWave(int wave)
    {
        switch (wave)
        {
            case 1:
                waveDuration = 5f;

                spawner.GetComponent<Spawner>().SetSpawn(enemies[2], 1f);
                Instantiate(spawner, contoller.position + new Vector3(4, -4), Quaternion.identity);

                yield return new WaitForSeconds(0.2f);
                spawner.GetComponent<Spawner>().SetSpawn(enemies[2], 1f);
                Instantiate(spawner, contoller.position + new Vector3(5, -4), Quaternion.identity);

                yield return new WaitForSeconds(0.2f);
                spawner.GetComponent<Spawner>().SetSpawn(enemies[2], 1f);
                Instantiate(spawner, contoller.position + new Vector3(6, -4), Quaternion.identity);
                break;
            case 2:
                waveDuration = 5f;

                spawner.GetComponent<Spawner>().SetSpawn(enemies[2], 1f);
                Instantiate(spawner, contoller.position + new Vector3(-4, -4), Quaternion.identity);

                yield return new WaitForSeconds(0.2f);
                spawner.GetComponent<Spawner>().SetSpawn(enemies[2], 1f);
                Instantiate(spawner, contoller.position + new Vector3(-5, -4), Quaternion.identity);

                yield return new WaitForSeconds(0.2f);
                spawner.GetComponent<Spawner>().SetSpawn(enemies[2], 1f);
                Instantiate(spawner, contoller.position + new Vector3(-6, -4), Quaternion.identity);
                break;
        }
    }

    IEnumerator SpawnGuraBoss()
    {
        spawner.GetComponent<Spawner>().SetSpawn(bosses[1], 4f);
        Instantiate(spawner, contoller.position + new Vector3(5, 1), Quaternion.identity);

        yield return null;
    }

    //Load variables on entering scene
    private void OnEnable()
    {
        stageRow = PlayerPrefs.GetInt("stageRow");
        stageCol = PlayerPrefs.GetInt("stageCol");

        switch (stageRow)
        {
            case 0:
                switch (stageCol)
                {
                    case 0: //Myth
                        collabStage = true;
                        mainStageText.text = "Main Stage";
                        subStageText.text = "Myth Collab";
                        break;
                    case 1: //Gura
                        collabStage = false;
                        mainStageText.text = "Atlantis";
                        subStageText.text = "Gura's stage";
                        break;
                    case 2: //Calli
                        
                        break;
                    case 3: //Ame
                        
                        break;
                    case 4: //Kiara
                        
                        break;
                }
                break;
            case 1:
                switch (stageCol)
                {
                    case 0: //Council
                        
                        break;
                    case 1: //Bae
                        
                        break;
                    case 2: //Mumei
                        
                        break;
                    case 3: //Fauna
                        
                        break;
                    case 4: //Kronii
                        
                        break;
                    case 5: //Sana
                        
                        break;
                }
                break;
        }
    }
}
