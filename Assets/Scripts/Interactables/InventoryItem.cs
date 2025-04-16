using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    Image itemIcon;
    public Item myItem { get; set; }
    public InventorySlot activeSlot { get; set; }

    void Awake()
    {
        itemIcon = GetComponent<Image>();
    }
    public void Initialize(Item item, InventorySlot invenSlot)
    {
        activeSlot = invenSlot;
        activeSlot.myItem = this;
        myItem = item;
        itemIcon.sprite = item.sprite;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            Inventory.Singleton.SetCarriedItem(this);
        }
        else if(eventData.button == PointerEventData.KeyCode.1)
        {
            Inventory.Singleton.SetCarriedItem(this);
        }
        else if(eventData.button == PointerEventData.KeyCode.2)
        {
            Inventory.Singleton.SetCarriedItem(this);
        }
        else if(eventData.button == PointerEventData.KeyCode.3)
        {
            Inventory.Singleton.SetCarriedItem(this);
        }
        else if(eventData.button == PointerEventData.KeyCode.4)
        {
            Inventory.Singleton.SetCarriedItem(this);
        }
        else if(eventData.button == PointerEventData.KeyCode.5)
        {
            Inventory.Singleton.SetCarriedItem(this);
        }
        else if(eventData.button == PointerEventData.KeyCode.6)
        {
            Inventory.Singleton.SetCarriedItem(this);
        }
        else if(eventData.button == PointerEventData.KeyCode.7)
        {
            Inventory.Singleton.SetCarriedItem(this);
        }
        else if(eventData.button == PointerEventData.KeyCode.8)
        {
            Inventory.Singleton.SetCarriedItem(this);
        }
    }
}