using UnityEngine;
using UnityEngine.Tilemaps;

public class ExitHouse : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Transform child = GetComponent<Transform>().Find("Roof");

        //If the player walks through, switch the roof off
        if (child!= null && collision.gameObject.tag == "Player")
        {
            child.gameObject.SetActive(false);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Transform child = GetComponent<Transform>().Find("Roof");
        //If the player walks through, switch the roof on
        if (child != null && collision.gameObject.tag == "Player")
        {
            child.gameObject.SetActive(true);
        }
    }

        private void OnTriggerStay2D(Collider2D collision)
    {
        Transform child = GetComponent<Transform>().Find("Roof");
        //If the player is inside the building, switch the roof off
        if (child != null && collision.gameObject.tag == "Player")
        {
            child.gameObject.SetActive(false);
            Debug.Log("Player is inside building");
        }
    }
}
