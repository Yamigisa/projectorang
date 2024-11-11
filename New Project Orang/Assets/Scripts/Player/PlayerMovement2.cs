using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 50f;
    [SerializeField] private float rotationSpeed = 10f;

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float smoothTime = 0.1f;
    private float updateInterval = 0.1f;
    private float timeSinceLastUpdate;

    private Vector3 movementDirection = Vector3.zero;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            int playerId = (int)OwnerClientId;
            Transform spawnPoint = GameManager.instance.GetSpawnPoint(playerId);

            // Call the ClientRpc to update the spawn position for all clients
            SetPositionClientRpc(spawnPoint.position, spawnPoint.rotation);

            // Optionally, you can update the server's position as well, if needed.
            UpdatePositionServerRPC(spawnPoint.position, spawnPoint.rotation);
        }
    }

    void Update()
    {
        if(GameManager.instance.canPlay)
        {
            if (IsOwner)
            {
                // Handle player movement inputs
                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");

                movementDirection = new Vector3(horizontalInput, verticalInput, 0).normalized;

                // Calculate new position and apply smooth transition
                Vector3 newPosition = transform.position + movementDirection * movementSpeed * Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, newPosition, smoothTime);

                if (movementDirection != Vector3.zero)
                {
                    float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
                    Quaternion newRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
                    transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
                }

                // Update the server position periodically
                timeSinceLastUpdate += Time.deltaTime;
                if (timeSinceLastUpdate >= updateInterval)
                {
                    UpdatePositionServerRPC(transform.position, transform.rotation);
                    timeSinceLastUpdate = 0f;
                }
            }
            else
            {
                // Smooth transition for non-owner players
                transform.position = Vector3.Lerp(transform.position, targetPosition, smoothTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothTime);
            }
        }
    }

    // Server RPC to update position and rotation on the server
    [ServerRpc(RequireOwnership = false)]
    public void UpdatePositionServerRPC(Vector3 position, Quaternion rotation)
    {
        targetPosition = position;
        targetRotation = rotation;
    }

    // ClientRpc to set the spawn position for all clients
    [ClientRpc]
    public void SetPositionClientRpc(Vector3 position, Quaternion rotation)
    {
        // Set the position and rotation for the player on the client side
        transform.position = position;
        transform.rotation = rotation;
    }
}
