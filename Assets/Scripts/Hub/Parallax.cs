using UnityEngine;

public class Parallax : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int layer;
    [SerializeField] private float speed;
    [SerializeField] private float leftBounds;
    [SerializeField] private float rightBounds;
    void Update()
    {
        transform.position -= new Vector3(1,0,0) * layer * Time.deltaTime * speed;
        if(transform.position.x <= leftBounds) transform.position = new Vector3(rightBounds,transform.position.y,transform.position.z);
    }
}
