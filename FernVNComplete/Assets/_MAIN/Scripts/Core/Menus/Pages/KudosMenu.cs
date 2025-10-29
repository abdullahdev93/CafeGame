using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KudosMenu : MenuPage
{
    public static KudosMenu Instance;

    [Header("Achievement Tracking")]
    public GameObject kudosParent; // Reference to the "Kudos" GameObject
    public List<AccomplishmentPrefab> accomplishments = new List<AccomplishmentPrefab>(); // Explicit list of prefabs
    public int totalAchievements => accomplishments.Count; // Total number of achievements (dynamic)
    public int accomplishedTasks = 0; // Number of tasks accomplished
    public TextMeshProUGUI percentageText; // UI text to display the percentage of accomplishments

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Populate the accomplishments list if kudosParent is assigned
        InitializeAccomplishments();

        // Calculate the initial state of accomplished tasks
        UpdateAccomplishedTasks();
    }

    private void Update()
    {
        // Populate the accomplishments list if kudosParent is assigned
        InitializeAccomplishments();

        // Calculate the initial state of accomplished tasks
        UpdateAccomplishedTasks();
    }

    public void InitializeAccomplishments()
    {
        // Clear existing list
        accomplishments.Clear();

        if (kudosParent != null)
        {
            // Populate the list with all AccomplishmentPrefabs under the kudosParent
            accomplishments.AddRange(kudosParent.GetComponentsInChildren<AccomplishmentPrefab>(true));
        }
        else
        {
            Debug.LogError("Kudos Parent GameObject is not assigned!");
        }
    }

    public void UpdateAccomplishedTasks()
    {
        if (accomplishments.Count == 0)
        {
            Debug.LogWarning("No accomplishments found!");
            accomplishedTasks = 0;
            UpdateAchievementPercentage();
            return;
        }

        // Count accomplished tasks
        accomplishedTasks = 0;
        foreach (var accomplishment in accomplishments)
        {
            if (accomplishment.isAccomplished)
            {
                accomplishedTasks++;
            }
        }

        // Update the UI
        UpdateAchievementPercentage();
    }

    public void UpdateAchievementPercentage()
    {
        if (percentageText != null)
        {
            int percentage = totalAchievements > 0
                ? Mathf.RoundToInt((float)accomplishedTasks / totalAchievements * 100)
                : 0;

            percentageText.text = $"Completion: {percentage} %"; 
        }
    }
}
