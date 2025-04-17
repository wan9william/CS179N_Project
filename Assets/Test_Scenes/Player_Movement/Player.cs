using UnityEngine;


public class Player : MonoBehaviour
{

    public Animator animator;
    public SpriteRenderer spriteRenderer;

    //Inventory
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject canvas;

    //States
    private enum PLAYER_MOVEMENT_STATES { IDLE, WALK };
    private enum PLAYER_ACTION_STATES { IDLE, SHOOT, INTERACT }


    private PLAYER_ACTION_STATES action_state = PLAYER_ACTION_STATES.IDLE;
    private PLAYER_MOVEMENT_STATES movement_state = PLAYER_MOVEMENT_STATES.IDLE;


    //Movement Parameters
    [SerializeField] private float movement_speed = 1.0f;
    [SerializeField] private float horizontal_multiplier = 1.0f;
    [SerializeField] private float vertical_multiplier = 1.0f;
    [SerializeField] private float dampening;
     
    private float eps = 1e-5f;


    //Components
    private Rigidbody2D _rb;
    [SerializeField] private GameObject _equipped;
    [SerializeField] private GameObject _interactable;
    [SerializeField] private Camera _camera;

    //FLAGS
    [SerializeField] private bool find_interact = false;

    //Equipped Tool
    private Weapon _weaponScript;
    private SpriteRenderer _weaponSR;





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = new Inventory(canvas.transform);
        
        _rb = GetComponent<Rigidbody2D>();
    if (_equipped != null)
        _weaponScript = _equipped.GetComponent<Weapon>();


    }


    // Update is called once per frame
    void Update()
    {
        float current_speed = Input.GetKey(KeyCode.LeftShift) ? 1.5f * movement_speed : movement_speed;
        //STATE ACTIONS & TRANSITIONS FOR THE ACTION STATE
        switch (action_state)
        {
            case PLAYER_ACTION_STATES.IDLE:


                //This will be the area where the player is in between actions. Some examples are between bullet shots, Not interacting with anything, etc.




                //This should be separated by state actions (what to do while in the state) and state transitions(which state to move to)






                //START OF STATE TRANSITIONS


                //If E is pressed, switch to interact state.
                if (Input.GetKeyDown(KeyCode.E))
                {
                    action_state = PLAYER_ACTION_STATES.INTERACT;
                    break;
                }

            if (_weaponScript != null)
            {
                var fireMode = _weaponScript.GetFireMode();

                if (fireMode == FireMode.FullAuto && Input.GetMouseButton(0))
                {
                    action_state = PLAYER_ACTION_STATES.SHOOT;
                }
                else if (fireMode == FireMode.SemiAuto && Input.GetMouseButtonDown(0))
                {
                    action_state = PLAYER_ACTION_STATES.SHOOT;
                }
            }

                break;
            case PLAYER_ACTION_STATES.SHOOT:


                //This will be the state that actually creates the bullet, muzzle flash, recoil, etc.


                //Any data specific to a weapon (fire rate, damage, etc) should likely be stored in a Scriptable Object (feel free to look it up). This will make our implementation easier.
                //As well as more memory friendly.

    _equipped.GetComponent<Weapon>().Shoot();
    action_state = PLAYER_ACTION_STATES.IDLE;
    break;


           
            case PLAYER_ACTION_STATES.INTERACT:


                //for now, simply make the object disappear. Will add resources in the future
                var player = this;
                if (_interactable)
                {
                    _interactable.GetComponent<Interactable>().Destroy(ref player);
                }
                //_interactable = null;


                //START OF STATE TRANSITIONS
                action_state = PLAYER_ACTION_STATES.IDLE;


                break;
            default:
                break;


        }


        //STATE ACTIONS & TRANSITIONS FOR THE MOVEMENT STATE
        switch (movement_state)
        {
            case PLAYER_MOVEMENT_STATES.IDLE:
                animator.SetBool("Walk", false);
                bool moved = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
                if (moved) movement_state = PLAYER_MOVEMENT_STATES.WALK;


                vertical_multiplier = Mathf.Abs( vertical_multiplier ) < eps ? 0 : Mathf.Lerp(vertical_multiplier, 0.0f, Time.deltaTime*dampening);
                horizontal_multiplier = Mathf.Abs( horizontal_multiplier ) < eps ? 0 : Mathf.Lerp(horizontal_multiplier, 0.0f, Time.deltaTime*dampening);


                break;


            case PLAYER_MOVEMENT_STATES.WALK:
                animator.SetBool("Walk", true); 
                bool horizontal_walk = false;
                bool vertical_walk = false;
                if (Input.GetKey(KeyCode.W))
                {
                    vertical_multiplier = Mathf.Lerp(vertical_multiplier, 1.0f, Time.deltaTime * dampening);
                    vertical_walk |= true;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    horizontal_multiplier = Mathf.Lerp(horizontal_multiplier, -1.0f, Time.deltaTime * dampening);
                    horizontal_walk |= true;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    vertical_multiplier = Mathf.Lerp(vertical_multiplier, -1.0f, Time.deltaTime * dampening);
                    vertical_walk |= true;
                   
                }
                if (Input.GetKey(KeyCode.D))
                {
                    horizontal_multiplier = Mathf.Lerp(horizontal_multiplier, 1.0f, Time.deltaTime * dampening);
                    horizontal_walk |= true;
                }


                if (!(horizontal_walk || vertical_walk)) movement_state = PLAYER_MOVEMENT_STATES.IDLE;
                if(!vertical_walk)  vertical_multiplier = Mathf.Abs(vertical_multiplier) < eps ? 0 : Mathf.Lerp(vertical_multiplier, 0.0f, Time.deltaTime * dampening);
                if(!horizontal_walk)horizontal_multiplier = Mathf.Abs(horizontal_multiplier) < eps ? 0 : Mathf.Lerp(horizontal_multiplier, 0.0f, Time.deltaTime * dampening);
                break;
            default:
                break;


        }


        //Rotate Equipped tool
        RotateEquipped();


        //Apply movement
        _rb.linearVelocity = new Vector2(horizontal_multiplier,vertical_multiplier)*current_speed;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If the player began touching a resource, make it glow
        if (collision.gameObject.tag == "Interactable")
        {
            _interactable = collision.gameObject;
            collision.gameObject.GetComponent<Interactable>().Glow();
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        //If the player began touching a resource, make it glow
        if (collision.gameObject.tag == "Interactable")
        {

            collision.gameObject.GetComponent<Interactable>().NoGlow();
        }
        if (collision.gameObject == _interactable) _interactable = null;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Interactable" && find_interact)
        {
            _interactable = collision.gameObject;
        }
    }

    private void RotateEquipped() {
        Vector2 MousePos = Input.mousePosition;
        Vector3 MouseWorldPos = _camera.ScreenToWorldPoint(MousePos);
        Vector2 dir = ((Vector2)MouseWorldPos - (Vector2)transform.position).normalized;
        MouseWorldPos.z = 0;

        animator.SetFloat("MouseX", dir.x);
        animator.SetFloat("MouseY", dir.y);
        _equipped.transform.up = dir;

        if(!_weaponSR) _weaponSR = _equipped.GetComponentInChildren<SpriteRenderer>();

        if (_weaponSR)
        {
            _weaponSR.flipY = (dir.x < 0);  // flip only when pointing left
        }

        _equipped.transform.localPosition = (Vector3)dir * 5f;
    }


    public void SetFindInteract(bool _i) { find_interact = _i; }

    public void SetInteract(GameObject _go) { _interactable = _go; }

    public Inventory getInventory() { return inventory; }

}



