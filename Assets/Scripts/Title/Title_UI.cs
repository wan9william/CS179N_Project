using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title_UI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //[SerializeField] SceneAsset _hubworld;

    public void ClickStart()
    {
        SceneManager.LoadScene("HubWorld 2");
    }

    public void ClickExit()
    {
        Application.Quit();
    }
}
