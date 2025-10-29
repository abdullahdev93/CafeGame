using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class FriendshipStat
{
    public string name;
    private int level;
    private int points;
    private int pointsForNextLevel;

    private bool hasPendingRankUp = false;

    public int Level
    {
        get { return level; }
        set
        {
            level = Mathf.Clamp(value, 0, 10);
            SaveStat(); // Save the level and points to PlayerPrefs
        }
    }

    public int Points
    {
        get => points;
        set
        {
            // Block adding more points if a rank-up is pending
            if (hasPendingRankUp)
            {
                Debug.Log($"[FriendshipStat] Cannot add more points to {name} until they rank up.");
                return;
            }

            points = value;
            CheckLevelUp(); // This sets hasPendingRankUp if threshold is reached
            SaveStat();
        }
    }

    /*public int Points
    {
        get { return points; }
        set
        {
            points = value;
            CheckLevelUp();
            SaveStat(); // Save the level and points to PlayerPrefs
        }
    }*/

    public int PointsForNextLevel
    {
        get { return pointsForNextLevel; }
        private set { pointsForNextLevel = value; }
    }

    public FriendshipStat(string name)
    {
        this.name = name;
        // Load level and points from PlayerPrefs
        //this.level = PlayerPrefs.GetInt(name + "_Level", 1);
        this.level = PlayerPrefs.GetInt(name + "_Level", 0);
        this.points = PlayerPrefs.GetInt(name + "_Points", 0);
        this.pointsForNextLevel = PlayerPrefs.GetInt(name + "_PointsForNextLevel", 10); // Default to 10 points for the first level up 

        RetryRankCheck(); // Ensure hasPendingRankUp is re-checked after loading
    }

    public static Dictionary<string, HashSet<int>> completedHangoutsByCharacter = new Dictionary<string, HashSet<int>>();

    /*private void CheckLevelUp()
    {
        while (points >= pointsForNextLevel)
        {
            LevelUp();
        }
    }*/

    private void CheckLevelUp()
    {
        if (!hasPendingRankUp && points >= pointsForNextLevel && Level < 10)
        {
            hasPendingRankUp = true;
        }
    }

    public bool HasPendingRankUp()
    {
        return hasPendingRankUp && points >= pointsForNextLevel && Level < 10;
    }

    public void ApplyPendingRankUp()
    {
        if (HasPendingRankUp())
        {
            level++;
            points -= pointsForNextLevel;
            pointsForNextLevel += 5;
            hasPendingRankUp = false;
            SaveStat();
        }
    }

    public bool HasCompletedHangoutForNextRank()
    {
        if (completedHangoutsByCharacter.TryGetValue(name, out var completedRanks))
        {
            return completedRanks.Contains(Level + 1);
        }

        return false;
    }

    // Check if enough points have been accumulated for a level up
    //private void CheckLevelUp()
    //{
    //while (points >= pointsForNextLevel)
    //{
    //LevelUp();
    //}
    //} 

    private void LevelUp()
    {
        level++;
        points -= pointsForNextLevel;
        pointsForNextLevel += 5;
        SaveStat();

        //RankUpPage rankUpPage = GameObject.FindObjectOfType<RankUpPage>();
        //if (rankUpPage != null)
        //{
        //rankUpPage.ShowRankUp(this);
        //}
    }

    public void RetryRankCheck()
    {
        CheckLevelUp();
    }

    public void ForceLevelUp()
    {
        if (points >= pointsForNextLevel && HasCompletedHangoutForNextRank())
        {
            level++;
            points -= pointsForNextLevel;
            pointsForNextLevel += 5;
            SaveStat();

            // Immediately update UI
            StatsMenu.instance?.UpdateFriendshipBars();
            StatsMenu.instance?.UpdateAllFriendshipRankTexts();
        }
    }

    // Save the current level, points, and points for the next level to PlayerPrefs
    private void SaveStat()
    {
        PlayerPrefs.SetInt(name + "_Level", level);
        PlayerPrefs.SetInt(name + "_Points", points);
        PlayerPrefs.SetInt(name + "_PointsForNextLevel", pointsForNextLevel);
        PlayerPrefs.Save();
    }
}

