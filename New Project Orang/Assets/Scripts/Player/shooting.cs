using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class Shooting : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    private bool shotgunActive = false;
    private bool burstFireActive = false;

    private int burstFireShots = 0;
    private float powerUpDuration = 60f;

    private PlayerStats playerStats;

    private int shotsFired = 0;
    private float cooldownTime = 10f;
    public float cooldownTimer = 0f;
    private bool isCooldown = false;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (IsOwner)
        {
            if(GameManager.instance.canPlay)
            {
                if (isCooldown)
                {
                    cooldownTimer -= Time.deltaTime;
                    if (cooldownTimer <= 0f)
                    {
                        isCooldown = false;
                        shotsFired = 0;
                    }
                }
                if (!playerStats.isActive.Value)
            {
                if (shotgunActive)
                {
                    DeactivePowerUp(PowerUp.PowerUpType.Shotgun);
                }
                if (burstFireActive)
                {
                    DeactivePowerUp(PowerUp.PowerUpType.BurstFire);
                }
            }

                if (playerStats.isActive.Value && !isCooldown)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        AudioManager.instance.PlaySFX("Shoot");
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

                        // Tambahkan jumlah tembakan dan cek apakah sudah mencapai batas
                        shotsFired++;
                        if (shotsFired >= 6)
                        {
                            isCooldown = true;
                            cooldownTimer = cooldownTime; // Mulai cooldown 10 detik
                        }
                    }
                }
            }
        }
    }

    void Shoot()
    {
        if (IsServer)
        {
            AudioManager.instance.PlaySFX("Shoot");
            SpawnBullet(firePoint.position, firePoint.rotation);
        }
        else
        {
            AudioManager.instance.PlaySFX("Shoot");
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
        AudioManager.instance.PlaySFX("Shoot");
        SpawnBullet(position, rotation);
    }

    void SpawnBullet(Vector3 position, Quaternion rotation)
    {
        AudioManager.instance.PlaySFX("Shoot");
        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = bullet.transform.up * bulletSpeed;

        bullet.GetComponent<NetworkObject>().Spawn();
    }

    public void ActivatePowerUp(PowerUp.PowerUpType powerUpType)
    {
        ResetActivePowerUps();
        shotsFired = 0;
        cooldownTimer = 0f;
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

    public void DeactivePowerUp(PowerUp.PowerUpType powerUpType){
        if (!playerStats.isActive.Value)
    {
        Debug.LogWarning("Power mati");
        if (powerUpType == PowerUp.PowerUpType.Shotgun)
        {
            shotgunActive = false;
        }
        else if (powerUpType == PowerUp.PowerUpType.BurstFire)
        {
            burstFireActive = false;
            burstFireShots = 0;
        }
    }
    
    }
    private void ResetActivePowerUps()
{
    // Nonaktifkan semua power-up
    shotgunActive = false;
    burstFireActive = false;
    burstFireShots = 0;
}

    private IEnumerator ResetPowerUpAfterDuration(System.Action onComplete)
    {
        yield return new WaitForSeconds(powerUpDuration);
        onComplete.Invoke();
    }
}
