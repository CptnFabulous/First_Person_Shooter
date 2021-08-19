using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Rendering;

public class VideoSettingsMenu : SettingsMenu
{
    [Header("Interactables for settings")]
    public Dropdown fullscreen;
    public Dropdown graphicalQualityPreset;
    public bool applyExpensiveQualityPresetChanges = true;
    public Dropdown resolution;
    public Dropdown refreshRate;
    


    /// <summary>
    /// Gets all the Resolution structs from Screen.resolutions, and organises them into a handy array of arrays, compartmentalising based on both dimensions and refresh rate.
    /// Each resolution has its own sub-array, with each value in it being a different refresh rate.
    /// </summary>
    public static Resolution[][] ResolutionAndRefreshRateTable
    {
        get
        {
            // Create an array of arrays, with a length equal to the number of resolution structs.
            // Stores the maximum necessary length for the main array (for each distinct resolution
            // Creates a list of maximum required lengths for each sub array
            Resolution[][] rough = new Resolution[Screen.resolutions.Length][];
            int rowLength = 1;
            int[] columnLengths = new int[Screen.resolutions.Length];
            // Making the array lengths equal to the total number of entries ensures that even if every entry has a different resolution or framerate, there will be space to hold them all.
            // This is necessary because array lengths cannot be adjusted, so after getting the required lengths I'll put the values into a shorter array.
            // For some reason when declaring the length of the array of arrays, you put the length of the main array in the first set of braces when you'd expect it to be the second.
            // But when referencing a variable in the main array, you put the index for the main array in the first, then the sub array in the second

            // Goes through every struct in the normal Screen.resolutions array, and sorts it into an appropriate place
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                // Get currently checked resolution struct
                Resolution currentChecking = Screen.resolutions[i];

                #region Check resolution against others recorded in the array. If already present, see if its refresh rate is new for that resolution.
                // xLength equals the number of separate required arrays for resolutions, even if the main array is larger.
                // All arrays beyond the length of xLength are unassigned
                bool resolutionIsNew = true;
                // Check the current struct against all distinct resolutions currently recorded in the table
                for (int resolutionIndex = 0; resolutionIndex < rowLength; resolutionIndex++)
                {
                    Resolution fromRow = rough[resolutionIndex][0];
                    // If a struct being currently checked has the same dimensions as the one in the table
                    if (currentChecking.width == fromRow.width && currentChecking.height == fromRow.height)
                    {
                        // Resolution is already recorded, mark as false so it doesn't get added in a separate array.
                        resolutionIsNew = false;
                        // Proceed to checking if struct has a different refresh rate

                        #region Since resolution is not new, check refresh rates in current column
                        // yLengthForCurrentColumn equals the number of recorded refresh rates for the currently checked resolution, even if the array is larger.
                        // All refresh rates in a particular resolution's array beyond the length of yLengthForCurrentColumn are unassigned for this resolution
                        bool refreshRateIsNew = true;
                        // Check the current struct against all distinct refresh rates currently recorded in the column
                        for (int refreshRateIndex = 0; refreshRateIndex < columnLengths[resolutionIndex]; refreshRateIndex++)
                        {
                            Resolution fromColumn = rough[resolutionIndex][refreshRateIndex];
                            // If the refresh rate of the struct currently being checked is the same as one in the current column
                            if (currentChecking.refreshRate == fromColumn.refreshRate)
                            {
                                // Trip refreshRateIsNew to false and end the loop prematurely
                                refreshRateIsNew = false;
                                refreshRateIndex = columnLengths[resolutionIndex];
                            }
                        }
                        #endregion

                        #region Add refresh rate to top of column if new
                        // If the refresh rate bool was not tripped false, the refresh rate is new even if the resolution is not
                        if (refreshRateIsNew)
                        {
                            // Expand the current column by one, and add the new struct since it has a different refresh rate.
                            columnLengths[resolutionIndex] += 1;
                            rough[resolutionIndex][columnLengths[resolutionIndex] - 1] = currentChecking;
                        }
                        #endregion

                        // End resolution checking loop prematurely, no more checking needed for resolution dimensions
                        resolutionIndex = rowLength;
                    }
                }
                #endregion

                #region Add resolution to table if new
                // If the bool was not tripped false earlier, this means the current resolution is different from all the previously documented ones.
                if (resolutionIsNew)
                {
                    // If the loop went through enough times to equal xLength without tripping the bool to false,
                    // that means the table needs to be expanded by 1 to accomodate the new, different resolution.
                    rowLength += 1;

                    // Creates a new array of more than adequate length to store every framerate for a particular resolution
                    rough[rowLength - 1] = new Resolution[Screen.resolutions.Length];
                    // Assigns the first new resolution as the first value in the newest array
                    rough[rowLength - 1][0] = currentChecking;
                    // Since xLength is the amount of entries and the index always starts at 0, the maximum entry is xLength - 1.
                    // I could have just set the array value before increasing xLength to save the - 1 calculation, but this is less confusing.

                    // Set the max length variable for the current column to one 
                    columnLengths[rowLength - 1] = 1;
                }
                #endregion
            }

            // Make a final array, with an appropriate length
            Resolution[][] final = new Resolution[rowLength][];
            // For each sub array within the main array
            for (int r = 0; r < final.Length; r++)
            {
                // Set the length of the sub array to equal its recorded length, matched by the correct index
                final[r] = new Resolution[columnLengths[r]];
                // For every value in the sub array
                for (int c = 0; c < final[r].Length; r++)
                {
                    // Set the resolution struct in this value in the sub array to the appropriate one in the old overly large array
                    final[r][c] = rough[r][c];
                }
            }

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
        // Updates interactable options to represent the current settings

        #region Refresh fullscreen option
        // Refreshes fullscreen options dropdown to reflect available graphical options
        string[] fullScreenOptions = System.Enum.GetNames(typeof(FullScreenMode));
        fullscreen.AddOptions(new List<string>(fullScreenOptions));
        fullscreen.value = (int)Screen.fullScreenMode;
        #endregion

        #region Refresh graphical quality options
        graphicalQualityPreset.AddOptions(new List<string>(QualitySettings.names));
        graphicalQualityPreset.value = QualitySettings.GetQualityLevel();
        #endregion

        #region Refresh resolution and framerate options
        Resolution[][] table = ResolutionAndRefreshRateTable;
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
        #endregion

        #region Refresh rate
        // We need to show the refresh rates for the currently selected resolution
        // Get the refresh rate array for the index specified by the resolution dropdown, since that's the visible one
        string[] refreshRateOptions = new string[table[resolution.value].Length];
        for (int i = 0; i < refreshRateOptions.Length; i++)
        {
            // Turns each value in the array into a neatly formatted string of the refresh rate, with "Hz" on the end for readability
            refreshRateOptions[i] = table[resolution.value][i].refreshRate + "Hz";
        }
        // Refreshes options to show available refresh rate, then sets the dropdown's value to the actual current refresh rate
        refreshRate.ClearOptions();
        refreshRate.AddOptions(new List<string>(refreshRateOptions));
        resolution.value = GetRefreshRateAsTableIndex(current, table);
        #endregion
        #endregion
    }
    public override void SaveSettings()
    {
        // Applies settings here. This has two benefits:
        // One, having a saving function means the player is not locked into any experimental decisions they make.
        // Two, I don't have to make a new function and add it as a listener for each option.

        //Screen.fullScreenMode = (FullScreenMode)fullscreen.value;
        QualitySettings.SetQualityLevel(graphicalQualityPreset.value, applyExpensiveQualityPresetChanges);
        Resolution newResolution = ResolutionAndRefreshRateTable[resolution.value][refreshRate.value];
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
