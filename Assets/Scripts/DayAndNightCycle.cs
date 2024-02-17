using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class DayAndNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [Range(0f, 24f)]
    public float currentTime;
    public float timeSpeed = 1f;

    [Header("CurrentTime")]
    public string currentTimeString;

    [Header("Light Settings")]
    public Light sunLight;
    public float sunPosition = 1f;

    // Start is called before the first frame update
    void Start()
    {
        UpdateTimeText();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime * timeSpeed;

        if(currentTime >= 24)
        {
            currentTime = 0;
        }
        
        UpdateTimeText();
        UpdateLight();
    }

    private void OnValidate()
    {
        UpdateLight();    
    }

    void UpdateTimeText()
    {
        currentTimeString = Mathf.Floor(currentTime).ToString("00") + ":" + ((currentTime % 1) * 60).ToString("00");
    }

    void UpdateLight()
    {
        float sunRotation = currentTime / 24f * 360f;
        sunLight.transform.rotation = Quaternion.Euler(sunRotation - 90f, sunPosition, 0f);
    }
}
