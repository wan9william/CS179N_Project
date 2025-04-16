using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Singleton;
    public static InventoryItem carriedItem;
    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] Transform dragTransform;
    [SerializeField] InventoryItem itemPrefab;
    [Header("Item List")]
    [SerializeField] Item[] items;
    [Header("Debug")]
    [SerializeField] Button itemButton;

    void Awake()
    {
        Singleton = this;
        itemButton.onClick.AddListener( delegate { SpawnInventoryItem(); });
    }

    public void SpawnInventoryItem(Item currItem = null)
    {
        Item _currItem = currItem;
        if(_currItem == null)
        {
            int rand = Random.Range(0, items.Length);
            _currItem = items[rand];
        }

        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i].myItem == null)
            {
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(_currItem, inventorySlots[i]);
                break;
            }
        }
    }

    void Update()
    {
        if(carriedItem == null)
        {
            return;
        }

        carriedItem.transform.position = Input.mousePosition;
    }

    public void SetCarriedItem(InventoryItem item)
    {
        if(carriedItem != null)
        {
            if(item.activeSlot.myTag != slotTag.None && item.activeSlot.myTag != carriedItem.myItem.itemTag)
            {
                return;
            }
            item.activeSlot.SetItem(carriedItem);
        }
        if(item.activeSlot.myTag != slotTag.None)
        {
            Equip(item.activeSlot.myTag, null);
        }

        carriedItem = item;
        item.transform.SetParent(dragTransform);
    }

    public void Equip(slotTag tag, InventoryItem item = null)
    {
        switch(tag)
        {
            case slotTag.Bullet:
                if(item == null)
                {
                    Debug.Log("Unequipped on " + tag);
                }
                else
                {
                    Debug.Log("Equipped " + item.myItem.name + " on " + tag);
                }
                break;
            case slotTag.Health:
                if(item == null)
                {
                    Debug.Log("Unequipped on " + tag);
                }
                else
                {
                    Debug.Log("Equipped " + item.myItem.name + " on " + tag);
                }
                break;
            case slotTag.Duck:
                if(item == null)
                {
                    Debug.Log("Unequipped on " + tag);
                }
                else
                {
                    Debug.Log("Equipped " + item.myItem.name + " on " + tag);
                }
                break;
            case slotTag.Shotgun:
                if(item == null)
                {
                    Debug.Log("Unequipped on " + tag);
                }
                else
                {
                    Debug.Log("Equipped " + item.myItem.name + " on " + tag);
                }
                break;
            case slotTag.Pistol:
                if(item == null)
                {
                    Debug.Log("Unequipped on " + tag);
                }
                else
                {
                    Debug.Log("Equipped " + item.myItem.name + " on " + tag);
                }
                break;
            case slotTag.Knife:
                if(item == null)
                {
                    Debug.Log("Unequipped on " + tag);
                }
                else
                {
                    Debug.Log("Equipped " + item.myItem.name + " on " + tag);
                }
                break;
            case slotTag.MachineGun:
                if(item == null)
                {
                    Debug.Log("Unequipped on " + tag);
                }
                else
                {
                    Debug.Log("Equipped " + item.myItem.name + " on " + tag);
                }
                break;
        }

    }
}