using UnityEngine;

public class Terminal : MonoBehaviour
{
    public enum UI_Elements { START, EXTRACT}; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Player player;
    [SerializeField] Game_Event_Manager game_event_manager;

    [SerializeField] GameObject Start_Button;
    [SerializeField] GameObject Extract_Button;
    [SerializeField] UI_Elements current_ui;

    public void ActivateUI(UI_Elements ui) {
        switch (ui)
        {
            case UI_Elements.START:
                Start_Button.SetActive(true);
                break;
            case UI_Elements.EXTRACT:
                Extract_Button.SetActive(true);
                break;
            default:
                break;
        }
        current_ui = ui;
    }

    public void ClickX() {
        DeactivateUI();
        player.setPaused(false);
    }

    public void ClickStart() {
        DeactivateUI();
        player.setPaused(false);
        game_event_manager.SetState(Game_Event_Manager.GM_STATES.START_MISSION);
    }

    public void ClickExtract()
    {
        DeactivateUI();
        player.setPaused(false);
        game_event_manager.SetState(Game_Event_Manager.GM_STATES.END_MISSION);
        game_event_manager.SetLoseMission(false);
    }

    private void DeactivateUI() {
        switch (current_ui)
        {
            case UI_Elements.START:
                Start_Button.SetActive(false);
                break;
            case UI_Elements.EXTRACT:
                Extract_Button.SetActive(false);
                break;
            default:
                break;
        }
    }
}
