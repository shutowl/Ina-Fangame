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
    public float firePosDelay = 0.3f;
    private float firePosDelayCounter = -1;
    private float flip;
    bool isCharging = false;
    private float chargeTime;
    [SerializeField] private int chargeLevel;

    void Start()
    {
        flip = offset.x;
        savedOffset = offset;
        savedSpeed = smoothSpeed;
    }

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, 1 - Mathf.Cos((smoothSpeed * Time.deltaTime)*Mathf.PI / 2));
        transform.position = smoothedPosition;

        if (!isCharging && firePosDelayCounter <= 0)
        {
            if (target.GetComponent<SpriteRenderer>().flipX == true) { offset = new Vector3(-flip, savedOffset.y, 0); }
            else { offset = new Vector3(flip, savedOffset.y, 0); }
            smoothSpeed = savedSpeed;
        }

        //Charge and Fire Mechanics
        if (isCharging)
        {
            if (target.GetComponent<SpriteRenderer>().flipX == true) { offset = new Vector3(flip +0.25f, 0, 0); }
            else { offset = new Vector3(-flip -0.25f, 0, 0); }

            if (chargeTime < 3.0f)
                chargeTime += Time.deltaTime;
        }

        if (chargeTime > 0f && chargeTime < 1f) chargeLevel = 0;
        else if (chargeTime > 1f && chargeTime < 2f) chargeLevel = 1;
        else if (chargeTime > 2f && chargeTime < 3f) chargeLevel = 2;
        else if (chargeTime > 3f) chargeLevel = 3;

        if (firePosDelayCounter > 0)
            firePosDelayCounter -= Time.deltaTime;

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
                Debug.Log("Fired Lv 0 shot");   //small spammable projectile
                break;
            case 1:
                Debug.Log("Fired Lv 1 shot");   //faster projectile
                break;
            case 2:
                Debug.Log("Fired Lv 2 shot");   //
                break;
            case 3:
                Debug.Log("Fired Lv 3 shot");   //piercing laser
                break;
        }
        chargeTime = 0f;

        //offset = savedOffset;
    }
}
