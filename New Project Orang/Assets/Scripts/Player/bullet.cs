using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 10f;
    public float bulletLifeTime = 10f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * bulletSpeed;
        rb.gravityScale = 0;
    }

    void Update()
    {
        rb.velocity = rb.velocity.normalized * bulletSpeed;
        Destroy(gameObject, bulletLifeTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Bullet collided with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Vector2 reflectDirection = Vector2.Reflect(rb.velocity.normalized, collision.contacts[0].normal);
            rb.velocity = reflectDirection * bulletSpeed;
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            Vector2 reflectDirection = Vector2.Reflect(rb.velocity.normalized, collision.contacts[0].normal);
            rb.velocity = reflectDirection * bulletSpeed;
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}



