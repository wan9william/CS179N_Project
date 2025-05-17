using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Button_Interact : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    enum BUTTON_STATES { BUTTON_IDLE, BUTTON_PRESSED, BUTTON_RELEASE, BUTTON_PRESSED_IDLE };
    BUTTON_STATES state;

    [SerializeField] private Animator animator;
    [SerializeField] private Func<int> func = null;
    [SerializeField] private bool pressed;
    [SerializeField] private Player presser;

    protected override void onInteract(ref Player player)
    {
        //This is where we change the button's state as well as send a signal to the recipient
        state = BUTTON_STATES.BUTTON_PRESSED;
        presser = player;
        pressed = true;
    }

    protected override void ExplosionVFX()
    {
        //Inefficient, replace later
        Instantiate(_explosion, transform.position, Quaternion.identity);
    }

    //This may cause issues in the future. Where should interactables have a tick function? Should it be local or inherited? Maybe it's inherited, but each child has an abstract
    //implementation of the tick function
    protected override void Tick()
    {
        switch (state) {
            case BUTTON_STATES.BUTTON_IDLE:
                break;
            case BUTTON_STATES.BUTTON_PRESSED:

                animator.SetBool("Pressed", true);
                pressed = true;
                state = BUTTON_STATES.BUTTON_PRESSED_IDLE;
                break;

            case BUTTON_STATES.BUTTON_RELEASE:

                //animator.SetBool("Pressed", false);
                state = BUTTON_STATES.BUTTON_IDLE;
                pressed = false;

                break;
            case BUTTON_STATES.BUTTON_PRESSED_IDLE:

                //This is for now, but there is potential for holding down a button.
                state = BUTTON_STATES.BUTTON_RELEASE;
                break;
            default:
                break;
        }
        return;
    }

    public bool GetPressed() { return pressed; }

    public Player GetPlayer() { return presser; }
}
