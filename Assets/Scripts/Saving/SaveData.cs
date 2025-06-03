using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData {
    public PlayerData playerData;
    public List<InventorySlotData> inventorySlots;
    public List<DroppedItemData> droppedItems;
}

[Serializable]
public class PlayerData {
    public float[] position; // [x, y]
    public int money;
}

[Serializable]
public class InventorySlotData {
    public string itemID;
    public int quantity;
    public int storedAmmo;
}

[Serializable]
public class DroppedItemData {
    public string itemID;
    public int quantity;
    public float[] position; // [x, y]
}
