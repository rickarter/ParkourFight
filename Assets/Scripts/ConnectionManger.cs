using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 using UnityEngine.SceneManagement;

using MLAPI;

public class ConnectionManger : MonoBehaviour
{
    static private string ip;

    void Start()
    {
        ip = "127.0.0.1";
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
                DisconnectButton();
            }

        GUILayout.EndArea();
    }

    static void StartButtons()
    {
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();

        GUILayout.BeginHorizontal();

            ip = GUILayout.TextField(ip);

            if (GUILayout.Button("Client")) 
            {
                NetworkManager.Singleton.StartClient();
            }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }

    static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }

    static void DisconnectButton()
    {
        if(GUILayout.Button("Disconnect"))
        {
            // var mode = NetworkManager.Singleton.IsHost ?
            // NetworkManager.Singleton.StopHost() : NetworkManager.Singleton.IsServer ? "Server" : "Client";
            if(NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.StopHost();
            }
            else if(NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.StopClient();
            }
            else
            {
                NetworkManager.Singleton.StopServer();
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
