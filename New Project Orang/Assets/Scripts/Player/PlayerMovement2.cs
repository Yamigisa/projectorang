using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement2 : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private Rigidbody2D rb;
    private Vector2 movement;

    [SerializeField] private float positionRange = 3f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
    }

    public override void OnNetworkSpawn()
    {
        if(!IsOwner)
        UpdatePositionServerRPC();
    }

    void FixedUpdate()
    {
        if (movement != Vector2.zero)
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime); // Interpolasi rotasi agar lebih halus
        }

        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    [ServerRpc(RequireOwnership = false)]

    public void UpdatePositionServerRPC()
    {
        transform.position = new Vector3(Random.Range(positionRange,-positionRange), Random.Range(positionRange,-positionRange), 0);
        transform.rotation = new Quaternion(0,0,0,0);
    }

}
