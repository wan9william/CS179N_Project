using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum slotTag { None, Bullet, Health, Duck, Shotgun, Pistol, Knife, MachineGun }

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]

public class Item : ScriptableObject
{
    public Sprite sprite;
    public slotTag itemTag;
    [Header("If the item can be equipped")]
    public GameObject equipPrefab;
}