using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Door : Interactable
{
    [SerializeField] Animator doorAnimator;
    [SerializeField] bool doorEnabled = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void onInteract(ref Player player)
    {
        player.SetFindInteract(true);
        player.SetInteract(null);
        doorEnabled = !doorEnabled;
        doorAnimator.SetBool("Open", doorEnabled);
        gameObject.SetActive(false);
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

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (Input.GetKeyDown(KeyCode.E)&& collision.tag=="Player")
        {
            doorEnabled = !doorEnabled;
            doorAnimator.SetBool("Open", doorEnabled);
        }
    }

}
