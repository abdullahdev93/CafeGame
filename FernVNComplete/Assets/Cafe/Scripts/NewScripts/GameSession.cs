using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance;

    private float totalMoney;
    private float totalEarnings;
    private float totalTips;
    private float totalPrices;
    private int servedCustomers;
    private int notServedCustomers;
    private int consecutiveFailedCustomers;
    private float fastestServeTime = float.MaxValue;
    private float slowestServeTime = 0f;

    public TextMeshProUGUI totalEarningsText;
    public TextMeshProUGUI totalCurrentEarningsText; 
    public GameObject[] timeLimitForGameM;
    public GameObject[] timeLimitForGameA;
    public GameObject[] timeLimitForGameE;
    public float timeToEndCafeGame = 360f;
    private float initialTime;

    public SpawnManager spawnManager;
    public GameObject endGameMenu;
    public TextMeshProUGUI servedCustomersText;
    public TextMeshProUGUI notServedCustomersText;
    public TextMeshProUGUI totalPricesText;
    public TextMeshProUGUI totalTipsText;
    public TextMeshProUGUI totalMoneyAmountText;
    public TextMeshProUGUI fastestServeTimeText;
    public TextMeshProUGUI slowestServeTimeText;
    public TextMeshProUGUI seasonDayText;
    public TextMeshProUGUI timeOfDayText;

    public Animator earningsAnimator; 

    public Button exitTheCafe; 

    public PlayerStats playerStats;
    private bool isGameStarted = false;

    public bool IsExhausted { get; private set; } = false;
    public int exhaustionPlayCount = 0; // Tracks consecutive plays with Exhaustion
    public int consecutiveGameCount = 0; // Tracks consecutive game plays

    private float weatherSpawnMult = 1f;
    private float weatherTipMult = 1f; // 1 + percent/100 

    public bool isSick { get; private set; } = false; // Boolean for sickness

    public float TotalMoney
    {
        get { return totalMoney; }
        set { totalMoney = value; }
    }

    public void Start()
    {
        exitTheCafe.onClick.AddListener(() => OnTownMapButtonClicked());

        UpdateSeasonDayText();

        // Load total earnings from PlayerPrefs, shared between Cafe and TownMap
        totalEarnings = PlayerPrefs.GetFloat("TotalMoney", 10000); // Default to $1000 if not found 
        //totalEarnings = PlayerPrefs.GetFloat("CurrentMoney", 1000); // Default to $1000 if not found 
        //totalMoney = totalEarnings;  // Update totalMoney with the same value to ensure consistency 
        Debug.Log($"Loaded CurrentMoney: {totalEarnings:N2}");

        // Load consecutive game count and exhaustion play count from PlayerPrefs
        consecutiveGameCount = PlayerPrefs.GetInt("ConsecutiveGameCount", 0);
        exhaustionPlayCount = PlayerPrefs.GetInt("ExhaustionPlayCount", 0);
        IsExhausted = PlayerPrefs.GetInt("IsExhausted", 0) == 1;

        //totalCurrentEarningsText.gameObject.SetActive(false); 

        UpdateEarningsText();
        //SaveEarnings();

        totalCurrentEarningsText.gameObject.SetActive(false);  
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }

        initialTime = timeToEndCafeGame;

        AssignComponentsByTag(); 
    }

    private void Update()
    {
        if (!isGameStarted) return; 

        UpdateTimeLimitMiniGames();

        if (timeToEndCafeGame <= 0f)
        {
            timeLimitForGameM[5].SetActive(false);
            timeLimitForGameM[6].SetActive(true);

            if (!AreCustomersRemaining())
            {
                EndGame();
            }
            return;
        }

        timeToEndCafeGame -= Time.deltaTime;
        UpdateDifficultyCurve();  
    }

    private void AssignComponentsByTag()
    {
        if (totalEarningsText == null)
            totalEarningsText = GameObject.FindWithTag("EarningsText").GetComponent<TextMeshProUGUI>();

        if (totalCurrentEarningsText == null)
            totalCurrentEarningsText = GameObject.FindWithTag("AllMoney").GetComponent<TextMeshProUGUI>(); 

        if (timeLimitForGameM == null || timeLimitForGameM.Length == 0)
        {
            timeLimitForGameM[0] = GameObject.Find("StartingHour");
            timeLimitForGameM[1] = GameObject.Find("HourOne");
            timeLimitForGameM[2] = GameObject.Find("HourTwo");
            timeLimitForGameM[3] = GameObject.Find("HourThree");
            timeLimitForGameM[4] = GameObject.Find("HourFour");
            timeLimitForGameM[5] = GameObject.Find("HourFive");
            timeLimitForGameM[6] = GameObject.Find("HourSix");
        }

        if (timeLimitForGameA == null || timeLimitForGameA.Length == 0)
        {
            timeLimitForGameA[0] = GameObject.Find("StartingHour");
            timeLimitForGameA[1] = GameObject.Find("HourOne");
            timeLimitForGameA[2] = GameObject.Find("HourTwo");
            timeLimitForGameA[3] = GameObject.Find("HourThree");
            timeLimitForGameA[4] = GameObject.Find("HourFour");
            timeLimitForGameA[5] = GameObject.Find("HourFive");
            timeLimitForGameA[6] = GameObject.Find("HourSix");
        }

        if (timeLimitForGameE == null || timeLimitForGameE.Length == 0)
        {
            timeLimitForGameE[0] = GameObject.Find("StartingHour");
            timeLimitForGameE[1] = GameObject.Find("HourOne");
            timeLimitForGameE[2] = GameObject.Find("HourTwo");
            timeLimitForGameE[3] = GameObject.Find("HourThree");
            timeLimitForGameE[4] = GameObject.Find("HourFour");
            timeLimitForGameE[5] = GameObject.Find("HourFive");
            timeLimitForGameE[6] = GameObject.Find("HourSix");
        }

        if (spawnManager == null)
            spawnManager = GameObject.FindWithTag("SpawnManager").GetComponent<SpawnManager>();

        if (endGameMenu == null)
            endGameMenu = GameObject.FindWithTag("EndGameMenu");

        if (servedCustomersText == null)
            servedCustomersText = GameObject.FindWithTag("CustomersServed").GetComponent<TextMeshProUGUI>();

        if (notServedCustomersText == null)
            notServedCustomersText = GameObject.FindWithTag("CustomersNotServed").GetComponent<TextMeshProUGUI>();

        if (totalPricesText == null)
            totalPricesText = GameObject.FindWithTag("MoneyFromPrices").GetComponent<TextMeshProUGUI>();

        if (totalTipsText == null)
            totalTipsText = GameObject.FindWithTag("MoneyFromTips").GetComponent<TextMeshProUGUI>();

        if (totalMoneyAmountText == null)
            totalMoneyAmountText = GameObject.FindWithTag("TotalMoney").GetComponent<TextMeshProUGUI>();

        if (fastestServeTimeText == null)
            fastestServeTimeText = GameObject.FindWithTag("FasteestTimeServed").GetComponent<TextMeshProUGUI>();

        if (slowestServeTimeText == null)
            slowestServeTimeText = GameObject.FindWithTag("SlowestTimeServed").GetComponent<TextMeshProUGUI>();

        // Find and assign the TextMeshPro components again in case they were lost during the scene transition
        if (seasonDayText == null)
            seasonDayText = GameObject.FindWithTag("SeasonDayText").GetComponent<TextMeshProUGUI>();
        
        if (timeOfDayText == null) 
            timeOfDayText = GameObject.FindWithTag("ActivityText").GetComponent<TextMeshProUGUI>();

        if (earningsAnimator == null)
            earningsAnimator = GameObject.FindWithTag("EarningsText").GetComponent<Animator>();   

        if (exitTheCafe == null)
            exitTheCafe = GameObject.FindWithTag("ExitTheCafeGame").GetComponent<Button>();

        if (playerStats == null)
            playerStats = GameObject.FindWithTag("PlayerStats").GetComponent<PlayerStats>();
    }

    public void UpdateSeasonDayText()
    {
        if (CalendarSystem.Instance != null)
        {
            CalendarSystem.Season currentSeason = CalendarSystem.Instance.GetCurrentSeason();
            int currentDay = CalendarSystem.Instance.GetCurrentDay();
            string seasonName = currentSeason.ToString();

            if (seasonDayText != null)
            {
                seasonDayText.text = $"{seasonName} / {currentDay:D2}";
            }

            if (timeOfDayText != null)
            {
                int activityCounter = CalendarSystem.Instance.activityCounter;
                timeOfDayText.text = CalendarSystem.Instance.activityPhases[activityCounter];
            }
        }
    }

    public void ApplyPlayerStats(PlayerStats stats)
    {
        playerStats = stats;
        if (playerStats.Charisma.Level > 3)
        {
            totalTips *= 1.2f;
        }
        if (playerStats.Endurance.Level > 4)
        {
            spawnManager.customerTimer *= 1.5f;
        }

        //UpgradeMenu.Instance.UnlockUpgradesBasedOnCreativity(playerStats.Creativity.Level);
        QuoteMenu.Instance.UnlockQuotesBasedOnCreativity(playerStats.Creativity.Level);
        QuoteMenu.Instance.SetEmpathyLevel(playerStats.Empathy.Level);
    }

    public void StartCafeGame()
    {
        isGameStarted = true;

        // Track consecutive plays
        consecutiveGameCount++;

        PlayerPrefs.SetInt("ConsecutiveGameCount", consecutiveGameCount);
        PlayerPrefs.Save();

        // Automatically set exhaustion after 21 consecutive games
        if (consecutiveGameCount >= 21) 
        {
            IsExhausted = true;
            PlayerPrefs.SetInt("IsExhausted", 1);  // Save exhaustion status
            PlayerPrefs.Save(); 
            Debug.Log("Player has become Exhausted after 21 consecutive plays.");
        }

        // Track consecutive plays with Exhaustion enabled
        if (IsExhausted)
        {
            exhaustionPlayCount++;
            PlayerPrefs.SetInt("ExhaustionPlayCount", exhaustionPlayCount);
            PlayerPrefs.Save(); 

            if (exhaustionPlayCount >= 15)
            {
                isSick = true; // Set Sick to TRUE after 15 consecutive Exhausted plays
                Debug.Log("Player is now Sick due to consecutive Exhausted plays.");
            }
        }
        else
        {
            exhaustionPlayCount = 0; // Reset the counter if not Exhausted 
            PlayerPrefs.SetInt("ExhaustionPlayCount", exhaustionPlayCount);
            PlayerPrefs.Save(); 
        }

        UpdateWeatherEffectsForCafeRun(); 
    }

    public void ResetConsecutiveGameCount()
    {
        consecutiveGameCount = 0;
        exhaustionPlayCount = 0;
        IsExhausted = false;

        // Reset values in PlayerPrefs
        PlayerPrefs.SetInt("ConsecutiveGameCount", consecutiveGameCount);
        PlayerPrefs.SetInt("ExhaustionPlayCount", exhaustionPlayCount);
        PlayerPrefs.SetInt("IsExhausted", 0);
        PlayerPrefs.Save();
    }

    /*public void AddMoney(float totalAmount, float price, float tip)
    { 
        totalMoney += Mathf.Round((price + tip) * 100f) / 100f;
        totalPrices += Mathf.Round(price * 100f) / 100f;
        totalTips += Mathf.Round(tip * 100f) / 100f;
        totalEarningsText.text = $"Earnings: ${FormatMoney(totalMoney)}";
        Debug.Log($"Money added: {price + tip:F2}, Total money: {totalMoney:F2}");

        // Save updated total earnings to PlayerPrefs
        PlayerPrefs.SetFloat("TotalMoney", totalMoney);
        PlayerPrefs.Save(); 
    }*/

    public void AddMoney(float totalAmount, float price, float tip)
    {
        // Apply weather tip modifier here
        float adjustedTip = Mathf.Round(tip * weatherTipMult * 100f) / 100f;

        totalMoney += Mathf.Round((price + adjustedTip) * 100f) / 100f;
        totalPrices += Mathf.Round(price * 100f) / 100f;
        totalTips += Mathf.Round(adjustedTip * 100f) / 100f;

        totalEarningsText.text = $"Earnings: ${FormatMoney(totalMoney)}";
        Debug.Log($"Money added: {price + adjustedTip:F2} (tip {adjustedTip:F2}), Total money: {totalMoney:F2}");

        PlayerPrefs.SetFloat("TotalMoney", totalMoney);
        PlayerPrefs.Save();
    } 

    public float GetTotalMoney()
    {
        return totalMoney;
    }

    public void DeductMoney(float amount)
    {
        totalMoney -= Mathf.Round(amount * 100f) / 100f;
    }

    public void CustomerServed(float serveTime)
    {
        servedCustomers++;
        consecutiveFailedCustomers = 0;
        if (serveTime < fastestServeTime)
        {
            fastestServeTime = serveTime;
        }
        if (serveTime > slowestServeTime)
        {
            slowestServeTime = serveTime;
        }
    }

    public void CustomerNotServed()
    {
        notServedCustomers++;
        consecutiveFailedCustomers++;

        if (consecutiveFailedCustomers >= 4)
        {
            GameOver();
        }
    }

    private GameObject[] GetCurrentTimeLimitArray()
    {
        int activityCounter = CalendarSystem.Instance.activityCounter;

        if (activityCounter == 0)
        {
            return timeLimitForGameM;
        }
        else if (activityCounter == 1)
        {
            return timeLimitForGameA;
        }
        else
        {
            return timeLimitForGameE;
        }
    }

    private void DisableAllTimeLimitMiniGames()
    {
        GameObject[] timeLimits = GetCurrentTimeLimitArray();

        foreach (GameObject miniGame in timeLimits)
        {
            miniGame.SetActive(false);
        }
    }

    private void UpdateTimeLimitMiniGames()
    {
        DisableAllTimeLimitMiniGames();

        GameObject[] timeLimits = GetCurrentTimeLimitArray();
        float interval = initialTime / 6f;

        if (timeToEndCafeGame <= interval)
        {
            timeLimits[5].SetActive(true);
        }
        else if (timeToEndCafeGame <= 2 * interval)
        {
            timeLimits[4].SetActive(true);
        }
        else if (timeToEndCafeGame <= 3 * interval)
        {
            timeLimits[3].SetActive(true);
        }
        else if (timeToEndCafeGame <= 4 * interval)
        {
            timeLimits[2].SetActive(true);
        }
        else if (timeToEndCafeGame <= 5 * interval)
        {
            timeLimits[1].SetActive(true);
        }
        else if (timeToEndCafeGame <= 6 * interval)
        {
            timeLimits[0].SetActive(true);
        }
    }

    /*private void UpdateDifficultyCurve()
    {
        float elapsedTime = initialTime - timeToEndCafeGame;
        float gameProgress = elapsedTime / initialTime;

        if (spawnManager != null)
        {
            if (gameProgress <= 0.2f)
            {
                spawnManager.minSpawnTime = Random.Range(10f, 15f);
                spawnManager.maxSpawnTime = Random.Range(20f, 25f);
                spawnManager.customerTimer = Random.Range(60f, 65f);
            }
            else if (gameProgress <= 0.8f)
            {
                float progressFactor = (gameProgress - 0.2f) / 0.6f;
                spawnManager.minSpawnTime = Mathf.Lerp(10f, 5f, progressFactor);
                spawnManager.maxSpawnTime = Mathf.Lerp(20f, 10f, progressFactor);
                spawnManager.customerTimer = Mathf.Lerp(60f, 30f, progressFactor);
            }
            else
            {
                spawnManager.minSpawnTime = Random.Range(15f, 20f);
                spawnManager.maxSpawnTime = Random.Range(25f, 30f);
                spawnManager.customerTimer = Random.Range(45f, 50f);

                if (timeToEndCafeGame <= 20f)
                {
                    spawnManager.StopSpawning();
                }
            }
        }
    }*/

    private void UpdateDifficultyCurve()
    {
        float elapsedTime = initialTime - timeToEndCafeGame;
        float gameProgress = elapsedTime / initialTime;

        if (spawnManager != null)
        {
            if (gameProgress <= 0.2f)
            {
                spawnManager.minSpawnTime = Random.Range(10f, 15f);
                spawnManager.maxSpawnTime = Random.Range(20f, 25f);
                spawnManager.customerTimer = Random.Range(60f, 65f);
            }
            else if (gameProgress <= 0.8f)
            {
                float progressFactor = (gameProgress - 0.2f) / 0.6f;
                spawnManager.minSpawnTime = Mathf.Lerp(10f, 5f, progressFactor);
                spawnManager.maxSpawnTime = Mathf.Lerp(20f, 10f, progressFactor);
                spawnManager.customerTimer = Mathf.Lerp(60f, 30f, progressFactor);
            }
            else
            {
                spawnManager.minSpawnTime = Random.Range(15f, 20f);
                spawnManager.maxSpawnTime = Random.Range(25f, 30f);
                spawnManager.customerTimer = Random.Range(45f, 50f);

                if (timeToEndCafeGame <= 20f)
                    spawnManager.StopSpawning();
            }

            // Apply weather spawn-rate multiplier last (lower times => faster spawns)
            float m = Mathf.Max(0.25f, weatherSpawnMult); // clamp just in case
            spawnManager.minSpawnTime = Mathf.Max(0.5f, spawnManager.minSpawnTime / m);
            spawnManager.maxSpawnTime = Mathf.Max(1.0f, spawnManager.maxSpawnTime / m);
        }
    } 

    private bool AreCustomersRemaining()
    {
        foreach (var spawnPoint in spawnManager.spawnPoints)
        {
            if (spawnPoint.IsOccupied)
            {
                return true;
            }
        }
        return false;
    }

    private void EndGame()
    {
        DisableAllTimeLimitMiniGames();
        GetCurrentTimeLimitArray()[6].SetActive(true);
        timeToEndCafeGame = 0f;
        //Time.timeScale = 0f;

        servedCustomersText.text = $"Customers Served: {servedCustomers}";
        notServedCustomersText.text = $"Customers Not Served: {notServedCustomers}";
        totalPricesText.text = $"Money: ${FormatMoney(totalPrices)}";
        totalTipsText.text = $"Tips: ${FormatMoney(totalTips)}";
        totalMoneyAmountText.text = $"Total Money: ${FormatMoney(totalPrices + totalTips)}";

        fastestServeTimeText.text = fastestServeTime < float.MaxValue ? $"Fastest Serve: {Mathf.Floor(fastestServeTime)} s" : "Fastest Serve: 0 s";
        slowestServeTimeText.text = slowestServeTime > 0 ? $"Slowest Serve: {Mathf.Floor(slowestServeTime)} s" : "Slowest Serve: 0 s";

        if (UpgradeMenu.Instance != null)
        {
            UpgradeMenu.Instance.TotalMoney += totalMoney;
        }

        PlayerPrefs.SetFloat("CafeTotalMoney", totalMoney);

        endGameMenu.SetActive(true);
    }

    private void GameOver()
    {
        //Time.timeScale = 0f;

        servedCustomersText.text = $"Customers Served: {servedCustomers}";
        notServedCustomersText.text = $"Customers Not Served: {notServedCustomers}";
        totalPricesText.text = $"Money: ${FormatMoney(totalPrices)}";
        totalTipsText.text = $"Tips: ${FormatMoney(totalTips)}";
        totalMoneyAmountText.text = $"Total Money: ${FormatMoney(totalMoney)}";

        fastestServeTimeText.text = fastestServeTime < float.MaxValue ? $"Fastest Serve: {Mathf.Floor(fastestServeTime)} s" : "Fastest Serve: 0 s";
        slowestServeTimeText.text = slowestServeTime > 0 ? $"Slowest Serve: {Mathf.Floor(slowestServeTime)} s" : "Slowest Serve: 0 s";

        if (UpgradeMenu.Instance != null)
        {
            UpgradeMenu.Instance.TotalMoney += totalMoney;
        }

        PlayerPrefs.SetFloat("CafeTotalMoney", totalMoney);

        endGameMenu.SetActive(true);
    }

    public void ToggleExhaustion()
    {
        IsExhausted = !IsExhausted;
        if (IsExhausted)
        {
            IncreaseRestockAndDecreaseCustomerTimers();
        }
        else
        {
            ResetTimersToDefault();
        }
    }

    private void IncreaseRestockAndDecreaseCustomerTimers()
    {
        foreach (var item in FindObjectsOfType<CoffeeMachineCom>())
        {
            item.restockTime += 3f;
        }

        foreach (var item in FindObjectsOfType<LemonadeDispenser>())
        {
            item.restockTime += 3f;
        }

        foreach (var item in FindObjectsOfType<CupStack>())
        {
            item.timerDuration += 3f;
        }

        foreach (var item in FindObjectsOfType<CoffeeBeanJar>())
        {
            item.timerDuration += 3f;
        }

        spawnManager.customerTimer -= 5f;
    }

    private void ResetTimersToDefault()
    {
        foreach (var item in FindObjectsOfType<CoffeeMachineCom>())
        {
            item.restockTime -= 3f;
        }

        foreach (var item in FindObjectsOfType<LemonadeDispenser>())
        {
            item.restockTime -= 3f;
        }

        foreach (var item in FindObjectsOfType<CupStack>())
        {
            item.timerDuration -= 3f;
        }

        foreach (var item in FindObjectsOfType<CoffeeBeanJar>())
        {
            item.timerDuration -= 3f;
        }

        spawnManager.customerTimer += 5f;
    }

    private IEnumerator ShowEarningsAndPlayAnimation()
    { 
        totalCurrentEarningsText.text = $"Earnings: ${FormatMoney(totalEarnings)}";
        //totalCurrentEarningsText.gameObject.SetActive(true); // Make total earnings visible

        yield return new WaitForSecondsRealtime(1f); 

        // Assuming you have an Animator attached to the earnings text or a related UI element
        //earningsAnimator = totalEarningsText.GetComponent<Animator>();
        if (earningsAnimator != null)
        { 
            earningsAnimator.SetTrigger("CafeMoneyAdded"); // Trigger the money animation 
        } 


    }

    public void OnTownMapButtonClicked()
    {
        totalCurrentEarningsText.gameObject.SetActive(true);

        StartCoroutine(PlayEarningsAndTransition()); 

    }

    private IEnumerator PlayEarningsAndTransition()
    {
        // Play earnings animation
        yield return StartCoroutine(ShowEarningsAndPlayAnimation());

        // Hide total earnings text after animation
        yield return StartCoroutine(AddMoneyAfterAnimation());

        // Transition to the next scene after delay
        yield return StartCoroutine(ExitCafeAfterDelay());
    }

    private IEnumerator AddMoneyAfterAnimation()
    {
        yield return new WaitForSecondsRealtime(0.8f);    
        totalEarningsText.gameObject.SetActive(false);
        totalEarnings += totalMoney;

        // Update the text to reflect new earnings
        totalCurrentEarningsText.text = $"Earnings: ${FormatMoney(totalEarnings)}";

        // Save the new totalEarnings value to be transferred to TownMap
        PlayerPrefs.SetFloat("CafeTotalMoney", totalEarnings);  

        // Save the new totalEarnings value
        SaveEarnings();
    }

    private IEnumerator ExitCafeAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2f); // Adjust time to match animation duration   
                                                     //totalCurrentEarningsText.gameObject.SetActive(false); // Make total earnings visible 

        // Save the total earnings to PlayerPrefs before transitioning
        PlayerPrefs.SetFloat("TotalMoney", totalEarnings); 
        // Save the total earnings and proceed to the next scene 
        //PlayerPrefs.SetFloat("CafeTotalMoney", totalEarnings);
        PlayerPrefs.SetString("LastScene", "Cafe");
        PlayerPrefs.Save(); 

        //this.enabled = false;

        if (CalendarSystem.Instance.activityCounter == 2)
        { 
            //CalendarSystem.Instance.AdvanceActivityOrDay();
            HandleHangOutClick("AfterGoodCafeALT");   
        }

        /*if (CalendarSystem.Instance.activityCounter < 2)
        {
            // Load Visual Novel scene and start MeiHangOutOne dialogue
            PlayerPrefs.SetString("VN_TextFile", "MeiHangOutOne"); // Save the text file to be loaded
            PlayerPrefs.Save();

            // Transfer to Visual Novel scene
            SceneManager.LoadScene("VisualNovel");
        }*/ 

        else
        {
            //CalendarSystem.Instance.AdvanceActivityOrDay();
            HandleHangOutClick("AfterGoodCafe"); 
            //SceneManager.LoadScene("TownMap");
        }
    }

    private void HandleHangOutClick(string locationFile)
    {
        // Set the starting file based on the location
        PlayerPrefs.SetString("StartingFile", locationFile);
        PlayerPrefs.Save();

        // Transfer to the VisualNovel scene
        SceneManager.LoadScene("VisualNovel");
    }

    private void UpdateEarningsText()
    {
        //totalEarningsText.text = $" Total: ${totalEarnings:N2}"; 
        totalCurrentEarningsText.text = $"Earnings: ${totalEarnings:N2}";  
    }

    /*private void UpdateWeatherEffectsForCafeRun()
    {
        var cal = CalendarSystem.Instance;
        var w = cal.GetCurrentWeather();
        int phase = cal.activityCounter;

        weatherSpawnMult = WeatherSystem.GetSpawnRateMultiplier(w, phase);
        int tipPct = WeatherSystem.GetTipModifierPercent(w, phase);
        weatherTipMult = 1f + (tipPct / 100f);

        // Let SpawnManager know we're using weather-adjusted spawning
        if (spawnManager != null) spawnManager.StartGame();

        Debug.Log($"[Cafe Weather Effects] {CalendarSystem.Instance.GetCurrentWeather()} {CalendarSystem.Instance.activityPhases[CalendarSystem.Instance.activityCounter]} | Spawn x{weatherSpawnMult:F2} | Tip {(weatherTipMult - 1f) * 100f:+0;-0}%"); 
    }*/

    private void UpdateWeatherEffectsForCafeRun()
    {
        var cal = CalendarSystem.Instance;
        var w = cal.GetCurrentWeather();
        var season = cal.GetCurrentSeason();
        var phase = cal.activityCounter;              // 0=Morning,1=Afternoon,2=Evening
        var weekday = cal.GetCurrentDayOfWeek();        // "Monday"... "Sunday"

        // Weather × Time
        float weatherSpawn = WeatherSystem.GetSpawnRateMultiplier(w, phase);
        int weatherTips = WeatherSystem.GetTipModifierPercent(w, phase);

        // Season × Time
        float seasonSpawn = WeatherSystem.GetSeasonSpawnMultiplier(season, phase);
        int seasonTips = WeatherSystem.GetSeasonTipBonusPercent(season, phase);

        // Weekday × Time
        float weekdaySpawn = WeatherSystem.GetWeekdaySpawnMultiplier(weekday, phase);
        int weekdayTips = WeatherSystem.GetWeekdayTipBonusPercent(weekday, phase);

        // Final combined
        weatherSpawnMult = weatherSpawn * seasonSpawn * weekdaySpawn;

        int totalTipPercent = weatherTips + seasonTips + weekdayTips;
        weatherTipMult = 1f + (totalTipPercent / 100f);

        Debug.Log($"[Cafe Effects] {weekday} | {season} | {w} | {cal.activityPhases[phase]}  ->  " +
                  $"Spawn x{weatherSpawnMult:F3}, Tips {totalTipPercent:+0;-0}%");
    } 

    private void SaveEarnings()
    {
        PlayerPrefs.SetFloat("CurrentMoney", totalEarnings); 
        PlayerPrefs.Save();
    }

    public void ResetTotalMoney()
    {
        totalMoney = 0;
    }

    private string FormatMoney(float amount)
    {
        return amount.ToString("F2");
    }

    public float GetElapsedTime()
    {
        return initialTime - timeToEndCafeGame;
    }

    public float GetInitialTime()
    {
        return initialTime;
    }
}
