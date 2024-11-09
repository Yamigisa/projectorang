using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Shooting : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    private bool shotgunActive = false;
    private bool burstFireActive = false;

    private int burstFireShots = 0;
    private float powerUpDuration = 60f;

    void Update()
    {
        if (IsOwner) 
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (shotgunActive)
                {
                    ShootShotgun();
                }
                else if (burstFireActive && burstFireShots < 6)
                {
                    StartCoroutine(ShootBurstFire());
                    burstFireShots++;
                }
                else
                {
                    Shoot();
                }
            }
        }
    }

    void Shoot()
    {
        if (IsServer)
        {
            SpawnBullet(firePoint.position, firePoint.rotation);
        }
        else
        {
            ShootServerRpc(firePoint.position, firePoint.rotation);
        }
    }

    void ShootShotgun()
    {
        float angleOffset = 15f;

        for (int i = -1; i <= 1; i++)
        {
            if (IsServer)
            {
                SpawnBullet(firePoint.position, Quaternion.Euler(0, 0, firePoint.eulerAngles.z + (angleOffset * i)));
            }
            else
            {
                ShootServerRpc(firePoint.position, Quaternion.Euler(0, 0, firePoint.eulerAngles.z + (angleOffset * i)));
            }
        }
    }

    IEnumerator ShootBurstFire()
    {
        for (int i = 0; i < 4; i++)
        {
            Shoot();
            yield return new WaitForSeconds(0.1f);
        }
        if (burstFireShots >= 6)
        {
            burstFireActive = false;
            burstFireShots = 0;
        }
    }

    [ServerRpc]
    void ShootServerRpc(Vector3 position, Quaternion rotation)
    {
        SpawnBullet(position, rotation);
    }

    void SpawnBullet(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = bullet.transform.up * bulletSpeed;

        bullet.GetComponent<NetworkObject>().Spawn();
    }

    public void ActivatePowerUp(PowerUp.PowerUpType powerUpType)
    {
        if (powerUpType == PowerUp.PowerUpType.Shotgun)
        {
            shotgunActive = true;
            StartCoroutine(ResetPowerUpAfterDuration(() => shotgunActive = false));
        }
        else if (powerUpType == PowerUp.PowerUpType.BurstFire)
        {
            burstFireActive = true;
            burstFireShots = 0;
            StartCoroutine(ResetPowerUpAfterDuration(() => burstFireActive = false));
        }
    }

    private IEnumerator ResetPowerUpAfterDuration(System.Action onComplete)
    {
        yield return new WaitForSeconds(powerUpDuration);
        onComplete.Invoke();
    }
}
