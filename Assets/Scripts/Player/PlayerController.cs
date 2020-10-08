using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    #region General
    [Header("General")]
    public GameObject head;
    public Camera playerCamera;
    [HideInInspector] public Rigidbody rb;
    CapsuleCollider cc;
    #endregion

    #region Camera control
    [Header("Camera control")]
    [Range(0, 180)]
    public float fieldOfView = 60;
    [Tooltip("Horizontal camera sensitivity. Set to minus to invert it."), Range(-100, 100)]
    public float sensitivityX = 50;
    [Tooltip("Vertical camera sensitivity. Set to minus to invert it."), Range(-100, 100)]
    public float sensitivityY = 50;
    [Tooltip("Minimum angle the player can look down."), Range(-90, 90)]
    public float minLookAngle = -90;
    [Tooltip("Maximum angle the player can look up."), Range(-90, 90)]
    public float maxLookAngle = 90;

    [HideInInspector] public bool canLook;
    [HideInInspector] public VariableValueFloat sensitivityModifier;
    Vector2 lookVector;
    #endregion

    #region Movement
    [Header("Movement")]
    public VariableValueFloat movementSpeed;
    public PercentageModifier crouchSpeedModifier;
    Vector2 moveInput;
    Vector3 movementValue;
    #endregion

    #region Jumping
    [Header("Jumping")]
    public float forceJump = 5;
    public float jumpDelay = 0.1f;
    public float groundedRayLength = 0.01f;

    bool willJump;
    float jumpTimer = 9999999;
    LayerMask terrainDetection;
    #endregion

    #region Crouching
    [Header("Crouching")]
    public float standHeight = 2;
    public float crouchHeight = 1;
    public float crouchTime = 0.25f;
    [Range(-0.5f, 0.5f)]
    public float relativeHeadHeight = 0.375f;
    public bool toggleCrouch;

    float crouchTimer;
    [HideInInspector] public bool isCrouching;
    #endregion

    [HideInInspector] public bool canMove = true;


    bool IsGrounded()
    {
        // Casts a ray to determine if the player is standing on solid ground.
        Ray r = new Ray(transform.position, -transform.up);
        if (Physics.SphereCast(r, cc.radius, cc.height / 2 + groundedRayLength, terrainDetection))
        {
            return true;
        }
        return false;
    }

    public Vector2 MoveDirection()
    {
        return moveInput;
    }


    void OnValidate()
    {
        // void Reset() { OnValidate(); }
        // Ensure minLookAngle is not larger than maxLookAngle, or the opposite way around.
        minLookAngle = Mathf.Clamp(minLookAngle, -90, maxLookAngle);
        maxLookAngle = Mathf.Clamp(maxLookAngle, minLookAngle, 90);
    }

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        terrainDetection = Misc.CollisionMask(gameObject.layer);

        // Establish a multiplier to be used for both camera sensitivity values
        sensitivityModifier.defaultValue = 1;

        // Add crouch speed modifier to movementSpeed
        movementSpeed.Add(crouchSpeedModifier, this);
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove == true)
        {
            #region Camera
            if (canLook == true)
            {
                float cameraInputX = Input.GetAxis("MouseX") * sensitivityModifier.Calculate() * sensitivityX * Time.deltaTime;
                float cameraInputY = Input.GetAxis("MouseY") * sensitivityModifier.Calculate() * sensitivityY * Time.deltaTime;
                LookAngle(new Vector2(cameraInputX, cameraInputY));
            }
            #endregion

            #region Crouching
            CrouchHandler();
            #endregion

            #region Movement
            moveInput.x = Input.GetAxis("Horizontal"); // Set to AD keys or analog stick.
            moveInput.y = Input.GetAxis("Vertical"); // Set to WS keys or analog stick.
            if (moveInput.magnitude > 1)
            {
                moveInput.Normalize(); // Prevent movement input from going past 1. This ensures that players cannot go faster than the normal movement speed.
            }

            float speed = movementSpeed.Calculate();
            movementValue = new Vector3(moveInput.x * speed, 0, moveInput.y * speed);
            movementValue = transform.rotation * movementValue; // movementValue is multiplied by transform.rotation so moveInput occurs in the direction the character is facing.
            #endregion

            #region Jumping
            jumpTimer += Time.deltaTime;
            if (Input.GetButtonDown("Jump") && jumpTimer >= jumpDelay && IsGrounded() == true) //Raycast isGrounded is cast to detect if there is a surface underneath the player. If so, canJump boolean is enabled to allow the player to jump off the surface, and disabled if false, i.e. if the player is in midair.
            {
                if (isCrouching == true)
                {
                    isCrouching = false;
                }
                willJump = true;
            }
            #endregion
        }


    }

    #region Camera control functions
    public void LookAngle(Vector2 cameraInput) // This variable is public so it can be altered by other sources such as gun recoil
    {
        lookVector.x = cameraInput.x;
        //lookVector.y = head.transform.localRotation.x;
        lookVector.y -= cameraInput.y;

        lookVector.y = Mathf.Clamp(lookVector.y, minLookAngle, maxLookAngle); // Camera.y is then clamped to ensure it does not move past 90* or 90*, ensuring that the player does not flip the camera over completely.
        transform.Rotate(0, lookVector.x, 0); // Player is rotated on y axis based on Camera.x, for turning left and right
        head.transform.localRotation = Quaternion.Euler(lookVector.y, 0, 0); // Player head is rotated in x axis based on Camera.y, for looking up and down
    }

    public void SetLookDirection(Vector3 lookDirection)
    {
        Quaternion direction = Quaternion.LookRotation(lookDirection, transform.up);
        transform.rotation = Quaternion.Euler(0, direction.y, direction.z);
        head.transform.rotation = Quaternion.Euler(direction.x, transform.rotation.y, direction.z);
        lookVector.y = head.transform.localRotation.x;
    }
    #endregion

    #region Crouching functions
    // Used for handling regular crouch input
    void CrouchHandler()
    {
        #region Determine whether the player is crouching or not
        if (toggleCrouch == true)
        {
            if (Input.GetButtonDown("Crouch"))
            {
                isCrouching = !isCrouching;
            }
        }
        else
        {
            if (Input.GetButton("Crouch"))
            {
                isCrouching = true;
            }
            else
            {
                isCrouching = false;
            }
        }

        float t;
        if (isCrouching)
        {
            t = 1;
        }
        else
        {
            t = -1;
        }
        #endregion

        #region Actual crouching code happens
        /*
        // If current crouch state does not match isCrouching, or if the crouchTimer is not 1 or 0 (indicating that crouching is in progress)
        if (crouchTimer == 1 && isCrouching == false || crouchTimer == 0 && isCrouching == true || (crouchTimer != 1 && crouchTimer != 0))
        {
            crouchTimer += Time.deltaTime / crouchTime * t;
            crouchTimer = Mathf.Clamp01(crouchTimer);
            LerpCrouch(crouchTimer);
        }
        */
        crouchTimer += Time.deltaTime / crouchTime * t;
        crouchTimer = Mathf.Clamp01(crouchTimer);
        LerpCrouch(crouchTimer);
        #endregion
    }

    // Instantly set crouch state. For cancelling crouch animation in case something else needs to happen.
    public void InstantCrouch(bool active)
    {
        float t;
        if (active)
        {
            t = 1;
        }
        else
        {
            t = 0;
        }
        LerpCrouch(t);
        isCrouching = active;
    }

    // Used for actually controlling crouch 'animation'
    void LerpCrouch(float t)
    {
        cc.height = Mathf.Lerp(standHeight, crouchHeight, t);
        crouchSpeedModifier.SetIntensity(t); // Lerps crouch speed multiplier between none and fully active
        head.transform.localPosition = new Vector3(0, Mathf.Lerp(relativeHeadHeight * standHeight, relativeHeadHeight * crouchHeight, t), 0);
    }
    #endregion

    void FixedUpdate()
    {
        rb.MovePosition(transform.position + movementValue * Time.fixedDeltaTime);

        if (willJump)
        {
            rb.velocity += transform.up * forceJump;
            willJump = false;
            jumpTimer = 0;
        }

        //rb.AddForce(Physics.gravity * rb.mass);
    }
}