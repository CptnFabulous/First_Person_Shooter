/*

ATTENTION! POSITIONS OF GUI ELEMENTS MESS UP WHEN CANVAS IS SET TO SCREEN SPACE - CAMERA! FIX IT SOMEHOW!

*/

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
    public float wheelRadius;
    [Range(-180, 180)]
    public float rotationOffset;
    AudioSource audioSource;
    public AudioClip scrollFeedback;
    int slots;
    int selectedIndex;
    Image[] wheelIcons = new Image[1];
    bool isActive;

    [Header("Input")]
    public string buttonName;
    public bool toggleInput;
    public float mouseThreshold;
    Vector3 relativeInputPosition;

    #region Variables
    public int ReturnIndex()
    {
        return Mathf.Clamp(selectedIndex, 0, slots - 1);
    }

    public bool MenuIsActive()
    {
        return gameObject.activeSelf;
    }

    public bool SelectionMade()
    {
        if (isActive == false && MenuIsActive() == true)
        {
            //Cursor.lockState = CursorLockMode.Locked;
            print(Cursor.lockState);
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
            if (toggleInput == true)
            {
                if (Input.GetButtonDown(buttonName))
                {
                    isActive = !isActive;
                }
            }
            else
            {
                if (Input.GetButton(buttonName))
                {
                    isActive = true;
                }
                else
                {
                    isActive = false;
                }
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

                float selectAngle = -Vector3.SignedAngle(transform.up, relativeInputPosition, transform.forward);

                cursorAxis.rotation = Quaternion.Euler(0, 0, -selectAngle);

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
                        highlight.rectTransform.rotation = Quaternion.Euler(0, 0, -segmentAngle);
                    }
                    else
                    {
                        highlight.rectTransform.rotation = Quaternion.identity;
                    }
                }
                #endregion
            }
        }
    }

    public void RefreshWheel(int slotCount, Sprite[] icons)
    {
        slots = slotCount;
        if (wheelIcons.Length != slots)
        {
            foreach (Image i in wheelIcons)
            {
                if (i != null)
                {
                    Destroy(i.gameObject);
                }
            }
            wheelIcons = new Image[slots];
            for (int i = 0; i < wheelIcons.Length; i++) // Put all icons from wheelIcons into newWheelIcons, then instantiate new icons until there is an icon for every item
            {
                Image icon = Instantiate(iconPrefab, Vector3.zero, Quaternion.identity, transform);
                wheelIcons[i] = icon;

                float segmentSize = 360 / slots;
                float segmentAngle = (segmentSize * i) + rotationOffset;
                Vector3 iconPosition = Quaternion.Euler(0, 0, -segmentAngle) * new Vector3(0, wheelRadius, 0);
                wheelIcons[i].rectTransform.anchoredPosition = iconPosition;
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

    

    /*
    float InverseClamp(float input, float min, float max)
    {
        if (input > max)
        {
            input = min;
        }
        else if (input < min)
        {
            input = max;
        }
        return input;
    }

    int InverseClamp(int input, int min, int max)
    {
        if (input > max)
        {
            input = min;
        }
        else if (input < min)
        {
            input = max;
        }
        return input;
    }
    */
}
