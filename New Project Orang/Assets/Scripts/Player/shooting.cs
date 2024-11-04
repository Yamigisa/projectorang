using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Shooting : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float bulletLifeTime = 20f;

    private int bulletsShot = 0;
    private bool isOnCooldown = false;
    public int maxBulletsBeforeCooldown = 6;
    public float cooldownTime = 10f;

    private PlayerStats playerStats;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isOnCooldown && IsOwner && playerStats.isActive.Value)
        {
            ShootServerRPC();
        }
    }

    IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        bulletsShot = 0;
        isOnCooldown = false;
    }

    [ServerRpc]
    private void ShootServerRPC()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.up * bulletSpeed;

        NetworkObject networkBullet = bullet.GetComponent<NetworkObject>();
        networkBullet.Spawn();

        StartCoroutine(DestroyBulletAfterLifetime(networkBullet));

        bulletsShot++;

        if (bulletsShot >= maxBulletsBeforeCooldown)
        {
            StartCoroutine(Cooldown());
        }
    }

    private IEnumerator DestroyBulletAfterLifetime(NetworkObject bullet)
    {
        yield return new WaitForSeconds(bulletLifeTime);
        
        if (IsServer && bullet != null && bullet.IsSpawned)
        {
            bullet.Despawn();
        }
    }
}
