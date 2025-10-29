using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerStatsUI : MonoBehaviour
{
    public static PlayerStatsUI Instance;

    public PlayerStats playerStats;
    public GameSession gameSession;
    public SpawnManager spawnManager;

    public TextMeshProUGUI charismaText;
    public TextMeshProUGUI creativityText;
    public TextMeshProUGUI empathyText;
    public TextMeshProUGUI humorText;
    public TextMeshProUGUI enduranceText;
    public TextMeshProUGUI patienceText;
    public TextMeshProUGUI intelligenceText;
    public TextMeshProUGUI courageText;
    public TextMeshProUGUI kindnessText;
    public TextMeshProUGUI confidenceText; 
    public TextMeshProUGUI exhaustedText;
    public Button startGameButton;
    //public Button backToMapButton; 
    //public Button toggleExhaustionButton; // Button to toggle exhaustion

    public GameObject playerStatsPage;

    private void Start()
    {
        UpdateStatTexts();
        startGameButton.onClick.AddListener(StartCafeGame);
        UpgradeMenu.Instance.upgradeMenuButton.onClick.AddListener(UpgradeMenu.Instance.ShowMainMenu);
        //backToMapButton.onClick.AddListener(BackToTownMap); 
        //toggleExhaustionButton.onClick.AddListener(ToggleExhaustion); // Add listener for exhaustion toggle
    }

    public void BackToTownMap()
    {
        //PlayerPrefs.SetString("LastScene", "TownMap");
        //PlayerPrefs.Save();
        SceneManager.LoadScene("TownMap");
    }

    public void IncreaseCharisma()
    {
        playerStats.IncreaseStat(playerStats.Charisma);
        UpdateStatTexts();
    }

    public void DecreaseCharisma()
    {
        playerStats.DecreaseStat(playerStats.Charisma);
        UpdateStatTexts();
    }

    public void IncreaseCreativity()
    {
        playerStats.IncreaseStat(playerStats.Creativity);
        UpdateStatTexts();
    }

    public void DecreaseCreativity()
    {
        playerStats.DecreaseStat(playerStats.Creativity);
        UpdateStatTexts();
    }

    public void IncreaseEmpathy()
    {
        playerStats.IncreaseStat(playerStats.Empathy);
        UpdateStatTexts();
    }

    public void DecreaseEmpathy()
    {
        playerStats.DecreaseStat(playerStats.Empathy);
        UpdateStatTexts();
    }

    public void IncreaseHumor()
    {
        playerStats.IncreaseStat(playerStats.Humor);
        UpdateStatTexts();
    }

    public void DecreaseHumor()
    {
        playerStats.DecreaseStat(playerStats.Humor);
        UpdateStatTexts();
    }

    public void IncreaseEndurance()
    {
        playerStats.IncreaseStat(playerStats.Endurance);
        UpdateStatTexts();
    }

    public void DecreaseEndurance()
    {
        playerStats.DecreaseStat(playerStats.Endurance);
        UpdateStatTexts();
    }

    public void IncreasePatience()
    {
        playerStats.IncreaseStat(playerStats.Patience);
        UpdateStatTexts();
    }

    public void DecreasePatience()
    {
        playerStats.DecreaseStat(playerStats.Patience);
        UpdateStatTexts();
    }

    public void IncreaseIntelliegence()
    {
        playerStats.IncreaseStat(playerStats.Intelligence);
        UpdateStatTexts();
    }

    public void DecreaseIntelliegence()
    {
        playerStats.DecreaseStat(playerStats.Intelligence);
        UpdateStatTexts();
    }

    public void IncreaseCourage()
    {
        playerStats.IncreaseStat(playerStats.Courage);
        UpdateStatTexts();
    }

    public void DecreaseCourage()
    {
        playerStats.DecreaseStat(playerStats.Courage);
        UpdateStatTexts();
    }

    public void IncreaseKindness()
    {
        playerStats.IncreaseStat(playerStats.Kindness);
        UpdateStatTexts();
    }

    public void DecreaseKindness()
    {
        playerStats.DecreaseStat(playerStats.Kindness);
        UpdateStatTexts();
    } 

    public void IncreaseConfidence()
    {
        playerStats.IncreaseStat(playerStats.Confidence);
        UpdateStatTexts();
    }

    public void DecreaseConfidence()
    {
        playerStats.DecreaseStat(playerStats.Confidence);
        UpdateStatTexts();
    }

    private void UpdateStatTexts()
    {
        charismaText.text = $"Charisma: {playerStats.Charisma.Level}";
        creativityText.text = $"Creativity: {playerStats.Creativity.Level}";
        empathyText.text = $"Empathy: {playerStats.Empathy.Level}";
        humorText.text = $"Humor: {playerStats.Humor.Level}";
        enduranceText.text = $"Endurance: {playerStats.Endurance.Level}";
        patienceText.text = $"Patience: {playerStats.Patience.Level}";
        intelligenceText.text = $"Intelligence: {playerStats.Intelligence.Level}"; 
        courageText.text = $"Courage: {playerStats.Courage.Level}";
        kindnessText.text = $"Kindness: {playerStats.Kindness.Level}"; 
        confidenceText.text = $"Confidence: {playerStats.Confidence.Level}"; 
        //exhaustedText;
    //exhaustedText.text = $"Exhausted: {gameSession.IsExhausted}"; // Display exhaustion status
}

    public void StartCafeGame()
    {
        gameSession.ApplyPlayerStats(playerStats);
        gameSession.StartCafeGame();
        spawnManager.StartGame();
        playerStatsPage.SetActive(false);
        UpgradeMenu.Instance.backToMapButton.gameObject.SetActive(false);
    }

    /*public void ToggleExhaustion()
    {   
        gameSession.ToggleExhaustion(); // Toggle exhaustion in GameSession
        UpdateStatTexts(); // Update UI to reflect the change
    }*/
}