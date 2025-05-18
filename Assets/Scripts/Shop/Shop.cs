using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shop : MonoBehaviour
{

    private const int MAX_ITEMS = 20; //20 is a random constant that seems appropriate
    [SerializeField] private List<GameObject> objects = new List<GameObject>(MAX_ITEMS);
    [SerializeField] private UnityEngine.Object[] catalog;
    [SerializeField] private int index = 0;
    [SerializeField] GameObject shop_item;
    enum SHOP_STATES { SHOP_IDLE, SHOP_SELL, SHOP_ROTATE, SHOP_BUY};
    private SHOP_STATES state;


    [Header("Button")]
    [SerializeField] private Button_Interact sell_button;
    [SerializeField] private Button_Interact rotate_left;
    [SerializeField] private Button_Interact rotate_right;
    [SerializeField] private Button_Interact buy;

    private bool rotated = false;

    private int rotateDir = 0;
    private Player recipient;
    void Start()
    {
        //Need some assertion that all buttons are not null

        //Load all catalog items
        catalog = Resources.LoadAll("Prefabs/Interactables", typeof(GameObject));
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case SHOP_STATES.SHOP_IDLE:
                if (sell_button.GetPressed()) {
                    state = SHOP_STATES.SHOP_SELL;
                    recipient = sell_button.GetPlayer();
                    break;
                }
                if ((rotate_left.GetPressed() || rotate_right.GetPressed()) && !rotated)
                {
                    rotateDir = rotate_right.GetPressed() ? 1 : -1; //1 is reserved for right, -1 for left
                    state = SHOP_STATES.SHOP_ROTATE;
                    rotated = true;
                    break;
                }
                else if ((!rotate_left.GetPressed() && !rotate_right.GetPressed())) { rotated = false; }
                if (buy.GetPressed()) {
                    state = SHOP_STATES.SHOP_BUY;
                    recipient = buy.GetPlayer();
                    break;
                }
                break;
            case SHOP_STATES.SHOP_SELL:

                SellItems();
                state = SHOP_STATES.SHOP_IDLE;
                break;

            case SHOP_STATES.SHOP_ROTATE:
                index += rotateDir;
                if(index < 0) index = catalog.Length - 1;
                if (index > catalog.Length-1) index = 0;

                shop_item.GetComponent<SpriteRenderer>().sprite = catalog[index].GetComponent<SpriteRenderer>().sprite;
                state = SHOP_STATES.SHOP_IDLE;
                break;

            case SHOP_STATES.SHOP_BUY:
                BuyItem();
                state = SHOP_STATES.SHOP_IDLE;
                break;

            default:
                break;

        }
    }

    private void SellItems() {
        //this is a monstrous lambda statement. Any way to simplify this to be better?
        if (objects.Count == 0) return;
        objects.ForEach(obj => { 
            Resource resource = obj.GetComponent<Resource>();

            if (!resource) return;

            //Add the value to the player's wallet
            recipient.GetMoney() += resource.GetResource().getValue();

            //Destroy the gameObject
            resource.Hit(float.MaxValue);

        });

        //Clear the list afterwards
        objects.Clear();
    }

    private void BuyItem() {
        Item_ScriptableObj _resource = catalog[index].GetComponent<Resource>().GetResource();
        int price = _resource.getValue();

        if (recipient.GetMoney() < catalog[index].GetComponent<Resource>().GetResource().getValue()) return;
        recipient.getInventory().addItem(new Tuple<Item_ScriptableObj, int>(_resource, 1));
        recipient.SetMoney(recipient.GetMoney() - price);
        shop_item.GetComponent<SpriteRenderer>().sprite = null;

    }

    //Adds interactable gameobject when entering the shop area
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Triggered!");
        if(collision.transform.tag == "Interactable") objects.Add(collision.gameObject);

    }


    //Removes interactable gameobject when leaving the shop area
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag != "Interactable") return;

        for (int i = 0; i < objects.Count; ++i) {
            if (objects[i] == collision.gameObject) objects.RemoveAt(i);
        }
    }
}
