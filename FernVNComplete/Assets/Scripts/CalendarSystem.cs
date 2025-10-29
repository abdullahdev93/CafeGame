using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class CalendarSystem : MonoBehaviour
{
    public static CalendarSystem Instance;
    public TextMeshProUGUI seasonDayText;
    public TextMeshProUGUI timeOfDayText; // Text component for displaying the current activity time
    public TextMeshProUGUI dayOfWeekText;
    public TextMeshProUGUI temperatureText;
    public TextMeshProUGUI previousTemperatureText; 
    public GameObject calendarTemplate;
    public GameObject calendarMockTemplate; 

    // Declare an event for when the day advances
    public delegate void OnDayChanged();
    public event OnDayChanged onDayChanged; 

    public enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter,
        SpringA
    }

    public enum Weather
    {
        Sunny,
        Cloudy,
        SunnyAndCloudy,
        Rainy, 
        Windy, 
        Snowy, 
        Foggy, 
        Thunderstorm, 
        Hail, 
        SunnyAndWindy 
    }

    public Season currentSeason; 
    public Weather currentWeather; // Add a variable for the current weather 

    public int currentDay; 
    public int activityCounter; // Counter for Morning, Afternoon, Evening 

    private int[] daysInSeason = { 30, 30, 30, 30, 12 }; // Adjusted for each season    

    public readonly string[] activityPhases = { "Morning", "Afternoon", "Evening" };

    private readonly string[] dayOfWeekNames = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

    public int currentYear = 2019; 

    /*public Image weatherImage; // Image component for displaying the weather icon
    public Sprite sunnySprite;
    public Sprite cloudySprite;
    public Sprite sunnyCloudySprite;
    public Sprite rainySprite;*/

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start() 
    {

        // Initialize seasonal and calendar-related variables
        VariableStore.CreateVariable("Season", currentSeason.ToString());
        VariableStore.CreateVariable("Weather", currentWeather.ToString());
        VariableStore.CreateVariable("DayOfWeek", GetCurrentDayOfWeek());
        VariableStore.CreateVariable("Day", currentDay);
        VariableStore.CreateVariable("ActivityPhase", activityPhases[activityCounter]);

        SaveSeasonAndDay(); // Now safe to call  

        LoadSeasonAndDay();
        DetermineWeather(); // Determine the weather based on the season and day 
        UpdateSeasonDayText();
        UpdateActivityText(); // Initialize with the loaded activity time

        if (SlidingCalendarMenu.instance != null)
        {
            SlidingCalendarMenu.instance.UpdateCalendarUI(true); // Show the previous season/day initially
        }  
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "VisualNovel")
        {
            // Find and assign the TextMeshPro components again in case they were lost during the scene transition
            seasonDayText = GameObject.FindWithTag("SeasonDayText")?.GetComponent<TextMeshProUGUI>();
            timeOfDayText = GameObject.FindWithTag("ActivityText")?.GetComponent<TextMeshProUGUI>(); 
            dayOfWeekText = GameObject.FindWithTag("DayOfWeekText")?.GetComponent<TextMeshProUGUI>();
            temperatureText = GameObject.FindWithTag("TemperatureText")?.GetComponent<TextMeshProUGUI>();
            //WeatherSystem.Instance.DisplayPreviousTemperature(temperatureText); 
            calendarTemplate = GameObject.FindWithTag("CalendarTemplate"); 
            calendarMockTemplate = GameObject.FindWithTag("CalendarMockTemplate");

            if (currentSeason == Season.Spring && currentDay == 15)
                calendarMockTemplate.SetActive(true);
            else
            {
                //calendarMockTemplate = null;
                calendarMockTemplate.SetActive(false); 
            }
                

            //Narrator "Finally, it's time to open Bean Bliss Caf�. I unlock the doors, flip over the open/close sign, adjacent to the door, waiting for the patrons."

            Season previousSeason = (Season)PlayerPrefs.GetInt("PreviousSeason", (int)currentSeason);
            int previousDay = PlayerPrefs.GetInt("PreviousDay", currentDay);

            LoadSeasonAndDay();
            DetermineWeather(); // Determine the weather whenever the scene is loaded 
            UpdateSeasonDayText();
            UpdateActivityText();

            // Only slide if there's a difference between the saved previous state and the current state
            if (SlidingCalendarMenu.instance != null && (currentSeason != previousSeason || currentDay != previousDay))
            {
                SlidingCalendarMenu.instance.ShowCalendarMenu();
                StartCoroutine(SlidingCalendarMenu.instance.SlideAndHide());
            }

            // Save the current state as the previous state for future comparisons
            PlayerPrefs.SetInt("PreviousSeason", (int)currentSeason);
            PlayerPrefs.SetInt("PreviousDay", currentDay);
            PlayerPrefs.Save();
        }
        else if (scene.name == "TownMap")
        {
            TownMapManager townMapManager = FindObjectOfType<TownMapManager>();
            if (townMapManager != null)
            {
                townMapManager.RefreshSeasonDayText();
            }
        }
    }

    // New method to get the current day of the week
    public string GetCurrentDayOfWeek()
    {
        // Day 1 is assumed to be Monday; adjust if needed
        int dayOfWeekIndex = (currentDay - 1) % 7;
        return dayOfWeekNames[dayOfWeekIndex];
    } 

    public void AdvanceDay()
    {
        Season previousSeason = currentSeason;
        int previousDay = currentDay; 

        currentDay++;

        if (currentDay > daysInSeason[(int)currentSeason])
        {
            currentDay = 1;
            AdvanceSeason();
        }

        DetermineWeather(); // Determine the weather when the day advances 

        activityCounter = 0;

        //UpdateSeasonDayText();
        SaveSeasonAndDay();

        /*if (SceneManager.GetActiveScene().name == "VisualNovel")
        {
            VNMenuManager.instance.HandleDayAdvance(); // Notify VNMenuManager
        }*/

        // Trigger the day change event
        onDayChanged?.Invoke();  // Notify listeners (such as the DirectMessageManager) that the day has changed 

        // Check if the current day of the week is Sunday
        if (GetCurrentDayOfWeek() == "Sunday")
        {
            // Invoke a specific event or notify listeners that it's Sunday
            PlayerPrefs.SetInt("IsSunday", 1); // Set a flag for Sunday
        }
        else
        {
            PlayerPrefs.SetInt("IsSunday", 0); // Reset the flag on other days
        }

        PlayerPrefs.Save(); 

        if (SlidingCalendarMenu.instance != null && (currentSeason != previousSeason || currentDay != previousDay))
        {
            StartCoroutine(ShowSlidingCalendarMenuForDuration(previousSeason, previousDay, 5f)); 
        }

        if (CalendarMenu.instance != null)
        {
            CalendarMenu.instance.displayedSeasonIndex = (int)currentSeason;
            CalendarMenu.instance.UpdateCalendar();
        }

        // Reset fake calendar position if needed
        if (FakeCalendarSlide.Instance != null && (currentSeason == Season.Spring && currentDay < 16))
        {
            FakeCalendarSlide.Instance.ResetPositionIfNeeded(); 
        } 

        // Save the current state as the previous state for future comparisons
        PlayerPrefs.SetInt("PreviousSeason", (int)currentSeason);
        PlayerPrefs.SetInt("PreviousDay", currentDay);
        PlayerPrefs.Save();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (!VNMenuManager.instance.developerMode)
                return; 

            AdvanceActivityOrDay();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!VNMenuManager.instance.developerMode)
                return; 

            ResetCalendar();
        }
    }

    public void AdvanceActivityOrDay()
    {
        activityCounter++;

        // Update Night Sprite Visibility
        //if (activityCounter == 2) // Evening phase
        //{
            //VNMenuManager.instance.weatherImage.sprite = VNMenuManager.instance.nightSprite; 
        //} 

        if (activityCounter < activityPhases.Length) 
        { 
            UpdateActivityText(); // Update the displayed activity phase
            UpdateTemperatureText(); 

            if (activityCounter == 2)
                VNMenuManager.instance.weatherImage.sprite = VNMenuManager.instance.nightSprite; 
        }

        else
        {
            activityCounter = 0;
            AdvanceDay(); // Advance the day when transitioning from Evening to Morning 
            //UpdateActivityText(); // Update the displayed activity phase                      
            SceneManager.LoadScene("SlidingCalendar"); 

        }

        // Check if it's evening to transition to the SlidingCalendar scene
        //if (activityCounter == 2) // Evening phase
        //{
        //SceneManager.LoadScene("SlidingCalendar");
        //}

        // Save the current activity phase (Morning, Afternoon, Evening)
        PlayerPrefs.SetInt("CurrentActivity", activityCounter);
        PlayerPrefs.Save(); 
    }

    /*public void AdvanceDay()
    {
        Season previousSeason = currentSeason;
        int previousDay = currentDay;

        currentDay++;

        if (currentDay > daysInSeason[(int)currentSeason])
        {
            currentDay = 1;
            AdvanceSeason();
        }

        UpdateSeasonDayText();
        SaveSeasonAndDay();

        if (SlidingCalendarMenu.instance != null)
        {
            if (currentSeason != previousSeason || currentDay != previousDay)
            {
                StartCoroutine(ShowSlidingCalendarMenuForDuration(previousSeason, previousDay, 5f));
            }
        }

        if (CalendarMenu.instance != null)
        {
            CalendarMenu.instance.displayedSeasonIndex = (int)currentSeason;
            CalendarMenu.instance.UpdateCalendar();
        }
    }*/

    private IEnumerator ShowSlidingCalendarMenuForDuration(Season previousSeason, int previousDay, float duration)
    {
        SlidingCalendarMenu.instance.ShowCalendarMenu();
        SlidingCalendarMenu.instance.UpdateCalendarUI(true);
        SlidingCalendarMenu.instance.SlideToDay(previousDay);

        yield return new WaitForSeconds(SlidingCalendarMenu.instance.slideDuration);

        SlidingCalendarMenu.instance.UpdateCalendarUI(false);

        yield return new WaitForSeconds(duration - SlidingCalendarMenu.instance.slideDuration);

        SlidingCalendarMenu.instance.HideCalendarMenu();

        //UpdateSeasonDayText(); 
        UpdateActivityText(); // Update the displayed activity phase      
    }

    private void AdvanceSeason()
    {
        currentSeason++;

        if (currentSeason > Season.SpringA)
        {
            currentSeason = Season.Spring;
        }
    }

    public void UpdateSeasonDayText() 
    {
        if (seasonDayText != null)
        {
            string dayText = currentDay < 10 ? "0" + currentDay.ToString() : currentDay.ToString();
            string year = currentSeason == Season.SpringA ? "2020" : "2019";

            if (currentSeason == Season.SpringA)
            {
                seasonDayText.text = $"Spring / {dayText}"; 
                dayOfWeekText.text = $"{GetCurrentDayOfWeek()}"; 
            }
            else
            {
                seasonDayText.text = $"{currentSeason} / {dayText}";
                dayOfWeekText.text = $"{GetCurrentDayOfWeek()}"; 
            }

            // Update Day of the Week Text
            if (dayOfWeekText != null)
            {
                dayOfWeekText.text = $"{GetCurrentDayOfWeek()}";
            }

            if (temperatureText != null)
            {
                int temp = GetTemperatureForToday();
                //temperatureText.text = $"{temp}�F"; 

                if (VN_Configuration.activeConfig.useCelsius)
                {
                    int celsius = Mathf.RoundToInt((temp - 32) * 5f / 9f);
                    temperatureText.text = $"{celsius}�C";
                }
                else
                {
                    temperatureText.text = $"{temp}�F";
                } 
            }

            // Update the TownMapManager's seasonDayText if it exists
            TownMapManager townMapManager = FindObjectOfType<TownMapManager>();
            if (townMapManager != null)
            {
                townMapManager.UpdateSeasonDayText();
            }
        }
    }

    public void UpdateTemperatureText()
    {
        Season season = currentSeason;
        int day = currentDay;
        int year = (season == Season.SpringA) ? 2020 : 2019;

        WeatherSystem.WeatherData weatherData = WeatherSystem.Instance.GetWeatherForSeasonDay(season, day, year);
        int temp = WeatherSystem.GetTemperatureForPhase(weatherData); 

        if (VN_Configuration.activeConfig.useCelsius)
            temp = Mathf.RoundToInt((temp - 32) * 5f / 9f);

        temperatureText.text = $"{temp}�" + (VN_Configuration.activeConfig.useCelsius ? "C" : "F");
    } 

    /*public void UpdateWeatherIcon()
    {
        Weather currentWeather = GetCurrentWeather();

        switch (currentWeather)
        {
            case Weather.Sunny:
                weatherImage.sprite = sunnySprite;
                break;
            case Weather.Cloudy:
                weatherImage.sprite = cloudySprite;
                break;
            case Weather.SunnyAndCloudy:
                weatherImage.sprite = sunnyCloudySprite;
                break;
            case Weather.Rainy: 
                weatherImage.sprite = rainySprite;
                break;
        }
    }*/

    private void UpdateActivityText()
    {
        if (timeOfDayText != null)
        {
            timeOfDayText.text = activityPhases[activityCounter];
        }
    }

    public void SaveSeasonAndDay()
    {
        PlayerPrefs.SetInt("CurrentSeason", (int)currentSeason);
        PlayerPrefs.SetInt("CurrentDay", currentDay);
        PlayerPrefs.SetString("CurrentDayOfWeek", GetCurrentDayOfWeek()); 

        // Store into VariableStore for dialogue referencing
        VariableStore.TrySetValue("Season", currentSeason.ToString());       // "Spring", "Summer", etc.
        VariableStore.TrySetValue("Weather", currentWeather.ToString());     // "Sunny", "Cloudy", etc.
        VariableStore.TrySetValue("DayOfWeek", GetCurrentDayOfWeek());       // "Monday", "Tuesday", etc.
        VariableStore.TrySetValue("Day", currentDay);                        // 1, 2, 3... 
        VariableStore.TrySetValue("ActivityPhase", activityPhases[activityCounter]);
        VariableStore.TrySetValue("Temperature", GetTemperatureForToday()); 

        PlayerPrefs.Save();
    }

    public void LoadSeasonAndDay()
    {
        currentSeason = (Season)PlayerPrefs.GetInt("CurrentSeason", (int)Season.Spring);
        currentDay = PlayerPrefs.GetInt("CurrentDay", 15);

        string savedDayOfWeek = PlayerPrefs.GetString("CurrentDayOfWeek", "Monday");
        dayOfWeekText.text = savedDayOfWeek; 

        // Load the saved activity time (Morning, Afternoon, Evening)
        activityCounter = PlayerPrefs.GetInt("CurrentActivity", 0);

        // Determine the weather again (for currentSeason/currentDay)
        DetermineWeather();

        // NEW: Update temperature text right after determining weather
        if (temperatureText != null)
        {
            int temp = GetTemperatureForToday();
            temperatureText.text = $"{temp}�F";
        }
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }

    public string GetCurrentWeekDay()
    {
        return GetCurrentDayOfWeek(); 
    }

    /*public int GetTemperatureForToday()
    {
        //return WeatherSystem.Instance.GetTemperature(currentSeason, currentWeather); 
        var weatherData = WeatherSystem.Instance.GetWeatherForSeasonDay(currentSeason, currentDay, currentSeason == Season.SpringA ? 2020 : 2019);
        return weatherData.temperature; 
    }*/

    public int GetTemperatureForToday()
    {
        var weatherData = WeatherSystem.Instance.GetWeatherForSeasonDay(currentSeason, currentDay, currentSeason == Season.SpringA ? 2020 : 2019);
        return weatherData.GetTemperatureForPhase(activityCounter);
    } 

    //public string GetCurrentDayOfWeek()
    //{
    // Assume Day 1 is Sunday; adjust the index to match your game's logic
    //int dayOfWeekIndex = (currentDay - 1) % 7;
    //return dayOfWeekNames[dayOfWeekIndex];
    //}

    public Season GetCurrentSeason()
    {
        return currentSeason;
    }

    public Weather GetCurrentWeather()
    {
        return currentWeather;
    } 

    public int GetDaysInSeason(Season season)
    {
        return daysInSeason[(int)season];
    }

    private void DetermineWeather()
    {
        currentWeather = DetermineWeatherForDay(currentDay, currentSeason);

        // Retrieve and display temperature using WeatherSystem
        int year = currentSeason == Season.SpringA ? 2020 : 2019;
        WeatherSystem.WeatherData weatherData = WeatherSystem.Instance.GetWeatherForSeasonDay(currentSeason, currentDay, year);

        int temp = 0;

        switch (activityCounter)
        {
            case 0:
                temp = weatherData.morningTemp;
                break;
            case 1:
                temp = weatherData.afternoonTemp;
                break;
            case 2:
                temp = weatherData.eveningTemp;
                break;
        }

        VariableStore.TrySetValue("Temperature", temp);
    } 

    /*private void DetermineWeather()
    {
        switch (currentSeason)
        {
            case Season.Spring:
                switch (currentDay)
                {
                    case 1:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 2:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 3:
                        currentWeather = Weather.Rainy;
                        break;
                    case 4:
                        currentWeather = Weather.Sunny;
                        break;
                    case 5:
                        currentWeather = Weather.Rainy;
                        break;
                    case 6:
                        currentWeather = Weather.Foggy;
                        break;
                    case 7:
                        currentWeather = Weather.Sunny;
                        break;
                    case 8:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 9:
                        currentWeather = Weather.Rainy;
                        break;
                    case 10:
                        currentWeather = Weather.SunnyAndWindy;
                        break;
                    case 11:
                        currentWeather = Weather.Rainy;
                        break;
                    case 12:
                        currentWeather = Weather.Foggy;
                        break;
                    case 13:
                        currentWeather = Weather.Rainy;
                        break;
                    case 14:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 15:
                        currentWeather = Weather.Sunny;
                        break;
                    case 16:
                        currentWeather = Weather.Sunny;
                        break;
                    case 17:
                        currentWeather = Weather.Rainy;
                        break;
                    case 18:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 19:
                        currentWeather = Weather.SunnyAndWindy;
                        break;
                    case 20:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 21:
                        currentWeather = Weather.Rainy;
                        break;
                    case 22:
                        currentWeather = Weather.Thunderstorm;
                        break;
                    case 23:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 24:
                        currentWeather = Weather.Sunny;
                        break;
                    case 25:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 26:
                        currentWeather = Weather.Rainy;
                        break;
                    case 27:
                        currentWeather = Weather.Hail;
                        break;
                    case 28:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 29:
                        currentWeather = Weather.Foggy;
                        break;
                    case 30:
                        currentWeather = Weather.Sunny;
                        break;
                    default:
                        currentWeather = Weather.Sunny;
                        break;
                }
                break;

            case Season.Summer:
                switch (currentDay)
                {
                    case 1:
                        currentWeather = Weather.Sunny;
                        break;
                    case 2:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 3:
                        currentWeather = Weather.Sunny;
                        break;
                    case 4:
                        currentWeather = Weather.Sunny;
                        break;
                    case 5:
                        currentWeather = Weather.SunnyAndWindy;
                        break;
                    case 6:
                        currentWeather = Weather.Sunny;
                        break;
                    case 7:
                        currentWeather = Weather.Sunny;
                        break;
                    case 8:
                        currentWeather = Weather.Sunny;
                        break;
                    case 9:
                        currentWeather = Weather.Sunny;
                        break;
                    case 10:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 11:
                        currentWeather = Weather.Sunny;
                        break;
                    case 12:
                        currentWeather = Weather.Foggy;
                        break;
                    case 13:
                        currentWeather = Weather.SunnyAndWindy;
                        break;
                    case 14:
                        currentWeather = Weather.Sunny;
                        break;
                    case 15:
                        currentWeather = Weather.Sunny;
                        break;
                    case 16:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 17:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 18:
                        currentWeather = Weather.Sunny;
                        break;
                    case 19:
                        currentWeather = Weather.Sunny;
                        break;
                    case 20:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 21:
                        currentWeather = Weather.Windy;
                        break;
                    case 22:
                        currentWeather = Weather.Sunny;
                        break;
                    case 23:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 24:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 25:
                        currentWeather = Weather.SunnyAndWindy;
                        break;
                    case 26:
                        currentWeather = Weather.Sunny;
                        break;
                    case 27:
                        currentWeather = Weather.Sunny;
                        break;
                    case 28:
                        currentWeather = Weather.Windy;
                        break;
                    case 29:
                        currentWeather = Weather.Sunny;
                        break;
                    case 30:
                        currentWeather = Weather.Sunny;
                        break;
                    default:
                        currentWeather = Weather.Sunny;
                        break;
                }
                break;

            case Season.Fall:
                switch (currentDay)
                {
                    case 1:
                        currentWeather = Weather.Foggy;
                        break;
                    case 2:
                        currentWeather = Weather.Rainy;
                        break;
                    case 3:
                        currentWeather = Weather.Foggy;
                        break;
                    case 4:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 5:
                        currentWeather = Weather.SunnyAndWindy;
                        break;
                    case 6:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 7:
                        currentWeather = Weather.Rainy;
                        break;
                    case 8:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 9:
                        currentWeather = Weather.Foggy;
                        break;
                    case 10:
                        currentWeather = Weather.Sunny;
                        break;
                    case 11:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 12:
                        currentWeather = Weather.Rainy;
                        break;
                    case 13:
                        currentWeather = Weather.Foggy;
                        break;
                    case 14:
                        currentWeather = Weather.Sunny;
                        break;
                    case 15:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 16:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 17:
                        currentWeather = Weather.Rainy;
                        break;
                    case 18:
                        currentWeather = Weather.Thunderstorm;
                        break;
                    case 19:
                        currentWeather = Weather.SunnyAndWindy;
                        break;
                    case 20:
                        currentWeather = Weather.Foggy;
                        break;
                    case 21:
                        currentWeather = Weather.Rainy;
                        break;
                    case 22:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 23:
                        currentWeather = Weather.Rainy;
                        break;
                    case 24:
                        currentWeather = Weather.Foggy;
                        break;
                    case 25:
                        currentWeather = Weather.SunnyAndWindy;
                        break;
                    case 26:
                        currentWeather = Weather.Windy;
                        break;
                    case 27:
                        currentWeather = Weather.Rainy;
                        break;
                    case 28:
                        currentWeather = Weather.Foggy;
                        break;
                    case 29:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 30:
                        currentWeather = Weather.Snowy;
                        break;
                    case 31:
                        currentWeather = Weather.Hail;
                        break;
                    default:
                        currentWeather = Weather.Sunny;
                        break;
                }
                break;

            case Season.Winter:
                switch (currentDay)
                {
                    case 1:
                    case 2:
                        currentWeather = Weather.Rainy;
                        break;
                    case 3:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 4:
                        currentWeather = Weather.Foggy;
                        break;
                    case 5:
                        currentWeather = Weather.Snowy;
                        break;
                    case 6:
                    case 7:
                        currentWeather = Weather.Foggy;
                        break;
                    case 8:
                        currentWeather = Weather.Windy;
                        break;
                    case 9:
                        currentWeather = Weather.Snowy;
                        break;
                    case 10:
                        currentWeather = Weather.Rainy;
                        break;
                    case 11:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 12:
                        currentWeather = Weather.Rainy;
                        break;
                    case 13:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 14:
                        currentWeather = Weather.Snowy;
                        break;
                    case 15:
                        currentWeather = Weather.Thunderstorm;
                        break;
                    case 16:
                        currentWeather = Weather.Foggy;
                        break;
                    case 17:
                        currentWeather = Weather.Windy;
                        break;
                    case 18:
                        currentWeather = Weather.Rainy;
                        break;
                    case 19:
                        currentWeather = Weather.Snowy;
                        break;
                    case 20:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 21:
                        currentWeather = Weather.Foggy;
                        break;
                    case 22:
                        currentWeather = Weather.Windy;
                        break;
                    case 23:
                        currentWeather = Weather.Snowy;
                        break;
                    case 24:
                        currentWeather = Weather.Hail;
                        break;
                    case 25:
                        currentWeather = Weather.Rainy;
                        break;
                    case 26:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 27:
                        currentWeather = Weather.Foggy;
                        break;
                    case 28:
                        currentWeather = Weather.Thunderstorm;
                        break;
                    case 29:
                        currentWeather = Weather.Rainy;
                        break;
                    case 30:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 31:
                        currentWeather = Weather.Snowy;
                        break;
                    default:
                        currentWeather = Weather.Sunny;
                        break;
                }
                break;

            case Season.SpringA:
                switch (currentDay)
                {
                    case 1:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 2:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 3:
                        currentWeather = Weather.Rainy;
                        break;
                    case 4:
                        currentWeather = Weather.Sunny;
                        break;
                    case 5:
                        currentWeather = Weather.Rainy;
                        break;
                    case 6:
                        currentWeather = Weather.Foggy;
                        break;
                    case 7:
                        currentWeather = Weather.Sunny;
                        break;
                    case 8:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 9:
                        currentWeather = Weather.Rainy;
                        break;
                    case 10:
                        currentWeather = Weather.SunnyAndWindy;
                        break;
                    case 11:
                        currentWeather = Weather.Rainy;
                        break;
                    case 12:
                        currentWeather = Weather.Foggy;
                        break;
                    case 13:
                        currentWeather = Weather.Rainy;
                        break;
                    case 14:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 15:
                        currentWeather = Weather.Sunny;
                        break;
                    case 16:
                        currentWeather = Weather.Sunny;
                        break;
                    case 17:
                        currentWeather = Weather.Rainy;
                        break;
                    case 18:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 19:
                        currentWeather = Weather.SunnyAndWindy;
                        break;
                    case 20:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 21:
                        currentWeather = Weather.Rainy;
                        break;
                    case 22:
                        currentWeather = Weather.Thunderstorm;
                        break;
                    case 23:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 24:
                        currentWeather = Weather.Sunny;
                        break;
                    case 25:
                        currentWeather = Weather.Cloudy;
                        break;
                    case 26:
                        currentWeather = Weather.Rainy;
                        break;
                    case 27:
                        currentWeather = Weather.Hail;
                        break;
                    case 28:
                        currentWeather = Weather.SunnyAndCloudy;
                        break;
                    case 29:
                        currentWeather = Weather.Foggy;
                        break;
                    case 30:
                        currentWeather = Weather.Sunny;
                        break;
                    default:
                        currentWeather = Weather.Sunny;
                        break;
                }
                break;
        }
    }

    public Weather DetermineWeatherForDay(int day, Season season)
    {
        Weather determinedWeather = Weather.Sunny; // Default to Sunny

        if (season == Season.Spring)
        {
            switch (day)
            {
                case 1:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 2:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 3:
                    determinedWeather = Weather.Rainy;
                    break;
                case 4:
                    determinedWeather = Weather.Sunny;
                    break;
                case 5:
                    determinedWeather = Weather.Rainy;
                    break;
                case 6:
                    determinedWeather = Weather.Foggy;
                    break;
                case 7:
                    determinedWeather = Weather.Sunny;
                    break;
                case 8:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 9:
                    determinedWeather = Weather.Rainy;
                    break;
                case 10:
                    determinedWeather = Weather.SunnyAndWindy;
                    break;
                case 11:
                    determinedWeather = Weather.Rainy;
                    break;
                case 12:
                    determinedWeather = Weather.Foggy;
                    break;
                case 13:
                    determinedWeather = Weather.Rainy;
                    break;
                case 14:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 15:
                    determinedWeather = Weather.Sunny;
                    break;
                case 16:
                    determinedWeather = Weather.Sunny;
                    break;
                case 17:
                    determinedWeather = Weather.Rainy;
                    break;
                case 18:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 19:
                    determinedWeather = Weather.SunnyAndWindy;
                    break;
                case 20:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 21:
                    determinedWeather = Weather.Rainy;
                    break;
                case 22:
                    determinedWeather = Weather.Thunderstorm;
                    break;
                case 23:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 24:
                    determinedWeather = Weather.Sunny;
                    break;
                case 25:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 26:
                    determinedWeather = Weather.Rainy;
                    break;
                case 27:
                    determinedWeather = Weather.Hail;
                    break;
                case 28:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 29:
                    determinedWeather = Weather.Foggy;
                    break;
                case 30:
                    determinedWeather = Weather.Sunny;
                    break;
                default:
                    determinedWeather = Weather.Sunny;
                    break;
            }
        }

        if (season == Season.Summer)
        {
            switch (day)
            {
                case 1:
                    determinedWeather = Weather.Sunny;
                    break;
                case 2:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 3:
                    determinedWeather = Weather.Sunny;
                    break;
                case 4:
                    determinedWeather = Weather.Sunny;
                    break;
                case 5:
                    determinedWeather = Weather.SunnyAndWindy;
                    break;
                case 6:
                    determinedWeather = Weather.Sunny;
                    break;
                case 7:
                    determinedWeather = Weather.Sunny;
                    break;
                case 8:
                    determinedWeather = Weather.Sunny;
                    break;
                case 9:
                    determinedWeather = Weather.Sunny;
                    break;
                case 10:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 11:
                    determinedWeather = Weather.Sunny;
                    break;
                case 12:
                    determinedWeather = Weather.Foggy;
                    break;
                case 13:
                    determinedWeather = Weather.SunnyAndWindy;
                    break;
                case 14:
                    determinedWeather = Weather.Sunny;
                    break;
                case 15:
                    determinedWeather = Weather.Sunny;
                    break;
                case 16:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 17:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 18:
                    determinedWeather = Weather.Sunny;
                    break;
                case 19:
                    determinedWeather = Weather.Sunny;
                    break;
                case 20:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 21:
                    determinedWeather = Weather.Windy;
                    break;
                case 22:
                    determinedWeather = Weather.Sunny;
                    break;
                case 23:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 24:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 25:
                    determinedWeather = Weather.SunnyAndWindy;
                    break;
                case 26:
                    determinedWeather = Weather.Sunny;
                    break;
                case 27:
                    determinedWeather = Weather.Sunny;
                    break;
                case 28:
                    determinedWeather = Weather.Windy;
                    break;
                case 29:
                    determinedWeather = Weather.Sunny;
                    break;
                case 30:
                    determinedWeather = Weather.Sunny;
                    break;
                default:
                    determinedWeather = Weather.Sunny;
                    break;
            }
        }

        if (season == Season.Fall)
        {
            switch (day)
            {
                case 1:
                    determinedWeather = Weather.Foggy;
                    break;
                case 2:
                    determinedWeather = Weather.Rainy;
                    break;
                case 3:
                    determinedWeather = Weather.Foggy;
                    break;
                case 4:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 5:
                    determinedWeather = Weather.SunnyAndWindy;
                    break;
                case 6:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 7:
                    determinedWeather = Weather.Rainy;
                    break;
                case 8:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 9:
                    determinedWeather = Weather.Foggy;
                    break;
                case 10:
                    determinedWeather = Weather.Sunny;
                    break;
                case 11:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 12:
                    determinedWeather = Weather.Rainy;
                    break;
                case 13:
                    determinedWeather = Weather.Foggy;
                    break;
                case 14:
                    determinedWeather = Weather.Sunny;
                    break;
                case 15:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 16:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 17:
                    determinedWeather = Weather.Rainy;
                    break;
                case 18:
                    determinedWeather = Weather.Thunderstorm;
                    break;
                case 19:
                    determinedWeather = Weather.SunnyAndWindy;
                    break;
                case 20:
                    determinedWeather = Weather.Foggy;
                    break;
                case 21:
                    determinedWeather = Weather.Rainy;
                    break;
                case 22:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 23:
                    determinedWeather = Weather.Rainy;
                    break;
                case 24:
                    determinedWeather = Weather.Foggy;
                    break;
                case 25:
                    determinedWeather = Weather.SunnyAndWindy;
                    break;
                case 26:
                    determinedWeather = Weather.Windy;
                    break;
                case 27:
                    determinedWeather = Weather.Rainy;
                    break;
                case 28:
                    determinedWeather = Weather.Foggy;
                    break;
                case 29:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 30:
                    determinedWeather = Weather.Snowy;
                    break;
                case 31:
                    determinedWeather = Weather.Hail;
                    break;
                default:
                    determinedWeather = Weather.Sunny;
                    break;
            }
        }

        if (season == Season.Winter)
        {
            switch (day)
            {
                case 1:
                case 2:
                    determinedWeather = Weather.Rainy;
                    break;
                case 3:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 4:
                    determinedWeather = Weather.Foggy;
                    break;
                case 5:
                    determinedWeather = Weather.Snowy;
                    break;
                case 6:
                case 7:
                    determinedWeather = Weather.Foggy;
                    break;
                case 8:
                    determinedWeather = Weather.Windy;
                    break;
                case 9:
                    determinedWeather = Weather.Snowy;
                    break;
                case 10:
                    determinedWeather = Weather.Rainy;
                    break;
                case 11:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 12:
                    determinedWeather = Weather.Rainy;
                    break;
                case 13:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 14:
                    determinedWeather = Weather.Snowy;
                    break;
                case 15:
                    determinedWeather = Weather.Thunderstorm;
                    break;
                case 16:
                    determinedWeather = Weather.Foggy;
                    break;
                case 17:
                    determinedWeather = Weather.Windy;
                    break;
                case 18:
                    determinedWeather = Weather.Rainy;
                    break;
                case 19:
                    determinedWeather = Weather.Snowy;
                    break;
                case 20:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 21:
                    determinedWeather = Weather.Foggy;
                    break;
                case 22:
                    determinedWeather = Weather.Windy;
                    break;
                case 23:
                    determinedWeather = Weather.Snowy;
                    break;
                case 24:
                    determinedWeather = Weather.Hail;
                    break;
                case 25:
                    determinedWeather = Weather.Rainy;
                    break;
                case 26:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 27:
                    determinedWeather = Weather.Foggy;
                    break;
                case 28:
                    determinedWeather = Weather.Thunderstorm;
                    break;
                case 29:
                    determinedWeather = Weather.Rainy;
                    break;
                case 30:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 31:
                    determinedWeather = Weather.Snowy;
                    break;
                default:
                    determinedWeather = Weather.Sunny;
                    break;
            }
        }

        if (season == Season.SpringA)
        {
            switch (day)
            {
                case 1:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 2:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 3:
                    determinedWeather = Weather.Rainy;
                    break;
                case 4:
                    determinedWeather = Weather.Sunny;
                    break;
                case 5:
                    determinedWeather = Weather.Rainy;
                    break;
                case 6:
                    determinedWeather = Weather.Foggy;
                    break;
                case 7:
                    determinedWeather = Weather.Sunny;
                    break;
                case 8:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 9:
                    determinedWeather = Weather.Rainy;
                    break;
                case 10:
                    determinedWeather = Weather.SunnyAndWindy;
                    break;
                case 11:
                    determinedWeather = Weather.Rainy;
                    break;
                case 12:
                    determinedWeather = Weather.Foggy;
                    break;
                case 13:
                    determinedWeather = Weather.Rainy;
                    break;
                case 14:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 15:
                    determinedWeather = Weather.Sunny;
                    break;
                case 16:
                    determinedWeather = Weather.Sunny;
                    break;
                case 17:
                    determinedWeather = Weather.Rainy;
                    break;
                case 18:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 19:
                    determinedWeather = Weather.SunnyAndWindy;
                    break;
                case 20:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 21:
                    determinedWeather = Weather.Rainy;
                    break;
                case 22:
                    determinedWeather = Weather.Thunderstorm;
                    break;
                case 23:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 24:
                    determinedWeather = Weather.Sunny;
                    break;
                case 25:
                    determinedWeather = Weather.Cloudy;
                    break;
                case 26:
                    determinedWeather = Weather.Rainy;
                    break;
                case 27:
                    determinedWeather = Weather.Hail;
                    break;
                case 28:
                    determinedWeather = Weather.SunnyAndCloudy;
                    break;
                case 29:
                    determinedWeather = Weather.Foggy;
                    break;
                case 30:
                    determinedWeather = Weather.Sunny;
                    break;
                default:
                    determinedWeather = Weather.Sunny;
                    break;
            }
        }

        return determinedWeather;
    }*/

    public Weather DetermineWeatherForDay(int day, Season season)
    {
        Weather determinedWeather = Weather.Sunny; // Default to Sunny

        // [Insert existing switch-case logic here to assign determinedWeather based on season and day]
        // Assume it's unchanged from before 
        switch (season)
        {
            case Season.Spring:
                switch (day)
                {
                    case 1:
                    case 2:
                        determinedWeather = Weather.SunnyAndCloudy;
                        break;
                    case 3:
                    case 5:
                    case 9:
                    case 11:
                    case 13:
                    case 17:
                    case 21:
                    case 26:
                        determinedWeather = Weather.Rainy;
                        break;
                    case 4:
                    case 7:
                    case 15:
                    case 16:
                    case 24:
                    case 30:
                        determinedWeather = Weather.Sunny;
                        break;
                    case 6:
                    case 12:
                    case 29:
                        determinedWeather = Weather.Foggy;
                        break;
                    case 8:
                    case 18:
                    case 25:
                        determinedWeather = Weather.Cloudy;
                        break;
                    case 10:
                    case 19:
                        determinedWeather = Weather.SunnyAndWindy;
                        break;
                    case 14:
                    case 20:
                    case 23:
                    case 28:
                        determinedWeather = Weather.SunnyAndCloudy;
                        break;
                    case 22:
                        determinedWeather = Weather.Thunderstorm;
                        break;
                    case 27:
                        determinedWeather = Weather.Hail;
                        break;
                }
                break;

            case Season.Summer:
                switch (day)
                {
                    case 1:
                    case 3:
                    case 4:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 11:
                    case 14:
                    case 15:
                    case 18:
                    case 19:
                    case 22:
                    case 26:
                    case 27:
                    case 29:
                    case 30:
                        determinedWeather = Weather.Sunny;
                        break;
                    case 2:
                    case 10:
                    case 16:
                    case 20:
                    case 23:
                        determinedWeather = Weather.SunnyAndCloudy;
                        break;
                    case 5:
                    case 13:
                    case 25:
                        determinedWeather = Weather.SunnyAndWindy;
                        break;
                    case 12:
                        determinedWeather = Weather.Foggy;
                        break;
                    case 17:
                    case 24:
                        determinedWeather = Weather.Cloudy;
                        break;
                    case 21:
                    case 28:
                        determinedWeather = Weather.Windy;
                        break;
                }
                break;

            case Season.Fall:
                switch (day)
                {
                    case 1:
                    case 3:
                    case 9:
                    case 13:
                    case 20:
                    case 24:
                    case 28:
                    case 30: 
                        determinedWeather = Weather.Foggy;
                        break;
                    case 2:
                    case 7:
                    case 12:
                    case 17:
                    case 21:
                    case 23:
                    case 27:
                        determinedWeather = Weather.Rainy;
                        break;
                    case 4:
                    case 16:
                    case 29:
                        determinedWeather = Weather.SunnyAndCloudy;
                        break;
                    case 5:
                    case 19:
                    case 25:
                        determinedWeather = Weather.SunnyAndWindy;
                        break;
                    case 6:
                    case 8:
                    case 11:
                    case 15:
                    case 22:
                    case 26:
                    case 31: 
                        determinedWeather = Weather.Cloudy; 
                        break;
                    case 10:
                    case 14:
                        determinedWeather = Weather.Sunny;
                        break;
                    case 18:
                        determinedWeather = Weather.Thunderstorm;
                        break;
                }
                break;

            case Season.Winter:
                switch (day)
                {
                    case 1:
                    case 2:
                    case 10:
                    case 12:
                    case 18: 
                    case 29:
                        determinedWeather = Weather.Rainy;
                        break;
                    case 3:
                    case 11:
                    case 13:
                    case 20:
                    case 26:
                    case 30:
                        determinedWeather = Weather.Cloudy;
                        break;
                    case 4:
                    case 6:
                    case 7:
                    case 16:
                    case 21:
                    case 27:
                        determinedWeather = Weather.Foggy;
                        break;
                    case 5:
                    case 9:
                    case 14:
                    case 19:
                    case 23:
                    case 24:
                    case 25: 
                    case 31:
                        determinedWeather = Weather.Snowy;
                        break;
                    case 8:
                    case 17:
                    case 22:
                        determinedWeather = Weather.Windy;
                        break;
                    case 15:
                    case 28:
                        determinedWeather = Weather.Thunderstorm;
                        break; 
                }
                break;

            case Season.SpringA:
                return DetermineWeatherForDay(day, Season.Spring);
        } 

        // After determining the weather, also get the temperature
        int year = season == Season.SpringA ? 2020 : 2019;
        WeatherSystem.WeatherData weatherData = WeatherSystem.Instance.GetWeatherForSeasonDay(season, day, year);

        int temp = 0;

        switch (activityCounter)
        {
            case 0:
                temp = weatherData.morningTemp;
                break;
            case 1:
                temp = weatherData.afternoonTemp;
                break;
            case 2:
                temp = weatherData.eveningTemp;
                break;
        }

        VariableStore.TrySetValue("Temperature", temp); 

        return determinedWeather;
    }

    public void ResetCalendar()
    {
        currentSeason = Season.Spring;
        currentDay = 15; 
        activityCounter = 0;

        PlayerPrefs.DeleteKey("CurrentSeason");
        PlayerPrefs.DeleteKey("CurrentDay");
        PlayerPrefs.DeleteKey("CalendarPosX");
        PlayerPrefs.DeleteKey("CalendarPosY");
        PlayerPrefs.Save();

        if (SlidingCalendarMenu.instance != null)
        {
            Vector2 resetPosition = new Vector2(3650f, -310f);
            SlidingCalendarMenu.instance.calendarContainer.anchoredPosition = resetPosition;

            SlidingCalendarMenu.instance.SaveCalendarPosition(resetPosition);

            SlidingCalendarMenu.instance.UpdateCalendarUI(false);
        }

        UpdateSeasonDayText();
        UpdateActivityText();
        SaveSeasonAndDay();

        if (CalendarMenu.instance != null)
        {
            CalendarMenu.instance.displayedSeasonIndex = (int)currentSeason; 
            CalendarMenu.instance.UpdateCalendar();
        }

        // Update the TownMapManager's seasonDayText if it exists
        TownMapManager townMapManager = FindObjectOfType<TownMapManager>();
        if (townMapManager != null)
        {
            townMapManager.UpdateSeasonDayText();
        }
    }
} 
