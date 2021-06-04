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

    public GameObject cameraTarget, rig;

    Vector2 move;
    Vector2 rotate;
    Vector3 mSticky = Vector3.zero;
    public float walkSpeed, sensitivity, rotateSpeed;

    public bool running;

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


        Vector3 m = new Vector3(-move.y, 0f, move.x) * walkSpeed * Time.deltaTime;


        if (m.magnitude > 0.05f)
            mSticky = m;

        transform.Translate(m, Space.Self);


        Vector3 r = new Vector3(0f, rotate.x, 0f);
        transform.Rotate(r * sensitivity * Time.deltaTime);

        rig.transform.rotation = Quaternion.Lerp(rig.transform.rotation, Quaternion.LookRotation(mSticky.normalized) * transform.rotation, rotateSpeed * Time.deltaTime);

        if (cameraControls.camClose)
        {
            Vector3 r2 = new Vector3(-rotate.y, 0f, 0f);
            cameraTarget.transform.Rotate(r2 * sensitivity * Time.deltaTime);
        }

        UpdateMovementBools();
        UpdatePlayerAnimations();
    }



    void UpdateMovementBools()
    {
        if (move.magnitude > 0)
            running = true;
        else
            running = false;
    }

    void UpdatePlayerAnimations()
    {
        anim.SetBool("Running", running);
    }
}
