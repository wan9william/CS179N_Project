using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private Material _mat;
    [SerializeField] private Material _glowMat;
    [SerializeField] protected GameObject _explosion;
    [SerializeField] private float health = 10;
    [SerializeField] protected Animator itemAnimator;


    //FOR TESTING PURPOSES
    public bool damage;
    public void Update()
    {
        Tick();
        CheckHealth();
    }
    /// ///////////////////////////
    public void Start()
    {
        itemAnimator = this.transform.GetComponent<Animator>();
        _mat = Resources.Load("Materials/Sprite-Lit") as Material;
        _glowMat = Resources.Load("Materials/Sprite_Outline") as Material;

        _explosion = Resources.Load("Explosion_0") as GameObject;

        Initialize();
    }

    public void Glow() { transform.GetComponent<Renderer>().material = _glowMat; }

    protected abstract void Initialize();

    public void Destroy(ref Player player) { 
        

        //call the interact function for a given object
        onInteract(ref player);
        

        //If the object is exhaustable

        //Add FX for pickup. Perhaps a particle system variable


        //Could add area for dependency. For example, a door needs to let its parent building know to disappear

        //This could be done through a child class of an abstract resource class
    }
    public void Hit(float damage) {
        itemAnimator.SetTrigger("Hit");
        health -= damage;
    }

    private void CheckHealth() { if (health < 0) SelfDestruct(); }

    public void SelfDestruct() {
        transform.gameObject.SetActive(false);
        ExplosionVFX();
    }

    public void NoGlow() { transform.GetComponent<Renderer>().material = _mat; }

    protected abstract void onInteract(ref Player player);

    protected abstract void ExplosionVFX();

    protected abstract void Tick();

}
