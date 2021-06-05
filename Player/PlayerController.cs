using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerControls controls;
    CameraController cameraControls;

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

    private void Update()
    {
        KeyboardInput();
        CheckInputs();

        Vector3 m = new Vector3(-move.y, 0f, move.x) * walkSpeed * Time.deltaTime;


       /* if (m.magnitude > 0.05f)
            mSticky = m;

        // transform.Translate(m, Space.Self);

        rb.MovePosition(transform.position + Time.deltaTime * transform.TransformDirection(m));
        Debug.DrawRay(transform.position, transform.TransformDirection(m) * 3f, Color.red);

        Vector3 r = new Vector3(0f, rotate.x, 0f);
        transform.Rotate(r * sensitivity * Time.deltaTime);

        rig.transform.rotation = Quaternion.Lerp(rig.transform.rotation, Quaternion.LookRotation(mSticky.normalized) * transform.rotation, rotateSpeed * Time.deltaTime);

        if (cameraControls.camClose)
        {
            Vector3 r2 = new Vector3(-rotate.y, 0f, 0f);
            cameraTarget.transform.Rotate(r2 * sensitivity * Time.deltaTime);
        }*/

        UpdateMovementBools();
        UpdatePlayerAnimations();
    }

    void CheckInputs()
    {
        if(Input.GetMouseButtonDown(2))
        {
            cameraControls.ToggleViewCamera();
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
}
