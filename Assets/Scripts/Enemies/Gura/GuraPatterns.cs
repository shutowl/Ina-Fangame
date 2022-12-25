using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuraPatterns : MonoBehaviour
{
    public enum pattern{
        normal,             //normal = alternate between normal attacks
        spiral,             //Moves to upper center of screen and creates spiral of bullets
        spiral2,
        pulse,
        rain,               //Activates bullets raining from above for some time
        geyser,             //Actives bullets exploding from floor for some time
        rest
    }
    public pattern currentState;

    [Header ("Others")]
    public float fireRate = 0.5f;           //Rate at which bullets are fired
    private float fireRateTimer = 0f;
    public int density = 5;                 //Number of bullets per wave
    public float bulletOffsetRate = 0.01f;  //Rate at which angle of bullets turn
    private float bulletOffset = 0;
    public float accelOffsetRate = 0.025f;
    private float accelOffset = 0;

    [Header ("Attack Duration")]
    public float spiralDuration = 5f;   //Duration of each attack
    public float rainDuration = 10f;
    public float geyserDuration = 5f;
    public float restPeriod = 3f;       //Time between attacks

    private float timer = 0f;

    public GameObject[] bullets;

    void Start(){
    }

    void Update()
    {
        if(currentState == pattern.spiral){
            fireRate = 0.3f;
            density = 10;
            bulletOffsetRate = 0.05f;

            if(fireRateTimer > 0) fireRateTimer -= Time.deltaTime;
            else{
                for(int i = 0; i < density; i++){
                    float angle = (i * Mathf.PI * 2 / density) + bulletOffset;
                    float x = Mathf.Cos(angle);
                    float y = Mathf.Sin(angle);
                    Vector2 pos = (Vector2)transform.position + new Vector2(x, y);
                    float angleDegrees = -angle * Mathf.Rad2Deg;
                    Quaternion rot = Quaternion.Euler(0, 0, angleDegrees);

                    GameObject bullet = Instantiate(bullets[0], pos, rot);
                    bullet.GetComponent<guraBullet1>().setDirection(x,y);
                    fireRateTimer = fireRate;
                }
                bulletOffset += bulletOffsetRate;
            }
            
            if(timer <= 0) rest();
        }
        else if(currentState == pattern.spiral2){
            if (fireRate > 0.1f)
                fireRate = 0.3f - accelOffset;
            density = 6;
            bulletOffsetRate = 0.05f + (accelOffset * 2);

            if(fireRateTimer > 0) fireRateTimer -= Time.deltaTime;
            else{
                for(int i = 0; i < density; i++){
                    float angle = (i * Mathf.PI * 2 / density) + bulletOffset;
                    float x = Mathf.Cos(angle);
                    float y = Mathf.Sin(angle);
                    Vector2 pos = (Vector2)transform.position + new Vector2(x, y);
                    float angleDegrees = -angle * Mathf.Rad2Deg;
                    Quaternion rot = Quaternion.Euler(0, 0, angleDegrees);

                    GameObject bullet = Instantiate(bullets[0], pos, rot);
                    bullet.GetComponent<guraBullet1>().setDirection(x,y);
                    fireRateTimer = fireRate;
                }
                accelOffset += accelOffsetRate;
                bulletOffset += bulletOffsetRate;
            }

            if (timer <= 0){
                fireRate = 0.3f;
                rest();
            }
        }
        else if(currentState == pattern.pulse){
            fireRate = 1.5f;
            density = 50;
            bulletOffsetRate = 0f;

            if(fireRateTimer > 0) fireRateTimer -= Time.deltaTime;
            else{
                for(int i = 0; i < density; i++){
                    float angle = (i * Mathf.PI * 2 / density) + bulletOffset;
                    float x = Mathf.Cos(angle);
                    float y = Mathf.Sin(angle);
                    Vector2 pos = (Vector2)transform.position + new Vector2(x, y);
                    float angleDegrees = -angle * Mathf.Rad2Deg;
                    Quaternion rot = Quaternion.Euler(0, 0, angleDegrees);

                    GameObject bullet = Instantiate(bullets[0], pos, rot);
                    bullet.GetComponent<guraBullet1>().setDirection(x,y);
                    fireRateTimer = fireRate;
                }
                bulletOffset += bulletOffsetRate;
            }
            
            if(timer <= 0) rest();
        }
        else if(currentState == pattern.rest){

            if(timer <= 0){
                int nextState = (int)Random.Range(1,4);
                //int nextState = 2;
                currentState = (pattern)nextState;
                //go back to normal attack or another pattern?
                switch(nextState){
                    case 0:
                    case 1:
                        timer = spiralDuration;
                        currentState = pattern.spiral;
                        break;
                    case 2:
                        timer = spiralDuration;
                        currentState = pattern.spiral2;
                        break;
                    case 3:
                        timer = spiralDuration;
                        currentState = pattern.pulse;
                        break;
                }
                
            }
        }

        if(timer > 0) timer -= Time.deltaTime;  //timer always counting down
    }

    void rest(){
        //bulletOffset = 0;
        accelOffset = 0;
        timer = restPeriod;
        currentState = pattern.rest;
    }
}
