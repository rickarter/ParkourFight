using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class PlayerMovement : NetworkBehaviour
{
    //Components
    private Rigidbody2D rb;

    //Movement
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public bool grounded;
    public LayerMask whatIsGround;

    public float counterMovement = 0.175f;
    private float threshold = 0.01f;

    //Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550;
    public float maxSlopeAngle = 35f; 
    private Vector3 normalVector = Vector3.up;

    //Input
    MyInput input = new MyInput();
    bool jumping;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(!IsLocalPlayer) return;
        input.InputClientRpc();

        ToggleJumping();
    }

    private bool hasChanged;
    void ToggleJumping()
    {
        if(input.jumping)
        {
            if(!hasChanged) 
            {
                jumping = true;
                hasChanged = true;

                float delay = 0.125f;
                Invoke(nameof(ResetJumping), delay);
            }
        }
        else
        {
            hasChanged = false;
        }
    }

    void ResetJumping()
    {
        jumping = false;
    }

    void FixedUpdate()
    {   
        Movement(input);
    }

    void Movement(MyInput input)
    {
        //Read input
        float x = input.x;
        float y = input.y;

        //Extra gravity
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        Vector2 mag = rb.velocity;
        CounterMovement(x, y, mag);

        if (grounded && jumping) Jump();
        else if(!grounded && jumping) Grab();

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && mag.x > maxSpeed) x = 0;
        if (x < 0 && mag.x < -maxSpeed) x = 0;

        //Some multipliers
        float multiplier = 1f;
        
        // Movement in air
        if (!grounded) {
            multiplier = 0.3f;
        }

        rb.AddForce(transform.right * x * moveSpeed * multiplier * Time.deltaTime);
        
    }

    void CounterMovement(float x, float y, Vector2 mag)
    {
        if(!grounded || jumping) return;

        //CounterMovement
        if (Mathf.Abs(mag.x) > threshold && Mathf.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0)) 
        {
            rb.AddForce(moveSpeed * transform.right * Time.deltaTime * -mag.x * counterMovement);
        }

        //Limit speed
        rb.velocity = new Vector2(Mathf.Clamp(mag.x, -maxSpeed, maxSpeed), mag.y);
    }

    void Jump()
    {   
        if(readyToJump)
        {
            readyToJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);

            //If jump while falling, reset y velocity
            Vector2 vel = rb.velocity;
            if(vel.y < 0.5f)
                rb.velocity = new Vector2(vel.x, 0);
            else if(vel.y > 0)
                rb.velocity = new Vector2(vel.x, vel.y/2);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void Grab()
    {

    }

    void ResetJump()
    {
        readyToJump = true;
    }

    bool IsFloor(Vector3 normal)
    {
        float angle = Vector3.Angle(Vector3.up, normal);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;
    void OnCollisionStay2D(Collision2D other)
    {
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        for (int i = 0; i < other.contactCount; i++) 
        {
            Vector3 normal = other.contacts[i].normal;

            if(IsFloor(normal))
            {
                normalVector = normal;
                grounded = true;
                cancellingGrounded = false;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    void StopGrounded()
    {
        grounded = false;
    }
}
