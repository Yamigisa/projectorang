using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
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
                playerShooting.ActivatePowerUp(powerUpType);
                Destroy(gameObject); // Hapus power-up setelah diambil
            }
        }
    }
}
