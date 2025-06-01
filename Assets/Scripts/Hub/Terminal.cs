using UnityEngine;
using UnityEngine.SceneManagement;

public class Terminal : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private string targetScene = "PlanetScene"; // Set this in the Inspector


    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // Ensures Terminal survives scene load
    }
    
    public void ClickX()
    {
        transform.gameObject.SetActive(false);
        player.setPaused(false);
    }

    public void StartMission()
    {
        // Disable UI and unpause
        ClickX();
        //capture the items inside the ship
        GameObject ship = GameObject.FindGameObjectWithTag("Ship");
        ShipItemCapture capture = ship?.GetComponentInChildren<ShipItemCapture>();
        capture?.CaptureItems();

        // Load the target scene and reposition the ship on arrival
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(targetScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject landingPad = GameObject.FindGameObjectWithTag("LandingPad");
        GameObject ship = GameObject.FindGameObjectWithTag("Ship");

        if (landingPad != null && ship != null)
        {
            ship.transform.position = landingPad.transform.position;
        }

        //Restore item positions
        ShipItemCapture capture = ship?.GetComponentInChildren<ShipItemCapture>();
        if (capture != null)
        {
            capture.RestoreItemPositions();
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
} 

