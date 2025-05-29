using UnityEngine;
using UnityEngine.Tilemaps;

public class Door : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Tilemap building_top;
    [SerializeField] private bool open = true;
    protected override void onInteract(ref Player player)
    {
        this.GetComponent<BoxCollider>().enabled = false;
        open = !open;
        itemAnimator.SetBool("Open", open);
        //building_top.gameObject.SetActive(false);
    }

    protected override void ExplosionVFX()
    {
        Instantiate(_explosion, transform.position, Quaternion.identity);
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
