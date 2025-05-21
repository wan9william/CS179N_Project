using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlanetTerminal : Interactable
{
    [SerializeField] private GameObject planetSelectionUI;
    private string selectedScene;

    /*
    void Start()
    {
        if (planetSelectionUI != null)
            planetSelectionUI.SetActive(false);
    }*/

    protected override void Initialize()
    {
        return;
    }

    protected override void onInteract(ref Player player)
    {
        player.SetFindInteract(true);
        player.SetInteract(null);

        if (planetSelectionUI != null)
        {
            planetSelectionUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void SelectPlanet(string sceneName)
    {
        selectedScene = sceneName;
        Debug.Log($"Selected scene: {sceneName}");
    }

    public void ConfirmTeleport()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(selectedScene);
    }

    protected override void ExplosionVFX() { } // not needed
    protected override void Tick() { } // not needed
}
