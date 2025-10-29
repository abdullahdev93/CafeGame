using System.Collections;
using UnityEngine;
using TMPro;

public class FakeCalendarSlide : MonoBehaviour
{
    public static FakeCalendarSlide Instance;

    public GameObject panel; // Reference to the panel GameObject
    [SerializeField] private RectTransform movingObject; // Reference to the moving object inside the panel
    [SerializeField] private TextMeshProUGUI dateText; // Reference to the TextMeshProUGUI for the date display
    [SerializeField] private float moveDistance = 105f; // Distance to move the object each time the panel appears 
    [SerializeField] private float moveLongerDistance = 315f; // Distance to move the object each time the panel appears
    [SerializeField] private float panelDuration = 3f; // Time in seconds before the panel disappears
    [SerializeField] private float moveSpeed = 200f; // Speed at which the object moves to the left
    [SerializeField] private float dateChangeDelay = 0.5f; // Delay before showing the new date 
    public TextMeshProUGUI seasonDayMockText;
    public TextMeshProUGUI timeOfDayMockText;
    public TextMeshProUGUI dayOfWeekMockText;
    public TextMeshProUGUI temperatureMockText;

    private Vector2 targetPosition;
    [SerializeField] private int currentDay;

    // Default starting position
    private Vector2 defaultPosition = new Vector2(3123f, -310f);

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
    }

    void Start()
    {
        // Load the saved position or use the default position
        float savedX = PlayerPrefs.GetFloat("MovingObjectX", defaultPosition.x);
        float savedY = PlayerPrefs.GetFloat("MovingObjectY", defaultPosition.y);
        targetPosition = new Vector2(savedX, savedY);
        movingObject.anchoredPosition = targetPosition;

        // Load the current day or start from 10 if it doesn't exist
        currentDay = PlayerPrefs.GetInt("CurrentFakeDay", 6);
        UpdateDateText();

        panel.SetActive(false); // Ensure the panel is hidden initially

        UpdateTemperatureDisplay();

        //ResetPositionIfNeeded(); 
    }

    void Update()
    {
        //ResetPositionIfNeeded(); 

        // Check if the "R" key is pressed to reset PlayerPrefs
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!VNMenuManager.instance.developerMode)
                return; 

            ResetPlayerPrefs();
        }
    }

    public void ResetPositionIfNeeded()
    {
        if (CalendarSystem.Instance != null &&
            (CalendarSystem.Instance.GetCurrentSeason() != CalendarSystem.Season.Spring &&
            CalendarSystem.Instance.GetCurrentDay() < 16))
        {
            ResetPlayerPrefs();

            // Reset to the default position
            targetPosition = defaultPosition;
            movingObject.anchoredPosition = defaultPosition;

            // Save the reset position
            PlayerPrefs.SetFloat("MovingObjectX", defaultPosition.x);
            PlayerPrefs.SetFloat("MovingObjectY", defaultPosition.y);
            PlayerPrefs.Save();
        }
    }

    public void FakeCalendarInfo(string fakeSeasonDay, string fakeTimeOfDay, string fakeWeekDay, string fakeTemperature)
    {
        seasonDayMockText.text = fakeSeasonDay;
        timeOfDayMockText.text = fakeTimeOfDay;
        dayOfWeekMockText.text = fakeWeekDay;

        PlayerPrefs.SetString("MockRawTemperature", fakeTemperature);
        UpdateTemperatureDisplay();

        //temperatureMockText.text = fakeTemperature; 
    }

    public void UpdateTemperatureDisplay()
    {
        string rawTemp = PlayerPrefs.GetString("MockRawTemperature", "67°F");

        int temperatureValue;
        if (int.TryParse(rawTemp.Replace("°F", "").Replace("°C", ""), out temperatureValue))
        {
            if (VN_Configuration.activeConfig.useCelsius)
            {
                int celsius = Mathf.RoundToInt((temperatureValue - 32) * 5f / 9f);
                temperatureMockText.text = celsius + "°C";
            }
            else
            {
                temperatureMockText.text = temperatureValue + "°F";
            }
        }
        else
        {
            temperatureMockText.text = rawTemp;
        }
    }

    public void ShowPanelAndMove(float moveDistance)
    {
        panel.SetActive(true); // Show the panel

        // Calculate new target position to the left by the move distance
        targetPosition += Vector2.left * moveDistance;

        // Start the sliding, date change, and panel hide coroutine
        StartCoroutine(MoveObjectAndChangeDate());
    }

    private IEnumerator MoveObjectAndChangeDate()
    {
        // Handle the special case for 13th, displaying 12th first, then 15th
        if (currentDay == 12)
        {
            // Show the 12th as the previous date
            dateText.text = $"Spring 12, 2019";
            yield return new WaitForSeconds(dateChangeDelay);

            // Move to the 15th instead of the 13th
            currentDay = 15;
        }
        else
        {
            // Save the previous day to display it briefly
            string previousDate = $"Spring {currentDay}, 2019";
            if (currentDay <= 9)
                previousDate = $"Spring 0{currentDay}, 2019";

            // Display the previous date momentarily
            dateText.text = previousDate;
            yield return new WaitForSeconds(dateChangeDelay);

            // Increment the day and save it
            currentDay++;
        }

        PlayerPrefs.SetInt("CurrentFakeDay", currentDay);
        PlayerPrefs.Save();

        // Smoothly move the object to the target position
        while (Vector2.Distance(movingObject.anchoredPosition, targetPosition) > 0.1f)
        {
            movingObject.anchoredPosition = Vector2.MoveTowards(movingObject.anchoredPosition, targetPosition, moveSpeed * Time.deltaTime);
            yield return null; // Wait for the next frame
        }

        // Update to the new date after the delay
        UpdateDateText();
        UpdateTemperatureDisplay();

        // Save the new position in PlayerPrefs
        PlayerPrefs.SetFloat("MovingObjectX", movingObject.anchoredPosition.x);
        PlayerPrefs.SetFloat("MovingObjectY", movingObject.anchoredPosition.y);
        PlayerPrefs.Save();

        // Wait for the specified duration before hiding the panel
        yield return new WaitForSeconds(panelDuration);

        // Hide the panel
        panel.SetActive(false);
    }

    private void UpdateDateText()
    {
        // Check if the current day is 13 or about to be 13
        if (currentDay == 13)
        {
            // Display it as the 15th instead
            dateText.text = $"Spring 15, 2019";
            currentDay = 15;
        }
        else
        {
            // Format the day with a leading zero if it's a single-digit
            string formattedDay = currentDay.ToString("D2");

            // Update the date text to reflect the current season and day
            dateText.text = $"Spring {formattedDay}, 2019";
        }
    }

    public void DisplayFakeDate()
    {
        // Update the date text to reflect the current season and day
        dateText.text = $"Spring 15, 2019";
    }

    private void ResetPlayerPrefs()
    {
        // Reset position and day PlayerPrefs
        PlayerPrefs.DeleteKey("MovingObjectX");
        PlayerPrefs.DeleteKey("MovingObjectY");
        PlayerPrefs.DeleteKey("CurrentFakeDay");
        PlayerPrefs.DeleteKey("MockRawTemperature");
        PlayerPrefs.Save();

        // Reset the current day and target position to default values
        currentDay = 6;
        targetPosition = defaultPosition;

        // Reset the moving object to the default position immediately
        movingObject.anchoredPosition = defaultPosition;

        // Update the displayed date to the reset day
        UpdateDateText();

        Debug.Log("PlayerPrefs reset and date reset to Spring 06, 2019.");
    }
}