using UnityEngine;

public class ReticleFollow : MonoBehaviour
{
    private RectTransform _rect;

    void Start()
    {
        _rect = GetComponent<RectTransform>();
        Cursor.visible = false; // Hide the default system cursor
    }

    void Update()
    {
        _rect.position = Input.mousePosition;
    }
}