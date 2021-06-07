using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ClientState
{
    public MyInput input;
    public Vector2 position;
    public float rotation;
    public Vector2 velocity;
    public float angularVelocity;
    public int tickNumber;

    public ClientState(MyInput input, Rigidbody2D rigidbody2D, int tickNumber)
    {
        this.input = input;
        position = rigidbody2D.position;
        rotation = rigidbody2D.rotation;
        velocity = rigidbody2D.velocity;
        angularVelocity = rigidbody2D.angularVelocity;
        this.tickNumber = tickNumber;
    }
}
