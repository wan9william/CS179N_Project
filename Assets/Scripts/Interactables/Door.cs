using UnityEngine;
using UnityEngine.Tilemaps;

public class Door : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Tilemap building_top;
    protected override void onInteract() {
        building_top.gameObject.SetActive(false);
    }
}
