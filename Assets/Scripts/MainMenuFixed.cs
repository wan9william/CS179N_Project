using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuFixed : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene"); // change to actual scene name
    }

    public void QuitGame()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
}
