using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuFixed : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Starting...");
        SceneManager.LoadScene(1); // change to actual scene name
    }

    public void QuitGame()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
}
