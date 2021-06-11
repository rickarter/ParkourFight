using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAPI;
using MLAPI.Messaging;
using MLAPI.Serialization;

public class MyInput : INetworkSerializable
{
    public float x, y;
    public float tuck;
    public bool jumping, backflip, frontflip;

    public MyInput()
    {
        x = 0;
        y = 0;
        tuck = 0;
        backflip = false;
        frontflip = false;
        jumping = false;
    }

    public MyInput(InputMessage message)
    {
        x = message.x;
        y = message.y;
        tuck = message.tuck;
        backflip = message.backflip;
        jumping = message.jumping;
        frontflip = message.frontflip;
    }

    public void Input()
    {
        x = UnityEngine.Input.GetAxisRaw("Horizontal");
        y = UnityEngine.Input.GetAxisRaw("Vertical");
        tuck = UnityEngine.Input.GetAxis("Tuck");
        jumping = UnityEngine.Input.GetButton("Jump");
        backflip = UnityEngine.Input.GetButton("Backflip");
        frontflip = UnityEngine.Input.GetButton("Frontflip");
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
        serializer.Serialize(ref tuck);
        serializer.Serialize(ref backflip);
        serializer.Serialize(ref frontflip);
    }
}
