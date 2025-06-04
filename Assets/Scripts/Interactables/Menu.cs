using UnityEngine;
using UnityEngine.Tilemaps;

public class Menu : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Terminal terminal;
    [SerializeField] Terminal.UI_Elements ui_element;
    protected override void onInteract(ref Player player)
    {
        if(!terminal) terminal = GameObject.FindWithTag("UI_Manager").GetComponent<Terminal>();
        terminal.Show();
        player.setPaused(true);
        player.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0,0);
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
