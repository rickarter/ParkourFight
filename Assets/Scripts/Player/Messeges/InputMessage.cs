using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAPI;
using MLAPI.Messaging;
using MLAPI.Serialization;

public struct InputMessage: INetworkSerializable
{
    public float x, y;
    public float tuck;
    public bool jumping, backflip, frontflip;
    public int tickNumber;

    public InputMessage(MyInput input, int tickNumber)
    {
        x = input.x;
        y = input.y;
        tuck = input.tuck;
        jumping = input.jumping;
        backflip = input.backflip;
        frontflip = input.frontflip;
        this.tickNumber = tickNumber;
    }

    // INetworkSerializable
    public void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref x);
        serializer.Serialize(ref y);
        serializer.Serialize(ref tuck);
        serializer.Serialize(ref jumping);
        serializer.Serialize(ref backflip);
        serializer.Serialize(ref frontflip);
        serializer.Serialize(ref tickNumber);
    }
}
