using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsStunts : MonoBehaviour
{
    //Asignables
    public float backflipTorque = 2;
    public Rigidbody2D rigidBody;
    public PhysicsMovement physicsMovement;
        //Tuck

        //Landing of feet
    private bool isRotatingToLand = false;
    public float landRotationTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        physicsMovement = GetComponent<PhysicsMovement>();
    }

    public void Stunts(MyInput input)
    {
        Tuck(input);
        BackFlip(input);
    }
    
    private float startTime = 0;
    private float stopAngle = 0;

    void Tuck(MyInput input)
    { 
        /*if(input.tuck >= 0.9f && !isRotatingToLand && !(input.backflip || input.frontflip))
        {
            isRotatingToLand = true;
            startTime = Time.time;

            stopAngle = rigidBody.angularVelocity > 0 ? 360 : 0;

            rigidBody.angularVelocity = 0;
            rigidBody.rotation = normalize(rigidBody.rotation);
        }

        if(isRotatingToLand)
        {
            float t = (Time.time - startTime) / landRotationTime;
            if(t > 1 || physicsMovement.grounded) isRotatingToLand = false;
            rigidBody.rotation = Mathf.SmoothStep(rigidBody.rotation, stopAngle, t);
        }*/
    }
    
    void BackFlip(MyInput input)
    {
        if(input.backflip && !input.frontflip)
        {
            rigidBody.AddTorque(backflipTorque);
        }
    }

    void FrontFlip(MyInput input)
    {
        if(input.frontflip && !input.backflip)
        {
            rigidBody.AddTorque(-backflipTorque);
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
