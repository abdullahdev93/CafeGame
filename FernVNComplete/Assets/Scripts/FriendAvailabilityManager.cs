using System.Collections.Generic;
using UnityEngine;

public class FriendAvailabilityManager : MonoBehaviour
{
    public static FriendAvailabilityManager Instance;

    [System.Serializable]
    public class FriendAvailability
    {
        public string friendName;
        public List<CalendarSystem.Season> availableSeasons;
        public string[] availableActivities; // New: Stores activities like "Jog", "Birdwatching" 
        public string[] availableWeekdays;
        public string[] availableActivityPhases;
        public List<ActivityFileEntry> activityFileEntries;
    }

    [System.Serializable]
    public class ActivityFileEntry
    {
        public string activityType; // New: Jog, Birdwatching, etc. 
        public List<CalendarSystem.Season> seasons;
        public List<string> activityPhases;
        public List<CalendarSystem.Weather> requiredWeatherConditions;
        public List<StatCondition> requiredStats; // All of these must be met 

        [Header("Friendship Level Conditions")]
        public int requiredFriendshipLevel = -1; // -1 means ignore this condition     

        public List<TextAsset> eligibleFiles; // Used only if all conditions are met
        public List<TextAsset> fallbackFiles; // Used if any condition fails
    }

    [System.Serializable]
    public class StatCondition
    {
        public string statName;                 // e.g., "Confidence", "Intelligence"
        public int statThreshold = 6;           // Minimum level required
    }

    public List<FriendAvailability> friendsAvailabilityList = new List<FriendAvailability>();

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

    public void SelectActivity(string activity)
    {
        Debug.Log("Selecting Activity: " + activity);

        ActivityTracker.Instance.SetActivity(activity);

        string updatedActivity = activity;
        Debug.Log("Activity Updated to: " + updatedActivity);

        Dictionary<string, string> availableFriends = GetAvailableFriends(
            CalendarSystem.Instance.GetCurrentSeason(),
            updatedActivity,
            CalendarSystem.Instance.activityPhases[CalendarSystem.Instance.activityCounter]
        );

        ChoicePanel.instance.ShowInviteMenu();
    }

    private string GetActivityFileForFriend(FriendAvailability friend, string activityType, CalendarSystem.Season season, string activityPhase)
    {
        CalendarSystem.Weather currentWeather = CalendarSystem.Instance.GetCurrentWeather();

        int bestMatchScore = -1;
        List<TextAsset> bestFilePool = null;

        foreach (var entry in friend.activityFileEntries)
        {
            if (entry.activityType != activityType)
                continue;

            int matchScore = 0;

            if (entry.seasons != null && entry.seasons.Contains(season))
                matchScore++;

            if (entry.activityPhases != null && entry.activityPhases.Contains(activityPhase))
                matchScore++;

            bool weatherMatch = (entry.requiredWeatherConditions == null || entry.requiredWeatherConditions.Count == 0 || entry.requiredWeatherConditions.Contains(currentWeather));
            if (weatherMatch)
                matchScore++;

            bool statConditionsMet = true;
            foreach (var statCondition in entry.requiredStats)
            {
                var statProperty = typeof(PlayerStats).GetProperty(statCondition.statName);
                PlayerStat selectedStat = statProperty != null ? statProperty.GetValue(PlayerStats.instance) as PlayerStat : null;

                if (selectedStat == null || selectedStat.Level < statCondition.statThreshold)
                {
                    statConditionsMet = false;
                    break;
                }
            }
            if (statConditionsMet && entry.requiredStats.Count > 0)
                matchScore++;

            bool friendshipMatch = true;
            if (entry.requiredFriendshipLevel >= 0)
            {
                FriendshipStat friendshipStat = FriendshipStats.instance.GetFriendshipStat(friend.friendName);
                if (friendshipStat == null || friendshipStat.Level < entry.requiredFriendshipLevel)
                {
                    friendshipMatch = false;
                }
            }
            if (friendshipMatch && entry.requiredFriendshipLevel >= 0)
                matchScore++;

            if (matchScore > bestMatchScore && entry.eligibleFiles != null && entry.eligibleFiles.Count > 0)
            {
                bestMatchScore = matchScore;
                bestFilePool = entry.eligibleFiles;
            }
            else if (bestFilePool == null && entry.fallbackFiles != null && entry.fallbackFiles.Count > 0)
            {
                bestFilePool = entry.fallbackFiles;
            }
        }

        return GetRandomFile(bestFilePool);
    }


    private string GetRandomFile(List<TextAsset> filePool)
    {
        if (filePool != null && filePool.Count > 0)
        {
            int randomIndex = Random.Range(0, filePool.Count);
            return filePool[randomIndex]?.name; // Return actual file content
        }

        return null;
    }

    public Dictionary<string, string> GetAvailableFriends(CalendarSystem.Season currentSeason, string currentWeekday, string currentActivityPhase)
    {
        Dictionary<string, string> availableFriends = new Dictionary<string, string>();

        string currentActivity = ActivityTracker.Instance.GetSelectedActivity();

        foreach (var friend in friendsAvailabilityList)
        {
            bool activityMatch = System.Array.Exists(friend.availableActivities, activity => activity == currentActivity);
            bool seasonMatch = friend.availableSeasons.Contains(currentSeason);
            bool phaseMatch = System.Array.Exists(friend.availableActivityPhases, phase => phase == currentActivityPhase);

            if (activityMatch && seasonMatch && phaseMatch)
            {
                string assignedFile = GetActivityFileForFriend(friend, currentActivity, currentSeason, currentActivityPhase);
                if (!string.IsNullOrEmpty(assignedFile))
                {
                    availableFriends.Add(friend.friendName, assignedFile);
                }
            }
        }

        return availableFriends;
    }
}
