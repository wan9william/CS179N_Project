using UnityEngine;

public class Resource_Class : MonoBehaviour
{
    [SerializeField] private Material _mat;
    [SerializeField] private Material _glowMat;

    public void Glow() { transform.GetComponent<Renderer>().material = _glowMat; }

    public void NoGlow() { transform.GetComponent<Renderer>().material = _mat; }

}
