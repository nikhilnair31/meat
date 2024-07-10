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
}