using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private void Awake()
    {
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

    void CheckGroundedState()
    {
        /*Collider[] groundedCollider = Physics.OverlapSphere(groundedChecker.transform.position, groundedRadius);

        grounded = false;

        for(int i = 0; i < groundedCollider.Length; i++)
        {
            if (groundedCollider[i].gameObject.tag == "Ground" || groundedCollider[i].gameObject.tag == "Rock")
                grounded = true;
        }*/

        groundedRaw = GetComponent<CharacterController>().isGrounded;

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

    IEnumerator GroundedDenoise()
    {
        coRunning = true;
        print("coroutine running");
        yield return new WaitForSeconds(groundedDelay);

        if (!groundedRaw)
            grounded = false;
        coRunning = false;
    }
}
