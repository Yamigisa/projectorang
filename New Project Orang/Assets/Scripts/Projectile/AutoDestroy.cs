using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AutoDestroy : NetworkBehaviour
{
    public float delayBeforeDestroy = 3f;
    void Start()
    {
    
    }

    [ServerRpc(RequireOwnership = false)]

    private void DestroyParticlesServerRPC()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject, delayBeforeDestroy);
    }
}
