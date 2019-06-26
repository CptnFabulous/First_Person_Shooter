using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof (Rigidbody))]
public class BasicMovement : MonoBehaviour
{

    [Header("References")]
    public GameObject head;
    Rigidbody rb;
    Ray isGrounded;
    RaycastHit floor;

    [Header("Camera control")]
    [Tooltip("Camera control sensitivity for the X axis i.e. rotating left and right. Set to minus to invert it.")]
    [Range(-100, 100)]
    public float sensitivityX;
    [Tooltip("Camera control sensitivity for the Y axis i.e. looking up and down. Set to minus to invert it.")]
    [Range(-100, 100)]
    public float sensitivityY;
    Vector2 Camera;

    [Header("Standard Movement")]
    public float speedRun;
    public float speedCrouch;
    public float forceJump;
    public float jumpDelay;
    float speed;
    Vector2 moveInput;
    Vector3 movementValue;
    bool willJump;
    float jumpTimer;

    bool canMove = true;



    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        willJump = false;
        jumpTimer = jumpDelay;
    }

    // Update is called once per frame
    void Update()
    {
        jumpTimer += Time.deltaTime;


        /*
        //X and Y values of Vector2 Camera are set to the moveInput of the player's mouse, multiplied by sensitivityX and sensitivityY to determine the player's camera sensitivity. Camera.y is then clamped to ensure it does not move past 90* or 90*, ensuring that the player does not flip the camera over completely.
        Camera.x = Input.GetAxis("MouseX") * sensitivityX;
        Camera.y -= Input.GetAxis("MouseY") * sensitivityY;
        Camera.y = Mathf.Clamp(Camera.y, -90f, 90f); // Player head rotation is clamped to prevent them from looking too far up or down
        transform.Rotate(0, Camera.x, 0); // Player is rotated on y axis based on Camera.x, for turning left and right
        head.transform.localRotation = Quaternion.Euler(Camera.y, 0, 0); // Player head is rotated in x axis based on Camera.y, for looking up and down
        */

        Camera.x = Input.GetAxis("MouseX") * sensitivityX * Time.deltaTime; // Camera X input is set to player's mouse X axis, multiplied by float 'sensitivityX'.
        Camera.y = Input.GetAxis("MouseY") * -sensitivityY * Time.deltaTime; // Camera Y input is set to player's mouse Y axis, multiplied by float 'sensitivityY'.
        if (canMove)
        {
            //Camera.y = Mathf.Clamp(Camera.y, -90f, 90f); // Player head rotation is clamped between 90* and -90* to prevent them from flipping the camera over completely
            transform.Rotate(0, Camera.x, 0); // Player is rotated on y axis based on Camera.x, for turning left and right
            head.transform.Rotate(Camera.y, 0, 0); // Player head is rotated on x axis based on Camera.y, for looking up and down
        }
        //head.transform.rotation = Quaternion.Euler(Mathf.Clamp(head.transform.rotation.x, -90, 90), head.transform.rotation.y, head.transform.rotation.z);
        
        
        
        //head.transform.localRotation = Quaternion.Euler(Mathf.Clamp(head.transform.localRotation.y, -90f, 90f), 0, 0);

        //head.transform.localRotation = Quaternion.Euler(head.transform.localRotation.y + Camera.y, 0, 0); // Player head is rotated in x axis based on Camera.y, for looking up and down

        moveInput.x = Input.GetAxis("Horizontal"); // Set to AD keys or analog stick.
        moveInput.y = Input.GetAxis("Vertical"); // Set to WS keys or analog stick.
        moveInput.Normalize(); // Subtract X and Y values so they add up to 1 but retain the same ratio. Ensures player moves at same speed regardless of direction.
        movementValue = new Vector3(moveInput.x * speedRun, 0, moveInput.y * speedRun); // X and Y values of Vector2 moveInput are set as X and Z values of Vector3 movementValue, turning horizontal and vertical values into horizontal and lateral ones.
        movementValue = transform.rotation * movementValue; // movementValue is multiplied by transform.rotation so moveInput occurs in the direction the character is facing.

        
        isGrounded.origin = transform.position; // Sets the origin of isGrounded ray to the player's body
        isGrounded.direction = Vector3.down; // Sets isGrounded direction to cast directly down under the player's 'feet'
        float characterWidth = GetComponent<CapsuleCollider>().radius; // Determines width of character
        
        if (Physics.SphereCast(isGrounded, characterWidth, 1.01f)) //Raycast isGrounded is cast to detect if there is a surface underneath the player. If so, canJump boolean is enabled to allow the player to jump off the surface, and disabled if false, i.e. if the player is in midair.
        {
            //print("Is grounded");
            if (Input.GetButtonDown("Jump") && jumpTimer >= jumpDelay)
            {
                willJump = true;
            }
        }
        else
        {
            if (Input.GetButton("Jump"))
            {
                //print("Is hovering");
            }
        }

    }

    void FixedUpdate()
    {
        rb.MovePosition(transform.position + movementValue * Time.deltaTime);

        if (willJump)
        {
            rb.velocity += transform.up * forceJump;
            //rb.velocity = transform.up * forceJump;
            willJump = false;
            jumpTimer = 0;
        }

        rb.AddForce(Physics.gravity * rb.mass);
    }


}