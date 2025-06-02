using UnityEngine;
using UnityEngine.SceneManagement;

public class Terminal : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private string targetScene = "PlanetScene";

    private Vector3 localShipPosition;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // Keep this Terminal alive between scenes
    }

    public void ClickX()
    {
        gameObject.SetActive(false);
        player.setPaused(false);
    }

    public void StartMission()
    {
        ClickX();

        GameObject ship = GameObject.FindGameObjectWithTag("Ship");
        GameObject inventoryObject = GameObject.Find("Inventory");
        
        if (ship != null && player != null && inventoryObject != null)
        {
            //Save position relative to ship
            localShipPosition = ship.transform.InverseTransformPoint(player.transform.position);

            //Persist player
            DontDestroyOnLoad(player.gameObject);

            //Persist inventory UI
            DontDestroyOnLoad(inventoryObject);

            //Persist ship items
            ship.GetComponentInChildren<ShipItemCapture>()?.CaptureItems();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(targetScene);
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject ship = GameObject.FindGameObjectWithTag("Ship");
        GameObject landingPad = GameObject.FindGameObjectWithTag("LandingPad");

        if (ship != null && landingPad != null)
        {
            ship.transform.position = landingPad.transform.position;

            //Reposition player back into the ship
            Player newPlayer = FindObjectOfType<Player>();
            if (newPlayer != null)
            {
                newPlayer.transform.position = ship.transform.TransformPoint(localShipPosition);
            }

            //Restore ship-carried items
            ship.GetComponentInChildren<ShipItemCapture>()?.RestoreItemPositions();
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
