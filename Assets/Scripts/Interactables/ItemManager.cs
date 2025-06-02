using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] public List<Item_ScriptableObj> allItemsList = new List<Item_ScriptableObj>();
    [SerializeField] public List<GameObject> allObjectsList = new List<GameObject>();
    [SerializeField][Range(0f, 1f)] public float itemChancePerTile = 1f;
    [SerializeField][Range(0f, 1f)] public float objectChancePerTile = 1f;
    private List<GameObject> items = new List<GameObject>();
    [SerializeField] private Vector2 offset = Vector2.zero;
    [SerializeField][Range(0f, 1f)] private float randomOffset = 0.2f;
    public void InstantiateObject(Vector3 spawnPosition, Transform parent)
    {
        if (UnityEngine.Random.value <= objectChancePerTile)
        {
            GameObject droppedItem = GetDroppedObject();
            if (droppedItem != null)
            {
                GameObject itemGameObject = Instantiate(droppedItem, parent);
                itemGameObject.transform.localPosition = spawnPosition;
                items.Add(itemGameObject);
            }
        }
    }

    public void InstantiateLoot(Vector3 spawnPosition, Transform parent)
    {
        if (UnityEngine.Random.value <= itemChancePerTile)
        {
            Item_ScriptableObj droppedItem = GetDroppedItem();
            if (droppedItem != null)
            {
                GameObject itemGameObject = Instantiate(droppedItem.getResourcePrefab(), parent);
                itemGameObject.transform.localPosition = spawnPosition + new Vector3(offset.x + Random.Range(-randomOffset, randomOffset), offset.y + Random.Range(-randomOffset, randomOffset));
                items.Add(itemGameObject);
            }
        }
    }

    //Loot Table for Dropped Item
    private Item_ScriptableObj GetDroppedItem()
    {
        int randomNumber = UnityEngine.Random.Range(1, 101); //Get a random number between 1-100
        List<Item_ScriptableObj> possibleItems = new List<Item_ScriptableObj>();
        foreach (Item_ScriptableObj item in allItemsList)
        {
            if (randomNumber <= item.dropChance)
            {
                possibleItems.Add(item);
            }
        }
        if (possibleItems.Count > 0)
        {
            Item_ScriptableObj droppedItem = possibleItems[UnityEngine.Random.Range(0, possibleItems.Count)];
            return droppedItem;
        }
        return null;
    }

    private GameObject GetDroppedObject()
    {
        List<GameObject> possibleItems = new List<GameObject>();
        foreach (GameObject item in allObjectsList)
        {
            possibleItems.Add(item);
        }
        if (possibleItems.Count > 0)
        {
            GameObject droppedItem = possibleItems[UnityEngine.Random.Range(0, possibleItems.Count)];
            return droppedItem;
        }
        return null;
    }

    public void Clear()
    {
        if(items.Count > 0)
        {
            foreach (GameObject item in items)
            {
                DestroyImmediate(item);
            }
        }
    }

}
