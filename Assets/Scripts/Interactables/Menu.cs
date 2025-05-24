using UnityEngine;
using UnityEngine.Tilemaps;

public class Menu : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Canvas menu;
    protected override void onInteract(ref Player player)
    {
        menu.gameObject.SetActive(false);
    }

    protected override void ExplosionVFX()
    {
        return;
    }

    protected override void Tick()
    {
        return;
    }

    protected override void Initialize()
    {
        return;
    }
}
