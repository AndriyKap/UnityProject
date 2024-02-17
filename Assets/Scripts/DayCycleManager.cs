using System.Collections;
using UnityEngine;

public class DayCycleManager : MonoBehaviour
{
    [Range(0, 1)]
    public float TimeOfDay;
    public float DayDuration = 600f;

    public AnimationCurve SunCurve;
    public AnimationCurve MoonCurve;
    public AnimationCurve SkyboxCurve;

    public Material DaySkybox;
    public Material NightSkybox;

    public ParticleSystem Stars;

    public Light Sun;
    public Light Moon;

    private float sunIntensity;
    private float moonIntensity;

    private void Start()
    {
        sunIntensity = Sun.intensity;
        moonIntensity = Moon.intensity;
    }

    private void Update()
    {
        TimeOfDay += Time.deltaTime / DayDuration;
        TimeOfDay = Mathf.Repeat(TimeOfDay, 1f); 

        UpdateSkybox();
        UpdateStarsColor();
        UpdateSunAndMoonRotation();
        UpdateSunAndMoonIntensity();
    }

    private void UpdateSkybox()
    {
        RenderSettings.skybox.Lerp(NightSkybox, DaySkybox, SkyboxCurve.Evaluate(TimeOfDay));
        RenderSettings.sun = SkyboxCurve.Evaluate(TimeOfDay) > 0.1 ? Sun : Moon;
        DynamicGI.UpdateEnvironment();
    }

    private void UpdateStarsColor()
    {
        var mainModule = Stars.main;
        mainModule.startColor = new Color(1f, 1f, 1f, 1f - SkyboxCurve.Evaluate(TimeOfDay));
    }

    private void UpdateSunAndMoonRotation()
    {
        Sun.transform.localRotation = Quaternion.Euler(TimeOfDay * 360f, 180f, 0f);
        Moon.transform.localRotation = Quaternion.Euler(TimeOfDay * 360f + 180f, 180f, 0f);
    }

    private void UpdateSunAndMoonIntensity()
    {
        Sun.intensity = sunIntensity * SunCurve.Evaluate(TimeOfDay);
        Moon.intensity = moonIntensity * MoonCurve.Evaluate(TimeOfDay);
    }
}
