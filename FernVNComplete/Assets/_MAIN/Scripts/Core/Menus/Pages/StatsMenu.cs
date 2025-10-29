using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsMenu : MenuPage
{
    public static StatsMenu instance { get; private set; }

    [SerializeField] private GameObject[] panels;
    [SerializeField] private GameObject[] statParents; // Parent GameObjects for each stat bar 
    [SerializeField] private GameObject[] friendshipStatParents; // Parent GameObjects for Friendship stats  

    // Add TextMeshProUGUI fields for rank texts
    public TextMeshProUGUI charismaRankText; 
    public TextMeshProUGUI creativityRankText;
    public TextMeshProUGUI empathyRankText;
    public TextMeshProUGUI humorRankText;
    public TextMeshProUGUI enduranceRankText;
    public TextMeshProUGUI patienceRankText; 
    public TextMeshProUGUI intelligenceRankText;
    public TextMeshProUGUI courageRankText;
    public TextMeshProUGUI kindnessRankText;
    public TextMeshProUGUI confidenceRankText;

    public TextMeshProUGUI charismaRankUpText;
    public TextMeshProUGUI creativityRankUpText;
    public TextMeshProUGUI empathyRankUpText;
    public TextMeshProUGUI humorRankUpText;
    public TextMeshProUGUI enduranceRankUpText;
    public TextMeshProUGUI patienceRankUpText;
    public TextMeshProUGUI intelligenceRankUpText;
    public TextMeshProUGUI courageRankUpText;
    public TextMeshProUGUI kindnessRankUpText;
    public TextMeshProUGUI confidenceRankUpText;

    public Button RankUpButtonCharisma;
    public Button RankUpButtonCreativity;
    public Button RankUpButtonEmpathy;
    public Button RankUpButtonHumor;
    public Button RankUpButtonEndurance;
    public Button RankUpButtonPatience;
    public Button RankUpButtonIntelligence;
    public Button RankUpButtonCourage;
    public Button RankUpButtonKindness;
    public Button RankUpButtonConfidence;  

    // TextMeshProUGUI fields for friendship rank texts
    public TextMeshProUGUI friendA_RankText; 
    public TextMeshProUGUI friendB_RankText;
    public TextMeshProUGUI friendC_RankText;
    public TextMeshProUGUI friendD_RankText;
    public TextMeshProUGUI friendE_RankText;
    public TextMeshProUGUI friendF_RankText;
    public TextMeshProUGUI friendG_RankText;
    public TextMeshProUGUI friendH_RankText;
    public TextMeshProUGUI friendI_RankText;
    public TextMeshProUGUI friendJ_RankText; 

    private GameObject activePanel;

    private AudioSource menuClickSound; 

    // Headshot Images 
    [SerializeField] private Image meiHeadShot;
    [SerializeField] private Image alexHeadShot;
    [SerializeField] private Image ninaHeadShot;
    [SerializeField] private Image simonHeadShot;
    [SerializeField] private Image friendEHeadShot;
    [SerializeField] private Image friendFHeadShot;
    [SerializeField] private Image friendGHeadShot;
    [SerializeField] private Image friendHHeadShot;
    [SerializeField] private Image friendIHeadShot;
    [SerializeField] private Image friendJHeadShot; 

    // Silhouette Sprites 
    [SerializeField] private Sprite meiSilhouetteSprite; 
    [SerializeField] private Sprite alexSilhouetteSprite;
    [SerializeField] private Sprite ninaSilhouetteSprite;
    [SerializeField] private Sprite simonSilhouetteSprite;
    [SerializeField] private Sprite friendESilhouetteSprite;
    [SerializeField] private Sprite friendFSilhouetteSprite;
    [SerializeField] private Sprite friendGSilhouetteSprite;
    [SerializeField] private Sprite friendHSilhouetteSprite;
    [SerializeField] private Sprite friendISilhouetteSprite;
    [SerializeField] private Sprite friendJSilhouetteSprite;

    // Normal Sprites 
    [SerializeField] private Sprite meiHeadSprite; 
    [SerializeField] private Sprite alexHeadSprite;
    [SerializeField] private Sprite ninaHeadSprite;
    [SerializeField] private Sprite simonHeadSprite;
    [SerializeField] private Sprite friendEHeadSprite;
    [SerializeField] private Sprite friendFHeadSprite;
    [SerializeField] private Sprite friendGHeadSprite;
    [SerializeField] private Sprite friendHHeadSprite;
    [SerializeField] private Sprite friendIHeadSprite;
    [SerializeField] private Sprite friendJHeadSprite; 

    // Title Texts 
    [SerializeField] private TextMeshProUGUI meiFriendTitleText; 
    [SerializeField] private TextMeshProUGUI alexFriendTitleText;
    [SerializeField] private TextMeshProUGUI ninaFriendTitleText;
    [SerializeField] private TextMeshProUGUI simonFriendTitleText;
    [SerializeField] private TextMeshProUGUI friendEFriendTitleText;
    [SerializeField] private TextMeshProUGUI friendFFriendTitleText;
    [SerializeField] private TextMeshProUGUI friendGFriendTitleText;
    [SerializeField] private TextMeshProUGUI friendHFriendTitleText;
    [SerializeField] private TextMeshProUGUI friendIFriendTitleText;
    [SerializeField] private TextMeshProUGUI friendJFriendTitleText; 

    private VN_Configuration config => VN_Configuration.activeConfig;

    // Declare a Coroutine variable to hold the reference to the running flicker coroutine
    private Coroutine flickerCoroutine;

    // Dictionary to track flicker coroutines for each stat slot
    private Dictionary<Image, Coroutine> flickerCoroutines = new Dictionary<Image, Coroutine>();

    private Dictionary<Image, Coroutine> flickerColorCoroutines = new Dictionary<Image, Coroutine>(); 

    private void Awake()
    {
        instance = this;

        menuClickSound = GameObject.Find("MenuClickSound").GetComponent<AudioSource>(); 
    }

    void Start()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == 0);
        }

        activePanel = panels[0];
        LoadConfig();
        UpdateStatsBars(); // Update stats bars at start 
        UpdateFriendshipBars(); 
        UpdateAllRankTexts(); // Update all rank texts at the start  
        UpdateAllFriendshipRankTexts(); // Update all friendship rank texts
        UpdateMeiHeadShot(); // Add this here 
    } 

    // New coroutine for flickering the stat slot
    private IEnumerator FlickerStatSlot(Image statSlot)
    {
        Color originalColor = statSlot.color;
        while (true) // Flicker indefinitely until stopped 
        {
            statSlot.color = Color.gray; // Flicker with white color for emphasis 
            yield return new WaitForSeconds(0.3f); 
            statSlot.color = Color.yellow; // Set it back to highlighted color 
            yield return new WaitForSeconds(0.3f);   
        }
    }

    // Method to start flickering and store the coroutine reference
    private void StartFlickering(Image statSlot)
    {
        if (flickerCoroutines.ContainsKey(statSlot))
        {
            StopFlickering(statSlot); // Ensure any existing flicker is stopped
        }

        // Start the flickering coroutine and store it in the dictionary
        Coroutine flickerCoroutine = StartCoroutine(FlickerStatSlot(statSlot));
        flickerCoroutines[statSlot] = flickerCoroutine;
    } 

    // New method to show the button for leveling up a stat
    // Method to show the Rank-Up button and link it with stopping the flickering
    private void ShowRankUpButton(Button rankUpButton, PlayerStat stat, Image statSlot, TextMeshProUGUI rankUpText)
    {
        rankUpButton.gameObject.SetActive(true); // Make the Rank Up button visible

        // Add a listener to stop the flickering and hide the text and button when clicked
        rankUpButton.onClick.RemoveAllListeners(); // Clear any previous listeners
        rankUpButton.onClick.AddListener(() =>
        {
            StopFlickering(statSlot); // Stop the flickering
            rankUpText.gameObject.SetActive(false); // Hide the Level-Up text
            rankUpButton.gameObject.SetActive(false); // Hide the button
        });
    }

    // Method to stop the flickering coroutine for a specific stat slot
    private void StopFlickering(Image statSlot)
    {
        if (flickerCoroutines.ContainsKey(statSlot))
        {
            StopCoroutine(flickerCoroutines[statSlot]);
            flickerCoroutines.Remove(statSlot); // Remove from the dictionary
            statSlot.color = Color.yellow; // Reset to highlighted color
        }
    } 

    // Stop flickering and hide level-up text after clicking the button
    private void StopFlickerAndHideText(PlayerStat stat, Image statSlot, TextMeshProUGUI rankUpText, Button rankUpButton)
    {
        statSlot.color = Color.yellow; // Reset to highlighted color
        StopCoroutine(FlickerStatSlot(statSlot)); // Stop the flicker effect
        rankUpText.gameObject.SetActive(false); // Hide the level-up text
        rankUpButton.gameObject.SetActive(false); // Hide the rank-up button
    }

    // Method to stop all flickering and hide rank-up texts when closing the Stats Menu
    public void StopAllFlickeringAndHideRankUpTexts()
    {
        // Stop flickering for all stat slots and hide rank-up texts
        foreach (var statSlot in statParents)
        {
            Image[] statImages = statSlot.GetComponentsInChildren<Image>();
            foreach (Image statImage in statImages)
            {
                StopFlickering(statImage); // Stop flickering for each slot
            }
        }

        // Hide rank-up texts and buttons
        charismaRankUpText.gameObject.SetActive(false);
        creativityRankUpText.gameObject.SetActive(false);
        empathyRankUpText.gameObject.SetActive(false);
        humorRankUpText.gameObject.SetActive(false);
        enduranceRankUpText.gameObject.SetActive(false);
        patienceRankUpText.gameObject.SetActive(false);
        intelligenceRankUpText.gameObject.SetActive(false);
        courageRankUpText.gameObject.SetActive(false);
        kindnessRankUpText.gameObject.SetActive(false);
        confidenceRankUpText.gameObject.SetActive(false);

        RankUpButtonCharisma.gameObject.SetActive(false);
        RankUpButtonCreativity.gameObject.SetActive(false);
        RankUpButtonEmpathy.gameObject.SetActive(false);
        RankUpButtonHumor.gameObject.SetActive(false);
        RankUpButtonEndurance.gameObject.SetActive(false);
        RankUpButtonPatience.gameObject.SetActive(false);
        RankUpButtonIntelligence.gameObject.SetActive(false);
        RankUpButtonCourage.gameObject.SetActive(false);
        RankUpButtonKindness.gameObject.SetActive(false);
        RankUpButtonConfidence.gameObject.SetActive(false);
    } 

    // New method to show the level-up text
    private IEnumerator ShowLevelUpText(TextMeshProUGUI levelUpText)
    {
        levelUpText.gameObject.SetActive(true); // Show the level-up text
        yield return new WaitForSeconds(1.5f);  // Keep it visible for 1.5 seconds
        levelUpText.gameObject.SetActive(false); // Hide the level-up text
    } 

    private void LoadConfig()
    {
        if (File.Exists(VN_Configuration.filePath))
            VN_Configuration.activeConfig = FileManager.Load<VN_Configuration>(VN_Configuration.filePath, encrypt: VN_Configuration.ENCRYPT);
        else
            VN_Configuration.activeConfig = new VN_Configuration();

        VN_Configuration.activeConfig.Load();
    }

    // Update the visual representation of each stat based on its level
    // Call the modified UpdateStatBar in UpdateStatsBars()
    public void UpdateStatsBars()
    {
        UpdateStatBar(statParents[0], PlayerStats.instance.Charisma.Level, charismaRankUpText, PlayerStats.instance.Charisma, RankUpButtonCharisma); // Charisma
        UpdateStatBar(statParents[1], PlayerStats.instance.Creativity.Level, creativityRankUpText, PlayerStats.instance.Creativity, RankUpButtonCreativity); // Creativity
        UpdateStatBar(statParents[2], PlayerStats.instance.Empathy.Level, empathyRankUpText, PlayerStats.instance.Empathy, RankUpButtonEmpathy); // Empathy
        UpdateStatBar(statParents[3], PlayerStats.instance.Humor.Level, humorRankUpText, PlayerStats.instance.Humor, RankUpButtonHumor); // Humor
        UpdateStatBar(statParents[4], PlayerStats.instance.Endurance.Level, enduranceRankUpText, PlayerStats.instance.Endurance, RankUpButtonEndurance); // Endurance
        UpdateStatBar(statParents[5], PlayerStats.instance.Patience.Level, patienceRankUpText, PlayerStats.instance.Patience, RankUpButtonPatience); // Patience
        UpdateStatBar(statParents[6], PlayerStats.instance.Intelligence.Level, intelligenceRankUpText, PlayerStats.instance.Intelligence, RankUpButtonIntelligence); // Intelligence
        UpdateStatBar(statParents[7], PlayerStats.instance.Courage.Level, courageRankUpText, PlayerStats.instance.Courage, RankUpButtonCourage); // Loyalty
        UpdateStatBar(statParents[8], PlayerStats.instance.Kindness.Level, kindnessRankUpText, PlayerStats.instance.Kindness, RankUpButtonKindness); // Kindness
        UpdateStatBar(statParents[9], PlayerStats.instance.Confidence.Level, confidenceRankUpText, PlayerStats.instance.Confidence, RankUpButtonConfidence); // Insight
    } 

    // Update the visual representation of each friendship stat (Friendship Stats)
    public void UpdateFriendshipBars()
    {
        UpdateFriendshipBar(friendshipStatParents[0], FriendshipStats.instance.FriendA.Level);
        UpdateFriendshipBar(friendshipStatParents[1], FriendshipStats.instance.FriendB.Level);
        UpdateFriendshipBar(friendshipStatParents[2], FriendshipStats.instance.FriendC.Level);
        UpdateFriendshipBar(friendshipStatParents[3], FriendshipStats.instance.FriendD.Level);
        UpdateFriendshipBar(friendshipStatParents[4], FriendshipStats.instance.FriendE.Level);
        UpdateFriendshipBar(friendshipStatParents[5], FriendshipStats.instance.FriendF.Level);
        UpdateFriendshipBar(friendshipStatParents[6], FriendshipStats.instance.FriendG.Level);
        UpdateFriendshipBar(friendshipStatParents[7], FriendshipStats.instance.FriendH.Level);
        UpdateFriendshipBar(friendshipStatParents[8], FriendshipStats.instance.FriendI.Level);
        UpdateFriendshipBar(friendshipStatParents[9], FriendshipStats.instance.FriendJ.Level); 
    }

    private void UpdateStatBar(GameObject statParent, int statLevel, TextMeshProUGUI rankUpText, PlayerStat stat, Button rankUpButton)
    {
        Image[] statSlots = statParent.GetComponentsInChildren<Image>();

        for (int i = 0; i < statSlots.Length; i++)
        {
            Image slot = statSlots[i];
            Animator animator = slot.GetComponent<Animator>();
            bool isFilled = i < statLevel;
            bool isCurrentSlot = (i == statLevel - 1);

            if (isFilled)
            {
                slot.color = Color.yellow;

                if (isCurrentSlot)
                {
                    int remainingPoints = stat.PointsForNextLevel - stat.Points;

                    // Stat is ready to rank up
                    if (!stat.IsLevelUpTextShown() && stat.Points >= stat.PointsForNextLevel)
                    {
                        StartFlickering(slot);
                        rankUpText.gameObject.SetActive(true);
                        stat.SetLevelUpTextShown();
                        ShowRankUpButton(rankUpButton, stat, slot, rankUpText);

                        // Stop pulsing animation
                        animator.SetBool("IsPulsing", false);

                        if (flickerColorCoroutines.ContainsKey(slot))
                        {
                            StopCoroutine(flickerColorCoroutines[slot]);
                            flickerColorCoroutines.Remove(slot);
                            slot.color = Color.yellow; // Reset to original
                        }
                    }
                    // Stat is close to ranking up (3 or fewer points away)
                    else if (remainingPoints <= 3 && remainingPoints > 0)
                    {
                        if (animator != null)
                        {
                            animator.SetBool("IsPulsing", true);

                            if (!flickerColorCoroutines.ContainsKey(slot))
                            {
                                Coroutine flicker = StartCoroutine(FlickerStatSlotColor(slot));
                                flickerColorCoroutines[slot] = flicker;
                            }
                        } 
                    }
                    else
                    {
                        if (animator != null)
                        {
                            animator.SetBool("IsPulsing", false);

                            if (flickerColorCoroutines.ContainsKey(slot))
                            {
                                StopCoroutine(flickerColorCoroutines[slot]);
                                flickerColorCoroutines.Remove(slot);
                                slot.color = Color.yellow; // Reset to original
                            }
                        } 
                    }
                }
                else
                {
                    // Not the current filled slot — disable animation just in case
                    animator.SetBool("IsPulsing", false);

                    if (flickerColorCoroutines.ContainsKey(slot))
                    {
                        StopCoroutine(flickerColorCoroutines[slot]);
                        flickerColorCoroutines.Remove(slot);
                        slot.color = Color.yellow; // Reset to original
                    } 
                }
            }
            else
            {
                slot.color = Color.gray;

                // Unfilled slots should not animate
                if (animator != null)
                    animator.SetBool("IsPulsing", false);
            }
        }
    } 

    private FriendshipStat GetFriendshipStatByParent(GameObject friendParent)
    {
        if (friendParent == friendshipStatParents[0]) return FriendshipStats.instance.FriendA;
        if (friendParent == friendshipStatParents[1]) return FriendshipStats.instance.FriendB;
        if (friendParent == friendshipStatParents[2]) return FriendshipStats.instance.FriendC;
        if (friendParent == friendshipStatParents[3]) return FriendshipStats.instance.FriendD;
        if (friendParent == friendshipStatParents[4]) return FriendshipStats.instance.FriendE;
        if (friendParent == friendshipStatParents[5]) return FriendshipStats.instance.FriendF;
        if (friendParent == friendshipStatParents[6]) return FriendshipStats.instance.FriendG;
        if (friendParent == friendshipStatParents[7]) return FriendshipStats.instance.FriendH;
        if (friendParent == friendshipStatParents[8]) return FriendshipStats.instance.FriendI;
        if (friendParent == friendshipStatParents[9]) return FriendshipStats.instance.FriendJ;

        return null;
    }

    private void UpdateFriendshipBar(GameObject friendParent, int friendLevel)
    {
        Image[] friendSlots = friendParent.GetComponentsInChildren<Image>();
        bool isMet = friendLevel >= 1;
        Color unfilledColor = isMet ? Color.gray : new Color(0f, 0f, 0f, 0.4f);

        // Get the corresponding FriendshipStat
        FriendshipStat stat = GetFriendshipStatByParent(friendParent);

        for (int i = 0; i < friendSlots.Length; i++)
        {
            Image slot = friendSlots[i];
            Animator animator = slot.GetComponent<Animator>();
            bool isCurrentSlot = (i == friendLevel - 1);

            if (i < friendLevel)
            {
                // Set default filled color
                slot.color = Color.blue;

                if (isCurrentSlot && stat != null && stat.HasPendingRankUp())
                {
                    // Trigger scale pulse animation
                    if (animator != null)
                    {
                        animator.ResetTrigger("SlotPulse"); // Clear previous trigger
                        animator.SetTrigger("SlotPulse");
                    }

                    // Start flickering color if not already
                    if (!flickerColorCoroutines.ContainsKey(slot))
                    {
                        Coroutine flicker = StartCoroutine(FlickerFriendshipSlotColor(slot));
                        flickerColorCoroutines[slot] = flicker;
                    }
                }
                else
                {
                    // Stop flickering if it was running
                    if (flickerColorCoroutines.ContainsKey(slot))
                    {
                        StopCoroutine(flickerColorCoroutines[slot]);
                        flickerColorCoroutines.Remove(slot);
                        slot.color = Color.blue; // Reset color
                    }

                    // Reset scale if Animator exists
                    if (animator != null)
                    {
                        slot.rectTransform.localScale = Vector3.one;
                    }
                }
            }
            else
            {
                // Unfilled slot color
                slot.color = unfilledColor;

                // Ensure no flicker on unused slots
                if (flickerColorCoroutines.ContainsKey(slot))
                {
                    StopCoroutine(flickerColorCoroutines[slot]);
                    flickerColorCoroutines.Remove(slot);
                }

                // Reset scale and disable Animator effects if needed
                if (animator != null)
                {
                    slot.rectTransform.localScale = Vector3.one;
                }
            }
        }
    }  

    // Update Rank Text for each stat
    public void UpdateRankText(PlayerStat stat, TextMeshProUGUI rankText)
    {
        rankText.text = stat.Level == 10 ? $"{stat.name}: Rank MAX" : $"{stat.name}: Rank {stat.Level:00}";
    }

    // Update Rank Text for each friendship stat
    public void UpdateFriendshipRankText(FriendshipStat stat, TextMeshProUGUI rankText)
    {
        rankText.text = stat.Level == 10 ? $"{stat.name}: Rank MAX" : $"{stat.name}: Rank {stat.Level:00}";

        UpdateMeiHeadShot();
        UpdateAlexHeadShot();
        UpdateNinaHeadShot();
        UpdateSimonHeadShot();
        UpdateFriendEHeadShot();
        UpdateFriendFHeadShot();
        UpdateFriendGHeadShot();
        UpdateFriendHHeadShot();
        UpdateFriendIHeadShot();
        UpdateFriendJHeadShot(); 
    } 

    public void UpdateAllRankTexts()
    {
        UpdateRankText(PlayerStats.instance.Charisma, charismaRankText);
        UpdateRankText(PlayerStats.instance.Creativity, creativityRankText);
        UpdateRankText(PlayerStats.instance.Empathy, empathyRankText);
        UpdateRankText(PlayerStats.instance.Humor, humorRankText);
        UpdateRankText(PlayerStats.instance.Endurance, enduranceRankText);
        UpdateRankText(PlayerStats.instance.Patience, patienceRankText); 
        UpdateRankText(PlayerStats.instance.Intelligence, intelligenceRankText);
        UpdateRankText(PlayerStats.instance.Courage, courageRankText);
        UpdateRankText(PlayerStats.instance.Kindness, kindnessRankText);
        UpdateRankText(PlayerStats.instance.Confidence, confidenceRankText); 
    }

    // Update Friendship Stats Rank Texts
    public void UpdateAllFriendshipRankTexts()
    {
        UpdateFriendshipRankText(FriendshipStats.instance.FriendA, friendA_RankText);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendB, friendB_RankText);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendC, friendC_RankText);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendD, friendD_RankText);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendE, friendE_RankText);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendF, friendF_RankText);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendG, friendG_RankText);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendH, friendH_RankText);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendI, friendI_RankText);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendJ, friendJ_RankText); 
    }

    // Method to stop all flickering and hide rank-up texts when closing the Stats Menu
    /*public void StopAllFlickeringAndHideRankUpTexts()
    {
        StopFlickerAndHideText(PlayerStats.instance.Charisma, statParents[0].GetComponentInChildren<Image>(), charismaRankUpText, RankUpButtonCharisma);
        StopFlickerAndHideText(PlayerStats.instance.Creativity, statParents[1].GetComponentInChildren<Image>(), creativityRankUpText, RankUpButtonCreativity);
        StopFlickerAndHideText(PlayerStats.instance.Empathy, statParents[2].GetComponentInChildren<Image>(), empathyRankUpText, RankUpButtonEmpathy);
        StopFlickerAndHideText(PlayerStats.instance.Humor, statParents[3].GetComponentInChildren<Image>(), humorRankUpText, RankUpButtonHumor);
        StopFlickerAndHideText(PlayerStats.instance.Endurance, statParents[4].GetComponentInChildren<Image>(), enduranceRankUpText, RankUpButtonEndurance);
        StopFlickerAndHideText(PlayerStats.instance.Patience, statParents[5].GetComponentInChildren<Image>(), patienceRankUpText, RankUpButtonPatience);
        StopFlickerAndHideText(PlayerStats.instance.Intelligence, statParents[6].GetComponentInChildren<Image>(), intelligenceRankUpText, RankUpButtonIntelligence);
        StopFlickerAndHideText(PlayerStats.instance.Loyalty, statParents[7].GetComponentInChildren<Image>(), loyaltyRankUpText, RankUpButtonLoyalty);
        StopFlickerAndHideText(PlayerStats.instance.Kindness, statParents[8].GetComponentInChildren<Image>(), kindnessRankUpText, RankUpButtonKindness);
        StopFlickerAndHideText(PlayerStats.instance.Insight, statParents[9].GetComponentInChildren<Image>(), insightRankUpText, RankUpButtonInsight);
    }*/ 

    // This method should be called when closing the Stats Menu
    public void CloseStatsMenu()
    {
        StopAllFlickeringAndHideRankUpTexts();
        //gameObject.SetActive(false); // Close the Stats Menu
    } 

    public void OpenPanel(string panelName)
    {
        GameObject panel = panels.First(p => p.name.ToLower() == panelName.ToLower());

        if (panel == null)
        {
            Debug.LogWarning($"Did not find panel called '{panelName}' in config menu.");
            return;
        }

        if (activePanel == panel)
            return; // Prevent sound if already on the panel 

        if (activePanel != null && activePanel != panel)
            activePanel.SetActive(false);

        panel.SetActive(true);
        activePanel = panel;

        menuClickSound.PlayOneShot(menuClickSound.clip); 

        // If the stats panel is opened, update the stats bars and rank texts
        if (panel.name.ToLower() == "perks")
        {
            UpdateStatsBars();
            UpdateAllRankTexts();
        }

        // If the friendship stats panel is opened, update the friendship bars and rank texts
        else if (panel.name.ToLower() == "bonds") 
        {
            UpdateFriendshipBars();
            UpdateAllFriendshipRankTexts();
        }
    }

    public void IncreaseStatOrFriendship<T>(T stat, int pointsToAdd, TextMeshProUGUI rankText)
    {
        if (stat is PlayerStat playerStat)
        {
            PlayerStats.instance.IncreaseStatPoints(playerStat, pointsToAdd);
            UpdateRankText(playerStat, rankText);
        }
        else if (stat is FriendshipStat friendshipStat)
        {
            FriendshipStats.instance.IncreaseStatPoints(friendshipStat, pointsToAdd);
            UpdateFriendshipRankText(friendshipStat, rankText);
        }
    }

    private void UpdateMeiHeadShot()
    {
        if (FriendshipStats.instance.FriendA.Level < 1)
        {
            meiHeadShot.sprite = meiSilhouetteSprite;
            meiFriendTitleText.text = "???";
        }
        else
        {
            meiHeadShot.sprite = meiHeadSprite;
        }
    }

    private void UpdateAlexHeadShot()
    {
        if (FriendshipStats.instance.FriendB.Level < 1)
        {
            alexHeadShot.sprite = alexSilhouetteSprite;
            alexFriendTitleText.text = "???";
        }
        else
        {
            alexHeadShot.sprite = alexHeadSprite;
        }
    }

    private void UpdateNinaHeadShot()
    {
        if (FriendshipStats.instance.FriendC.Level < 1)
        {
            ninaHeadShot.sprite = ninaSilhouetteSprite;
            ninaFriendTitleText.text = "???";
        }
        else
        {
            ninaHeadShot.sprite = ninaHeadSprite;
        }
    }

    private void UpdateSimonHeadShot()
    {
        if (FriendshipStats.instance.FriendD.Level < 1)
        {
            simonHeadShot.sprite = simonSilhouetteSprite;
            simonFriendTitleText.text = "???";
        }
        else
        {
            simonHeadShot.sprite = simonHeadSprite;
        }
    }

    private void UpdateFriendEHeadShot()
    {
        if (FriendshipStats.instance.FriendE.Level < 1)
        {
            friendEHeadShot.sprite = friendESilhouetteSprite;
            friendEFriendTitleText.text = "???";
        }
        else
        {
            friendEHeadShot.sprite = friendEHeadSprite;
        }
    }

    private void UpdateFriendFHeadShot()
    {
        if (FriendshipStats.instance.FriendF.Level < 1)
        {
            friendFHeadShot.sprite = friendFSilhouetteSprite;
            friendFFriendTitleText.text = "???";
        }
        else
        {
            friendFHeadShot.sprite = friendFHeadSprite;
        }
    }

    private void UpdateFriendGHeadShot()
    {
        if (FriendshipStats.instance.FriendG.Level < 1)
        {
            friendGHeadShot.sprite = friendGSilhouetteSprite;
            friendGFriendTitleText.text = "???";
        }
        else
        {
            friendGHeadShot.sprite = friendGHeadSprite;
        }
    }

    private void UpdateFriendHHeadShot()
    {
        if (FriendshipStats.instance.FriendH.Level < 1)
        {
            friendHHeadShot.sprite = friendHSilhouetteSprite;
            friendHFriendTitleText.text = "???";
        }
        else
        {
            friendHHeadShot.sprite = friendHHeadSprite;
        }
    }

    private void UpdateFriendIHeadShot()
    {
        if (FriendshipStats.instance.FriendI.Level < 1)
        {
            friendIHeadShot.sprite = friendISilhouetteSprite;
            friendIFriendTitleText.text = "???";
        }
        else
        {
            friendIHeadShot.sprite = friendIHeadSprite;
        }
    }

    private void UpdateFriendJHeadShot()
    {
        if (FriendshipStats.instance.FriendJ.Level < 1)
        {
            friendJHeadShot.sprite = friendJSilhouetteSprite;
            friendJFriendTitleText.text = "???";
        }
        else
        {
            friendJHeadShot.sprite = friendJHeadSprite;
        }
    }

    public void IncreaseFriendALevelByOne()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendA, 1);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendA, friendA_RankText); 
    }

    public void IncreaseFriendALevelByTwo()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendA, 1);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendA, friendA_RankText);
    }

    public void IncreaseFriendALevelByThree()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendA, 1);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendA, friendA_RankText);
    }

    public void IncreaseFriendBLevelByOne()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendB, 1); 
        UpdateFriendshipRankText(FriendshipStats.instance.FriendB, friendB_RankText);
    }

    public void IncreaseFriendBLevelByTwo()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendB, 1);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendB, friendB_RankText);
    }

    public void IncreaseFriendBLevelByThree()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendB, 1);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendB, friendB_RankText); 
    } 

    // Adding methods for FriendC to FriendJ

    public void IncreaseFriendCLevelByOne()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendC, 1);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendC, friendC_RankText);
    }

    public void IncreaseFriendCLevelByTwo()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendC, 2);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendC, friendC_RankText);
    }

    public void IncreaseFriendCLevelByThree()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendC, 3);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendC, friendC_RankText);
    }

    public void IncreaseFriendDLevelByOne()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendD, 1);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendD, friendD_RankText);
    }

    public void IncreaseFriendDLevelByTwo()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendD, 2);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendD, friendD_RankText);
    }

    public void IncreaseFriendDLevelByThree()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendD, 3);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendD, friendD_RankText);
    }

    public void IncreaseFriendELevelByOne()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendE, 1);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendE, friendE_RankText);
    }

    public void IncreaseFriendELevelByTwo()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendE, 2);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendE, friendE_RankText);
    }

    public void IncreaseFriendELevelByThree()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendE, 3);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendE, friendE_RankText);
    }

    public void IncreaseFriendFLevelByOne()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendF, 1);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendF, friendF_RankText);
    }

    public void IncreaseFriendFLevelByTwo()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendF, 2);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendF, friendF_RankText);
    }

    public void IncreaseFriendFLevelByThree()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendF, 3);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendF, friendF_RankText);
    }

    public void IncreaseFriendGLevelByOne()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendG, 1);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendG, friendG_RankText);
    }

    public void IncreaseFriendGLevelByTwo()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendG, 2);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendG, friendG_RankText);
    }

    public void IncreaseFriendGLevelByThree()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendG, 3);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendG, friendG_RankText);
    }

    public void IncreaseFriendHLevelByOne()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendH, 1);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendH, friendH_RankText);
    }

    public void IncreaseFriendHLevelByTwo()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendH, 2);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendH, friendH_RankText);
    }

    public void IncreaseFriendHLevelByThree()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendH, 3);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendH, friendH_RankText);
    }

    public void IncreaseFriendILevelByOne()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendI, 1);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendI, friendI_RankText);
    }

    public void IncreaseFriendILevelByTwo()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendI, 2);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendI, friendI_RankText);
    }

    public void IncreaseFriendILevelByThree()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendI, 3);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendI, friendI_RankText);
    }

    public void IncreaseFriendJLevelByOne()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendJ, 1);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendJ, friendJ_RankText);
    }

    public void IncreaseFriendJLevelByTwo()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendJ, 2);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendJ, friendJ_RankText);
    }

    public void IncreaseFriendJLevelByThree()
    {
        FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendJ, 3);
        UpdateFriendshipRankText(FriendshipStats.instance.FriendJ, friendJ_RankText);
    }


    // Charisma methods
    public void CharismaLevelByOne()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Charisma, 1);
        UpdateRankText(PlayerStats.instance.Charisma, charismaRankText); // Update the rank text
    }

    public void CharismaLevelByTwo()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Charisma, 2);
        UpdateRankText(PlayerStats.instance.Charisma, charismaRankText); // Update the rank text
    }

    public void CharismaLevelByThree()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Charisma, 3);
        UpdateRankText(PlayerStats.instance.Charisma, charismaRankText); // Update the rank text
    } 

    public void CreativityLevelByOne()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Creativity, 1);
        UpdateRankText(PlayerStats.instance.Creativity, creativityRankText); // Update the rank text 
    }

    public void CreativityLevelByTwo()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Creativity, 2);
        UpdateRankText(PlayerStats.instance.Creativity, creativityRankText); // Update the rank text  
    }

    public void CreativityLevelByThree()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Creativity, 3);
        UpdateRankText(PlayerStats.instance.Creativity, creativityRankText); // Update the rank text      
    }

    // Empathy methods
    public void EmpathyLevelByOne()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Empathy, 1);
        UpdateRankText(PlayerStats.instance.Empathy, empathyRankText); // Update the rank text      
    }

    public void EmpathyLevelByTwo()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Empathy, 2);
        UpdateRankText(PlayerStats.instance.Empathy, empathyRankText); // Update the rank text      
    }

    public void EmpathyLevelByThree()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Empathy, 3);
        UpdateRankText(PlayerStats.instance.Empathy, empathyRankText); // Update the rank text     
    }

    // Humor methods
    public void HumorLevelByOne()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Humor, 1);
        UpdateRankText(PlayerStats.instance.Humor, humorRankText); // Update the rank text    
    }

    public void HumorLevelByTwo()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Humor, 2);
        UpdateRankText(PlayerStats.instance.Humor, humorRankText); // Update the rank text    
    }

    public void HumorLevelByThree()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Humor, 3);
        UpdateRankText(PlayerStats.instance.Humor, humorRankText); // Update the rank text     
    }

    // Endurance methods
    public void EnduranceLevelByOne()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Endurance, 1);
        UpdateRankText(PlayerStats.instance.Endurance, enduranceRankText); // Update the rank text     
    }

    public void EnduranceLevelByTwo()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Endurance, 2);
        UpdateRankText(PlayerStats.instance.Endurance, enduranceRankText); // Update the rank text    
    }

    public void EnduranceLevelByThree()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Endurance, 3);
        UpdateRankText(PlayerStats.instance.Endurance, enduranceRankText); // Update the rank text    
    }

    // Patience methods
    public void PatienceLevelByOne()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Patience, 1);
        UpdateRankText(PlayerStats.instance.Patience, patienceRankText); // Update the rank text     
    }

    public void PatienceLevelByTwo()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Patience, 2);
        UpdateRankText(PlayerStats.instance.Patience, patienceRankText); // Update the rank text     
    }

    public void PatienceLevelByThree()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Patience, 3);
        UpdateRankText(PlayerStats.instance.Patience, patienceRankText); // Update the rank text     
    }

    // Methods for new stats
    public void IntelligenceLevelByOne()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Intelligence, 1);
        UpdateRankText(PlayerStats.instance.Intelligence, intelligenceRankText);
    }

    public void IntelligenceLevelByTwo()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Intelligence, 2);
        UpdateRankText(PlayerStats.instance.Intelligence, intelligenceRankText);
    }

    public void IntelligenceLevelByThree()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Intelligence, 3);
        UpdateRankText(PlayerStats.instance.Intelligence, intelligenceRankText);
    }

    public void LoyaltyLevelByOne()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Courage, 1);
        UpdateRankText(PlayerStats.instance.Courage, courageRankText);
    }

    public void LoyaltyLevelByTwo()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Courage, 2);
        UpdateRankText(PlayerStats.instance.Courage, courageRankText);
    }

    public void LoyaltyLevelByThree()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Courage, 3);
        UpdateRankText(PlayerStats.instance.Courage, courageRankText);
    }

    public void KindnessLevelByOne()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Kindness, 1);
        UpdateRankText(PlayerStats.instance.Kindness, kindnessRankText);
    }

    public void KindnessLevelByTwo()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Kindness, 2);
        UpdateRankText(PlayerStats.instance.Kindness, kindnessRankText);
    }

    public void KindnessLevelByThree()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Kindness, 3);
        UpdateRankText(PlayerStats.instance.Kindness, kindnessRankText);
    }

    public void InsightLevelByOne()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Confidence, 1);
        UpdateRankText(PlayerStats.instance.Confidence, confidenceRankText);
    }

    public void InsightLevelByTwo()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Confidence, 2);
        UpdateRankText(PlayerStats.instance.Confidence, confidenceRankText);
    }

    public void InsightLevelByThree()
    {
        PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Confidence, 3); 
        UpdateRankText(PlayerStats.instance.Confidence, confidenceRankText);
    }

    private IEnumerator FlickerFriendshipSlotColor(Image slot)
    {
        Color originalColor = Color.blue;
        Color flickerColor = new Color(0.6f, 0.6f, 1f); // Brighter blue
        float flickerSpeed = 0.5f;

        while (true)
        {
            slot.color = flickerColor;
            yield return new WaitForSeconds(flickerSpeed);
            slot.color = originalColor;
            yield return new WaitForSeconds(flickerSpeed);
        }
    }

    private IEnumerator FlickerStatSlotColor(Image slot)
    {
        Color originalColor = Color.yellow;
        Color flickerColor = new Color(1f, 1f, 0.6f); // Brighter yellow 
        float flickerSpeed = 0.5f;

        while (true)
        {
            slot.color = flickerColor;
            yield return new WaitForSeconds(flickerSpeed);
            slot.color = originalColor;
            yield return new WaitForSeconds(flickerSpeed);
        }
    }
}     