using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PowerUp : NetworkBehaviour
{
    public enum PowerUpType { Shotgun, BurstFire }
    public PowerUpType powerUpType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Shooting playerShooting = collision.GetComponent<Shooting>();
            if (playerShooting != null)
            {
                Debug.Log("power up: " + powerUpType);
                playerShooting.ActivatePowerUp(powerUpType);
                DespawnPowerUpServerRpc();
            }
        }
    }

    [ServerRpc]
    private void DespawnPowerUpServerRpc()
    {
        NetworkObject.Despawn();
    }
}
