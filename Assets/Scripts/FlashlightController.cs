using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Transform mainCamera; // Посилання на камеру гравця (приціл)
    public float offsetAngle = 30f; // Кут зміщення для регулювання положення світла
    private Light flashlight;
    public AudioSource flashlightSwitchingSound;
    public InventoryManager inventoryManager;

    // Значення range, які ви хочете використовувати
    private float[] rangeValues = { 20f, 50f, 80f };
    private int currentRangeIndex = 1; // Поточний індекс значення range

    void Start()
    {
        flashlight = GetComponentInChildren<Light>();
    }

    void Update()
    {
        // Включаємо/виключаємо світло при натисканні лівої кнопки миші
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (inventoryManager != null && !inventoryManager.isOpened)
            {
                ToggleFlashlight();
                flashlightSwitchingSound.Play();
            }
        }

        // Переключення значення range при натисканні середньої кнопки миші
        if (flashlight != null && Input.GetKeyDown(KeyCode.Mouse2))
        {
            if (inventoryManager != null && !inventoryManager.isOpened)
            {
                SwitchRange();
            }
        }
    }

    void ToggleFlashlight()
    {
        if (flashlight != null)
        {
            flashlight.enabled = !flashlight.enabled;

            if (flashlight.enabled)
            {
                // Отримуємо напрямок лінії через середину половини екрану
                Ray desiredTargetRay = mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector2(Screen.width / 4, Screen.height / 2));

                // Отримуємо кут орієнтації для світла
                Quaternion desiredRotation = Quaternion.LookRotation(desiredTargetRay.direction);

                // Застосовуємо зміщення кута
                desiredRotation *= Quaternion.Euler(Vector3.up * offsetAngle);

                // Орієнтуємо світло в напрямку лінії з врахуванням кута
                flashlight.transform.rotation = desiredRotation;
            }
        }
    }

    void SwitchRange()
    {
        if (flashlight != null)
        {
            currentRangeIndex = (currentRangeIndex + 1) % rangeValues.Length; // Зміна індексу в межах довжини масиву
            flashlight.range = rangeValues[currentRangeIndex];
        }
    }
}
