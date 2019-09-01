using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{

    [Header("References")]
    Rigidbody rb;
    CapsuleCollider cc;
    Ray isGrounded;
    RaycastHit floor;

    [Header("Camera control")]
    public Camera playerCamera;
    [Range(0, 180)]
    public float fieldOfView = 60;
    public GameObject head;
    [Tooltip("Camera control sensitivity for the X axis i.e. rotating left and right. Set to minus to invert it.")]
    [Range(-100, 100)]
    public float sensitivityX = 50;
    [Tooltip("Camera control sensitivity for the Y axis i.e. looking up and down. Set to minus to invert it.")]
    [Range(-100, 100)]
    public float sensitivityY = 50;

    //[HideInInspector] public float cameraSensitivityModifier;

    [Range(-90, 90)]
    public float minLookAngle = -90;
    [Range(-90, 90)]
    public float maxLookAngle = 90;

    Vector2 lookVector;

    [Header("Standard Movement")]
    [Tooltip("The player's standard movement speed.")]
    public float movementSpeed = 10;
    [Range(-1, 0)]
    public float crouchSpeedMultiplier = -0.5f;


    public StatModifier speedModifier = new StatModifier(); // StatModifier for movement speed, this does not work


    public float forceJump = 5;
    public float jumpDelay = 0.1f;

    float speed;

    Vector2 moveInput;
    Vector3 movementValue;
    bool willJump;
    float jumpTimer = 9999999;

    [Header("Crouching")]
    public float standHeight = 2;
    public float crouchHeight = 1;
    public float crouchTime = 0.25f;
    [Range(-1, 1)]
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

        if (isCrouching == true)
        {
            head.transform.localPosition = new Vector3(0, relativeHeadHeight * crouchHeight, 0);
        }
        else
        {
            head.transform.localPosition = new Vector3(0, relativeHeadHeight * standHeight, 0);
        }
    }
    #endif
    #endregion

    bool IsGrounded()
    {
        isGrounded.origin = transform.position; // Sets the origin of isGrounded ray to the player's body
        isGrounded.direction = Vector3.down; // Sets isGrounded direction to cast directly down under the player's 'feet'
        if (Physics.SphereCast(isGrounded, cc.radius, cc.height / 2 + 0.01f)) //Raycast isGrounded is cast to detect if there is a surface underneath the player. If so, canJump boolean is enabled to allow the player to jump off the surface, and disabled if false, i.e. if the player is in midair.
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
    }

    // Update is called once per frame
    void Update()
    {
        #region Camera
        LookAngle(new Vector2(Input.GetAxis("MouseX") * sensitivityX * Time.deltaTime, Input.GetAxis("MouseY") * sensitivityY * Time.deltaTime));
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
        //print(cc.height + "/" + movementSpeed + "/" + head.transform.localPosition.y);
        // LERP CODE IS SCREWY AND DOESN'T ROUND PROPERLY
        //print(cc.height + ", " + head.transform.localPosition.y + ", " + speed);
        //print(crouchTimer + ", " + cc.height + ", " + head.transform.localPosition.y + ", " + speed);
        //print("Crouch timer = " + crouchTimer + ", height = " + cc.height + ", head height = " + head.transform.localPosition.y + ", speed = " + speed);
        #endregion

        #region Movement
        moveInput.x = Input.GetAxis("Horizontal"); // Set to AD keys or analog stick.
        moveInput.y = Input.GetAxis("Vertical"); // Set to WS keys or analog stick.
        if (moveInput.magnitude > 1)
        {
            moveInput.Normalize();
        }
        //movementValue = new Vector3(moveInput.x * speed, 0, moveInput.y * speed); // X and Y values of Vector2 moveInput are set as X and Z values of Vector3 movementValue, turning horizontal and vertical values into horizontal and lateral ones.



        // This line uses the statModifier code that doesn't work properly
        movementValue = new Vector3(moveInput.x * speedModifier.ModifiedFloat(movementSpeed), 0, moveInput.y * speedModifier.ModifiedFloat(movementSpeed));



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

        //speed = Mathf.Lerp(movementSpeed, movementSpeed * -crouchSpeedMultiplier, crouchTimer);



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

        rb.AddForce(Physics.gravity * rb.mass);
    }
}