using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private string savePath;

    void Awake()
    {
        if (Instance == null) { 
            Instance = this;
            savePath = Application.persistentDataPath + "/save_file.json";
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void SaveGame(Player player, Inventory inventory)
    {
        SaveData data = new SaveData();

        // Player data
        data.playerData = new PlayerData {
            position = new float[] { player.transform.position.x, player.transform.position.y },
            money = player.GetMoney()
        };

        // Inventory data
        data.inventorySlots = new List<InventorySlotData>();
        foreach (var slot in inventory.getInventorySlots())
        {
            var item = slot.GetItem();
            if (item != null)
            {
                data.inventorySlots.Add(new InventorySlotData {
                    itemID = item.getID().ToString(),
                    quantity = slot.GetQuantity(),
                    storedAmmo = slot.storedAmmo
                });
            }
        }

        // Ground items
        GameObject[] droppedItems = GameObject.FindGameObjectsWithTag("Interactable");
        data.droppedItems = new List<DroppedItemData>();
        foreach (GameObject obj in droppedItems)
        {
            var res = obj.GetComponent<Resource>();
            if (res != null && !res.IsNatural()) {
                data.droppedItems.Add(new DroppedItemData {
                    itemID = res.GetResource().getID().ToString(),
                    quantity = res.GetQuantity(),
                    position = new float[] { obj.transform.position.x, obj.transform.position.y }
                });
            }
        }

        string json = JsonUtility.ToJson(data, true);
        Debug.Log("Saved game to: " + Application.persistentDataPath);
        File.WriteAllText(savePath, json);
        Debug.Log("Game Saved to: " + savePath);
    }

    public SaveData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<SaveData>(json);
        }

        Debug.LogWarning("No save file found!");
        return null;
    }
}
