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
    public float slideCounterMovement = 2f;
    private float threshold = 0.01f;

    //Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550;
    public float maxSlopeAngle = 35f; 
    private Vector2 normalVector = Vector3.up;

    //Grabbing
    private bool readyToGrab = true;
    private float grabCooldown = 1f;
    public float grabForce = 250f;
    public float grabRadius = 2f;

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

        Flip();
        
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
            Vector2 normal = other.contacts[i].normal;

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

    void Grab()
    {
        //Load points into a list
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, grabRadius, whatIsGround);
        List<Vector2> points = new List<Vector2>();

        bool hasPoints = false;
        foreach(Collider2D collider in colliders)
        {
            SpriteRenderer renderer = collider.gameObject.GetComponent<SpriteRenderer>();
            if(renderer)
            {
                Vector2[] corners = GetSpriteUpperCorners(renderer);
                points.Add(corners[0]);
                points.Add(corners[1]);
                hasPoints = true;
            }
        }

        //Find nearest point
        Vector2 grabPoint = GetClosestPoint(points.ToArray());

        //Apply forces
        Vector2 legPos = (Vector2)transform.position + transform.localScale.y * Vector2.down;
        Vector2 headPos = (Vector2)transform.position + transform.localScale.y * Vector2.up;
        Vector2 distance = grabPoint - legPos;

        float verticalForce = distance.y > 0 ? 8*distance.y/Mathf.Pow(0.1f, 2) : 0;
        float horizontalForce = 2*distance.x/Mathf.Pow(0.1f, 2);
        Vector2 vel = rb.velocity;
        Vector2 force = new Vector2(horizontalForce, verticalForce) * 1.5f;
        
        //Recall if the point isn't reachable
        RaycastHit2D bottomHit = Physics2D.Raycast(legPos, Vector2.down, grabRadius, whatIsGround);
        RaycastHit2D ceilingHit = Physics2D.Raycast(headPos, Vector2.up, grabRadius, whatIsGround);
        bool stopCondition = Vector2.Distance(transform.position, grabPoint) > grabRadius 
        || !hasPoints
        || bottomHit
        || ceilingHit
        || (Mathf.Sign(input.x) != Mathf.Sign(distance.x) && input.x != 0);

        if(stopCondition) return;
        rb.velocity = new Vector2(vel.x * 0.5f, 0);
        rb.AddForce(force);

        readyToGrab = false;
        readyToJump = false;
        Invoke(nameof(ResetGrab), grabCooldown);
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    void ResetGrab()
    {
        readyToGrab = true;
    }

    Vector2[] GetSpriteUpperCorners(SpriteRenderer renderer)
    {
        Vector2 topRight = renderer.transform.TransformPoint(renderer.sprite.bounds.max);
        Vector2 topLeft = renderer.transform.TransformPoint(new Vector2(renderer.sprite.bounds.min.x, renderer.sprite.bounds.max.y));
        return new Vector2[]{topRight, topLeft};
    }

    Vector2 GetClosestPoint(Vector2[] points)
    {
        Vector2 closest = new Vector2();
        float minDist = Mathf.Infinity;
        Vector2 currentPos = transform.position;

        foreach(Vector2 point in points)
        {
            float dist = Vector2.Distance(currentPos, point);
            if(dist < minDist)
            {
                closest = point;
                minDist = dist;
            }
        }

        return closest;
    }

    void OnDrawGizmost()
    {
        //Grabbing
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, grabRadius);
        Gizmos.DrawSphere(transform.localScale.z*Vector2.down+(Vector2)transform.position, 0.1f);

    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        if(rb.velocity.x >= 0) transform.localScale = new Vector3(1, scale.y, scale.z);
        else transform.localScale = new Vector3(-1, scale.y, scale.z);
    }
}
