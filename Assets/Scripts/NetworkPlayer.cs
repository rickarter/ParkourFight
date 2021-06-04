using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAPI;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerMovement), typeof(PlayerAnimation))]
[RequireComponent(typeof(PlayerInput))]
public class NetworkPlayer : NetworkBehaviour
{
    //Components
    public Rigidbody2D rigidBody;
    public PlayerMovement playerMovement;
    public PlayerInput playerInput;
    public PlayerAnimation playerAnimation;

    void Reset()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
