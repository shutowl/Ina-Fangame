using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Main Variables")]
    public float speed = 50f;
    public float maxSpeed = 3f;
    public float jumpPower = 100f;
    private float tempSpeed;
    private float walkSpeed;
    public float walkDelay = 1.0f;
    private float walkDelayCounter;

    public float gravityScale = 1.5f;
    public float fallGravityMultiplier = 1.5f;

    [Header ("Coyote Time And Jump Buffers")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    public bool grounded;

    private Rigidbody2D rb;
    public SpriteRenderer hitbox;

    //Store Input Values
    private InputActions inputActions;
    private Vector2 moveVal;
    Keyboard keyboard = Keyboard.current;

    private void Awake()
    {
        inputActions = new InputActions();
        rb = GetComponent<Rigidbody2D>();
        if (rb is null)
            Debug.LogError("Rigidbody is NULL");
        //hitbox = GetComponentInChildren<SpriteRenderer>();

        tempSpeed = speed;
        walkSpeed = speed / 2;
        walkDelayCounter = walkDelay;
    }

    void Update()
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
        if(rb.velocity.y < 0)
        {
            rb.gravityScale = gravityScale * fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }

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
        if (inputActions.Player.Jump.WasPressedThisFrame())
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            if(jumpBufferCounter > 0f)
                jumpBufferCounter -= Time.deltaTime;
        }

        //Walk Delay
        if (inputActions.Player.Attack.WasReleasedThisFrame())
        {
            walkDelayCounter = walkDelay;
            speed = tempSpeed;
            hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 0);
        }
        else if(inputActions.Player.Attack.IsPressed())
        {
            walkDelayCounter -= Time.deltaTime;
        }

        //Walk
        if (walkDelayCounter <= 0f && inputActions.Player.Attack.IsPressed())
        {
            speed = walkSpeed;
            hitbox.color = new Color(hitbox.color.r, hitbox.color.g, hitbox.color.b, 1);
        }

    }

    void FixedUpdate()
    {
        Vector3 easeVelocity = rb.velocity;
        easeVelocity.y = rb.velocity.y;
        easeVelocity.z = 0.0f;
        easeVelocity.x *= 0.75f;

        float h = moveVal.x; // Direction (Left/Right)

        if (grounded)
            rb.velocity = easeVelocity;

        rb.AddForce((Vector2.right * speed) * h); //Increases speed
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y); //Limits the player's speed

    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

}
