using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType {Default,Food,Weapon,Instrument,Clothing}
public enum ClothType{None,Head,Body,Legs, Feet};
public class ItemScriptableObject : ScriptableObject
{
    
    public string itemName;
    public int maximumAmount;
    public GameObject itemPrefab;
    public GameObject clothingPrefab;
    public Sprite icon;
    public ItemType itemType;
    public ClothType clothType = ClothType.None;
    public string itemDescription;
    public bool isConsumeable;
    public string inHandName; 

    [Header("Consumable Characteristics")]
    public float changeHealth;
    public float changeHunger;
    public float changeThirst;
}
