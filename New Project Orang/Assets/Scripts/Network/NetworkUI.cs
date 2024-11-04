using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections;

public class NetworkUI : NetworkBehaviour
{
    [SerializeField] private TMP_Text playersCountText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject leaderboardPlayerPrefab; 
    [SerializeField] private Transform leaderboardParent; 

    private NetworkVariable<float> remainingTime = new NetworkVariable<float>(20f, NetworkVariableReadPermission.Everyone);
    private NetworkVariable<int> playersNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    // Struct to store player info
    public struct PlayerInfo : INetworkSerializable
    {
        public FixedString128Bytes PlayerName;
        public int Score;

        public PlayerInfo(FixedString128Bytes playerName, int score)
        {
            PlayerName = playerName;
            Score = score;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref Score);
        }
    }

    private void Update()
    {
        playersCountText.text = "Players: " + playersNum.Value.ToString();
        
        if (IsServer)
        {
            playersNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
            CountdownServerRPC(); 
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CountdownServerRPC()
    {
        if (remainingTime.Value > 0)
        {
            remainingTime.Value -= Time.deltaTime;
            UpdateTimerTextClientRPC(remainingTime.Value);
        }

        if (remainingTime.Value <= 0)
        {
            remainingTime.Value = 0;
            HandleGameOverClientRPC();
        }
    }

    [ClientRpc]
    private void UpdateTimerTextClientRPC(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    [ClientRpc]
    private void HandleGameOverClientRPC()
    {
        if (gameOverPanel != null)
        {
            Time.timeScale = 0;
            gameOverPanel.SetActive(true);
        }
        
        if (IsServer)
        {
            ShowLeaderboard();
        }
    }

    // Gather and send leaderboard data to clients
    private void ShowLeaderboard()
    {
        List<PlayerInfo> leaderboardData = new List<PlayerInfo>();

        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (client.Value.PlayerObject != null)
            {
                PlayerSettings playerSettings = client.Value.PlayerObject.GetComponent<PlayerSettings>();
                if (playerSettings != null)
                {
                    leaderboardData.Add(new PlayerInfo(playerSettings.networkPlayerName.Value, playerSettings.playerStats.score.Value));
                }
            }
        }

        ShowLeaderboardClientRpc(leaderboardData.ToArray());
    }

    [ClientRpc]
    private void ShowLeaderboardClientRpc(PlayerInfo[] leaderboardData)
    {
        foreach (Transform child in leaderboardParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var playerData in leaderboardData)
        {
            GameObject leaderboardPlayerEntry = Instantiate(leaderboardPlayerPrefab, leaderboardParent);
            LeaderboardPlayer leaderboardPlayer = leaderboardPlayerEntry.GetComponent<LeaderboardPlayer>();
            leaderboardPlayer.SetPlayerInfo(playerData.PlayerName.ToString(), playerData.Score);
        }
    }
}
