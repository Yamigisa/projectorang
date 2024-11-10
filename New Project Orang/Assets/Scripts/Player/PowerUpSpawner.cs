using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PowerUpSpawner : NetworkBehaviour
{
    public GameObject[] powerUpPrefabs;
    public float spawnInterval = 3f;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            StartCoroutine(SpawnPowerUp());
        }
    }

    private IEnumerator SpawnPowerUp()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            Vector2 spawnPosition = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            int randomPowerUp = Random.Range(0, powerUpPrefabs.Length);

            SpawnPowerUpServerRpc(spawnPosition, randomPowerUp);
        }
    }

    [ServerRpc]
    private void SpawnPowerUpServerRpc(Vector2 spawnPosition, int powerUpIndex)
    {
        if (powerUpIndex < 0 || powerUpIndex >= powerUpPrefabs.Length) return;

        GameObject powerUpInstance = Instantiate(powerUpPrefabs[powerUpIndex], spawnPosition, Quaternion.identity);
        
        NetworkObject networkObject = powerUpInstance.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
        }
    }
}