public class FriendshipStats : MonoBehaviour
{
    public static FriendshipStats instance { get; private set; }

    public FriendshipStat FriendA { get; private set; }
    public FriendshipStat FriendB { get; private set; }
    public FriendshipStat FriendC { get; private set; }
    public FriendshipStat FriendD { get; private set; }
    public FriendshipStat FriendE { get; private set; }
    public FriendshipStat FriendF { get; private set; }
    public FriendshipStat FriendG { get; private set; }
    public FriendshipStat FriendH { get; private set; }
    public FriendshipStat FriendI { get; private set; }
    public FriendshipStat FriendJ { get; private set; }

    //public Animator statIncreaseAnimator; // Reference to Animator
    //public GameObject statIncreaseTextPrefab; // Prefab for stat increase UI
    //public Transform statIncreaseTextParent; // Parent transform to display UI 

    private void Awake()
    {
        instance = this;
        FriendA = new FriendshipStat("Mei");
        FriendB = new FriendshipStat("Alex");
        FriendC = new FriendshipStat("Nina");
        FriendD = new FriendshipStat("Simon");
        FriendE = new FriendshipStat("Friend E");
        FriendF = new FriendshipStat("Friend F");
        FriendG = new FriendshipStat("Friend G");
        FriendH = new FriendshipStat("Friend H");
        FriendI = new FriendshipStat("Friend I");
        FriendJ = new FriendshipStat("Friend J");

        RegisterFriendStatsToVariableStore();
        FriendA.RetryRankCheck();
        StatsMenu.instance?.UpdateFriendshipBars();

    }

    private void RegisterFriendStatsToVariableStore()
    {
        var dbName = "FriendshipStats";

        VariableStore.CreateVariable(dbName + ".Mei", FriendA.Level, () => FriendA.Level, v => FriendA.Level = v);
        VariableStore.CreateVariable(dbName + ".Alex", FriendB.Level, () => FriendB.Level, v => FriendB.Level = v);
        VariableStore.CreateVariable(dbName + ".Nina", FriendC.Level, () => FriendC.Level, v => FriendC.Level = v);
        VariableStore.CreateVariable(dbName + ".Simon", FriendD.Level, () => FriendD.Level, v => FriendD.Level = v);
        VariableStore.CreateVariable(dbName + ".FriendE", FriendE.Level, () => FriendE.Level, v => FriendE.Level = v);
        VariableStore.CreateVariable(dbName + ".FriendF", FriendF.Level, () => FriendF.Level, v => FriendF.Level = v);
        VariableStore.CreateVariable(dbName + ".FriendG", FriendG.Level, () => FriendG.Level, v => FriendG.Level = v);
        VariableStore.CreateVariable(dbName + ".FriendH", FriendH.Level, () => FriendH.Level, v => FriendH.Level = v);
        VariableStore.CreateVariable(dbName + ".FriendI", FriendI.Level, () => FriendI.Level, v => FriendI.Level = v);
        VariableStore.CreateVariable(dbName + ".FriendJ", FriendJ.Level, () => FriendJ.Level, v => FriendJ.Level = v);
    }

    private void Start()
    {
        // Load the saved friendship levels and update UI
        StatsMenu.instance.UpdateFriendshipBars(); // Update the Friendship Bars when the game starts
        StatsMenu.instance.UpdateAllFriendshipRankTexts(); // Update all friendship rank texts when the game starts
    }

    public void IncreaseStatPoints(FriendshipStat stat, int pointsToAdd)
    {
        stat.Points += pointsToAdd;
        StatsMenu.instance.UpdateFriendshipBars(); // Update Friendship Bars after stat point increase
        StatsMenu.instance.UpdateAllFriendshipRankTexts(); // Update all friendship rank texts after stat point increase 

        // Trigger the animation and show the stat increase UI
        VNMenuManager.instance.PlayFriendIncreaseAnimation(stat.name, pointsToAdd);
    }

