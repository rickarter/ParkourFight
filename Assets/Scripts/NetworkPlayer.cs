using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public  GameObject playerDummy;

    //Network
    public float threhold = 1;

    const int bufferLength = 1024;
    private ClientState[] clientStates = new ClientState[bufferLength];

    private int tickNumber = 0;

    private Scene sceneClone;
    private PhysicsScene2D physicsScene2D;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    void Start()
    {
        if(IsClient) ClientCloneScene();
    }

    // Update is called once per frame
    void FixedUpdate() 
    {
        if(IsClient) ClientUpdate();
        else ServerUpdate();
    }

    void ClientUpdate()
    {
        if(!IsLocalPlayer) return;

        MyInput input = playerInput.input;

        int bufferSlot = tickNumber % bufferLength;
        clientStates[bufferSlot] = new ClientState(input, rigidBody, tickNumber);

        InputMessage inputMessage = new InputMessage(input, tickNumber);
        SendInputServerRpc(inputMessage);

        playerMovement.Movement(input);

        Physics2D.Simulate(Time.deltaTime);

        tickNumber++;
    }

    void ServerUpdate()
    {

    }

    [ServerRpc]
    void SendInputServerRpc(InputMessage message)
    {
        MyInput input = new MyInput(message);

        playerInput.input = input;
        playerMovement.Movement(input);

        Physics2D.Simulate(Time.fixedDeltaTime);

        SendStateClientRpc(new StateMessage(rigidBody, message.tickNumber+1), input);
    }

    [ClientRpc]
    void SendStateClientRpc(StateMessage message, MyInput input)
    {
        message.input = input;

        int bufferSlot = message.tickNumber % bufferLength;
        Vector2 difference = message.position - clientStates[bufferSlot].position;

        if(IsLocalPlayer)
        {
            /*if(message.tickNumber != clientStates[bufferSlot].tickNumber) 
            {
                Debug.Log(message.tickNumber.ToString() + "--" + clientStates[bufferSlot].tickNumber.ToString() + "--" + tickNumber.ToString());
                Debug.Log(message.position.ToString() + "--" + clientStates[bufferSlot].position.ToString());
            }*/
            
            if(difference.magnitude > threhold && message.tickNumber == clientStates[bufferSlot].tickNumber)
            {
                // Debug.Log(difference.magnitude);
                // Debug.Break();
                GameObject dummy = Instantiate(playerDummy);
                SceneManager.MoveGameObjectToScene(dummy, sceneClone);

                Rigidbody2D dummyRigidbody = dummy.GetComponent<Rigidbody2D>();
                PhysicsMovement dummyMovement = dummy.GetComponent<PhysicsMovement>();

                dummyRigidbody.position = message.position;
                dummyRigidbody.velocity = message.velocity;
                

                int rewindTick = message.tickNumber;
                while (rewindTick < tickNumber)
                {
                    int rewindBufferSlot = rewindTick % bufferLength;

                    dummyMovement.Movement(clientStates[rewindBufferSlot].input);

                    clientStates[rewindBufferSlot].position = dummyRigidbody.position;
                    clientStates[rewindBufferSlot].velocity = dummyRigidbody.velocity;

                    physicsScene2D.Simulate(Time.fixedDeltaTime);

                    rewindTick++;
                }

                Vector2 positionError = dummyRigidbody.position - rigidBody.position;
                if(positionError.magnitude > 2.0f)
                    rigidBody.position = dummyRigidbody.position;
                else if(positionError.magnitude > 0.1f)
                    rigidBody.position += positionError * Time.deltaTime * 10;

                rigidBody.velocity = dummyRigidbody.velocity;

                Destroy(dummy);
            }
        }
        else
        {
            if(difference.magnitude > 2.0f)
                rigidBody.position = message.position;
            else
                rigidBody.position += difference * Time.deltaTime * 10;
            playerInput.input = message.input;
            rigidBody.velocity = message.velocity;
        }
    }

    void ClientCloneScene()
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

    MyInput InputMSGtoInput(InputMessage message)
    {
        MyInput input = new MyInput();
        input.x = message.x;
        input.y = message.y;
        input.jumping = message.jumping;

        return input;
    }
}