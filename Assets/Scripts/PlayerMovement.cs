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
        hitstun,
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

    [Header("Hitstun")]
    public float knockbackStrength = 5f;
    public float hitstun = 1f;              //default hitstun value
    private float hitstunCounter;
    public float damagediFrames = 1f;
    private float damagediFramesCounter;

    [Header ("Coyote Time And Jump Buffers")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Header ("Hitbox")]
    public float hitboxSize = 0.35f;
    public SpriteRenderer hitbox;
    public Transform hitboxTransform;

    [Header("Attacking")]
    private float attackTimer = 0f;          //If attack is pressed again while active, goes into the next attack in the combo. Also acts as attack duration for single attack combos

    //Animations
    private Animator anim;

    private float cutsceneDelay;
    public bool grounded;
    private Rigidbody2D rb;

    //Store Input Values
    private InputActions inputActions;
    private Vector2 moveVal;

    private Vector2 respawnPoint; //Respawn point

    private void Awake()
    {
        anim = GetComponent<Animator>();
        inputActions = new InputActions();
        rb = GetComponent<Rigidbody2D>();
        if (rb is null)
            Debug.LogError("Rigidbody is NULL");

        tempSpeed = speed;
        walkSpeed = speed / 2;
        walkDelayCounter = walkDelay;

        respawnPoint = transform.position;
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

            //Reset Walk back to Run
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

            //Attack
            if (inputActions.Player.Attack.WasPressedThisFrame())
            {
                if (grounded)
                {
                    //First attack doesn't stop movement
                    //Neutral Attack 1: Quick forward swing
                    if (anim.GetInteger("attackNum") == -1)
                    {
                        StartCoroutine(Attack(0, 0.5f, 1));
                    }
                    //Second attack and onwards stop movement
                    else if(anim.GetInteger("attackNum") == 1)
                    {
                        StartCoroutine(Attack(attackTimer, 0.5f, 2));
                    }
                }
                //aerial attacks
                else if(attackTimer <= 0)
                {
                    //Downwards Aerial: Quick swing downwards. Bounces up upon hit.
                    if (inputActions.Player.Move.ReadValue<Vector2>().y == -1)
                    {
                        StartCoroutine(Attack(0, 0.3f, 5));
                    }
                    //Neutral Aerial: Quick swing forward
                    else
                    {
                        StartCoroutine(Attack(0, 0.3f, 4));
                    }
                }
            }

            //Roll
            if (inputActions.Player.Roll.WasPressedThisFrame())
            {
                //rb.velocity = Vector2.zero;           //Setting velocity to zero makes it look a lil choppy
                currentState = playerState.rolling;
                rollCounter = rollDuration;
                rolliFramesCounter = rolliFrames;
            }
            //Slide
            if(grounded && inputActions.Player.Slide.WasPressedThisFrame())
            {
                //rb.velocity = Vector2.zero;
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
            if (GetComponent<SpriteRenderer>().flipX) direction = -1;
            else direction = 1;

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

            //Walk Delay
            if (inputActions.Player.Attack.WasReleasedThisFrame())
            {
                walkDelayCounter = walkDelay;
                speed = tempSpeed;
                hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0);
            }
        }
        //-----SLIDE STATE-----
        else if(currentState == playerState.sliding)
        {
            if (slideCounter >= slideDuration - 0.1)   //Fixes the same bug as the roll bug
                rb.velocity = Vector2.zero;

            slideCounter -= Time.deltaTime;
            hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0.5f); //DEBUG - remove later (turn on hitbox)
            hitboxTransform.position = new Vector2(transform.position.x, transform.position.y + hitboxOffset.y);  //lower hitbox
            //hitboxTransform.localScale = new Vector2(hitboxSize, hitboxSize + hitboxOffset.x);   //squish size

            float direction;
            if (GetComponent<SpriteRenderer>().flipX) direction = -1;
            else direction = 1;

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

            //Walk Delay
            if (inputActions.Player.Attack.WasReleasedThisFrame())
            {
                walkDelayCounter = walkDelay;
                speed = tempSpeed;
                hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0);
            }
        }
        //-----ATTACKING STATE-----
        else if(currentState == playerState.attacking)
        {
            //Neutral Attack 2: Quick swing infront of player

            //Neutral Attack 3: Throws spinning weapon forward and hits multiple times. Returns to player after a short delay

            if(attackTimer < 0)
            {
                currentState = playerState.moving;
            }
        }
        //-----DAMAGED STATE-----
        else if(currentState == playerState.hitstun)
        {
            float direction;
            if (GetComponent<SpriteRenderer>().flipX) direction = -1;
            else direction = 1;

            hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0.5f); //DEBUG - remove later

            //knockback
            rb.velocity = new Vector2(Mathf.Lerp(0, knockbackStrength, 1 - Mathf.Pow(1 - (hitstunCounter / hitstun), 3)) * -direction, rb.velocity.y);

            if (hitstunCounter <= 0)
            {
                resetHitbox();
                currentState = playerState.moving;
            }
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

            //DEBUG: reset position to recent respawn point
            transform.position = respawnPoint;
            GetComponent<PlayerHealth>().fullHeal();
            currentState = playerState.moving;
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

        //Hitstun
        if (hitstunCounter >= 0)
        {
            hitstunCounter -= Time.deltaTime;
        }
        if (damagediFramesCounter > 0)
        {
            damagediFramesCounter -= Time.deltaTime;
            hitbox.GetComponent<BoxCollider2D>().enabled = false;
        }

        //Attack
        if(attackTimer >= 0)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            anim.SetInteger("attackNum", -1);   //Not attacking
        }


        //------Animations-------
        /* STATES:
         * 0 = moving
         * 1 = attacking
         * 2 = rolling
         * 3 = sliding
         * 4 = parrying
         * 5 = hitstun
         * 6 = dead
         * 7 = cutscene
         */
        anim.SetInteger("curState", (int)currentState);
        anim.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("grounded", grounded);

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

            if (h > 0) GetComponent<SpriteRenderer>().flipX = false;
            if (h < 0) GetComponent<SpriteRenderer>().flipX = true;

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
        currentState = playerState.hitstun;
        hitstunCounter = hitstun;
        damagediFramesCounter = damagediFrames;
    }

    public void setDamageState(float hitstun)   //used for variable hitstun lengths
    {
        rb.velocity = Vector2.zero;
        currentState = playerState.hitstun;
        hitstunCounter = hitstun;
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
        hitboxTransform.localPosition = new Vector2(0, -0.125f);    //reset hitbox position
        hitboxTransform.localScale = new Vector2(hitboxSize, hitboxSize);   //reset size
        //hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0);    //turn off color
        hitbox.GetComponent<BoxCollider2D>().enabled = true;    //turn on hitbox
    }

    IEnumerator Attack(float lastDuration, float duration, int attackNum)
    {
        yield return new WaitForSeconds(lastDuration);  //wait until last attack is finished
        anim.SetInteger("attackNum", attackNum);
        if(attackNum == 2)
        {
            rb.velocity = Vector2.zero;
            currentState = playerState.attacking;
        }
        attackTimer = duration;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("RespawnPoint"))
        {
            respawnPoint = col.transform.position;
        }
    }
}
