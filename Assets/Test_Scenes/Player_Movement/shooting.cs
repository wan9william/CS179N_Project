using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooting : MonoBehaviour
{
    public AudioSource shootingSound;
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            shootingSound.enabled = true;
        }
        else
        {
            shootingSound.enabled = false;
        }
    }
}
