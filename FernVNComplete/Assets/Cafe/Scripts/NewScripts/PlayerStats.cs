using System.Collections;
using TMPro;
using UnityEngine;

[System.Serializable]
public class PlayerStat
{
    public string name;
    private int level;
    private int points;
    private int pointsForNextLevel;

    // New flag to track if the level-up text has been shown
    private bool levelUpTextShown;

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
        get { return points; }
        set
        {
            points = value;
            CheckLevelUp();
            SaveStat(); // Save the level and points to PlayerPrefs
        }
    }

    public int PointsForNextLevel
    {
        get { return pointsForNextLevel; }
        private set { pointsForNextLevel = value; }
    }

    public PlayerStat(string name)
    {
        this.name = name;
        // Load level and points from PlayerPrefs
        this.level = PlayerPrefs.GetInt(name + "_Level", 1);

        // Ensure Karma starts at 50 instead of default 0
        this.points = (name == "Karma") ? Mathf.Max(50, PlayerPrefs.GetInt(name + "_Points", 0)) : PlayerPrefs.GetInt(name + "_Points", 0);
        this.pointsForNextLevel = PlayerPrefs.GetInt(name + "_PointsForNextLevel", 5); // Default to 5 points for the first level up 
        this.levelUpTextShown = PlayerPrefs.GetInt(name + "_LevelUpTextShown", 0) == 1; // Load the flag 
    }

    // Check if enough points have been accumulated for a level up
    private void CheckLevelUp()
    {
        while (points >= pointsForNextLevel)
        {
            LevelUp();
        }
    }

    // Increase level and adjust points for the next level
    private void LevelUp()
    {
        level++;
        points -= pointsForNextLevel; // Carry over any excess points
        pointsForNextLevel += 5; // Increase the requirement for the next level
        levelUpTextShown = false; // Reset the flag when the stat levels up 
        SaveStat(); // Save the new level, points, and points required for the next level 

        // Notify the StatsMenu to update bars and trigger the flickering effect
        StatsMenu.instance.UpdateStatsBars();
    }

    // Save the current level, points, and points for the next level to PlayerPrefs
    private void SaveStat()
    {
        PlayerPrefs.SetInt(name + "_Level", level);
        PlayerPrefs.SetInt(name + "_Points", points);
        PlayerPrefs.SetInt(name + "_PointsForNextLevel", pointsForNextLevel);
        PlayerPrefs.SetInt(name + "_LevelUpTextShown", levelUpTextShown ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Method to check if the level-up text has been shown
    public bool IsLevelUpTextShown()
    {
        return levelUpTextShown;
    }

    // Method to mark that the level-up text has been shown
    public void SetLevelUpTextShown()
    {
        levelUpTextShown = true;
        SaveStat(); // Save the flag status
    }
}

[System.Serializable]
public class PronounSet
{
    public string Subject; // he, she, they
    public string Object; // him, her, them
    public string PossessiveAdjective; // his, her, their
    public string PossessivePronoun; // his, hers, theirs
    public string Reflexive; // himself, herself, themself

    public PronounSet(string subject, string obj, string possessiveAdj, string possessivePro, string reflexive)
    {
        Subject = subject;
        Object = obj;
        PossessiveAdjective = possessiveAdj;
        PossessivePronoun = possessivePro;
        Reflexive = reflexive;
    }
}

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance { get; private set; }

    public PlayerStat Charisma { get; private set; }
    public PlayerStat Creativity { get; private set; }
    public PlayerStat Empathy { get; private set; }
    public PlayerStat Humor { get; private set; }
    public PlayerStat Endurance { get; private set; }
    public PlayerStat Patience { get; private set; }
    public PlayerStat Intelligence { get; private set; }
    public PlayerStat Courage { get; private set; }
    public PlayerStat Kindness { get; private set; }
    public PlayerStat Confidence { get; private set; }
    public PlayerStat Karma { get; private set; } 
    public PlayerStat Appreciation { get; private set; } 
    public PronounSet Pronouns { get; private set; }

    public bool sleptWithNina = false; 

    //public Animator statIncreaseAnimator; // Reference to Animator
    //public GameObject statIncreaseTextPrefab; // Prefab for stat increase UI
    //public Transform statIncreaseTextParent; // Parent transform to display UI 

    private void Awake()
    {
        instance = this;
        Charisma = new PlayerStat("Charisma");
        Creativity = new PlayerStat("Creativity");
        Empathy = new PlayerStat("Empathy");
        Humor = new PlayerStat("Humor");
        Endurance = new PlayerStat("Endurance");
        Patience = new PlayerStat("Patience");
        Intelligence = new PlayerStat("Intelligence");
        Courage = new PlayerStat("Courage");
        Kindness = new PlayerStat("Kindness");
        Confidence = new PlayerStat("Confidence");
        Karma = new PlayerStat("Karma");
        Appreciation = new PlayerStat("Appreciation"); 

        InitializePronouns();
        RegisterStatsToVariableStore();
    }

    private void InitializePronouns()
    {
        string subject = PlayerPrefs.GetString("Pronoun_Subject", "he");
        string obj = PlayerPrefs.GetString("Pronoun_Object", "him");
        string posAdj = PlayerPrefs.GetString("Pronoun_PossessiveAdjective", "his");
        string posPro = PlayerPrefs.GetString("Pronoun_PossessivePronoun", "his");
        string reflexive = PlayerPrefs.GetString("Pronoun_Reflexive", "himself");

        Pronouns = new PronounSet(subject, obj, posAdj, posPro, reflexive);

        Debug.Log($"Subject: {Pronouns.Subject}, Object: {Pronouns.Object}");
    }

    private void RegisterStatsToVariableStore()
    {
        // Create or update PlayerStats database
        var dbName = "PlayerStats";

        VariableStore.CreateVariable(dbName + ".Charisma", Charisma.Level, () => Charisma.Level, v => Charisma.Level = v);
        VariableStore.CreateVariable(dbName + ".Creativity", Creativity.Level, () => Creativity.Level, v => Creativity.Level = v);
        VariableStore.CreateVariable(dbName + ".Empathy", Empathy.Level, () => Empathy.Level, v => Empathy.Level = v);
        VariableStore.CreateVariable(dbName + ".Humor", Humor.Level, () => Humor.Level, v => Humor.Level = v);
        VariableStore.CreateVariable(dbName + ".Endurance", Endurance.Level, () => Endurance.Level, v => Endurance.Level = v);
        VariableStore.CreateVariable(dbName + ".Patience", Patience.Level, () => Patience.Level, v => Patience.Level = v);
        VariableStore.CreateVariable(dbName + ".Intelligence", Intelligence.Level, () => Intelligence.Level, v => Intelligence.Level = v);
        VariableStore.CreateVariable(dbName + ".Courage", Courage.Level, () => Courage.Level, v => Courage.Level = v);
        VariableStore.CreateVariable(dbName + ".Kindness", Kindness.Level, () => Kindness.Level, v => Kindness.Level = v);
        VariableStore.CreateVariable(dbName + ".Confidence", Confidence.Level, () => Confidence.Level, v => Confidence.Level = v);
        VariableStore.CreateVariable(dbName + ".Karma", Karma.Level, () => Karma.Level, v => Karma.Level = v);
        VariableStore.CreateVariable(dbName + ".Appreication", Appreciation.Level, () => Appreciation.Level, v => Appreciation.Level = v);

        VariableStore.CreateVariable("PlayerStats.Pronoun.Subject", Pronouns.Subject, () => Pronouns.Subject, v => SetPronoun("Subject", v));
        VariableStore.CreateVariable("PlayerStats.Pronoun.Object", Pronouns.Object, () => Pronouns.Object, v => SetPronoun("Object", v));
        VariableStore.CreateVariable("PlayerStats.Pronoun.PossessiveAdjective", Pronouns.PossessiveAdjective, () => Pronouns.PossessiveAdjective, v => SetPronoun("PossessiveAdjective", v));
        VariableStore.CreateVariable("PlayerStats.Pronoun.PossessivePronoun", Pronouns.PossessivePronoun, () => Pronouns.PossessivePronoun, v => SetPronoun("PossessivePronoun", v));
        VariableStore.CreateVariable("PlayerStats.Pronoun.Reflexive", Pronouns.Reflexive, () => Pronouns.Reflexive, v => SetPronoun("Reflexive", v));

        string firstName = PlayerPrefs.GetString("FirstName", "");
        string lastName = PlayerPrefs.GetString("LastName", "");

        VariableStore.CreateVariable("PlayerStats.FirstName", firstName, () => InputPanel.instance.firstName, v => InputPanel.instance.firstName = v);
        VariableStore.CreateVariable("PlayerStats.LastName", lastName, () => InputPanel.instance.lastName, v => InputPanel.instance.lastName = v);

        VariableStore.CreateVariable("PlayerStats.Money", 1000f);

        VariableStore.CreateVariable("Default.StartingFile", "");
        VariableStore.CreateVariable("Scene.LastScene", "");
        VariableStore.CreateVariable("Scene.StartingFile", "");

        VariableStore.CreateVariable("PlayerSleptWithNina", sleptWithNina); 

        //VariableStore.TrySetValue("PlayerSelptWithNina", sleptWithNina); 


        VariableStore.PrintAllVariables();
    }

    public void SetPronoun(string key, object value)
    {
        string val = value.ToString();
        switch (key)
        {
            case "Subject": Pronouns.Subject = val; PlayerPrefs.SetString("Pronoun_Subject", val); break;
            case "Object": Pronouns.Object = val; PlayerPrefs.SetString("Pronoun_Object", val); break;
            case "PossessiveAdjective": Pronouns.PossessiveAdjective = val; PlayerPrefs.SetString("Pronoun_PossessiveAdjective", val); break;
            case "PossessivePronoun": Pronouns.PossessivePronoun = val; PlayerPrefs.SetString("Pronoun_PossessivePronoun", val); break;
            case "Reflexive": Pronouns.Reflexive = val; PlayerPrefs.SetString("Pronoun_Reflexive", val); break;
        }
        PlayerPrefs.Save();
    }

    private void Start()
    {
        // Load the saved stat levels and update UI
        StatsMenu.instance.UpdateStatsBars(); // Update the Stats Bars when the game starts
        StatsMenu.instance.UpdateAllRankTexts(); // Update all rank texts when the game starts
    }

    public void IncreaseStatPoints(PlayerStat stat, int pointsToAdd)
    {
        stat.Points += pointsToAdd;
        StatsMenu.instance.UpdateStatsBars(); // Update Stats Bars after stat point increase
        StatsMenu.instance.UpdateAllRankTexts(); // Update all rank texts after stat point increase 

        // Trigger the animation and show the stat increase UI
        VNMenuManager.instance.PlayStatIncreaseAnimation(stat.name, pointsToAdd);
    }

    public void IncreaseKarmaPoints(PlayerStat stat, int pointsToAdd)
    {
        stat.Points += pointsToAdd;
        //StatsMenu.instance.UpdateStatsBars(); // Update Stats Bars after stat point increase
        //StatsMenu.instance.UpdateAllRankTexts(); // Update all rank texts after stat point increase 

        // Trigger the animation and show the stat increase UI
        //VNMenuManager.instance.PlayStatIncreaseAnimation(stat.name, pointsToAdd);
    }

    public void DecreaseKarmaPoints(PlayerStat stat, int pointsToDeduct)
    {
        stat.Points -= pointsToDeduct;
        //StatsMenu.instance.UpdateStatsBars(); // Update Stats Bars after stat point increase
        //StatsMenu.instance.UpdateAllRankTexts(); // Update all rank texts after stat point increase 

        // Trigger the animation and show the stat increase UI
        //VNMenuManager.instance.PlayStatIncreaseAnimation(stat.name, pointsToDeduct);
    } 

    public void IncreaseAppreciationGrowthPoints(PlayerStat stat, int AppreciationPointsToAdd)
    {
        stat.Points += AppreciationPointsToAdd; 
    }

    private IEnumerator DisableAfterAnimation()
    {
        // Wait until the animation has finished playing
        yield return new WaitForSeconds(VNMenuManager.instance.statIncreaseAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Optionally, add a small delay before disabling
        yield return new WaitForSeconds(0.5f);

        // Disable the GameObject after the animation completes
        VNMenuManager.instance.statIncreaseGameObject.SetActive(false);
    }

    public void IncreaseStat(PlayerStat stat)
    {
        if (stat.Level < 10)
        {
            stat.Level++;
            StatsMenu.instance.UpdateStatsBars(); // Update Stats Bars after stat increase
            StatsMenu.instance.UpdateAllRankTexts(); // Update all rank texts after stat increase
        }
    }

    public void DecreaseStat(PlayerStat stat)
    {
        if (stat.Level > 0)
        {
            stat.Level--;
            StatsMenu.instance.UpdateStatsBars(); // Update Stats Bars after stat decrease
            StatsMenu.instance.UpdateAllRankTexts(); // Update all rank texts after stat decrease
        }
    }

    public void ResetStats()
    {
        // Reset all stats and save them to PlayerPrefs
        Charisma.Level = 1;
        Creativity.Level = 1;
        Empathy.Level = 1;
        Humor.Level = 1;
        Endurance.Level = 1;
        Patience.Level = 1;
        Intelligence.Level = 1;
        Courage.Level = 1;
        Kindness.Level = 1;
        Confidence.Level = 1;
        //Karma.Level = 1; 

        Charisma.Points = 0;
        Creativity.Points = 0;
        Empathy.Points = 0;
        Humor.Points = 0;
        Endurance.Points = 0;
        Patience.Points = 0;
        Intelligence.Points = 0;
        Courage.Points = 0;
        Kindness.Points = 0;
        Confidence.Points = 0;
        Karma.Points = 50;
        Appreciation.Points = 0; 

        // Save new Karma value in PlayerPrefs
        //PlayerPrefs.SetInt("Karma_Points", 50);
        //PlayerPrefs.Save(); 

        StatsMenu.instance.UpdateStatsBars(); // Update the Stats Bars after resetting stats
        StatsMenu.instance.UpdateAllRankTexts(); // Update all rank texts after resetting stats 
        VNMenuManager.instance.UpdateKarmaMeter();
    }

    public static float GetMoney()
    {
        VariableStore.TryGetValue("PlayerStats.Money", out object value);
        return value != null ? (float)value : 0f;
    }

    //public static void SetMoney(float amount)
    //{
    //VariableStore.TrySetValue("PlayerStats.Money", amount);
    //}

    public static void SetMoney(float amount)
    {
        float clampedAmount = Mathf.Max(0, amount);
        VariableStore.TrySetValue("PlayerStats.Money", clampedAmount);

        // Update the earnings text immediately
        if (VNMenuManager.instance != null)
            VNMenuManager.instance.UpdateEarningsText(clampedAmount);
    }

    public void DisplayStats()
    {
        Debug.Log($"Charisma: {Charisma.Level}");
        Debug.Log($"Creativity: {Creativity.Level}");
        Debug.Log($"Empathy: {Empathy.Level}");
        Debug.Log($"Humor: {Humor.Level}");
        Debug.Log($"Endurance: {Endurance.Level}");
        Debug.Log($"Patience: {Patience.Level}");
        Debug.Log($"Intelligence: {Intelligence.Level} (Points: {Intelligence.Points}/{Intelligence.PointsForNextLevel})");
        Debug.Log($"Courage: {Courage.Level} (Points: {Courage.Points}/{Courage.PointsForNextLevel})");
        Debug.Log($"Kindness: {Kindness.Level} (Points: {Kindness.Points}/{Kindness.PointsForNextLevel})");
        Debug.Log($"Confidence: {Confidence.Level} (Points: {Confidence.Points}/{Confidence.PointsForNextLevel})");
        Debug.Log($"Karma: {Karma.Level} (Points: {Karma.Points}/{Karma.PointsForNextLevel})");
        Debug.Log($"Appreciation: {Appreciation.Level} (Points: {Appreciation.Points}/{Appreciation.PointsForNextLevel})");
    }

    public void UnlockQuotes(QuoteMenu quoteMenu)
    {
        int creativityLevel = Creativity.Level;
        quoteMenu.UnlockQuotesBasedOnCreativity(creativityLevel);
    }
}