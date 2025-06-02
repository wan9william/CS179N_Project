using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Door : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void onInteract(ref Player player)
    {
        player.SetFindInteract(true);
        player.SetInteract(null);
        gameObject.SetActive(false);
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

}
