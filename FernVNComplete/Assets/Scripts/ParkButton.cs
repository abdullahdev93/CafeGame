using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ParkButton : MonoBehaviour 
{
    [TextArea(3, 10)]
    [SerializeField] private string buttonDetails;

    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private GameObject displayTextBox;
    //[SerializeField] private CalendarSystem.Season requiredSeason;
    [SerializeField] private CalendarSystem.Season[] requiredSeasons; 
    //[SerializeField] private string requiredActivityPhase;
    [SerializeField] private string[] requiredActivityPhases;
    [SerializeField] private CalendarSystem.Weather[] requiredWeatherConditions;
    [SerializeField] private string[] requiredDaysOfWeek; 
    //[SerializeField] private float parkActivityPrice;

    private Image parkActivityImage;

    // Reference to the ShopItemNavigator
    private ParkActivityNavigator parkActivityNavigator;

    // Reference to the ShopItemNavigator
    private FriendInviteNavigator friendInviteNavigator;

    private int activityIndex;

    private void Start()
    {
        if (displayText == null)
        {
            Debug.LogError("Display Text is not assigned! Please assign a Text object in the Inspector.");
        }

        displayTextBox.SetActive(false);
        parkActivityImage = GetComponent<Image>(); // Get the Image component 

        // Find the ShopItemNavigator in the scene
        parkActivityNavigator = FindObjectOfType<ParkActivityNavigator>();

        friendInviteNavigator = FindObjectOfType<FriendInviteNavigator>(); 

        // Get the index of this ShopItem in the ShopItemNavigator
        if (parkActivityNavigator != null)
        {
            activityIndex = parkActivityNavigator.GetShopItemIndex(gameObject);
        }

        // Get the index of this ShopItem in the ShopItemNavigator
        if (friendInviteNavigator != null)
        {
            activityIndex = friendInviteNavigator.GetShopItemIndex(gameObject);
        }

        parkActivityNavigator.ResetAllParkActivities();

        friendInviteNavigator.ResetFriendInvites(); 
    }

    public void OnHoverEnter()
    {
        if (displayText != null)
        {
            displayText.text = buttonDetails;
            displayTextBox.SetActive(true);

            // Reset all other ShopItems to blue
            if (parkActivityNavigator != null)
            {
                parkActivityNavigator.ResetAllParkActivities();
            }

            // Set this ShopItem's color to black
            if (parkActivityImage != null)
            {
                parkActivityImage.color = Color.black;
            }

            // Update the currentIndex in the ShopItemNavigator
            if (parkActivityNavigator != null)
            {
                parkActivityNavigator.SetCurrentIndex(activityIndex);
            }
        }
    }

    public void OnHoverExit()
    {
        if (displayText != null)
        {
            displayText.text = ""; // Clear the text
            displayTextBox.SetActive(false);

            if (parkActivityImage != null)
            {
                //barItemImage.color = Color.blue; // Reset color to blue on hover exit 
                //parkActivityImage.color = new Color(0.5f, 0f, 0.5f); // RGB: 128, 0, 128
                parkActivityImage.color = new Color(1f, 0.41f, 0.71f); // RGB: 255, 105, 180 (Hot Pink) 

            }
        }
    }

    public void OnPurchaseButtonClick(string ToFile)
    {
        Debug.Log("Activity Button Clicked: " + ToFile); // Debugging

        //Update ActivityTracker before handling scene transition
        ActivityTracker.Instance.SetActivity(ToFile);

        string currentActivity = ActivityTracker.Instance.GetSelectedActivity(); // Retrieve updated activity
        Debug.Log("Updated Activity in ActivityTracker: " + currentActivity); // Debugging

        string currentActivityPhase = CalendarSystem.Instance.activityPhases[CalendarSystem.Instance.activityCounter];
        CalendarSystem.Season currentSeason = CalendarSystem.Instance.GetCurrentSeason();

        if (ToFile == "Jog" && currentSeason == CalendarSystem.Season.Spring && currentActivityPhase == "Morning")
        {
            HandleHangOutClick("JogMorningSpring");
        }
        else if (ToFile == "Jog" && currentSeason == CalendarSystem.Season.Spring && currentActivityPhase == "Afternoon")
        {
            HandleHangOutClick("JogAfternoonSpring");
        }
        else if (ToFile == "Jog" && currentSeason == CalendarSystem.Season.Spring && currentActivityPhase == "Evening")
        {
            HandleHangOutClick("JogEveningSpring");
        } 
    } 

    public string GetButtonDetails()
    {
        return buttonDetails;
    }

    public bool IsAvailable(CalendarSystem.Season currentSeason, string currentPhase, CalendarSystem.Weather currentWeather, string currentDayOfWeek) 
    {
        //return currentSeason == requiredSeason && System.Array.Exists(requiredActivityPhases, phase => phase == currentPhase);
        //return System.Array.Exists(requiredSeasons, season => season == currentSeason) && System.Array.Exists(requiredActivityPhases, phase => phase == currentPhase);
        return System.Array.Exists(requiredSeasons, season => season == currentSeason) &&
               System.Array.Exists(requiredActivityPhases, phase => phase == currentPhase) &&
               System.Array.Exists(requiredWeatherConditions, weather => weather == currentWeather) &&
               System.Array.Exists(requiredDaysOfWeek, day => day == currentDayOfWeek); 
    } 

    private void HandleHangOutClick(string locationFile)
    {
        // Set the starting file based on the location
        PlayerPrefs.SetString("StartingFile", locationFile);
        PlayerPrefs.Save();

        // Transfer to the VisualNovel scene
        SceneManager.LoadScene("VisualNovel");
    }
}
