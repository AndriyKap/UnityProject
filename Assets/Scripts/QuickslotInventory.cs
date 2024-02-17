using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickslotInventory : MonoBehaviour
{
    // Об'єкт, у якого діти є слотами
    public Transform quickslotParent;
    public InventoryManager inventoryManager;
    public int currentQuickslotID = 0;
    public Sprite selectedSprite;
    public Sprite notSelectedSprite;
    public TMP_Text healthText;
    public InventorySlot activeSlot = null;
    public Transform allWeapons;
    public Indicators indicators;

    // Update is called once per frame
    void Update()
    {
        if(!inventoryManager.isOpened)
        {
            float mw = Input.GetAxis("Mouse ScrollWheel");
            // Використовуємо коліщатко мишки
            if (mw > 0.1)
            {
                // Беремо попередній слот і змінюємо його картинку на звичайну
                quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;
                // Якщо крутимо коліщатком мишки вперед і наше число currentQuickslotID дорівнює останньому слоту, то вибираємо наш перший слот (перший слот вважається нульовим)
                if (currentQuickslotID >= quickslotParent.childCount-1)
                {
                    currentQuickslotID = 0;
                }
                else
                {
                    // Додаємо до currentQuickslotID одиничку
                    currentQuickslotID++;
                }
                // Беремо попередній слот і змінюємо його картинку на "обрану"
                quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;
                activeSlot = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>();
                ShowItemInHand();

            }
            if (mw < -0.1)
            {
                // Беремо попередній слот і змінюємо його картинку на звичайну
                quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;
                // Якщо крутимо коліщатком мишки назад і наше число currentQuickslotID дорівнює 0, то вибираємо наш останній слот
                if (currentQuickslotID <= 0)
                {
                    currentQuickslotID = quickslotParent.childCount-1;
                }
                else
                {
                    // Зменшуємо число currentQuickslotID на 1
                    currentQuickslotID--;
                }
                // Беремо попередній слот і змінюємо його картинку на "обрану"
                quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;
                activeSlot = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>();
                ShowItemInHand();
                
            }
            // Використовуємо цифри
            for(int i = 0; i < quickslotParent.childCount; i++)
            {
                // якщо ми натискаємо на клавіші 1 по 5, то...
                if (Input.GetKeyDown((i + 1).ToString())) {
                    // перевіряємо, якщо наш обраний слот дорівнює слоту, який у нас вже обраний, то
                    if (currentQuickslotID == i)
                    {
                        // Ставимо картинку "selected" на слот, якщо він "not selected" або навпаки
                        if (quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite == notSelectedSprite)
                        {
                            quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;
                            activeSlot = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>();
                            ShowItemInHand();
                        }
                        else
                        {
                            quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;
                            activeSlot = null;
                            HideItemInHand();
                        }
                    }
                    // Інакше ми прибираємо світіння з попереднього слота і світимо слот, який ми вибираємо
                    else
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;
                        currentQuickslotID = i;
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;
                        activeSlot = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>();
                        ShowItemInHand();
                    }
                }
            }
            // Використовуємо предмет натискання на ліву кнопку миші
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().item != null)
                {
                    if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().item.isConsumeable && !inventoryManager.isOpened && quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite == selectedSprite)
                    {
                        // Застосовуємо зміни до здоров'я (майбутньому до голоду та спраги)
                        ChangeCharacteristics();

                        if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount <= 1)
                        {
                            quickslotParent.GetChild(currentQuickslotID).GetComponentInChildren<DragAndDropItem>().NullifySlotData();
                        }
                        else
                        {
                            quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount--;
                            quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().itemAmountText.text = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount.ToString();
                        }
                    }
                }
            }
        }
        else
        {
            foreach (Transform slot in quickslotParent)
            {
                // Змінюємо зображення на невибране
                slot.GetComponent<Image>().sprite = notSelectedSprite;
                // Очищуємо активний слот
                activeSlot = null;
            }
        }
    }

    public void CheckItemInHand()
    {
        if(activeSlot != null)
        {
            ShowItemInHand();
        }
        else
        {
            HideItemInHand();
        }
    }

    private void ChangeCharacteristics()
    {
        indicators.ChangeFoodAmount(quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().item.changeHunger);
        indicators.ChangeWaterAmount(quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().item.changeThirst);
        indicators.ChangeHealthAmount(quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().item.changeHealth);
    }

    private void ShowItemInHand()
    {
        HideItemInHand();
        if(activeSlot.item == null)
        {
            return;
        }

        for(int i = 0; i < allWeapons.childCount; i++)
        {
            if(activeSlot.item.inHandName == allWeapons.GetChild(i).name)
            {
                allWeapons.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    private void HideItemInHand()
    {
        for(int i = 0; i < allWeapons.childCount; i++)
        {
            allWeapons.GetChild(i).gameObject.SetActive(false);
        }
    }
}
