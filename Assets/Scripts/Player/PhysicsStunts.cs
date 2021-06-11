using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsStunts : MonoBehaviour
{
    //Asignables
    public float backflipTorque = 2;
    public Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public void Stunts(MyInput input)
    {
        Tuck(input);
        BackFlip(input);
    }

    void Tuck(MyInput input)
    {
    }
    
    void BackFlip(MyInput input)
    {
        if(input.backflip)
        {
            rigidBody.constraints = RigidbodyConstraints2D.None;
            rigidBody.AddTorque(backflipTorque);
        }
        else
        {
            rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
}
