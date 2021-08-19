using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Rendering;

public class VideoSettingsMenu : SettingsMenu
{
    PlayerHandler playerToUpdate;

    [Header("Interactables for settings")]
    public Dropdown fullscreen;

    public Dropdown graphicalQualityPreset;

    public bool applyExpensiveQualityPresetChanges = true;
    public Dropdown monitor;


    public Dropdown resolution;
    public Dropdown refreshRate;
    Resolution[][] table;

    
    /// <summary>
    /// Gets all the Resolution structs from Screen.resolutions, and organises them into a handy array of arrays, compartmentalising based on both dimensions and refresh rate.
    /// Each resolution has its own sub-array, with each value in it being a different refresh rate.
    /// </summary>
    public static Resolution[][] ResolutionAndRefreshRateTable
    {
        get
        {
            // Creates an array of arrays for resolution structs
            Resolution[][] rough = new Resolution[Screen.resolutions.Length][];
            // Specifies the number of necessary columns
            int numberOfColumns = 0;
            // Specifies the length for each column. numberOfColumns is used to match the column length to the column in the array.
            int[] columnLengths = new int[Screen.resolutions.Length];
            // The extra large arrays and separate length variables are because array lengths can't be changed and we don't know the lengths yet.
            // We make the lengths large enough to account for any 'dimension'
            // Then, once we find the real sizes, transfer the values to an appropriately sized array.

            /*
            string listMessage = "All values:\n";
            for (int r = 0; r < Screen.resolutions.Length; r++)
            {
                listMessage += Screen.resolutions[r] + "\n";
            }
            listMessage += "End of table.";
            Debug.Log(listMessage);
            */

            // Goes through every struct in the normal Screen.resolutions array, and sorts it into an appropriate place
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                Resolution checking = Screen.resolutions[i];

                #region Add resolution if new. If not, add the refresh rate to its column if it is new for that resolution.

                bool resolutionIsNew = true;
                // Checks the first struct in each column with a resolution already recorded
                for (int x = 0; x < numberOfColumns; x++)
                {
                    Resolution assignedToSlot = rough[x][0];
                    // If an identical resolution appears in a column
                    if (checking.width == assignedToSlot.width && checking.height == assignedToSlot.height)
                    {
                        //Debug.Log("Resolution already exists in column " + x + " out of " + numberOfColumns);
                        // Mark it so it isn't added to a new one
                        resolutionIsNew = false;

                        #region Check refresh rates in current column in case that's new

                        bool refreshRateIsNew = true;
                        //Debug.Log("Checking refresh rate");
                        // Checks each struct in the current column that has a refresh rate assigned
                        for (int y = 0; y < columnLengths[x]; y++)
                        {
                            //Debug.Log("Checking assigned refresh rate " + (y + 1) + " out of " + columnLengths[x] + " in column " + (x + 1) + " out of " + numberOfColumns);
                            assignedToSlot = rough[x][y];
                            // If an identical refresh rate appears
                            if (checking.refreshRate == assignedToSlot.refreshRate)
                            {
                                refreshRateIsNew = false; // Mark it so it isn't added again
                                y = columnLengths[x]; // End loop prematurely because we already know the refresh rate is a duplicate
                            }
                        }

                        // If check was not tripped to false, refresh rate is new.
                        if (refreshRateIsNew == true)
                        {
                            columnLengths[x] += 1; // 'Increase' column length by one
                            //Debug.Log("Column " + x + " is " + columnLengths);


                            rough[x][columnLengths[x] - 1] = checking; // Add struct to latest value
                        }

                        #endregion

                        x = numberOfColumns; // End loop prematurely because we already know the resolution is a duplicate
                    }
                }

                // If check was not tripped to false, resolution is new.
                if (resolutionIsNew == true)
                {
                    // 'Increase' the array by one
                    numberOfColumns += 1;
                    // Make the newest column in the array not null, so values can be assigned
                    rough[numberOfColumns - 1] = new Resolution[Screen.resolutions.Length];
                    // Populate the first entry of the latest column with the new resolution
                    rough[numberOfColumns - 1][0] = checking;
                    // Manually set the length for the current column to 1, since the first option was manually added as well.
                    columnLengths[numberOfColumns - 1] = 1;
                }

                #endregion

                //Debug.Log("Finishing check " + (i + 1) + " out of " + Screen.resolutions.Length);
            }

            // Make a final array with an appropriate number of columns
            Resolution[][] final = new Resolution[numberOfColumns][];

            // For each column
            for (int x = 0; x < final.Length; x++)
            {
                // Use index x to match the length variable in columnLengths to the appropriate column
                final[x] = new Resolution[columnLengths[x]];
                // For each value in the current column
                for (int y = 0; y < final[x].Length; y++)
                {
                    // Use integer x to get the appropriate column from rough, and y to get the appropriate value from said column.
                    // Assign it to the new table.
                    final[x][y] = rough[x][y];
                }
            }

            /*
            string message = "Assembled resolutions and framerates:\n";
            Debug.Log(final.Length);
            for (int r = 0; r < final.Length; r++)
            {
                Resolution[] current = final[r];
                //Debug.Log("column Length = " + (current.Length));
                
                Resolution text = current[0];
                message += text.width + " X " + text.height + ": ";
                for (int c = 0; c < current.Length - 1; c++)
                {
                    message += current[c].refreshRate + "Hz, ";
                }
                message += current[current.Length - 1].refreshRate + "Hz\n";
            }
            message += "End of table.";
            Debug.Log(message);
            */
            return final;
        }
    }
    public int GetResolutionAsTableIndex(Resolution resolution, Resolution[][] table)
    {
        for (int i = 0; i < table.Length; i++)
        {
            Resolution checking = table[i][0];
            if (resolution.width == checking.width && resolution.height == checking.height)
            {
                return i;
            }
        }

        return -1;
    }
    public int GetResolutionAsTableIndex(Resolution resolution)
    {
        return GetResolutionAsTableIndex(resolution, ResolutionAndRefreshRateTable);
    }
    public int GetRefreshRateAsTableIndex(Resolution resolution, Resolution[][] table)
    {
        int correctArrayToSearchIn = GetResolutionAsTableIndex(resolution, table);
        if (correctArrayToSearchIn < 0)
        {
            return -1;
        }

        Resolution[] refreshRates = table[correctArrayToSearchIn];
        for (int i = 0; i < refreshRates.Length; i++)
        {
            Resolution checking = refreshRates[i];
            if (resolution.refreshRate == checking.refreshRate)
            {
                return i;
            }
        }

        return -1;
    }
    public int GetRefreshRateAsTableIndex(Resolution resolution)
    {
        return GetRefreshRateAsTableIndex(resolution, ResolutionAndRefreshRateTable);
    }


    public override void MenuSpecificSetup()
    {
        base.MenuSpecificSetup();
        fullscreen.onValueChanged.AddListener(_ => OnOptionAlter());
        graphicalQualityPreset.onValueChanged.AddListener(_ => OnOptionAlter());
        resolution.onValueChanged.AddListener(_ => OnOptionAlter());
        refreshRate.onValueChanged.AddListener(_ => OnOptionAlter());
    }
    public override void RefreshSettings()
    {
        base.RefreshSettings();
        Debug.Log("Refreshing options in video settings menu");
        playerToUpdate = GetComponentInParent<PlayerHandler>();
        // Updates interactable options to represent the current settings

        #region Refresh fullscreen option
        // Refreshes fullscreen options dropdown to reflect available graphical options
        string[] fullScreenOptions = System.Enum.GetNames(typeof(FullScreenMode));
        fullscreen.ClearOptions();
        fullscreen.AddOptions(new List<string>(fullScreenOptions));
        fullscreen.value = (int)Screen.fullScreenMode;
        fullscreen.RefreshShownValue();
        #endregion

        #region Refresh graphical quality options
        graphicalQualityPreset.ClearOptions();
        graphicalQualityPreset.AddOptions(new List<string>(QualitySettings.names));
        graphicalQualityPreset.value = QualitySettings.GetQualityLevel();
        graphicalQualityPreset.RefreshShownValue();
        #endregion

        /*
        #region Refresh monitor options
        string[] monitorListings = new string[Display.displays.Length];
        for (int i = 0; i < Display.displays.Length; i++)
        {
            monitorListings[i] = "Display " + (i + 1) + " (" + Display.displays[i].systemWidth + " X " + Display.displays[i].systemHeight + "): ";
            float f = Display.displays[i].

            if (Display.displays[i].active == true)
            {
                monitorListings[i] += "Active";
            }
            else
            {
                monitorListings[i] += "Inactive";
            }
        }
        monitor.ClearOptions();
        monitor.AddOptions(new List<string>(monitorListings));
        Debug.Log(Display.main + ", " + playerToUpdate.movement.playerCamera.targetDisplay + ", " + playerToUpdate.hud.hudCamera.targetDisplay);
        #endregion
        */

        #region Refresh resolution and framerate options
        table = ResolutionAndRefreshRateTable;
        Resolution current = Screen.currentResolution;

        #region Resolution
        // Creates an array of strings equal to the max number of supported resolutions
        string[] resolutionOptions = new string[table.Length];
        for (int i = 0; i < resolutionOptions.Length; i++)
        {
            // Creates a neatly formatted string of just the dimensions
            resolutionOptions[i] = table[i][0].width + " X " + table[i][0].height;
        }
        // Refreshes options to show available resolutions, then sets the dropdown's value to the actual current resolution
        resolution.ClearOptions();
        resolution.AddOptions(new List<string>(resolutionOptions));
        resolution.value = GetResolutionAsTableIndex(current, table);
        resolution.RefreshShownValue();
        #endregion

        #region Refresh rate
        // We need to show the refresh rates for the currently selected resolution
        // Get the refresh rate array for the index specified by the resolution dropdown, since that's the visible one
        string[] refreshRateOptions = new string[table[resolution.value].Length];
        for (int i = 0; i < refreshRateOptions.Length; i++)
        {
            // Turns each value in the array into a neatly formatted string of the refresh rate, with "Hz" on the end for readability
            refreshRateOptions[i] = table[resolution.value][i].refreshRate + "Hz";
            Debug.Log(refreshRateOptions[i]);
        }
        // Refreshes options to show available refresh rate, then sets the dropdown's value to the actual current refresh rate
        refreshRate.ClearOptions();
        refreshRate.AddOptions(new List<string>(refreshRateOptions));
        refreshRate.value = GetRefreshRateAsTableIndex(current, table);
        refreshRate.RefreshShownValue();
        
        #endregion
        #endregion
    }
    public override void SaveSettings()
    {
        // Applies settings here. This has two benefits:
        // One, having a saving function means the player is not locked into any experimental decisions they make.
        // Two, I don't have to make a new function and add it as a listener for each option.

        QualitySettings.SetQualityLevel(graphicalQualityPreset.value, applyExpensiveQualityPresetChanges);
        Resolution newResolution = table[resolution.value][refreshRate.value];
        Screen.SetResolution(newResolution.width, newResolution.height, (FullScreenMode)fullscreen.value, newResolution.refreshRate);
        
        

        base.SaveSettings();
    }
    public override void RevertToDefaultSettings()
    {
        Debug.Log("This presently does nothing, as I don't really know how to get 'default' options for various graphical settings.");
        base.RevertToDefaultSettings();
    }

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


}
