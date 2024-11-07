using System.Collections;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject[] powerUpPrefabs; // Array untuk tipe power-up
    public float spawnInterval = 60f; // Waktu spawn ulang (60 detik)

    private void Start()
    {
        StartCoroutine(SpawnPowerUp());
    }

    private IEnumerator SpawnPowerUp()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            Vector2 spawnPosition = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            int randomPowerUp = Random.Range(0, powerUpPrefabs.Length);
            Instantiate(powerUpPrefabs[randomPowerUp], spawnPosition, Quaternion.identity);
        }
    }
}
