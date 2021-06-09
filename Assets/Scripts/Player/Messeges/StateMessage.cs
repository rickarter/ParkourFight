using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAPI.Serialization;

public struct StateMessage : INetworkSerializable
{
    public Vector2 position;
    public float rotation;
    public Vector2 velocity;
    public float angularVelocity;
    public int tickNumber;
    public MyInput input;

    public StateMessage(Rigidbody2D rigidbody2D, int tickNumber)
    {
        position = rigidbody2D.position;
        rotation = rigidbody2D.rotation;
        velocity = rigidbody2D.velocity;
        angularVelocity = rigidbody2D.angularVelocity;

        this.tickNumber = tickNumber;

        input = new MyInput();
    }

    public void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref position);
        serializer.Serialize(ref rotation);
        serializer.Serialize(ref velocity);
        serializer.Serialize(ref angularVelocity);
        serializer.Serialize(ref tickNumber);
    }
}
