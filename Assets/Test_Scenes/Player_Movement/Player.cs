using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.Switch;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    //Player Animation
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    //Player Sprint Bar
    [SerializeField] private Slider sprint_bar;
    [SerializeField] private AudioSource sfxAudioSource;

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


    //Health Parameters
    [SerializeField] private float health;

    //Movement Parameters
    [Header("Movement Parameters")]
    [SerializeField] private float movement_speed = 1.0f;
    [SerializeField] private float horizontal_multiplier = 1.0f;
    [SerializeField] private float vertical_multiplier = 1.0f;
    [SerializeField] private float dampening;
    [SerializeField] private bool paused;

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
    [SerializeField] private PlayerHealth _healthbar;
    [SerializeField] private Camera _camera;
    [SerializeField] private TextMeshProUGUI _moneyText;

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
    private SpriteRenderer _muzzleFlashSR;
    [SerializeField] private int muzzleFlashOffset = 0;
    [SerializeField] private bool releaseMouse = false;

    //Currency
    [SerializeField] private int money = 0;

    //Managers
    [SerializeField] private Game_Event_Manager game_event_manager;



public static Player Singleton;

void Awake()
{
    Singleton = this;
}
    //Hand
    private SpriteRenderer _handSR;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selected_slot = 0;
        inventory = new Inventory(canvas.transform);
        health = 100;
        
        _rb = GetComponent<Rigidbody2D>();
        if (_equipped != null)
            _weaponScript = _equipped.GetComponent<Weapon>();

        if(sprint_bar != null) sprint_bar.maxValue = 100f;
        inventory.SelectSlot(selected_slot);
        SelectEquipped();
    }

    public void RevivePlayer() {
        animator.SetBool("Dead", false);
        health = 100;
        _healthbar.SetHealth(100);
    }

    // Update is called once per frame
    void Update()
    {
        float current_speed = movement_speed;
        var player = this;

        //we could check if we have a weapon every frame
        //_weaponScript = _equipped != null ? _equipped.GetComponentInChildren<Weapon>() : null;

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    TakeDamage(10);

        //}

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

        if (_equipped != null)
        {
            _hand.gameObject.SetActive(true);
        }
        else { _hand.gameObject.SetActive(false); }

        //STATE ACTIONS & TRANSITIONS FOR THE ACTION STATE
        switch (action_state)
        {
            case PLAYER_ACTION_STATES.IDLE:


                //This will be the area where the player is in between actions. Some examples are between bullet shots, Not interacting with anything, etc.

                if (paused) break;


                //This should be separated by state actions (what to do while in the state) and state transitions(which state to move to)


                if (!Input.GetKey(KeyCode.LeftShift)) { 
                    sprint_amount += drain_amount * Time.deltaTime;
                    sprint_amount = Mathf.Clamp(sprint_amount, 0, 100);
                    animator.SetFloat("SpeedMult", 1.0f);
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
                // 🔧 Store current ammo before dropping
                    // 🔁 First, store the weapon's ammo into the inventory
                    if (_equipped != null && _weaponScript != null)
                    {
                        inventory.getInventorySlots()[selected_slot].storedAmmo = _weaponScript.GetCurrentAmmo();
                    }

                    // 💥 Drop logic
                    GameObject resourceItem = inventory.GetSelectedResource(selected_slot);
                    if (resourceItem == null) break;

                    GameObject droppedItem = Instantiate(resourceItem, transform.position, Quaternion.identity);
                    droppedItem.transform.localScale = Vector3.one;
                    droppedItem.layer = 6;

                    Resource resScript = droppedItem.GetComponent<Resource>();
                    if (resScript != null)
                    {
                        resScript.SetNatural(false);

                        int storedAmmo = inventory.getInventorySlots()[selected_slot].storedAmmo;
                        resScript.SetQuantity(storedAmmo);  // ✅ Set correct ammo
                    }

                    // Add rigidbody and physics
                    Rigidbody2D rb = !droppedItem.GetComponent<Rigidbody2D>() ? droppedItem.AddComponent<Rigidbody2D>() : droppedItem.GetComponent<Rigidbody2D>();
                    BoxCollider2D bc = !droppedItem.GetComponent<BoxCollider2D>() ? droppedItem.AddComponent<BoxCollider2D>() : droppedItem.GetComponent<BoxCollider2D>();
                    bc.isTrigger = true;

                    Vector2 MousePos = Input.mousePosition;
                    Vector3 MouseWorldPos = _camera.ScreenToWorldPoint(MousePos);
                    Vector2 dir = ((Vector2)MouseWorldPos - (Vector2)transform.position).normalized;
                    rb.linearVelocity = dir * 3f;
                    rb.gravityScale = 0;
                    rb.linearDamping = 2f;

                    dropForce = 0f;

                    Destroy(_equipped);
                    _equipped = null;

                    InventorySlot slot = inventory.getInventorySlots()[selected_slot];
                    if (slot.storedAmmo > 0)
                    {
                        slot.SetQuantity(0);
                        slot.storedAmmo = -1;
                        slot.UpdateItem();
                    }
                    else
                    {
                        inventory.DecrementItem(selected_slot);
                    }

                    SelectEquipped();

                    break;
                }
                else if (Input.GetKey(KeyCode.Q))
                {
                    dropForce += 0.3f * Time.deltaTime;
                    Mathf.Clamp(dropForce, 0, 1);
                }

                releaseMouse = !Input.GetMouseButton(0) ? true : releaseMouse;

                //EVERYTHING BEYOND THIS ASSUMES THERE IS AN ATTACHED EQUIPPABLE SCRIPT
                Equippable _equipScript = (_equipped) ? _equipped.GetComponent<Equippable>() : null;
                if (Input.GetMouseButton(0) && _equipScript) {
                    action_state = PLAYER_ACTION_STATES.SHOOT;
                }

                //EVERYTHING BEYOND THIS STATEMENT ASSUMES THAT THERE IS AN ATTACHED WEAPONSCRIPT
                if (!_equipped) 
                {
                    animator.SetBool("Equipped", false); 
                    _hand.gameObject.SetActive(false); 
                }
                else
                {
                    animator.SetBool("Equipped", true);
                    _hand.gameObject.SetActive(true);
                }
                if (!_weaponScript) { break; }

                var fireMode = _weaponScript.GetFireMode();
                
                

                if ((fireMode == FireMode.FullAuto || fireMode == FireMode.SemiAuto) && Input.GetMouseButton(0) && _equipped) action_state = PLAYER_ACTION_STATES.SHOOT;


                if (_weaponScript && Input.GetKeyDown(KeyCode.R))//reloading
                {
                    action_state = PLAYER_ACTION_STATES.RELOAD;
                    break;
                }



                

                break;
            case PLAYER_ACTION_STATES.SHOOT:


                //This will be the state that actually creates the bullet, muzzle flash, recoil, etc.

                //Any data specific to a weapon (fire rate, damage, etc) should likely be stored in a Scriptable Object (feel free to look it up). This will make our implementation easier.
                //As well as more memory friendly.
                if (_equipped != null)
                {
                    Weapon weapon = _equipped.GetComponentInChildren<Weapon>();
                    if (weapon != null)
                    {
                        Debug.Log("[Player] Firing weapon");
                        bool canShoot = weapon.Shoot();
                        if (canShoot) {
                            trauma += 0.5f;
                            trauma = Mathf.Clamp(trauma, 0f, 1f);
                        }
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

                if (releaseMouse)
                {
                    _equipped.GetComponent<Equippable>().Use(ref player);
                    releaseMouse = false;
                }

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
                
                if (_interactable)
                {
                    Debug.Log("Interact door!");
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
                if (paused) break;

                bool moved = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
                if (moved && health > 0 && !paused) movement_state = PLAYER_MOVEMENT_STATES.WALK;


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
                    animator.SetFloat("SpeedMult", 1.5f);
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

    public void TakeDamage(int damage) {
        health -= damage;

        //need a death check here as well
        if (health <= 0)
        {
            animator.SetBool("Dead", true);
        }

        _healthbar.TakeDamage(damage);
    }

    public float GetHealth() { return health; }

    public float GetMaxHealth() { return 100f; }

    public void MissionReset()
    {
        game_event_manager.SetState(Game_Event_Manager.GM_STATES.END_MISSION);
        game_event_manager.SetLoseMission(true);
    }

    public void SendToZero()
    {
        this.gameObject.transform.localPosition = Vector3.zero;
    }
    public void SelectEquipped() {
    GameObject selectedPrefab = inventory.selecteditem(selected_slot);
    InventorySlot[] slots = inventory.getInventorySlots();

    //  Store ammo from the currently equipped weapon into its corresponding slot
    if (_equipped != null)
    {
        string equippedName = _equipped.name.Replace("(Clone)", "").Trim();

        for (int i = 0; i < slots.Length; i++)

        {
            GameObject slotFab = slots[i].getFab();
            if (slotFab != null && slotFab.name == equippedName)
            {
                Weapon oldWeapon = _equipped.GetComponentInChildren<Weapon>();
                if (oldWeapon != null)
                {
                    slots[i].storedAmmo = oldWeapon.GetCurrentAmmo();
                    slots[i].UpdateQuantityDisplay();  //  Update UI
                    Debug.Log($"[SelectEquipped] Stored ammo {oldWeapon.GetCurrentAmmo()} into slot {i}");
                }
                break;
            }
        }

        Destroy(_equipped);
        _equipped = null;
    }

    //  Equip if not already equipped
    if (_equipped == null && selectedPrefab != null)
    {
        _equipped = Instantiate(selectedPrefab);
        _equipped.transform.SetParent(transform, false);
        _equipped.transform.localPosition = Vector3.zero;
        _equipped.transform.localRotation = Quaternion.identity;
        

        Vector3 inverseScale = new Vector3(
            1f / transform.localScale.x,
            1f / transform.localScale.y,
            1f / transform.localScale.z
        );
        _equipped.transform.localScale = inverseScale;
        NormalizeChildScale(_equipped.transform);

        _weaponScript = _equipped.GetComponentInChildren<Weapon>();

        if (_weaponScript != null)
        {
            GameObject prefabFromSlot = slots[selected_slot].getFab();
            if (prefabFromSlot != null && prefabFromSlot.name == selectedPrefab.name && slots[selected_slot].storedAmmo >= 0)
            {
                _weaponScript.SetCurrentAmmo(slots[selected_slot].storedAmmo);
                Debug.Log($"[SelectEquipped] Restored ammo {slots[selected_slot].storedAmmo} from slot {selected_slot}");
            }
            else
            {
                // Fresh weapon (new pickup or different weapon type), fill magazine
                _weaponScript.SetCurrentAmmo(_weaponScript.GetMagazineSize());
                Debug.Log("[SelectEquipped] New weapon equipped. Full magazine applied.");
            }

            slots[selected_slot].UpdateQuantityDisplay(); //  Always update display
        }

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

        if (!_weaponSR && _equipped)
            _weaponSR = _equipped.GetComponentInChildren<SpriteRenderer>(); 

        if (_weaponSR&&_weaponScript)
        {
            _weaponSR.flipY = (dir.x < 0);  // flip only when pointing left
        }

        if(_equipped)

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

    public void setPaused(bool pause) { paused = pause; }

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

    public void PlaySFX(AudioClip clip)
    {
        if (sfxAudioSource && clip)
            sfxAudioSource.PlayOneShot(clip);
    }

    public ref int GetMoney() { return ref money; }

    public void SetMoney(int val) { 
        money = val;

        //update money value on UI
        if(_moneyText) _moneyText.text = money.ToString();
    }

}


