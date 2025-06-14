using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory
{
    public static Inventory Singleton;
    public InventoryItem carriedItem;
    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] int selectedSlot;


    [SerializeField] Transform dragTransform;
    [SerializeField] InventoryItem itemPrefab;
    [Header("Item List")]
    [SerializeField] Item[] items;
    [Header("Debug")]
    [SerializeField] Button itemButton;

    [Header("Stack Settings")]
    [SerializeField] private int maxStackSize = 64;
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
    public int addItem(Tuple<Item_ScriptableObj, int> new_item)
    {
        Item_ScriptableObj item = new_item.Item1;
        int quantity = new_item.Item2;

        // Detect if item is a weapon by checking if its prefab has a Weapon component
        bool isWeapon = item.getPrefab()?.GetComponentInChildren<Weapon>() != null;

        // 1. Stack only if it's not a weapon
        if (!isWeapon)
        {
            for (int i = 0; i < 8; i++)
            {
                if (inventorySlots[i].item && inventorySlots[i].GetItem() == item)
                {
                    int currentQuantity = inventorySlots[i].GetQuantity();
                    if (currentQuantity < maxStackSize)
                    {
                        int spaceInStack = maxStackSize - currentQuantity;
                        int amountToAdd = Mathf.Min(quantity, spaceInStack);

                        inventorySlots[i].SetItem_A(new Tuple<Item_ScriptableObj, int>(item, currentQuantity + amountToAdd));
                        quantity -= amountToAdd;

                        inventorySlots[i].storedAmmo = -1;
                        inventorySlots[i].UpdateQuantityDisplay();

                        if (quantity <= 0) return 0;
                    }
                }
            }
        }

        // 2. Add to next empty slot (always for weapons)
        for (int i = 0; i < 8; i++)
        {
            if (inventorySlots[i].item.getID() == 0)
            {
                int amountToAdd = Mathf.Min(quantity, maxStackSize);
                inventorySlots[i].SetItem_A(new Tuple<Item_ScriptableObj, int>(item, amountToAdd));
                quantity -= amountToAdd;

                if (isWeapon)
                {
                    inventorySlots[i].storedAmmo = new_item.Item2; // from pickup
                }
                else
                {
                    inventorySlots[i].storedAmmo = -1;
                }

                inventorySlots[i].UpdateQuantityDisplay();

                if (quantity <= 0) return 0;
            }
        }

        return quantity;
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

    public int GetSelectedSlot() { return selectedSlot; }

    public bool SelectedSlotIsEmpty() { return inventorySlots[selectedSlot].GetQuantity() <= 0; }


    public void InitializeInventory(Transform _tf) {
        for (int i = 0; i < 8; ++i) {
            InventorySlot slot = _tf.gameObject.transform.GetChild(i).GetComponent<InventorySlot>();
            slot.SetInven(this);

            inventorySlots[i] = slot;
        }
    }


    public void SpawnInventoryItem(Item currItem = null)
    {
        /*Item _currItem = currItem;
        if(_currItem == null)
        {
            int rand = UnityEngine.Random.Range(0, items.Length);
            _currItem = items[rand];
        }

        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i].myItem == null)
            {
                // Find or create the Image component under the slot
                Transform imageTransform = inventorySlots[i].transform.Find("ItemImage");
                if (imageTransform == null)
                {
                    // Create a new GameObject for the item image
                    GameObject imageObj = new GameObject("ItemImage");
                    imageObj.transform.SetParent(inventorySlots[i].transform);
                    imageObj.transform.localPosition = Vector3.zero;
                    imageTransform = imageObj.transform;
                    
                    // Add required components
                    Image image = imageObj.AddComponent<Image>();
                    image.raycastTarget = true;
                }
                
                // Add InventoryItem component to the image object
                InventoryItem newItem = imageTransform.gameObject.AddComponent<InventoryItem>();
                newItem.Initialize(_currItem, inventorySlots[i]);
                inventorySlots[i].myItem = newItem;
                return; // Exit after spawning one item
            }
        }*/
    }

    void Update()
    {
        //Not even running?

        isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        
        if(carriedItem != null)
        {
            carriedItem.transform.position = Input.mousePosition;
        }

        Debug.Log(carriedItem);
    }

    public void SetCarriedItem(InventoryItem item)
    {
        carriedItem = item;
        /*
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
                    Debug.Log("Set to null here");
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
            
            /*
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
        }*/
    }

    public void Equip(slotTag tag, InventoryItem item = null)
    {
        string action = (item == null) ? "Unequipped on " : "Equipped " + item.myItem.name + " on ";
        Debug.Log(action + tag);

    }

    public void DecrementItem(int index) {

        int quantity = inventorySlots[index].GetQuantity() - 1;
        inventorySlots[index].SetQuantity(quantity);
        inventorySlots[index].UpdateItem();
    }

    public GameObject selecteditem(int index)
    {
        Item_ScriptableObj item = inventorySlots[index].GetItem();

        if (item != null && item.getPrefab() != null)
        {
            Debug.Log("[Inventory] Returning equipped prefab: " + item.getPrefab().name);
            return item.getPrefab();
        }

        GameObject fallback = Player.Singleton?.GetFlashlightPrefab();
        Debug.Log("[Inventory] Slot is empty. Returning flashlight: " + (fallback != null ? fallback.name : "null"));
        return fallback;
    }

    public GameObject GetSelectedResource(int index) {
        Item_ScriptableObj item = inventorySlots[index].GetItem();
        return item.getResourcePrefab();
    }
    
    public int GetMaxStackSize()
    {
        return maxStackSize;
    }


    public int GetTotalAmmo(Item_ScriptableObj ammoType)
    {
        int total = 0;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].GetItem() == ammoType)
            {
                total += inventorySlots[i].GetQuantity();
            }
        }
        return total;
    }

    public void ConsumeAmmo(Item_ScriptableObj ammoType, int amount)
    {
        for (int i = 0; i < inventorySlots.Length && amount > 0; i++)
        {
            if (inventorySlots[i].GetItem() == ammoType)
            {
                int available = inventorySlots[i].GetQuantity();
                int subtract = Mathf.Min(available, amount);

                inventorySlots[i].SetQuantity(available - subtract);
                inventorySlots[i].UpdateItem();

                amount -= subtract;
            }
        }
    }

    public InventorySlot[] getInventorySlots()
    {
        return inventorySlots;
    }
}