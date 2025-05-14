using UnityEngine;
using UnityEngine.Tilemaps;

public class Door : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Tilemap building_top;
    protected override void onInteract(ref Player player) {
        building_top.gameObject.SetActive(false);
    }

    protected override void ExplosionVFX()
    {
        throw new System.NotImplementedException();
    }

    protected override void Tick()
    {
        return;
    }
}
