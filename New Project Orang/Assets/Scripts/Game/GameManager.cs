using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    public Camera mainCamera; 
    public bool canPlay;

    public Transform[] spawnPoints;

    void Start()
    {
        instance = this;

        canPlay = false;
        
        mainCamera = Camera.main;

        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        if(RelayManager.instance.isHost)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionApproval;
            (byte[] allocationId, byte[] key, byte[] connectionData, string ip, int port) = RelayManager.instance.GetHostConnectionInfo();
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(ip, (ushort)port, allocationId, key, connectionData, true);
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            (byte[] allocationId, byte[] key, byte[] connectionData, byte[] hostConnectionData, string ip, int port) = RelayManager.instance.GetClientConnectionInfo();
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(ip, (ushort)port, allocationId, key, connectionData, hostConnectionData, true);
            NetworkManager.Singleton.StartClient();
        }
    }

    private void ConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;   
        response.CreatePlayerObject = true;
        response.Pending = false;
    }

    public Transform GetSpawnPoint(int playerId)
    {   
        return spawnPoints[playerId % spawnPoints.Length];
    }

    public IEnumerator RespawnCoroutine(PlayerStats playerStats)
    {
        yield return new WaitForSeconds(2f); 

        playerStats.health = 1; 
        playerStats.isActive.Value = true;

        playerStats.gameObject.SetActive(true);
        playerStats.UpdatePosition();
    }

    [ClientRpc]
    public void ShakeCameraClientRpc(ulong networkObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId).NetworkObjectId == networkObjectId)
        {
            StartCoroutine(CameraShakeCoroutine());
        }
    }

    private IEnumerator CameraShakeCoroutine()
    {
        Vector3 originalPosition = mainCamera.transform.position;
        float shakeAmount = 0.1f; 
        float shakeDuration = 0.5f;

        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeAmount;
            mainCamera.transform.position = originalPosition + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalPosition;
    }
    
    public void BackToMainMenu()
    {
        NetworkManager.Singleton.Shutdown();

        SceneManager.LoadScene("Main Menu");
    }
}
