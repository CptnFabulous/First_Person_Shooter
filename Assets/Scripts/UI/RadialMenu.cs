using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class RadialMenu : MonoBehaviour
{
    [Header("Input")]
    public Dropdown.DropdownEvent onValueChanged;
    public Dropdown.DropdownEvent onValueConfirmed;
    //public UnityEvent<int> onValueChanged;
    //public UnityEvent<int> onValueConfirmed;
    public bool toggleInput;
    public string buttonName;
    public float mouseThreshold;
    int currentIndex;
    public bool InSelection { get; private set; }
    Vector3 relativeInputPosition;

    [Header("GUI")]
    public Image iconPrefab;
    public Transform cursorAxis;
    public Image highlight;
    public bool lockHighlightRotation;
    public float wheelRadius;
    [Range(-180, 180)]
    public float rotationOffset;
    Image[] selectables = new Image[0];

    RectTransform rt;
    CanvasGroup elementVisibility;




    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        elementVisibility = GetComponent<CanvasGroup>();
        ExitSelection();
    }


    public void Control()
    {
        #region Check to activate menu
        // If there's only one thing (or nothing) to select, no need to run switching code
        if (selectables.Length <= 1)
        {
            return;
        }
        
        // Check input to open menu
        if (Input.GetButtonDown(buttonName) && InSelection == false)
        {
            EnterSelection();
        }

        // If window was not activated, exit prematurely
        if (InSelection == false)
        {
            return;
        }
        #endregion

        #region Make selection if active
        relativeInputPosition += new Vector3(Input.GetAxis("MouseX"), Input.GetAxis("MouseY"));
        Vector3 relativeControllerPosition = new Vector3(Input.GetAxis("StickAim_X"), Input.GetAxis("StickAim_Y"));
        if (relativeControllerPosition.magnitude > 0)
        {
            relativeInputPosition = relativeControllerPosition;
        }
        if (relativeInputPosition.magnitude > mouseThreshold)
        {
            relativeInputPosition = relativeInputPosition.normalized * mouseThreshold;
            //relativeInputPosition = new Vector3(Mathf.Clamp(relativeInputPosition.x, -mouseThreshold, mouseThreshold), Mathf.Clamp(relativeInputPosition.y, -mouseThreshold, mouseThreshold));
        }

        float selectionAngle = Vector3.SignedAngle(rt.rotation * relativeInputPosition, rt.up, rt.forward); // Produce an angle based on the player's mouse 'position' relative to the radial menu.
        cursorAxis.localRotation = Quaternion.Euler(0, 0, -selectionAngle);
        selectionAngle -= rotationOffset; // Angle is adjusted by rotationOffset to account for the changed positions of the icons based on rotationOffset
        if (selectionAngle < 0)
        {
            selectionAngle += 360;
        }

        // Calculates segment size and divides angle to determine a selection index
        float segmentSize = 360 / selectables.Length;
        int newIndex = Mathf.RoundToInt(selectionAngle / segmentSize);
        if (newIndex >= selectables.Length)
        {
            newIndex = 0;
        }

        if (currentIndex != newIndex)
        {
            UpdateSelection(newIndex);
        }
        #endregion

        #region Check to deactivate menu
        bool released = Input.GetButton(buttonName) == false && toggleInput == false;
        bool depressed = Input.GetButtonDown(buttonName) && toggleInput == true;
        if ((released || depressed) && InSelection == true)
        {
            ExitSelection();
        }
        #endregion
    }
    public void RefreshOptions(Sprite[] icons)
    {
        Image[] newSelectables = new Image[icons.Length];

        // Determines which array is larger so all selections in the old and new array are accounted for
        int min = Mathf.Min(selectables.Length, newSelectables.Length);
        int max = Mathf.Max(selectables.Length, newSelectables.Length);

        for (int i = 0; i < max; i++)
        {
            //Debug.Log(i);
            if (i >= min) // If there is a difference in the array lengths
            {
                // If the loop has surpassed the amount of necessary icons, destroy extras
                if (newSelectables.Length <= selectables.Length)
                {
                    Debug.Log(i + ", destroying " + selectables[i].name);
                    Destroy(selectables[i].gameObject);
                    continue;
                }
                else // If there are more new icons than existing images, make more
                {
                    newSelectables[i] = Instantiate(iconPrefab, transform);
                }
            }
            else // Reuse existing icons
            {
                newSelectables[i] = selectables[i];
            }

            // Determine angle for icon based off order
            float segmentAngle = (360 / newSelectables.Length * i) + rotationOffset;
            Vector3 iconPosition = Quaternion.Euler(0, 0, -segmentAngle) * new Vector3(0, wheelRadius, 0);
            newSelectables[i].transform.localPosition = iconPosition;
            newSelectables[i].transform.localRotation = Quaternion.identity;
            newSelectables[i].sprite = icons[i];
            newSelectables[i].name = icons[i].name;
        }

        selectables = newSelectables;
    }

    void EnterSelection()
    {
        InSelection = true;
        relativeInputPosition = Vector3.zero;
        elementVisibility.alpha = 1;

        onValueChanged.Invoke(currentIndex);
    }
    void UpdateSelection(int newIndex)
    {
        currentIndex = newIndex;
        highlight.rectTransform.anchoredPosition = selectables[currentIndex].rectTransform.anchoredPosition;
        if (lockHighlightRotation == false)
        {
            float segmentSize = 360 / selectables.Length;
            float segmentAngle = (segmentSize * currentIndex) + rotationOffset;
            highlight.rectTransform.localRotation = Quaternion.Euler(0, 0, -segmentAngle);
        }
        else
        {
            highlight.rectTransform.localRotation = Quaternion.identity;
        }

        onValueChanged.Invoke(newIndex);
    }
    void ExitSelection()
    {
        InSelection = false;
        onValueConfirmed.Invoke(currentIndex);
        elementVisibility.alpha = 0;
    }
    
    


}
