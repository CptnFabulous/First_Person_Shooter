using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteScreen : MonoBehaviour
{
    public Text remainingOptionalObjectives;
    public Button nextLevelButton;
    
    /*
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    */
    public void GenerateScreen(ObjectiveHandler oh)
    {
        


        #region Update optional objective list
        string message = "Optional objectives completed:";
        int remaining = 0;
        bool optionalObjectives = false;

        foreach(PlayerObjective o in oh.objectives)
        {
            if (o.mandatory == false)
            {
                optionalObjectives = true;
                if (o.state == ObjectiveState.Completed)
                {
                    message += "\n";
                    message += o.name;
                }
                else
                {
                    remaining += 1;
                }
            }
        }

        if (optionalObjectives == true)
        {
            message += "\n";
            if (remaining > 0)
            {
                message += remaining;

                if (remaining > 1)
                {
                    message += " objectives";
                }
                else
                {
                    message += " objective";
                }

                message += " remaining";
            }
            else
            {
                message += "All completed";
            }
        }
        else
        {
            message = "No optional objectives";
        }
        
        remainingOptionalObjectives.text = message;
        #endregion

        if (oh.nextLevelName == "")
        {
            nextLevelButton.interactable = false;
        }
        else
        {
            nextLevelButton.onClick.AddListener(() => GetComponent<Menu>().LoadScene(oh.nextLevelName));
        }
    }
}
