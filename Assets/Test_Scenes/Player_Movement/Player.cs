using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;


public class Player : MonoBehaviour
{
    //Player Animation
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    //Player Sprint Bar
    [SerializeField] private Slider sprint_bar;


    //Inventory
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject canvas;
    [SerializeField] private int selected_slot;
    [SerializeField] private GameObject flashlightPrefab;

    //States
    private enum PLAYER_MOVEMENT_STATES { IDLE, WALK };
    private enum PLAYER_ACTION_STATES { IDLE, SHOOT, INTERACT, SELECT, DROP, RELOAD }


    private PLAYER_ACTION_STATES action_state = PLAYER_ACTION_STATES.IDLE;
    private PLAYER_MOVEMENT_STATES movement_state = PLAYER_MOVEMENT_STATES.IDLE;


    //Movement Parameters
    [Header("Movement Parameters")]
    [SerializeField] private float movement_speed = 1.0f;
    [SerializeField] private float horizontal_multiplier = 1.0f;
    [SerializeField] private float vertical_multiplier = 1.0f;
    [SerializeField] private float dampening;

    //Sprinting Parameters
    [SerializeField] private float sprint_amount = 100f;
    [SerializeField] private float drain_amount = 25f;
    //Drop Parameters
    [SerializeField] private float dropForce = 0.0f;
     
    private float eps = 1e-5f;


    //Components
    [Header("Components")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private GameObject _equipped;
    [SerializeField] private GameObject _hand;
    [SerializeField] private GameObject _interactable;
    [SerializeField] private Camera _camera;

    //FLAGS
    [Header("Flags")]
    [SerializeField] private bool find_interact = false;

    [Header("Hand Settings")]
    [SerializeField] private Vector3 handOffset = Vector3.zero;
    [SerializeField] private float handRadius = 3.5f;
    [SerializeField] private int handLayerFront = 4;
    [SerializeField] private int handLayerBack = 2;

    //Camera Parameters
    [Header("Camera Settings")]
    [SerializeField] private float trauma;
    [SerializeField] private float translational_shake_max;
    [SerializeField] private float rotational_shake_max;


    //Equipped Tool
    private Weapon _weaponScript;
    private SpriteRenderer _weaponSR;

    //Currency
    [SerializeField] private int money = 0;



public static Player Singleton;

void Awake()
{
    Singleton = this;
}
    //Hand
    private SpriteRenderer _handSR;

    //Muzzle Flash
    private SpriteRenderer _muzzleFlashSR;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selected_slot = 0;
        inventory = new Inventory(canvas.transform);
        
        _rb = GetComponent<Rigidbody2D>();
        if (_equipped != null)
            _weaponScript = _equipped.GetComponent<Weapon>();

        if(sprint_bar != null) sprint_bar.maxValue = 100f;

    }

