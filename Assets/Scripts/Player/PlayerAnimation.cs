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
    public Transform overridenFrontArmEffector;
    public Transform frontArmEffector;

    public Transform overridenBackArmEffector;
    public Transform backArmEffector;

    public Transform overidenFrontLegEffector;
    public Transform frontLegEffector;

    public Transform overridenBackLegEffector;
    public Transform BackLegEffector;

    //Animations
    private string currentState = string.Empty;
    const string IDLE = "Idle";
    const string RUN = "Run";
    const string JUMP = "Jump";
    const string FLOATING = "Floating";

    //Grabing
    private bool grab = false;
    private Vector3 grabPoint;

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
    }

    public void GrabAnimation(Vector3 point)
    {
        grab = true;
        grabPoint = point;
        overridenFrontArmEffector.parent.gameObject.SetActive(false);
        overridenBackArmEffector.parent.gameObject.SetActive(false);

        frontArmEffector.parent.gameObject.SetActive(true);
        backArmEffector.parent.gameObject.SetActive(true);
        Invoke(nameof(StopGrabAnimation), 0.2f);
    }

    void StopGrabAnimation()
    {
        grab = false;
            overridenFrontArmEffector.parent.gameObject.SetActive(true);
        overridenBackArmEffector.parent.gameObject.SetActive(true);

        frontArmEffector.parent.gameObject.SetActive(false);
        backArmEffector.parent.gameObject.SetActive(false);
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