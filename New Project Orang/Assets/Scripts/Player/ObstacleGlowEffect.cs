using UnityEngine;
using System.Collections;

public class ObstacleGlowEffect : MonoBehaviour
{
    private Material obstacleMaterial;

    [ColorUsage(true, true)]
    public Color glowColor = Color.white;
    public float glowMultiplier = 5f;
    private Color originalEmissionColor;
    public float glowDuration = 0.5f;

    private void Start()
    {
        obstacleMaterial = GetComponent<Renderer>().material;

        if (obstacleMaterial.HasProperty("_Color"))
        {
            originalEmissionColor = obstacleMaterial.GetColor("_Color");
        }
    }

    public void TriggerGlow()
    {
        if (obstacleMaterial.HasProperty("_Color"))
        {
            StopAllCoroutines();
            StartCoroutine(GlowCoroutine());
        }
    }

    private IEnumerator GlowCoroutine()
    {
        Color boostedColor = glowColor * glowMultiplier;
        obstacleMaterial.SetColor("_Color", boostedColor);

        yield return new WaitForSeconds(glowDuration);

        obstacleMaterial.SetColor("_Color", originalEmissionColor);
    }
}
