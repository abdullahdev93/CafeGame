using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DirectMessageManager : MonoBehaviour
{
    public static DirectMessageManager Instance;

    [System.Serializable]
    public class Message
    {
        public string activityName;
        [TextArea(4, 10)]
        public string activityContext;
        public Sprite characterImage;
        public Sprite characterFullImage;
    }

    // Friend message lists
    public List<Message> friendAMessagesSpring, friendAMessagesSummer, friendAMessagesFall, friendAMessagesWinter;
    public List<Message> friendBMessagesSpring, friendBMessagesSummer, friendBMessagesFall, friendBMessagesWinter;
    public List<Message> friendCMessagesSpring, friendCMessagesSummer, friendCMessagesFall, friendCMessagesWinter; 
    public List<Message> friendDMessagesSpring, friendDMessagesSummer, friendDMessagesFall, friendDMessagesWinter;
    // ... Add other friend lists here ...

    public List<Message> friendAMessagesMajor;
    public List<Message> friendBMessagesMajor;
    public List<Message> friendCMessagesMajor;
    public List<Message> friendDMessagesMajor;
    //private List<Message> currentFriendAMessages; // Store Friend A messages for the current day
    //private List<Message> currentFriendBMessages; // Store Friend B messages for the current day
    // You can add similar lists for other friends as needed 

    public GameObject messageUIPrefab;
    public Transform messageContainer;
    public GameObject messagePanel;
    public TextMeshProUGUI messageCounterText, notificationsCounterText;
    public TextMeshProUGUI daysLeftText; 

    private int messageCounter;
    private int notificationCounter;

    public GameObject messageDetailPanel;
    public Image detailCharacterImage;
    public TextMeshProUGUI detailActivityName;
    public Button acceptButton, declineButton;

    private bool alreadyRead = false;
    private int selectedFriendIndex; 

    private Dictionary<int, int> friendMessageCounters = new Dictionary<int, int>();

    // Track read status for messages (boolean list for each friend)
    private Dictionary<int, List<bool>> messageReadStatus = new Dictionary<int, List<bool>>();

    // Store the currently selected message for each friend
    private Message selectedFriendAMessage;
    private Message selectedFriendBMessage; 
    private Message selectedFriendCMessage; 
    private Message selectedFriendDMessage; 

    // Keep track of whether the messages have been selected for the current day
    private bool messagesSelectedForDay = false;

    public bool alreadyAcceptedMeiHangOutTwo;
    public bool alreadyAcceptedMeiHangOutThree;
    public bool alreadyAcceptedMeiHangOutFour;
    public bool alreadyAcceptedMeiHangOutFive;
    public bool alreadyAcceptedMeiHangOutSix;
    public bool alreadyAcceptedMeiHangOutSeven;
    public bool alreadyAcceptedMeiHangOutEight;
    public bool alreadyAcceptedMeiHangOutNine;
    public bool alreadyAcceptedMeiHangOutTen;

    public bool alreadyAcceptedAlexHangOutTwo;
    public bool alreadyAcceptedAlexHangOutThree;
    public bool alreadyAcceptedAlexHangOutFour;
    public bool alreadyAcceptedAlexHangOutFive;
    public bool alreadyAcceptedAlexHangOutSix;
    public bool alreadyAcceptedAlexHangOutSeven;
    public bool alreadyAcceptedAlexHangOutEight;
    public bool alreadyAcceptedAlexHangOutNine;
    public bool alreadyAcceptedAlexHangOutTen;

    public bool alreadyAcceptedNinaHangOutTwo;
    public bool alreadyAcceptedNinaHangOutThree;
    public bool alreadyAcceptedNinaHangOutFour;
    public bool alreadyAcceptedNinaHangOutFive;
    public bool alreadyAcceptedNinaHangOutSix;
    public bool alreadyAcceptedNinaHangOutSeven;
    public bool alreadyAcceptedNinaHangOutEight;
    public bool alreadyAcceptedNinaHangOutNine;
    public bool alreadyAcceptedNinaHangOutTen;

    public bool alreadyAcceptedSimonHangOutTwo;
    public bool alreadyAcceptedSimonHangOutThree;
    public bool alreadyAcceptedSimonHangOutFour;
    public bool alreadyAcceptedSimonHangOutFive;
    public bool alreadyAcceptedSimonHangOutSix;
    public bool alreadyAcceptedSimonHangOutSeven;
    public bool alreadyAcceptedSimonHangOutEight;
    public bool alreadyAcceptedSimonHangOutNine;
    public bool alreadyAcceptedSimonHangOutTen; 


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

    private void Start()
    {
        messagePanel.SetActive(false);
        messageDetailPanel.SetActive(false);

        notificationCounter = LoadNotificationCounter();

        InitializeNotificationCounter();
        InitializeFriendMessageCounters();  // Load message counters from PlayerPrefs
        InitializeMessageReadStatus();  // Load message read status from PlayerPrefs  

        // Subscribe to day change event from CalendarSystem
        if (CalendarSystem.Instance != null)
        {
            CalendarSystem.Instance.onDayChanged += HandleDayChanged;
        }

        // Load the messages for the current day when the scene starts
        CalendarSystem.Season currentSeason = CalendarSystem.Instance.GetCurrentSeason();
        int currentDay = CalendarSystem.Instance.GetCurrentDay();

        // Load the saved messages for the current day
        if (PlayerPrefs.HasKey("FriendAMessage_" + currentSeason.ToString() + "_" + currentDay))
        {
            LoadMessagesForCurrentDay(currentSeason, currentDay);
        }
        else
        {
            // If no saved message is found, select new messages
            SelectMessagesForCurrentDay();
        }

        ResetNotificationCounter();

        alreadyAcceptedMeiHangOutTwo = PlayerPrefs.GetInt("AlreadyAcceptedMeiHangOutTwo", 0) == 1;
        alreadyAcceptedMeiHangOutThree = PlayerPrefs.GetInt("AlreadyAcceptedMeiHangOutThree", 0) == 1;
        alreadyAcceptedMeiHangOutFour = PlayerPrefs.GetInt("AlreadyAcceptedMeiHangOutFour", 0) == 1;
        alreadyAcceptedMeiHangOutFive = PlayerPrefs.GetInt("AlreadyAcceptedMeiHangOutFive", 0) == 1;
        alreadyAcceptedMeiHangOutSix = PlayerPrefs.GetInt("AlreadyAcceptedMeiHangOutSix", 0) == 1;
        alreadyAcceptedMeiHangOutSeven = PlayerPrefs.GetInt("AlreadyAcceptedMeiHangOutSeven", 0) == 1;
        alreadyAcceptedMeiHangOutEight = PlayerPrefs.GetInt("AlreadyAcceptedMeiHangOutEight", 0) == 1;
        alreadyAcceptedMeiHangOutNine = PlayerPrefs.GetInt("AlreadyAcceptedMeiHangOutNine", 0) == 1;
        alreadyAcceptedMeiHangOutTen = PlayerPrefs.GetInt("AlreadyAcceptedMeiHangOutTen", 0) == 1;

        alreadyAcceptedAlexHangOutTwo = PlayerPrefs.GetInt("AlreadyAcceptedAlexHangOutTwo", 0) == 1;
        alreadyAcceptedAlexHangOutThree = PlayerPrefs.GetInt("AlreadyAcceptedAlexHangOutThree", 0) == 1;
        alreadyAcceptedAlexHangOutFour = PlayerPrefs.GetInt("AlreadyAcceptedAlexHangOutFour", 0) == 1;
        alreadyAcceptedAlexHangOutFive = PlayerPrefs.GetInt("AlreadyAcceptedAlexHangOutFive", 0) == 1;
        alreadyAcceptedAlexHangOutSix = PlayerPrefs.GetInt("AlreadyAcceptedAlexHangOutSix", 0) == 1;
        alreadyAcceptedAlexHangOutSeven = PlayerPrefs.GetInt("AlreadyAcceptedAlexHangOutSeven", 0) == 1;
        alreadyAcceptedAlexHangOutEight = PlayerPrefs.GetInt("AlreadyAcceptedAlexHangOutEight", 0) == 1;
        alreadyAcceptedAlexHangOutNine = PlayerPrefs.GetInt("AlreadyAcceptedAlexHangOutNine", 0) == 1;
        alreadyAcceptedAlexHangOutTen = PlayerPrefs.GetInt("AlreadyAcceptedAlexHangOutTen", 0) == 1;

        alreadyAcceptedNinaHangOutTwo = PlayerPrefs.GetInt("AlreadyAcceptedNinaHangOutTwo", 0) == 1;
        alreadyAcceptedNinaHangOutThree = PlayerPrefs.GetInt("AlreadyAcceptedNinaHangOutThree", 0) == 1;
        alreadyAcceptedNinaHangOutFour = PlayerPrefs.GetInt("AlreadyAcceptedNinaHangOutFour", 0) == 1;
        alreadyAcceptedNinaHangOutFive = PlayerPrefs.GetInt("AlreadyAcceptedNinaHangOutFive", 0) == 1;
        alreadyAcceptedNinaHangOutSix = PlayerPrefs.GetInt("AlreadyAcceptedNinaHangOutSix", 0) == 1;
        alreadyAcceptedNinaHangOutSeven = PlayerPrefs.GetInt("AlreadyAcceptedNinaHangOutSeven", 0) == 1;
        alreadyAcceptedNinaHangOutEight = PlayerPrefs.GetInt("AlreadyAcceptedNinaHangOutEight", 0) == 1;
        alreadyAcceptedNinaHangOutNine = PlayerPrefs.GetInt("AlreadyAcceptedNinaHangOutNine", 0) == 1;
        alreadyAcceptedNinaHangOutTen = PlayerPrefs.GetInt("AlreadyAcceptedNinaHangOutTen", 0) == 1;

        alreadyAcceptedSimonHangOutTwo = PlayerPrefs.GetInt("AlreadyAcceptedSimonHangOutTwo", 0) == 1;
        alreadyAcceptedSimonHangOutThree = PlayerPrefs.GetInt("AlreadyAcceptedSimonHangOutThree", 0) == 1;
        alreadyAcceptedSimonHangOutFour = PlayerPrefs.GetInt("AlreadyAcceptedSimonHangOutFour", 0) == 1;
        alreadyAcceptedSimonHangOutFive = PlayerPrefs.GetInt("AlreadyAcceptedSimonHangOutFive", 0) == 1;
        alreadyAcceptedSimonHangOutSix = PlayerPrefs.GetInt("AlreadyAcceptedSimonHangOutSix", 0) == 1;
        alreadyAcceptedSimonHangOutSeven = PlayerPrefs.GetInt("AlreadyAcceptedSimonHangOutSeven", 0) == 1;
        alreadyAcceptedSimonHangOutEight = PlayerPrefs.GetInt("AlreadyAcceptedSimonHangOutEight", 0) == 1;
        alreadyAcceptedSimonHangOutNine = PlayerPrefs.GetInt("AlreadyAcceptedSimonHangOutNine", 0) == 1;
        alreadyAcceptedSimonHangOutTen = PlayerPrefs.GetInt("AlreadyAcceptedSimonHangOutTen", 0) == 1; 
    }

    public bool[] friendMessageAvailable = new bool[10]; // Array to track message availability for up to 10 friends 

    private void SelectMessagesForCurrentDay()
    { 
        CalendarSystem.Season currentSeason = CalendarSystem.Instance.GetCurrentSeason();
        int currentDay = CalendarSystem.Instance.GetCurrentDay();

        notificationCounter = 0;

        string dayOfWeek = CalendarSystem.Instance.GetCurrentDayOfWeek();

        // Friend A message selection
        if (dayOfWeek == "Tuesday" || dayOfWeek == "Friday" || dayOfWeek == "Saturday")
        {
            if (FriendshipStats.instance.FriendA.Level >= 10 && !alreadyAcceptedMeiHangOutTen)
            {
                selectedFriendAMessage = friendAMessagesMajor[8];
                daysLeftText.text = "Days left: No Time Limit";
            }
            if (FriendshipStats.instance.FriendA.Level >= 9 && !alreadyAcceptedMeiHangOutNine)
            {
                selectedFriendAMessage = friendAMessagesMajor[7];
                daysLeftText.text = "Days left: No Time Limit";
            }
            if (FriendshipStats.instance.FriendA.Level >= 8 && !alreadyAcceptedMeiHangOutEight)
            {
                selectedFriendAMessage = friendAMessagesMajor[6];
                daysLeftText.text = "Days left: No Time Limit";
            }
            if (FriendshipStats.instance.FriendA.Level >= 7 && !alreadyAcceptedMeiHangOutSeven)
            {
                selectedFriendAMessage = friendAMessagesMajor[5];
                daysLeftText.text = "Days left: No Time Limit";
            }
            if (FriendshipStats.instance.FriendA.Level >= 6 && !alreadyAcceptedMeiHangOutSix)
            {
                selectedFriendAMessage = friendAMessagesMajor[4];
                daysLeftText.text = "Days left: No Time Limit";
            }
            if (FriendshipStats.instance.FriendA.Level >= 5 && !alreadyAcceptedMeiHangOutFive)
            {
                selectedFriendAMessage = friendAMessagesMajor[3];
                daysLeftText.text = "Days left: No Time Limit";
            }
            if (FriendshipStats.instance.FriendA.Level >= 4 && !alreadyAcceptedMeiHangOutFour)
            {
                selectedFriendAMessage = friendAMessagesMajor[2];
                daysLeftText.text = "Days left: No Time Limit";
            }
            if (FriendshipStats.instance.FriendA.Level >= 3 && !alreadyAcceptedMeiHangOutThree)
            {
                selectedFriendAMessage = friendAMessagesMajor[1];
                daysLeftText.text = "Days left: No Time Limit";
            }
            if (FriendshipStats.instance.FriendA.Level >= 2 && !alreadyAcceptedMeiHangOutTwo)
            {
                selectedFriendAMessage = friendAMessagesMajor[0];
                daysLeftText.text = "Days left: No Time Limit"; 
            } 
            else 
                selectedFriendAMessage = GetRandomMessageForSeason(friendAMessagesSpring, friendAMessagesSummer, friendAMessagesFall, friendAMessagesWinter, currentSeason);
            
            friendMessageAvailable[0] = selectedFriendAMessage != null;
            friendMessageCounters[0] = selectedFriendAMessage != null ? 1 : 0;
            if (friendMessageAvailable[0]) notificationCounter++;
        }
        else
        {
            selectedFriendAMessage = null;
            friendMessageAvailable[0] = false;
            friendMessageCounters[0] = 0;
        }

        // Friend B message selection
        if (dayOfWeek == "Monday" || dayOfWeek == "Thursday" || dayOfWeek == "Sunday")
        {
            selectedFriendBMessage = GetRandomMessageForSeason(friendBMessagesSpring, friendBMessagesSummer, friendBMessagesFall, friendBMessagesWinter, currentSeason);
            friendMessageAvailable[1] = selectedFriendBMessage != null;
            friendMessageCounters[1] = selectedFriendBMessage != null ? 1 : 0;
            if (friendMessageAvailable[1]) notificationCounter++;
        }
        else
        {
            selectedFriendBMessage = null;
            friendMessageAvailable[1] = false;
            friendMessageCounters[1] = 0;
        }

        // Friend C selection
        if (dayOfWeek == "Monday" || dayOfWeek == "Wednesday" || dayOfWeek == "Friday")   
        {
            selectedFriendCMessage = GetRandomMessageForSeason(friendCMessagesSpring, friendCMessagesSummer, friendCMessagesFall, friendCMessagesWinter, currentSeason);
            friendMessageAvailable[2] = selectedFriendCMessage != null;
            friendMessageCounters[2] = selectedFriendCMessage != null ? 1 : 0;
            if (friendMessageAvailable[2]) notificationCounter++;
        }
        else
        {
            selectedFriendBMessage = null;
            friendMessageAvailable[2] = false;
            friendMessageCounters[2] = 0;
        }

        // Friend D message selection
        if (dayOfWeek == "Wednesday" || dayOfWeek == "Saturday" || dayOfWeek == "Sunday")
        {
            selectedFriendDMessage = GetRandomMessageForSeason(friendDMessagesSpring, friendDMessagesSummer, friendDMessagesFall, friendDMessagesWinter, currentSeason);
            friendMessageAvailable[3] = selectedFriendDMessage != null;
            friendMessageCounters[3] = selectedFriendDMessage != null ? 1 : 0;
            if (friendMessageAvailable[3]) notificationCounter++;
        }
        else
        {
            selectedFriendDMessage = null;
            friendMessageAvailable[3] = false;
            friendMessageCounters[3] = 0;
        }

        SaveNotificationCounter();
        UpdateNotificationCounter(); 
    }

    private void OnDestroy()
    {
        // Unsubscribe from the day change event
        if (CalendarSystem.Instance != null)
        {
            CalendarSystem.Instance.onDayChanged -= HandleDayChanged;
        }
    }

    // Handle day change by resetting the selected messages
    private void HandleDayChanged()
    {
        // Clear the previous day's messages
        PlayerPrefs.DeleteKey("FriendAMessage_" + CalendarSystem.Instance.GetCurrentSeason().ToString() + "_" + CalendarSystem.Instance.GetCurrentDay());
        PlayerPrefs.DeleteKey("FriendBMessage_" + CalendarSystem.Instance.GetCurrentSeason().ToString() + "_" + CalendarSystem.Instance.GetCurrentDay());
        PlayerPrefs.DeleteKey("FriendCMessage_" + CalendarSystem.Instance.GetCurrentSeason().ToString() + "_" + CalendarSystem.Instance.GetCurrentDay()); 
        PlayerPrefs.DeleteKey("FriendDMessage_" + CalendarSystem.Instance.GetCurrentSeason().ToString() + "_" + CalendarSystem.Instance.GetCurrentDay());
        PlayerPrefs.Save();

        // Select new messages for the new day
        SelectMessagesForCurrentDay();

        //ResetNotificationCounter(); 

        // Reset the read status for all friends
        ResetMessageReadStatus();  
    }

    // Helper method to get the days left in the current season
    private int GetDaysLeftInCurrentSeason(CalendarSystem.Season season)
    {
        int daysInSeason = CalendarSystem.Instance.GetDaysInSeason(season);
        int currentDay = CalendarSystem.Instance.GetCurrentDay();
        return daysInSeason - currentDay;
    } 

    private void ResetMessageReadStatus()
    {
        messageReadStatus[0] = new List<bool>(new bool[friendAMessagesSpring.Count]);
        messageReadStatus[1] = new List<bool>(new bool[friendBMessagesSpring.Count]);
        messageReadStatus[2] = new List<bool>(new bool[friendCMessagesSpring.Count]); 
        messageReadStatus[3] = new List<bool>(new bool[friendDMessagesSpring.Count]);  
        SaveMessageReadStatus(0, messageReadStatus[0]);
        SaveMessageReadStatus(1, messageReadStatus[1]);
        SaveMessageReadStatus(2, messageReadStatus[2]);
        SaveMessageReadStatus(3, messageReadStatus[3]); 
    } 

    // Select a single message for each friend for the current day
    // Save the selected messages for the current day
    private void SaveMessagesForCurrentDay(CalendarSystem.Season season, int day)
    {
        if (selectedFriendAMessage != null)
        {
            PlayerPrefs.SetString("FriendAMessage_" + season.ToString() + "_" + day, selectedFriendAMessage.activityName);
        }

        if (selectedFriendBMessage != null)
        {
            PlayerPrefs.SetString("FriendBMessage_" + season.ToString() + "_" + day, selectedFriendBMessage.activityName);
        }

        if (selectedFriendCMessage != null)
        {
            PlayerPrefs.SetString("FriendCMessage_" + season.ToString() + "_" + day, selectedFriendCMessage.activityName); 
        } 

        if (selectedFriendDMessage != null)
        {
            PlayerPrefs.SetString("FriendDMessage_" + season.ToString() + "_" + day, selectedFriendDMessage.activityName);
        } 

        PlayerPrefs.Save();
    } 

    // Load the saved messages for the current day
    private void LoadMessagesForCurrentDay(CalendarSystem.Season season, int day)
    {
        string friendAMessageActivityName = PlayerPrefs.GetString("FriendAMessage_" + season.ToString() + "_" + day, null);
        string friendBMessageActivityName = PlayerPrefs.GetString("FriendBMessage_" + season.ToString() + "_" + day, null);
        string friendCMessageActivityName = PlayerPrefs.GetString("FriendCMessage_" + season.ToString() + "_" + day, null); 
        string friendDMessageActivityName = PlayerPrefs.GetString("FriendDMessage_" + season.ToString() + "_" + day, null); 

        if (!string.IsNullOrEmpty(friendAMessageActivityName))
        {
            if (FriendshipStats.instance.FriendA.Level >= 10 && !alreadyAcceptedMeiHangOutTen)
            { 
                selectedFriendAMessage = friendAMessagesMajor[8];
                daysLeftText.text = "Days left: No Time Limit";
            }
            if (FriendshipStats.instance.FriendA.Level >= 9 && !alreadyAcceptedMeiHangOutNine)
            {
                selectedFriendAMessage = friendAMessagesMajor[7];
                daysLeftText.text = "Days left: No Time Limit";
            }
            if (FriendshipStats.instance.FriendA.Level >= 8 && !alreadyAcceptedMeiHangOutEight)
            { 
                selectedFriendAMessage = friendAMessagesMajor[6];
                daysLeftText.text = "Days left: No Time Limit";
            }
            if (FriendshipStats.instance.FriendA.Level >= 7 && !alreadyAcceptedMeiHangOutSeven)
            { 
                selectedFriendAMessage = friendAMessagesMajor[5];
                daysLeftText.text = "Days left: No Time Limit";
            }
            if (FriendshipStats.instance.FriendA.Level >= 6 && !alreadyAcceptedMeiHangOutSix)
            { 
                selectedFriendAMessage = friendAMessagesMajor[4];
                daysLeftText.text = "Days left: No Time Limit";
            }
            if (FriendshipStats.instance.FriendA.Level >= 5 && !alreadyAcceptedMeiHangOutFive)
            { 
                selectedFriendAMessage = friendAMessagesMajor[3];
                daysLeftText.text = "Days left: No Time Limit";
            }
            if (FriendshipStats.instance.FriendA.Level >= 4 && !alreadyAcceptedMeiHangOutFour)
            { 
                selectedFriendAMessage = friendAMessagesMajor[2];
                daysLeftText.text = "Days left: No Time Limit";
            }
            if (FriendshipStats.instance.FriendA.Level >= 3 && !alreadyAcceptedMeiHangOutThree)
            { 
                selectedFriendAMessage = friendAMessagesMajor[1];
                daysLeftText.text = "Days left: No Time Limit";
            }
            if (FriendshipStats.instance.FriendA.Level >= 2 && !alreadyAcceptedMeiHangOutTwo)
            {
                selectedFriendAMessage = friendAMessagesMajor[0];
                daysLeftText.text = "Days left: No Time Limit"; 
            } 
            else 
                selectedFriendAMessage = FindMessageByActivityName(friendAMessageActivityName, friendAMessagesSpring, friendAMessagesSummer, friendAMessagesFall, friendAMessagesWinter); 
        }

        if (!string.IsNullOrEmpty(friendBMessageActivityName))
        {
            selectedFriendBMessage = FindMessageByActivityName(friendBMessageActivityName, friendBMessagesSpring, friendBMessagesSummer, friendBMessagesFall, friendBMessagesWinter);
        }

        if (!string.IsNullOrEmpty(friendCMessageActivityName))
        {
            selectedFriendCMessage = FindMessageByActivityName(friendCMessageActivityName, friendCMessagesSpring, friendCMessagesSummer, friendCMessagesFall, friendCMessagesWinter); 
        } 

        if (!string.IsNullOrEmpty(friendDMessageActivityName))
        {
            selectedFriendDMessage = FindMessageByActivityName(friendDMessageActivityName, friendDMessagesSpring, friendDMessagesSummer, friendDMessagesFall, friendDMessagesWinter);
        } 
    } 

    // Helper function to find message by activity name
    private Message FindMessageByActivityName(string activityName, List<Message> spring, List<Message> summer, List<Message> fall, List<Message> winter)
    {
        List<Message>[] allSeasons = { spring, summer, fall, winter };

        foreach (List<Message> seasonMessages in allSeasons)
        {
            foreach (Message message in seasonMessages)
            {
                if (message.activityName == activityName)
                {
                    return message;
                }
            }
        }
        return null; // Return null if no match is found
    } 

    public void HandleSceneChange()
    {
        // Save the currently selected messages before changing scenes
        CalendarSystem.Season currentSeason = CalendarSystem.Instance.GetCurrentSeason();
        int currentDay = CalendarSystem.Instance.GetCurrentDay();

        SaveMessagesForCurrentDay(currentSeason, currentDay); 
    } 

    // Update method to check for the "D" key press and reset PlayerPrefs
    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (!VNMenuManager.instance.developerMode)
                return; 

            ResetPlayerPrefs();
        }
    }

    private void InitializeNotificationCounter()
    {
        // Load or recalculate the notification counter
        notificationCounter = LoadNotificationCounter() - CountReadMessages();

        if (notificationCounter > 0)
        {
            notificationsCounterText.gameObject.SetActive(true);
            UpdateNotificationCounter();
        }
        
        if (notificationCounter <= 0) 
        {
            notificationsCounterText.gameObject.SetActive(false);  // Disable the notification counter when it's zero
            UpdateNotificationCounter(); 
        }
    }

    private int CountReadMessages()
    {
        int readMessages = 0;
        foreach (var readStatus in messageReadStatus)
        {
            readMessages += readStatus.Value.FindAll(r => r).Count; // Count all 'true' values indicating messages that have been read
        }
        return readMessages;
    }

    private void InitializeFriendMessageCounters()
    {
        // Load the message counter for each friend from PlayerPrefs, defaulting to 1 if it hasn't been saved yet
        friendMessageCounters[0] = LoadMessageCounter(0, 1);  // For friend A
        friendMessageCounters[1] = LoadMessageCounter(1, 1);  // For friend B
        friendMessageCounters[2] = LoadMessageCounter(2, 1);  // For friend C 
        friendMessageCounters[3] = LoadMessageCounter(3, 1);  // For Friend D 
        // ... Initialize counters for other friends as needed ...
    }

    private void InitializeMessageReadStatus()
    {
        // Initialize the read status for each friend (load from PlayerPrefs)
        messageReadStatus[0] = LoadMessageReadStatus(0, friendAMessagesSpring.Count);
        messageReadStatus[1] = LoadMessageReadStatus(1, friendBMessagesSpring.Count);
        messageReadStatus[2] = LoadMessageReadStatus(2, friendCMessagesSpring.Count); 
        messageReadStatus[3] = LoadMessageReadStatus(3, friendDMessagesSpring.Count);  // For Friend D 
        // ... Initialize for other friends as needed ...
    }

    private void SaveMessageCounter(int friendIndex, int counterValue)
    {
        // Save the message counter to PlayerPrefs, using the friend index to differentiate between friends
        PlayerPrefs.SetInt("FriendMessageCounter_" + friendIndex, counterValue);
        PlayerPrefs.Save();
    }

    private int LoadMessageCounter(int friendIndex, int defaultValue)
    {
        // Load the message counter from PlayerPrefs, defaulting to 1 if it's not found
        return PlayerPrefs.GetInt("FriendMessageCounter_" + friendIndex, defaultValue);
    }

    private void SaveNotificationCounter()
    {
        PlayerPrefs.SetInt("NotificationCounter", notificationCounter);
        PlayerPrefs.Save();
    }

    private int LoadNotificationCounter()
    {
        return PlayerPrefs.GetInt("NotificationCounter", 0);
    }

    public void ResetNotificationCounter() 
    {
        // Set the notification counter to the total number of messages for the day
        //notificationCounter = 0;   
        SaveNotificationCounter();
        UpdateNotificationCounter();
    } 

    private void SaveMessageReadStatus(int friendIndex, List<bool> readStatus)
    {
        for (int i = 0; i < readStatus.Count; i++)
        {
            PlayerPrefs.SetInt($"MessageReadStatus_{friendIndex}_{i}", readStatus[i] ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    private List<bool> LoadMessageReadStatus(int friendIndex, int messageCount)
    {
        List<bool> readStatus = new List<bool>();

        for (int i = 0; i < messageCount; i++)
        {
            bool isRead = PlayerPrefs.GetInt($"MessageReadStatus_{friendIndex}_{i}", 0) == 1;
            readStatus.Add(isRead);
        }

        return readStatus;
    }

    // New method to reset all PlayerPrefs data related to this script
    private void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();  // Clears all PlayerPrefs data
        PlayerPrefs.Save();

        Debug.Log("All PlayerPrefs data has been reset.");

        // Reinitialize the counters and read status after resetting
        notificationCounter = 2; 
        SaveNotificationCounter(); 
        InitializeNotificationCounter(); 
        InitializeMessageReadStatus();
        UpdateNotificationCounter();
    }

    public void OpenMessagePanel(int friendIndex)
    {
        // Ensure the message panel is active
        messagePanel.SetActive(true);

        // Update the selected friend index correctly
        selectedFriendIndex = friendIndex;  // This sets the friend correctly

        // Retrieve the preselected message for the current day based on the friend index
        List<Message> currentMessages = GetMessagesForCurrentSeason(friendIndex);  // This should retrieve the correct friend messages

        // Populate the message panel with the retrieved messages
        PopulateMessagePanel(currentMessages);
    } 

    public void CloseMessagePanel()
    {
        messagePanel.SetActive(false);
    }

    public void PopulateMessagePanel(List<Message> messages)
    {
        // Clear any existing messages in the container
        foreach (Transform child in messageContainer)
        {
            Destroy(child.gameObject);
        }

        // Randomly shuffle the messages and select 1
        List<Message> randomMessages = GetRandomMessages(messages, 1);

        messageCounter = randomMessages.Count;

        for (int i = 0; i < randomMessages.Count; i++)
        {
            GameObject messageUI = Instantiate(messageUIPrefab, messageContainer);

            Image messageImage = messageUI.transform.Find("MessageImage").GetComponent<Image>();
            TextMeshProUGUI messageText = messageUI.transform.Find("MessageText").GetComponent<TextMeshProUGUI>();

            messageImage.sprite = randomMessages[i].characterImage;
            messageText.text = randomMessages[i].activityName;

            // Determine the season and set text for remaining days
            CalendarSystem.Season currentSeason = CalendarSystem.Instance.GetCurrentSeason();
            int daysLeft = GetDaysLeftInCurrentSeason(currentSeason);

            if (IsMessagePartOfSeason(randomMessages[i], currentSeason))
            {
                daysLeftText.text = $"Days left: {daysLeft}";  
            } 

            AddClickListener(messageUI, randomMessages[i], i);

            // Check if the message has been read and apply the shrunken/darkened effect if necessary
            if (messageReadStatus[selectedFriendIndex][i])
            {
                ShrinkAndDarkenMessage(messageUI);
            }
        }

        alreadyRead = true;
        UpdateMessageCounter();
    }

    // Helper function to determine if a message belongs to a specific season
    private bool IsMessagePartOfSeason(Message message, CalendarSystem.Season season)
    {
        switch (season)
        {
            case CalendarSystem.Season.Spring:
                return friendAMessagesSpring.Contains(message) || friendBMessagesSpring.Contains(message) ||
                       friendCMessagesSpring.Contains(message) || friendDMessagesSpring.Contains(message);
            case CalendarSystem.Season.Summer:
                return friendAMessagesSummer.Contains(message) || friendBMessagesSummer.Contains(message) ||
                       friendCMessagesSummer.Contains(message) || friendDMessagesSummer.Contains(message);
            case CalendarSystem.Season.Fall:
                return friendAMessagesFall.Contains(message) || friendBMessagesFall.Contains(message) ||
                       friendCMessagesFall.Contains(message) || friendDMessagesFall.Contains(message);
            case CalendarSystem.Season.Winter:
                return friendAMessagesWinter.Contains(message) || friendBMessagesWinter.Contains(message) ||
                       friendCMessagesWinter.Contains(message) || friendDMessagesWinter.Contains(message);
            default:
                return false;
        }
    } 

    // Helper method to get random messages
    private List<Message> GetRandomMessages(List<Message> messages, int count)
    {
        List<Message> randomizedMessages = new List<Message>(messages);
        // Shuffle the messages
        for (int i = randomizedMessages.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            // Swap elements
            Message temp = randomizedMessages[i];
            randomizedMessages[i] = randomizedMessages[randomIndex];
            randomizedMessages[randomIndex] = temp;
        }

        // Return only the requested number of messages (up to 1)
        return randomizedMessages.GetRange(0, Mathf.Min(count, randomizedMessages.Count));
    }

    private void AddClickListener(GameObject messageUI, Message message, int messageIndex)
    {
        Button messageButton = messageUI.GetComponent<Button>();
        if (messageButton != null)
        {
            messageButton.onClick.AddListener(() => OnMessageClick(messageUI, message, messageIndex));
        }
    }

    private void OnMessageClick(GameObject messageUI, Message message, int messageIndex)
    {
        if (!messageReadStatus[selectedFriendIndex][messageIndex])
        {
            // Mark message as read
            messageReadStatus[selectedFriendIndex][messageIndex] = true;
            SaveMessageReadStatus(selectedFriendIndex, messageReadStatus[selectedFriendIndex]);

            // Reduce the notification counter
            notificationCounter--;
            SaveNotificationCounter();

            // Set the friend message counter to 0 and hide the text for that friend profile
            friendMessageCounters[selectedFriendIndex] = 0;
            messageCounterText.gameObject.SetActive(false); 
        }

        ShrinkAndDarkenMessage(messageUI);
        ShowMessageDetails(message);
        UpdateNotificationCounter();
    } 

    private void ShrinkAndDarkenMessage(GameObject messageUI)
    {
        Transform messageTransform = messageUI.transform;
        messageTransform.localScale = new Vector3(0.9f, 0.9f, 0.9f);

        Image messageImage = messageUI.transform.Find("MessageImage").GetComponent<Image>();
        TextMeshProUGUI messageText = messageUI.transform.Find("MessageText").GetComponent<TextMeshProUGUI>();

        Color darkerColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        messageImage.color = darkerColor;
        messageText.color = darkerColor;
    }

    private void ShowMessageDetails(Message message)
    {
        detailCharacterImage.sprite = message.characterFullImage;
        detailActivityName.text = message.activityContext;
        messageDetailPanel.SetActive(true);

        // Clear previous listeners
        acceptButton.onClick.RemoveAllListeners();

        switch (message.activityName.Trim())  
        {
            case "Theater Activity":
                Debug.Log("Assigning Theater Activity method");
                acceptButton.onClick.AddListener(() => OnMeiTheaterActivity());
                break;
            case "Swimming Activity":
                Debug.Log("Assigning Swimming Activity method");
                acceptButton.onClick.AddListener(() => OnMeiSwimmingActivity());
                break;
            case "Rehearsal Activity":
                Debug.Log("Assigning Rehearsal Activity method");
                acceptButton.onClick.AddListener(() => OnMeiRehearsalActivity());
                break;
            case "Major #1":
                Debug.Log("Assigning Major #1 method");
                acceptButton.onClick.AddListener(() => OnMeiHangOutTwo());
                break;
            case "Major #2":
                Debug.Log("Assigning Major #2 method");
                acceptButton.onClick.AddListener(() => OnMeiHangOutThree());
                break;
            case "Major #3":
                Debug.Log("Assigning Major #3 method");
                acceptButton.onClick.AddListener(() => OnMeiHangOutFour());
                break;
            case "Major #4":
                Debug.Log("Assigning Major #4 method");
                acceptButton.onClick.AddListener(() => OnMeiHangOutFive());
                break;
            case "Major #5":
                Debug.Log("Assigning Major #5 method");
                acceptButton.onClick.AddListener(() => OnMeiHangOutSix());
                break;
            case "Major #6":
                Debug.Log("Assigning Major #6 method");
                acceptButton.onClick.AddListener(() => OnMeiHangOutSeven());
                break;
            case "Major #7":
                Debug.Log("Assigning Major #7 method");
                acceptButton.onClick.AddListener(() => OnMeiHangOutEight());
                break;
            case "Major #8":
                Debug.Log("Assigning Major #8 method");
                acceptButton.onClick.AddListener(() => OnMeiHangOutNine());
                break;
            case "Major #9":
                Debug.Log("Assigning Major #9 method");
                acceptButton.onClick.AddListener(() => OnMeiHangOutTen()); 
                break;
            case "Company?":
                Debug.Log("Assigning Company method"); 
                acceptButton.onClick.AddListener(() => NinaCompany()); 
                break;
            case "Critter Trails?":
                Debug.Log("Assigning Critter Trails method"); 
                acceptButton.onClick.AddListener(() => AlexCritterTrails()); 
                break;
            case "Help Me?": 
                Debug.Log("Assigning Help Me method");
                acceptButton.onClick.AddListener(() => SimonHelpMe()); 
                break;
            default:
                Debug.LogWarning("No method assigned for this activity: " + message.activityName);
                break;
        } 

        // Clear and add the listener for the decline button
        declineButton.onClick.RemoveAllListeners();
        declineButton.onClick.AddListener(CloseMessageDetailPanel);
    }

    public void OnMeiTheaterActivity()
    {
        HandleHangOutClick("MoviesFileMei");
        //Debug.Log("Movie Theater Activity");
    }
    public void OnMeiSwimmingActivity()
    {
        HandleHangOutClick("PoolFileMei");
        //Debug.Log("Swimming Activity");
    }
    public void OnMeiRehearsalActivity()
    {
        HandleHangOutClick("RehearsalFileMei");
        //Debug.Log("Rehearsal Activity");
    } 

    public void OnMeiHangOutTwo()
    {
        HandleHangOutClick("MeiHangOutTwo"); 
    }

    public void OnMeiHangOutThree()
    {
        HandleHangOutClick("MeiHangOutThree");
    }

    public void OnMeiHangOutFour()
    {
        HandleHangOutClick("MeiHangOutFour");
    }

    public void OnMeiHangOutFive()
    {
        HandleHangOutClick("MeiHangOutFive");
    }

    public void OnMeiHangOutSix()
    {
        HandleHangOutClick("MeiHangOutSix");
    }

    public void OnMeiHangOutSeven()
    {
        HandleHangOutClick("MeiHangOutSeven");
    }

    public void OnMeiHangOutEight()
    {
        HandleHangOutClick("MeiHangOutEight");
    }

    public void OnMeiHangOutNine()
    {
        HandleHangOutClick("MeiHangOutNine");
    }

    public void OnMeiHangOutTen()
    {
        HandleHangOutClick("MeiHangOutTen");
    }

    public void NinaCompany()  
    {
        HandleHangOutClick("NinaCompany"); 
    } 

    public void AlexCritterTrails()
    {
        HandleHangOutClick("AlexCritterTrails"); 
    }

    public void SimonHelpMe()
    {
        HandleHangOutClick("SimonUniversity"); 
    }

    private void HandleHangOutClick(string locationFile)
    { 
        // After marking the Hangout as complete, retry the rank-up
        string friendName = locationFile.Replace("HangOut", "").Replace("File", "").Replace("Mei", "Mei").Replace("Alex", "Alex").Replace("Nina", "Nina").Replace("Simon", "Simon");

        var friendStat = FriendshipStats.instance.GetFriendshipStat(friendName);
        if (friendStat != null)
        {
            friendStat.RetryRankCheck();
        }

        // Now load the Visual Novel scene
        PlayerPrefs.SetString("StartingFile", locationFile);
        PlayerPrefs.Save();
        SceneManager.LoadScene("VisualNovel");
    } 

    private void CloseMessageDetailPanel()
    {
        messageDetailPanel.SetActive(false);
    }

    private void UpdateMessageCounter()
    {
        if (friendMessageCounters.ContainsKey(selectedFriendIndex))
        {
            messageCounterText.text = $"{friendMessageCounters[selectedFriendIndex]}";
            messageCounterText.gameObject.SetActive(friendMessageCounters[selectedFriendIndex] > 0);
        }
    } 

    private void UpdateNotificationCounter()
    {
        if (notificationCounter > 0)
        {
            notificationsCounterText.gameObject.SetActive(true);
            notificationsCounterText.text = $"{notificationCounter}";
        }
        else
        {
            notificationsCounterText.gameObject.SetActive(false);  // Hide when no notifications
        }
    }

    public void SetSelectedFriend(int friendIndex)
    {
        selectedFriendIndex = friendIndex;
        UpdateMessageCounter();
    } 

    // Retrieve the selected message for a specific friend
    public List<Message> GetMessagesForCurrentSeason(int friendIndex)
    {
        List<Message> selectedMessages = new List<Message>();

        if (!messagesSelectedForDay)
        {
            SelectMessagesForCurrentDay();
        }

        switch (friendIndex)
        {
            case 0: // Friend A
                if (selectedFriendAMessage != null)
                {
                    if (FriendshipStats.instance.FriendA.Level >= 10 && !alreadyAcceptedMeiHangOutTen) 
                    {
                        selectedMessages.Add(selectedFriendAMessage);
                        selectedFriendAMessage = friendAMessagesMajor[8];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendA.Level >= 9 && !alreadyAcceptedMeiHangOutNine)
                    {
                        selectedMessages.Add(selectedFriendAMessage);
                        selectedFriendAMessage = friendAMessagesMajor[7];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendA.Level >= 8 && !alreadyAcceptedMeiHangOutEight)
                    {
                        selectedMessages.Add(selectedFriendAMessage);
                        selectedFriendAMessage = friendAMessagesMajor[6];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendA.Level >= 7 && !alreadyAcceptedMeiHangOutSeven)
                    {
                        selectedMessages.Add(selectedFriendAMessage);
                        selectedFriendAMessage = friendAMessagesMajor[5];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendA.Level >= 6 && !alreadyAcceptedMeiHangOutSix) 
                    {
                        selectedMessages.Add(selectedFriendAMessage);
                        selectedFriendAMessage = friendAMessagesMajor[4];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendA.Level >= 5 && !alreadyAcceptedMeiHangOutFive)
                    {
                        selectedMessages.Add(selectedFriendAMessage);
                        selectedFriendAMessage = friendAMessagesMajor[3];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendA.Level >= 4 && !alreadyAcceptedMeiHangOutFour)
                    {
                        selectedMessages.Add(selectedFriendAMessage);
                        selectedFriendAMessage = friendAMessagesMajor[2];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendA.Level >= 3 && !alreadyAcceptedMeiHangOutThree)
                    {
                        selectedMessages.Add(selectedFriendAMessage);
                        selectedFriendAMessage = friendAMessagesMajor[1];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendA.Level >= 2 && !alreadyAcceptedMeiHangOutTwo)   
                    { 
                        selectedMessages.Add(selectedFriendAMessage);
                        selectedFriendAMessage = friendAMessagesMajor[0];
                        daysLeftText.text = "Days left: No Time Limit";  
                    } 
                    else
                        selectedMessages.Add(selectedFriendAMessage);
                    //if (alreadyAcceptedHangOutTwo)  
                    //selectedMessages.Add(selectedFriendAMessage); 
                }
                break;
            case 1: // Friend B
                if (selectedFriendBMessage != null)
                {
                    if (FriendshipStats.instance.FriendB.Level >= 10 && !alreadyAcceptedAlexHangOutTen)
                    {
                        selectedMessages.Add(selectedFriendBMessage);
                        selectedFriendBMessage = friendBMessagesMajor[8];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendB.Level >= 9 && !alreadyAcceptedAlexHangOutNine)
                    {
                        selectedMessages.Add(selectedFriendBMessage);
                        selectedFriendBMessage = friendBMessagesMajor[7];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendB.Level >= 8 && !alreadyAcceptedAlexHangOutEight)
                    {
                        selectedMessages.Add(selectedFriendBMessage);
                        selectedFriendBMessage = friendBMessagesMajor[6];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendB.Level >= 7 && !alreadyAcceptedAlexHangOutSeven)
                    {
                        selectedMessages.Add(selectedFriendBMessage);
                        selectedFriendBMessage = friendBMessagesMajor[5];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendB.Level >= 6 && !alreadyAcceptedAlexHangOutSix)
                    {
                        selectedMessages.Add(selectedFriendBMessage);
                        selectedFriendBMessage = friendBMessagesMajor[4];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendB.Level >= 5 && !alreadyAcceptedAlexHangOutFive)
                    {
                        selectedMessages.Add(selectedFriendBMessage);
                        selectedFriendBMessage = friendBMessagesMajor[3];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendB.Level >= 4 && !alreadyAcceptedAlexHangOutFour)
                    {
                        selectedMessages.Add(selectedFriendBMessage);
                        selectedFriendBMessage = friendBMessagesMajor[2];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendB.Level >= 3 && !alreadyAcceptedAlexHangOutThree)
                    {
                        selectedMessages.Add(selectedFriendBMessage);
                        selectedFriendBMessage = friendBMessagesMajor[1];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendB.Level >= 2 && !alreadyAcceptedAlexHangOutTwo)
                    {
                        selectedMessages.Add(selectedFriendBMessage);
                        selectedFriendBMessage = friendBMessagesMajor[0];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    else
                        selectedMessages.Add(selectedFriendBMessage); 
                }
                break; 
            case 2: // Friend C 
                if (selectedFriendCMessage != null) 
                {
                    if (FriendshipStats.instance.FriendC.Level >= 10 && !alreadyAcceptedNinaHangOutTen)
                    {
                        selectedMessages.Add(selectedFriendCMessage);
                        selectedFriendCMessage = friendCMessagesMajor[8];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendC.Level >= 9 && !alreadyAcceptedNinaHangOutNine)
                    {
                        selectedMessages.Add(selectedFriendCMessage); 
                        selectedFriendCMessage = friendCMessagesMajor[7];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendC.Level >= 8 && !alreadyAcceptedNinaHangOutEight)
                    {
                        selectedMessages.Add(selectedFriendCMessage);
                        selectedFriendCMessage = friendCMessagesMajor[6];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendC.Level >= 7 && !alreadyAcceptedNinaHangOutSeven)
                    {
                        selectedMessages.Add(selectedFriendCMessage);
                        selectedFriendCMessage = friendCMessagesMajor[5];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendC.Level >= 6 && !alreadyAcceptedNinaHangOutSix)
                    {
                        selectedMessages.Add(selectedFriendCMessage);
                        selectedFriendCMessage = friendCMessagesMajor[4];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendC.Level >= 5 && !alreadyAcceptedNinaHangOutFive)
                    {
                        selectedMessages.Add(selectedFriendCMessage);
                        selectedFriendCMessage = friendCMessagesMajor[3];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendC.Level >= 4 && !alreadyAcceptedNinaHangOutFour)
                    {
                        selectedMessages.Add(selectedFriendCMessage);
                        selectedFriendCMessage = friendCMessagesMajor[2];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendC.Level >= 3 && !alreadyAcceptedNinaHangOutThree)
                    {
                        selectedMessages.Add(selectedFriendCMessage);
                        selectedFriendCMessage = friendCMessagesMajor[1];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendC.Level >= 2 && !alreadyAcceptedNinaHangOutTwo)
                    {
                        selectedMessages.Add(selectedFriendCMessage);
                        selectedFriendCMessage = friendCMessagesMajor[0];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    else
                        selectedMessages.Add(selectedFriendCMessage); 
                } 
                break;
            case 3: // Friend D
                if (selectedFriendDMessage != null)
                {
                    if (FriendshipStats.instance.FriendD.Level >= 10 && !alreadyAcceptedSimonHangOutTen)
                    {
                        selectedMessages.Add(selectedFriendDMessage);
                        selectedFriendDMessage = friendDMessagesMajor[8];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendD.Level >= 9 && !alreadyAcceptedSimonHangOutNine)
                    {
                        selectedMessages.Add(selectedFriendDMessage);
                        selectedFriendDMessage = friendDMessagesMajor[7];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendD.Level >= 8 && !alreadyAcceptedSimonHangOutEight)
                    {
                        selectedMessages.Add(selectedFriendDMessage);
                        selectedFriendDMessage = friendDMessagesMajor[6];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendD.Level >= 7 && !alreadyAcceptedSimonHangOutSeven)
                    {
                        selectedMessages.Add(selectedFriendDMessage);
                        selectedFriendDMessage = friendDMessagesMajor[5];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendD.Level >= 6 && !alreadyAcceptedSimonHangOutSix)
                    {
                        selectedMessages.Add(selectedFriendDMessage);
                        selectedFriendDMessage = friendDMessagesMajor[4];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendD.Level >= 5 && !alreadyAcceptedSimonHangOutFive)
                    {
                        selectedMessages.Add(selectedFriendDMessage);
                        selectedFriendDMessage = friendDMessagesMajor[3];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendD.Level >= 4 && !alreadyAcceptedSimonHangOutFour)
                    {
                        selectedMessages.Add(selectedFriendDMessage);
                        selectedFriendDMessage = friendDMessagesMajor[2];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendD.Level >= 3 && !alreadyAcceptedSimonHangOutThree)
                    {
                        selectedMessages.Add(selectedFriendDMessage);
                        selectedFriendDMessage = friendDMessagesMajor[1];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    if (FriendshipStats.instance.FriendD.Level >= 2 && !alreadyAcceptedSimonHangOutTwo)
                    {
                        selectedMessages.Add(selectedFriendDMessage);
                        selectedFriendDMessage = friendDMessagesMajor[0];
                        daysLeftText.text = "Days left: No Time Limit";
                    }
                    else
                        selectedMessages.Add(selectedFriendDMessage);
                } 
                break;
            default:
                Debug.LogWarning("Invalid friend index: " + friendIndex);
                break; 
        }

        return selectedMessages;
    } 

    // Helper function to get one random message from the correct season list
    private Message GetRandomMessageForSeason(List<Message> spring, List<Message> summer, List<Message> fall, List<Message> winter, CalendarSystem.Season season)
    {
        List<Message> seasonMessages = null;

        switch (season)
        {
            case CalendarSystem.Season.Spring:
                seasonMessages = spring;
                break;
            case CalendarSystem.Season.Summer:
                seasonMessages = summer;
                break;
            case CalendarSystem.Season.Fall:
                seasonMessages = fall;
                break;
            case CalendarSystem.Season.Winter:
                seasonMessages = winter;
                break;
        }

        if (seasonMessages != null && seasonMessages.Count > 0)
        {
            return seasonMessages[Random.Range(0, seasonMessages.Count)];
        }

        return null; // Return null if no messages are found
    } 
} 

