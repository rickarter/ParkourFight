using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class NetworkPhysics : NetworkBehaviour
{
    private NetworkVariableVector3 netTransfrom = new NetworkVariableVector3(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.Everyone,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    public override void NetworkStart()
    {
        if(!IsLocalPlayer)
        {
            // GetComponent<Rigidbody2D>().isKinematic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        SyncTransform();
    }

    void SyncTransform()
    {
        if(IsLocalPlayer)
        {
            netTransfrom.Value = transform.position;
        }
        else
        {
            transform.position = netTransfrom.Value;
        }
    }
}
