using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using MLAPI;
using MLAPI.Connection;

public class NetworkPhysicsManager : NetworkBehaviour
{
    private int tickNumber = 0;

    const int bufferLength = 1024;
    const int maxClients = 4;
    private InputMessage[,] clientMessages = new InputMessage[maxClients, bufferLength];
    private int[] clientTicks = new int[maxClients];

    public override void NetworkStart()
    {
        if(!IsServer) return;
        NetworkManager.Singleton.OnClientConnectedCallback += ResetTicks;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!NetworkManager.Singleton.IsListening || NetworkManager.Singleton.ConnectedClientsList.Count == 0) return;

        if(HasReceivedInputs())
        {
            while(tickNumber < GetLastCorrespondingTick())
            {
                int bufferSlot = tickNumber % bufferLength;

                foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    MyInput input = new MyInput(clientMessages[client.ClientId-2, bufferSlot]);

                    client.PlayerObject.GetComponent<NetworkPlayer>().playerMovement.Movement(input);
                }

                Physics2D.Simulate(Time.fixedDeltaTime);

                foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    client.PlayerObject.GetComponent<NetworkPlayer>().SendStateClientRpc(new StateMessage(client.PlayerObject.GetComponent<Rigidbody2D>(), tickNumber+1), 
                    new MyInput(clientMessages[client.ClientId-2, bufferSlot]));
                }

                tickNumber++;
            }
        }
    }

    void ResetTicks(ulong clientId)
    {
        tickNumber = 0;
        for(int i = 0; i < maxClients; i++)
            clientTicks[i] = 0;

        foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            client.PlayerObject.GetComponent<NetworkPlayer>().ResetTickClientRpc();
        }
    }

    bool HasReceivedInputs()
    {
        bool result = true;
        for(int i = 0; i < NetworkManager.Singleton.ConnectedClientsList.Count; i++)
            if(clientTicks[i] < tickNumber)
                result = false;

        return result;
    }

    int GetLastCorrespondingTick()
    {
        int min = clientTicks[0];

        for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsList.Count; i++) 
        {
            int number = clientTicks[i];
            if (number < min) 
            {
                min = number;
            }
        }

        return min;
    }

    public void AddInputMessage(ulong clientId, InputMessage message)
    {
        int bufferSlot = message.tickNumber % bufferLength;
        ulong clientSlot = clientId-2;

        clientMessages[clientSlot, bufferSlot] = message;

        clientTicks[clientSlot] = message.tickNumber;
    }
}
