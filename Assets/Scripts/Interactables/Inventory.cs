using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory
{
    public static Inventory Singleton;
    public static InventoryItem carriedItem;
    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] int selectedSlot;


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

        inventorySlots = new InventorySlot[8];
    }

    public Inventory(Transform _tf) {
        inventorySlots = new InventorySlot[8];
        InitializeInventory(_tf); 
    }

    //When adding an item to the inventory
    public void addItem(Tuple<Item_ScriptableObj,int> new_item) {
        //search array. If an empty box is found, add to it. The sprite and quantity will be managed by individual boxes?
        for (int i = 0; i < 8; i++)
        {
            if (!inventorySlots[i].item || inventorySlots[i].GetItem() == new_item.Item1)
            {
                //add item and its quantity through a slot's respective script
                inventorySlots[i].SetItem_A(new_item);
                break;
            }
        }
    }

    public void SelectSlot(int index) {
        if (selectedSlot == index) return;

        //Unselect the previous slot
        inventorySlots[selectedSlot].SetUnselect(true);

        //Assign new slot
        selectedSlot = index;

        //Select the new slot
        inventorySlots[selectedSlot].SetSelect(true);
    }




    public void InitializeInventory(Transform _tf) {
        for (int i = 0; i < 8; ++i) {
            InventorySlot slot = _tf.gameObject.transform.GetChild(i).GetComponent<InventorySlot>();
            inventorySlots[i] = slot;
        }
    }


    public void SpawnInventoryItem(Item currItem = null)
    {
        Item _currItem = currItem;
        if(_currItem == null)
        {
            int rand = UnityEngine.Random.Range(0, items.Length);
            _currItem = items[rand];
        }

        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i].myItem == null)
            {
                //Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(_currItem, inventorySlots[i]);
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

    public GameObject selecteditem(int index)
    {
        Item_ScriptableObj item = inventorySlots[index].GetItem();

        // If the slot has an item, return its prefab
        if (item != null && item.getPrefab() != null)
        {
            return item.getPrefab();
        }

        // If the slot is empty, return currently equipped prefab
        if (Player.Singleton != null && Player.Singleton.GetEquippedPrefab() != null)
        {
            return Player.Singleton.GetEquippedPrefab();
        }

        // Nothing to equip
        return null;
    }

}