using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;

public class ShootProjectile : NetworkBehaviour
{
    [SerializeField] private GameObject hitParticles;

    [SerializeField] private Transform shootTransform;
    
    [SerializeField] private List<GameObject> spawnedProjectiles = new List<GameObject>();
    void Update()
    {
        if(!IsOwner) return;

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            ShootServerRPC();
        }
    }

    [ServerRpc]

    private void ShootServerRPC()
    {
        GameObject go = Instantiate(hitParticles, shootTransform.position, shootTransform.rotation);
        spawnedProjectiles.Add(go);
        go.GetComponent<MoveProjectile>().parent = this;
        go.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc]

    public void DestroyServerRPC()
    {
        GameObject toDestroy = spawnedProjectiles[0];
        toDestroy.GetComponent<NetworkObject>().Despawn();
        spawnedProjectiles.Remove(toDestroy);
        Destroy(toDestroy);
    }
}
