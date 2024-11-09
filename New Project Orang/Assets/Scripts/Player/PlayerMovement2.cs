using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 5f;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Get the spawn point based on player index
            int playerId = (int)OwnerClientId; // OwnerClientId is the unique ID for the client that owns the object
            Transform spawnPoint = GameManager.instance.GetSpawnPoint(playerId);

            // Update the position and rotation to match the spawn point
            UpdatePositionServerRPC(spawnPoint.position, spawnPoint.rotation);
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        // Handle movement logic
        float horizontalInput = Input.GetAxis("Horizontal"); 
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, verticalInput, 0);
        movementDirection.Normalize();

        transform.Translate(movementDirection * movementSpeed * Time.deltaTime, Space.World);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePositionServerRPC(Vector3 position, Quaternion rotation)
    {
        // Update the position and rotation on the server to sync with other clients
        transform.position = position;
        transform.rotation = rotation;
    }
}
