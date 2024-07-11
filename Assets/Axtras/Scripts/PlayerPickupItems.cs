using System;
using UnityEngine;

[Serializable]
public class PlayerPickupItems
{
    public enum PickupType { None, Swingable, Throwable, Consumable }

    [Header("Main Properties")]
    public PickupType type;
    public string name;

    [Header("Object Properties")]
    public GameObject prefab;
    public GameObject obj;

    public void PrintDetails() {
        string details = 
            $"Pickup Item Details:\n" +
            $"Type: {type}\n" +
            $"Name: {name}\n" +
            $"Prefab: {(prefab != null ? prefab.name : "None")}\n" +
            $"Object: {(obj != null ? obj.name : "None")}"
        ;
        Debug.Log(details);
    }
}