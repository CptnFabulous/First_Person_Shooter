using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    #region General
    [Header("General")]
    public Transform head;
    public Transform torso;
    public Camera playerCamera;
    [HideInInspector] public Rigidbody rb;
    CapsuleCollider cc;

    [HideInInspector] public bool canMove = true;
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

    Vector3 headDirectionLastFrame;
    #endregion

    #region Movement
    [Header("Movement")]
    public VariableValueFloat movementSpeed;
    public PercentageModifier crouchSpeedModifier;
    Vector2 moveInput;
    Vector3 movementValue;

    Vector3 positionLastFrame;
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

    #region Cosmetics
    [Header("Cosmetics")]
    // Noises
    public AudioClip footstepNoise;
    public float alternateStepPitchShift;
    public float footstepDelay;
    public AudioClip jumpNoise;
    public AudioClip landNoise;
    // public AudioClip slideNoise;

    // Head bobbing while walking
    public float bobLoopTime;
    public Vector2 bobExtents;
    public AnimationCurve bobCurveX;
    public AnimationCurve bobCurveY;
    float bobTimer;
    
    // Torso lingering/dragging when moving
    public float upperBodyLingerDistance;
    public float speedForMaxLinger;

    // Torso swaying/dragging when looking around
    public float lookSwayDegrees;
    public float speedForMaxSway;
    #endregion

    #region Info on player movement
    // Is the player standing on solid ground, or are they airborne?
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

    // In what direction is the player intending to move?
    public Vector2 MoveDirection()
    {
        return moveInput;
    }

    // Figure out how far the player has moved since the last frame, and in what direction
    Vector3 DeltaMoveDistance()
    {
        // Gets a vector3 of the direction the player has moved in since the last frame. It then also changes the magnitude to equal the full distance the player has moved.
        Vector3 v = (transform.position - positionLastFrame).normalized;
        v *= Vector3.Distance(transform.position, positionLastFrame);
        return v;
    }

    // Figure out how far the player's rotation has changed since the last frame, and in what direction
    float DeltaRotateDistance()
    {
        Debug.Log("Old: " + headDirectionLastFrame + " New: " + head.transform.forward);
        return Vector3.Angle(headDirectionLastFrame, head.transform.forward);
    }

    // In what direction is the player turning?
    public Vector2 DeltaRotateDirection()
    {
        Vector3 outOld = headDirectionLastFrame.normalized;
        Vector3 outNew = head.transform.forward.normalized;

        // Obtains the 'direction' the player looks in.
        Vector3 direction = (outNew - outOld).normalized;

        // Converts the direction from world to local space.
        direction = transform.InverseTransformDirection(direction);

        return new Vector2(direction.x, direction.y).normalized;


        // Now I have a direction value. I just need to convert it to be relative to the transform's head.
    }
    #endregion

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
        //Vector3 m = DeltaMoveDistance();
        //Debug.Log("Moved " + m.magnitude + " distance in " + m.normalized + "direction. Rotated " + DeltaRotateDistance() + " distance in " + DeltaRotateDirection() + " direction.");
        
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

        CosmeticUpdate();
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

    public void LookAt(Vector3 lookDirection)
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
        //crouchSpeedModifier.SetIntensity(t); // Lerps crouch speed multiplier between none and fully active
        crouchSpeedModifier.SetActiveFully(isCrouching);
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

    private void LateUpdate()
    {
        positionLastFrame = transform.position;
        headDirectionLastFrame = head.transform.forward;
    }


    void CosmeticUpdate()
    {
        RunAnimationHandler();
        
        
        //torso.transform.localPosition
    }



    void RunAnimationHandler()
    {
        Vector3 bodyPosition = Vector3.zero;
        
        
        Vector2 moveInputValue = moveInput;
        if (moveInputValue.magnitude > 0 && IsGrounded())
        {
            float speedMagnitude = movementSpeed.Calculate() / movementSpeed.defaultValue;
            float time = bobLoopTime / speedMagnitude;

            bobTimer += Time.deltaTime / time;
            bobTimer = Misc.InverseClamp(bobTimer, 0, 1);

            float bobX = Mathf.LerpUnclamped(0, bobExtents.x, bobCurveX.Evaluate(bobTimer)) * moveInputValue.magnitude * speedMagnitude;
            float bobY = Mathf.LerpUnclamped(0, bobExtents.y, bobCurveY.Evaluate(bobTimer)) * moveInputValue.magnitude * speedMagnitude;

            bodyPosition = new Vector3(bobX, bobY, 0);
        }
        else
        {
            bobTimer = 0;
        }

        torso.transform.localPosition = bodyPosition;
        
        

    }

    /*
    void CosmeticUpdate()
    {
        Vector3 torsoPosition = Vector3.zero;

        // Add together various animation forces on the player
        #region Run animation handler
        Vector3 bobBodyPosition = Vector3.zero;
        Vector2 moveInputValue = moveInput;
        if (moveInputValue.magnitude > 0 && IsGrounded())
        {
            float speedMagnitude = movementSpeed.Calculate() / movementSpeed.defaultValue;
            float time = bobLoopTime / speedMagnitude;

            bobTimer += Time.deltaTime / time;
            bobTimer = Misc.InverseClamp(bobTimer, 0, 1);

            float bobX = Mathf.LerpUnclamped(0, bobExtents.x, bobCurveX.Evaluate(bobTimer)) * moveInputValue.magnitude * speedMagnitude;
            float bobY = Mathf.LerpUnclamped(0, bobExtents.y, bobCurveY.Evaluate(bobTimer)) * moveInputValue.magnitude * speedMagnitude;

            bobBodyPosition = new Vector3(bobX, bobY, 0);
        }
        else
        {
            bobTimer = 0;
        }

        torsoPosition += bobBodyPosition;
        #endregion

        torso.transform.localPosition = torsoPosition;
    }



    void RunAnimationHandler()
    {
        

        
        
        

    }
    */


}