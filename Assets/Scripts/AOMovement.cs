using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOMovement : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 10f;
    private float savedSpeed;
    public Vector3 offset;
    private Vector3 savedOffset;
    public float firePosDelay = 0.3f;           //Time AO stays in front of player before going back to default position
    private float firePosDelayCounter = -1;
    public float chargeBulletFireRate = 0.3f;   //Fire rate of lv0 bullets when player is charging
    private float chargeBulletFireRateTimer = 0f;
    private float flip;
    bool isCharging = false;
    private float chargeTime;
    [SerializeField] private int chargeLevel;
    private Vector2 direction;

    public GameObject[] bullets = new GameObject[4];

    void Start()
    {
        flip = offset.x;
        savedOffset = offset;
        savedSpeed = smoothSpeed;
    }

    void Update()
    {
        direction = FindObjectOfType<PlayerMovement>().getInputActions().Player.Move.ReadValue<Vector2>();
        Debug.Log(direction);
    }

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, 1 - Mathf.Cos((smoothSpeed * Time.deltaTime)*Mathf.PI / 2));
        transform.position = smoothedPosition;

        if (!isCharging && firePosDelayCounter <= 0)
        {
            if (target.GetComponent<SpriteRenderer>().flipX) { offset = new Vector3(flip, savedOffset.y, 0); }
            else { offset = new Vector3(flip, savedOffset.y, 0); }
            smoothSpeed = savedSpeed;
        }

        //Charge and Fire Mechanics
        if (isCharging)
        {
            if(direction.y > 0)
            {
                offset = new Vector3(0, 1.25f, 0);
            }
            else if (target.GetComponent<SpriteRenderer>().flipX) { offset = new Vector3(flip +0.25f, 0, 0); }
            else { offset = new Vector3(-flip -0.25f, 0, 0); }

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
            if (chargeBulletFireRateTimer <= 0)
            {
                chargeBulletFireRateTimer = chargeBulletFireRate;
                Instantiate(bullets[0], target.position + offset, Quaternion.Euler(0, 0, direction.x * 270f));
            }
        }

        if (chargeTime > 0f && chargeTime <= 0.5f) chargeLevel = 0;
        else if (chargeTime > 0.5f && chargeTime <= 1.5f) chargeLevel = 1;
        else if (chargeTime > 1.5f && chargeTime <= 3f) chargeLevel = 2;
        else if (chargeTime > 3f) chargeLevel = 2;

        //Timers
        if (firePosDelayCounter > 0)
            firePosDelayCounter -= Time.deltaTime;
        if (chargeBulletFireRateTimer > 0)
            chargeBulletFireRateTimer -= Time.deltaTime;

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
                if (chargeBulletFireRateTimer <= 0)
                    Instantiate(bullets[0], target.position + offset,  Quaternion.Euler(0, 0, direction.x * 270f));
                break;
            case 1:
                //Debug.Log("Fired Lv 1 shot");   //slower but stronger projectile
                Instantiate(bullets[1], target.position + offset, Quaternion.Euler(0, 0, direction.x * 270f));   
                break;
            case 2:
                //Debug.Log("Fired Lv 2 shot");   //piercing laser
                Instantiate(bullets[2], target.position + offset, Quaternion.Euler(0, 0, direction.x * 270f));
                break;
            default:
                //Debug.Log("Fired Lv 3 shot");   //stronger piercing laser
                Instantiate(bullets[2], target.position + offset, Quaternion.Euler(0, 0, direction.x * 270f));
                break;
        }
        chargeTime = 0f;

        //offset = savedOffset;
    }

    public Vector2 getDirection()
    {
        return direction;
    }

}
