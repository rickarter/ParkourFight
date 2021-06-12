using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAPI;

public class PlayerStunts : NetworkBehaviour
{
    //Components
    private PlayerScript player;
    private CapsuleCollider2D capsuleCollider;

    //Asignables
    public float backflipTorque = 2;

        //Tuck

        //Landing of feet
    private bool isRotatingToLand = false;
    public float landRotationTime = 0.1f;


    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerScript>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    public void Stunts(MyInput input)
    {
        Tuck(input);
        BackFlip(input);
        FrontFlip(input);
    }

    
    private float startTime = 0;
    private float stopAngle = 0;

    void Tuck(MyInput input)
    { 
        if(input.tuck != 0f && !player.playerMovement.grounded)
        {
            player.playerAnimation.TuckAnimation();
        }
        else
        {
            player.playerAnimation.StopTuckAnimation();
        }

        /*if(input.tuck >= 0.9f && !isRotatingToLand && !(input.backflip || input.frontflip))
        {
            isRotatingToLand = true;
            startTime = Time.time;

            stopAngle = player.rigidBody.angularVelocity > 0 ? 360 : 0;

            player.rigidBody.angularVelocity = 0;
            player.rigidBody.rotation = normalize(player.rigidBody.rotation);
        }

        if(isRotatingToLand)
        {
            float t = (Time.time - startTime) / landRotationTime;
            if(t > 1 || player.playerMovement.grounded) isRotatingToLand = false;
            player.rigidBody.rotation = Mathf.SmoothStep(player.rigidBody.rotation, stopAngle, t);
        }*/
    }
    
    void BackFlip(MyInput input)
    {
        if(input.backflip && !input.frontflip)
        {
            player.rigidBody.AddTorque(backflipTorque);
        }
    }

    void FrontFlip(MyInput input)
    {
        if(input.frontflip && !input.backflip)
        {
            player.rigidBody.AddTorque(-backflipTorque);
        }
    }

    private float normalize(float angle)
    {
        float newAngle = 0;

        if(angle >= 0)
            newAngle = angle - Mathf.Floor(angle/360) * 360;
        else
            newAngle = 360 + (Mathf.Floor(Mathf.Abs(angle) / 360) * 360 + angle);

        return newAngle;
    }
}
