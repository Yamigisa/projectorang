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

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.up * bulletSpeed;
    }

    void ShootShotgun()
    {
        float angleOffset = 15f;

        for (int i = -1; i <= 1; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, firePoint.eulerAngles.z + (angleOffset * i)));
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = bullet.transform.up * bulletSpeed;
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
