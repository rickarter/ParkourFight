using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyInput
{
    public float x, y;
    public float tuck, fire;
    public bool jumping, backflip, frontflip;
    public Vector2 aim;

    public MyInput()
    {
        x = 0;
        y = 0;
        tuck = 0;
        backflip = false;
        frontflip = false;
        jumping = false;
        aim = Vector2.zero;
    }

    public void Input()
    {
        x = UnityEngine.Input.GetAxisRaw("Horizontal");
        y = UnityEngine.Input.GetAxisRaw("Vertical");
        tuck = UnityEngine.Input.GetAxis("Tuck");
        jumping = UnityEngine.Input.GetButton("Jump");
        backflip = UnityEngine.Input.GetButton("Backflip");
        frontflip = UnityEngine.Input.GetButton("Frontflip");
        fire = UnityEngine.Input.GetAxisRaw("Fire");
        aim = new Vector3(UnityEngine.Input.GetAxis("AimX"), UnityEngine.Input.GetAxis("AimY"));
        aim.Normalize();
    }
}
