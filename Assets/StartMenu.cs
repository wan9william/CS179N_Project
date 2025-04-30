using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public void OnStartButtonPressed()
    {
        Debug.Log("Start Game pressed!");
    }

    public void OnQuitButtonPressed()
    {
        Debug.Log("Quit Game pressed!");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}