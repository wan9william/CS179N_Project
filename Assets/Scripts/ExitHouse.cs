using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ExitHouse : MonoBehaviour
{
    [SerializeField] GameObject roofTilemap;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If the player walks through, switch the roof off
        if (roofTilemap!= null && collision.gameObject.tag == "Player")
        {
            roofTilemap.SetActive(false);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //If the player walks through, switch the roof on
        if (roofTilemap != null && collision.gameObject.tag == "Player")
        {
            roofTilemap.SetActive(true);
        }
    }

}
