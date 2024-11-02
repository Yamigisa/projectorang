using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;
public class PlayerSettings : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerName;

    private NetworkVariable<FixedString128Bytes> networkPlayerName = new NetworkVariable<FixedString128Bytes>("Player 0", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private SpriteRenderer spriteRenderer;
    
    public List<Color> color = new List<Color>();

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    public override void OnNetworkSpawn()
    {
        networkPlayerName.Value = "Player: " + (OwnerClientId + 1); 
        playerName.text = networkPlayerName.Value.ToString();
        spriteRenderer.color = color[(int)OwnerClientId];
    }
}
