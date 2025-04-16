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