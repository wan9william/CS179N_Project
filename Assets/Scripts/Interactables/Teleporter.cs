using UnityEngine;
using UnityEngine.Tilemaps;

public class Teleporter : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Canvas menu;
    [SerializeField] private Game_Event_Manager game_event_manager;
    protected override void onInteract(ref Player player)
    {
        menu.gameObject.SetActive(true);
        player.setPaused(true);
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
