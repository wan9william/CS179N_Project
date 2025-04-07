using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private Material _mat;
    [SerializeField] private Material _glowMat;

    public void Start()
    {
        _mat = Resources.Load("Materials/Sprite-Lit") as Material;
        _glowMat = Resources.Load("Materials/Sprite_Outline") as Material;
    }

    public void Glow() { transform.GetComponent<Renderer>().material = _glowMat; }

    public void Destroy() { 
        transform.gameObject.SetActive(false);

        //call the interact function for a given object
        onInteract();

        //Add FX for pickup. Perhaps a particle system variable


        //Could add area for dependency. For example, a door needs to let its parent building know to disappear

        //This could be done through a child class of an abstract resource class
    }

    public void NoGlow() { transform.GetComponent<Renderer>().material = _mat; }

    protected abstract void onInteract();

}
