using UnityEngine;

public class DamageOverlayController : MonoBehaviour
{
    public static DamageOverlayController Instance;

    [Header("Referencia Vignette")]
    public Renderer vignetteRenderer;

    [Header("Daño Instantáneo")]
    public float damageFlashIntensity = 0.7f;
    public float fadeSpeed = 2f;

    [Header("Poca Vida")]
    public float lowHealthMaxIntensity = 0.5f;
    public float lowHealthThreshold = 0.3f;

    private Material vignetteMaterial;

    private float currentDamageIntensity = 0f;

    private void Awake()
    {
        Instance = this;

        if (vignetteRenderer != null)
        {
            vignetteMaterial = vignetteRenderer.material;
        }
    }

    private void Update()
    {
        if (vignetteMaterial == null || PlayerController.Instance == null)
            return;

        // Fade del daño
        currentDamageIntensity = Mathf.MoveTowards(
            currentDamageIntensity,
            0f,
            fadeSpeed * Time.deltaTime
        );

        // Vida normalizada
        float healthPercent =
            PlayerController.Instance.health /
            PlayerController.Instance.maxHealth;

        // Intensidad por low health
        float lowHealthIntensity = 0f;

        if (healthPercent < lowHealthThreshold)
        {
            float pulse =
            (Mathf.Sin(Time.time * 4f) + 1f) * 0.5f;

                    float baseIntensity =
                        Mathf.Lerp(
                            lowHealthMaxIntensity,
                            0f,
                            healthPercent / lowHealthThreshold
                        );

                    lowHealthIntensity =
                        Mathf.Lerp(baseIntensity * 0.7f, baseIntensity, pulse);
        }

        // Intensidad final
        float finalIntensity =
            Mathf.Clamp01(lowHealthIntensity + currentDamageIntensity);

        vignetteMaterial.SetColor("_VignetteColor", Color.red);

        vignetteMaterial.SetFloat(
            "_ApertureSize",
            Mathf.Lerp(1f, 0.6f, finalIntensity)
        );
    }

    public void ShowDamage()
    {
        currentDamageIntensity = damageFlashIntensity;
    }

    public void OnPlayerDeath()
    {
        currentDamageIntensity = 0f;

        if (vignetteMaterial != null)
        {
            vignetteMaterial.SetFloat("_ApertureSize", 1f);

            vignetteMaterial.SetColor(
                "_VignetteColor",
                new Color(1f, 0f, 0f, 0f)
            );
        }
    }
}