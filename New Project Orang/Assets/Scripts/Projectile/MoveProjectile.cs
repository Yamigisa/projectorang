using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class MoveProjectile : NetworkBehaviour
{
    public ShootProjectile parent;

    [SerializeField] private GameObject hitParticles;
    [SerializeField] private float shootForce;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = rb.transform.forward * shootForce;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!IsOwner) return;
        InstantiateHitParticlesServerRPC();
        parent.DestroyServerRPC();
    }

    [ServerRpc]

    private void InstantiateHitParticlesServerRPC ()
    {
        GameObject hitImpact = Instantiate(hitParticles, transform.position, Quaternion.identity);
        hitImpact.GetComponent<NetworkObject>().Spawn();
        hitImpact.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
    }
}
