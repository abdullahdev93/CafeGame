using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class FriendButton : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string buttonDetails; 

    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private TextMeshProUGUI friendShipLevelText; 
    [SerializeField] private GameObject displayTextBox;
    [SerializeField] private float barItemPrice;

    private Image barItemImage;

    // Reference to the ShopItemNavigator
    private FriendInviteNavigator friendInviteNavigator;

    private int itemIndex;

    [SerializeField] private string friendName; // Assign this in the Inspector (e.g., "Mei", "Alex", etc.) 

    private void Start()
    {
        if (displayText == null)
        {
            Debug.LogError("Display Text is not assigned! Please assign a Text object in the Inspector.");
        }

        displayTextBox.SetActive(false);
        barItemImage = GetComponent<Image>(); // Get the Image component

        // Find the ShopItemNavigator in the scene
        friendInviteNavigator = FindObjectOfType<FriendInviteNavigator>();

        // Get the index of this ShopItem in the ShopItemNavigator
        if (friendInviteNavigator != null)
        {
            itemIndex = friendInviteNavigator.GetShopItemIndex(gameObject);
        }

        friendInviteNavigator.ResetFriendInvites();

        // Set the Friendship Level Text
        UpdateFriendshipLevelText(); 
    }

    private void UpdateFriendshipLevelText()
    {
        if (friendShipLevelText != null)
        {
            FriendshipStat friendStat = FriendshipStats.instance?.GetFriendshipStat(friendName);
            if (friendStat != null)
            {
                friendShipLevelText.text = "Friendship Level: " + friendStat.Level.ToString();
            }
            else
            {
                Debug.LogError("FriendshipStat not found for " + friendName);
            }
        }
    } 

    public void OnHoverEnter()
    {
        if (displayText != null)
        {
            displayText.text = buttonDetails;
            displayTextBox.SetActive(true);

            // Reset all other ShopItems to blue
            if (friendInviteNavigator != null)
            {
                friendInviteNavigator.ResetFriendInvites(); 
            }

            // Set this ShopItem's color to black
            if (barItemImage != null)
            {
                barItemImage.color = Color.black;
            }

            // Update the currentIndex in the ShopItemNavigator
            if (friendInviteNavigator != null)
            {
                friendInviteNavigator.SetCurrentIndex(itemIndex);
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
                barItemImage.color = Color.blue; // Reset color to blue on hover exit 
                //barItemImage.color = new Color(0.5f, 0f, 0.5f); // RGB: 128, 0, 128   
            }
        }
    }

    public void OnPurchaseButtonClick(string ToFile)
    {
        string currentDay = CalendarSystem.Instance.GetCurrentDayOfWeek();
        string currentActivityPhase = CalendarSystem.Instance.activityPhases[CalendarSystem.Instance.activityCounter];

        if (ToFile == "Jog" && currentActivityPhase == "Morning")
        {
            HandleHangOutClick("JogMorningSpring");
        }

        else if (ToFile == "Jog" && currentActivityPhase == "Afternoon")
        {
            HandleHangOutClick("JogAfternoonSpring");
        }

        else if (ToFile == "Jog" && currentActivityPhase == "Evening") 
        {
            HandleHangOutClick("JogEveningSpring"); 
        }

        if (ToFile == "Jog" && (currentDay == "Monday" || currentDay == "Wednesday" || currentDay == "Sunday") && currentActivityPhase == "Morning")
        {
            HandleHangOutClick("JogMorningSpring");
        }
        else
        {
            HandleHangOutClick(ToFile);
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
}
