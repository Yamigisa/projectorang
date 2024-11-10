using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public float bulletSpeed = 10f;
    public float bulletLifeTime = 10f;
    private Rigidbody2D rb;
    public GameObject impactEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * bulletSpeed;
        rb.gravityScale = 0;

        if (IsServer)
        {
            StartCoroutine(DestroyBulletAfterLifetime());
        }
    }

    private void Update()
    {
        rb.velocity = rb.velocity.normalized * bulletSpeed;
    }

    private IEnumerator DestroyBulletAfterLifetime()
    {
        yield return new WaitForSeconds(bulletLifeTime);
        if (IsServer)
        {
            DespawnBullet();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Buat VFX di posisi tabrakan
            GameObject vfx = Instantiate(impactEffect, collision.contacts[0].point, Quaternion.identity);
            ReflectBullet(collision);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            if (IsServer)
            {
                HandlePlayerCollision(collision);
                DespawnBullet();
            }
        }
        else
        {
            if (IsServer)
            {
                DespawnBullet();
            }
        }
    }

    private void ReflectBullet(Collision2D collision)
    {
        Vector2 reflectDirection = Vector2.Reflect(rb.velocity.normalized, collision.contacts[0].normal);
        rb.velocity = reflectDirection * bulletSpeed;
    }

    private void HandlePlayerCollision(Collision2D collision)
    {
        PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.TakeDamage(1);
        }
    }

    private void DespawnBullet()
    {
        if (IsSpawned)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
