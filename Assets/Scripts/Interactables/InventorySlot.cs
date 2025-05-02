using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public InventoryItem myItem { get; set;}

    //Item Properties
    public Item_ScriptableObj item;
    public int quantity;

    [SerializeField] private TextMeshProUGUI quantityText;

    //Slot state machine
    private enum SLOT_ACTION_STATES { IDLE, SELECT, UNSELECT };
    private SLOT_ACTION_STATES state;
    private bool select;
    private bool unselect;

    public slotTag myTag;

    void Start()
    {
        // adds a text number of how many of that item there is to bottom right of inventory square
        if (quantityText == null)
        {
            GameObject textObj = new GameObject("QuantityText");
            textObj.transform.SetParent(transform);
            quantityText = textObj.AddComponent<TextMeshProUGUI>();
            quantityText.alignment = TextAlignmentOptions.BottomRight;
            quantityText.fontSize = 12;
            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = new Vector2(2, 2);
            rect.offsetMax = new Vector2(-2, -2);
        }
        UpdateQuantityDisplay();
    }

    public void Update()
    {
        switch (state)
        {
            case SLOT_ACTION_STATES.IDLE:
                if (select) {
                    state = SLOT_ACTION_STATES.SELECT;
                    select = false;
                    break;
                }

                if (unselect)
                {
                    state = SLOT_ACTION_STATES.UNSELECT;
                    unselect = false;
                    break;
                }
                break;
            case SLOT_ACTION_STATES.SELECT:

                //grow the square
                transform.localScale = Vector3.one * 1.2f;
                state = SLOT_ACTION_STATES.IDLE;
                break;
            case SLOT_ACTION_STATES.UNSELECT:

                state = SLOT_ACTION_STATES.IDLE;
                transform.localScale = Vector3.one;
                break;
            default:
                break;
        }
    }

    public void SetSelect(bool _s) { select = _s; }

    public void SetUnselect(bool _s) { unselect = _s; }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(Inventory.carriedItem == null)
            {
                if(myItem != null)
                {
                    // if more than 1 item of that type exists in a slot and shift is pressed, split the stack of that item
                    if((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift)) && quantity > 1)
                    {
                        int split = quantity / 2;
                        quantity = quantity - split;
                        UpdateQuantityDisplay();
                        // creating a new carried item with the quantity of split result
                        InventoryItem splitNewItem = Instantiate(myItem, Inventory.Singleton.transform);
                        splitNewItem.Initialize(myItem.myItem, null);
                        splitNewItem.SetQuantity(split);
                        Inventory.carriedItem = splitNewItem;
                    }
                }
                return;
            }
            if(myTag != slotTag.None && Inventory.carriedItem.myItem.itemTag != myTag)
            {
                return;
            }
            SetItem(Inventory.carriedItem);
        }
    }

    public void SetItem(InventoryItem item)
    {
        // If already have an item and it's the same type, add to the quantity of that existing item
        if (myItem != null && myItem.myItem == item.myItem)
        {
            int totalQuantity = myItem.GetQuantity() + item.GetQuantity();
            int maxStack = Inventory.Singleton.GetMaxStackSize();
            
            if (totalQuantity <= maxStack)
            {
                myItem.SetQuantity(totalQuantity);
                Destroy(item.gameObject);
                Inventory.carriedItem = null;
                return;
            }
        }

        // Regular item placement
        Inventory.carriedItem = null;
        if (item.activeSlot != null)
        {
            item.activeSlot.myItem = null;
        }

        myItem = item;
        myItem.activeSlot = this;
        myItem.transform.SetParent(transform);
        myItem.transform.localPosition = Vector3.zero;

        if(myTag != slotTag.None)
        {
            Inventory.Singleton.Equip(myTag, myItem);
        }
    }

    public void SetItem_A(Tuple<Item_ScriptableObj,int> new_item)
    {
        item = new_item.Item1;
        quantity = new_item.Item2;

        //Reset visuals
        Image child_image = transform.GetChild(0).GetComponent<Image>();
        child_image.sprite = item.getSprite();
        child_image.color = item.getSprite() ? new Color(1, 1, 1, 1) : new Color(1,1,1,0);
        //Get quantity as well
        UpdateQuantityDisplay();
    }

    private void UpdateQuantityDisplay()
    {
        if(quantityText != null)
        {
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
        }
    }

    public Item_ScriptableObj GetItem()
    {
        return item;
    }

    public int GetQuantity()
    {
        return quantity;
    }

    // Called when an item is dropped onto slot and handles stacking of same items and placement of new items
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            InventoryItem dragItem = dropped.GetComponent<InventoryItem>();
            dragItem.parentAfterDrag = transform;
        }
        if (Inventory.carriedItem != null)
        {
            // Check if the slot has a tag restriction
            if (myTag != slotTag.None && myTag != Inventory.carriedItem.myItem.itemTag)
            {
                return;
            }

            // If we already have an item and it's the same type, try to stack
            if (myItem != null && myItem.myItem == Inventory.carriedItem.myItem)
            {
                int totalQuantity = myItem.GetQuantity() + Inventory.carriedItem.GetQuantity();
                int maxStack = Inventory.Singleton.GetMaxStackSize();
                
                if (totalQuantity <= maxStack)
                {
                    myItem.SetQuantity(totalQuantity);
                    Destroy(Inventory.carriedItem.gameObject);
                    Inventory.carriedItem = null;
                    return;
                }
            }

            // Regular item placement
            SetItem(Inventory.carriedItem);
        }
    }

}