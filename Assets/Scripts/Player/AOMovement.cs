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
    private Animator anim;
    private float scale = 0.75f;

    public GameObject[] bullets = new GameObject[4];

    void Start()
    {
        savedOffset = offset;
        savedSpeed = smoothSpeed;
        autoAmountCounter = autoAmount;

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        direction = player.GetComponent<PlayerMovement>().GetInputActions().Player.Move.ReadValue<Vector2>();
        direction.x = (player.GetComponent<Transform>().localScale.x > 0) ? 1 : -1;
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
                transform.localEulerAngles = Vector3.zero;
            }
            //Charge and Fire Mechanics
            if (isCharging)
            {
                if (direction.y > 0) //aim upwards
                {
                    offset = new Vector3(0, 1.25f, 0);
                    transform.localScale = new Vector2(direction.x * scale, scale);
                    if (direction.x > 0) transform.localEulerAngles = new Vector3(0, 0, 90);
                    else transform.localEulerAngles = new Vector3(0, 0, -90);
                }
                else
                { //aim left or right
                    offset = new Vector3(direction.x * 1.25f, 0, 0);
                    transform.localEulerAngles = Vector3.zero;
                    transform.localScale = new Vector2(direction.x * scale, scale);
                } 

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
                    Instantiate(bullets[0], player.transform.position + offset, Quaternion.identity);

                    AudioManager.Instance.Play("PlayerBullet");
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

        //Animations
        anim.SetBool("charging", isCharging || firePosDelayCounter > 0);
    }

    public void Charge()
    {
        firePosDelayCounter = firePosDelay;
        smoothSpeed = savedSpeed * 2;
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
                {
                    Instantiate(bullets[0], player.transform.position + offset, Quaternion.identity);
                    AudioManager.Instance.Play("PlayerBullet");
                }
                break;
            case 1:
                //Debug.Log("Fired Lv 2 shot");   //piercing laser
                GameObject laser = Instantiate(bullets[2], player.transform.position + offset, Quaternion.identity);
                if(direction.y > 0)
                    laser.GetComponent<aoBullet>().SetPositions(Vector2.zero, Vector2.up * 30);
                else
                    laser.GetComponent<aoBullet>().SetPositions(Vector2.zero, 30 * direction.x * Vector2.right);

                AudioManager.Instance.Play("PlayerLaser");
                break;
            default:
                //Debug.Log("Fired Lv 3 shot");   //stronger piercing laser
                GameObject laser2 = Instantiate(bullets[2], player.transform.position + offset, Quaternion.identity);
                if (direction.y > 0)
                    laser2.GetComponent<aoBullet>().SetPositions(Vector2.zero, Vector2.up * 30);
                else
                    laser2.GetComponent<aoBullet>().SetPositions(Vector2.zero, 30 * direction.x * Vector2.right);

                AudioManager.Instance.Play("PlayerLaser");
                break;
        }
        chargeTime = 0f;
        if(autoFireRateTimer <= 0) autoFireRateTimer = autoFireRate;
        autoCDTimer = autoCD/2;
        autoAmountCounter = autoAmount;

        //offset = savedOffset;
    }

    public void Reset()
    {
        isCharging = false;
        chargeTime = 0f;
        chargeLevel = 0;
    }

    public Vector2 getDirection()
    {
        return direction;
    }

}
