using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Terminal : MonoBehaviour
{
    public enum UI_Elements { START, EXTRACT };

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Game_Event_Manager game_event_manager;

    [Header("UI Containers")]
    [SerializeField] private GameObject UI_Parent;
    [SerializeField] private GameObject Start_Mission_UI;
    [SerializeField] private GameObject Extract_Mission_UI;

    [Header("UI Buttons")]
    [SerializeField] private GameObject Start_Button_Obj;
    [SerializeField] private GameObject Planet1_Button_Obj;
    [SerializeField] private GameObject Planet2_Button_Obj;
    [SerializeField] private GameObject Planet3_Button_Obj;

    [Header("Audio Elements")]
    [SerializeField] private AudioClip selectClip;
    [SerializeField] private AudioSource audioSource;

    private UI_Elements current_ui;
    private string selectedPlanet = null;
    private Button startButton;
    private Button selectedButton;

    private ColorBlock activeLookColors;
    private ColorBlock disabledLookColors;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        startButton = Start_Button_Obj.GetComponent<Button>();
        if (startButton == null)
        {
            Debug.LogError("Start_Button_Obj must reference a UI Button GameObject.");
            return;
        }

        startButton.interactable = false;

        // Store active color block from one of the planet buttons
        Button planetBtnRef = Planet1_Button_Obj.GetComponent<Button>();
        activeLookColors = planetBtnRef.colors;

        // Create disabled-style visual color block
        disabledLookColors = activeLookColors;
        disabledLookColors.normalColor = activeLookColors.disabledColor;
        disabledLookColors.highlightedColor = activeLookColors.disabledColor;
        disabledLookColors.pressedColor = activeLookColors.disabledColor;
        disabledLookColors.selectedColor = activeLookColors.disabledColor;

        // Apply disabled look to all buttons initially
        Planet1_Button_Obj.GetComponent<Button>().colors = disabledLookColors;
        Planet2_Button_Obj.GetComponent<Button>().colors = disabledLookColors;
        Planet3_Button_Obj.GetComponent<Button>().colors = disabledLookColors;

        // Add listeners
        Planet1_Button_Obj.GetComponent<Button>().onClick.AddListener(() => SelectPlanet("Derelict Echo", Planet1_Button_Obj));
        Planet2_Button_Obj.GetComponent<Button>().onClick.AddListener(() => SelectPlanet("Virelia Prime", Planet2_Button_Obj));
        Planet3_Button_Obj.GetComponent<Button>().onClick.AddListener(() => SelectPlanet("Elarin Reach", Planet3_Button_Obj));
    }

    /// <summary>
    /// Show the terminal UI based on mission state.
    /// </summary>
    public void Show()
    {
        UI_Parent.SetActive(true);
        player.setPaused(true);

        if (game_event_manager.IsInMission())
        {
            Extract_Mission_UI.SetActive(true);
            Start_Mission_UI.SetActive(false);
            current_ui = UI_Elements.EXTRACT;
        }
        else
        {
            Start_Mission_UI.SetActive(true);
            Extract_Mission_UI.SetActive(false);
            current_ui = UI_Elements.START;

            selectedPlanet = null;
            startButton.interactable = false;
            ResetPlanetButtonColors();
        }
    }

    private void SelectPlanet(string planetName, GameObject buttonObj)
    {
        selectedPlanet = planetName;
        startButton.interactable = true;

        // Reset all planet buttons to disabled look
        Planet1_Button_Obj.GetComponent<Button>().colors = disabledLookColors;
        Planet2_Button_Obj.GetComponent<Button>().colors = disabledLookColors;
        Planet3_Button_Obj.GetComponent<Button>().colors = disabledLookColors;

        // Highlight the selected one
        selectedButton = buttonObj.GetComponent<Button>();
        selectedButton.colors = activeLookColors;

        Debug.Log("Selected Planet: " + selectedPlanet);
        audioSource.Play();
    }

    private void ResetPlanetButtonColors()
    {
        Planet1_Button_Obj.GetComponent<Button>().colors = disabledLookColors;
        Planet2_Button_Obj.GetComponent<Button>().colors = disabledLookColors;
        Planet3_Button_Obj.GetComponent<Button>().colors = disabledLookColors;
        selectedButton = null;
    }

    public void ClickX()
    {
        DeactivateUI();
        UI_Parent.SetActive(false);
        player.setPaused(false);
        audioSource.Play();
    }

    public void ClickStart()
    {
        if (selectedPlanet == null)
        {
            Debug.LogWarning("Start pressed but no planet selected!");
            return;
        }

        current_ui = UI_Elements.EXTRACT;
        DeactivateUI();
        UI_Parent.SetActive(false);
        player.setPaused(false);
        audioSource.Play();

        game_event_manager.SetSelectedPlanet(selectedPlanet);
        game_event_manager.SetState(Game_Event_Manager.GM_STATES.START_MISSION);

        Debug.Log("Starting mission on: " + selectedPlanet);
        
    }

    public void ClickExtract()
    {
        DeactivateUI();
        UI_Parent.SetActive(false);
        player.setPaused(false);

        game_event_manager.SetState(Game_Event_Manager.GM_STATES.END_MISSION);
        game_event_manager.SetLoseMission(false);

        current_ui = UI_Elements.START;
        audioSource.Play();
    }

    private void DeactivateUI()
    {
        Start_Mission_UI.SetActive(false);
        Extract_Mission_UI.SetActive(false);
    }
}
