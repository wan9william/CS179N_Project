using UnityEngine;

public class Terminal : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Player player;
    public void ClickX() {
        transform.gameObject.SetActive(false);
        player.setPaused(false);
    }
}
