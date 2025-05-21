using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlanetSelector : MonoBehaviour
{
    public Button planet1Button;
    public Button planet2Button;
    public Button launchButton;

    private string selectedScene = null;

    void Start()
    {
        launchButton.interactable = false;

        planet1Button.onClick.AddListener(() => SelectPlanet("ProcGen Test Scene", planet1Button));
        planet2Button.onClick.AddListener(() => SelectPlanet("Planet2Scene", planet2Button));

        launchButton.onClick.AddListener(() => {
            if (!string.IsNullOrEmpty(selectedScene))
            {
                SceneManager.LoadScene(selectedScene);
            }
        });
    }

    void SelectPlanet(string sceneName, Button clickedButton)
    {
        selectedScene = sceneName;
        launchButton.interactable = true;

        // Optional: visual highlight
        planet1Button.image.color = Color.white;
        planet2Button.image.color = Color.white;
        clickedButton.image.color = Color.green;
    }
}
