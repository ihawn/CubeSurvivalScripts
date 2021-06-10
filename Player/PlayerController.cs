using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerControls controls;
    CameraController cameraControls;
    public UIController theUIController;
    public InventoryUI inventoryUI;

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

    public bool running;

    public CharacterController controller;
    public float speed = 6f;

    private void Awake()
    {
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
        KeyboardInput();
    }

    private void Update()
    {
        CheckInputs();
        UpdateMovementBools();
        UpdatePlayerAnimations();
        UpdateItemSlotInput();
    }

    void CheckInputs()
    {
        if(Input.GetMouseButtonDown(2))
        {
            cameraControls.ToggleViewCamera();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            print("tabpressed");
            theUIController.ToggleMenu();
        }
    }


    void KeyboardInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

    }


    void UpdateMovementBools()
    {
        if (move.magnitude > 0 || direction.magnitude >= 0.1)
            running = true;
        else
            running = false;
    }

    void UpdatePlayerAnimations()
    {
        anim.SetBool("Running", running);
    }

    void UpdateItemSlotInput()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            inventoryUI.ChangeSelectedSlot(Input.mouseScrollDelta.y);
        }
    }
}
