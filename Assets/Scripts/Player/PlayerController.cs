using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    #region General
    [Header("General")]
    public Transform head;
    public Transform torso;
    public Transform lookDirectionTransform;
    public Transform aimDirectionTransform;
    public Camera playerCamera;
    [HideInInspector] public Rigidbody rb;
    CapsuleCollider cc;

    [HideInInspector] public bool canMove = true;
    #endregion

    #region Camera control
    [Header("Camera control")]
    public VariableValueFloat fieldOfView = new VariableValueFloat(60);
    //public float fieldOfView = 60;
    [Tooltip("Horizontal camera sensitivity. Set to minus to invert it."), Range(-100, 100)]
    public float sensitivityX = 50;
    [Tooltip("Vertical camera sensitivity. Set to minus to invert it."), Range(-100, 100)]
    public float sensitivityY = 50;
    [Tooltip("Minimum angle the player can look down."), Range(-90, 90)]
    public float minLookAngle = -90;
    [Tooltip("Maximum angle the player can look up."), Range(-90, 90)]
    public float maxLookAngle = 90;

    [HideInInspector] public bool canLook;
    [HideInInspector] public VariableValueFloat sensitivityModifier = new VariableValueFloat(1);
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

    // Cosmetics
    public Vector2 bobExtents;
    public AnimationCurve bobCurveX;
    public AnimationCurve bobCurveY;
    public float stepCycleTime = 1;
    public int stepsPerCycle = 2;
    public UnityEvent onStep;
    float walkCycleTimer;
    float stepTimer;

    #endregion

    #region Jumping
    [Header("Jumping")]
    public float forceJump = 5;
    public float jumpDelay = 0.1f;
    public float groundedRayLength = 0.01f;
    bool willJump;
    float jumpTimer = float.MaxValue;
    LayerMask terrainDetection;

    // Cosmetics
    public UnityEvent onJump;
    public UnityEvent onLand;
    #endregion

    #region Crouching
    [Header("Crouching")]
    public float standHeight = 2;
    public float crouchHeight = 1;
    public float standHeadHeight = 1.5f;
    public float crouchHeadHeight = 0.8f;
    public float crouchTime = 0.25f;
    public bool toggleCrouch;
    float crouchTimer;
    [HideInInspector] public bool isCrouching;

    // Cosmetics
    public UnityEvent onCrouch;
    public UnityEvent onStand;
    #endregion

    #region Cosmetics

    [Header("Cosmetics - Drag and Sway")]
    
    // Torso lingering/dragging when moving
    public float upperBodyDragDistance;
    public float speedForMaxDrag;


    // Torso swaying/dragging when looking around
    public float lookSwayDegrees;
    public float speedForMaxSway;

    [Header("Cosmetics - Return to default position and rotation")]
    public float torsoResetPositionSpeed = 1;
    //public float torsoResetPositionTime = 1;
    public float torsoResetRotationSpeed = 1;
    //public float torsoResetRotationTime = -0.1f;
    
    Vector3 torsoPosition;
    Vector3 torsoRotationAxes;

    #endregion

    #region Info on player movement
    


    public Vector2 MoveDirection
    {
        get
        {
            return moveInput;
        }
    } // In what direction is the player intending to move?
    Vector3 DeltaMoveDistance
    {
        get
        {
            // Gets a vector3 of the direction the player has moved in since the last frame. It then also changes the magnitude to equal the full distance the player has moved.
            Vector3 v = (transform.position - positionLastFrame).normalized;
            v *= Vector3.Distance(transform.position, positionLastFrame);
            return v;
        }
    } // Figure out how far the player has moved since the last frame, and in what direction
    float DeltaRotateDistance
    {
        get
        {
            //Debug.Log("Old: " + headDirectionLastFrame + " New: " + head.transform.forward);
            return Vector3.Angle(headDirectionLastFrame, head.transform.forward);
        }
    } // Figure out how far the player's rotation has changed since the last frame, and in what direction
    public Vector2 DeltaRotateDirection
    {
        get
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
    } // In what direction is the player turning?

    // Is the player standing on solid ground, or are they airborne?
    void CheckGrounding()
    {
        // Casts a ray to determine if the player is standing on solid ground.
        Ray r = new Ray(transform.position + transform.up * 0.5f, -transform.up);
        isGrounded = Physics.SphereCast(r, cc.radius, 0.5f + groundedRayLength, terrainDetection);

        // If there is a mismatch, this means the player's grounded state has changed.
        if (wasGrounded != isGrounded)
        {
            if (wasGrounded == true) // Player has left the ground
            {

            }
            else // Player has landed
            {
                onLand.Invoke();
            }

            wasGrounded = isGrounded;
        }
    }
    bool isGrounded;
    bool wasGrounded;

    #endregion

    void OnValidate()
    {
        // void Reset() { OnValidate(); }
        // Ensure minLookAngle is not larger than maxLookAngle, or the opposite way around.
        minLookAngle = Mathf.Clamp(minLookAngle, -90, maxLookAngle);
        maxLookAngle = Mathf.Clamp(maxLookAngle, minLookAngle, 90);

        cc = GetComponent<CapsuleCollider>();
        InstantCrouch(isCrouching);
        //cc.height = standHeight;
    }

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        terrainDetection = Misc.CollisionMask(gameObject.layer);

        sensitivityModifier.defaultValue = 1;

        // Add crouch speed modifier to movementSpeed
        movementSpeed.Add(crouchSpeedModifier, this);
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounding();
        
        if (canMove == false)
        {
            return;
        }

        #region Camera
        if (canLook == true)
        {
            float cameraInputX = Input.GetAxis("MouseX") * sensitivityX;
            float cameraInputY = Input.GetAxis("MouseY") * sensitivityY;
            LookAngle(new Vector2(cameraInputX, cameraInputY) * sensitivityModifier.Calculate() * Time.deltaTime);
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
        if (Input.GetButtonDown("Jump") && jumpTimer >= jumpDelay && isGrounded == true) //Raycast isGrounded is cast to detect if there is a surface underneath the player. If so, canJump boolean is enabled to allow the player to jump off the surface, and disabled if false, i.e. if the player is in midair.
        {
            if (isCrouching == true)
            {
                isCrouching = false;
            }

            willJump = true;
        }
        #endregion
    }

    void FixedUpdate()
    {
        rb.MovePosition(transform.position + movementValue * Time.fixedDeltaTime);

        if (willJump)
        {
            rb.velocity += transform.up * forceJump;
            willJump = false;
            jumpTimer = 0;
            onJump.Invoke();
        }

        //rb.AddForce(Physics.gravity * rb.mass);
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
        float colliderCenter = Mathf.Lerp(standHeight * 0.5f, crouchHeight * 0.5f, t);
        cc.center = new Vector3(0, colliderCenter, 0);
        //crouchSpeedModifier.SetIntensity(t); // Lerps crouch speed multiplier between none and fully active
        crouchSpeedModifier.SetActiveFully(isCrouching);
        float headHeight = Mathf.Lerp(standHeadHeight, crouchHeadHeight, t);
        head.transform.localPosition = new Vector3(0, headHeight, 0);
    }
    #endregion

    #region Cosmetics
    private void LateUpdate()
    {
        torsoPosition = Vector3.zero;
        torsoRotationAxes = Vector3.zero;

        WalkCycle();
        TorsoDrag();
        TorsoSway();

        #region Update position and rotation of torso to match drag and sway values


        if (torsoPosition.magnitude > 0)
        {
            torso.localPosition = torsoPosition;
        }
        else
        {
            torso.localPosition = Vector3.MoveTowards(torso.localPosition, torsoPosition, torsoResetPositionSpeed * Time.deltaTime);
        }

        Quaternion torsoRotation = Quaternion.Euler(torsoRotationAxes);
        if (Quaternion.Angle(Quaternion.identity, torsoRotation) > 0)
        {
            torso.localRotation = torsoRotation;
        }
        else
        {
            torso.localRotation = Quaternion.RotateTowards(torso.localRotation, torsoRotation, torsoResetRotationSpeed * Time.deltaTime);
        }

        
        
        //Debug.Log(torsoRotationAxes + ", " + torso.localEulerAngles);

        /*
        Vector3 torsoAnimateVelocity = (torsoPosition - torso.transform.localPosition).normalized * torsoTranslateSpeed;
        torso.transform.localPosition = Vector3.SmoothDamp(torso.transform.localPosition, torsoPosition, ref torsoAnimateVelocity, torsoTranslateTime);

        Vector3 torsoCurrentAngles = torso.localEulerAngles;
        Vector3 torsoAnimateAngleVelocity = (torsoRotationAxes - torsoCurrentAngles).normalized * torsoRotateSpeed;
        Vector3 dampedTorsoRotationAxes = Vector3.SmoothDamp(torsoCurrentAngles, torsoRotationAxes, ref torsoAnimateAngleVelocity, torsoRotateTime);
        torso.transform.localRotation = Quaternion.Euler(dampedTorsoRotationAxes);
        */
        #endregion

        positionLastFrame = transform.position;
        headDirectionLastFrame = head.transform.forward;
    }




    void WalkCycle()
    {
        Vector2 moveInputValue = moveInput;
        if (moveInputValue.magnitude > 0 && isGrounded) // If player is walking
        {
            // Calculate timer for step cycle and bobbing animation
            float speedMagnitude = movementSpeed.Calculate() / movementSpeed.defaultValue;
            float timeToAdd = Time.deltaTime / (stepCycleTime / speedMagnitude);
            

            walkCycleTimer += timeToAdd; // Counts up bobTimer with a value based on walk cycle time and current movement speed
            walkCycleTimer = Misc.InverseClamp(walkCycleTimer, 0, 1); // If timer exceeds one, revert to zero

            // Calculate bob axis values (unclamped lerp is used so values return as -1 to 1)
            //float bobX = Mathf.LerpUnclamped(0, bobExtents.x, bobCurveX.Evaluate(walkCycleTimer));
            //float bobY = Mathf.LerpUnclamped(0, bobExtents.y, bobCurveY.Evaluate(walkCycleTimer));
            float bobX = bobCurveX.Evaluate(walkCycleTimer) * bobExtents.x;
            float bobY = bobCurveY.Evaluate(walkCycleTimer) * bobExtents.y;
            Vector3 bodyPosition = new Vector3(bobX, bobY, 0) * moveInputValue.magnitude * speedMagnitude;
            torsoPosition += bodyPosition;


            // Separate timer for counting up individual steps
            stepTimer += timeToAdd;
            if (stepTimer >= 1f / stepsPerCycle)
            {
                stepTimer = 0;
                onStep.Invoke();
            }
        }
        else if (isCrouching == false)
        {
            // If the player is standing normally but has stopped moving, reset walk and step timers.
            walkCycleTimer = 0;
            stepTimer = 0;
        }
    }
    void TorsoDrag()
    {
        Vector3 velocity = rb.velocity;
        float speed = velocity.magnitude;
        float dragIntensity = Mathf.Clamp01(speed / speedForMaxDrag);
        Vector3 direction = transform.InverseTransformDirection(velocity);
        Vector3 dragMax = direction.normalized * -upperBodyDragDistance;
        Vector3 dragValue = Vector3.Lerp(Vector3.zero, dragMax, dragIntensity);
        torsoPosition += dragValue;
    }
    void TorsoSway()
    {
        float intensity = Mathf.Clamp01(DeltaRotateDistance / speedForMaxSway);
        Vector3 swayAxes = new Vector3(DeltaRotateDirection.y, -DeltaRotateDirection.x, 0);
        swayAxes = Vector3.Lerp(Vector3.zero, swayAxes.normalized * lookSwayDegrees, intensity);
        torsoRotationAxes += swayAxes;
    }





    /* // A guy named Willis on Discord gave me this code. It doesn't do the exact thing I want but it might be helpful anyway.

    private float swayVal = 0.5f;
    private float baselineZAngle = 0f;
    public float swayScalar = 1f;
    public float maxSwayAngle = 30f;

    void Awake()
    {
      baselineZAngle = torso.localRotation.eulerAngles.z;
    }

    void Update()
    {
      swayVal = Mathf.PingPong(Time.time, 1);
    }

    public void UpdateSway()
    {
      var current = torso.localRotation.eulerAngles;
      current.z = Mathf.Lerp(baselineZAngle - (maxSwayAngle * swayScalar / 2), baselineZAngle + (maxSwayAngle * swayScalar / 2), swayVal);
      torso.localRotation = Quaternion.Euler(current);
    }

    */


    /*
    public static IEnumerator ShakeCamera(Camera camera, float duration, float shakesPerSecond, Vector2 intensity)
    {
        // Obtains original camera position
        Vector3 originalPosition = camera.transform.localPosition;
        float timer = 0;




        while (timer < duration)
        {
            timer += Time.deltaTime;

            //float numberOfShakesCompleted = timer * shakesPerSecond;
            //float individualShakeTimer = Misc.SubtractDecimalFromFloat(numberOfShakesCompleted);

            yield return new WaitForEndOfFrame();
        }

        // Returns camera to normal
        camera.transform.localPosition = originalPosition;
    }
    */
    #endregion
}