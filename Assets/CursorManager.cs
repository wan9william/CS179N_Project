using UnityEngine;

public class CursorManager : MonoBehaviour
{

    [Header("Mouse Settings")]
    [SerializeField] private Texture2D cursorTexture;
    //Variable to center the cursor to the middle of the sprite
    private Vector2 cursorHotspot;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cursorHotspot = new Vector2(cursorTexture.width/2, cursorTexture.height/2);
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
