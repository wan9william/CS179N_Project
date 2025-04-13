using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : Interactable, IPointerClickHandler
{
    Image itemIcon;
    public Item userItem { get; set; }
    public InventorySlot slot { get; set; }

    void Awake()
    {
        itemIcon = GetComponent<Image>();
    }
    public void Initialize(Item item, InventorySlot invenSlot)
    {
        slot = invenSlot;
        slot.userItem = this;
        userItem = item;
        itemIcon.sprite = item.sprite;
    }
    public void onPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            Inventory.Singleton.SetCarriedItem(this);
        }
    }
}