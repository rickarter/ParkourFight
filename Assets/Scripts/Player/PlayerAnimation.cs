using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAPI;

public class PlayerAnimation : NetworkBehaviour
{
    //Components
    private NetworkPlayer player;

    public Animator animator;
    private Rigidbody2D rb;

    //Asignables
        //Inverse kinematics
    public Transform frontArmEffector;
    public Transform backArmEffector;
    public Transform frontLegEffector;
    public Transform backLegEffector;

        //Animations
    private string currentState = string.Empty;
    const string IDLE = "Idle";
    const string RUN = "Run";
    const string JUMP = "Jump";
    const string FLOATING = "Floating";

        //Grabing
    private bool grab = false;
    private bool tuck = false;
    private Vector3 grabPoint;

        //Jump tucking
    public Transform startArms;
    public Transform endArms;
    public Transform startLegs;
    public Transform endLegs;

    void Start()
    {
        player = GetComponent<NetworkPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
        OverrideAnimation();
    }

    void LateUpdate()
    {

    }

    void Animate()
    {
        Flip();

        string newState = currentState;
        bool isMoving = Mathf.Abs(player.rigidBody.velocity.x) > 0.05f && player.playerInput.input.x != 0;
        if(isMoving && player.playerMovement.grounded) newState = RUN;
        else if(!isMoving && player.playerMovement.grounded) newState = IDLE;
        else if(!player.playerMovement.grounded) newState = FLOATING;

        SetAnimationState(newState);
    }

    void OverrideAnimation()
    {
        if(grab)
        {
            frontArmEffector.position = grabPoint;
            backArmEffector.position = grabPoint;
        }
        if(tuck)
        {
            float t = player.playerInput.input.tuck;
            
            frontArmEffector.position = Vector3.Slerp(startArms.position, endArms.position, t);
            backArmEffector.position = Vector3.Slerp(startArms.position, endArms.position, t);

            frontLegEffector.position = Vector3.Slerp(startLegs.position, endLegs.position, t);
            backLegEffector.position = Vector3.Slerp(startLegs.position, endLegs.position, t);
        }
    }

    public void GrabAnimation(Vector3 point)
    {
        grab = true;
        grabPoint = point;

        animator.enabled = false;

        Invoke(nameof(StopGrabAnimation), 0.2f);
    }

    void StopGrabAnimation()
    {
        grab = false;
        animator.enabled = true;
    }

    public void TuckAnimation()
    {
        if(tuck) return;

        tuck = true;
        animator.enabled = false;
    }

    public void StopTuckAnimation()
    {
        if(!tuck) return;

        tuck =  false;
        animator.enabled = true;
    }

    void SetAnimationState(string newState)
    {
        if(newState == currentState) return;
        currentState = newState;

        animator.Play(newState);
    }

    void Flip()
    {
        if(player.playerInput.input.x == 0) return;

        Vector3 scale = transform.localScale;
        if(player.rigidBody.velocity.x >= 0) transform.localScale = new Vector3(1, scale.y, scale.z);
        else transform.localScale = new Vector3(-1, scale.y, scale.z);
    }

}