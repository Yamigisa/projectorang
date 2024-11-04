using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    //[SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private float positionRange = 5f;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        UpdatePositionServerRPC();
    }
    void Update()
    {
        if(!IsOwner) return;
        float horizontalInput = Input.GetAxis("Horizontal"); 
        float verticalInput = Input.GetAxis("Vertical");       

        Vector3 movementDirection = new Vector3(horizontalInput, verticalInput, 0);
        movementDirection.Normalize();

        transform.Translate(movementDirection * movementSpeed * Time.deltaTime, Space.World);
    }

    [ServerRpc(RequireOwnership = false)]

    public void UpdatePositionServerRPC()
    {
        transform.position = new Vector3(Random.Range(positionRange,-positionRange), Random.Range(positionRange,-positionRange), 0);
        transform.rotation = new Quaternion(0,0,0,0);
    }
}
