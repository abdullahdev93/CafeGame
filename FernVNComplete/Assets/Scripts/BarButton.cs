/*using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BarButton : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string buttonDetails;

    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private GameObject displayTextBox;
    [SerializeField] private float barItemPrice;

    private Image barItemImage;

    // Reference to the ShopItemNavigator
    private BarItemNavigator barItemNavigator;

    private int itemIndex;

    private void Start()
    {
        if (displayText == null)
        {
            Debug.LogError("Display Text is not assigned! Please assign a Text object in the Inspector.");
        }

        displayTextBox.SetActive(false);
        barItemImage = GetComponent<Image>(); // Get the Image component

        // Find the ShopItemNavigator in the scene
        barItemNavigator = FindObjectOfType<BarItemNavigator>();

        // Get the index of this ShopItem in the ShopItemNavigator
        if (barItemNavigator != null)
        {
            itemIndex = barItemNavigator.GetShopItemIndex(gameObject);
        }

        barItemNavigator.ResetAllShopItems();
    }

    public void OnHoverEnter()
    {
        if (displayText != null)
        {
            displayText.text = buttonDetails;
            displayTextBox.SetActive(true);

            // Reset all other ShopItems to blue
            if (barItemNavigator != null)
            {
                barItemNavigator.ResetAllShopItems();
            }

            // Set this ShopItem's color to black
            if (barItemImage != null)
            {
                barItemImage.color = Color.black;
            }

            // Update the currentIndex in the ShopItemNavigator
            if (barItemNavigator != null)
            {
                barItemNavigator.SetCurrentIndex(itemIndex);
            }
        }
    }

    public void OnHoverExit()
    {
        if (displayText != null)
        {
            displayText.text = ""; // Clear the text
            displayTextBox.SetActive(false);

            if (barItemImage != null)
            {
                //barItemImage.color = Color.blue; // Reset color to blue on hover exit 
                barItemImage.color = new Color(0.5f, 0f, 0.5f); // RGB: 128, 0, 128   
            }
        }
    }

    public void OnPurchaseButtonClick(string ToFile)
    {
        if (barItemNavigator != null)
        {
            if (barItemNavigator.DeductMoney(barItemPrice)) // Deduct money if sufficient funds
            {
                Debug.Log($"Item purchased for ${barItemPrice}.");
                //SceneManager.LoadScene(ToScene); 
                HandleHangOutClick(ToFile); 
            }
            else
            {
                Debug.Log("Insufficient funds!");
            }
        }
    }

    public string GetButtonDetails()
    {
        return buttonDetails;
    }

    private void HandleHangOutClick(string locationFile)
    {
        // Set the starting file based on the location
        PlayerPrefs.SetString("StartingFile", locationFile);
        PlayerPrefs.Save();

        // Transfer to the VisualNovel scene
        SceneManager.LoadScene("VisualNovel");
    }
}*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BarButton : MonoBehaviour
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

    private Image barDrinkImage;

    // Reference to the ShopItemNavigator
    private BarItemNavigator barItemNavigator;

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
        barDrinkImage = GetComponent<Image>(); // Get the Image component 

        // Find the ShopItemNavigator in the scene
        barItemNavigator = FindObjectOfType<BarItemNavigator>();

        friendInviteNavigator = FindObjectOfType<FriendInviteNavigator>();

        // Get the index of this ShopItem in the ShopItemNavigator
        if (barItemNavigator != null)
        {
            activityIndex = barItemNavigator.GetShopItemIndex(gameObject);
        }

        // Get the index of this ShopItem in the ShopItemNavigator
        if (friendInviteNavigator != null)
        {
            activityIndex = friendInviteNavigator.GetShopItemIndex(gameObject);
        }

        barItemNavigator.ResetAllParkActivities();

        friendInviteNavigator.ResetFriendInvites();
    }

    public void OnHoverEnter()
    {
        if (displayText != null)
        {
            displayText.text = buttonDetails;
            displayTextBox.SetActive(true);

            // Reset all other ShopItems to blue
            if (barItemNavigator != null)
            {
                barItemNavigator.ResetAllParkActivities();
            }

            // Set this ShopItem's color to black
            if (barDrinkImage != null)
            {
                barDrinkImage.color = Color.black;
            }

            // Update the currentIndex in the ShopItemNavigator
            if (barItemNavigator != null)
            {
                barItemNavigator.SetCurrentIndex(activityIndex);
            }
        }
    }

    public void OnHoverExit()
    {
        if (displayText != null)
        {
            displayText.text = ""; // Clear the text
            displayTextBox.SetActive(false);

            if (barDrinkImage != null)
            {
                //barItemImage.color = Color.blue; // Reset color to blue on hover exit 
                //parkActivityImage.color = new Color(0.5f, 0f, 0.5f); // RGB: 128, 0, 128
                barDrinkImage.color = new Color(1f, 0.41f, 0.71f); // RGB: 255, 105, 180 (Hot Pink) 

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

        if (ToFile == "Darts" && currentSeason == CalendarSystem.Season.Spring && currentActivityPhase == "Evening")
        {
            HandleHangOutClick("JogMorningSpring");
        }
        else if (ToFile == "Karaoke" && currentSeason == CalendarSystem.Season.Spring && currentActivityPhase == "Evening")
        {
            HandleHangOutClick("JogAfternoonSpring");
        }
        else if (ToFile == "Drinks" && currentSeason == CalendarSystem.Season.Spring && currentActivityPhase == "Evening")
        {
            HandleHangOutClick("JogEveningSpring");
        }
        else if (ToFile == "EspressoMartini" && (currentSeason == CalendarSystem.Season.Spring || currentSeason == CalendarSystem.Season.Summer || currentSeason == CalendarSystem.Season.Fall || currentSeason == CalendarSystem.Season.Winter || currentSeason == CalendarSystem.Season.SpringA) && currentActivityPhase == "Evening")
        {
            HandleHangOutClick("EspressoMartini");
        }
        else if (ToFile == "OldFashioned" && (currentSeason == CalendarSystem.Season.Spring || currentSeason == CalendarSystem.Season.Summer || currentSeason == CalendarSystem.Season.Fall || currentSeason == CalendarSystem.Season.Winter || currentSeason == CalendarSystem.Season.SpringA) && currentActivityPhase == "Evening")
        {
            HandleHangOutClick("OldFashioned");
        }
        else if (ToFile == "Margarita" && (currentSeason == CalendarSystem.Season.Spring || currentSeason == CalendarSystem.Season.Summer || currentSeason == CalendarSystem.Season.Fall || currentSeason == CalendarSystem.Season.Winter || currentSeason == CalendarSystem.Season.SpringA) && currentActivityPhase == "Evening")
        {
            HandleHangOutClick("Margarita");
        }
        else if (ToFile == "ShotOfTequila" && (currentSeason == CalendarSystem.Season.Spring || currentSeason == CalendarSystem.Season.Summer || currentSeason == CalendarSystem.Season.Fall || currentSeason == CalendarSystem.Season.Winter || currentSeason == CalendarSystem.Season.SpringA) && currentActivityPhase == "Evening")
        {
            HandleHangOutClick("ShotOfTequila");
        }
        else if (ToFile == "RedWine" && (currentSeason == CalendarSystem.Season.Spring || currentSeason == CalendarSystem.Season.Summer || currentSeason == CalendarSystem.Season.Fall || currentSeason == CalendarSystem.Season.Winter || currentSeason == CalendarSystem.Season.SpringA) && currentActivityPhase == "Evening")
        {
            HandleHangOutClick("RedWine");
        }
        else if (ToFile == "Mojito" && (currentSeason == CalendarSystem.Season.Spring || currentSeason == CalendarSystem.Season.Summer || currentSeason == CalendarSystem.Season.Fall || currentSeason == CalendarSystem.Season.Winter || currentSeason == CalendarSystem.Season.SpringA) && currentActivityPhase == "Evening")
        {
            HandleHangOutClick("Mojito");
        }
        else if (ToFile == "Manhattan" && (currentSeason == CalendarSystem.Season.Spring || currentSeason == CalendarSystem.Season.Summer || currentSeason == CalendarSystem.Season.Fall || currentSeason == CalendarSystem.Season.Winter || currentSeason == CalendarSystem.Season.SpringA) && currentActivityPhase == "Evening")
        {
            HandleHangOutClick("Manhattan");
        }
        else if (ToFile == "HotToddy" && (currentSeason == CalendarSystem.Season.Spring || currentSeason == CalendarSystem.Season.Summer || currentSeason == CalendarSystem.Season.Fall || currentSeason == CalendarSystem.Season.Winter || currentSeason == CalendarSystem.Season.SpringA) && currentActivityPhase == "Evening")
        {
            HandleHangOutClick("HotToddy"); 
        }
        else if (ToFile == "WhiskeySour" && (currentSeason == CalendarSystem.Season.Spring || currentSeason == CalendarSystem.Season.Summer || currentSeason == CalendarSystem.Season.Fall || currentSeason == CalendarSystem.Season.Winter || currentSeason == CalendarSystem.Season.SpringA) && currentActivityPhase == "Evening")
        {
            HandleHangOutClick("WhiskeySour");
        }
        else if (ToFile == "Negroni" && (currentSeason == CalendarSystem.Season.Spring || currentSeason == CalendarSystem.Season.Summer || currentSeason == CalendarSystem.Season.Fall || currentSeason == CalendarSystem.Season.Winter || currentSeason == CalendarSystem.Season.SpringA) && currentActivityPhase == "Evening")
        {
            HandleHangOutClick("Negroni"); 
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

