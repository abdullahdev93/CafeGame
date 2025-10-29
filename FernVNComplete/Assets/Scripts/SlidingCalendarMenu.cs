using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SlidingCalendarMenu : MonoBehaviour
{
    public RectTransform calendarContainer; // The container that holds the calendar days
    public GameObject CalendarSlideMenu;
    public GameObject parentOfCalendarDays; // Parent GameObject containing the calendar day images
    public float slideDuration = 1f; // Duration of the sliding effect 

    private Image[] calendarDays; // Array of images representing the days in the calendar
    private int targetDayIndex; // The index of the day we're sliding to

    public TextMeshProUGUI seasonDayText; // Text component to display the current season and day 

    public TextMeshProUGUI temperatureText;

    //public TextMeshProUGUI previousTemperatureText;
    //public TextMeshProUGUI nextTemperatureText; 

    public Sprite sunnySprite;
    public Sprite cloudySprite;
    public Sprite sunnyCloudySprite;
    public Sprite rainySprite;
    public Sprite windySprite;
    public Sprite snowySprite;
    public Sprite foggySprite;
    public Sprite thunderstormSprite; 

    public Image weatherImage; // Image component for displaying the weather

    public static SlidingCalendarMenu instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            calendarDays = parentOfCalendarDays.GetComponentsInChildren<Image>();

            if (calendarContainer == null)
            {
                Debug.LogError("Calendar Container is not assigned!");
            }

            if (calendarDays == null || calendarDays.Length == 0)
            {
                Debug.LogError("Calendar Days are not properly initialized!");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Time.timeScale = 1f;
        LoadCalendarPosition();
        UpdateCalendarUI(true);
        SlideToDay(CalendarSystem.Instance.GetCurrentDay());
    }

    public void SlideToDay(int dayIndex)
    {
        targetDayIndex = dayIndex - 1; // Subtract 1 because arrays are 0-indexed
        StartCoroutine(TransitionToVisualNovel());
    }

    public void UpdateCalendarUI(bool showPrevious = false)
    {
        int day = showPrevious ? CalendarSystem.Instance.GetCurrentDay() - 1 : CalendarSystem.Instance.GetCurrentDay();
        CalendarSystem.Season season = CalendarSystem.Instance.GetCurrentSeason();

        if (day < 1)
        {
            season = (CalendarSystem.Season)((int)season - 1);
            if (season < CalendarSystem.Season.Spring)
            {
                season = CalendarSystem.Season.SpringA; // Wrap around to the last season
            }

            day = CalendarSystem.Instance.GetDaysInSeason(season); // Get the last day of the previous season
        }

        targetDayIndex = day - 1; // Subtract 1 because arrays are 0-indexed

        string dayText = day < 10 ? "0" + day.ToString() : day.ToString();
        string year = season == CalendarSystem.Season.SpringA ? "2020" : "2019";

        // Update the season and day text
        seasonDayText.text = $"{season} {dayText}, {year}";

        WeatherSystem.WeatherData weatherData = WeatherSystem.Instance.GetWeatherForSeasonDay(season, day, int.Parse(year));
        //CalendarSystem.Weather weather = weatherData.weather; 
        CalendarSystem.Weather weather = weatherData.weatherType;
        
        int temp = 0;

        switch (CalendarSystem.Instance.activityCounter)
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

        int displayTemp = VN_Configuration.activeConfig.useCelsius
       ? Mathf.RoundToInt((temp - 32) * 5f / 9f)
       : temp;

        string tempString = VN_Configuration.activeConfig.useCelsius
            ? $"{displayTemp}°C"
            : $"{displayTemp}°F";

        // Always update the main temperature text
        if (temperatureText != null)
            temperatureText.text = tempString; 

        /*if (CalendarSystem.Instance.temperatureText != null)
        {
            if (showPrevious && previousTemperatureText != null)
            {
                if (VN_Configuration.activeConfig.useCelsius)
                {
                    int celsius = Mathf.RoundToInt((temp - 32) * 5f / 9f);
                    previousTemperatureText.text = $"{celsius}°C";
                }
                else
                {
                    previousTemperatureText.text = $"{temp}°F";
                }
            }
            else if (!showPrevious && nextTemperatureText != null)
            {
                if (VN_Configuration.activeConfig.useCelsius)
                {
                    int celsius = Mathf.RoundToInt((temp - 32) * 5f / 9f);
                    nextTemperatureText.text = $"{celsius}°C";
                }
                else
                {
                    nextTemperatureText.text = $"{temp}°F";
                }
            }

            VariableStore.TrySetValue("Temperature", temp);
        }*/

        UpdateWeatherIcon(weather);
    } 

    private IEnumerator SlideCalendar()
    {
        Vector2 originalPosition = calendarContainer.anchoredPosition;
        Vector2 targetPosition = originalPosition - new Vector2(105f, 0); // Move left by 105 units   

        // 1. Show Previous Day's Evening Temperature
        int prevDay = CalendarSystem.Instance.currentDay - 1;
        CalendarSystem.Season prevSeason = CalendarSystem.Instance.currentSeason;
        if (prevDay < 1)
        {
            prevSeason = (CalendarSystem.Season)((int)prevSeason - 1);
            if ((int)prevSeason < 0) prevSeason = CalendarSystem.Season.SpringA;
            prevDay = CalendarSystem.Instance.GetDaysInSeason(prevSeason);
        }

        WeatherSystem.WeatherData prevWeather = WeatherSystem.Instance.GetWeatherForSeasonDay(prevSeason, prevDay, prevSeason == CalendarSystem.Season.SpringA ? 2020 : 2019);
        int prevEveningTemp = prevWeather.eveningTemp;
        if (VN_Configuration.activeConfig.useCelsius)
            prevEveningTemp = Mathf.RoundToInt((prevEveningTemp - 32) * 5f / 9f);
        temperatureText.text = $"{prevEveningTemp}°" + (VN_Configuration.activeConfig.useCelsius ? "C" : "F");
        yield return new WaitForSeconds(1f); 

        // 2. Show Current Day's Evening Temperature
        //CalendarSystem.Season currentSeason = CalendarSystem.Instance.currentSeason;
        //int currentDay = CalendarSystem.Instance.currentDay;
        //WeatherSystem.WeatherData todayWeather = WeatherSystem.Instance.GetWeatherForSeasonDay(currentSeason, currentDay, currentSeason == CalendarSystem.Season.SpringA ? 2020 : 2019);
        //int todayEveningTemp = todayWeather.eveningTemp;
        //if (VN_Configuration.activeConfig.useCelsius)
            //todayEveningTemp = Mathf.RoundToInt((todayEveningTemp - 32) * 5f / 9f);
        //temperatureText.text = $"{todayEveningTemp}°" + (VN_Configuration.activeConfig.useCelsius ? "C" : "F");
        //yield return new WaitForSeconds(1f);

        float elapsedTime = 0;

        while (elapsedTime < slideDuration)
        {
            calendarContainer.anchoredPosition = Vector2.Lerp(originalPosition, targetPosition, elapsedTime / slideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        } 

        calendarContainer.anchoredPosition = targetPosition;

        SaveCalendarPosition(targetPosition);

        // 4. Show New Day's Morning Temperature
        CalendarSystem.Season newSeason = CalendarSystem.Instance.currentSeason;
        int newDay = CalendarSystem.Instance.currentDay;
        if (++newDay > CalendarSystem.Instance.GetDaysInSeason(newSeason))
        {
            newDay = 1;
            newSeason = (CalendarSystem.Season)(((int)newSeason + 1) % 5);
        }

        WeatherSystem.WeatherData nextWeather = WeatherSystem.Instance.GetWeatherForSeasonDay(newSeason, newDay, newSeason == CalendarSystem.Season.SpringA ? 2020 : 2019);
        int morningTemp = nextWeather.morningTemp;
        if (VN_Configuration.activeConfig.useCelsius)
            morningTemp = Mathf.RoundToInt((morningTemp - 32) * 5f / 9f);
        temperatureText.text = $"{morningTemp}°" + (VN_Configuration.activeConfig.useCelsius ? "C" : "F"); 

        UpdateCalendarUI(false); // Update UI after the slide

        // Now showing current day's temperature
        //int newTemp = CalendarSystem.Instance.GetTemperatureForToday(); 

        //WeatherSystem.WeatherData weatherData = WeatherSystem.Instance.GetWeatherForSeasonDay(CalendarSystem.Instance.GetCurrentSeason(), CalendarSystem.Instance.GetCurrentDay(), 2019);
        //WeatherSystem.WeatherData weatherData = WeatherSystem.Instance.GetWeatherForSeasonDay(CalendarSystem.Instance.GetCurrentSeason(), CalendarSystem.Instance.GetCurrentDay(), CalendarSystem.Instance.GetCurrentSeason() == CalendarSystem.Season.SpringA ? 2020 : 2019);
        //int newTemp = weatherData.temperature; 

        /*if (nextTemperatureText != null)
        {
            if (VN_Configuration.activeConfig.useCelsius)
            {
                int celsius = Mathf.RoundToInt((newTemp - 32) * 5f / 9f);
                nextTemperatureText.text = $"{celsius}°C";
            }
            else
            {
                nextTemperatureText.text = $"{newTemp}°F"; 
            }
        }*/

        CalendarSystem.Instance.UpdateSeasonDayText();

        //UpdateWeatherIcon(weather); // Update the weather icon after the slide 

        // **Reset the Notification Counter after the slide**
        //DirectMessageManager.Instance.ResetNotificationCounter(); // Reset the notification counter after the slide finishes.
    }

    private void UpdateWeatherIcon(CalendarSystem.Weather weather)
    {
        //CalendarSystem.Weather weather = CalendarSystem.Instance.GetCurrentWeather();

        switch (weather)
        {
            case CalendarSystem.Weather.Sunny:
                weatherImage.sprite = sunnySprite;
                break;
            case CalendarSystem.Weather.Cloudy:
                weatherImage.sprite = cloudySprite;
                break;
            case CalendarSystem.Weather.SunnyAndCloudy:
                weatherImage.sprite = sunnyCloudySprite;
                break;
            case CalendarSystem.Weather.Rainy:
                weatherImage.sprite = rainySprite;
                break;
            case CalendarSystem.Weather.Windy:
                weatherImage.sprite = windySprite;
                break;
            case CalendarSystem.Weather.Snowy: 
                weatherImage.sprite = snowySprite; 
                break;
            case CalendarSystem.Weather.Foggy:
                weatherImage.sprite = foggySprite; 
                break;
            case CalendarSystem.Weather.Thunderstorm:
                weatherImage.sprite = thunderstormSprite; 
                break;
        }
    }

    private IEnumerator TransitionToVisualNovel()
    {
        yield return SlideCalendar();
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("VisualNovel");
    }

    public void ShowCalendarMenu()
    {
        UpdateCalendarUI(true); // Show the previous season and day before sliding
        CalendarSlideMenu.SetActive(true);
    }

    public void HideCalendarMenu()
    {
        CalendarSlideMenu.SetActive(false);
    }

    public IEnumerator SlideAndHide()
    {
        SlideToDay(CalendarSystem.Instance.GetCurrentDay());
        yield return new WaitForSeconds(slideDuration);
        yield return new WaitForSeconds(3f);
        HideCalendarMenu();
    }

    public void SaveCalendarPosition(Vector2 position)
    {
        PlayerPrefs.SetFloat("CalendarPosX", position.x);
        PlayerPrefs.SetFloat("CalendarPosY", position.y);
        PlayerPrefs.Save();
    }

    public void LoadCalendarPosition()
    {
        if (PlayerPrefs.HasKey("CalendarPosX") && PlayerPrefs.HasKey("CalendarPosY"))
        {
            float x = PlayerPrefs.GetFloat("CalendarPosX");
            float y = PlayerPrefs.GetFloat("CalendarPosY");
            calendarContainer.anchoredPosition = new Vector2(x, y);
        }
        else
        {
            calendarContainer.anchoredPosition = new Vector2(6373f, -310f); 
        }
    }
}
