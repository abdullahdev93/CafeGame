using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CalendarMenu : MenuPage
{
    public GameObject[] seasonBase; // Array of season GameObjects
    public Image[] calendarDays; // Array of images representing the days in the calendar
    public TextMeshProUGUI seasonTitleText; // Text component for displaying the season 
    public TextMeshProUGUI seasonNameText; // Text component for displaying the season name only
    public Image[] springIcons; // Array of icons for Spring
    public Image[] summerIcons; // Array of icons for Summer
    public Image[] fallIcons; // Array of icons for Fall
    public Image[] winterIcons; // Array of icons for Winter
    public Image[] springAIcons; // Array of icons for SpringA

    [Header("Weather Icons for Calendar Days")]
    public Image[] weatherIcons; // Should match calendarDays.Length

    [Header("Temperature Texts for Calendar Days")]
    public TextMeshProUGUI[] temperatureTexts; // Must match calendarDays.Length 

    private int selectedObjectIndex = -1;
    public int displayedSeasonIndex;

    public Button nextButton;
    public Button previousButton;

    public static CalendarMenu instance { get; private set; }

    private VN_Configuration config => VN_Configuration.activeConfig;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        LoadConfig();
        CalendarSystem.Instance.LoadSeasonAndDay();
        // Load the stored displayedSeasonIndex from PlayerPrefs, defaulting to the current season if not found
        //displayedSeasonIndex = PlayerPrefs.GetInt("DisplayedSeasonIndex", (int)CalendarSystem.Instance.GetCurrentSeason());
        displayedSeasonIndex = (int)CalendarSystem.Instance.GetCurrentSeason();
        UpdateCalendar();
    }

    /*void OnDisable()
    {
        // Save the currently displayed season when the calendar menu is closed
        PlayerPrefs.SetInt("DisplayedSeasonIndex", displayedSeasonIndex);
        PlayerPrefs.Save();
    }*/ 

    private void LoadConfig()
    {
        // Load your configuration here
    }

    public void OpenCalendarMenu()
    {
        // Reset displayedSeasonIndex to the current season when opening the menu
        displayedSeasonIndex = (int)CalendarSystem.Instance.GetCurrentSeason();
        UpdateCalendar();
    }

    public void UpdateCalendar()
    {
        // Get the displayed season and the actual current season/day
        int currentSeason = displayedSeasonIndex;
        int currentDay = CalendarSystem.Instance.GetCurrentDay();
        int actualCurrentSeason = (int)CalendarSystem.Instance.GetCurrentSeason();

        // Update the title text for the actual current season
        string seasonName = GetSeasonName(actualCurrentSeason);
        string year = actualCurrentSeason == (int)CalendarSystem.Season.SpringA ? "2020" : "2019";
        seasonTitleText.text = $"{seasonName} {currentDay:D2}, {year}"; 

        // Update the seasonNameText with the name of the displayed season
        string displayedSeasonName = GetSeasonName(displayedSeasonIndex);
        string displayedYear = displayedSeasonIndex == (int)CalendarSystem.Season.SpringA ? "2020" : "2019"; 
        seasonNameText.text = $"{displayedSeasonName} {displayedYear}";

        SetIconsVisibility(currentSeason); 

        // Set the season name text with only the season name
        //seasonNameText.text = seasonName; 

        // Determine the number of days and starting offset for the displayed season
        int daysInSeason = GetDaysInSeason(currentSeason);
        int startingDayOffset = GetStartingDayOffset(currentSeason);

        // Reset all calendar days to default state
        for (int i = 0; i < calendarDays.Length; i++)
        {
            calendarDays[i].color = new Color32(0, 185, 255, 255); // Default blue color
            SetDayTextVisible(calendarDays[i], false); // Hide text by default
            calendarDays[i].gameObject.SetActive(false); // Disable all by default 
        }

        // Update active days for the displayed season
        for (int i = 0; i < calendarDays.Length; i++)
        {
            if (i < startingDayOffset)
            {
                // Blank spaces before the start of the season
                calendarDays[i].color = new Color32(0, 0, 0, 0); // Transparent
                calendarDays[i].gameObject.SetActive(true); 
            }
            else if (i < daysInSeason + startingDayOffset)
            {
                // Active days within the season
                int dayNumber = i - startingDayOffset + 1;
                calendarDays[i].color = new Color32(0, 185, 255, 255); // Blue for active days
                SetDayTextVisible(calendarDays[i], true);
                SetDayTextLabel(calendarDays[i], dayNumber);
                calendarDays[i].gameObject.SetActive(true);

                // Highlight the current day if this is the actual current season
                if (currentSeason == actualCurrentSeason && dayNumber == currentDay)
                {
                    //calendarDays[i].color = Color.yellow; // Highlight the current day 
                    calendarDays[i].color = Color.green; // Now highlights the current day in green 
                }
            }
            else
            {
                // Blank spaces after the season
                calendarDays[i].color = new Color32(0, 0, 0, 0); // Transparent
                calendarDays[i].gameObject.SetActive(true);
            }

            if (i >= startingDayOffset && i < daysInSeason + startingDayOffset)
            {
                int dayNumber = i - startingDayOffset + 1;
                CalendarSystem.Season season = (CalendarSystem.Season)currentSeason;
                int weatherYear = season == CalendarSystem.Season.SpringA ? 2020 : 2019;

                var weatherData = WeatherSystem.Instance.GetWeatherForSeasonDay(season, dayNumber, weatherYear); 
                Sprite weatherSprite = weatherData.sprite;

                int temperature = 0;

                switch (CalendarSystem.Instance.activityCounter)
                {
                    case 0:
                        temperature = weatherData.morningTemp;
                        break;
                    case 1:
                        temperature = weatherData.afternoonTemp;
                        break;
                    case 2:
                        temperature = weatherData.eveningTemp;
                        break;
                } 

                if (temperatureTexts != null && i < temperatureTexts.Length && temperatureTexts[i] != null)
                {
                    if (VN_Configuration.activeConfig.useCelsius)
                    {
                        int celsius = Mathf.RoundToInt((temperature - 32) * 5f / 9f);
                        temperatureTexts[i].text = $"{celsius}°C";
                    }
                    else
                    {
                        temperatureTexts[i].text = $"{temperature}°F";
                    }

                    temperatureTexts[i].enabled = true;
                } 

                if (weatherIcons != null && i < weatherIcons.Length && weatherIcons[i] != null)
                {
                    weatherIcons[i].sprite = weatherSprite;
                    weatherIcons[i].enabled = true;

                    //Add this block to enlarge "SunnyAndWindy" icons 
                    if (weatherData.weatherType == CalendarSystem.Weather.SunnyAndWindy)
                    {
                        weatherIcons[i].rectTransform.localScale = new Vector3(1.25f, 1.25f, 1f); // 25% larger
                    }
                    else
                    {
                        weatherIcons[i].rectTransform.localScale = Vector3.one; // Normal size
                    }
                }
            }
            else
            {
                if (weatherIcons != null && i < weatherIcons.Length && weatherIcons[i] != null)
                {
                    weatherIcons[i].enabled = false;
                }

                if (temperatureTexts != null && i < temperatureTexts.Length && temperatureTexts[i] != null)
                {
                    temperatureTexts[i].enabled = false;
                } 
            }
        }
    }

    private void SetIconsVisibility(int seasonIndex)
    {
        HideAllIcons();

        switch ((CalendarSystem.Season)seasonIndex)
        {
            case CalendarSystem.Season.Spring:
                SetIconsActive(springIcons, true);
                break;
            case CalendarSystem.Season.Summer:
                SetIconsActive(summerIcons, true);
                break;
            case CalendarSystem.Season.Fall:
                SetIconsActive(fallIcons, true);
                break;
            case CalendarSystem.Season.Winter:
                SetIconsActive(winterIcons, true);
                break;
            case CalendarSystem.Season.SpringA:
                SetIconsActive(springAIcons, true);
                break;
        }
    }

    private void HideAllIcons()
    {
        SetIconsActive(springIcons, false);
        SetIconsActive(summerIcons, false);
        SetIconsActive(fallIcons, false);
        SetIconsActive(winterIcons, false);
        SetIconsActive(springAIcons, false);
    }

    private void SetIconsActive(Image[] icons, bool isActive)
    {
        foreach (var icon in icons)
        {
            icon.gameObject.SetActive(isActive);
        }
    } 

    /*public void UpdateCalendar()
    {
        // Keep the calendar display logic based on the displayedSeasonIndex (Next/Previous)
        int currentSeason = displayedSeasonIndex;
        int currentDay = CalendarSystem.Instance.GetCurrentDay();

        // Update the visibility of seasons
        for (int i = 0; i < seasonBase.Length; i++)
        {
            seasonBase[i].SetActive(i == displayedSeasonIndex);
        }

        // --- ALWAYS Use Current Season and Day for Title Text ---
        int actualCurrentSeason = (int)CalendarSystem.Instance.GetCurrentSeason();
        int actualCurrentDay = CalendarSystem.Instance.GetCurrentDay();
        string seasonName = GetSeasonName(actualCurrentSeason);
        string year = actualCurrentSeason == (int)CalendarSystem.Season.SpringA ? "2020" : "2019";

        // Update the season title text with the actual current season and day
        seasonTitleText.text = $"{seasonName} {actualCurrentDay:D2}, {year}";

        // Set the correct number of active days and blank spaces for the displayed season
        int daysInSeason = GetDaysInSeason(currentSeason);
        int startingDayOffset = GetStartingDayOffset(currentSeason);

        for (int i = 0; i < calendarDays.Length; i++)
        {
            if (i < startingDayOffset)
            {
                // Blank spaces before the start of the season
                calendarDays[i].color = new Color32(0, 0, 0, 0); // Transparent color
                SetDayTextVisible(calendarDays[i], false); // Make the text invisible
                calendarDays[i].gameObject.SetActive(true);
            }
            else if (i < daysInSeason + startingDayOffset)
            {
                // Active days of the season, labeled starting from 1
                calendarDays[i].color = new Color32(0, 185, 255, 255); // Blue color
                SetDayTextVisible(calendarDays[i], true); // Make the text visible
                SetDayTextLabel(calendarDays[i], i - startingDayOffset + 1); // Set the day number starting from 1
                calendarDays[i].gameObject.SetActive(true);
            }
            else
            {
                // Blank spaces after the end of the season
                calendarDays[i].color = new Color32(0, 0, 0, 0); // Transparent color
                SetDayTextVisible(calendarDays[i], false); // Make the text invisible
                calendarDays[i].gameObject.SetActive(true);
            }
        }

        // Highlight the current day only if we're viewing the current season
        if (currentSeason == (int)CalendarSystem.Instance.GetCurrentSeason() && currentDay > 0 && currentDay <= daysInSeason)
        {
            calendarDays[currentDay - 1 + startingDayOffset].color = Color.yellow;
        }
    }*/

    private int GetDaysInSeason(int seasonIndex)
    {
        switch ((CalendarSystem.Season)seasonIndex)
        {
            case CalendarSystem.Season.Spring:
            case CalendarSystem.Season.Summer: return 30;
            case CalendarSystem.Season.Fall:
            case CalendarSystem.Season.Winter: return 31; 
            case CalendarSystem.Season.SpringA: return 30; 
            default: return 30; // Default case
        }
    }

    private int GetStartingDayOffset(int seasonIndex)
    {
        switch ((CalendarSystem.Season)seasonIndex)
        {
            case CalendarSystem.Season.Spring: return 0; // Starts on Sunday 
            case CalendarSystem.Season.Summer: return 2; // Starts on Tuesday 
            case CalendarSystem.Season.Fall: return 4; // Starts on Thursday 
            case CalendarSystem.Season.Winter: return 0; // Starts on Friday 
            case CalendarSystem.Season.SpringA: return 3; // Starts on Monday 
            default: return 0; // Default case
        }
    }

    private void SetDayTextVisible(Image dayImage, bool isVisible)
    {
        TextMeshProUGUI dayText = dayImage.GetComponentInChildren<TextMeshProUGUI>();
        if (dayText != null)
        {
            dayText.color = isVisible ? new Color32(0, 0, 0, 255) : new Color32(0, 0, 0, 0); // Visible or invisible text
        }
    }

    private void SetDayTextLabel(Image dayImage, int dayNumber)
    {
        TextMeshProUGUI dayText = dayImage.GetComponentInChildren<TextMeshProUGUI>();
        if (dayText != null)
        {
            dayText.text = dayNumber < 10 ? "0" + dayNumber.ToString() : dayNumber.ToString();
        }
    }

    private string GetSeasonName(int seasonIndex)
    {
        switch ((CalendarSystem.Season)seasonIndex)
        {
            case CalendarSystem.Season.SpringA: return "Spring";
            default: return ((CalendarSystem.Season)seasonIndex).ToString();
        }
    }

    public void NextPrefabButtonClick()
    {
        // Make the current season invisible
        //seasonBase[displayedSeasonIndex].SetActive(false);

        // Update the displayedSeasonIndex to the next season
        displayedSeasonIndex = (displayedSeasonIndex + 1) % seasonBase.Length;

        // Make the new current season visible
        //seasonBase[displayedSeasonIndex].SetActive(true);

        // Update the calendar UI to reflect the change, while keeping season title text as the actual current season
        UpdateCalendar();
    }

    public void PreviousPrefabButtonClick()
    {
        // Make the current season invisible
        //seasonBase[displayedSeasonIndex].SetActive(false);

        // Update the displayedSeasonIndex to the previous season
        displayedSeasonIndex = (displayedSeasonIndex - 1 + seasonBase.Length) % seasonBase.Length;

        // Make the new current season visible
        //seasonBase[displayedSeasonIndex].SetActive(true);

        // Update the calendar UI to reflect the change, while keeping season title text as the actual current season
        UpdateCalendar();
    } 
}


