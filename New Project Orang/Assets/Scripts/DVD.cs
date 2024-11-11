using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBullet : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    public GameObject impactEffect;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        rb.velocity = randomDirection * speed;
    }

    void Update()
    {
        if (rb.velocity.magnitude < speed)
        {
            rb.velocity = rb.velocity.normalized * speed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            var glowEffect = collision.gameObject.GetComponent<ObstacleGlowEffect>();
            if (glowEffect != null)
            {
                glowEffect.TriggerGlow();
            }
            if (impactEffect != null)
            {
                GameObject vfx = Instantiate(impactEffect, collision.contacts[0].point, Quaternion.identity);
                Destroy(vfx, 5f);
            }
        }

    }
}

