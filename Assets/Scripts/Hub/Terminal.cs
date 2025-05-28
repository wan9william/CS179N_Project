using UnityEngine;

public class Terminal : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Player player;
    [SerializeField] Game_Event_Manager game_event_manager;
    public void ClickX() {
        transform.gameObject.SetActive(false);
        player.setPaused(false);
    }

    public void ClickStart() {
        transform.gameObject.SetActive(false);
        player.setPaused(false);
        game_event_manager.SetState(Game_Event_Manager.GM_STATES.START_MISSION);
    }

    public void ClickExtract()
    {
        transform.gameObject.SetActive(false);
        player.setPaused(false);
        game_event_manager.SetState(Game_Event_Manager.GM_STATES.END_MISSION);
        game_event_manager.SetLoseMission(false);
    }
}
