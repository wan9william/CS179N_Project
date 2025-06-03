using UnityEngine;
using System;

public class SceneInitializer : MonoBehaviour
{
    void Start()
    {
        SaveData data = SaveManager.Instance.LoadGame();
        if (data == null) return;

        // Move player
        Player.Singleton.transform.position = new Vector3(data.playerData.position[0], data.playerData.position[1], 0);
        Player.Singleton.SetMoney(data.playerData.money);

        // Load inventory
        Inventory inventory = Player.Singleton.getInventory();
        InventorySlot[] slots = inventory.getInventorySlots();

        for (int i = 0; i < data.inventorySlots.Count && i < slots.Length; i++)
        {
            var slotData = data.inventorySlots[i];
            var itemSO = FindItemByID(slotData.itemID);
            if (itemSO != null)
            {
                slots[i].SetItem_A(new Tuple<Item_ScriptableObj, int>(itemSO, slotData.quantity));
                slots[i].storedAmmo = slotData.storedAmmo;
                slots[i].UpdateQuantityDisplay();
            }
        }

        // Spawn dropped items
        foreach (var drop in data.droppedItems)
        {
            var itemSO = FindItemByID(drop.itemID);
            if (itemSO != null)
            {
                GameObject prefab = itemSO.getResourcePrefab();
                if (prefab != null)
                {
                    GameObject spawned = GameObject.Instantiate(prefab, new Vector3(drop.position[0], drop.position[1], 0), Quaternion.identity);
                    var res = spawned.GetComponent<Resource>();
                    if (res != null)
                    {
                        res.SetQuantity(drop.quantity);
                        res.SetNatural(false); // Mark it as player-dropped
                    }
                }
            }
        }
    }

    private Item_ScriptableObj FindItemByID(string id)
    {
        foreach (var item in Resources.LoadAll<Item_ScriptableObj>("Items"))
        {
            if (item.getID().ToString() == id)
                return item;
        }
        return null;
    }
}
