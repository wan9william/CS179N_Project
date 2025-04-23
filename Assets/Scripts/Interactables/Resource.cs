using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Resource : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Item_ScriptableObj _resource;
    protected override void onInteract(ref Player player)
    {
        player.SetFindInteract(true);
        player.SetInteract(null);

        player.getInventory().addItem(new Tuple<Item_ScriptableObj,int>(_resource,1));
    }

    protected override void ExplosionVFX()
    {
        //Inefficient, replace later
        Instantiate(_explosion, transform.position, Quaternion.identity);
    }

    public Item_ScriptableObj GetResource() { return _resource; }
}
