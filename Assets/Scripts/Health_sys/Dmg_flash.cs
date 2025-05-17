using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dmg_flash : MonoBehaviour
{
    [SerializeField] private Image flashImage;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float fadeSpeed = 5f;

    private Color clearRed = new Color(1, 0, 0, 0); // Transparent red
    private Coroutine flashCoroutine;

    void Start()
    {
        if (flashImage != null)
        {
            flashImage.color = clearRed;
        }
    }

    public void Flash()
    {
        if (flashImage == null)
            return;

       
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        flashImage.color = new Color(1, 0, 0, 0.5f); // Semi-transparent red

        yield return new WaitForSeconds(flashDuration);

        while (flashImage.color.a > 0f)
        {
            flashImage.color = Color.Lerp(flashImage.color, clearRed, Time.deltaTime * fadeSpeed);
            yield return null;
        }

        flashImage.color = clearRed;
        flashCoroutine = null;
    }
}
