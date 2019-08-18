using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class BasicMovement : MonoBehaviour
{

    [Header("References")]
    public GameObject head;
    Rigidbody rb;
    CapsuleCollider cc;
    Ray isGrounded;
    RaycastHit floor;

    [Header("Camera control")]
    [Tooltip("Camera control sensitivity for the X axis i.e. rotating left and right. Set to minus to invert it.")]
    [Range(-100, 100)]
    public float sensitivityX = 50;
    [Tooltip("Camera control sensitivity for the Y axis i.e. looking up and down. Set to minus to invert it.")]
    [Range(-100, 100)]
    public float sensitivityY = 50;
    Vector2 Camera;

    [Header("Standard Movement")]
    public float speedRun = 10;
    public float speedCrouch = 5;
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
    public float relativeHeadHeight = 0.375f;
    public float crouchTime = 0.25f;
    float crouchTimer;
    bool isCrouching;

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
        AlterHeight(crouchTimer);
    }

    // Update is called once per frame
    void Update()
    {
        jumpTimer += Time.deltaTime;

        #region Camera
        Camera.x = Input.GetAxis("MouseX") * sensitivityX * Time.deltaTime; // Camera X input is set to player's mouse X axis, multiplied by float 'sensitivityX'.
        Camera.y -= Input.GetAxis("MouseY") * sensitivityY * Time.deltaTime; // Camera Y input is set to player's mouse Y axis, multiplied by float 'sensitivityY'.
        Camera.y = Mathf.Clamp(Camera.y, -90f, 90f); // Camera.y is then clamped to ensure it does not move past 90* or 90*, ensuring that the player does not flip the camera over completely.
        transform.Rotate(0, Camera.x, 0); // Player is rotated on y axis based on Camera.x, for turning left and right
        head.transform.localRotation = Quaternion.Euler(Camera.y, 0, 0); // Player head is rotated in x axis based on Camera.y, for looking up and down
        #endregion

        #region Crouching
        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = !isCrouching;
        }

        if (isCrouching && crouchTimer <= 1)
        {
            crouchTimer += Time.deltaTime / crouchTime;
            AlterHeight(crouchTimer);
        }
        else if (!isCrouching && crouchTimer >= 0)
        {
            crouchTimer -= Time.deltaTime / crouchTime;
            AlterHeight(crouchTimer);
        }
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
        movementValue = new Vector3(moveInput.x * speed, 0, moveInput.y * speed); // X and Y values of Vector2 moveInput are set as X and Z values of Vector3 movementValue, turning horizontal and vertical values into horizontal and lateral ones.
        movementValue = transform.rotation * movementValue; // movementValue is multiplied by transform.rotation so moveInput occurs in the direction the character is facing.
        #endregion

        #region Jumping
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

    void AlterHeight(float t)
    {
        cc.height = Mathf.Lerp(standHeight, crouchHeight, t);
        speed = Mathf.Lerp(speedRun, speedCrouch, t);
        head.transform.localPosition = new Vector3(0, Mathf.Lerp(relativeHeadHeight * standHeight, relativeHeadHeight * crouchHeight, t), 0);
    }

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