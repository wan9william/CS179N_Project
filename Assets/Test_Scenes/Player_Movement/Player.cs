using UnityEngine;

public class Player : MonoBehaviour
{
    private enum PLAYER_MOVEMENT_STATES { IDLE, WALK };
    private enum PLAYER_ACTION_STATES { IDLE, SHOOT, INTERACT }

    private PLAYER_ACTION_STATES action_state = PLAYER_ACTION_STATES.IDLE;
    private PLAYER_MOVEMENT_STATES movement_state = PLAYER_MOVEMENT_STATES.IDLE;


    [SerializeField] private float movement_speed = 1.0f;
    [SerializeField] private float horizontal_multiplier = 1.0f;
    [SerializeField] private float vertical_multiplier = 1.0f;
    [SerializeField] private float dampening;
     
    private float eps = 1e-5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //STATE ACTIONS & TRANSITIONS FOR THE ACTION STATE
        switch (action_state)
        {
            case PLAYER_ACTION_STATES.IDLE:
                break;
            default:
                break;

        }

        //STATE ACTIONS & TRANSITIONS FOR THE MOVEMENT STATE
        switch (movement_state)
        {
            case PLAYER_MOVEMENT_STATES.IDLE:

                bool moved = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
                if (moved) movement_state = PLAYER_MOVEMENT_STATES.WALK;

                vertical_multiplier = Mathf.Abs( vertical_multiplier ) < eps ? 0 : Mathf.Lerp(vertical_multiplier, 0.0f, Time.deltaTime*dampening);
                horizontal_multiplier = Mathf.Abs( horizontal_multiplier ) < eps ? 0 : Mathf.Lerp(horizontal_multiplier, 0.0f, Time.deltaTime*dampening);

                break;

            case PLAYER_MOVEMENT_STATES.WALK:

                bool walk = false;
                if (Input.GetKey(KeyCode.W))
                {
                    vertical_multiplier = Mathf.Lerp(vertical_multiplier, 1.0f, Time.deltaTime);
                    walk |= true;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    horizontal_multiplier = Mathf.Lerp(horizontal_multiplier, -1.0f, Time.deltaTime);
                    walk |= true;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    vertical_multiplier = Mathf.Lerp(vertical_multiplier, -1.0f, Time.deltaTime);
                    walk |= true;
                    
                }
                if (Input.GetKey(KeyCode.D))
                {
                    horizontal_multiplier = Mathf.Lerp(horizontal_multiplier, 1.0f, Time.deltaTime);
                    walk |= true;
                }

                if (!walk) movement_state = PLAYER_MOVEMENT_STATES.IDLE;
                break;
            default:
                break;

        }

        //Apply movement
        transform.position += new Vector3(horizontal_multiplier,vertical_multiplier,0)*Time.deltaTime*movement_speed;
    }

    
}
