using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAPI;

public class PlayerStunts : NetworkBehaviour
{
    //Components
    private NetworkPlayer player;

    //Asignables
    public float backflipTorque = 2;

    //Landing of feet
    private bool isRotatingToLand = false;
    public float landRotationTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<NetworkPlayer>();
    }

    public void Stunts(MyInput input)
    {
        Tuck(input);
        BackFlip(input);
        FrontFlip(input);
    }

    private float angularAcceleration = 0;
    void Tuck(MyInput input)
    { 
        if(input.tuck > 0.05f)
            player.playerAnimation.TuckAnimation();
        else
            player.playerAnimation.StopTuckAnimation();

        /*if(input.tuck >= 0.9f && !isRotatingToLand)
        {
            isRotatingToLand = true;
            float angle = 360 - transform.localEulerAngles.z;
            float velocity = player.rigidBody.angularVelocity;
            angularAcceleration = (angle-velocity*(landRotationTime+1))/(landRotationTime*(landRotationTime/2 + 1));
            Debug.Log(angularAcceleration);
        }

        if(isRotatingToLand)
        {
            player.rigidBody.angularVelocity += angularAcceleration * Time.fixedDeltaTime;

            if(transform.localEulerAngles.z > 350 || transform.localEulerAngles.z < 10) isRotatingToLand = false;
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
}
