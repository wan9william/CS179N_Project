using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Allows us to use UI Image
public class Dmg_flash : MonoBehaviour
{
    // This is the red UI Image that will flash on the screen.
    [SerializeField] private Image flashImage;

    // How long the red flash should stay fully visible before fading out
    [SerializeField] private float flashDuration = 0.1f;

    // How quickly the red fades away
    [SerializeField] private float fadeSpeed = 5f;

    // This is the final color we want (fully transparent red)
    private Color clearRed = new Color(1, 0, 0, 0); // R, G, B, A

    // Runs once when the game starts
    void Start()
    {
        // Make sure the flash image starts off invisible
        if (flashImage != null)
        {
            flashImage.color = clearRed;
        }
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         Flash(); // this should make the screen flash red
    //         Debug.Log("Flash triggered!");
    //     }
    // }


    // This method is called when the player takes damage
    public void Flash()
    {
        Debug.Log("Flash called!"); // 🛠 Add this for testing
        // Start the flashing routine if the image is set
        if (flashImage != null)
        {
            StartCoroutine(FlashRoutine());
        }
    }

    // This slowly shows and then fades the flash effect
    private IEnumerator FlashRoutine()
    {
        // Set the image to be visible red (alpha = 0.5 for semi-transparent)
        flashImage.color = new Color(1, 0, 0, 0.5f);

        // Wait for a short moment before fading out
        yield return new WaitForSeconds(flashDuration);

        // Gradually fade the image back to invisible
        while (flashImage.color.a > 0f) // While alpha is greater than 0
        {
            // Lerp moves the color closer to fully transparent each frame
            flashImage.color = Color.Lerp(flashImage.color, clearRed, Time.deltaTime * fadeSpeed);

            // Wait for the next frame before continuing
            yield return null;
        }

        // Make sure it's completely clear at the end
        flashImage.color = clearRed;
    }
}
