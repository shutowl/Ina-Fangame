using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOMovement : MonoBehaviour
{
    public GameObject player;
    public float smoothSpeed = 10f;
    private float savedSpeed;
    public Vector3 offset;
    private Vector3 savedOffset;
    public float firePosDelay = 0.3f;           //Time AO stays in front of player before going back to default position
    private float firePosDelayCounter = -1;
    public float autoFireRate = 0.3f;           //Fire rate of lv0 bullets when player is charging
    private float autoFireRateTimer = 0f;
    public int autoAmount = 4;                  //Number of auto bullets fired per volley
    private int autoAmountCounter;
    public float autoCD = 1f;                   //Cooldown period inbetween auto volleys
    private float autoCDTimer = 0f;
    bool isCharging = false;
    private float chargeTime;
    [SerializeField] private int chargeLevel;
    private Vector2 direction;

    public GameObject[] bullets = new GameObject[4];

    void Start()
    {
        savedOffset = offset;
        savedSpeed = smoothSpeed;
        autoAmountCounter = autoAmount;
    }

    void Update()
    {
        direction = player.GetComponent<PlayerMovement>().getInputActions().Player.Move.ReadValue<Vector2>();
        if (player.GetComponent<SpriteRenderer>().flipX) direction.x = -1;
        else direction.x = 1;
        //Debug.Log(direction);
    }

    void FixedUpdate()
    {
        Vector3 desiredPosition = player.transform.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, 1 - Mathf.Cos((smoothSpeed * Time.deltaTime)*Mathf.PI / 2));
        transform.position = smoothedPosition;

        if (player.GetComponent<PlayerMovement>().currentState != PlayerMovement.playerState.inCutscene)
        {
            if (!isCharging && firePosDelayCounter <= 0)
            {
                offset = new Vector3(-direction.x, savedOffset.y, 0);
                smoothSpeed = savedSpeed;
            }

            //Charge and Fire Mechanics
            if (isCharging)
            {
                if(direction.y > 0) //aim upwards
                {
                    offset = new Vector3(0, 1.25f, 0);
                }
                else { offset = new Vector3(direction.x * 1.25f, 0, 0); }  //aim left

                if (chargeTime < 3.0f)
                    chargeTime += Time.deltaTime;
                /*
                //DEBUG DrawRay
                RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 100f, LayerMask.GetMask("Ground")); //layer mask "Ground" = 7
                if (hit.collider != null)
                {
                    Debug.Log("Raycast: " + hit.point);
                    Debug.DrawRay(transform.position, new Vector2(hit.distance * direction, 0), Color.red);
                }
                */
            
                //Fire lv0 bullet every so often
                if (autoFireRateTimer <= 0 && autoAmountCounter > 0 && autoCDTimer <= 0)
                {
                    autoFireRateTimer = autoFireRate;
                    Instantiate(bullets[0], player.transform.position + offset, Quaternion.Euler(new Vector3(0, 0, 90 * offset.x)));
                    autoAmountCounter--;
                }
                if(autoAmountCounter <= 0)
                {
                    autoAmountCounter = autoAmount;
                    autoCDTimer = autoCD;
                }
            }

            if (chargeTime > 0f && chargeTime <= 1f) chargeLevel = 0;
            else if (chargeTime > 1.5f && chargeTime <= 3f) chargeLevel = 1;
            else if (chargeTime > 3f) chargeLevel = 2;

            //Timers
            if (firePosDelayCounter > 0)
                firePosDelayCounter -= Time.deltaTime;
            if (autoFireRateTimer > 0)
                autoFireRateTimer -= Time.deltaTime;
            if (autoCDTimer > 0)
                autoCDTimer -= Time.deltaTime;
        }
        else
        {

        }

    }

    public void Charge()
    {
        firePosDelayCounter = firePosDelay;
        smoothSpeed = savedSpeed * 3;
        isCharging = true;
    }
    
    public void Fire()
    {
        isCharging = false;

        switch (chargeLevel)
        {
            case 0:
                //Debug.Log("Fired Lv 0 shot");   //small spammable projectile
                //instantiate a bullet in AO's direction and position
                if (autoFireRateTimer <= 0)
                    Instantiate(bullets[0], player.transform.position + offset, Quaternion.Euler(new Vector3(0, 0, 90 * offset.x)));
                break;
            case 1:
                //Debug.Log("Fired Lv 2 shot");   //piercing laser
                Instantiate(bullets[2], player.transform.position + offset, Quaternion.identity);
                break;
            default:
                //Debug.Log("Fired Lv 3 shot");   //stronger piercing laser
                Instantiate(bullets[2], player.transform.position + offset, Quaternion.identity);
                break;
        }
        chargeTime = 0f;
        autoFireRateTimer = autoFireRate/2;
        autoCDTimer = autoCD/2;
        autoAmountCounter = autoAmount;

        //offset = savedOffset;
    }

    public Vector2 getDirection()
    {
        return direction;
    }

}
