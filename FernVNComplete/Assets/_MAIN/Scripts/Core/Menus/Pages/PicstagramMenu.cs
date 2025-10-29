using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Include the TextMeshPro namespace

// Class representing a post with an image and a caption
[System.Serializable]
public class Post
{
    public Sprite image;  // Image for the post

    [TextArea(3, 10)]  // This will make the caption field larger in the Inspector
    public string caption;  // Caption for the post
    public int captionFontSize = 14;  // Font size for the caption, default set to 14
}

// Class representing a character's profile, which contains 6 posts, profile picture, username, bio, likes, and dislikes
[System.Serializable]
public class CharacterProfile
{
    public Sprite profilePicture;  // Profile picture for the character
    public string username;  // Username for the character

    public int usernameFontSize = 14;  // Font size for the username, default set to 14

    [TextArea(3, 10)]  // This will make the bio field larger in the Inspector
    public string bio;  // Bio for the character
    public int bioFontSize = 14;  // Font size for the bio, default set to 14

    [TextArea(3, 10)]  // This will make the likes field larger in the Inspector
    public string likes;  // Things the character likes
    public int likesFontSize = 14;  // Font size for the likes, default set to 14

    [TextArea(3, 10)]  // This will make the dislikes field larger in the Inspector
    public string dislikes;  // Things the character dislikes
    public int dislikesFontSize = 14;  // Font size for the dislikes, default set to 14

    public Post[] posts = new Post[6];  // Each character has 6 posts
}

public class PicstagramMenu : MenuPage
{
    public DirectMessageManager directMessageManager;

    public static PicstagramMenu Instance;

    // Public references for the buttons and friend list
    public Button[] friendButtons;  // Array to store buttons for each friend (10 buttons for 10 characters)
    public GameObject friendPanel;  // The single panel used to display the friend’s profile
    public GameObject friendListPanel;  // Reference to the FriendList panel
    public Button returnButton;  // Return button to go back to the friend list from the profile panel

    // Public references for the profile details UI elements
    public Image profilePictureImage;  // UI Image for displaying the profile picture
    public TextMeshProUGUI usernameText;  // TextMeshProUGUI for displaying the username
    public TextMeshProUGUI bioText;  // TextMeshProUGUI for displaying the bio
    public TextMeshProUGUI likesText;  // TextMeshProUGUI for displaying likes
    public TextMeshProUGUI dislikesText;  // TextMeshProUGUI for displaying dislikes

    public Image[] postImages;  // Array of Image components for displaying the posts (6 images)
    public TextMeshProUGUI[] postCaptions;  // Array of TextMeshProUGUI components for displaying the captions (6 captions)

    public CharacterProfile[] characterProfiles = new CharacterProfile[10];  // Array of 10 character profiles (each with 6 posts)

    // Separate references for enlarged image panels for profile picture and post images
    public GameObject enlargedProfilePicturePanel;  // Panel for the enlarged profile picture
    public Image enlargedProfilePictureDisplay;  // UI Image for displaying the enlarged profile picture

    public GameObject enlargedPostImagePanel;  // Panel for the enlarged post images
    public Image enlargedPostImageDisplay;  // UI Image for displaying the enlarged post image

    private int currentProfileIndex;  // Track the index of the currently displayed friend profile  
    private int selectedFriendIndex;  // This will store the currently selected friend index  

    public Button messengerButton;

    private AudioSource picstagramSelectSound; 

    void Start()
    {
        // Assign listeners to each button based on its index
        for (int i = 0; i < friendButtons.Length; i++)
        {
            int index = i;  // Store the index to avoid closure issues
            friendButtons[i].onClick.AddListener(() => OpenFriendProfile(index));
        }

        // Add click listener to the profile picture to enlarge
        profilePictureImage.GetComponent<Button>().onClick.AddListener(() => ShowEnlargedProfilePicture(profilePictureImage.sprite));

        // Add click listeners to post images to enlarge
        for (int i = 0; i < postImages.Length; i++)
        {
            int index = i;  // Store the index to avoid closure issues
            postImages[i].GetComponent<Button>().onClick.AddListener(() => ShowEnlargedPostImage(postImages[index].sprite));
        }

        // Add click listener to hide the enlarged profile picture panel
        enlargedProfilePicturePanel.GetComponent<Button>().onClick.AddListener(HideEnlargedProfilePicture);

        // Add click listener to hide the enlarged post image panel
        enlargedPostImagePanel.GetComponent<Button>().onClick.AddListener(HideEnlargedPostImage);

        messengerButton.onClick.AddListener(() => OpenFriendMessages(selectedFriendIndex));

        returnButton.onClick.AddListener(ReturnToFriendList);
        returnButton.gameObject.SetActive(false);

        friendListPanel.SetActive(true);

        // Ensure the friend panel and enlarged image panels are initially hidden
        friendPanel.SetActive(false);
        enlargedProfilePicturePanel.SetActive(false);
        enlargedPostImagePanel.SetActive(false);

        // Subscribe to the day change event in CalendarSystem
        if (CalendarSystem.Instance != null)
        {
            CalendarSystem.Instance.onDayChanged += HandleDayChanged;
        }

        picstagramSelectSound = GameObject.Find("PicstagramSelectSound").GetComponent<AudioSource>(); 
    }

