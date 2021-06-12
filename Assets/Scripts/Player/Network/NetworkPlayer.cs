using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using MLAPI;
using MLAPI.Messaging;
using MLAPI.Connection;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerMovement), typeof(PlayerAnimation))]
[RequireComponent(typeof(PlayerInput), typeof(PlayerStunts))]
public class NetworkPlayer : NetworkBehaviour
{
    //Components
    public Rigidbody2D rigidBody;
    public PlayerMovement playerMovement;
    public PlayerInput playerInput;
    public PlayerAnimation playerAnimation;
    public PlayerStunts playerStunts;
    public  GameObject playerDummy;

    //Network
        //Client
    const int bufferLength = 1024;
    private ClientState[] clientStates = new ClientState[bufferLength];
    private int tickNumber = 0;
    public float threhold = 1;
    private Scene sceneClone;
    private PhysicsScene2D physicsScene2D;
        //Server
    private NetworkPhysicsManager physicsManager;

    public override void NetworkStart()
    {
        base.NetworkStart();
    }

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerStunts = GetComponent<PlayerStunts>();
    }

    void Start()
    {
        if(IsClient) CreateCloneScene();
        else physicsManager = GameObject.FindObjectOfType<NetworkPhysicsManager>();
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
        SendInputServerRpc(inputMessage, NetworkManager.Singleton.LocalClientId);

        playerMovement.Movement(input);
        playerStunts.Stunts(input);

        Physics2D.Simulate(Time.deltaTime);

        tickNumber++;
    }

    void ServerUpdate()
    {

    }

    [ServerRpc]
    void SendInputServerRpc(InputMessage message, ulong clientId)
    {
        MyInput input = new MyInput(message);

        playerMovement.Movement(input);
        playerStunts.Stunts(input);
        playerInput.input = input;

        Physics2D.Simulate(Time.fixedDeltaTime);

        // SendStateClientRpc(new StateMessage(rigidBody, message.tickNumber+1), input);
    }

    [ClientRpc]
    public void SendStateClientRpc(StateMessage message, MyInput input)
    {
        message.input = input;

        int bufferSlot = message.tickNumber % bufferLength;
        Vector2 difference = message.position - clientStates[bufferSlot].position;
        float rotationDiffrenece = message.rotation - clientStates[bufferSlot].rotation;

        if(IsLocalPlayer)
        {            
            if((difference.magnitude > threhold || rotationDiffrenece > threhold)  && message.tickNumber == clientStates[bufferSlot].tickNumber)
            {
                GameObject dummy = Instantiate(playerDummy);
                SceneManager.MoveGameObjectToScene(dummy, sceneClone);

                Rigidbody2D dummyRigidbody = dummy.GetComponent<Rigidbody2D>();
                PhysicsMovement dummyMovement = dummy.GetComponent<PhysicsMovement>();
                PhysicsStunts dummyStunts = dummy.GetComponent<PhysicsStunts>();

                dummyRigidbody.position = message.position;
                dummyRigidbody.velocity = message.velocity;
                dummyRigidbody.rotation = message.rotation;
                dummyRigidbody.angularVelocity = message.angularVelocity;
                

                int rewindTick = message.tickNumber;
                while (rewindTick < tickNumber)
                {
                    int rewindBufferSlot = rewindTick % bufferLength;

                    dummyMovement.Movement(clientStates[rewindBufferSlot].input);
                    dummyStunts.Stunts(clientStates[rewindBufferSlot].input);

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
                rigidBody.rotation = Mathf.Lerp(rigidBody.rotation, dummyRigidbody.rotation, Time.deltaTime * 10);
                rigidBody.angularVelocity = dummyRigidbody.angularVelocity;

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

    void CreateCloneScene()
    {
        if(!IsLocalPlayer) return;

        sceneClone = SceneManager.CreateScene(
            "ClientScene",
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

    [ClientRpc]
    public void ResetTickClientRpc()
    {
        tickNumber = 0;
    }
}