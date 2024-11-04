using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public float bulletSpeed = 10f;
    public float bulletLifeTime = 10f;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * bulletSpeed;
        rb.gravityScale = 0;

        // Only the server should handle lifetime destruction
        if (IsServer)
        {
            StartCoroutine(DestroyBulletAfterLifetime());
        }
    }

    private void Update()
    {
        // Keep the bullet's speed constant
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
        Debug.Log("Bullet collided with: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Bullet"))
        {
            ReflectBullet(collision);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            // Only apply damage and destroy on the server
            if (IsServer)
            {
                HandlePlayerCollision(collision);
                DespawnBullet();
            }
        }
        else
        {
            // Only the server should despawn the bullet
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
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(1);
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
