using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections;

public class NetworkUI : NetworkBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private GameObject timerGameObject;
    [SerializeField] private GameObject countdownGameObject;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject leaderboardPlayerPrefab;
    [SerializeField] private Transform leaderboardParent;
    
    private NetworkVariable<bool> countdownFinished = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    private NetworkVariable<bool> isSongPlaying = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> remainingTime = new NetworkVariable<float>(420f, NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> countdownTime = new NetworkVariable<float>(10f, NetworkVariableReadPermission.Everyone);

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
        if (IsServer)
        {
            if (!countdownFinished.Value && !GameManager.instance.canPlay)
            {
                CountdownServerRPC();
                timerGameObject.SetActive(false);
            }
            else if(countdownFinished.Value)
            {
                // GameManager.instance.canPlay = true;
                // timerGameObject.SetActive(true);
                // Time.timeScale = 1;
                // TimerServerRPC();

                CountdownFinishedClientRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TimerServerRPC()
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

    [ServerRpc(RequireOwnership = false)]
    public void CountdownServerRPC()
    {
        if (countdownTime.Value > 0)
        {
            timerGameObject.SetActive(false);
            countdownTime.Value -= Time.deltaTime;
            UpdateCountdownTimerTextClientRPC(countdownTime.Value);
        }

        if (countdownTime.Value <= 0 && !countdownFinished.Value)
        {
            countdownTime.Value = 0;
            countdownFinished.Value = true;
            CountdownFinishedClientRpc();
        }
    }

    [ClientRpc]
    private void CountdownFinishedClientRpc()
    {
        if(isSongPlaying.Value)
        {
            AudioManager.instance.PlayMusic("Game Theme");
            isSongPlaying.Value = false;
        }
        GameManager.instance.canPlay = true;
        countdownGameObject.SetActive(false);
        timerGameObject.SetActive(true);
        TimerServerRPC();
    }

    [ClientRpc]
    private void UpdateTimerTextClientRPC(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    [ClientRpc]
    private void UpdateCountdownTimerTextClientRPC(float time)
    {
        int seconds = Mathf.FloorToInt(time % 60);
        countdownText.text = string.Format("{0:00}", seconds);
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
