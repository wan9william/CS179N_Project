using UnityEngine;

public class DestructibleProp : Interactable
{
    private Collider2D col;
    private SpriteRenderer spriteRenderer;

    protected override void Initialize()
    {
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void ExplosionVFX()
    {
        if (_explosion != null)
        {
            Instantiate(_explosion, transform.position, Quaternion.identity);
        }
    }

    protected override void Tick()
    {
        // No ticking behavior for destructible props
    }

    protected override void onInteract(ref Player player)
    {
        // This object cannot be picked up or interacted with
        player.SetFindInteract(false);
        player.SetInteract(null);
    }
}
