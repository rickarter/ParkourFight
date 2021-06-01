using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAPI;
using MLAPI.Messaging;

public class MyInput
{
    public float x, y;
    public bool jumping;

    [ClientRpc]
    public void InputClientRpc()
    {
        Input();
    }

    public void Input()
    {
        x = UnityEngine.Input.GetAxisRaw("Horizontal");
        y = UnityEngine.Input.GetAxisRaw("Vertical");
        jumping = UnityEngine.Input.GetButton("Jump");
    }
}
