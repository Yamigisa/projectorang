using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 1; // Health pemain, set ke 1 untuk mati dengan satu peluru

    // Fungsi untuk menerima damage
    public void TakeDamage(int damage)
    {
        health -= damage; // Kurangi health sesuai damage yang diterima

        if (health <= 0)
        {
            Die(); // Panggil fungsi Die jika health habis
        }
    }

    // Fungsi kematian pemain
    void Die()
    {
        Debug.Log("Player has died!");
        // Kamu bisa menambahkan efek mati di sini, seperti animasi, menonaktifkan objek, dsb.
        gameObject.SetActive(false); // Nonaktifkan pemain
    }
}
