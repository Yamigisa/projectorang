using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    public int health = 1; // Set to 1 for one-shot kill

    // Networked variable to synchronize active state
    private NetworkVariable<bool> isActive = new NetworkVariable<bool>(true);

    private void Start()
    {
        // Apply the initial active state to match the NetworkVariable
        gameObject.SetActive(isActive.Value);
    }

    private void Update()
    {
        // Continuously apply the active state from the NetworkVariable
        gameObject.SetActive(isActive.Value);
    }

    // Function to receive damage
    public void TakeDamage(int damage)
    {
        if (!IsServer) return; // Only the server should modify health

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isActive.Value = false; // Update the NetworkVariable to make the player inactive across clients
    }

    private void OnEnable()
    {
        // Register to be notified of NetworkVariable changes
        isActive.OnValueChanged += OnIsActiveChanged;
    }

    private void OnDisable()
    {
        // Unregister from the NetworkVariable changes
        isActive.OnValueChanged -= OnIsActiveChanged;
    }

    // Method triggered when isActive value changes
    private void OnIsActiveChanged(bool previousValue, bool newValue)
    {
        gameObject.SetActive(newValue); // Set active state according to the NetworkVariable
    }
}
