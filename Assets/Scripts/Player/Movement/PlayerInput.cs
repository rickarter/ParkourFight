using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    //Components
    private PlayerScript player;
    public bool isFirstPlayer = true;

    public MyInput input = new MyInput();

    void Start()
    {
        player = GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isFirstPlayer)
            input.Input();
        else
            SecondInput();
    }

    void SecondInput()
    {
        input.x = UnityEngine.Input.GetAxisRaw("Horizontal1");
        input.y = UnityEngine.Input.GetAxisRaw("Vertical");
        input.tuck = UnityEngine.Input.GetAxis("Tuck1");
        input.jumping = UnityEngine.Input.GetButton("Jump1");
        input.backflip = UnityEngine.Input.GetButton("Backflip1");
        input.frontflip = UnityEngine.Input.GetButton("Frontflip1");
        input.fire = UnityEngine.Input.GetAxis("Fire1");
        Vector2 direction = (Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position).normalized;
        input.aim = direction;
    }
}