    public void IncreaseStat(FriendshipStat stat)
    {
        if (stat.Level < 10)
        {
            stat.Level++;
            StatsMenu.instance.UpdateFriendshipBars(); // Update Friendship Bars after stat increase
            StatsMenu.instance.UpdateAllFriendshipRankTexts(); // Update all friendship rank texts after stat increase
        }
    }

    public void DecreaseStat(FriendshipStat stat)
    {
        if (stat.Level > 0)
        {
            stat.Level--;
            StatsMenu.instance.UpdateFriendshipBars(); // Update Friendship Bars after stat decrease
            StatsMenu.instance.UpdateAllFriendshipRankTexts(); // Update all friendship rank texts after stat decrease
        }
    }

    public void ResetStats()
    {
        // Reset all friendship stats and save them to PlayerPrefs 
        FriendA.Level = 0;
        FriendB.Level = 0;
        FriendC.Level = 0;
        FriendD.Level = 0;
        FriendE.Level = 0;
        FriendF.Level = 0;
        FriendG.Level = 0;
        FriendH.Level = 0;
        FriendI.Level = 0;
        FriendJ.Level = 0;

        /*FriendA.Level = 1;
        FriendB.Level = 1;
        FriendC.Level = 1;
        FriendD.Level = 1;
        FriendE.Level = 1;
        FriendF.Level = 1;
        FriendG.Level = 1;
        FriendH.Level = 1;
        FriendI.Level = 1;
        FriendJ.Level = 1;*/

        FriendA.Points = 0;
        FriendB.Points = 0;
        FriendC.Points = 0;
        FriendD.Points = 0;
        FriendE.Points = 0;
        FriendF.Points = 0;
        FriendG.Points = 0;
        FriendH.Points = 0;
        FriendI.Points = 0;
        FriendJ.Points = 0;

        StatsMenu.instance.UpdateFriendshipBars(); // Update the Friendship Bars after resetting stats
        StatsMenu.instance.UpdateAllFriendshipRankTexts(); // Update all friendship rank texts after resetting stats
    }

    public FriendshipStat GetFriendshipStat(string name)
    {
        switch (name)
        {
            case "Mei": return FriendA;
            case "Alex": return FriendB;
            case "Nina": return FriendC;
            case "Simon": return FriendD;
            case "Friend E": return FriendE;
            case "Friend F": return FriendF;
            case "Friend G": return FriendG;
            case "Friend H": return FriendH;
            case "Friend I": return FriendI;
            case "Friend J": return FriendJ;
            default:
                Debug.LogError("FriendshipStat not found for: " + name);
                return null;
        }
    }

    public void DisplayStats()
    {
        Debug.Log($"Friend A: {FriendA.Level} (Points: {FriendA.Points}/{FriendA.PointsForNextLevel})");
        Debug.Log($"Friend B: {FriendB.Level} (Points: {FriendB.Points}/{FriendB.PointsForNextLevel})");
        Debug.Log($"Friend C: {FriendC.Level} (Points: {FriendC.Points}/{FriendC.PointsForNextLevel})");
        Debug.Log($"Friend D: {FriendD.Level} (Points: {FriendD.Points}/{FriendD.PointsForNextLevel})");
        Debug.Log($"Friend E: {FriendE.Level} (Points: {FriendE.Points}/{FriendE.PointsForNextLevel})");
        Debug.Log($"Friend F: {FriendF.Level} (Points: {FriendF.Points}/{FriendF.PointsForNextLevel})");
        Debug.Log($"Friend G: {FriendG.Level} (Points: {FriendG.Points}/{FriendG.PointsForNextLevel})");
        Debug.Log($"Friend H: {FriendH.Level} (Points: {FriendH.Points}/{FriendH.PointsForNextLevel})");
        Debug.Log($"Friend I: {FriendI.Level} (Points: {FriendI.Points}/{FriendI.PointsForNextLevel})");
        Debug.Log($"Friend J: {FriendJ.Level} (Points: {FriendJ.Points}/{FriendJ.PointsForNextLevel})");
    }
}