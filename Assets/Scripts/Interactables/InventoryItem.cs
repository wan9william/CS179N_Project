using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Image itemIcon;
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
        Debug.Log("OnBeginDrag called");
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
        
        if (activeSlot != null)
        {
            Debug.Log("Setting carried item");
            Inventory.Singleton.SetCarriedItem(this);
        }
    }

    // makes it carried item follow the cursor
    public void OnDrag(PointerEventData eventData)
    {
        if (Inventory.carriedItem == this)
        {
            transform.position = Input.mousePosition;
        }
    }

    // places it in a new slot or if no available slots, returns it to the original slot
    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
        
        // If we're still being carried (wasn't placed in a slot)
        if (Inventory.carriedItem == this)
        {
            // Return to original slot if there exists
            if (activeSlot != null)
            {
                transform.SetParent(activeSlot.transform);
                transform.localPosition = Vector3.zero;
                Inventory.carriedItem = null;
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