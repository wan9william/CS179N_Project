using UnityEngine;

public class Resource_Class : MonoBehaviour
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
        //Add FX for pickup. Perhaps a particle system variable
    }

    public void NoGlow() { transform.GetComponent<Renderer>().material = _mat; }

}
