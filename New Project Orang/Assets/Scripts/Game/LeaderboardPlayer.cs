using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LeaderboardPlayer : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerScoreText;

    public void SetPlayerInfo(string playerName, int playerScore)
    {
        Debug.Log("SetPlayerInfo");
        playerNameText.text = playerName;
        playerScoreText.text = playerScore.ToString();
    }
}
