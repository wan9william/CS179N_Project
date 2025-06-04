using UnityEngine;
using UnityEngine.Tilemaps;

public class Door : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Tilemap building_top;
    [SerializeField] private GameObject collider;
    [SerializeField] private AudioClip openClip;
    private AudioSource audioSource;
    [SerializeField] private bool open = false;
    protected override void onInteract(ref Player player) {

        //if open, close, otherwise open
        bool currentState = itemAnimator.GetBool("Open");
        
        open = !currentState;
        itemAnimator.SetBool("Open", !currentState);


        if(!audioSource) audioSource = GetComponent<AudioSource>();
        if (open)
        {
            audioSource.clip = openClip;

            audioSource.Play();
        }

        //this.gameObject.GetComponent<BoxCollider2D>().enabled = !currentState;
        collider.SetActive(currentState);
    }

    protected override void ExplosionVFX()
    {
        throw new System.NotImplementedException();
    }

    protected override void Tick()
    {
        return;
    }

    protected override void Initialize()
    {
        return;
    }

    public override bool Hit(float damage)
    {
        if (open) return false;
        itemAnimator.SetTrigger("Hit");
        health -= damage;

        return true;
    }

}
