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

    [Header("Main Variables")]
    private int direction = 1;
    public float speed = 50f;               //How fast the player accelerates
    public float maxSpeed = 3f;             //Max player speed
    public float jumpPower = 100f;          //How high the player jumps
    private bool bufferJump = false;
    private float runSpeed;
    private float walkSpeed;
    public float walkDelay = 1.0f;          //How long a button is held before walk is enabled
    private float walkDelayCounter;
    public float size = 1.7f;

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
    public float knockbackLimit = 0.5f;     //Max duration of knockback
    public float knockbackStrength = 5f;
    private float hitstun = 0.5f;              //hitstun value
    private float hitstunCounter;
    public float damagediFrames = 1f;
    private float damagediFramesCounter;
    public float hitstunFlashRate = 0.2f;
    float hitstunFlashTimer = 0f;
    bool flashOn = false;
    int timesHit = 0;

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
    public Transform hurtbox;
    public float animOffset;
    private int attackNum = -1;
    private float attackTimer = 0f;          //If attack is pressed again while active, goes into the next attack in the combo. Also acts as attack duration for single attack combos
    private float flipOffset = 0.1f;         //Player continues looking towards attack for [flipOffset] seconds longer.
    private float bounceTimer = 0f;
    AOMovement AO;

    [Header("Other")]
    public ParticleSystem dust;
    bool paused = false;

    //Animations
    private Animator anim;

    private float cutsceneDelay;
    public bool grounded;
    private Rigidbody2D rb;
    AnimationClip[] clips;

    //Store Input Values
    private InputActions inputActions;
    private Vector2 moveVal;

    private Vector2 respawnPoint; //Respawn point

    private void Awake()
    {
        AO = FindObjectOfType<AOMovement>();
        anim = GetComponent<Animator>();
        clips = anim.runtimeAnimatorController.animationClips;
        inputActions = new InputActions();
        inputActions.Player.Enable();
        rb = GetComponent<Rigidbody2D>();
        if (rb is null)
            Debug.LogError("Rigidbody is NULL");

        runSpeed = speed;
        walkSpeed = speed / 2;
        walkDelayCounter = walkDelay;

        respawnPoint = transform.position;
        paused = false;
        timesHit = 0;
    }

    void Update()
    {
        float lastDir = moveVal.x;  //detects changes in direction using (lastDir != moveVal.x)

        //-----MOVE STATE-----
        if (currentState == playerState.moving && !paused)
        {
            moveVal = inputActions.Player.Move.ReadValue<Vector2>();

            //Jump
            if (coyoteTimeCounter >= 0f && jumpBufferCounter > 0f)
            {
                CreateDust();
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(Vector2.up * jumpPower);

                if (bufferJump)
                {
                    rb.AddForce(Vector2.down * jumpPower/3);
                    bufferJump = false;
                }

                jumpBufferCounter = 0f;

                AudioManager.Instance.Play("JumpPlayer");
            }
            //Jump Buffer
            if (inputActions.Player.Jump.WasPressedThisFrame())
            {
                jumpBufferCounter = jumpBufferTime;

                if (!grounded) bufferJump = false;
            }
            //Control Jump Height
            if (inputActions.Player.Jump.WasReleasedThisFrame())
            {
                if(jumpBufferCounter > 0)
                    bufferJump = true;

                if(rb.velocity.y >= 0.1)
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2); //Slows down y-axis momentum
                    coyoteTimeCounter = 0f;
                }
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


            //Reset Walk back to Run
            if (inputActions.Player.Attack.WasReleasedThisFrame())
            {
                walkDelayCounter = walkDelay;
                speed = runSpeed;
                ResetHitbox();
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
                //Grounded Attacks
                if (grounded)
                {
                    //First attack doesn't stop movement
                    //Neutral Attack 1: Quick forward swing
                    if (attackNum == -1)
                    {
                        attackNum = 1;
                        StartCoroutine(Attack(0, GetAnimationClipLength("ina_neutral_1") - animOffset, attackNum));
                    }
                    else if (attackNum == 1 || attackNum == 2)
                    {
                        attackTimer += 2 * Time.deltaTime;  //+2 frames to hopefully remove attack bug
                        currentState = playerState.attacking;
                    }
                }
                //Aerial Attacks
                else if (attackTimer <= 0 - flipOffset)
                {
                    //Downwards Aerial: Quick swing downwards. Bounces up upon hit.
                    if (inputActions.Player.Move.ReadValue<Vector2>().y == -1)
                    {
                        StartCoroutine(Attack(0, GetAnimationClipLength("ina_dair") - animOffset, 5));
                    }
                    //Neutral Aerial: Quick swing forward
                    else
                    {
                        StartCoroutine(Attack(0, GetAnimationClipLength("ina_nair") - animOffset, 4));
                    }
                }

            }

            //Roll
            if (inputActions.Player.Roll.WasPressedThisFrame())
            {
                if(grounded) CreateDust();
                StopAllCoroutines();
                attackTimer = -10;

                //determine roll direction, if no input (x = 0), roll forwards
                if (inputActions.Player.Move.ReadValue<Vector2>().x > 0) direction = 1;
                else if (inputActions.Player.Move.ReadValue<Vector2>().x < 0) direction = -1;

                //rb.velocity = Vector2.zero;           //Setting velocity to zero makes it look a lil choppy
                currentState = playerState.rolling;
                rollCounter = rollDuration;
                rolliFramesCounter = rolliFrames;
            }
            //Slide
            if(grounded && inputActions.Player.Slide.WasPressedThisFrame() && attackTimer <= 0)
            {
                //rb.velocity = Vector2.zero;
                currentState = playerState.sliding;
                slideCounter = slideDuration;
            }
            //Damaged IFrames after knockback
            if(damagediFramesCounter <= 0 && walkDelayCounter > 0)
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
                AO.Charge();
            }
            if (inputActions.Player.Fire.WasReleasedThisFrame())
            {
                AO.Fire();
            }
        }
        //-----ROLL STATE-----
        else if(currentState == playerState.rolling)
        {
            if (rollCounter >= rollDuration - 0.05) //Fixes a bug where jumping and rolling on the same frame makes player roll upwards
                rb.velocity = Vector2.zero;

            rollCounter -= Time.deltaTime;
            rolliFramesCounter -= Time.deltaTime;

            rb.velocity = new Vector2(Mathf.Lerp(0, rollSpeed, 1 - Mathf.Pow(1 - (rollCounter / rollDuration), 3)) * direction, rb.velocity.y);

            if (rollCounter <= 0)
            {
                currentState = playerState.moving;
            }
            if(rolliFramesCounter > 0)
            {
                //hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0.5f); //DEBUG - remove later
                hitbox.GetComponent<BoxCollider2D>().enabled = false;
            }
            else
            {
                if (damagediFramesCounter <= 0)
                {
                    if (grounded) CreateDust();
                    //hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0);    //DEBUG
                    hitbox.GetComponent<BoxCollider2D>().enabled = true;
                }
            }

            //Enables AO abilities during roll
            if (inputActions.Player.Fire.WasPressedThisFrame())
            {
                AO.Charge();
            }
            if (inputActions.Player.Fire.WasReleasedThisFrame())
            {
                AO.Fire();
            }

            //Walk Delay
            if (inputActions.Player.Attack.WasReleasedThisFrame())
            {
                walkDelayCounter = walkDelay;
                speed = runSpeed;
                hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0);
            }
        }
        //-----SLIDE STATE-----
        else if(currentState == playerState.sliding)
        {
            CreateDust();
            if (slideCounter >= slideDuration - 0.1)   //Fixes the same bug as the roll bug
                rb.velocity = Vector2.zero;

            slideCounter -= Time.deltaTime;
            //hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0.5f); //DEBUG - remove later (turn on hitbox)
            hitboxTransform.position = new Vector2(transform.position.x, transform.position.y + hitboxOffset.y);  //lower hitbox
            //hitboxTransform.localScale = new Vector2(hitboxSize, hitboxSize + hitboxOffset.x);   //squish size

            rb.velocity = new Vector2(slideSpeed * direction, rb.velocity.y);   //constant movement

            if (slideCounter <= 0)
            {
                currentState = playerState.moving;
                ResetHitbox();
            }

            //Enables AO abilities during slide
            if (inputActions.Player.Fire.WasPressedThisFrame())
            {
                AO.Charge();
            }
            if (inputActions.Player.Fire.WasReleasedThisFrame())
            {
                AO.Fire();
            }

            //Walk Delay
            if (inputActions.Player.Attack.WasReleasedThisFrame())
            {
                walkDelayCounter = walkDelay;
                speed = runSpeed;
                hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0);
            }
        }
        //-----ATTACKING STATE----- (only applies to Neutral 2 and 3)
        else if(currentState == playerState.attacking)
        {
            if (attackNum == 2 || attackNum == 3)
            {
                rb.velocity = Vector2.zero;
                moveVal.x = 0;
            }
            else
                moveVal = inputActions.Player.Move.ReadValue<Vector2>();

            //Neutral Attack 2: Quick swing forwards
            if (attackNum == 1)
            {
                StartCoroutine(Attack(attackTimer, GetAnimationClipLength("ina_neutral_2") - animOffset, 2));
            }
            //Neutral Attack 3: Throws crowbar out and hits multiple times
            if(attackNum == 2 && inputActions.Player.Attack.WasPressedThisFrame())
            {
                StartCoroutine(Attack(attackTimer, GetAnimationClipLength("ina_neutral_3") - animOffset, 3));
            }

            if (attackNum == -1) currentState = playerState.moving;


            //Enables AO abilities during attacks
            if (inputActions.Player.Fire.WasPressedThisFrame())
            {
                AO.Charge();
            }
            if (inputActions.Player.Fire.WasReleasedThisFrame())
            {
                AO.Fire();
            }
            //Cancels attacks into a roll
            if (inputActions.Player.Roll.WasPressedThisFrame())
            {
                StopAllCoroutines();
                attackTimer = -10;

                //determine roll direction, if no input (x = 0), roll forwards
                if (inputActions.Player.Move.ReadValue<Vector2>().x > 0) direction = 1;
                else if (inputActions.Player.Move.ReadValue<Vector2>().x < 0) direction = -1;

                currentState = playerState.rolling;
                rollCounter = rollDuration;
                rolliFramesCounter = rolliFrames;
            }

            //Prevents ground attacks in midair
            if (!grounded)
            {
                StopAllCoroutines();
                attackNum = -1;
                currentState = playerState.moving;
                attackTimer = -10;

                //Aerial Attacks
                if (attackTimer <= 0 - flipOffset)
                {
                    //Downwards Aerial: Quick swing downwards. Bounces up upon hit.
                    if (inputActions.Player.Move.ReadValue<Vector2>().y == -1)
                    {
                        StartCoroutine(Attack(0, GetAnimationClipLength("ina_dair") - animOffset, 5));
                        AudioManager.Instance.Play("Swing");

                    }
                    //Neutral Aerial: Quick swing forward
                    else
                    {
                        StartCoroutine(Attack(0, GetAnimationClipLength("ina_nair") - animOffset, 4));
                        AudioManager.Instance.Play("Swing");
                    }
                }
            }
        }
        //-----HITSTUN STATE-----
        else if(currentState == playerState.hitstun)
        {
            //hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0.5f); //DEBUG - remove later

            //knockback            
            if(hitstunCounter < (hitstun - knockbackLimit) && grounded) //prevents player from constantly sliding backwards
            {
                rb.velocity = Vector2.zero;
            }
            else
            {
                rb.velocity = new Vector2(Mathf.Lerp(0, knockbackStrength, 1 - Mathf.Pow(1 - (hitstunCounter / hitstun), 3)) * -direction, rb.velocity.y);
            }

            if (hitstunCounter <= 0)
            {
                walkDelayCounter = walkDelay;
                speed = runSpeed;
                ResetHitbox();
                currentState = playerState.moving;
            }

            //Enables AO abilities during hitstun
            if (inputActions.Player.Fire.WasPressedThisFrame())
            {
                AO.Charge();
            }
            if (inputActions.Player.Fire.WasReleasedThisFrame())
            {
                AO.Fire();
            }
        }

        //-----CUTSCENE STATE-----
        /*else if(currentState == playerState.inCutscene)
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
        }*/

        //------DEAD STATE-----
        else if(currentState == playerState.dead)
        {
            rb.velocity = Vector2.zero;
            //Play death animation

            //DEBUG: reset position to recent respawn point
            transform.position = respawnPoint;
            GetComponent<PlayerHealth>().FullHeal();
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
        else
            bufferJump = false;

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

            //flash
            hitstunFlashTimer -= Time.deltaTime;
            if (hitstunFlashTimer <= 0)
            {
                if (flashOn)
                {
                    GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
                }
                else
                {
                    GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                }
                flashOn = !flashOn;
                hitstunFlashTimer = hitstunFlashRate;
            }
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        }

        //Attack
        if(attackTimer > 0 - flipOffset)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            attackNum = -1;
        }

        //Bounce
        if(bounceTimer >= 0)
        {
            bounceTimer -= Time.deltaTime;
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
        anim.SetInteger("attackNum", attackNum);


        //-----Others-----
        if (attackTimer <= 0 - flipOffset)    //prevent flipping on attacks (+ some leeway)
            transform.localScale = new Vector3(size * direction, size, size);   //flips sprite of this object and its children (like hurtbox)

        //Particle system (Creates dust upon changing directions on ground)
        if(lastDir != moveVal.x && moveVal.x != 0 && grounded)
        {
            CreateDust();
        }

    }

    void FixedUpdate()
    {
        if (currentState == playerState.moving || (currentState == playerState.attacking && (attackNum != 2 || attackNum != 3)))
        {
            Vector3 easeVelocity = rb.velocity;
            easeVelocity.y = rb.velocity.y;
            easeVelocity.z = 0.0f;
            easeVelocity.x *= 0.75f;

            float h = moveVal.x; // Direction (Left/Right)

            if (grounded)
                rb.velocity = easeVelocity;

            if (h > 0) direction = 1;   //1 = right
            if (h < 0) direction = -1;  //-1 = left

            rb.AddForce((Vector2.right * speed) * h); //Increases speed
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y); //Limits the player's speed
        }
    }

    //Gets the Animation Clip length of a given name (invalid names return error)
    float GetAnimationClipLength(string name)
    {
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == name)
            {
                return clip.length;
            }
        }

        Debug.LogError("Animation clip name not found!: " + name);
        return 0f;
    }

    //Currently should give player invincibility for a certain duration
    //And should stop player movement/momentum for a short time.
    public void StartDefenseSkill()
    {
        Debug.Log("Defense Skill Activated");
    }

    public void setDamageState()
    {
        rb.velocity = Vector2.zero;
        currentState = playerState.hitstun;
        hitstunCounter = hitstun;
        damagediFramesCounter = damagediFrames + hitstun;
        hitstunFlashTimer = hitstunFlashRate;
        flashOn = false;

        timesHit++;

        AudioManager.Instance.Play("Damaged");
    }

    public void setDamageState(float hitstun)   //used for variable hitstun lengths
    {
        StopAllCoroutines();

        rb.velocity = Vector2.zero;
        currentState = playerState.hitstun;
        hitstunCounter = hitstun;
        this.hitstun = hitstun;
        damagediFramesCounter = damagediFrames + hitstun;
        hitstunFlashTimer = hitstunFlashRate;
        flashOn = false;

        timesHit++;

        AudioManager.Instance.Play("Damaged");
    }

    public void setCutsceneState(float delay)
    {
        currentState = playerState.inCutscene;
        cutsceneDelay = delay;            
    }

    public IEnumerator PausePlayer(bool paused)
    {
        yield return new WaitForEndOfFrame();
        if (paused)
        {
            hitbox.GetComponent<BoxCollider2D>().enabled = false;
            AO.Reset();
            inputActions.Player.Disable();
            rb.velocity = Vector2.zero;
            moveVal.x = 0;
        }
        else
        {
            hitbox.GetComponent<BoxCollider2D>().enabled = true;
            inputActions.Player.Enable();
        }
        this.paused = paused;
    }

    public Vector2 GetMoveVal()
    {
        return moveVal;
    }

    public InputActions GetInputActions()
    {
        return inputActions;
    }

    private void ResetHitbox(){
        hitboxTransform.localPosition = new Vector2(0, -0.125f);    //reset hitbox position
        hitboxTransform.localScale = new Vector2(hitboxSize, hitboxSize);   //reset size
        hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0);    //turn off color
        hitbox.GetComponent<BoxCollider2D>().enabled = true;    //turn on hitbox
    }

    IEnumerator Attack(float lastDuration, float duration, int attackNum)
    {
        attackTimer = lastDuration;
        yield return new WaitForSeconds(Mathf.Clamp(lastDuration, 0, lastDuration));  //wait until last attack is finished

        attackTimer = duration;
        this.attackNum = attackNum;

        AudioManager.Instance.Play("Swing");
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("RespawnPoint"))
        {
            respawnPoint = col.transform.position;
        }
    }

    public int getDirection()
    {
        return direction;
    }

    public int getAttackNum()
    {
        return attackNum;
    }

    public int GetTimesHit()
    {
        return timesHit;
    }

    public bool IsPaused()
    {
        return paused;
    }

    public void Bounce()    //used for when DAir hits something
    {
        //Prevents "super bouncing" due to hitting multiple DAirs in 1 frame
        if(bounceTimer <= 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, jumpPower * 2 / 3));
        }
        bounceTimer = 0.3f;
    }

    public void ResetAttackTimer()
    {
        attackTimer = -10;
    }

    public void CreateDust()
    {
        dust.Play();
    }
}
