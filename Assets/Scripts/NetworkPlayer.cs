    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAPI;
using MLAPI.Messaging;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerMovement), typeof(PlayerAnimation))]
[RequireComponent(typeof(PlayerInput))]
public class NetworkPlayer : NetworkBehaviour
{
    //Components
    public Rigidbody2D rigidBody;
    public PlayerMovement playerMovement;
    public PlayerInput playerInput;
    public PlayerAnimation playerAnimation;

    //Network
    private List<InputMessage> inputMessages = new List<InputMessage>();
    private List<StateMessage> stateMessages = new List<StateMessage>();
    private int tickNumber = 0;
    private int lastInputRead = 0;
    private int lastStateRead = 0;

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
    void FixedUpdate() 
    {
        if(IsClient) ClientUpdate();
        else ServerUpdate();
    }

    void ClientUpdate()
    {
        MyInput input = playerInput.input;

        if(IsLocalPlayer) SendInputServerRpc(new InputMessage()
        {
            x = input.x,
            y = input.y,
            jumping = input.jumping
        });

        if(lastStateRead < stateMessages.Count)
        {
            StateMessage message = stateMessages[lastStateRead];
            Vector2 difference = message.position - rigidBody.position;

            float distance = difference.magnitude;

            if(distance > 2.0f)
                rigidBody.position = message.position;
            else if(distance > 0.1f)
                rigidBody.position += difference * 0.1f;

            rigidBody.velocity = message.velocity;
            playerInput.input = message.input;

            //To sync animations

            lastStateRead++;
        }

        // Physics2D.Simulate(Time.fixedDeltaTime);
    }

    void ServerUpdate()
    {
        MyInput input = new MyInput();

        if(lastInputRead < inputMessages.Count)
        {
            InputMessage message = inputMessages[lastInputRead];
            input.x = message.x;
            input.y = message.y;
            input.jumping = message.jumping;

            //To sync animations
            playerInput.input = input;

            lastInputRead++;
        }

        playerMovement.Movement(input);

        SendStateClientRpc(new StateMessage()
        {
            position = rigidBody.position,
            rotation = rigidBody.rotation,
            velocity = rigidBody.velocity,
            angularVelocity = rigidBody.angularVelocity,
            tickNumber = tickNumber,
        }, input);
        // Physics2D.Simulate(Time.fixedDeltaTime);
    }

    [ServerRpc]
    void SendInputServerRpc(InputMessage message)
    {
        inputMessages.Add(message);
    }

    [ClientRpc]
    void SendStateClientRpc(StateMessage message, MyInput input)
    {
        message.input = input;
        stateMessages.Add(message);
    }
}
