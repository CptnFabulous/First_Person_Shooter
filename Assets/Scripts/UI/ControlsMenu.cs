using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ControlsMenu : MonoBehaviour
{
    public KeyCode jump;
    public KeyCode crouch;
    public KeyCode shoot;
    
    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */
    

    public IEnumerator CheckForNewInputToAssign(KeyCode inputToUpdate)
    {
        WaitForEndOfFrame loop = new WaitForEndOfFrame();
        // Check if a new key has been pressed down this frame.
        // It's very unlikely that more than one key will be pressed at the same time
        // But if it does it will just select the first one in the list
        while (GetAllKeysPressed.Length <= 0)
        {
            // If no new keys are pressed, wait until next frame and check again
            yield return loop;
        }
        // If the loop is 
        inputToUpdate = GetAllKeysPressed[0];
    }




    public static KeyCode[] GetAllKeysHeld
    {
        get
        {
            List<KeyCode> inputs = new List<KeyCode>();
            foreach (KeyCode input in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(input))
                {
                    inputs.Add(input);
                }
            }
            return inputs.ToArray();
        }
    }

    public static KeyCode[] GetAllKeysPressed
    {
        get
        {
            List<KeyCode> inputs = new List<KeyCode>();
            foreach (KeyCode input in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(input))
                {
                    inputs.Add(input);
                }
            }
            return inputs.ToArray();
        }
    }

    public static KeyCode[] GetAllKeysReleased
    {
        get
        {
            List<KeyCode> inputs = new List<KeyCode>();
            foreach (KeyCode input in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyUp(input))
                {
                    inputs.Add(input);
                }
            }
            return inputs.ToArray();
        }
    }



    /*
    static Sprite[] controllerIcons = new Sprite[]
    {
        (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Art/UI/Icons/Input/InputIcon_A", typeof(Sprite))
    };


    public static KeyCode[] inputsWithIcons = new KeyCode[]
    {
        
    };

    public static Sprite GetControllerInputIcon(KeyCode input)
    {
        for (int i = 0; i < inputsWithIcons.Length; i++)
        {
            if (inputsWithIcons[i] == input)
            {
                return controllerIcons[i];
            }
        }
        return null;
    }



    public static Sprite GetPrompt(KeyCode input)
    {
        Sprite icon;


        string filePath = "Assets/Art/UI/Icons/Input/InputIcon_";
        switch(input)
        {
            case KeyCode.A:
                filePath += "A";
                break;
            default:
                filePath += "A";
                break;
        }

        icon = (Sprite)AssetDatabase.LoadAssetAtPath(filePath, typeof(Sprite));

        return icon;
    }
    */
}
