using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAPI;
using MLAPI.Messaging;
using MLAPI.Serialization;

public class MyInput : INetworkSerializable
{
    public float x, y;
    public bool jumping;

    public MyInput()
    {
        x = 0;
        y = 0;
        jumping = false;
    }

    public MyInput(InputMessage message)
    {
        x = message.x;
        y = message.y;
        jumping = message.jumping;
    }

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
    
    public void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref x);
        serializer.Serialize(ref y);
        serializer.Serialize(ref jumping);
    }
}
