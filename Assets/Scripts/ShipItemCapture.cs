using System.Collections.Generic;
using UnityEngine;

public class ShipItemCapture : MonoBehaviour
{
    // Store items and their relative position to the ship
    private Dictionary<GameObject, Vector3> relativeOffsets = new();

    public void CaptureItems()
    {
        relativeOffsets.Clear();

        GameObject ship = GameObject.FindGameObjectWithTag("Ship");
        if (ship == null) return;

        Vector3 shipPos = ship.transform.position;
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        Vector2 center = (Vector2)transform.position + box.offset;
        Vector2 size = box.size;

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f);

        foreach (Collider2D col in hits)
        {
            if (col.CompareTag("Interactable") && !relativeOffsets.ContainsKey(col.gameObject))
            {
                DontDestroyOnLoad(col.gameObject);
                Vector3 offset = col.transform.position - shipPos;
                relativeOffsets[col.gameObject] = offset;

                Debug.Log($"[ShipItemCapture] Marked {col.name} to persist (relative offset {offset})");
            }
        }
    }

    public void RestoreItemPositions()
    {
        GameObject ship = GameObject.FindGameObjectWithTag("Ship");
        if (ship == null) return;

        Vector3 shipPos = ship.transform.position;

        foreach (var kvp in relativeOffsets)
        {
            GameObject item = kvp.Key;
            Vector3 offset = kvp.Value;

            if (item != null)
            {
                item.transform.position = shipPos + offset;
                Debug.Log($"[ShipItemCapture] Restored {item.name} to {item.transform.position}");
            }
        }
    }
}
