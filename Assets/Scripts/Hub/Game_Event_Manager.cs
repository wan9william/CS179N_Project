using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game_Event_Manager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool startMission;
    private bool initialize = true;
    private bool loseMission = false;
    private ShipItemCapture shipItemCapture;
    private Vector3 shipPositionOffset;
    private string selectedPlanetScene = "ItemSpawnerTestScene"; // fallback default

    //Screen fade effect
    [SerializeField] private float ScreenFadeT = 0f;
    [SerializeField] private Image fade;

    //Dead Text
    [SerializeField] private GameObject Dead_Text;
    [SerializeField] private GameObject Success_Text;
    [SerializeField] private GameObject Object_Manager;
    [SerializeField] private Player player;
    [SerializeField] private GameObject UI;

    public enum GM_STATES { 
        IDLE,
        INITIALIZE,
        START_MISSION,
        END_MISSION
    };

    private GM_STATES state;

    void Start()
    {
        state = GM_STATES.IDLE;
        if(!initialize) initialize = true;


        //Prevents the following objects from being destroyed across scenes.
        if (GameObject.FindGameObjectsWithTag("Player").Length > 1) Destroy(player.gameObject);
        if (GameObject.FindGameObjectsWithTag("UI_Manager").Length > 1) Destroy(UI.gameObject);
        if (GameObject.FindGameObjectsWithTag("Object_Manager").Length > 1) Destroy(Object_Manager.gameObject);
        if (GameObject.FindGameObjectsWithTag("Game_Manager").Length > 1) Destroy(gameObject);

        //Absolutely terrible code devised by yours truly. There are million ways to optimize this or even make this more readable.
        DontDestroyOnLoad(gameObject);
        if(player) DontDestroyOnLoad(player.gameObject);
        if(Object_Manager) DontDestroyOnLoad(Object_Manager);
        if(UI) DontDestroyOnLoad(UI);

        GameObject ship = GameObject.FindWithTag("Ship");
        if (ship != null)
        {
            shipItemCapture = ship.GetComponentInChildren<ShipItemCapture>();
            DontDestroyOnLoad(ship);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Tick();
    }

    void Tick() {

        switch (state) { 
            case GM_STATES.IDLE:
            if (initialize)
            {
                ScreenFadeT = 1f;
                state = GM_STATES.INITIALIZE;

                GameObject ship = GameObject.FindWithTag("Ship");
                GameObject landingPad = GameObject.FindWithTag("Landingpad");

                if (ship != null && landingPad != null)
                {
                    ship.transform.position = landingPad.transform.position;

                    // Move player to relative position inside ship
                    player.transform.position = ship.transform.TransformPoint(shipPositionOffset);

                    // Restore dropped ship items
                    ShipItemCapture capture = ship.GetComponentInChildren<ShipItemCapture>();
                    if (capture != null)
                    {
                        capture.RestoreItemPositions();
                    }
                }
            }
            break;
            case GM_STATES.INITIALIZE:
                initialize = false;
                ForwardFadeAnimation(true);
                break;
            case GM_STATES.START_MISSION:
                ForwardFadeAnimation(false);

                if (ScreenFadeT >= 1f)
                {
                    if (shipItemCapture != null)
                    {
                        shipItemCapture.CaptureItems();
                    }

                    GameObject ship = GameObject.FindWithTag("Ship");
                    if (ship != null && player != null)
                    {
                        shipPositionOffset = ship.transform.InverseTransformPoint(player.transform.position);
                    }

                    SceneManager.LoadScene(selectedPlanetScene);
                    state = GM_STATES.IDLE;
                    initialize = true;
                }
                break;
            case GM_STATES.END_MISSION:
                if (loseMission)
                {
                    Dead_Text.SetActive(true);
                }
                else
                {
                    Success_Text.SetActive(true);
                }

                ForwardFadeAnimation(false);

                if (ScreenFadeT >= 1f)
                {
                    GameObject ship = GameObject.FindWithTag("Ship");
                    if (ship != null)
                    {
                        shipItemCapture = ship.GetComponentInChildren<ShipItemCapture>();
                        if (shipItemCapture != null)
                        {
                            shipItemCapture.CaptureItems();
                        }

                        if (player != null)
                        {
                            shipPositionOffset = ship.transform.InverseTransformPoint(player.transform.position);
                        }
                    }

                    // Delay restore until scene is loaded
                    SceneManager.sceneLoaded += OnSceneLoaded_Extract;

                    SceneManager.LoadScene("Space");
                    state = GM_STATES.IDLE;
                    initialize = true;
                }
                break;
            default:
                break;
        }

    }

    void ForwardFadeAnimation(bool reverse) {
        Color currentCol = fade.color;

        //Progress the t factor
        ScreenFadeT += reverse ? -Time.deltaTime : Time.deltaTime;
        if (ScreenFadeT >= 1f) { ScreenFadeT = 1f; }
        if (ScreenFadeT <= 0f) { ScreenFadeT = 0; }

        //Simplification of a + (b-a)*t where a=0, b=1
        currentCol.a = ScreenFadeT;
        fade.color = currentCol;
    }

    public void SetState(GM_STATES new_state) {
        state = new_state;
    }

    public void SetLoseMission(bool lose) {
        loseMission = lose;
    }

    public void SetSelectedPlanet(string planetName)
    {
        switch (planetName)
        {
            case "Derelict Echo":
                selectedPlanetScene = "Scifi Stage";
                break;
            case "Virelia Prime":
                selectedPlanetScene = "Japan Stage";
                break;
            case "Elarin Reach":
                selectedPlanetScene = "Forest Stage";
                break;
            default:
                selectedPlanetScene = "ItemSpawnerTestScene";
                break;
        }
    }

    public bool IsInMission()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        return currentScene != "HubWorld 2" && currentScene != "Space";
    }

    private void OnSceneLoaded_Extract(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Space")
        {
            GameObject ship = GameObject.FindWithTag("Ship");
            if (ship != null)
            {
                shipItemCapture = ship.GetComponentInChildren<ShipItemCapture>();
                if (shipItemCapture != null)
                {
                    shipItemCapture.RestoreItemPositions();
                }

                // Reposition player
                if (player != null)
                {
                    player.transform.position = ship.transform.TransformPoint(shipPositionOffset);
                    player.RevivePlayer();
                }
            }

            // Hide result UI
            Dead_Text.SetActive(false);
            Success_Text.SetActive(false);

            // Unsubscribe so it doesn't trigger again
            SceneManager.sceneLoaded -= OnSceneLoaded_Extract;
        }
    }
}