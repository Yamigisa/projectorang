using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    public int health = 1;
    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(true);
    public NetworkVariable<int> score = new NetworkVariable<int>();

    private PlayerMovement playerMovement;
    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();    
        SetRendererActive(isActive.Value);
    }

    private void Update()
    {
        SetRendererActive(isActive.Value);
    }
    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.instance.ShakeCameraClientRpc(GetComponent<NetworkObject>().NetworkObjectId);
        AddScore(false,100);
        isActive.Value = false;
        StartCoroutine(GameManager.instance.RespawnCoroutine(this));
    }

    public void UpdatePosition()
    {
        playerMovement.UpdatePositionServerRPC(GameManager.instance.GetSpawnPoint((int)OwnerClientId).position, GameManager.instance.GetSpawnPoint((int)OwnerClientId).rotation );
    }
    private void OnEnable()
    {
        isActive.OnValueChanged += OnIsActiveChanged;
    }

    private void OnDisable()
    {
        isActive.OnValueChanged -= OnIsActiveChanged;
    }
    
    private void OnIsActiveChanged(bool previousValue, bool newValue)
    {
        SetRendererActive(newValue);
    }

    private void SetRendererActive(bool isActive)
    {
        var rb = playerMovement.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            var spriteRenderer = rb.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = isActive;
            }
        }
    }
    public void AddScore(bool add, int amount)
    {
        if (!IsServer) return;

        Debug.Log("Addscore");
        if(add)
        score.Value += amount;
        else
        score.Value -= amount;
    }

    public int GetScore()
    {
        return score.Value;
    }
}
