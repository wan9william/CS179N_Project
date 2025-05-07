using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    public Image itemIcon;
    public Item myItem { get; set; }
    public InventorySlot activeSlot { get; set; }
    private int quantity = 1;

    private void Awake()
    {
        itemIcon = GetComponent<Image>();
    }
    public void Initialize(Item item, InventorySlot invenSlot)
    {
        activeSlot = invenSlot;
        if(activeSlot != null)
        {
            activeSlot.myItem = this;
        }
        myItem = item;
        itemIcon.sprite = item.sprite;
    }

    // marks the item as carried
    public void OnBeginDrag(PointerEventData eventData)
    {

        Debug.Log("drag!");
        // Only allow right-click dragging
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        // Store original parent for returning if drag fails
        parentAfterDrag = transform.parent;
        
        // Move to canvas level for dragging
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        
        // Disable raycast on item while dragging
        itemIcon.raycastTarget = false;
        
        // Set as carried item in inventory
        if (activeSlot != null)
        {
            // If shift is held and we have more than 1 item, prepare for splitting
            bool isShiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (isShiftHeld && quantity > 1)
            {
                int splitAmount = quantity / 2;
                SetQuantity(quantity - splitAmount);
                
                // Create new carried item with split quantity
                InventoryItem splitItem = Instantiate(gameObject, transform.root).GetComponent<InventoryItem>();
                splitItem.Initialize(myItem, null);
                splitItem.SetQuantity(splitAmount);
                Inventory.Singleton.SetCarriedItem(splitItem);
            }
            else
            {
                Inventory.Singleton.SetCarriedItem(this);
            }
        }
    }

    // makes it carried item follow the cursor
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;
            
        Debug.Log("Dragging at: " + Input.mousePosition);
        // Update position to follow cursor
        transform.position = Input.mousePosition;
    }

    // places it in a new slot or if no available slots, returns it to the original slot
    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        // Re-enable raycast
        itemIcon.raycastTarget = true;
        
        // If we're still being carried (wasn't dropped on a valid slot)
        if (Inventory.carriedItem == this)
        {
            // Return to original slot
            if (activeSlot != null)
            {
                transform.SetParent(activeSlot.transform);
                transform.localPosition = Vector3.zero;
                Inventory.carriedItem = null;
            }
            else
            {
                // If somehow we don't have an active slot, return to last known parent
                transform.SetParent(parentAfterDrag);
                transform.localPosition = Vector3.zero;
            }
        }
    }

    public void SetQuantity(int weight)
    {
        quantity = Mathf.Max(0,weight);

        if(quantity <= 0)
        {
            if(activeSlot != null)
            {
                activeSlot.myItem = null;
                activeSlot.item = null;
                activeSlot.quantity = 0;
            }
        }
    }

    public int GetQuantity()
    {
        return quantity;
    }

    void KeyboardControls()
    {
        for(int i = 0; i < 8; i++)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1+i))
            {
                Inventory.Singleton.SetCarriedItem(this);
                break;
            }
        }
    }
}