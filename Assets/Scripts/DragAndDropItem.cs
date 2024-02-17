using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
/// IPointerDownHandler - Слідкує за натисканнями мишки по об'єкту на якому висить цей скрипт
/// IPointerUpHandler - Слідкує за відпусканням мишки по об'єкту на якому висить цей скрипт
/// IDragHandler - Слідкує за тим, чи не вводимо ми натиснуту мишку по об'єкту
public class DragAndDropItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public InventorySlot oldSlot;
    private Transform player;
    private QuickslotInventory quickslotInventory; // added this++
    public List<ClotheAdder> clothAdders;
 
    private void Start()
    {
        quickslotInventory = FindObjectOfType<QuickslotInventory>();
        //ПОСТАВИТИ ТЕГ "PLAYER" НА ОБ'ЄКТ ПЕРСОНАЖУ!
        player = GameObject.FindObjectOfType<CustomCharacterController>().transform;
        // Знаходимо скрипт InventorySlot у слоті в ієрархії
        oldSlot = transform.GetComponentInParent<InventorySlot>();

        if(oldSlot.clothType != ClothType.None)
        {
            clothAdders = new List<ClotheAdder>();
            clothAdders.AddRange(FindObjectsOfType<ClotheAdder>());
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        // Якщо слот порожній, то ми не виконуємо те, що нижче return;
        if (oldSlot.isEmpty)
            return;
        GetComponent<RectTransform>().position += new Vector3(eventData.delta.x, eventData.delta.y);
    }
 
    public void OnPointerDown(PointerEventData eventData)
    {
        if (oldSlot.isEmpty)
            return;
        //Робимо картинку прозоріше
        GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.75f);
        // Робимо так, щоб натискання мишкою не ігнорували цю картинку
        GetComponentInChildren<Image>().raycastTarget = false;
        // Робимо наш DraggableObject дитиною InventoryPanel, щоб DraggableObject був над іншими слотами інвентору
        transform.SetParent(transform.parent.parent.parent);
    }
 
    public void OnPointerUp(PointerEventData eventData)
    {
        if (oldSlot.isEmpty)
            return;
        // Робимо картинку знову не прозорою
        GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1f);
        // І щоб мишка знову могла її засікти
        GetComponentInChildren<Image>().raycastTarget = true;
 
        //Поставити DraggableObject назад у свій старий слот
        transform.SetParent(oldSlot.transform);
        transform.position = oldSlot.transform.position;
        //Якщо мишка відпущена над об'єктом на ім'я UIPanel, то...
        if (eventData.pointerCurrentRaycast.gameObject.name == "UIBG") // renamed to UIBG
        {         
            if(oldSlot.clothType != ClothType.None && oldSlot.item != null)
            {
                foreach(ClotheAdder clothAdder in clothAdders)
                {
                    clothAdder.RemoveClothes(oldSlot.item.clothingPrefab);
                    oldSlot.clothingImage.enabled = true;
                }
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                // Викид об'єктів із інвентарю - Спавнимо префаб об'єкта перед персонажем
                float spawnHeight = 0.1f; // Adjust this value based on the height at which you want the item to spawn
                float spawnDistance = 1.0f;
                Vector3 spawnPosition = player.position + player.forward * spawnDistance + new Vector3(0, spawnHeight, 0);
                Quaternion spawnRotation = player.rotation;
                GameObject itemObject = Instantiate(oldSlot.item.itemPrefab, spawnPosition, spawnRotation);
                itemObject.GetComponent<Item>().amount = Mathf.CeilToInt((float)oldSlot.amount / 2);
                oldSlot.amount -= Mathf.CeilToInt((float)oldSlot.amount / 2);
                oldSlot.itemAmountText.text = oldSlot.amount.ToString();
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                // Викид об'єктів із інвентарю - Спавнимо префаб об'єкта перед персонажем
                float spawnHeight = 0.1f; // Adjust this value based on the height at which you want the item to spawn
                float spawnDistance = 1.0f;
                Vector3 spawnPosition = player.position + player.forward * spawnDistance + new Vector3(0, spawnHeight, 0);
                Quaternion spawnRotation = player.rotation;
                GameObject itemObject = Instantiate(oldSlot.item.itemPrefab, spawnPosition, spawnRotation);
                // Устанавливаем количество объектов такое какое было в слоте
                itemObject.GetComponent<Item>().amount = 1;
                oldSlot.amount--;
                oldSlot.itemAmountText.text = oldSlot.amount.ToString();
            }
            else 
            {
                // Викид об'єктів із інвентарю - Спавнимо префаб об'єкта перед персонажем
                float spawnHeight = 0.1f; // Adjust this value based on the height at which you want the item to spawn
                float spawnDistance = 1.0f;
                Vector3 spawnPosition = player.position + player.forward * spawnDistance + new Vector3(0, spawnHeight, 0);
                Quaternion spawnRotation = player.rotation;
                GameObject itemObject = Instantiate(oldSlot.item.itemPrefab, spawnPosition, spawnRotation);
                // Встановлюємо кількість об'єктів таке, яке було в слоті
                itemObject.GetComponent<Item>().amount = oldSlot.amount;
                // забираємо значення InventorySlot
                NullifySlotData();
            }
            quickslotInventory.CheckItemInHand();
        }
        else if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent == null)
        {
            return;
        }

        else if(eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>() != null)
        {
            InventorySlot inventorySlot = eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>();
            if(oldSlot.clothType != ClothType.None && oldSlot.item != null)
            {
                foreach(ClotheAdder clothAdder in clothAdders)
                {
                    clothAdder.RemoveClothes(oldSlot.item.clothingPrefab);
                    oldSlot.clothingImage.enabled = true;
                }
            }
            
            
            if(inventorySlot.clothType != ClothType.None)
            {
                if(inventorySlot.clothType == oldSlot.item.clothType)
                {
                    ExchangeSlotData(inventorySlot);
                    foreach(ClotheAdder clothAdder in inventorySlot.GetComponentInChildren<DragAndDropItem>().clothAdders)
                    {
                        clothAdder.addClothes(inventorySlot.item.clothingPrefab);
                        inventorySlot.clothingImage.enabled = false;
                    }
                }
                else
                {   
                    return;
                }
            }
            else
            {
                ExchangeSlotData(inventorySlot);
                quickslotInventory.CheckItemInHand();
            }
        }

        if(oldSlot.amount <= 0)
        {
            NullifySlotData();
        }
       
    }
    public void NullifySlotData() // made public 
    {
        // забираємо значення InventorySlot
        oldSlot.item = null;
        oldSlot.amount = 0;
        oldSlot.isEmpty = true;
        oldSlot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        oldSlot.iconGO.GetComponent<Image>().sprite = null;
        oldSlot.itemAmountText.text = "";
    }
    void ExchangeSlotData(InventorySlot newSlot)
    {
        // Тимчасово зберігаємо дані newSlot в окремих змінних
        ItemScriptableObject item = newSlot.item;
        int amount = newSlot.amount;
        bool isEmpty = newSlot.isEmpty;
        GameObject iconGO = newSlot.iconGO;
        TMP_Text itemAmountText = newSlot.itemAmountText;
        if(item == null)
        {
            if (oldSlot.item.maximumAmount > 1 && oldSlot.amount > 1)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    newSlot.item = oldSlot.item;
                    newSlot.amount = Mathf.CeilToInt((float)oldSlot.amount/2);
                    newSlot.isEmpty = false;
                    newSlot.SetIcon(oldSlot.iconGO.GetComponent<Image>().sprite);
                    newSlot.itemAmountText.text = newSlot.amount.ToString();
 
                    oldSlot.amount = Mathf.FloorToInt((float)oldSlot.amount / 2); ;
                    oldSlot.itemAmountText.text = oldSlot.amount.ToString();
                    return;
                }
                else if (Input.GetKey(KeyCode.LeftControl))
                {
                    newSlot.item = oldSlot.item;
                    newSlot.amount = 1;
                    newSlot.isEmpty = false;
                    newSlot.SetIcon(oldSlot.iconGO.GetComponent<Image>().sprite);
                    newSlot.itemAmountText.text = newSlot.amount.ToString();
                    
                    oldSlot.amount--;
                    oldSlot.itemAmountText.text = oldSlot.amount.ToString();
                    return;
                }
            }
        }
        if (newSlot.item != null)
        {
            if (oldSlot.item.name.Equals(newSlot.item.name))
            {
                if (Input.GetKey(KeyCode.LeftShift) && oldSlot.amount > 1)
                {
                    if (Mathf.CeilToInt((float)oldSlot.amount / 2) < newSlot.item.maximumAmount - newSlot.amount)
                    {
                        newSlot.amount += Mathf.CeilToInt((float)oldSlot.amount / 2);
                        newSlot.itemAmountText.text = newSlot.amount.ToString();
 
                        oldSlot.amount -= Mathf.CeilToInt((float)oldSlot.amount / 2);
                        oldSlot.itemAmountText.text = oldSlot.amount.ToString();
                    }
                    else
                    {
                        int difference = newSlot.item.maximumAmount - newSlot.amount;
                        newSlot.amount = newSlot.item.maximumAmount;
                        newSlot.itemAmountText.text = newSlot.amount.ToString();
 
                        oldSlot.amount -= difference;
                        oldSlot.itemAmountText.text = oldSlot.amount.ToString();
 
                    }
                    return;
                }
                else if (Input.GetKey(KeyCode.LeftControl) && oldSlot.amount > 1)
                {
                    if (newSlot.item.maximumAmount != newSlot.amount)
                    {
                        newSlot.amount++;
                        newSlot.itemAmountText.text = newSlot.amount.ToString();
 
                        oldSlot.amount--;
                        oldSlot.itemAmountText.text = oldSlot.amount.ToString();
                    }
                    return;
                }
                else
                {
                    if (newSlot.amount + oldSlot.amount >= newSlot.item.maximumAmount)
                    {
                        int difference = newSlot.item.maximumAmount - newSlot.amount;
                        newSlot.amount = newSlot.item.maximumAmount;
                        newSlot.itemAmountText.text = newSlot.amount.ToString();
 
                        oldSlot.amount -= difference;
                        oldSlot.itemAmountText.text = oldSlot.amount.ToString();
                    }
                    else
                    {
                        newSlot.amount += oldSlot.amount;
                        newSlot.itemAmountText.text = newSlot.amount.ToString();
                        NullifySlotData();
                    }
                    return;
                }
                
            }
        }
         
        // Замінюємо значення newSlot на значення oldSlot
        newSlot.item = oldSlot.item;
        newSlot.amount = oldSlot.amount;
        if (oldSlot.isEmpty == false)
        {
            newSlot.SetIcon(oldSlot.iconGO.GetComponent<Image>().sprite);
            if (oldSlot.item.maximumAmount != 1) // added this if statement for single items
            {
                newSlot.itemAmountText.text = oldSlot.amount.ToString();
            }
            else
            {
                newSlot.itemAmountText.text = "";
            }
        }
        else
        {
            newSlot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            newSlot.iconGO.GetComponent<Image>().sprite = null;
            newSlot.itemAmountText.text = "";
        }
        
        newSlot.isEmpty = oldSlot.isEmpty;
 
        // Замінюємо значення oldSlot на значення newSlot збережені у змінних
        oldSlot.item = item;
        oldSlot.amount = amount;
        if (isEmpty == false)
        {
            oldSlot.SetIcon(item.icon);
            if (item.maximumAmount != 1) // added this if statement for single items
            {
                oldSlot.itemAmountText.text = amount.ToString();
            }
            else
            {
                oldSlot.itemAmountText.text = "";
            }
        }
        else
        {
            oldSlot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            oldSlot.iconGO.GetComponent<Image>().sprite = null;
            oldSlot.itemAmountText.text = "";
        }
        
        oldSlot.isEmpty = isEmpty;
    }
}