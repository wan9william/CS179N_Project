using UnityEngine;

public abstract class Equippable : MonoBehaviour
{
    //This is a wrapper for all equippable items so that we can use them
    public abstract void Use(ref Player player);
    public AudioSource audioSource;
    
}
