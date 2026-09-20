using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private float dayLengthInMinutes = 24f;
    [SerializeField, Range(0f, 24f)] private float currentTime = 8f;

    [Header("Light References")]
    [SerializeField] private Light sun;
    [SerializeField] private Light moon;

    [Header("Lighting")]
    [SerializeField] private Material proceduralSkybox;

    private AnimationCurve sunIntensityCurve;
    private AnimationCurve moonIntensityCurve;
    private AnimationCurve skyExposureCurve;

    //[Header("Environment")]

    [SerializeField] private Gradient fogColorGradient;

    [SerializeField] private AnimationCurve fogDensityCurve;

    [SerializeField] private AnimationCurve ambientIntensityCurve;

    [SerializeField] private AnimationCurve reflectionIntensityCurve;

    //[Header("Ambient Lighting")]
    [SerializeField] private Gradient ambientLightGradient;

    private float timeOfDayNormalized;
    private float environmentUpdateTimer;

    [Header("Day / Night Time")]
    [SerializeField, Range(0f, 24f)] private float dayStartTime = 6f;
    [SerializeField, Range(0f, 24f)] private float nightStartTime = 18f;

    public float CurrentTime => currentTime;

    public bool IsDay =>
        currentTime >= dayStartTime &&
        currentTime < nightStartTime;

    public bool IsNight => !IsDay;
    private void Awake()
    {
        CreateDefaultCurves();
    }

    private void Update()
    {
        UpdateTime();

        RotateCelestialBodies();

        UpdateLighting();

        UpdateSkybox();
    }

    private void UpdateTime()
    {
        float cycleLengthInSeconds = dayLengthInMinutes * 60f;

        currentTime += (24f / cycleLengthInSeconds) * Time.deltaTime;

        if (currentTime >= 24f)
        {
            currentTime = 0f;
        }

        timeOfDayNormalized = currentTime / 24f;
    }

    private void RotateCelestialBodies()
    {
        // Convert normalized time (0-1) to a full 360° rotation
        float sunRotation = timeOfDayNormalized * 360f;

        // Rotate the sun around the X-axis
        sun.transform.rotation = Quaternion.Euler(sunRotation - 90f, 170f, 0f);

        // Moon is always opposite the sun
        moon.transform.rotation = Quaternion.Euler(sunRotation + 90f, 170f, 0f);
    }
    private void UpdateLighting()
    {
        // Sun intensity
        if (sun != null)
        {
            sun.intensity =
                sunIntensityCurve.Evaluate(timeOfDayNormalized);
        }

        // Moon intensity
        if (moon != null)
        {
            moon.intensity =
                moonIntensityCurve.Evaluate(timeOfDayNormalized);
        }

        // Ambient lighting
        float ambientIntensity =
            ambientIntensityCurve.Evaluate(timeOfDayNormalized);

        Color ambientColor =
            ambientLightGradient.Evaluate(timeOfDayNormalized);

        RenderSettings.ambientLight =
            ambientColor * ambientIntensity;

        // Reflection intensity
        RenderSettings.reflectionIntensity =
            reflectionIntensityCurve.Evaluate(timeOfDayNormalized);

        // Update environment
        environmentUpdateTimer += Time.deltaTime;

        if (environmentUpdateTimer >= 0.5f)
        {
            DynamicGI.UpdateEnvironment();
            environmentUpdateTimer = 0f;
        }
    }

    private void UpdateSkybox()
    {
        if (proceduralSkybox == null)
            return;

        // Control Procedural Skybox exposure
        proceduralSkybox.SetFloat("_Exposure",
            skyExposureCurve.Evaluate(timeOfDayNormalized));
    }
    private void CreateDefaultCurves()
    {
        // Sun Intensity
        sunIntensityCurve = new AnimationCurve(
            new Keyframe(0.00f, 0f),
            new Keyframe(0.20f, 0f),
            new Keyframe(0.23f, 0.05f),
            new Keyframe(0.25f, 0.25f),
            new Keyframe(0.30f, 0.65f),
            new Keyframe(0.40f, 1.0f),
            new Keyframe(0.50f, 1.2f),
            new Keyframe(0.60f, 1.0f),
            new Keyframe(0.70f, 0.65f),
            new Keyframe(0.75f, 0.25f),
            new Keyframe(0.77f, 0.05f),
            new Keyframe(0.80f, 0f),
            new Keyframe(1.00f, 0f)
        );

        // Moon Intensity
        moonIntensityCurve = new AnimationCurve(
            new Keyframe(0.00f, 0.25f),
            new Keyframe(0.15f, 0.25f),
            new Keyframe(0.20f, 0.20f),
            new Keyframe(0.23f, 0.10f),
            new Keyframe(0.25f, 0.02f),

            // Day
            new Keyframe(0.30f, 0f),
            new Keyframe(0.70f, 0f),

            // Sunset / night
            new Keyframe(0.75f, 0.02f),
            new Keyframe(0.77f, 0.10f),
            new Keyframe(0.80f, 0.20f),
            new Keyframe(0.85f, 0.25f),
            new Keyframe(1.00f, 0.25f)
        );

        // Sky Exposure
        skyExposureCurve = new AnimationCurve(
            new Keyframe(0.00f, 0.22f), // Midnight
            new Keyframe(0.15f, 0.20f),
            new Keyframe(0.20f, 0.25f),

            // Dawn
            new Keyframe(0.23f, 0.40f),
            new Keyframe(0.25f, 0.60f),
            new Keyframe(0.30f, 0.90f),

            // Day
            new Keyframe(0.40f, 1.15f),
            new Keyframe(0.50f, 1.30f),
            new Keyframe(0.60f, 1.15f),

            // Sunset
            new Keyframe(0.70f, 0.90f),
            new Keyframe(0.75f, 0.60f),
            new Keyframe(0.77f, 0.40f),

            // Night
            new Keyframe(0.80f, 0.25f),
            new Keyframe(0.85f, 0.20f),
            new Keyframe(1.00f, 0.22f)
        );

        // Ambient Intensity
        ambientIntensityCurve = new AnimationCurve(
            new Keyframe(0.00f, 0.28f),
            new Keyframe(0.15f, 0.25f),
            new Keyframe(0.20f, 0.30f),

            new Keyframe(0.25f, 0.45f),
            new Keyframe(0.30f, 0.70f),

            new Keyframe(0.50f, 1.0f),

            new Keyframe(0.70f, 0.70f),
            new Keyframe(0.75f, 0.45f),

            new Keyframe(0.80f, 0.30f),
            new Keyframe(0.85f, 0.25f),
            new Keyframe(1.00f, 0.28f)
        );

        // Reflection Intensity
        reflectionIntensityCurve = new AnimationCurve(
            new Keyframe(0.00f, 0.30f),
            new Keyframe(0.20f, 0.35f),
            new Keyframe(0.25f, 0.50f),
            new Keyframe(0.30f, 0.75f),
            new Keyframe(0.50f, 1.0f),
            new Keyframe(0.70f, 0.75f),
            new Keyframe(0.75f, 0.50f),
            new Keyframe(0.80f, 0.35f),
            new Keyframe(1.00f, 0.30f)
        );

        // Fog Density
        fogDensityCurve = new AnimationCurve(
            new Keyframe(0.0f, 0.018f),
            new Keyframe(0.25f, 0.010f),
            new Keyframe(0.50f, 0.004f),
            new Keyframe(0.75f, 0.010f),
            new Keyframe(1.0f, 0.018f)
        );

        SmoothCurve(ambientIntensityCurve);
        SmoothCurve(reflectionIntensityCurve);
        SmoothCurve(fogDensityCurve);

        SmoothCurve(sunIntensityCurve);
        SmoothCurve(moonIntensityCurve);
        SmoothCurve(skyExposureCurve);
    }
    private void SmoothCurve(AnimationCurve curve)
    {
        for (int i = 0; i < curve.length; i++)
        {
            curve.SmoothTangents(i, 0f);
        }
    }
}