    private void OnDestroy()
    {
        // Unsubscribe from the day change event when this object is destroyed
        if (CalendarSystem.Instance != null)
        {
            CalendarSystem.Instance.onDayChanged -= HandleDayChanged;
        }
    }

    // Function to open the friend profile and load the respective details
    void OpenFriendProfile(int index)
    {
        picstagramSelectSound.PlayOneShot(picstagramSelectSound.clip);

        selectedFriendIndex = index;
        currentProfileIndex = index;
        friendListPanel.SetActive(false);
        friendPanel.SetActive(true);

        LoadCharacterProfile(index);
        directMessageManager.SetSelectedFriend(selectedFriendIndex);

        // Enable or disable messengerButton based on message availability
        messengerButton.interactable = directMessageManager.friendMessageAvailable[index];

        returnButton.gameObject.SetActive(true);
    }

    // Call this method when the OpenMessagePanel() button is clicked
    public void OpenFriendMessages()
    {
        //messagingClickSound.PlayOneShot(messagingClickSound.clip);

        // Ensure the message panel opens based on the selected friend
        directMessageManager.OpenMessagePanel(selectedFriendIndex);  // Use selectedFriendIndex here
    } 

    // Function to handle when the day changes (triggered by the CalendarSystem)
    void HandleDayChanged()
    {
        LoadMessagesForSelectedFriend(currentProfileIndex);
    } 

    // Function to load a character's profile details (profile picture, username, bio, likes, dislikes, posts)
    void LoadCharacterProfile(int characterIndex)
    {
        CharacterProfile profile = characterProfiles[characterIndex];

        // Set profile details
        profilePictureImage.sprite = profile.profilePicture;  // Set the profile picture
        usernameText.text = profile.username;  // Set the username
        usernameText.fontSize = profile.usernameFontSize;  // Set the font size for the username

        bioText.text = profile.bio;  // Set the bio
        bioText.fontSize = profile.bioFontSize;  // Set the font size of the bio

        likesText.text = "Likes: " + profile.likes;  // Set the likes
        likesText.fontSize = profile.likesFontSize;  // Set the font size for likes

        dislikesText.text = "Dislikes: " + profile.dislikes;  // Set the dislikes
        dislikesText.fontSize = profile.dislikesFontSize;  // Set the font size for dislikes

        // Set posts (images and captions)
        for (int i = 0; i < profile.posts.Length; i++)
        {
            postImages[i].sprite = profile.posts[i].image;  // Set the image for each post
            postCaptions[i].text = profile.posts[i].caption;  // Set the caption for each post
            postCaptions[i].fontSize = profile.posts[i].captionFontSize;  // Set the font size for each caption
        }
    }

    void LoadMessagesForSelectedFriend(int friendIndex)
    {
        directMessageManager.SetSelectedFriend(friendIndex);

        // Retrieve the single stored message for the current day
        List<DirectMessageManager.Message> messageList = directMessageManager.GetMessagesForCurrentSeason(friendIndex);

        if (messageList != null && messageList.Count > 0)
        {
            directMessageManager.PopulateMessagePanel(messageList);  // Only one message should be in the list
        }

        // Add a listener to open messages when the friend button is clicked
        OpenFriendMessages(friendIndex); 
    } 

    // Helper function to return the correct message list for the current season
    List<DirectMessageManager.Message> GetMessagesForSeason(List<DirectMessageManager.Message> spring, List<DirectMessageManager.Message> summer, List<DirectMessageManager.Message> fall, List<DirectMessageManager.Message> winter, CalendarSystem.Season season)
    {
        switch (season)
        {
            case CalendarSystem.Season.Spring:
                return spring;
            case CalendarSystem.Season.Summer:
                return summer;
            case CalendarSystem.Season.Fall:
                return fall;
            case CalendarSystem.Season.Winter:
                return winter;
            default:
                return spring;  // Default to spring if season is unknown
        }
    } 

    // Function to return to the FriendList from the profile panel
    void ReturnToFriendList()
    {
        friendPanel.SetActive(false);  // Hide the active friend panel
        friendListPanel.SetActive(true);  // Show the FriendList panel 
        returnButton.gameObject.SetActive(false);
    }

    // Function to display an enlarged profile picture
    void ShowEnlargedProfilePicture(Sprite image)
    {
        enlargedProfilePictureDisplay.sprite = image;  // Set the enlarged profile picture
        enlargedProfilePicturePanel.SetActive(true);  // Show the enlarged profile picture panel
    }

    // Function to display an enlarged post image
    void ShowEnlargedPostImage(Sprite image)
    {
        enlargedPostImageDisplay.sprite = image;  // Set the enlarged post image
        enlargedPostImagePanel.SetActive(true);  // Show the enlarged post image panel 
    }

    void OpenFriendMessages(int friendIndex)
    {
        // Open the message panel for the selected friend
        directMessageManager.OpenMessagePanel(friendIndex);
    } 

    // Function to hide the enlarged profile picture panel
    void HideEnlargedProfilePicture()
    {
        enlargedProfilePicturePanel.SetActive(false);  // Hide the enlarged profile picture panel
    }

    // Function to hide the enlarged post image panel
    void HideEnlargedPostImage()
    {
        enlargedPostImagePanel.SetActive(false);  // Hide the enlarged post image panel
    }
}