    // Update is called once per frame
    void Update()
    {
        float current_speed = movement_speed;

        //we could check if we have a weapon every frame
        //_weaponScript = _equipped != null ? _equipped.GetComponentInChildren<Weapon>() : null;

        //reduce trauma after each frame
        trauma -= 1f * Time.deltaTime;
        trauma = Mathf.Clamp(trauma, 0f, 1f);

        if (_camera != null)
        {
            _camera.transform.localPosition = new Vector3(Random.Range(-translational_shake_max, translational_shake_max) * Mathf.Pow(trauma, 3), Random.Range(-translational_shake_max, translational_shake_max) * Mathf.Pow(trauma, 3), -10);
            _camera.transform.localEulerAngles = new Vector3(0, 0, Random.Range(-rotational_shake_max, rotational_shake_max) * Mathf.Pow(trauma, 3));
        }

        if (sprint_bar != null)
        {
            if (sprint_amount < 100)
            {
                sprint_bar.gameObject.SetActive(true);
                sprint_bar.value = sprint_amount;
            }
            else
            {
                sprint_bar.gameObject.SetActive(false);
            }
        }

        if (_equipped == null) _hand.gameObject.SetActive(false);
        else _hand.gameObject.SetActive(true);

        //STATE ACTIONS & TRANSITIONS FOR THE ACTION STATE
        switch (action_state)
        {
            case PLAYER_ACTION_STATES.IDLE:


                //This will be the area where the player is in between actions. Some examples are between bullet shots, Not interacting with anything, etc.




                    //This should be separated by state actions (what to do while in the state) and state transitions(which state to move to)


                 if (!Input.GetKey(KeyCode.LeftShift)) { 
                    sprint_amount += drain_amount * Time.deltaTime;
                    sprint_amount = Mathf.Clamp(sprint_amount, 0, 100);
                 }



                //START OF STATE TRANSITIONS


                //If E is pressed, switch to interact state.
                if (Input.GetKeyDown(KeyCode.E))
                {
                    action_state = PLAYER_ACTION_STATES.INTERACT;
                    break;
                }

                //If the scroll wheel is used, get current selected slot and add accordingly, then transition
                if (Input.mouseScrollDelta.y != 0)
                {
                    Debug.Log(Input.mouseScrollDelta.y);
                    selected_slot += Input.mouseScrollDelta.y > 0 ? 1 : -1;
                    selected_slot = Mathf.Clamp(selected_slot, 0, 7);


                    action_state = PLAYER_ACTION_STATES.SELECT;
                    break;
                }

                //If a num is pressed, 
                int selected_key = GetNumKey();
                if (selected_key != -1)
                {
                    selected_slot = selected_key;
                    selected_slot = Mathf.Clamp(selected_slot, 0, 7);

                    action_state = PLAYER_ACTION_STATES.SELECT;
                    break;
                }

                //Dropping Items
                if (Input.GetKeyUp(KeyCode.Q) && dropForce > 0f)
                {

                    //Instantiate object
                    //GET THE RESOURCE VERSION OF THE OBJECT
                    GameObject resourceItem = inventory.GetSelectedResource(selected_slot);

                    //This may cause issues in the future
                    if (resourceItem == null) break;

                    GameObject droppedItem = Instantiate(resourceItem, transform.position, Quaternion.identity);//Instantiate(_equipped, transform.position, Quaternion.identity);


                    droppedItem.transform.localScale = Vector3.one;
                    droppedItem.layer = 6; //Item_RB layer

                    //Add rigidbody if necessary and calculate direction
                    Rigidbody2D rb = !droppedItem.GetComponent<Rigidbody2D>() ? droppedItem.AddComponent<Rigidbody2D>() : droppedItem.GetComponent<Rigidbody2D>();
                    BoxCollider2D bc = !droppedItem.GetComponent<BoxCollider2D>() ? droppedItem.AddComponent<BoxCollider2D>() : droppedItem.GetComponent<BoxCollider2D>();
                    bc.isTrigger = true;
                    Vector2 MousePos = Input.mousePosition;
                    Vector3 MouseWorldPos = _camera.ScreenToWorldPoint(MousePos);
                    Vector2 dir = ((Vector2)MouseWorldPos - (Vector2)transform.position).normalized;
                    rb.linearVelocity = dir * 3f;
                    rb.gravityScale = 0;
                    rb.linearDamping = 2f;

                    //reset drop force
                    dropForce = 0f;

                    Destroy(_equipped);
                    _equipped = null;

                    //REMOVE 1 OF THE SELECTED SLOT'S RESOURCE. DELETION WILL BE HANDLED BY THE SLOT ITSELF
                    inventory.DecrementItem(selected_slot);

                    //Select same slot
                    SelectEquipped();


                    break;
                }
                else if (Input.GetKey(KeyCode.Q))
                {
                    dropForce += 0.3f * Time.deltaTime;
                    Mathf.Clamp(dropForce, 0, 1);
                }

                //EVERYTHING BEYOND THIS STATEMENT ASSUMES THAT THERE IS AN ATTACHED WEAPONSCRIPT
                if (!_weaponScript) { animator.SetBool("Equipped", false); _hand.SetActive(false); break; }

                animator.SetBool("Equipped", true);
                _hand.SetActive(true);
                var fireMode = _weaponScript.GetFireMode();
                
                

                if (fireMode == FireMode.FullAuto && Input.GetMouseButton(0) && _equipped)
                {
                    action_state = PLAYER_ACTION_STATES.SHOOT;
                }
                else if (fireMode == FireMode.SemiAuto && Input.GetMouseButtonDown(0) && _equipped)
                {
                    action_state = PLAYER_ACTION_STATES.SHOOT;
                }

            if (_weaponScript && Input.GetKeyDown(KeyCode.R))//reloading
            {
                action_state = PLAYER_ACTION_STATES.RELOAD;
                break;
            }

                

                break;
            case PLAYER_ACTION_STATES.SHOOT:


                //This will be the state that actually creates the bullet, muzzle flash, recoil, etc.


                trauma += 0.5f;
                trauma = Mathf.Clamp(trauma, 0f, 1f);

                //Any data specific to a weapon (fire rate, damage, etc) should likely be stored in a Scriptable Object (feel free to look it up). This will make our implementation easier.
                //As well as more memory friendly.
                if (_equipped != null)
                {
                    Weapon weapon = _equipped.GetComponentInChildren<Weapon>();
                    if (weapon != null)
                    {
                        Debug.Log("[Player] Firing weapon");
                        weapon.Shoot();
                    }
                    else
                    {
                        Debug.LogWarning("[Player] Equipped item does not have a Weapon component (even in children).");
                    }
                }
                else
                {
                    Debug.LogWarning("[Player] No weapon equipped.");
                }

                _equipped.GetComponent<Equippable>().Use();
                action_state = PLAYER_ACTION_STATES.IDLE;
                break;

            case PLAYER_ACTION_STATES.SELECT:

                //actions go here for selecting a slot

                inventory.SelectSlot(selected_slot);
                SelectEquipped();
                

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


            case PLAYER_ACTION_STATES.RELOAD:
                if (_weaponScript != null)
                {
                    if (!_weaponScript.IsReloading())
                    {
                        _weaponScript.StartReload();
                    }

                    if (_weaponScript.IsReloading())
                    {
                        // Still reloading — stay in RELOAD state
                        break;
                    }

                    // Done reloading — go back to idle
                    action_state = PLAYER_ACTION_STATES.IDLE;
                }
                else
                {
                    action_state = PLAYER_ACTION_STATES.IDLE;
                }
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

                //Start walking animation
                animator.SetBool("Walk", true);


                //Movement parameters
                current_speed = movement_speed;
                bool horizontal_walk = false;
                bool vertical_walk = false;


                //Sprinting
                if (Input.GetKey(KeyCode.LeftShift) && sprint_amount > 0)
                {
                    current_speed = 1.5f * movement_speed;
                    sprint_amount -= drain_amount * Time.deltaTime;
                    sprint_amount = Mathf.Clamp(sprint_amount, 0, 100);
                }
                else if (!Input.GetKey(KeyCode.LeftShift)) {
                    sprint_amount += drain_amount * Time.deltaTime;
                    sprint_amount = Mathf.Clamp(sprint_amount, 0, 100);
                }

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
        if(_rb != null) _rb.linearVelocity = new Vector2(horizontal_multiplier,vertical_multiplier)*current_speed;
    }

    private void SelectEquipped() {
        GameObject selectedPrefab = inventory.selecteditem(selected_slot);

        // Remove currently equipped object if it's different
        if (_equipped != null && selectedPrefab != null && _equipped.name != selectedPrefab.name + "(Clone)")
        {
            Destroy(_equipped);
            _equipped = null;
        }

        // Equip if not already equipped
        if (_equipped == null && selectedPrefab != null)
        {
            _equipped = Instantiate(selectedPrefab);
            _equipped.transform.SetParent(transform, false);
            _equipped.transform.localPosition = Vector3.zero;
            _equipped.transform.localRotation = Quaternion.identity;

            // Scale up to cancel out the player's scale (e.g., 0.2 becomes 5x)
            Vector3 inverseScale = new Vector3(
            1f / transform.localScale.x,
            1f / transform.localScale.y,
            1f / transform.localScale.z
        );
            _equipped.transform.localScale = inverseScale;

            // Optional but helpful if the prefab has nested children with messed-up scales
            NormalizeChildScale(_equipped.transform);

            //properly update the weapon script to newly created weapon
            _weaponScript = _equipped.GetComponentInChildren<Weapon>();

            // Debug log
            Debug.Log($"[EQUIP DEBUG] Final equipped scale: {_equipped.transform.lossyScale}");
        }
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
        
        if (!_equipped) return;
        _equipped.transform.up = dir;

        if (!_weaponSR&&_equipped) _weaponSR = _equipped.GetComponentInChildren<SpriteRenderer>();

        if (_weaponSR)
        {
            _weaponSR.flipY = (dir.x < 0);  // flip only when pointing left
        }

        _handSR = _hand.GetComponent<SpriteRenderer>();
        if (dir.y > 0)
        {
            _handSR.sortingOrder = handLayerBack;
        }
        else _handSR.sortingOrder = handLayerFront;

        if(_equipped) _equipped.transform.localPosition = (Vector3)dir * handRadius + handOffset;
        _hand.transform.localPosition = (Vector3)dir * handRadius + handOffset;
    }


    public void SetFindInteract(bool _i) { find_interact = _i; }

    public void SetInteract(GameObject _go) { _interactable = _go; }

    public Inventory getInventory() { return inventory; }

    int GetNumKey()
    {
        //Return the key that was pressed, 0 indexed
        for (int i = 0; i < 8; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) return i;
        }

        //-1 represents that nothing was pressed
        return -1;
    }

    public GameObject GetEquippedPrefab()
    {
        return _equipped != null ? _equipped : null;
    }

    public GameObject GetFlashlightPrefab()
    {
        return flashlightPrefab;
    }

    private void NormalizeChildScale(Transform root)
    {
        foreach (Transform child in root)
        {
            child.localScale = Vector3.one;
            NormalizeChildScale(child); // Recursively reset nested children
        }
    }

    public ref int GetMoney() { return ref money; }

    public void SetMoney(int val) { money = val; }

}


