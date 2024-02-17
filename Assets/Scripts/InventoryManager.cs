using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject UIBG; // renamed
    public GameObject crosshair;
    public Transform inventoryPanel;
    public Transform inventoryAndClothingPanel;
    public Transform quickslotPanel;
    public List<InventorySlot> slots = new List<InventorySlot>();
    public bool isOpened;
    public float reachDistance = 3f;
    private Camera mainCamera;
    public CinemachineVirtualCamera CVC;
    [SerializeField] private Transform player;
    [SerializeField] private List<ClotheAdder> _clothAdders = new List<ClotheAdder>();
    // Start is called before the first frame update
    private void Awake()
    {
        UIBG.SetActive(true);
    }
    void Start()
    {
        mainCamera = Camera.main;
        for(int i = 0; i < inventoryPanel.childCount; i++)
        {
            if(inventoryPanel.GetChild(i).GetComponent<InventorySlot>() != null)
            {
                slots.Add(inventoryPanel.GetChild(i).GetComponent<InventorySlot>());
            }
        }
        for (int i = 0; i < quickslotPanel.childCount; i++)
        {
            if (quickslotPanel.GetChild(i).GetComponent<InventorySlot>() != null)
            {
                slots.Add(quickslotPanel.GetChild(i).GetComponent<InventorySlot>());
            }
        }

        CVC.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisName = "Mouse X";
        CVC.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InputAxisName = "Mouse Y"; 
        UIBG.SetActive(false);
        inventoryAndClothingPanel.gameObject.SetActive(false);//new line
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isOpened = !isOpened;
            if (isOpened)
            {
                CVC.enabled = false;
                Time.timeScale = 0f;
                UIBG.SetActive(true);
                inventoryAndClothingPanel.gameObject.SetActive(true); // new line
                crosshair.SetActive(false);
                CVC.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisName = "";
                CVC.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InputAxisName = "";
                CVC.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisValue = 0;
                CVC.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InputAxisValue = 0;
                // Прикріплюємо курсор до середини екрану
                Cursor.lockState = CursorLockMode.None;
                // і робимо його невидимим
                Cursor.visible = true;

            }
            else
            {
                CVC.enabled = true;
                Time.timeScale = 1f;
                UIBG.SetActive(false);
                inventoryAndClothingPanel.gameObject.SetActive(false); // new line
                crosshair.SetActive(true);
                CVC.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisName = "Mouse X";
                CVC.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InputAxisName = "Mouse Y";
                // Прикріплюємо курсор до середини екрану
                Cursor.lockState = CursorLockMode.Locked;
                // і робимо його невидимим
                Cursor.visible = false;
            }
        }
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(ray, out hit, reachDistance))
            {
                if (hit.collider.gameObject.GetComponent<Item>() != null)
                {
                    AddItem(hit.collider.gameObject.GetComponent<Item>().item, hit.collider.gameObject.GetComponent<Item>().amount);
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }


    public void RemoveItemFromSlot(int slotId)
    {
        InventorySlot slot = slots[slotId];
        
        if (slot.clothType != ClothType.None && !slot.isEmpty)
        {
            foreach (ClotheAdder clothAdder in _clothAdders)
            {
                clothAdder.RemoveClothes(slot.item.clothingPrefab);
            }
        }
        slot.item = null;
        slot.isEmpty = true;
        slot.amount = 0;
        slot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        slot.iconGO.GetComponent<Image>().sprite = null;
        slot.itemAmountText.text = "";
    }


    public void AddItemToSlot(ItemScriptableObject _item, int _amount, int slotId)
    {
        InventorySlot slot = slots[slotId];
        slot.item = _item;
        slot.isEmpty = false;
        slot.SetIcon(_item.icon);
        
        if (_amount <= _item.maximumAmount)
        {
            slot.amount = _amount;
            if (slot.item.maximumAmount != 1) // added this if statement for single items
            {
                slot.itemAmountText.text = slot.amount.ToString();
            }
        }
        else
        {
            slot.amount = _item.maximumAmount;
            _amount -= _item.maximumAmount;
            if (slot.item.maximumAmount != 1) // added this if statement for single items
            {
                slot.itemAmountText.text = slot.amount.ToString();
            }
        }

        if (slot.clothType != ClothType.None)
        {
            foreach (ClotheAdder clothAdder in _clothAdders)
            {
                clothAdder.addClothes(slot.item.clothingPrefab);
            }
        }
    }


    public void AddItem(ItemScriptableObject _item, int _amount)
    {
       
        int amount = _amount;
        foreach (InventorySlot slot in slots)
        {
            // Стакаєм предмети разом
            // В слоті вже є предмет
            if (slot.item == _item)
            {
                if (slot.amount + amount <= _item.maximumAmount) {
                    slot.amount += amount;
                    slot.itemAmountText.text = slot.amount.ToString();
                    return;
                }
                else
                {
                    amount -= _item.maximumAmount - slot.amount;
                    slot.amount = _item.maximumAmount;
                    slot.itemAmountText.text = slot.amount.ToString();
                }
                continue;
            }
        }

        bool allFull = true;
        foreach (InventorySlot inventorySlot in slots)
        {
            if (inventorySlot.isEmpty)
            {
                allFull = false;
                break;
            }
        }
        if (allFull)
        {
            float spawnHeight = 0.1f;
            float spawnDistance = 1.0f;
            Vector3 spawnPosition = player.position + player.forward * spawnDistance + new Vector3(0, spawnHeight, 0);
            Quaternion spawnRotation = player.rotation;
            GameObject itemObject = Instantiate(_item.itemPrefab, spawnPosition, spawnRotation);
            itemObject.GetComponent<Item>().amount = _amount;
        }

        foreach (InventorySlot slot in slots)
        {
            if (amount <= 0)
                return;
            // додаємо предмети в пусті слоти
            if(slot.isEmpty == true)
            {
                slot.item = _item;
                //slot.amount = amount;
                slot.isEmpty = false;
                slot.SetIcon(_item.icon);
                
                if (amount <= _item.maximumAmount)
                {
                    slot.amount = amount;
                    if (slot.item.maximumAmount != 1)
                    {
                        slot.itemAmountText.text = slot.amount.ToString();
                    }
                    break;
                }
                else
                {
                    slot.amount = _item.maximumAmount;
                    amount -= _item.maximumAmount;
                    if (slot.item.maximumAmount != 1) 
                    {
                        slot.itemAmountText.text = slot.amount.ToString();
                    }
                }

                allFull = true;
                foreach (InventorySlot inventorySlot in slots)
                {
                    if (inventorySlot.isEmpty)
                    {
                        allFull = false;
                        break;
                    }
                }
                if (allFull)
                {
                    float spawnHeight = 0.1f;
                    float spawnDistance = 1.0f;
                    Vector3 spawnPosition = player.position + player.forward * spawnDistance + new Vector3(0, spawnHeight, 0);
                    Quaternion spawnRotation = player.rotation;
                    GameObject itemObject = Instantiate(_item.itemPrefab, spawnPosition, spawnRotation);
                    itemObject.GetComponent<Item>().amount = amount;
                    return;
                }
            }
        }
    }
}
