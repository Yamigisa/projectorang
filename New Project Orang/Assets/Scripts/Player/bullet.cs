using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 10f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.velocity = transform.up * bulletSpeed;

        rb.gravityScale = 0;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

    }


    void Update()
    {
        Destroy(gameObject, 5f);
    }
}
