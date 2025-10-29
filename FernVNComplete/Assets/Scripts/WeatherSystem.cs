using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeatherSystem : MonoBehaviour
{
    public static WeatherSystem Instance;

    public Sprite sunnySprite;
    public Sprite cloudySprite;
    public Sprite sunnyCloudySprite;
    public Sprite rainySprite;
    public Sprite foggySprite;
    public Sprite sunnyWindySprite;
    public Sprite thunderstormSprite;
    public Sprite hailSprite;
    public Sprite windySprite;
    public Sprite snowySprite; 

    private Image weatherImage;

    public TextMeshProUGUI temperatureText;

    private string nextTemperature;
    private string currentTemperature; 

    private void Awake()
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
    }

    public void SetWeatherImage(Image imageComponent)
    {
        weatherImage = imageComponent;
    }

    public void UpdateWeather(CalendarSystem.Season season, int day, int year)
    {
        WeatherData weatherData = GetWeatherForSeasonDay(season, day, year);
        Sprite weatherSprite = weatherData.sprite;
        
        int temperature = 0;

        if (CalendarSystem.Instance.activityCounter == 0)
            temperature = weatherData.morningTemp;
        else if (CalendarSystem.Instance.activityCounter == 1)
            temperature = weatherData.afternoonTemp;
        else if (CalendarSystem.Instance.activityCounter == 2)
            temperature = weatherData.eveningTemp;


        //CalendarSystem.Weather weather = CalendarSystem.Instance.DetermineWeatherForDay(day, season); 
        CalendarSystem.Weather weather = weatherData.weatherType; 
        //int temperature = GetTemperature(season, weather); 
        if (weatherImage != null && weatherSprite != null)
        {
            weatherImage.sprite = weatherSprite;
        }

        if (temperatureText != null)
        {
            temperatureText.text = $"{temperature}°F";
        } 

        // OPTIONAL: Log or use this temp in UI
        Debug.Log($"Weather for Day {day} of {season}: {weather}, Temperature: {temperature}°F");

        VariableStore.TrySetValue("Temperature", temperature); // Store for use in dialogue, etc. 
    }

    public void LoadPreviousWeather()
    {
        int previousDay = PlayerPrefs.GetInt("PreviousDay", 1);
        CalendarSystem.Season previousSeason = (CalendarSystem.Season)PlayerPrefs.GetInt("PreviousSeason", (int)CalendarSystem.Season.Spring);
        string year = previousSeason == CalendarSystem.Season.SpringA ? "2020" : "2019";

        UpdateWeather(previousSeason, previousDay, int.Parse(year));
    }

    public struct WeatherData
    {
        public Sprite sprite;
        //public int temperature; 
        public int morningTemp;
        public int afternoonTemp;
        public int eveningTemp;
        public CalendarSystem.Weather weatherType;

        public WeatherData(Sprite sprite, int morningTemp, int afternoonTemp, int eveningTemp, CalendarSystem.Weather weatherType)
        {
            this.sprite = sprite;
            this.morningTemp = morningTemp;
            this.afternoonTemp = afternoonTemp;
            this.eveningTemp = eveningTemp;
            this.weatherType = weatherType;
        }

        public int GetTemperatureForPhase(int activityCounter)
        {
            switch (activityCounter)
            {
                case 0: return morningTemp;
                case 1: return afternoonTemp;
                case 2: return eveningTemp;
                default: return morningTemp;
            }
        } 
    }

    public WeatherData GetWeatherForSeasonDay(CalendarSystem.Season season, int day, int year)
    {
        if (season == CalendarSystem.Season.Spring)
        {
            switch (day)
            {
                case 1: return new(sunnyCloudySprite, 54, 66, 59, CalendarSystem.Weather.SunnyAndCloudy);
                case 2: return new(sunnyCloudySprite, 52, 64, 56, CalendarSystem.Weather.SunnyAndCloudy);
                case 3: return new(rainySprite, 46, 58, 50, CalendarSystem.Weather.Rainy);
                case 4: return new(sunnySprite, 53, 67, 60, CalendarSystem.Weather.Sunny);
                case 5: return new(rainySprite, 47, 60, 51, CalendarSystem.Weather.Rainy);
                case 6: return new(foggySprite, 43, 55, 46, CalendarSystem.Weather.Foggy);
                case 7: return new(sunnySprite, 55, 68, 61, CalendarSystem.Weather.Sunny);
                case 8: return new(cloudySprite, 50, 62, 54, CalendarSystem.Weather.Cloudy);
                case 9: return new(rainySprite, 48, 59, 52, CalendarSystem.Weather.Rainy);
                case 10: return new(sunnyWindySprite, 53, 65, 58, CalendarSystem.Weather.SunnyAndWindy);
                case 11: return new(rainySprite, 46, 57, 50, CalendarSystem.Weather.Rainy);
                case 12: return new(foggySprite, 44, 54, 48, CalendarSystem.Weather.Foggy);
                case 13: return new(rainySprite, 45, 56, 49, CalendarSystem.Weather.Rainy);
                case 14: return new(sunnyCloudySprite, 54, 67, 60, CalendarSystem.Weather.SunnyAndCloudy);
                case 15: return new(sunnySprite, 56, 70, 63, CalendarSystem.Weather.Sunny);
                case 16: return new(sunnySprite, 58, 71, 64, CalendarSystem.Weather.Sunny);
                case 17: return new(rainySprite, 49, 61, 53, CalendarSystem.Weather.Rainy);
                case 18: return new(cloudySprite, 51, 63, 56, CalendarSystem.Weather.Cloudy);
                case 19: return new(sunnyWindySprite, 55, 68, 61, CalendarSystem.Weather.SunnyAndWindy);
                case 20: return new(sunnyCloudySprite, 53, 66, 60, CalendarSystem.Weather.SunnyAndCloudy);
                case 21: return new(rainySprite, 47, 59, 52, CalendarSystem.Weather.Rainy);
                case 22: return new(thunderstormSprite, 48, 60, 54, CalendarSystem.Weather.Thunderstorm);
                case 23: return new(sunnyCloudySprite, 52, 65, 59, CalendarSystem.Weather.SunnyAndCloudy);
                case 24: return new(sunnySprite, 57, 69, 63, CalendarSystem.Weather.Sunny);
                case 25: return new(cloudySprite, 50, 62, 56, CalendarSystem.Weather.Cloudy);
                case 26: return new(rainySprite, 46, 58, 51, CalendarSystem.Weather.Rainy);
                case 27: return new(hailSprite, 42, 53, 47, CalendarSystem.Weather.Hail);
                case 28: return new(sunnyCloudySprite, 54, 67, 60, CalendarSystem.Weather.SunnyAndCloudy);
                case 29: return new(foggySprite, 45, 56, 49, CalendarSystem.Weather.Foggy);
                case 30: return new(sunnySprite, 56, 70, 63, CalendarSystem.Weather.Sunny);
                default: return new(sunnySprite, 55, 68, 62, CalendarSystem.Weather.Sunny); 
            }
        }

        if (season == CalendarSystem.Season.Summer)
        {
            switch (day)
            {
                case 1: return new(sunnySprite, 63, 85, 69, CalendarSystem.Weather.Sunny);
                case 2: return new(sunnyCloudySprite, 62, 83, 68, CalendarSystem.Weather.SunnyAndCloudy);
                case 3: return new(sunnySprite, 65, 88, 70, CalendarSystem.Weather.Sunny);
                case 4: return new(sunnySprite, 66, 89, 72, CalendarSystem.Weather.Sunny);
                case 5: return new(sunnyWindySprite, 64, 86, 70, CalendarSystem.Weather.SunnyAndWindy);
                case 6: return new(sunnySprite, 67, 90, 73, CalendarSystem.Weather.Sunny);
                case 7: return new(sunnySprite, 66, 91, 72, CalendarSystem.Weather.Sunny);
                case 8: return new(sunnySprite, 65, 89, 71, CalendarSystem.Weather.Sunny);
                case 9: return new(sunnySprite, 64, 88, 70, CalendarSystem.Weather.Sunny);
                case 10: return new(sunnyCloudySprite, 63, 84, 69, CalendarSystem.Weather.SunnyAndCloudy);
                case 11: return new(sunnySprite, 65, 87, 71, CalendarSystem.Weather.Sunny);
                case 12: return new(foggySprite, 58, 77, 64, CalendarSystem.Weather.Foggy);
                case 13: return new(sunnyWindySprite, 62, 85, 69, CalendarSystem.Weather.SunnyAndWindy);
                case 14: return new(sunnySprite, 66, 89, 72, CalendarSystem.Weather.Sunny);
                case 15: return new(sunnySprite, 67, 90, 73, CalendarSystem.Weather.Sunny);
                case 16: return new(sunnyCloudySprite, 62, 83, 68, CalendarSystem.Weather.SunnyAndCloudy);
                case 17: return new(cloudySprite, 61, 81, 67, CalendarSystem.Weather.Cloudy);
                case 18: return new(sunnySprite, 65, 88, 70, CalendarSystem.Weather.Sunny);
                case 19: return new(sunnySprite, 66, 89, 72, CalendarSystem.Weather.Sunny);
                case 20: return new(sunnyCloudySprite, 63, 84, 69, CalendarSystem.Weather.SunnyAndCloudy);
                case 21: return new(windySprite, 60, 82, 67, CalendarSystem.Weather.Windy);
                case 22: return new(sunnySprite, 65, 87, 71, CalendarSystem.Weather.Sunny);
                case 23: return new(sunnyCloudySprite, 62, 83, 68, CalendarSystem.Weather.SunnyAndCloudy);
                case 24: return new(cloudySprite, 59, 80, 66, CalendarSystem.Weather.Cloudy);
                case 25: return new(sunnyWindySprite, 63, 85, 70, CalendarSystem.Weather.SunnyAndWindy);
                case 26: return new(sunnySprite, 65, 88, 71, CalendarSystem.Weather.Sunny);
                case 27: return new(sunnySprite, 66, 89, 72, CalendarSystem.Weather.Sunny);
                case 28: return new(windySprite, 61, 83, 68, CalendarSystem.Weather.Windy);
                case 29: return new(sunnySprite, 64, 87, 70, CalendarSystem.Weather.Sunny);
                case 30: return new(sunnySprite, 63, 86, 69, CalendarSystem.Weather.Sunny);
                default: return new(sunnySprite, 63, 85, 70, CalendarSystem.Weather.Sunny); 
            }
        }

        if (season == CalendarSystem.Season.Fall)
        {
            switch (day)
            {
                case 1: return new(foggySprite, 48, 60, 52, CalendarSystem.Weather.Foggy);
                case 2: return new(rainySprite, 46, 58, 50, CalendarSystem.Weather.Rainy);
                case 3: return new(foggySprite, 45, 57, 49, CalendarSystem.Weather.Foggy);
                case 4: return new(sunnyCloudySprite, 52, 65, 56, CalendarSystem.Weather.SunnyAndCloudy);
                case 5: return new(sunnyWindySprite, 54, 67, 59, CalendarSystem.Weather.SunnyAndWindy);
                case 6: return new(cloudySprite, 50, 62, 55, CalendarSystem.Weather.Cloudy);
                case 7: return new(rainySprite, 44, 56, 48, CalendarSystem.Weather.Rainy);
                case 8: return new(cloudySprite, 49, 61, 54, CalendarSystem.Weather.Cloudy);
                case 9: return new(foggySprite, 47, 59, 51, CalendarSystem.Weather.Foggy);
                case 10: return new(sunnySprite, 55, 70, 60, CalendarSystem.Weather.Sunny);
                case 11: return new(cloudySprite, 51, 64, 56, CalendarSystem.Weather.Cloudy);
                case 12: return new(rainySprite, 46, 58, 50, CalendarSystem.Weather.Rainy);
                case 13: return new(foggySprite, 43, 55, 47, CalendarSystem.Weather.Foggy);
                case 14: return new(sunnySprite, 54, 69, 59, CalendarSystem.Weather.Sunny);
                case 15: return new(cloudySprite, 50, 63, 55, CalendarSystem.Weather.Cloudy);
                case 16: return new(sunnyCloudySprite, 53, 66, 58, CalendarSystem.Weather.SunnyAndCloudy);
                case 17: return new(rainySprite, 45, 57, 49, CalendarSystem.Weather.Rainy);
                case 18: return new(thunderstormSprite, 44, 56, 48, CalendarSystem.Weather.Thunderstorm);
                case 19: return new(sunnyWindySprite, 56, 68, 60, CalendarSystem.Weather.SunnyAndWindy);
                case 20: return new(foggySprite, 42, 54, 46, CalendarSystem.Weather.Foggy);
                case 21: return new(rainySprite, 46, 59, 51, CalendarSystem.Weather.Rainy);
                case 22: return new(cloudySprite, 48, 61, 53, CalendarSystem.Weather.Cloudy);
                case 23: return new(rainySprite, 47, 58, 50, CalendarSystem.Weather.Rainy);
                case 24: return new(foggySprite, 44, 55, 48, CalendarSystem.Weather.Foggy);
                case 25: return new(sunnyWindySprite, 53, 66, 58, CalendarSystem.Weather.SunnyAndWindy);
                case 26: return new(windySprite, 51, 64, 57, CalendarSystem.Weather.Windy);
                case 27: return new(rainySprite, 45, 57, 49, CalendarSystem.Weather.Rainy);
                case 28: return new(foggySprite, 42, 53, 46, CalendarSystem.Weather.Foggy);
                case 29: return new(sunnyCloudySprite, 52, 65, 57, CalendarSystem.Weather.SunnyAndCloudy);
                case 30: return new(foggySprite, 41, 51, 44, CalendarSystem.Weather.Foggy);
                case 31: return new(cloudySprite, 40, 50, 43, CalendarSystem.Weather.Cloudy);
                default: return new(sunnySprite, 52, 65, 57, CalendarSystem.Weather.Sunny); 
            }
        }

        if (season == CalendarSystem.Season.Winter)
        {
            switch (day)
            {
                case 1: return new(rainySprite, 36, 42, 38, CalendarSystem.Weather.Rainy);
                case 2: return new(rainySprite, 37, 43, 39, CalendarSystem.Weather.Rainy);
                case 3: return new(cloudySprite, 34, 40, 36, CalendarSystem.Weather.Cloudy);
                case 4: return new(foggySprite, 33, 39, 35, CalendarSystem.Weather.Foggy);
                case 5: return new(snowySprite, 25, 30, 28, CalendarSystem.Weather.Snowy);
                case 6: return new(foggySprite, 32, 37, 34, CalendarSystem.Weather.Foggy);
                case 7: return new(foggySprite, 31, 36, 33, CalendarSystem.Weather.Foggy);
                case 8: return new(windySprite, 33, 38, 35, CalendarSystem.Weather.Windy);
                case 9: return new(snowySprite, 23, 28, 26, CalendarSystem.Weather.Snowy);
                case 10: return new(rainySprite, 35, 41, 37, CalendarSystem.Weather.Rainy);
                case 11: return new(cloudySprite, 34, 40, 36, CalendarSystem.Weather.Cloudy);
                case 12: return new(rainySprite, 36, 42, 38, CalendarSystem.Weather.Rainy);
                case 13: return new(cloudySprite, 35, 41, 37, CalendarSystem.Weather.Cloudy);
                case 14: return new(snowySprite, 24, 29, 27, CalendarSystem.Weather.Snowy);
                case 15: return new(thunderstormSprite, 37, 43, 39, CalendarSystem.Weather.Thunderstorm);
                case 16: return new(foggySprite, 33, 38, 35, CalendarSystem.Weather.Foggy);
                case 17: return new(windySprite, 31, 36, 33, CalendarSystem.Weather.Windy);
                case 18: return new(rainySprite, 34, 40, 36, CalendarSystem.Weather.Rainy);
                case 19: return new(snowySprite, 22, 27, 25, CalendarSystem.Weather.Snowy);
                case 20: return new(cloudySprite, 32, 39, 35, CalendarSystem.Weather.Cloudy);
                case 21: return new(foggySprite, 30, 35, 32, CalendarSystem.Weather.Foggy);
                case 22: return new(windySprite, 32, 37, 34, CalendarSystem.Weather.Windy);
                case 23: return new(snowySprite, 21, 26, 24, CalendarSystem.Weather.Snowy);
                case 24: return new(snowySprite, 26, 31, 29, CalendarSystem.Weather.Snowy);
                case 25: return new(snowySprite, 30, 39, 34, CalendarSystem.Weather.Snowy);
                case 26: return new(cloudySprite, 31, 38, 34, CalendarSystem.Weather.Cloudy);
                case 27: return new(foggySprite, 28, 34, 30, CalendarSystem.Weather.Foggy);
                case 28: return new(thunderstormSprite, 34, 40, 36, CalendarSystem.Weather.Thunderstorm);
                case 29: return new(rainySprite, 32, 38, 34, CalendarSystem.Weather.Rainy);
                case 30: return new(cloudySprite, 30, 36, 33, CalendarSystem.Weather.Cloudy);
                case 31: return new(snowySprite, 20, 25, 23, CalendarSystem.Weather.Snowy);
                default: return new(sunnySprite, 33, 40, 36, CalendarSystem.Weather.Sunny); 
            }
        }

        if (season == CalendarSystem.Season.SpringA)
        {
            switch (day)
            {
                case 1: return new(sunnyCloudySprite, 55, 66, 60, CalendarSystem.Weather.SunnyAndCloudy);
                case 2: return new(sunnyCloudySprite, 57, 68, 62, CalendarSystem.Weather.SunnyAndCloudy);
                case 3: return new(rainySprite, 52, 63, 57, CalendarSystem.Weather.Rainy);
                case 4: return new(sunnySprite, 59, 70, 64, CalendarSystem.Weather.Sunny);
                case 5: return new(rainySprite, 54, 65, 58, CalendarSystem.Weather.Rainy);
                case 6: return new(foggySprite, 50, 60, 53, CalendarSystem.Weather.Foggy);
                case 7: return new(sunnySprite, 60, 71, 65, CalendarSystem.Weather.Sunny);
                case 8: return new(cloudySprite, 56, 67, 61, CalendarSystem.Weather.Cloudy);
                case 9: return new(rainySprite, 55, 66, 60, CalendarSystem.Weather.Rainy);
                case 10: return new(sunnyWindySprite, 58, 69, 64, CalendarSystem.Weather.SunnyAndWindy);
                case 11: return new(rainySprite, 53, 64, 58, CalendarSystem.Weather.Rainy);
                case 12: return new(foggySprite, 51, 61, 55, CalendarSystem.Weather.Foggy);
                case 13: return new(rainySprite, 52, 63, 57, CalendarSystem.Weather.Rainy);
                case 14: return new(sunnyCloudySprite, 59, 70, 64, CalendarSystem.Weather.SunnyAndCloudy);
                case 15: return new(sunnySprite, 61, 72, 66, CalendarSystem.Weather.Sunny);
                case 16: return new(sunnySprite, 62, 73, 67, CalendarSystem.Weather.Sunny);
                case 17: return new(rainySprite, 55, 66, 60, CalendarSystem.Weather.Rainy);
                case 18: return new(cloudySprite, 57, 68, 63, CalendarSystem.Weather.Cloudy);
                case 19: return new(sunnyWindySprite, 60, 71, 65, CalendarSystem.Weather.SunnyAndWindy);
                case 20: return new(sunnyCloudySprite, 58, 69, 63, CalendarSystem.Weather.SunnyAndCloudy);
                case 21: return new(rainySprite, 54, 65, 59, CalendarSystem.Weather.Rainy);
                case 22: return new(thunderstormSprite, 53, 64, 58, CalendarSystem.Weather.Thunderstorm);
                case 23: return new(sunnyCloudySprite, 59, 70, 64, CalendarSystem.Weather.SunnyAndCloudy);
                case 24: return new(sunnySprite, 61, 72, 66, CalendarSystem.Weather.Sunny);
                case 25: return new(cloudySprite, 56, 67, 61, CalendarSystem.Weather.Cloudy);
                case 26: return new(rainySprite, 54, 65, 59, CalendarSystem.Weather.Rainy);
                case 27: return new(hailSprite, 49, 59, 53, CalendarSystem.Weather.Hail);
                case 28: return new(sunnyCloudySprite, 57, 68, 62, CalendarSystem.Weather.SunnyAndCloudy);
                case 29: return new(foggySprite, 50, 60, 54, CalendarSystem.Weather.Foggy);
                case 30: return new(sunnySprite, 60, 71, 65, CalendarSystem.Weather.Sunny);
                default: return new(sunnySprite, 59, 70, 64, CalendarSystem.Weather.Sunny); 
            }
        }

        return new(sunnySprite, 55, 65, 59, CalendarSystem.Weather.Sunny); 
    }

    /*public void DisplayPreviousTemperature(CalendarSystem.Season season, int day, int year)
    {
        WeatherData previousWeather = GetWeatherForSeasonDay(season, day, year);
        int temperatureToShow = GetTemperatureForPhase(previousWeather); 

        if (VN_Configuration.activeConfig.useCelsius)
        {
            temperatureToShow = Mathf.RoundToInt((temperatureToShow - 32) * 5f / 9f);
            temperatureText.text = $"{temperatureToShow}°C";
        }
        else
        {
            temperatureText.text = $"{temperatureToShow}°F";
        }

        Debug.Log($"[Previous Weather] Day {day} of {season}: {temperatureToShow}°"); 
    }*/

    /*public void DisplayNextTemperature(CalendarSystem.Season season, int day, int year)
    {
        WeatherData nextWeather = GetWeatherForSeasonDay(season, day, year);
        int temperatureToShow = GetTemperatureForPhase(nextWeather);

        if (VN_Configuration.activeConfig.useCelsius)
        {
            temperatureToShow = Mathf.RoundToInt((temperatureToShow - 32) * 5f / 9f);
            temperatureText.text = $"{temperatureToShow}°C";
        }
        else
        {
            temperatureText.text = $"{temperatureToShow}°F";
        }

        Debug.Log($"[Next Weather] Day {day} of {season}: {temperatureToShow}°");
    }*/ 

    /*public void PrepareNextTemperature(CalendarSystem.Season season, int day, int year)
    {
        WeatherData weatherData = GetWeatherForSeasonDay(season, day, year);

        int temperature = 0;

        if (CalendarSystem.Instance.activityCounter == 0)
            temperature = weatherData.morningTemp;
        else if (CalendarSystem.Instance.activityCounter == 1)
            temperature = weatherData.afternoonTemp;
        else if (CalendarSystem.Instance.activityCounter == 2)
            temperature = weatherData.eveningTemp;

        int displayTemp = VN_Configuration.activeConfig.useCelsius
            ? Mathf.RoundToInt((temperature - 32) * 5f / 9f)
            : temperature;

        nextTemperature = VN_Configuration.activeConfig.useCelsius
            ? $"{displayTemp}°C"
            : $"{displayTemp}°F";
    }*/


    public static int GetTemperatureForPhase(WeatherData data)
    {
        int phase = CalendarSystem.Instance.activityCounter;
        return phase == 0 ? data.morningTemp :
               phase == 1 ? data.afternoonTemp :
               data.eveningTemp;
    }

    // --- Weather/time -> spawn & tips -------------------------------------------
    // Spawn-rate multipliers relative to "Moderate" (1.0).
    // Lower min/max spawn times == faster spawns, so we'll divide by this later.
    private static float RateToMultiplier(string rate)
    {
        switch (rate)
        {
            case "Very Low": return 0.50f;
            case "Low": return 0.75f;
            case "Low–Moderate": return 0.90f;
            case "Moderate": return 1.00f;
            case "High": return 1.25f;
            case "Very High": return 1.50f;
            default: return 1.00f;
        }
    }

    // phase: 0=Morning,1=Afternoon,2=Evening
    public static float GetSpawnRateMultiplier(CalendarSystem.Weather w, int phase)
    {
        string rate = "Moderate";
        switch (w)
        {
            case CalendarSystem.Weather.Sunny:
                rate = (phase == 0) ? "Low–Moderate" : (phase == 1) ? "High" : "Moderate";
                break;

            case CalendarSystem.Weather.Cloudy:
                rate = (phase == 0) ? "Low" : (phase == 1) ? "Moderate" : "Moderate";
                break;

            case CalendarSystem.Weather.SunnyAndCloudy:
                rate = (phase == 0) ? "Moderate" : (phase == 1) ? "High" : "Low–Moderate";
                break;

            case CalendarSystem.Weather.Rainy:
                rate = (phase == 0) ? "High" : (phase == 1) ? "Very High" : "Moderate";
                break;

            case CalendarSystem.Weather.Windy:
                rate = (phase == 0) ? "Low" : (phase == 1) ? "Moderate" : "Low";
                break;

            case CalendarSystem.Weather.Snowy:
                rate = (phase == 0) ? "Very Low" : (phase == 1) ? "Low–Moderate" : "Moderate";
                break;

            case CalendarSystem.Weather.Foggy:
                rate = (phase == 0) ? "Low" : (phase == 1) ? "Moderate" : "Very Low";
                break;

            case CalendarSystem.Weather.Thunderstorm:
                rate = (phase == 0) ? "High" : (phase == 1) ? "Very High" : "High";
                break;

            case CalendarSystem.Weather.Hail:
                rate = (phase == 0) ? "Moderate" : (phase == 1) ? "High" : "Moderate";
                break;

            case CalendarSystem.Weather.SunnyAndWindy:
                rate = (phase == 0) ? "Low" : (phase == 1) ? "Moderate" : "Low–Moderate";
                break;
        }
        return RateToMultiplier(rate);
    }

    // Tip modifier in PERCENT, matching the chart
    public static int GetTipModifierPercent(CalendarSystem.Weather w, int phase)
    {
        switch (w)
        {
            case CalendarSystem.Weather.Sunny:
                return (phase == 0) ? 0 : (phase == 1) ? 5 : 10;

            case CalendarSystem.Weather.Cloudy:
                return (phase == 0) ? 5 : (phase == 1) ? 0 : 10;

            case CalendarSystem.Weather.SunnyAndCloudy:
                return (phase == 0) ? 0 : (phase == 1) ? 5 : 5;

            case CalendarSystem.Weather.Rainy:
                return (phase == 0) ? 10 : (phase == 1) ? 15 : 20;

            case CalendarSystem.Weather.Windy:
                return (phase == 0) ? -10 : (phase == 1) ? -5 : 0;

            case CalendarSystem.Weather.Snowy:
                return (phase == 0) ? 10 : (phase == 1) ? 15 : 20;

            case CalendarSystem.Weather.Foggy:
                return (phase == 0) ? 5 : (phase == 1) ? 10 : 15;

            case CalendarSystem.Weather.Thunderstorm:
                return (phase == 0) ? 15 : (phase == 1) ? 10 : 25;

            case CalendarSystem.Weather.Hail:
                return (phase == 0) ? 10 : (phase == 1) ? 15 : 20;

            case CalendarSystem.Weather.SunnyAndWindy:
                return (phase == 0) ? -5 : (phase == 1) ? 0 : 5;
        }
        return 0;
    }

    // ---------------- Season -> spawn & tips (phase: 0=Morning,1=Afternoon,2=Evening)
    public static float GetSeasonSpawnMultiplier(CalendarSystem.Season s, int phase)
    {
        switch (s)
        {
            case CalendarSystem.Season.Spring:
            case CalendarSystem.Season.SpringA:
                // Slightly busier afternoons, small boost evenings
                return (phase == 0) ? 0.98f : (phase == 1) ? 1.02f : 1.04f;

            case CalendarSystem.Season.Summer:
                // Mornings a little slower, afternoons/evenings slightly faster
                return (phase == 0) ? 0.96f : (phase == 1) ? 1.04f : 1.03f;

            case CalendarSystem.Season.Fall:
                // Fairly balanced, evenings taper off a bit
                return (phase == 0) ? 0.99f : (phase == 1) ? 1.00f : 0.98f;

            case CalendarSystem.Season.Winter:
                // Slightly reduced traffic overall
                return (phase == 0) ? 0.97f : (phase == 1) ? 0.98f : 0.96f;
        }
        return 1f;
    } 

    // Seasonal tip deltas are additive (in percent) to the Weather × Time table
    public static int GetSeasonTipBonusPercent(CalendarSystem.Season s, int phase)
    {
        switch (s)
        {
            case CalendarSystem.Season.Spring:
            case CalendarSystem.Season.SpringA:
                // Light lift for Spring
                return (phase == 0) ? 1 : (phase == 1) ? 2 : 3;

            case CalendarSystem.Season.Summer:
                // Small morning dip, tiny evening lift
                return (phase == 0) ? -2 : (phase == 1) ? 0 : 2;

            case CalendarSystem.Season.Fall:
                // Mild cozy-season boost
                return (phase == 0) ? 2 : (phase == 1) ? 2 : 4;

            case CalendarSystem.Season.Winter:
                // Slight generosity bump in cold weather
                return (phase == 0) ? 3 : (phase == 1) ? 3 : 5;
        }
        return 0;
    }

    // ---------------- Weekday -> spawn & tips (phase: 0=Morning,1=Afternoon,2=Evening)
    // Use CalendarSystem.Instance.GetCurrentDayOfWeek() to feed these.

    // Spawn multiplier: tiny nudges for workdays vs. weekend patterns
    public static float GetWeekdaySpawnMultiplier(string weekday, int phase)
    {
        switch (weekday)
        {
            case "Monday":
                // Slow evening, modest commuter morning
                return (phase == 0) ? 1.02f : (phase == 1) ? 0.99f : 0.97f;

            case "Tuesday":
            case "Wednesday":
                // Steady weekdays
                return (phase == 0) ? 1.01f : (phase == 1) ? 1.00f : 0.99f;

            case "Thursday":
                // Pre-Friday bump in evening
                return (phase == 0) ? 1.00f : (phase == 1) ? 1.01f : 1.02f;

            case "Friday":
                // Lunch pop + stronger evening
                return (phase == 0) ? 1.00f : (phase == 1) ? 1.02f : 1.04f;

            case "Saturday":
                // Leisure crowd: bigger afternoon/evening
                return (phase == 0) ? 1.00f : (phase == 1) ? 1.05f : 1.05f;

            case "Sunday":
                // Brunchy morning + chill evening
                return (phase == 0) ? 1.04f : (phase == 1) ? 1.02f : 0.98f;
        }
        return 1f;
    }

    // Tip bonus in PERCENT: tiny, additive on top of Weather% + Season%
    public static int GetWeekdayTipBonusPercent(string weekday, int phase)
    {
        switch (weekday)
        {
            case "Monday":
                return (phase == 0) ? -1 : (phase == 1) ? 0 : 0;

            case "Tuesday":
            case "Wednesday":
                return (phase == 0) ? 0 : (phase == 1) ? 0 : 1;

            case "Thursday":
                return (phase == 0) ? 0 : (phase == 1) ? 1 : 2;

            case "Friday":
                return (phase == 0) ? 0 : (phase == 1) ? 2 : 3;

            case "Saturday":
                return (phase == 0) ? 1 : (phase == 1) ? 2 : 3;

            case "Sunday":
                return (phase == 0) ? 2 : (phase == 1) ? 1 : 0;
        }
        return 0;
    } 

    /*public void ApplyNextTemperature(TextMeshProUGUI temperatureText)
    {
        currentTemperature = nextTemperature;
        if (temperatureText != null)
            temperatureText.text = currentTemperature;
    }*/
}
