using UnityEngine;

public class Explosion : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnFinish() {
        transform.gameObject.SetActive(false);
    }
}
