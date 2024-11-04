using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    void Start()
    {
        instance = this;

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

    public void Respawn(PlayerStats playerStats)
    {
        Debug.Log("Respawn");
        if(!IsServer) return;
    }
    public IEnumerator RespawnCoroutine(PlayerStats playerStats)
    {
        Debug.Log("started coroutine respawn");
        yield return new WaitForSeconds(2f); 

        Debug.Log("Couroutine respanw finished");
        playerStats.health = 1; 
        playerStats.isActive.Value = true;

        playerStats.gameObject.SetActive(true);
        playerStats.UpdatePosition();
    }
}
