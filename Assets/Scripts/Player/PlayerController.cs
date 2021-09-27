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
    #endregion

    #region Jumping
    [Header("Jumping")]
    public float forceJump = 5;
    public float jumpDelay = 0.1f;
    public float groundedRayLength = 0.01f;

    bool willJump;
    float jumpTimer = float.MaxValue;
    LayerMask terrainDetection;
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
    public UnityEvent onCrouch;
    public UnityEvent onStand;
    #endregion

    #region Cosmetics

    [Header("Cosmetics - General")]

    public float torsoTranslateSpeed = 1;
    public float torsoTranslateTime = 1;
    public float torsoRotateSpeed = 1;
    public float torsoRotateTime = -0.1f;


    [Header("Cosmetics - Walking")]
    // Head bobbing while walking
    public float bobLoopTime;
    public Vector2 bobExtents;
    public AnimationCurve bobCurveX;
    public AnimationCurve bobCurveY;
    float bobTimer;
    public int numberOfLegsForSteps = 2;
    public UnityEvent onStep;

    [Header("Cosmetics - Torso Drag")]
    // Torso lingering/dragging when moving
    public float upperBodyDragDistance;
    public float speedForMaxDrag;

    [Header("Cosmetics - Torso Sway")]
    // Torso swaying/dragging when looking around
    public float lookSwayDegrees;
    public float speedForMaxSway;
    #endregion

    #region Info on player movement
    // Is the player standing on solid ground, or are they airborne?
    bool IsGrounded
    {
        get
        {
            // Casts a ray to determine if the player is standing on solid ground.
            Ray r = new Ray(transform.position + transform.up * 0.5f, -transform.up);
            if (Physics.SphereCast(r, cc.radius, 0.5f + groundedRayLength, terrainDetection))
            {
                return true;
            }
            return false;
        }
    }

    // In what direction is the player intending to move?
    public Vector2 MoveDirection
    {
        get
        {
            return moveInput;
        }
    }

    // Figure out how far the player has moved since the last frame, and in what direction
    Vector3 DeltaMoveDistance
    {
        get
        {
            // Gets a vector3 of the direction the player has moved in since the last frame. It then also changes the magnitude to equal the full distance the player has moved.
            Vector3 v = (transform.position - positionLastFrame).normalized;
            v *= Vector3.Distance(transform.position, positionLastFrame);
            return v;
        }
    }

    // Figure out how far the player's rotation has changed since the last frame, and in what direction
    float DeltaRotateDistance
    {
        get
        {
            //Debug.Log("Old: " + headDirectionLastFrame + " New: " + head.transform.forward);
            return Vector3.Angle(headDirectionLastFrame, head.transform.forward);
        }
    }

    // In what direction is the player turning?
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
    }
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
            if (Input.GetButtonDown("Jump") && jumpTimer >= jumpDelay && IsGrounded == true) //Raycast isGrounded is cast to detect if there is a surface underneath the player. If so, canJump boolean is enabled to allow the player to jump off the surface, and disabled if false, i.e. if the player is in midair.
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
        Vector3 torsoPosition = Vector3.zero;
        Vector3 torsoRotationAxes = Vector3.zero;

        #region Bobbing and footsteps
        Vector2 moveInputValue = moveInput;
        if (moveInputValue.magnitude > 0 && IsGrounded) // If player is walking
        {
            float speedMagnitude = movementSpeed.Calculate() / movementSpeed.defaultValue;
            float time = bobLoopTime / speedMagnitude;


            bobTimer += Time.deltaTime / time;
            bobTimer = Misc.InverseClamp(bobTimer, 0, 1);

            //if (bobTimer >= )

            float bobX = Mathf.LerpUnclamped(0, bobExtents.x, bobCurveX.Evaluate(bobTimer)) * moveInputValue.magnitude * speedMagnitude;
            float bobY = Mathf.LerpUnclamped(0, bobExtents.y, bobCurveY.Evaluate(bobTimer)) * moveInputValue.magnitude * speedMagnitude;

            Vector3 bodyPosition = new Vector3(bobX, bobY, 0);
            torsoPosition += bodyPosition;


        }
        else
        {
            bobTimer = 0;
        }
        #endregion

        #region Drag
        //Vector3 velocity = DeltaMoveDistance();
        //float speed = velocity.magnitude / Time.deltaTime;
        Vector3 velocity = rb.velocity;
        float speed = velocity.magnitude;
        float dragIntensity = Mathf.Clamp01(speed / speedForMaxDrag);
        Vector3 direction = transform.InverseTransformDirection(velocity);
        Vector3 dragMax = direction.normalized * -upperBodyDragDistance;
        Vector3 dragValue = Vector3.Lerp(Vector3.zero, dragMax, dragIntensity);
        torsoPosition += dragValue;

        #endregion

        #region Sway
        float intensity = Mathf.Clamp01(DeltaRotateDistance / speedForMaxSway);
        Vector3 swayAxes = new Vector3(DeltaRotateDirection.y, -DeltaRotateDirection.x, 0);
        swayAxes = Vector3.Lerp(Vector3.zero, swayAxes.normalized * lookSwayDegrees, intensity);
        torsoRotationAxes += swayAxes;
        #endregion

        #region Update position
        Vector3 torsoAnimateVelocity = (torsoPosition - torso.transform.localPosition).normalized * torsoTranslateSpeed;
        torso.transform.localPosition = Vector3.SmoothDamp(torso.transform.localPosition, torsoPosition, ref torsoAnimateVelocity, torsoTranslateTime);
        #endregion

        #region Update rotation

        //Vector3 v = new Vector3(torso.lo)

        Vector3 torsoCurrentAngles = new Vector3(torso.localRotation.x, torso.localRotation.y, torso.localRotation.z);

        //Vector3 torsoCurrentAngles = torso.localEulerAngles;


        Vector3 torsoAnimateAngleVelocity = (torsoRotationAxes - torsoCurrentAngles).normalized * torsoRotateSpeed;
        Vector3 dampedTorsoRotationAxes = Vector3.SmoothDamp(torsoCurrentAngles, torsoRotationAxes, ref torsoAnimateAngleVelocity, torsoRotateTime);
        torso.transform.localRotation = Quaternion.Euler(dampedTorsoRotationAxes);
        #endregion




        positionLastFrame = transform.position;
        headDirectionLastFrame = head.transform.forward;
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