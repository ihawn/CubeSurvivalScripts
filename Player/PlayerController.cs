using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    public string[] shouldSlowAnimations;
    public string[] punchDoesDamageAnimations;

    PlayerControls controls;
    CameraController cameraControls;
    public UIController theUIController;
    public InventoryUI inventoryUI;
    public PlayerInterract playerInterract;
    public GameObject groundedChecker;

    Animator anim;
    Rigidbody rb;

    public Transform cam;

    public GameObject cameraTarget, rig;

    Vector2 move;
    Vector2 rotate;
    Vector3 direction;
    public Vector3 mSticky = Vector3.zero;
    public float walkSpeed, sensitivity, rotateSpeed, turnSmoothTime;
    float turnSmoothVelocity;

    public string[] tagsConsideredGround;
    public bool running, grounded, groundedRaw, jump, sprinting, hasWeapon, punching, shouldSlow, modifiedAttack, kicking;
    int attackID = 0;

    public CharacterController controller;
    public float speed = 6f, runSpeed = 6f, sprintSpeed = 12f, gravity = -9.81f, jumpHeight = 1f, groundedRadius = 0.8f, playerDecell = 0.75f, stopPunchingDelay = 0.5f, acceleration = 1f;

    KeyCode keycode;
    public Vector3 playerVelocity, airborneVelocity;
    Coroutine attackCo;

    float horizontalMove = 0f, verticalMove = 0f;

    public bool armed;
    public GameObject rightHand, leftHand;

    public GameObject debrisParticles;

    public float groundedDelay;
    bool coRunning;

    Inventory inventory;

    public float throwChargeTimer;
    public float medThrowChargeTime, bigThrowChargeTime, shortThrowDelay;
    public bool smallCharge, bigCharge, throwRelease = true, throwing, canThrow = true;
    public float throwForce, medThrowForceMultiplier, bigThrowForceMultiplier, smallThrowDelay, medThrowDelay, maxThrowDelay;
    public VisualEffect medChargeEffect, bigChargeEffect;
    Coroutine canThrowCo;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
        LockCurser();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        cameraControls = FindObjectOfType<CameraController>();
        controls = new PlayerControls();

        controls.Gameplay.MoveController.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Gameplay.MoveController.canceled += ctx => move = Vector2.zero;
    
        controls.Gameplay.RotatePlayerController.performed += ctx => rotate = ctx.ReadValue<Vector2>();
        controls.Gameplay.RotatePlayerController.canceled += ctx => rotate = Vector2.zero;

        controls.Gameplay.ChangeViewController.performed += ctx => cameraControls.ToggleViewCamera();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void FixedUpdate()
    {
        //Movement(horizontalMove, verticalMove);
    }

    private void Update()
    {
        CheckInputs();
        UpdateMovementBools();
        UpdatePlayerAnimations();
        UpdateItemSlotInput();
        UpdateKeyPresses();
        UpdateSlotSwitching();
        CheckGroundedState();
        KeyboardInput();
        Movement(horizontalMove, verticalMove);
        UpdateAttackDamage();
    }

    private void LateUpdate()
    {

    }

    void CheckGroundedState()
    {
        RaycastHit hit;
        groundedRaw = false;
        Debug.DrawRay(transform.position, -transform.up * groundedRadius, Color.blue);
        if (Physics.Raycast(transform.position, -transform.up, out hit, groundedRadius))
        {
            for(int i = 0; i < tagsConsideredGround.Length; i++)
            {
                if(hit.transform.gameObject.tag == tagsConsideredGround[i])
                {
                    groundedRaw = true;
                    break;
                }
            }
        }

        //   groundedRaw = GetComponent<CharacterController>().isGrounded;


        if (!groundedRaw)
        {
        if (!coRunning)
                StartCoroutine(GroundedDenoise());
        }
        else
            grounded = groundedRaw;
    }

    void CheckInputs()
    {
        if(Input.GetMouseButtonDown(2))
        {
            cameraControls.ToggleViewCamera();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            theUIController.ToggleMenu();
        }
    }

    void Movement(float horizontal, float vertical)
    {
        shouldSlow = ShouldSlow();

        direction = new Vector3(horizontal, 0f, vertical).normalized;
        if (sprinting)
            speed = sprintSpeed;
        else
            speed = runSpeed;

        if(smallCharge || bigCharge)
            transform.rotation = Quaternion.Euler(0f, cam.eulerAngles.y, 0f);

        if (direction.magnitude >= 0.1 && grounded && !shouldSlow)
        {
           
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward.normalized;
            playerVelocity.x = Mathf.Lerp(playerVelocity.x, moveDir.x * speed, acceleration);
            playerVelocity.z = Mathf.Lerp(playerVelocity.z, moveDir.z * speed, acceleration);
            airborneVelocity.x = playerVelocity.x;
            airborneVelocity.z = playerVelocity.z;
        }
        else if (!grounded)
        {
            playerVelocity.x = airborneVelocity.x;
            playerVelocity.z = airborneVelocity.z;
        }
        else
        {
            playerVelocity.x *= playerDecell;
            playerVelocity.z *= playerDecell;
            airborneVelocity.x *= playerDecell;
            airborneVelocity.z *= playerDecell;
        }

        if (grounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            jump = false;
        }

        if(shouldSlow)
        {
            playerVelocity.x *= playerDecell;
            playerVelocity.z *= playerDecell;
            airborneVelocity.x *= playerDecell;
            airborneVelocity.z *= playerDecell;
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void KeyboardInput()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        

        if (Input.GetKey(KeyCode.LeftShift))
            sprinting = true;
        else
            sprinting = false;


        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            jump = true;
        }

        if (Input.GetMouseButton(1))
            modifiedAttack = true;
        else
            modifiedAttack = false;

        if(Input.GetMouseButtonDown(0))
        {
            if(hasWeapon)
            {
                //todo
            }
            else
            {
                if (attackCo != null)
                    StopCoroutine(attackCo);

                if (modifiedAttack)
                {
                    attackCo = StartCoroutine(Kick());
                }
                else
                {
                    attackCo = StartCoroutine(Punch());
                }
            }
        }

        UpdateThrow();
    }

    IEnumerator Punch()
    {
        punching = true;
        yield return new WaitForSeconds(stopPunchingDelay);
        punching = false;
    }

    IEnumerator Kick()
    {
        kicking = true;
        yield return new WaitForSeconds(stopPunchingDelay);
        kicking = false;
    }

    void UpdateAttackDamage()
    {
        if(!rightHand.GetComponent<BoxCollider>().enabled && PunchDoesDamage())
        {
            rightHand.GetComponent<BoxCollider>().enabled = true;
            leftHand.GetComponent<BoxCollider>().enabled = true;
        }

        if(rightHand.GetComponent<BoxCollider>().enabled && !PunchDoesDamage())
        {
            rightHand.GetComponent<BoxCollider>().enabled = false;
            leftHand.GetComponent<BoxCollider>().enabled = false;
        }
    }

    void UpdateMovementBools()
    {
        if (new Vector2(playerVelocity.x, playerVelocity.z).magnitude >= 0.05f)
            running = true;
        else
            running = false;
    }

    void UpdatePlayerAnimations()
    {
        anim.SetBool("Running", running);
        anim.SetBool("Grounded", grounded);
        anim.SetBool("Jump", jump);
        anim.SetBool("Sprint", sprinting);
        anim.SetBool("Punching", punching);
        anim.SetBool("Kicking", kicking);
        anim.SetFloat("VerticalSpeed", GetComponent<CharacterController>().velocity.y);
        anim.SetBool("SmallCharge", smallCharge);
        anim.SetBool("BigCharge", bigCharge);
        anim.SetBool("ThrowRelease", throwRelease);
        anim.SetBool("Throwing", throwing);
    }

    void UpdateItemSlotInput()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            inventoryUI.ChangeSelectedSlot(Input.mouseScrollDelta.y);
        }


    }

    void UpdateSlotSwitching()
    {
        if (!playerInterract.inChoicesMenu)
        {
            switch(keycode)
            {
                case KeyCode.Alpha1:
                    inventoryUI.ChangeSelectedSlot(0);
                break;

            }
        }
    }

    void UpdateKeyPresses()
    {
        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            keycode = vKey;
    }

    bool PunchDoesDamage()
    {
        for(int i = 0; i < punchDoesDamageAnimations.Length; i++)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(punchDoesDamageAnimations[i]))
                return true;
        }

        return false;
    }

    bool ShouldSlow()
    {
        for(int i = 0; i < shouldSlowAnimations.Length; i++)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(shouldSlowAnimations[i]))
                return true;
        }

        
        return false;
    }

    public void UnlockCurser()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LockCurser()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UpdateThrow()
    {
        if (Input.GetButtonDown("Throw") && inventory.visableInventory[inventoryUI.selectedSlot] != null)
        {
            throwing = true;
            throwRelease = false;
        }
        if (Input.GetButtonUp("Throw") && throwing)
        {
            if (canThrowCo != null)
                StopCoroutine(canThrowCo);
            canThrowCo = StartCoroutine(ThrowCooldown());
            throwChargeTimer = 0;

            ThrowObject();
            throwing = false;
            throwRelease = true;
        }
        if (throwChargeTimer >= medThrowChargeTime)
        {
            smallCharge = true;
            cameraControls.SetAimCamera(true);
        }
        else
            smallCharge = false;
        if (throwChargeTimer >= bigThrowChargeTime)
            bigCharge = true;
        else
            bigCharge = false;
        if(throwing)
            throwChargeTimer += Time.deltaTime;

        if (!smallCharge && !bigCharge)
            cameraControls.SetAimCamera(false);

        //Update charge effects
        if (smallCharge && !bigCharge)
        {
            medChargeEffect.gameObject.SetActive(true);
            medChargeEffect.SetFloat("GlobalSpawnRate", 1);
        }
        else
        {
            medChargeEffect.SetFloat("GlobalSpawnRate", 0);
            medChargeEffect.gameObject.SetActive(false);
        }
        if (bigCharge)
        {
            bigChargeEffect.gameObject.SetActive(true);
            bigChargeEffect.SetFloat("GlobalSpawnRate", 1);
        }
        else
        {
            bigChargeEffect.SetFloat("GlobalSpawnRate", 0);
            bigChargeEffect.gameObject.SetActive(false);
        }
    }

    void ThrowObject()
    {
        if(inventoryUI.rightHandObject != null)
        {
            if (!smallCharge && !bigCharge)
                StartCoroutine(ThrowDelay(smallThrowDelay, 1));
            else if (smallCharge && !bigCharge)
                StartCoroutine(ThrowDelay(medThrowDelay, medThrowForceMultiplier));
            else if (bigCharge)
                StartCoroutine(ThrowDelay(maxThrowDelay, bigThrowForceMultiplier));
        }
    }

    IEnumerator ThrowCooldown()
    {
        canThrow = false;
        yield return new WaitForSeconds(shortThrowDelay);
        canThrow = true;
    }

    IEnumerator ThrowDelay(float delay, float multiplier)
    {
        yield return new WaitForSeconds(delay);
        if (inventory.RetrieveGameObjectByName(inventory.visableInventory[inventoryUI.selectedSlot]) != null)
        {
            GameObject item = Instantiate(inventory.RetrieveGameObjectByName(inventory.visableInventory[inventoryUI.selectedSlot]),
            inventoryUI.rightHandObject.transform.position, inventoryUI.rightHandObject.transform.rotation);
            inventory.visableInventoryQuantity[inventoryUI.selectedSlot]--;
            item.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;

            if(multiplier == 1 || !cameraControls.camClose)
                item.GetComponent<Rigidbody>().AddForce(transform.forward * throwForce * multiplier);
            else
                item.GetComponent<Rigidbody>().AddForce(cam.transform.forward.normalized * throwForce * multiplier);
        }
    }

    IEnumerator GroundedDenoise()
    {
        coRunning = true;

        yield return new WaitForSeconds(groundedDelay);

        if (!groundedRaw)
            grounded = false;
        coRunning = false;
    }


}
