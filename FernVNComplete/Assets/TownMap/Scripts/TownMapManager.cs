using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class TownMapManager : MenuPage
{
    public TextMeshProUGUI earningsText;
    public TextMeshProUGUI seasonDayText;
    public TextMeshProUGUI timeOfDayText; // Text component for displaying the current activity time 
    public TextMeshProUGUI dayOfWeekText;
    public TextMeshProUGUI temperatureText; 
    public TextMeshProUGUI transferMessageText; // Text component for displaying the transfer confirmation message
    public Animator transferMessageAnimator; // Animator component for the transferMessageText 
    public Animator errorMessageAnimator; // Animator for error message 

    public TextMeshProUGUI errorMessageText; // TextMeshProUGUI for displaying error messages
    public float errorMessageDisplayDuration = 2f; // Duration to display error message 

    public Image weatherImage; // Image component for displaying the weather icon
    public Sprite sunnySprite;
    public Sprite cloudySprite;
    public Sprite sunnyCloudySprite;
    public Sprite rainySprite;
    public Sprite foggySprite;
    public Sprite thunderstormSprite;

    public Button toCafeButton;
    public Button toApartmentButton; 

    public static TownMapManager instance { get; private set; }
    private VN_Configuration config => VN_Configuration.activeConfig;

    //private float earnings;

    private float earnings => PlayerStats.GetMoney(); 

    private bool hasMetNina; // Boolean to check if the player already met Nina

    private AudioSource iconSelectSound;  

    private void Start()
    {
        // Load the shared TotalMoney from PlayerPrefs, which includes earnings from the Cafe
        //earnings = PlayerPrefs.GetFloat("TotalMoney", 1000); // Default to $1000 if not found
        //earnings = PlayerStats.GetMoney();
        UpdateEarningsText();  
        //earnings = PlayerPrefs.GetFloat("TownMapTotalMoney", 1000); // Default to $1000 if not found
        Debug.Log($"Loaded TownMapTotalMoney: {earnings}");

        // Initialize hasMetNina from PlayerPrefs
        hasMetNina = PlayerPrefs.GetInt("HasMetNina", 0) == 1; 

        transferMessageText.gameObject.SetActive(false);

        errorMessageText.gameObject.SetActive(false); 

        //if (PlayerPrefs.GetString("LastScene", "") == "Cafe")
        //{
        //float cafeMoney = PlayerPrefs.GetFloat("CafeTotalMoney", 0); // Default to 0 if not found
        //Debug.Log($"CafeTotalMoney retrieved: {cafeMoney}");

        //if (cafeMoney > 0)
        //{
        //earnings = cafeMoney; // Add cafe earnings to current money

        /*if (transferMessageText != null)
        {
            transferMessageText.text = $"+ ${cafeMoney:F2}";
            StartCoroutine(HideTransferMessageAfterDelay(cafeMoney)); // Hide after 3 seconds and play animation
        }*/

        //PlayerPrefs.SetFloat("CafeTotalMoney", 0);
        //PlayerPrefs.Save();
        //Debug.Log("CafeTotalMoney reset to 0 after transfer.");
        //}
        //}

        UpdateEarningsText();
        UpdateSeasonDayText();
        UpdateWeatherIcon(); // Update the weather icon when the TownMap scene is loaded  

        toApartmentButton.gameObject.SetActive(false);
        toCafeButton.gameObject.SetActive(false);

        iconSelectSound = GameObject.Find("IconSelectSound").GetComponent<AudioSource>();   
    }

    private void Update()
    {
        // Check if the "M" key is pressed
        //if (Input.GetKeyDown(KeyCode.M)) 
        //{
            //ResetTotalMoney();
        //}

        // Check if the "R" key is pressed
        //if (Input.GetKeyDown(KeyCode.R))
        //{
            //ResetHasMetNina();
        //} 

        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(HideTransferMessageAfterDelay(0));
        }*/
    }

    private void ResetTotalMoney()
    {
        PlayerStats.SetMoney(1000f); 

        // Save the updated earnings to PlayerPrefs
        PlayerPrefs.SetFloat("TotalMoney", earnings);
        PlayerPrefs.Save();

        // Update the earnings text on the UI
        UpdateEarningsText();

        Debug.Log("Total money has been reset to 10000.");
    }

    // Method to reset HasMetNina PlayerPrefs
    private void ResetHasMetNina()
    {
        PlayerPrefs.SetInt("HasMetNina", 0);
        PlayerPrefs.Save();
        hasMetNina = false; // Reset the local variable
        Debug.Log("HasMetNina has been reset to 0.");
    }

    /*public void BackToPhoneMenu()
    {
        PlayerPrefs.SetString("LastScene", "TownMap");
        PlayerPrefs.Save();
        SceneManager.LoadScene("VisualNovel");
    }*/

    public void BackToPhoneMenu()
    {
        // Ensure "LastScene" is correctly saved
        PlayerPrefs.SetString("LastScene", "TownMap");

        // Retrieve the previous StartingFile
        string previousStartingFile = PlayerPrefs.GetString("StartingFile", "ApartmentFile"); 

        // Ensure it is saved again before transitioning
        PlayerPrefs.SetString("StartingFile", previousStartingFile);
        PlayerPrefs.Save();

        //VariableStore.TrySetValue("Scene.LastScene", "TownMap");

        //string previousStartingFile = "ApartmentFile";
        //if (VariableStore.TryGetValue("Scene.StartingFile", out object file)) 
            //previousStartingFile = (string)file;

        //VariableStore.TrySetValue("Scene.StartingFile", previousStartingFile);

        Debug.Log($"Returning to VisualNovel with StartingFile: {previousStartingFile}");

        StartCoroutine(LoadVisualNovelSceneAsync()); // Use Coroutine for async loading

        // Load the VisualNovel scene
        //SceneManager.LoadScene("VisualNovel");
    }

    private IEnumerator LoadVisualNovelSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("VisualNovel");
        asyncLoad.allowSceneActivation = false; // Prevent auto-switch until fully loaded

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f) // Scene is fully loaded
            {
                asyncLoad.allowSceneActivation = true; // Activate scene
            }
            yield return null;
        }
    } 

    /*private void PlayTransferMessageAnimation()
    {
        if (transferMessageAnimator != null)
        {
            transferMessageAnimator.SetTrigger("AddCafeMoney");
            Debug.Log("Playing transfer message animation by pressing Space.");
        }
    }*/

    /*private IEnumerator HideTransferMessageAfterDelay(float cafeMoney)
    {
        Debug.Log("Coroutine started, waiting for 3 seconds.");
        transferMessageText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);

        PlayTransferMessageAnimation();

        yield return new WaitForSecondsRealtime(1.5f);

        if (cafeMoney > 0)
        {
            earnings += cafeMoney;
            Debug.Log($"Updated TownMapTotalMoney after adding Cafe earnings: {earnings}");

            // Save the updated earnings to PlayerPrefs
            PlayerPrefs.SetFloat("TotalMoney", earnings);
            PlayerPrefs.Save(); 

            //SaveEarnings();
            UpdateEarningsText();
        }

        transferMessageText.gameObject.SetActive(false);
        Debug.Log("transferMessageText is now hidden.");
    }*/

    public void UpdateSeasonDayText()
    {
        if (CalendarSystem.Instance != null)
        {
            CalendarSystem.Season currentSeason = CalendarSystem.Instance.GetCurrentSeason();
            int currentDay = CalendarSystem.Instance.GetCurrentDay();
            string currentWeekDay = CalendarSystem.Instance.GetCurrentDayOfWeek();  
            string seasonName = currentSeason.ToString();
            int tempValue = CalendarSystem.Instance.GetTemperatureForToday();
            Debug.Log($"Current temperature from CalendarSystem: {tempValue}"); 

            if (seasonDayText != null)
            {
                seasonDayText.text = $"{seasonName} / {currentDay:D2}";
            }

            if (timeOfDayText != null)
            {
                int activityCounter = CalendarSystem.Instance.activityCounter;
                timeOfDayText.text = CalendarSystem.Instance.activityPhases[activityCounter];
            } 

            if (dayOfWeekText != null)
            {
                dayOfWeekText.text = $"{currentWeekDay}"; 
            } 

            if (temperatureText != null)
            {
                //temperatureText.text = $"{temValue}°F"; 
                temperatureText.text = FormatTemperature(tempValue); 
            }
        }
    }

    public void UpdateWeatherIcon()
    {
        CalendarSystem.Weather currentWeather = CalendarSystem.Instance.GetCurrentWeather();

        switch (currentWeather)
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
            case CalendarSystem.Weather.Foggy:
                weatherImage.sprite = sunnySprite;
                break;
            case CalendarSystem.Weather.Thunderstorm: 
                weatherImage.sprite = cloudySprite;
                break;
        }
    }

    public void OnParkIconClicked(float cost) 
    {
        iconSelectSound.PlayOneShot(iconSelectSound.clip);  
        HandleLocationIconClick(cost, "ParkFile", "Can't Afford To Go To The Park!"); 
    }

    public void OnMallIconClicked(float cost)
    {
        iconSelectSound.PlayOneShot(iconSelectSound.clip); 
        HandleLocationIconClick(cost, "MallFile", "Can't Afford To Go To The Mall!"); 
    }

    public void OnLibraryIconClicked(float cost)
    {
        iconSelectSound.PlayOneShot(iconSelectSound.clip); 
        HandleLocationIconClick(cost, "LibraryFile", "Can't Afford To Go To The Library!");
    }

    public void OnConvienceStoreIconClicked(float cost)
    {
        iconSelectSound.PlayOneShot(iconSelectSound.clip); 
        HandleLocationIconClick(cost, "ConvienceStoreFile", "Can't Afford To Go To The ConvienceStore!"); 
    }

    public void OnBarIconClicked(float cost)
    {
        iconSelectSound.PlayOneShot(iconSelectSound.clip); 

        // Determine if it's evening
        bool isEvening = CalendarSystem.Instance.activityCounter == 2; // Assuming activityCounter == 2 represents Evening 
        int barEveningVisitCount = PlayerPrefs.GetInt("BarEveningVisitCount", 0); 
        
        if (isEvening && !hasMetNina)      
        {
            barEveningVisitCount++;
            PlayerPrefs.SetInt("BarEveningVisitCount", barEveningVisitCount);

            if (barEveningVisitCount >= 3)
            {
                HandleLocationIconClick(cost, "BarEveningFile", "Can't Afford To Go To The Bar!");
                hasMetNina = true; // Set the player as having met Nina
                PlayerPrefs.SetInt("HasMetNina", 1);
                PlayerPrefs.Save(); 
            } 
        }

        else
            HandleLocationIconClick(cost, "BarFile", "Can't Afford To Go To The Bar!"); 
    }

    public void OnMoviesIconClicked(float cost)
    {
        iconSelectSound.PlayOneShot(iconSelectSound.clip); 

        // Get the current season and day from CalendarSystem
        CalendarSystem.Season currentSeason = CalendarSystem.Instance.GetCurrentSeason();
        int currentDay = CalendarSystem.Instance.GetCurrentDay();

        string movieFileToLoad = "MoviesFile"; // Default file

        // Check for specific conditions to load different movie files
        if (currentSeason == CalendarSystem.Season.Spring)
        {
            //if (currentDay >= 8 && currentDay <= 14)
            //{
                //movieFileToLoad = "MoviesFile";
            //}
            if (currentDay >= 15 && currentDay <= 21)
            {
                //movieFileToLoad = "MoviesFileTwo";
                movieFileToLoad = "MoviesFile";
            }
            else if (currentDay >= 22 && currentDay <= 28)
            {
                //movieFileToLoad = "MoviesFileThree";
                movieFileToLoad = "MoviesFileTwo";
            }
            else if (currentDay >= 29 || currentDay <= 6 && currentSeason == CalendarSystem.Season.Summer)
            {
                //movieFileToLoad = "MoviesFileFour"; // For Spring 29 - Summer 06 
                movieFileToLoad = "MoviesFileThree"; 
            }
        }

        HandleLocationIconClick(cost, movieFileToLoad, "Can't Afford To Go To The Movies!");
    } 

    public void OnBowlingAlleyIconClicked(float cost)
    {
        iconSelectSound.PlayOneShot(iconSelectSound.clip); 
        HandleLocationIconClick(cost, "BowlingAlleyFile", "Can't Afford To Go To The Bowling Alley!");
    }

    public void OnArcadeIconClicked(float cost)
    {
        iconSelectSound.PlayOneShot(iconSelectSound.clip); 
        HandleLocationIconClick(cost, "ArcadeFile", "Can't Afford To Go To The Arcade!");
    }

    public void OnUniversityIconClicked(float cost)
    {
        iconSelectSound.PlayOneShot(iconSelectSound.clip); 
        HandleLocationIconClick(cost, "UniversityFile", "Can't Afford To Go To The University!"); 
    }

    // Reusable method to handle different locations
    private void HandleLocationIconClick(float cost, string locationFile, string errorMessage)
    {
        if (earnings >= cost)
        {
            PlayerStats.SetMoney(PlayerStats.GetMoney() - cost);
            UpdateEarningsText(); 

            // Set the last scene to "TownMap"
            //PlayerPrefs.SetString("LastScene", "TownMap");

            // Reset consecutive game counts and exhaustion
            PlayerPrefs.SetInt("ConsecutiveGameCount", 0);
            PlayerPrefs.SetInt("ExhaustionPlayCount", 0);
            PlayerPrefs.SetInt("IsExhausted", 0); 
            PlayerPrefs.Save();

            // Set the starting file based on the location
            PlayerPrefs.SetString("StartingFile", locationFile);
            PlayerPrefs.Save();

            //VariableStore.TrySetValue("Scene.LastScene", "TownMap");
            //VariableStore.TrySetValue("Scene.StartingFile", locationFile);


            // Transfer to the VisualNovel scene
            SceneManager.LoadScene("VisualNovel"); 
        }
        else
        {
            ShowErrorMessage(errorMessage); 
            Debug.Log($"Not enough money to enter {locationFile.Replace("File", "")}.");
        }
    }

    private void ShowErrorMessage(string message)
    {
        if (errorMessageText != null && errorMessageAnimator != null)
        {
            // Set the error message text
            errorMessageText.text = message;

            // Show the error message object before the animation starts
            errorMessageText.gameObject.SetActive(true);

            // Trigger the animation to show the error message
            errorMessageAnimator.SetTrigger("PlayTMErrorM"); 

            // Start a coroutine to hide the message after the animation
            StartCoroutine(HideErrorMessageAfterAnimation());
        }
    }

    private IEnumerator HideErrorMessageAfterAnimation()
    {
        // Get the AnimatorStateInfo for the current animation state
        AnimatorStateInfo animationState = errorMessageAnimator.GetCurrentAnimatorStateInfo(0);

        // Wait for the length of the animation to complete before hiding the message
        yield return new WaitForSeconds(animationState.length);

        // Hide the error message after the animation is finished
        errorMessageText.gameObject.SetActive(false);
    }

    public void HoverOverButtons()
    {
        toCafeButton.gameObject.SetActive(true);
        toApartmentButton.gameObject.SetActive(true);
    }

    public void HoverAwayFromButtons()
    {
        toCafeButton.gameObject.SetActive(false);
        toApartmentButton.gameObject.SetActive(false);
    }

    /*public void OnParkIconClicked(float cost)
    {
        if (earnings >= cost)
        {
            earnings -= cost;

            // Save the updated earnings to PlayerPrefs
            PlayerPrefs.SetFloat("TotalMoney", earnings);
            PlayerPrefs.Save();

            // Reset consecutive game counts and exhaustion
            PlayerPrefs.SetInt("ConsecutiveGameCount", 0);
            PlayerPrefs.SetInt("ExhaustionPlayCount", 0);
            PlayerPrefs.SetInt("IsExhausted", 0);
            PlayerPrefs.Save(); 

            // Set the last scene to "TownMap"
            //PlayerPrefs.SetString("LastScene", "TownMap");

            // Set the starting file to ParkFile
            PlayerPrefs.SetString("StartingFile", "ParkFile");
            PlayerPrefs.Save();

            // Transfer to the VisualNovel scene
            SceneManager.LoadScene("VisualNovel");
        }
        else
        {
            Debug.Log("Not enough money to enter the park.");
        }
    }*/

    /*public void OnMallIconClicked(float cost)
    {
        if (earnings >= cost)
        {
            earnings -= cost;

            // Save the updated earnings to PlayerPrefs
            PlayerPrefs.SetFloat("TotalMoney", earnings);
            PlayerPrefs.Save();

            // Set the last scene to "TownMap"
            //PlayerPrefs.SetString("LastScene", "TownMap");

            // Set the starting file to MallFile
            PlayerPrefs.SetString("StartingFile", "MallFile");
            PlayerPrefs.Save();

            // Transfer to the VisualNovel scene
            SceneManager.LoadScene("VisualNovel");
        }
        else
        {
            Debug.Log("Not enough money to enter the mall.");
        }
    }*/

    /*public void OnLocationIconClicked(float cost)
    {
        if (earnings >= cost)
        {
            earnings -= cost;

            // Save the updated earnings to PlayerPrefs
            PlayerPrefs.SetFloat("TotalMoney", earnings);
            PlayerPrefs.Save();

            // Reset consecutive game counts and exhaustion
            PlayerPrefs.SetInt("ConsecutiveGameCount", 0);
            PlayerPrefs.SetInt("ExhaustionPlayCount", 0);
            PlayerPrefs.SetInt("IsExhausted", 0);
            PlayerPrefs.Save();

            //SaveEarnings();
            UpdateEarningsText();
            //GameSession.Instance.consecutiveGameCount = 0;
            //GameSession.Instance.exhaustionPlayCount = 0; 
        }
        else
        {
            Debug.Log("Not enough money to enter this location.");
        }
    }*/

    public void TravelToCafe()
    {
        iconSelectSound.PlayOneShot(iconSelectSound.clip); 
        PlayerPrefs.SetString("LastScene", "TownMap");
        SceneManager.LoadScene("Cafe");
    } 

    public void TravelToApartment()
    {
        iconSelectSound.PlayOneShot(iconSelectSound.clip); 
        PlayerPrefs.SetString("StartingFile", "ApartmentFile");
        PlayerPrefs.Save();

        //VariableStore.TrySetValue("Scene.StartingFile", "ApartmentFile"); 

        PlayerPrefs.SetString("LastScene", "TownMap");
        SceneManager.LoadScene("VisualNovel"); 
    } 

    public void ClickOnApartment()
    {
        iconSelectSound.PlayOneShot(iconSelectSound.clip); 
        toCafeButton.gameObject.SetActive(true);
        toApartmentButton.gameObject.SetActive(true);
    }

    //private void UpdateEarningsText()
    //{
        //earningsText.text = $"Earnings: ${earnings:N2}";
    //}

    private void UpdateEarningsText()
    {
        float updatedEarnings = PlayerStats.GetMoney();
        earningsText.text = $"Earnings: ${updatedEarnings:N2}"; 
    } 

    public void RefreshSeasonDayText()
    {
        UpdateSeasonDayText();
        UpdateWeatherIcon(); // Ensure the weather icon is updated when refreshing the season/day text
    }

    private void SaveEarnings()
    {
        PlayerPrefs.SetFloat("TownMapTotalMoney", earnings);
        PlayerPrefs.SetFloat("CafeTotalMoney", earnings); // Sync Cafe and TownMap earnings 
        PlayerPrefs.Save();
    }

    private string FormatTemperature(int tempFahrenheit)
    {
        if (config.useCelsius)
        {
            int celsius = Mathf.RoundToInt((tempFahrenheit - 32) * 5f / 9f);
            return $"{celsius}°C";
        } 
        else 
            return $"{tempFahrenheit}°F";
    } 
}
