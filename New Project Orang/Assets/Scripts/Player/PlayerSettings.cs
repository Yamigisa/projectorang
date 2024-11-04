using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class PlayerSettings : NetworkBehaviour
{
    [SerializeField] private GameObject playerHUD;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text playerScore;
    public NetworkVariable<FixedString128Bytes> networkPlayerName = new NetworkVariable<FixedString128Bytes>("Player 0", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private SpriteRenderer spriteRenderer;
    
    public List<Color> color = new List<Color>();

    public PlayerStats playerStats;


    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        playerScore.text = playerStats.GetScore().ToString();

        if(!playerStats.isActive.Value)
        playerHUD.SetActive(false);
        else
        playerHUD.SetActive(true);
    }
    public override void OnNetworkSpawn()
    {
        networkPlayerName.Value = "Player: " + (OwnerClientId + 1); 
        playerName.text = networkPlayerName.Value.ToString();
        spriteRenderer.color = color[(int)OwnerClientId];
    }

    public void GetScore()
    {
        playerStats.GetScore();
    }
}
