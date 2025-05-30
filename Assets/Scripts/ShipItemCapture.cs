using UnityEngine;

public class ShipItemCapture : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            Transform ship = GameObject.FindGameObjectWithTag("Ship")?.transform;
            if (ship != null && other.transform.parent != ship)
            {
                other.transform.SetParent(ship);
                Debug.Log($"[Ship] Captured interactable item: {other.name}");
            }
        }
    }
}
