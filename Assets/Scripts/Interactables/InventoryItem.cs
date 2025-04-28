using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            Inventory.Singleton.SetCarriedItem(this);
        }
    }

    public void SetQuantity(int weight)
    {
        quantity = Mathf.Max(0,weight);
        if (activeSlot != null)
        {
            // Update quantity display in the slot
            activeSlot.SetItem_A(new System.Tuple<Item_ScriptableObj, int>(myItem, quantity));
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