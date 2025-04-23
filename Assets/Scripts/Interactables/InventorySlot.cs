using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem myItem { get; set;}

    //Item Properties
    public Item_ScriptableObj item;
    public int quantity;

    //Slot state machine
    private enum SLOT_ACTION_STATES { IDLE, SELECT, UNSELECT };
    private SLOT_ACTION_STATES state;
    private bool select;
    private bool unselect;

    public slotTag myTag;


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

    public void SetItem_A(Tuple<Item_ScriptableObj,int> new_item)
    {
        item = new_item.Item1;
        quantity = new_item.Item2;

        //Reset visuals
        Image child_image = transform.GetChild(0).GetComponent<Image>();
        child_image.sprite = item.getSprite();
        child_image.SetNativeSize();
        child_image.color = item.getSprite() ? new Color(1, 1, 1, 1) : new Color(1,1,1,0);
        //Get quantity as well
    }

    public Item_ScriptableObj GetItem() {
        return item;
    }
}