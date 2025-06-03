using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game_Event_Manager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool startMission;
    [SerializeField] private bool initialize = true;
    private bool loseMission = false;
    [SerializeField] private ShipItemCapture shipItemCapture;
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
                    // ✅ Restore items from mission
                    if (shipItemCapture != null)
                    {
                        shipItemCapture.CaptureItems();
                    }

                    GameObject ship = GameObject.FindWithTag("Ship");
                    if (ship != null && player != null)
                    {
                        player.transform.position = ship.transform.TransformPoint(shipPositionOffset);
                    }

                    SceneManager.LoadScene("Space");

                    //Revive the player
                    player.RevivePlayer();

                    //Turn off UI messages
                    Dead_Text.SetActive(false);
                    Success_Text.SetActive(false);

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
}