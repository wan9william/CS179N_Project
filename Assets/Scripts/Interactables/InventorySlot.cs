using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework.Constraints;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public InventoryItem myItem { get; set;}
    [SerializeField] private Inventory inven;
    [SerializeField] private Inventory inven2;

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

    Item_ScriptableObj empty;
    public int storedAmmo = -1;

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
            quantityText.raycastTarget = false;
            textObj.transform.localScale = Vector3.one;
            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = new Vector2(2, 2);
            rect.offsetMax = new Vector2(-2, -2);
        }

        empty = Resources.Load("Empty") as Item_ScriptableObj;

        myItem = transform.GetChild(0).GetComponent<InventoryItem>();

        UpdateQuantityDisplay();
    }

    public void SetInven(Inventory _inventory) { 
        inven = _inventory; 
        transform.GetChild(0).transform.GetComponent<InventoryItem>().SetInven(inven);
    }

    public Inventory GetInven() {return inven; }

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
        if (eventData.button != PointerEventData.InputButton.Right || inven.carriedItem == null) return;

        if(myItem != null)
        {
            // if more than 1 item of that type exists in a slot and shift is pressed, split the stack of that item
            if((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift)) && quantity > 1)
            {
                int split = quantity / 2;
                quantity = quantity - split;
                UpdateQuantityDisplay();
                // creating a new carried item with the quantity of split result
            }
        }
        /*
        if(myTag != slotTag.None && inven.carriedItem.myItem.itemTag != myTag)
        {
            return;
        }*/
        SetItem(inven.carriedItem);
    }

    public void SetItem(InventoryItem item)
    {
        // If already have an item and it's the same type, add to the quantity of that existing item
        Debug.Log(myItem);

        //If same item
        if (myItem.myItem == item.myItem)
        {
            int totalQuantity = myItem.GetQuantity() + item.GetQuantity();
            int maxStack = inven.GetMaxStackSize();

            if (totalQuantity <= maxStack)
            {

                //myItem.SetItem(totalQuantity,item.myItem);
                SetItem_A(new Tuple<Item_ScriptableObj, int>(item.myItem, totalQuantity));


                inven.carriedItem = null;
                //send item back
                item.transform.parent = item.parentAfterDrag;
                item.transform.SetAsFirstSibling();
                item.transform.localPosition = Vector3.zero;
                return;
            }
        }
        else if (myItem.myItem.getID() == 0) //If empty
        {
            int totalQuantity = item.GetQuantity();
            int maxStack = inven.GetMaxStackSize();

            if (totalQuantity <= maxStack)
            {

                SetItem_A(new Tuple<Item_ScriptableObj, int>(item.myItem, totalQuantity));

                inven.carriedItem = null;
                //send item back
                item.transform.parent = item.parentAfterDrag;
                item.transform.localPosition = Vector3.zero;
                item.transform.SetAsFirstSibling();

                Item_ScriptableObj empty = Resources.Load("Empty") as Item_ScriptableObj;

                item.transform.parent.GetComponent<InventorySlot>().SetItem_A(new Tuple<Item_ScriptableObj, int>(empty, 0));
                return;
            }
        }
        else { //If another item exists in the box

            int totalQuantity = item.GetQuantity();
            int maxStack = inven.GetMaxStackSize();

            if (totalQuantity <= maxStack)
            {

                

                inven.carriedItem = null;
                //send item back
                item.transform.parent = item.parentAfterDrag;
                item.transform.localPosition = Vector3.zero;
                item.transform.SetAsFirstSibling();

                Item_ScriptableObj replacement_resource = myItem.myItem;
                Item_ScriptableObj current_resource = item.myItem; //copies item. There is a better way

                //Set original slot with current item (first part of swap)
                item.transform.parent.GetComponent<InventorySlot>().SetItem_A(new Tuple<Item_ScriptableObj, int>(replacement_resource, quantity));

                //Set this slot to the original slot item (second part of swap)
                SetItem_A(new Tuple<Item_ScriptableObj, int>(current_resource, totalQuantity));
                return;
            }
        }

        item.transform.parent = item.parentAfterDrag;
        item.transform.localPosition = Vector3.zero;
        item.transform.SetAsFirstSibling();
        // Regular item placement
        inven.carriedItem = null;
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
        myItem.myItem= item;
        //Get quantity as well
        
        // Check if this is a weapon prefab
        Weapon weapon = item.getPrefab()?.GetComponentInChildren<Weapon>();
        if (weapon != null)
        {
            if (storedAmmo < 0)  // Only initialize if not set already
                storedAmmo = weapon.GetMagazineSize();  // full mag by default
        }
        else
        {
            storedAmmo = -1;  // Reset if non-weapon
        }
            UpdateQuantityDisplay();
    }

    public void UpdateItem() {

        UpdateQuantityDisplay();

        if (quantity > 0) return; //if quantity is not less than 0, no need to update anything else

        item = empty;
        quantity = 0;

        Image child_image = transform.GetChild(0).GetComponent<Image>();
        child_image.sprite = item.getSprite();
        child_image.color = item.getSprite() ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);
        myItem.myItem = item;

        UpdateQuantityDisplay(); // 💡 Ensure display updates after resetting
    }

    public void UpdateQuantityDisplay()
    {
    if (quantityText == null) return;

    // If this slot holds a weapon with stored ammo
    if (storedAmmo >= 0 && item != null && item.getPrefab() != null)
    {
        Weapon weapon = item.getPrefab().GetComponentInChildren<Weapon>();
        if (weapon != null)
        {
            quantityText.SetText($"{storedAmmo} / {weapon.GetMagazineSize()}");
            return;
        }
    }

    // Default behavior for stackable items
    quantityText.SetText(quantity > 1 ? quantity.ToString() : "");
    }

    public void UpdateAmmoDisplay(int current, int max)
    {
        if (quantityText != null)
        {
            quantityText.SetText($"{current} / {max}");
        }
    }

    public Item_ScriptableObj GetItem()
    {
        return item;
    }

    public GameObject getFab()
    {
        return item.getPrefab();
    }
    public int GetQuantity()
    {
        return quantity;
    }

    public void SetQuantity(int q) { quantity = q; }

    // Called when an item is dropped onto slot and handles stacking of same items and placement of new items
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log(inven.carriedItem);

        if (inven.carriedItem == null) return;
        //This is being set to NULL

        // Regular item placement
        Debug.Log("Normal placement!");
        SetItem(inven.carriedItem);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        Debug.Log($"CarriedItem is {inven.carriedItem}");
        // Update position to follow cursor
        //transform.position = Input.mousePosition;
    }

}