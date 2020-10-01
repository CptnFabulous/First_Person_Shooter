using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public GameObject head;
    public Camera playerCamera;
    [HideInInspector] public Rigidbody rb;
    CapsuleCollider cc; 

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
    public StatModifier sensitivityModifier = new StatModifier();
    Vector2 lookVector;

    [Header("Movement")]
    public float movementSpeed = 10;
    [Range(-1, 0)]
    public float crouchSpeedMultiplier = -0.5f;

    public StatModifier speedModifier = new StatModifier();
    Vector2 moveInput;
    Vector3 movementValue;

    [Header("Jumping")]
    public float forceJump = 5;
    public float jumpDelay = 0.1f;
    public float groundedRayLength = 0.01f;

    bool willJump;
    float jumpTimer = 9999999;
    LayerMask terrainDetection;

    [Header("Crouching")]
    public float standHeight = 2;
    public float crouchHeight = 1;
    public float crouchTime = 0.25f;
    [Range(-0.5f, 0.5f)]
    public float relativeHeadHeight = 0.375f;
    public bool toggleCrouch;

    float crouchTimer;
    bool isCrouching;


    #region Validate variables
#if UNITY_EDITOR
    void Reset() { OnValidate(); }
    void OnValidate()
    {

        minLookAngle = Mathf.Clamp(minLookAngle, -90, maxLookAngle);
        maxLookAngle = Mathf.Clamp(maxLookAngle, minLookAngle, 90);
        crouchSpeedMultiplier = Mathf.Clamp(crouchSpeedMultiplier, -1, 0);
    }
    #endif
    #endregion

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

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        terrainDetection = Misc.CollisionMask(gameObject.layer);
    }

    // Update is called once per frame
    void Update()
    {
        #region Camera
        if (canLook == true)
        {
            LookAngle(new Vector2(Input.GetAxis("MouseX") * sensitivityModifier.NewFloat(sensitivityX) * Time.deltaTime, Input.GetAxis("MouseY") * sensitivityModifier.NewFloat(sensitivityY) * Time.deltaTime));
        }
        #endregion

        #region Crouching
        HoldOrToggleCrouch();
        if (isCrouching)
        {
            LerpCrouch(crouchTime);
        }
        else
        {
            LerpCrouch(-crouchTime);
        }
        #endregion

        #region Movement
        moveInput.x = Input.GetAxis("Horizontal"); // Set to AD keys or analog stick.
        moveInput.y = Input.GetAxis("Vertical"); // Set to WS keys or analog stick.
        if (moveInput.magnitude > 1)
        {
            moveInput.Normalize(); // Prevent movement input from going past 1. This ensures that players cannot go faster than the normal movement speed.
        }
        float speed = Mathf.Clamp(speedModifier.NewFloat(movementSpeed), 0, Mathf.Infinity);
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
        /*
        else if (Input.GetButton("Jump"))
        {
            // Do jetpack hover
        }
        */
        #endregion
    }

    public void LookAngle(Vector2 cameraInput) // This variable is public so it can be altered by other sources such as gun recoil
    {
        lookVector.x = cameraInput.x;
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

    public void SetToTransform(Transform newTransform)
    {
        transform.position = newTransform.position;
        SetLookDirection(newTransform.forward);
    }

    #region Crouch functions
    void HoldOrToggleCrouch()
    {
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
    }



    void LerpCrouch(float t)
    {
        crouchTimer += Time.deltaTime / t;
        crouchTimer = Mathf.Clamp01(crouchTimer);
        cc.height = Mathf.Lerp(standHeight, crouchHeight, crouchTimer);

        float sm = Mathf.Lerp(0, crouchSpeedMultiplier, crouchTimer); // Lerps crouch speed multiplier between none and the normal amount
        speedModifier.ApplyEffect("Crouching", sm, 0); // Adds crouch speed multiplier to movement speed effects

        head.transform.localPosition = new Vector3(0, Mathf.Lerp(relativeHeadHeight * standHeight, relativeHeadHeight * crouchHeight, crouchTimer), 0);
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

    private void LateUpdate()
    {
        speedModifier.CheckStatDuration();
        sensitivityModifier.CheckStatDuration();
    }
}