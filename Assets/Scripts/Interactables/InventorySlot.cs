using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem myItem { get; set;}
    public slotTag myTag;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if(Inventory.carriedItem == null)
            {
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
        Inventory.carriedItem = null;
        item.activeSlot.myItem = null;

        myItem = item;
        myItem.activeSlot = this;
        myItem.transform.SetParent(transform);

        if(myTag != slotTag.None)
        {
            Inventory.Singleton.Equip(myTag, myItem);
        }
    }
}