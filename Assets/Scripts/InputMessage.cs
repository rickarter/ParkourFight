using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAPI;
using MLAPI.Messaging;
using MLAPI.Serialization;

public struct InputMessage: INetworkSerializable
{
    public float x, y;
    public bool jumping;
    public int tickNumber;

    // INetworkSerializable
    public void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref x);
        serializer.Serialize(ref y);
        serializer.Serialize(ref jumping);
        serializer.Serialize(ref tickNumber);
    }
}
