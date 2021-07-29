using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public CinemachineFreeLook closeCam;

    public Item currentSelection;

    Vector3 currentPos, lastPos = Vector3.zero;
    float realSpeed;

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
    public bool running, grounded, groundedRaw, jump, sprinting, hasWeapon, melee,
        shouldSlow, modifiedAttack, rolling, goingLeft, goingRight;
    int attackID = 0;

    public CharacterController controller;
    public float speed = 6f, runSpeed = 6f, sprintSpeed = 12f, gravity = -9.81f, jumpHeight = 1f, groundedRadius = 0.8f, playerDecell = 0.75f,  acceleration = 1f;


    public float attackTime = 0.2f;

    KeyCode keycode;
    public Vector3 playerVelocity, airborneVelocity;
    Coroutine attackCo;

    float horizontalMove = 0f, verticalMove = 0f;

    public bool armed;
    public GameObject rightHand, leftHand, rightFoot, leftFoot;

    public GameObject debrisParticles;

    public float groundedDelay;
    bool coRunning;

    Inventory inventory;

    public float throwChargeTimer;
    public float medThrowChargeTime, bigThrowChargeTime, shortThrowDelay;
    public bool smallCharge, bigCharge, throwRelease = true, throwing, smallThrow, canThrow = true;
    public float throwForce, medThrowForceMultiplier, bigThrowForceMultiplier, smallThrowDelay, medThrowDelay, maxThrowDelay;
    public VisualEffect medChargeEffect, bigChargeEffect;
    Coroutine canThrowCo;
    public float thrownObjectOverrideSpeedThreshold;

    public GameObject portalSparkle;

    public Grabber rightHandGrabber;

    bool pressedQ;


    private void Awake()
    {

        rightHandGrabber = GetComponent<Grabber>();
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

    private void Start()
    {

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

        if(inventoryUI.rightHandObject != null)
        {
            rightHandGrabber.rightHandObj = inventoryUI.rightHandObject.transform;
            rightHandGrabber.lookObj = inventoryUI.rightHandObject.transform;
        }
        else
        {
            rightHandGrabber.rightHandObj = null;
            rightHandGrabber.lookObj = null;
        }

        hasWeapon = WeaponActive();

        try
        {
            currentSelection = inventory.ItemByName(inventory.visableInventory[inventoryUI.selectedSlot]);
        }
        catch
        {
            currentSelection = null;
        }

        //speed update
        currentPos = transform.position;

        if (lastPos != Vector3.zero)
        {
            realSpeed = (currentPos - lastPos).magnitude / Time.deltaTime;
        }

        lastPos = currentPos;

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
        UpdateAttackCurves();
    }

    private void LateUpdate()
    {

    }

    bool GroundedRayHitGround(Vector3 start)
    {
        RaycastHit hit;
        Debug.DrawRay(start + new Vector3(0f, 1f, 0f), -transform.up * groundedRadius, Color.blue);
        if (Physics.Raycast(transform.position, -transform.up, out hit, groundedRadius))
        {
            for (int i = 0; i < tagsConsideredGround.Length; i++)
            {
                if (hit.transform.gameObject.tag == tagsConsideredGround[i])
                {
                    return true;
                }
            }
        }
        return false;
    }

    void CheckGroundedState()
    {
        groundedRaw = false;
        bool rightGrounded = false, leftGrounded = false, middleGrounded = false;

        rightGrounded = GroundedRayHitGround(rightFoot.transform.position);
        leftGrounded = GroundedRayHitGround(leftFoot.transform.position);
        middleGrounded = GroundedRayHitGround(0.5f*(rightFoot.transform.position + leftFoot.transform.position));

        if (leftGrounded || rightGrounded || middleGrounded || realSpeed < 1f)
            groundedRaw = true;


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

        if(smallCharge || bigCharge && !smallThrow)
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
        if (Input.GetKeyDown(KeyCode.A))
        {
            goingLeft = true;
            goingRight = false;
        }
        if (Input.GetKeyUp(KeyCode.A))
            goingLeft = false;
        if(Input.GetKeyDown(KeyCode.D))
        {
            goingRight = true;
            goingLeft = false;
        }
        if (Input.GetKeyUp(KeyCode.D))
            goingRight = false;



        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        

        if (Input.GetKey(KeyCode.LeftShift))
            sprinting = true;
        else
            sprinting = false;

         
        if (Input.GetKeyDown(KeyCode.Space) && grounded && !rolling && !modifiedAttack)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            jump = true;
        }

        if (Input.GetMouseButton(1))
            modifiedAttack = true;
        else
            modifiedAttack = false;

        if (Input.GetKeyDown(KeyCode.Space) && grounded && modifiedAttack)
        {
            StartCoroutine(DelayFalsify(0.2f));
            rolling = true;
        }

        if(Input.GetMouseButtonDown(0) && !theUIController.inMenus)
        {
            if (attackCo != null)
            {
                StopCoroutine(attackCo);
                melee = false;
            }

            attackCo = StartCoroutine(Melee(attackTime));

            if (hasWeapon)
            {
                
            }
            else
            {
                if (modifiedAttack)
                {
                }
                else
                {
                }
            }
        }

        UpdateThrow();
    }

    IEnumerator Melee(float time)
    {   
        melee = true;
        yield return new WaitForSeconds(time);
        melee = false;
    }

    void UpdateAttackCurves()
    {
        //Melee 
        if (inventoryUI.rightHandObject != null && inventoryUI.rightHandObject.transform.childCount > 0)
        {
            DamageGiver meleeDamager = inventoryUI.rightHandObject.transform.GetChild(0).GetComponent<DamageGiver>();
            if (meleeDamager != null)
            {
                if (anim.GetFloat("MeleeCurve") < 0.5)
                    meleeDamager.dontGiveDamage = true;
                else
                    meleeDamager.dontGiveDamage = false;
            }
        }

        //Right hand
        DamageGiver rh = rightHand.GetComponent<DamageGiver>();
        if (anim.GetFloat("RHCurve") < 0.5)
            rh.dontGiveDamage = true;
        else
            rh.dontGiveDamage = false;

        //Left hand
        DamageGiver lh = leftHand.GetComponent<DamageGiver>();
        if (anim.GetFloat("LHCurve") < 0.5)
            lh.dontGiveDamage = true;
        else
            lh.dontGiveDamage = false;

        //Right foot
        DamageGiver rf = rightFoot.GetComponent<DamageGiver>();
        if (anim.GetFloat("RFCurve") < 0.5)
            rf.dontGiveDamage = true;
        else
            rf.dontGiveDamage = false;

        //Left hand
        DamageGiver lf = leftFoot.GetComponent<DamageGiver>();
        if (anim.GetFloat("LFCurve") < 0.5)
            lf.dontGiveDamage = true;
        else
            lf.dontGiveDamage = false;
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
        anim.SetBool("Melee", melee);
        anim.SetBool("ModifiedAttack", modifiedAttack);
        anim.SetFloat("VerticalSpeed", GetComponent<CharacterController>().velocity.y);
        anim.SetBool("SmallCharge", smallCharge);
        anim.SetBool("BigCharge", bigCharge);
        anim.SetBool("ThrowRelease", throwRelease);
        anim.SetBool("Throwing", throwing);
        anim.SetFloat("RealSpeed", realSpeed);
        anim.SetBool("Rolling", rolling);
        anim.SetBool("GoingRight", goingRight);
        anim.SetBool("GoingLeft", goingLeft);
        anim.SetBool("HasWeapon", hasWeapon);
        anim.SetBool("SmallThrow", smallThrow);
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
            pressedQ = true;
        }
        if ((Input.GetButtonUp("Throw") || !Input.GetButton("Throw")) && throwing && pressedQ)
        {
            if (!smallCharge && !bigCharge)
                smallThrow = true;

            if (canThrowCo != null)
                StopCoroutine(canThrowCo);
            canThrowCo = StartCoroutine(ThrowCooldown());
            throwChargeTimer = 0;
            pressedQ = false;

            ThrowObject(1, inventoryUI.selectedSlot, true);
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

    public void ThrowObject(int count, int slotID, bool isSingleThrow)
    {
        if (inventory.visableInventoryQuantity[slotID] > 0)
        {
            if (!smallCharge && !bigCharge)
                StartCoroutine(ThrowDelay(smallThrowDelay, 1, count, slotID, isSingleThrow));
            else if (smallCharge && !bigCharge)
                StartCoroutine(ThrowDelay(medThrowDelay, medThrowForceMultiplier, count, slotID, isSingleThrow));
            else if (bigCharge)
                StartCoroutine(ThrowDelay(maxThrowDelay, bigThrowForceMultiplier, count, slotID, isSingleThrow));
        }
    }

    bool WeaponActive()
    {
        if (currentSelection != null)
        {
            if (currentSelection.isWeapon)
                return true;
            else
                return false;
        }

        return false;
    }

    IEnumerator ThrowCooldown()
    {
        
        canThrow = false;
        yield return new WaitForSeconds(shortThrowDelay);
        canThrow = true;
        smallThrow = false;
    }

    IEnumerator ThrowDelay(float delay, float multiplier, int count, int slotID, bool isSingleThrow)
    {
        throwing = true;
        yield return new WaitForSeconds(delay);
        if (inventory.RetrieveGameObjectByName(inventory.visableInventory[slotID]) != null)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject item = Instantiate(inventory.RetrieveGameObjectByName(inventory.visableInventory[slotID]),
                rightHand.transform.position, Quaternion.identity);
                inventory.visableInventoryQuantity[slotID]--;
                item.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;


                if (!isSingleThrow)
                {
                    multiplier = 1;
                    item.layer = 18;
                }

                if (multiplier == 1 || !cameraControls.camClose || !isSingleThrow)
                    item.GetComponent<Rigidbody>().AddForce(transform.forward * throwForce * multiplier);
                else
                    item.GetComponent<Rigidbody>().AddForce(cam.transform.forward.normalized * throwForce * multiplier);

                if (multiplier == 1)
                {
                    item.GetComponent<DamageGiver>().dontGiveDamage = true;
                    smallThrow = true;
                    StartCoroutine(ThrowCooldown());
                }

                item.GetComponent<DamageGiver>().overrideSpeedThreshold = true;
                item.GetComponent<DamageGiver>().overriddenSpeedThreshold = thrownObjectOverrideSpeedThreshold;

                if (i % 50 == 0)
                    yield return null;
            }
        }

        if (!isSingleThrow) //Threw by dragging item ui into scene
        {
            throwing = false;
            smallCharge = false;
            bigCharge = false;
            throwChargeTimer = 0f;
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

    IEnumerator DelayFalsify(float delay)
    {
        yield return new WaitForSeconds(delay);
        rolling = false;
    }

}
