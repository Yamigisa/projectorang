using UnityEngine;
using System.Collections;

public class ObstacleGlowEffect : MonoBehaviour
{
    private Material obstacleMaterial;

    [ColorUsage(true, true)] // Menyatakan bahwa ini adalah HDR color
    public Color glowColor = Color.white;  // Warna dasar HDR untuk efek glow
    public float glowMultiplier = 5f;  // Intensitas glow HDR saat terkena peluru
    private Color originalEmissionColor;  // Warna emission asli
    public float glowDuration = 0.5f; // Durasi glow aktif

    private void Start()
    {
        obstacleMaterial = GetComponent<Renderer>().material;

        // Simpan warna emission asli dari material obstacle
        if (obstacleMaterial.HasProperty("_Color"))
        {
            originalEmissionColor = obstacleMaterial.GetColor("_Color");
        }
    }

    public void TriggerGlow()
    {
        if (obstacleMaterial.HasProperty("_Color"))
        {
            StopAllCoroutines(); // Hentikan efek glow sebelumnya jika ada
            StartCoroutine(GlowCoroutine());
        }
    }

    private IEnumerator GlowCoroutine()
    {
        // Tingkatkan intensitas HDR glow
        Color boostedColor = glowColor * glowMultiplier;
        obstacleMaterial.SetColor("_Color", boostedColor);

        // Tunggu sesuai durasi glow
        yield return new WaitForSeconds(glowDuration);

        // Kembalikan ke warna emission awal
        obstacleMaterial.SetColor("_Color", originalEmissionColor);
    }
}
