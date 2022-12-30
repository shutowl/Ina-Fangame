using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public enum playerState
    {
        moving,
        attacking,
        rolling,
        sliding,
        parrying,
        damaged,
        dead,
        inCutscene
    }
    public playerState currentState;
    //Animation has variable PlayerAnimState based on int 

    [Header ("Main Variables")]
    public float speed = 50f;               //How fast the player accelerates
    public float maxSpeed = 3f;             //Max player speed
    public float jumpPower = 100f;          //How high the player jumps
    private float tempSpeed;
    private float walkSpeed;
    public float walkDelay = 1.0f;          //How long a button is held before walk is enabled
    private float walkDelayCounter;

    public float gravityScale = 1.5f;               //Player Gravity
    public float fallGravityMultiplier = 1.5f;      //Player fall gravity

    [Header("Rolling")]
    public float rollSpeed = 5f;            //How fast the roll goes
    public float rollDuration = 1f;         //How long the roll lasts
    private float rollCounter;
    public float rolliFrames = 0.3f;        //How many seconds the player is invincible during a roll
    private float rolliFramesCounter;

    [Header("Sliding")]
    public float slideSpeed = 5f;
    public float slideDuration = 1f;
    public Vector2 hitboxOffset;            //x = box collider y size offset, y = y position offset
    private float slideCounter;

    [Header("Damaged")]
    public float knockbackStrength = 5f;
    public float damagedDuration = 1f;
    private float damagedDurationCounter;
    public float damagediFrames = 1f;
    private float damagediFramesCounter;

    [Header ("Coyote Time And Jump Buffers")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private float cutsceneDelay;

    public bool grounded;

    private Rigidbody2D rb;
    public float hitboxSize = 0.35f;
    public SpriteRenderer hitbox;
    public Transform hitboxTransform;

    //Store Input Values
    private InputActions inputActions;
    private Vector2 moveVal;

    private void Awake()
    {
        inputActions = new InputActions();
        rb = GetComponent<Rigidbody2D>();
        if (rb is null)
            Debug.LogError("Rigidbody is NULL");

        tempSpeed = speed;
        walkSpeed = speed / 2;
        walkDelayCounter = walkDelay;
    }

    void Update()
    {
        //-----MOVE STATE-----
        if (currentState == playerState.moving)
        {
            moveVal = inputActions.Player.Move.ReadValue<Vector2>();

            //Jump
            if (coyoteTimeCounter >= 0f && jumpBufferCounter > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(Vector2.up * jumpPower);

                jumpBufferCounter = 0f;
            }
            //Control Jump Height
            if (inputActions.Player.Jump.WasReleasedThisFrame() && rb.velocity.y >= 0.1)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2); //Slows down y-axis momentum

                coyoteTimeCounter = 0f;
            }

            //Jump Gravity
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravityScale * fallGravityMultiplier;
            }
            else
            {
                rb.gravityScale = gravityScale;
            }

            //Jump Buffer
            if (inputActions.Player.Jump.WasPressedThisFrame())
            {
                jumpBufferCounter = jumpBufferTime;
            }

            //Walk Delay
            if (inputActions.Player.Attack.WasReleasedThisFrame())
            {
                walkDelayCounter = walkDelay;
                speed = tempSpeed;
                hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0);
            }

            //Walk
            if (walkDelayCounter <= 0f && inputActions.Player.Attack.IsPressed())
            {
                speed = walkSpeed;
                hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 1);
            }

            //Roll
            if (inputActions.Player.Roll.WasPressedThisFrame())
            {
                rb.velocity = Vector2.zero;
                currentState = playerState.rolling;
                rollCounter = rollDuration;
                rolliFramesCounter = rolliFrames;
            }
            //Slide
            if(grounded && inputActions.Player.Slide.WasPressedThisFrame())
            {
                rb.velocity = Vector2.zero;
                currentState = playerState.sliding;
                slideCounter = slideDuration;
            }
            //Damaged IFrames after knockback
            else if(damagediFramesCounter <= 0 && walkDelayCounter > 0)
            {
                hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0);    //DEBUG
                hitbox.GetComponent<BoxCollider2D>().enabled = true;
            }

            //Interaction
            if (inputActions.Player.Interact.WasPressedThisFrame())
            {
                //Debug.Log("Interact button pressed");
                //Code done in DialogueTrigger.cs
            }

            //Fire
            if (inputActions.Player.Fire.WasPressedThisFrame())
            {
                FindObjectOfType<AOMovement>().Charge();
            }
            if (inputActions.Player.Fire.WasReleasedThisFrame())
            {
                FindObjectOfType<AOMovement>().Fire();
            }

            resetHitbox();
        }
        //-----ROLL STATE-----
        else if(currentState == playerState.rolling)
        {
            if (rollCounter >= rollDuration - 0.05) //Fixes a bug where jumping and rolling on the same frame makes player roll upwards
                rb.velocity = Vector2.zero;

            rollCounter -= Time.deltaTime;
            rolliFramesCounter -= Time.deltaTime;

            float direction;
            if (GetComponent<SpriteRenderer>().flipX) direction = 1;
            else direction = -1;

            rb.velocity = new Vector2(Mathf.Lerp(0, rollSpeed, 1 - Mathf.Pow(1 - (rollCounter / rollDuration), 3)) * direction, rb.velocity.y);

            if (rollCounter <= 0)
            {
                currentState = playerState.moving;
            }
            if(rolliFramesCounter > 0)
            {
                hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0.5f); //DEBUG - remove later
                hitbox.GetComponent<BoxCollider2D>().enabled = false;
            }
            else
            {
                if (damagediFramesCounter <= 0)
                {
                    hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0);    //DEBUG
                    hitbox.GetComponent<BoxCollider2D>().enabled = true;
                }
            }

            //Enables AO abilities during roll
            if (inputActions.Player.Fire.WasPressedThisFrame())
            {
                FindObjectOfType<AOMovement>().Charge();
            }
            if (inputActions.Player.Fire.WasReleasedThisFrame())
            {
                FindObjectOfType<AOMovement>().Fire();
            }
        }
        //-----SLIDE STATE-----
        else if(currentState == playerState.sliding)
        {
            slideCounter -= Time.deltaTime;
            hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0.5f); //DEBUG - remove later (turn on hitbox)
            hitboxTransform.position = new Vector2(transform.position.x, transform.position.y + hitboxOffset.y);  //lower hitbox
            hitboxTransform.localScale = new Vector2(hitboxSize, hitboxSize + hitboxOffset.x);   //squish size

            float direction;
            if (GetComponent<SpriteRenderer>().flipX) direction = 1;
            else direction = -1;

            rb.velocity = new Vector2(slideSpeed * direction, rb.velocity.y);   //constant movement

            if (slideCounter <= 0)
            {
                currentState = playerState.moving;
                resetHitbox();

            }

            //Enables AO abilities during slide
            if (inputActions.Player.Fire.WasPressedThisFrame())
            {
                FindObjectOfType<AOMovement>().Charge();
            }
            if (inputActions.Player.Fire.WasReleasedThisFrame())
            {
                FindObjectOfType<AOMovement>().Fire();
            }
        }
        //-----DAMAGED STATE-----
        else if(currentState == playerState.damaged)
        {
            float direction;
            if (GetComponent<SpriteRenderer>().flipX) direction = 1;
            else direction = -1;

            hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0.5f); //DEBUG - remove later

            //knockback
            rb.velocity = new Vector2(Mathf.Lerp(0, knockbackStrength, 1 - Mathf.Pow(1 - (damagedDurationCounter / damagedDuration), 3)) * -direction, rb.velocity.y);

            if (damagedDurationCounter <= 0)
            {
                resetHitbox();
                currentState = playerState.moving;
            }
        }
        if(damagedDurationCounter >= 0)
        {
            damagedDurationCounter -= Time.deltaTime;
        }
        if (damagediFramesCounter > 0)
        {
            damagediFramesCounter -= Time.deltaTime;
            hitbox.GetComponent<BoxCollider2D>().enabled = false;
        }

        //-----CUTSCENE STATE-----
        else if(currentState == playerState.inCutscene)
        {
            hitbox.GetComponent<BoxCollider2D>().enabled = false;
            rb.velocity = new Vector2(0, rb.velocity.y);

            if (cutsceneDelay < 0)
            {
                if (inputActions.Player.Confirm.WasPressedThisFrame())
                {
                    FindObjectOfType<DialogueManager>().DisplayNextSentence();
                }
            }
            if(cutsceneDelay >= 0)
            {
                cutsceneDelay -= Time.deltaTime;
            }
        }

        //------DEAD STATE-----
        else if(currentState == playerState.dead)
        {
            rb.velocity = Vector2.zero;
            //Play death animation
        }

        //-----TIMERS-----

        //Coyote Time
        if (grounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        //Jump Buffer
        if (jumpBufferCounter > 0f)
            jumpBufferCounter -= Time.deltaTime;

        //Walk
        if (inputActions.Player.Attack.IsPressed())
        {
            walkDelayCounter -= Time.deltaTime;
        }

    }

    void FixedUpdate()
    {
        if (currentState == playerState.moving)
        {
            Vector3 easeVelocity = rb.velocity;
            easeVelocity.y = rb.velocity.y;
            easeVelocity.z = 0.0f;
            easeVelocity.x *= 0.75f;

            float h = moveVal.x; // Direction (Left/Right)

            if (grounded)
                rb.velocity = easeVelocity;

            if (h > 0) GetComponent<SpriteRenderer>().flipX = true;
            if (h < 0) GetComponent<SpriteRenderer>().flipX = false;

            rb.AddForce((Vector2.right * speed) * h); //Increases speed
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y); //Limits the player's speed
        }
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    public void setDamageState()
    {
        rb.velocity = Vector2.zero;
        currentState = playerState.damaged;
        damagedDurationCounter = damagedDuration;
        damagediFramesCounter = damagediFrames;
    }

    public void setCutsceneState(float delay)
    {
        currentState = playerState.inCutscene;
        cutsceneDelay = delay;            
    }

    public Vector2 getMoveVal()
    {
        return moveVal;
    }

    public InputActions getInputActions()
    {
        return inputActions;
    }

    private void resetHitbox(){
        hitboxTransform.position = transform.position;    //reset hitbox position
        hitboxTransform.localScale = new Vector2(hitboxSize, hitboxSize);   //reset size
        //hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0);    //turn off color
        hitbox.GetComponent<BoxCollider2D>().enabled = true;    //turn on hitbox
    }

}
