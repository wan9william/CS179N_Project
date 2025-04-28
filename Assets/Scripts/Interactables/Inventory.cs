using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
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

    [Header("Stack Settings")]
    [SerializeField] private int maxStackSize = 99;
    private bool isShiftPressed = false;

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
        Item_ScriptableObj item = new_item.Item1;
        int quantity = new_item.Item2;

        for (int i = 0; i < 8; i++)
        {
            // stacking existing items of same type in inventory
            if (inventorySlots[i].item && inventorySlots[i].GetItem() == item)
            {
                int currentQuantity = inventorySlots[i].GetQuantity();
                if (currentQuantity < maxStackSize)
                {
                    int spaceInStack = maxStackSize - currentQuantity;
                    int amountToAdd = Mathf.Min(quantity, spaceInStack);
                    
                    inventorySlots[i].SetItem_A(new Tuple<Item_ScriptableObj, int>(item, currentQuantity + amountToAdd));
                    quantity -= amountToAdd;

                    if (quantity <= 0) return;
                }
            }
        }

        for (int i = 0; i < 8; i++)
        {
            // if items of a different type, then find new available inventory slots
            if (!inventorySlots[i].item)
            {
                int amountToAdd = Mathf.Min(quantity, maxStackSize);
                inventorySlots[i].SetItem_A(new Tuple<Item_ScriptableObj, int>(item, amountToAdd));
                quantity -= amountToAdd;

                if (quantity <= 0) return; // All items have been placed
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
        isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        
        if(carriedItem != null)
        {
            carriedItem.transform.position = Input.mousePosition;
        }
    }

    public void SetCarriedItem(InventoryItem item)
    {
        if (carriedItem != null)
        {
            if (item.activeSlot.myTag != slotTag.None && item.activeSlot.myTag != carriedItem.myItem.itemTag)
            {
                return;
            }

            // Stack items if same type
            if (item.myItem == carriedItem.myItem)
            {
                int totalQuantity = item.GetQuantity() + carriedItem.GetQuantity();
                if (totalQuantity <= maxStackSize)
                {
                    item.SetQuantity(totalQuantity);
                    carriedItem = null;
                    return;
                }
            }

            item.activeSlot.SetItem(carriedItem);
        }

        if (isShiftPressed && item.GetQuantity() > 1)
        {
            // Split stack in item quantity
            int originalQuantity = item.GetQuantity();
            int splitQuantity = originalQuantity / 2;
            
            item.SetQuantity(originalQuantity - splitQuantity);
            
            // Create new carried item with split quantity
            InventoryItem splitItem = Instantiate(itemPrefab, dragTransform);
            splitItem.Initialize(item.myItem, null);
            splitItem.SetQuantity(splitQuantity);
            carriedItem = splitItem;
        }
        else
        {
            if (item.activeSlot.myTag != slotTag.None)
            {
                Equip(item.activeSlot.myTag, null);
            }

            carriedItem = item;
            item.transform.SetParent(dragTransform);
        }
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