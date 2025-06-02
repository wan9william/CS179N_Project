using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Resource : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Item_ScriptableObj _resource;
    [SerializeField] int quantity;
    [SerializeField] bool natural = true;
    protected override void onInteract(ref Player player)
    {
        player.SetFindInteract(true);
        player.SetInteract(null);

        quantity = player.getInventory().addItem(new Tuple<Item_ScriptableObj,int>(_resource,quantity)); //sets the quantity to the leftover quantity after adding to the inventory

        if(quantity <= 0) transform.gameObject.SetActive(false);
    }

    protected override void Initialize() {

        quantity = transform.name.Contains("Ammo") && natural ? 20 : 1;
    }

    //Meant to differentiate between items dropped and naturally spawn items
    public void SetNatural(bool nat) {
        natural = nat;
    }

    protected override void ExplosionVFX()
    {
        //Inefficient, replace later
        Instantiate(_explosion, transform.position, Quaternion.identity);
    }
    protected override void Tick()
    {
        //Not implemented
        return;
    }

    public Item_ScriptableObj GetResource() { return _resource; }

    public void SetResource(Item_ScriptableObj resource) { _resource = resource; }
}
