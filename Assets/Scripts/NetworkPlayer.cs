using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    const int bufferLength = 1024;
    private InputMessage[] inputBuffer = new InputMessage[bufferLength];
    private StateMessage[] stateBuffer = new StateMessage[bufferLength];

    private InputMessage[] inputMessages = new InputMessage[bufferLength];
    private StateMessage[] stateMessages = new StateMessage[bufferLength];
    private int tickNumber = 0;
    private int lastInputWriteTick = 0;
    private int lastInputReadTick = 0;
    private int lastStateWriteTick = 0;
    private int lastStateReadTick = 0;

    private Scene sceneClone;
    private PhysicsScene2D physicsScene2D;

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
        CreateCloneScene();
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

        if(IsLocalPlayer)
        {
            InputMessage inputMessage = new InputMessage()
            {
                x = input.x,
                y = input.y,
                jumping = input.jumping,
                tickNumber = tickNumber
            };
            SendInputServerRpc(inputMessage);

            playerMovement.Movement(input);

            int bufferSlot = tickNumber % bufferLength;
            inputBuffer[bufferSlot] = inputMessage;
            stateBuffer[bufferSlot] = new StateMessage()
            {
                position = rigidBody.position,
                rotation = rigidBody.rotation,
                velocity = rigidBody.velocity,
                angularVelocity = rigidBody.angularVelocity,
                tickNumber = tickNumber,
            };

            tickNumber++;
        }

        if(HasAvailiableStateMessages())
        {
            int bufferSlot = lastStateReadTick % bufferLength;
            StateMessage message = stateMessages[bufferSlot];   

            Vector2 difference = message.position - rigidBody.position;
            float distance = difference.magnitude;
            if(IsLocalPlayer)
            {
                if(distance > 4)
                {
                    rigidBody.position = message.position;
                }
            }
            else
            {
                if(distance > 2.0f)
                    rigidBody.position = message.position;
                else if(distance > 0.1f)
                    rigidBody.position += difference * 0.1f;
                rigidBody.velocity = message.velocity;
                playerInput.input = message.input;
            }

            lastStateReadTick++;
        }
    }

    void ServerUpdate()
    {
        MyInput input = new MyInput();

        if(HasAvailiableInputMessages())
        {
            int bufferSlot = lastInputReadTick % bufferLength;
            InputMessage message = inputMessages[bufferSlot];

            input.x = message.x;
            input.y = message.y;
            input.jumping = message.jumping;

            playerInput.input = input;

            playerMovement.Movement(input);

            SendStateClientRpc(new StateMessage()
            {
                position = rigidBody.position,
                rotation = rigidBody.rotation,
                velocity = rigidBody.velocity,
                angularVelocity = rigidBody.angularVelocity,
                tickNumber = lastInputReadTick,
            }, input);

            lastInputReadTick++;
        }
    }

    [ServerRpc]
    void SendInputServerRpc(InputMessage message)
    {
        inputMessages[message.tickNumber % bufferLength] = message;
        lastInputWriteTick++;
    }

    [ClientRpc]
    void SendStateClientRpc(StateMessage message, MyInput input)
    {
        message.input = input;
        stateMessages[message.tickNumber % bufferLength] = message;
        lastStateWriteTick++;
    }

    bool HasAvailiableInputMessages()
    {
        return lastInputReadTick < lastInputWriteTick;
    }

    bool HasAvailiableStateMessages()
    {
        return lastStateReadTick < lastStateWriteTick;
    }

    void CreateCloneScene()
    {
        if(!IsLocalPlayer) return;

        sceneClone = SceneManager.CreateScene(
            "CloneScene",
            new CreateSceneParameters(LocalPhysicsMode.Physics2D)
        );
        physicsScene2D = sceneClone.GetPhysicsScene2D();

        GameObject level = Instantiate(GameObject.FindGameObjectWithTag("Level"));

        SceneManager.MoveGameObjectToScene(level, sceneClone);

     }
}