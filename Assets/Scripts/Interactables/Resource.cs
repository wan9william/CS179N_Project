using UnityEngine;
using UnityEngine.Tilemaps;

public class Resource : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void onInteract()
    {
        
    }

    protected override void ExplosionVFX()
    {
        //Inefficient, replace later
        Instantiate(_explosion,transform.position,Quaternion.identity);
    }
}
