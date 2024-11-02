using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float bulletLifeTime = 20f;

    private int bulletsShot = 0;
    private bool isOnCooldown = false;
    public int maxBulletsBeforeCooldown = 6;
    public float cooldownTime = 10f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isOnCooldown)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.up * bulletSpeed;

        Destroy(bullet, bulletLifeTime);

        bulletsShot++;

        if (bulletsShot >= maxBulletsBeforeCooldown)
        {
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        bulletsShot = 0;
        isOnCooldown = false;
    }
}
