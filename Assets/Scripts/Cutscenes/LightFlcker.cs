using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    [Tooltip("Base intensity of the light (average brightness).")]
    public float baseIntensity = 2f;

    [Tooltip("Maximum random variation added/subtracted from base intensity.")]
    public float intensityVariation = 0.5f;

    [Tooltip("How fast the light flickers.")]
    public float flickerSpeed = 5f;

    [Header("Optional Range Flicker")]
    public bool flickerRange = false;
    public float baseRange = 10f;
    public float rangeVariation = 1f;

    [Header("Optional Color Flicker")]
    public bool flickerColor = false;
    public Color warmColor = new Color(1f, 0.56f, 0.25f);
    public Color coolColor = new Color(1f, 0.3f, 0.05f);
    public float colorBlendSpeed = 2f;

    private Light fireLight;
    private float randomOffset;

    void Start()
    {
        fireLight = GetComponent<Light>();
        randomOffset = Random.Range(0f, 100f); // desync multiple fires
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, randomOffset);

        // Smooth random intensity around base
        float targetIntensity = baseIntensity + (noise - 0.5f) * 2f * intensityVariation;
        fireLight.intensity = Mathf.Lerp(fireLight.intensity, targetIntensity, Time.deltaTime * flickerSpeed);

        // Optional: flicker range
        if (flickerRange)
        {
            float targetRange = baseRange + (noise - 0.5f) * 2f * rangeVariation;
            fireLight.range = Mathf.Lerp(fireLight.range, targetRange, Time.deltaTime * flickerSpeed);
        }

        // Optional: subtle warm/cool flicker
        if (flickerColor)
        {
            Color targetColor = Color.Lerp(warmColor, coolColor, noise);
            fireLight.color = Color.Lerp(fireLight.color, targetColor, Time.deltaTime * colorBlendSpeed);
        }
    }
}