/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CalendarMenu : MenuPage
{
    public GameObject[] seasonBase;
    public Image[] calendarDays; // Array of images representing the days in the calendar
    public TextMeshProUGUI seasonTitleText; // Text component for displaying the season

    private int selectedObjectIndex = -1;
    public int displayedSeasonIndex;

    public Button nextButton;
    public Button previousButton;

    public static CalendarMenu instance { get; private set; }

    private VN_Configuration config => VN_Configuration.activeConfig;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        LoadConfig();
        CalendarSystem.Instance.LoadSeasonAndDay();
        displayedSeasonIndex = (int)CalendarSystem.Instance.GetCurrentSeason();
        UpdateCalendar();
    }

    private void LoadConfig()
    {
        // Load your configuration here
    }

    public void UpdateCalendar()
    {
        int currentSeason = displayedSeasonIndex;
        int currentDay = CalendarSystem.Instance.GetCurrentDay();

        string seasonName = ((CalendarSystem.Season)currentSeason).ToString();

        if (currentSeason == (int)CalendarSystem.Season.SpringA)
            seasonTitleText.text = $"Spring 2020";
        else
            seasonTitleText.text = $"{seasonName} 2019"; // Update to the correct year as needed

        // Set the correct number of active days for the current season
        int daysInSeason = GetDaysInSeason(currentSeason);

        for (int i = 0; i < calendarDays.Length; i++)
        {
            if (i < daysInSeason)
            {
                // Reset active days to the default color (blue in this case)
                calendarDays[i].color = new Color32(0, 185, 255, 255); // Blue color in hexadecimal 
                calendarDays[i].gameObject.SetActive(true); // Activate the days that should be visible

                // Ensure the text is visible
                SetDayTextVisible(calendarDays[i], true);
            }
            else if (i == 12 || i == 13 && currentSeason == (int)CalendarSystem.Season.SpringA)
            {
                // Show blank spaces for Days 13 and 14
                calendarDays[i].color = new Color32(0, 0, 0, 0); // Transparent color to make it invisible
                calendarDays[i].gameObject.SetActive(true); // Keep the blank spaces visible but transparent

                // Make the text invisible
                SetDayTextVisible(calendarDays[i], false);
            }
            else
            {
                calendarDays[i].gameObject.SetActive(false); // Hide the rest of the days
            }
        }

        // Highlight the current day only if we're viewing the current season
        if (currentSeason == (int)CalendarSystem.Instance.GetCurrentSeason() && currentDay > 0 && currentDay <= daysInSeason)
        {
            calendarDays[currentDay - 1].color = Color.yellow;
        }
    }

    private void SetDayTextVisible(Image dayImage, bool isVisible)
    {
        TextMeshProUGUI dayText = dayImage.GetComponentInChildren<TextMeshProUGUI>();
        if (dayText != null)
        {
            dayText.color = isVisible ? new Color32(0, 0, 0, 255) : new Color32(0, 0, 0, 0); // Visible or invisible text
        }
    }

    private int GetDaysInSeason(int seasonIndex)
    {
        switch ((CalendarSystem.Season)seasonIndex)
        {
            case CalendarSystem.Season.SpringA: return 12; // Spring 2020 has 12 active days + 2 blank spaces
            case CalendarSystem.Season.Spring:
            case CalendarSystem.Season.Summer:
            case CalendarSystem.Season.Fall:
            case CalendarSystem.Season.Winter: return 35; // Other seasons have 35 days   
            default: return 35;
        }
    }

    public void NextPrefabButtonClick()
    {
        displayedSeasonIndex = (displayedSeasonIndex + 1) % seasonBase.Length;
        UpdateCalendar();
    }

    public void PreviousPrefabButtonClick()
    {
        displayedSeasonIndex = (displayedSeasonIndex - 1 + seasonBase.Length) % seasonBase.Length;
        UpdateCalendar();
    }
}
*/
