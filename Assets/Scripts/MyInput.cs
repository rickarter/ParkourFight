using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAPI;
using MLAPI.Messaging;
using MLAPI.Serialization;

public class MyInput : INetworkSerializable
{
    public float x = 0f, y = 0f;
    public bool jumping = false;

    public void Input()
    {
        x = UnityEngine.Input.GetAxisRaw("Horizontal");
        y = UnityEngine.Input.GetAxisRaw("Vertical");
        jumping = UnityEngine.Input.GetButton("Jump");
    }

    [ClientRpc]
    public void InputClientRpc()
    {
        Input();
    }

    // INetworkSerializable
    public void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref x);
        serializer.Serialize(ref y);
        serializer.Serialize(ref jumping);
    }
}
