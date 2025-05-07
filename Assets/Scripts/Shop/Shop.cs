using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{

    private const int MAX_ITEMS = 20; //20 is a random constant that seems appropriate
    [SerializeField] private List<GameObject> objects = new List<GameObject>(MAX_ITEMS); 
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Adds interactable gameobject when entering the shop area
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Triggered!");
        if(collision.transform.tag == "Interactable") objects.Add(collision.gameObject);

    }


    //Removes interactable gameobject when leaving the shop area
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag != "Interactable") return;

        for (int i = 0; i < objects.Count; ++i) {
            if (objects[i] == collision.gameObject) objects.RemoveAt(i);
        }
    }
}
