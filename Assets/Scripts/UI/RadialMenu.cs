using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour
{
    [Header("GUI")]
    public Image iconPrefab;
    public Transform cursorAxis;
    public Image highlight;
    public bool lockHighlightRotation;
    Transform radiusMarker;
    public float wheelRadius;
    [Range(-180, 180)]
    public float rotationOffset;
    AudioSource audioSource;
    public AudioClip scrollFeedback;
    int slots;
    int selectedIndex;
    Image[] wheelIcons = new Image[0];
    bool isActive;

    [Header("Input")]
    public string buttonName;
    public bool toggleInput;
    public float mouseThreshold;
    Vector3 relativeInputPosition;

    #region Variables
    public int ReturnIndex // Gets the radial menu's current index
    {
        get
        {
            return Mathf.Clamp(selectedIndex, 0, slots - 1);
        }
    }

    public bool MenuIsActive // Is the menu active and functioning?
    {
        get
        {
            return gameObject.activeSelf;
        }
    }

    public bool SelectionMade() // Returns true on the frame that the player exits the radial menu. Used to determine when to switch.
    {
        if (isActive == false && MenuIsActive == true)
        {
            //print(Cursor.lockState);
            gameObject.SetActive(false);
            return true;
        }
        return false;
    }
    #endregion

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void WheelHandler()
    {
        if (slots > 1)
        {
            #region Player input

            if (Input.GetButtonDown(buttonName))
            {
                isActive = !isActive;
                if (isActive == true)
                {
                    relativeInputPosition = Vector3.zero;
                }
            }
            if (!Input.GetButton(buttonName) && toggleInput == false)
            {
                isActive = false;
            }

            #endregion

            #region Functions while active

            if (isActive) // If button is pressed and there is another slot to swap to
            {
                gameObject.SetActive(true);
                
                relativeInputPosition += new Vector3(Input.GetAxis("MouseX"), Input.GetAxis("MouseY"));
                Vector3 relativeControllerPosition = new Vector3(Input.GetAxis("StickAim_X"), Input.GetAxis("StickAim_Y"));
                if (relativeControllerPosition != Vector3.zero)
                {
                    relativeInputPosition = relativeControllerPosition;
                }
                
                if (relativeInputPosition.magnitude > mouseThreshold)
                {
                    relativeInputPosition = new Vector3(Mathf.Clamp(relativeInputPosition.x, -mouseThreshold, mouseThreshold), Mathf.Clamp(relativeInputPosition.y, -mouseThreshold, mouseThreshold));
                }

                RectTransform rt = GetComponent<RectTransform>();

                float selectAngle = Vector3.SignedAngle(rt.rotation * relativeInputPosition, rt.up, rt.forward); // Produce an angle based on the player's mouse 'position' relative to the radial menu.

                cursorAxis.localRotation = Quaternion.Euler(0, 0, -selectAngle);

                selectAngle -= rotationOffset; // Angle is changed based on rotationOffset to account for the changed positions of the icons based on rotationOffset

                if (selectAngle < 0)
                {
                    selectAngle += 360;
                }
                float segmentSize = 360 / slots;
                int index = Mathf.RoundToInt(selectAngle / segmentSize);
                if (index >= slots)
                {
                    index = 0;
                }

                if (selectedIndex != index)
                {
                    selectedIndex = index;
                    audioSource.PlayOneShot(scrollFeedback);
                    highlight.rectTransform.anchoredPosition = wheelIcons[selectedIndex].rectTransform.anchoredPosition;
                    if (lockHighlightRotation == false)
                    {
                        float segmentAngle = (segmentSize * selectedIndex) + rotationOffset;
                        highlight.rectTransform.localRotation = Quaternion.Euler(0, 0, -segmentAngle);
                    }
                    else
                    {
                        highlight.rectTransform.localRotation = Quaternion.identity;
                    }
                }

                #endregion
            }
        }
    }

    public void RefreshWheel(Sprite[] icons)
    {
        slots = icons.Length;
        if (wheelIcons.Length != slots)
        {
            // Make a new array of icons
            Image[] newWheelIcons = new Image[slots];
            // Go through both the new and old icons. I used Mathf.Max to ensure it runs enough times to go through whichever is larger.
            for (int i = 0; i < Mathf.Max(wheelIcons.Length, newWheelIcons.Length); i++)
            {
                if (i >= newWheelIcons.Length)
                {
                    // If i has exceeded the length of the new array, there are too many icons. Start destroying them
                    if (wheelIcons[i] != null)
                    {
                        Destroy(wheelIcons[i].gameObject);
                    }
                }
                else if (i >= wheelIcons.Length)
                {
                    // If i exceeds the length of the old one, there are not enough. Start creating new ones
                    newWheelIcons[i] = Instantiate(iconPrefab, transform);
                }
                else
                {
                    // Assign an existing icon to the new array to save unnecessary destruction and garbage collection
                    newWheelIcons[i] = wheelIcons[i];
                }
            }
            // Overwrite old array with new one
            wheelIcons = newWheelIcons;

            // Set the positions of each icon
            for (int i = 0; i < wheelIcons.Length; i++)
            {
                float segmentSize = 360 / slots;
                float segmentAngle = (segmentSize * i) + rotationOffset;
                Vector3 iconPosition = Quaternion.Euler(0, 0, -segmentAngle) * new Vector3(0, wheelRadius, 0);
                wheelIcons[i] = Instantiate(iconPrefab, transform);
                wheelIcons[i].transform.localPosition = iconPosition;
                wheelIcons[i].transform.localRotation = Quaternion.identity;
            }
        }
        
        for (int i = 0; i < wheelIcons.Length; i++)
        {
            if (icons[i] != null)
            {
                wheelIcons[i].sprite = icons[i];
            }
        }
    }
}
