using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private Material _mat;
    [SerializeField] private Material _glowMat;
    [SerializeField] protected GameObject _explosion;
    [SerializeField] private float health = 10;


    //FOR TESTING PURPOSES
    public bool damage;
    public void Update()
    {
        if (damage) {
            damage = false;
            Hit(5);
        }
    }
    /// ///////////////////////////
    public void Start()
    {
        _mat = Resources.Load("Materials/Sprite-Lit") as Material;
        _glowMat = Resources.Load("Materials/Sprite_Outline") as Material;

        _explosion = Resources.Load("Explosion_0") as GameObject;
    }

    public void Glow() { transform.GetComponent<Renderer>().material = _glowMat; }

    public void Destroy() { 
        

        //call the interact function for a given object
        onInteract();
        transform.gameObject.SetActive(false);

        //Add FX for pickup. Perhaps a particle system variable


        //Could add area for dependency. For example, a door needs to let its parent building know to disappear

        //This could be done through a child class of an abstract resource class
    }
    public void Hit(float damage) { 
        health -= damage;
        if(health < 0) SelfDestruct();
    }

    public void SelfDestruct() {
        transform.gameObject.SetActive(false);
        ExplosionVFX();
    }

    public void NoGlow() { transform.GetComponent<Renderer>().material = _mat; }

    protected abstract void onInteract();

    protected abstract void ExplosionVFX();

}
