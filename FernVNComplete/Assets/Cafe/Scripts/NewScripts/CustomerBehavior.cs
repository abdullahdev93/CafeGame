using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CustomerBehavior : MonoBehaviour
{
    public static event System.Action<CustomerBehavior> OnServed; 

    public enum CustomerType
    {
        Regular,
        Impatient,
        GenerousTipper,
        BusyProfessional,
        SocialButterfly,
        Tourist,
        Student,
        Elderly,
        NightOwl,
        Emo,
        Athletic,
        Sad
    }

    public CustomerType customerType;
    public float timer;
    private float initialTime;
    private SpawnPoint spawnPoint;
    private Image timerImage;
    private Image itemImage;
    private bool isSlidingAway;
    private bool buttonAppeared = false;

    private bool customerLovesQuote = false;
    private bool customerLikesQuote = false; 

     // Tutorial flags (set by SpawnManager on tutorial spawns)
    [HideInInspector] public bool showPatienceUI = true;
    [HideInInspector] public bool allowTipsOverride = true;

    // CustomerBehavior.cs  (add near other tutorial flags)
    [HideInInspector] public bool timerPaused = false;
    public void SetTimerPaused(bool paused) => timerPaused = paused; 

    public CafeItem[] cafeItems;
    private CafeItem displayedItem;

    public GameObject priceTipUIPrefab;
    public GameObject addTimeButtonPrefab;

    private float serveTime;
    private bool itemServed = false;
    private BoxCollider2D itemCollider;

    private Button addTimeButton;

    private int addTimeChance;

    public GameObject emojiPrefab;
    private GameObject emojiInstance;

    public Sprite smileEmoji;
    public Sprite happyEmoji;
    public Sprite straightFaceEmoji;
    public Sprite frustratedEmoji;
    public Sprite whateverEmoji;
    public Sprite angryEmoji;
    public Sprite gleefulEmoji; // New emoji for customer loving the quoted item

    private float minTip;
    private float maxTip;

    public void Initialize(SpawnPoint spawnPoint, float gameProgress, CafeItem item)
    {
        this.spawnPoint = spawnPoint;
        this.timer = GetWaitTime(gameProgress);
        this.initialTime = this.timer;
        this.isSlidingAway = false;
        this.buttonAppeared = false;

        // Hide patience UI if requested by tutorial
        if (timerImage != null) timerImage.gameObject.SetActive(showPatienceUI); 

        timerImage = spawnPoint.transform.Find("TimerImage")?.GetComponent<Image>();
        if (timerImage == null)
        {
            GameObject timerImageObj = new GameObject("TimerImage");
            timerImageObj.transform.SetParent(spawnPoint.transform);
            timerImage = timerImageObj.AddComponent<Image>();
            timerImage.type = Image.Type.Filled;
            timerImage.fillMethod = Image.FillMethod.Radial360;
            timerImage.fillOrigin = (int)Image.Origin360.Top;
            timerImage.fillClockwise = false;
            RectTransform timerRectTransform = timerImageObj.GetComponent<RectTransform>();
            timerRectTransform.sizeDelta = new Vector2(100, 100);
            timerRectTransform.localPosition = Vector3.zero;
        }
        //timerImage.gameObject.SetActive(true); 

        // Instead: show/hide based on the tutorial flag
        timerImage.gameObject.SetActive(showPatienceUI);

        //if (timerImage != null) timerImage.fillAmount = 1f;   // NEW: show full ring at start 

        itemImage = spawnPoint.transform.Find("ItemImage")?.GetComponent<Image>();
        if (itemImage == null)
        {
            GameObject itemImageObj = new GameObject("ItemImage");
            itemImageObj.transform.SetParent(spawnPoint.transform);
            itemImage = itemImageObj.AddComponent<Image>();
            RectTransform itemRectTransform = itemImageObj.GetComponent<RectTransform>();
            itemRectTransform.sizeDelta = new Vector2(150, 150);
            itemRectTransform.localPosition = new Vector3(0, 150, 0);
            itemCollider = itemImageObj.AddComponent<BoxCollider2D>();
            itemCollider.isTrigger = true;
        }

        displayedItem = item;
        itemImage.sprite = displayedItem.sprite;

        addTimeButton = Instantiate(addTimeButtonPrefab, spawnPoint.transform).GetComponent<Button>();
        addTimeButton.onClick.AddListener(AddMoreTime);
        addTimeButton.gameObject.SetActive(false);

        addTimeChance = Random.Range(0, 4);

        emojiInstance = Instantiate(emojiPrefab, spawnPoint.transform);
        RectTransform emojiRectTransform = emojiInstance.GetComponent<RectTransform>();
        emojiRectTransform.sizeDelta = new Vector2(75, 75);
        emojiRectTransform.localPosition = new Vector3(100, 200, 0);

        ShowEmoji(happyEmoji);

        // Adjust the tip percentage based on the customer's empathy level and a generic quote type.
        AdjustTipPercentage(GameSession.Instance.playerStats.Empathy.Level, new List<QuoteMenu.QuoteType> { QuoteMenu.QuoteType.Generic });
    }

    private float GetWaitTime(float gameProgress)
    {
        if (gameProgress <= 0.2f) // First 20% of the game
        {
            switch (customerType)
            {
                case CustomerType.Impatient:
                case CustomerType.BusyProfessional:
                    return Random.Range(40f, 60f);
                case CustomerType.SocialButterfly:
                case CustomerType.NightOwl:
                case CustomerType.Sad:
                    return Random.Range(70f, 90f);
                default:
                    return Random.Range(60f, 80f);
            }
        }
        else if (gameProgress <= 0.8f) // Middle 60% of the game
        {
            switch (customerType)
            {
                case CustomerType.Impatient:
                case CustomerType.BusyProfessional:
                    return Random.Range(20f, 40f);
                case CustomerType.SocialButterfly:
                case CustomerType.NightOwl:
                case CustomerType.Sad:
                    return Random.Range(50f, 70f);
                default:
                    return Random.Range(50f, 70f);
            }
        }
        else // Last 20% of the game
        {
            switch (customerType)
            {
                case CustomerType.Impatient:
                case CustomerType.BusyProfessional:
                    return Random.Range(40f, 50f);
                case CustomerType.SocialButterfly:
                case CustomerType.NightOwl:
                case CustomerType.Sad:
                    return Random.Range(60f, 80f);
                default:
                    return Random.Range(50f, 70f);
            }
        }
    }

    private void Update()
    {
        if (isSlidingAway || itemServed) return;

        // If patience is disabled (tutorial’s first customer), do not countdown or timeout.
        if (!showPatienceUI)
        {
            // You can still update emoji if you want a static mood:
            // ShowEmoji(happyEmoji);
            return;
        }

        if (timerPaused) return;      // NEW: gate the countdown until tutorial says go 

        // Normal countdown flow when patience is enabled:
        timer -= Time.deltaTime;

        if (timerImage != null)
        {
            timerImage.fillAmount = timer / initialTime;
        }

        UpdateEmoji();
        CheckPatienceButton();

        if (timer <= 0)
        {
            GameSession.Instance.CustomerNotServed();
            ShowEmoji(angryEmoji);
            StartCoroutine(SlideAwayAndDespawn());
        }
    } 

    private void UpdateEmoji()
    {
        if (timer > 0)
        {
            float timeFraction = timer / initialTime;
            if (timeFraction > 2 / 3f)
            {
                ShowEmoji(happyEmoji);
            }
            else if (timeFraction > 1 / 3f)
            {
                ShowEmoji(straightFaceEmoji);
            }
            else
            {
                ShowEmoji(frustratedEmoji);
            }
        }
    }

    private void CheckPatienceButton()
    {
        if (!buttonAppeared && addTimeChance == 0)
        {
            if (!showPatienceUI) return; // <-- skip in tutorial's first step 

            int patienceLevel = GameSession.Instance.playerStats.Patience.Level;
            if ((patienceLevel > 9 && timer / initialTime <= 0.5f) ||
                (patienceLevel > 8 && timer / initialTime <= 0.35f) ||
                (patienceLevel > 5 && timer / initialTime <= 0.25f))
            {
                addTimeButton.gameObject.SetActive(true);
                buttonAppeared = true;
            }
        }
    }

    private void AddMoreTime()
    {
        timer += initialTime * 0.25f;
        addTimeButton.gameObject.SetActive(false);
    }

    private float CalculateTipPercentage()
    {
        int charismaLevel = GameSession.Instance.playerStats.Charisma.Level;
        float minTip = 0.20f;
        float maxTip = 0.25f;

        if (charismaLevel > 2)
        {
            maxTip = 0.30f;
        }
        if (charismaLevel > 5)
        {
            minTip = 0.25f;
            maxTip = 0.35f;
        }
        if (charismaLevel > 7)
        {
            minTip = 0.30f;
            maxTip = 0.35f;
        }
        if (charismaLevel > 9)
        {
            minTip = 0.35f;
            maxTip = 0.40f;
        }

        return Random.Range(minTip, maxTip);
    }

    public void AdjustTipPercentage(float tipIncrease, List<QuoteMenu.QuoteType> quoteTypes)
    {
        float adjustedTipIncrease = tipIncrease;

        foreach (var quoteType in quoteTypes)
        {
            switch (customerType)
            {
                case CustomerType.Regular:
                    if (quoteType == QuoteMenu.QuoteType.Generic)
                    {
                        adjustedTipIncrease += 0.01f;
                    }
                    break;

                case CustomerType.Impatient:
                    if (quoteType == QuoteMenu.QuoteType.Positivity)
                    {
                        adjustedTipIncrease += 0.01f;
                    }
                    break;

                case CustomerType.GenerousTipper:
                    if (quoteType == QuoteMenu.QuoteType.Generic)
                    {
                        adjustedTipIncrease += 0.01f;
                    }
                    break;

                case CustomerType.BusyProfessional:
                    if (quoteType == QuoteMenu.QuoteType.Motivational)
                    {
                        adjustedTipIncrease += 0.01f;
                    }
                    break;

                case CustomerType.SocialButterfly:
                    if (quoteType == QuoteMenu.QuoteType.Positivity)
                    {
                        adjustedTipIncrease += 0.01f;
                    }
                    break;

                case CustomerType.Tourist:
                    if (quoteType == QuoteMenu.QuoteType.Generic)
                    {
                        adjustedTipIncrease += 0.01f;
                    }
                    break;

                case CustomerType.Student:
                    if (quoteType == QuoteMenu.QuoteType.Inspiring)
                    {
                        adjustedTipIncrease += 0.01f;
                    }
                    break;

                case CustomerType.Elderly:
                    if (quoteType == QuoteMenu.QuoteType.Generic)
                    {
                        adjustedTipIncrease += 0.01f;
                    }
                    break;

                case CustomerType.NightOwl:
                    if (quoteType == QuoteMenu.QuoteType.Generic)
                    {
                        adjustedTipIncrease += 0.01f;
                    }
                    break;

                case CustomerType.Emo:
                    if (quoteType == QuoteMenu.QuoteType.Positivity)
                    {
                        adjustedTipIncrease += 0.01f;
                    }
                    break;

                case CustomerType.Athletic:
                    if (quoteType == QuoteMenu.QuoteType.Motivational)
                    {
                        adjustedTipIncrease += 0.01f;
                    }
                    break;

                case CustomerType.Sad:
                    if (quoteType == QuoteMenu.QuoteType.Positivity)
                    {
                        adjustedTipIncrease += 0.01f;
                    }
                    break;
            }
        }

        minTip += adjustedTipIncrease;
        maxTip += adjustedTipIncrease;
    }

    public void ResetTipPercentage()
    {
        minTip = 0.20f;
        maxTip = 0.25f;
        AdjustTipPercentage(GameSession.Instance.playerStats.Empathy.Level, new List<QuoteMenu.QuoteType> { QuoteMenu.QuoteType.Generic });
    }

    private void ShowPriceAndTip(float price, float tip)
    {
        GameObject priceTipUI = Instantiate(priceTipUIPrefab, transform.position, Quaternion.identity);
        priceTipUI.transform.SetParent(GameObject.Find("Canvas").transform, false);
        Vector3 timerImagePosition = timerImage.transform.position;
        priceTipUI.transform.position = timerImagePosition + new Vector3(0, 300, 0);
        //TextMeshProUGUI priceText = priceTipUI.transform.Find("PriceText").GetComponent<TextMeshProUGUI>();
        //TextMeshProUGUI tipText = priceTipUI.transform.Find("TipText").GetComponent<TextMeshProUGUI>();
        var priceText = priceTipUI.transform.Find("PriceText").GetComponent<TextMeshProUGUI>();
        var tipText = priceTipUI.transform.Find("TipText").GetComponent<TextMeshProUGUI>();

        priceText.text = price % 1 == 0 ? $"${price:F0}" : $"${price:F2}";

        if (tip <= 0.0001f)
            tipText.gameObject.SetActive(false); // hide line entirely when no tip
        else 
            tipText.text = tip % 1 == 0 ? $"+${tip:F0}" : $"+${tip:F2}";

        StartCoroutine(DestroyPriceTipUI(priceTipUI, 2f));
    }

    private IEnumerator DestroyPriceTipUI(GameObject priceTipUI, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(priceTipUI);
        StartCoroutine(SlideAwayAndDespawn());
    }

    private IEnumerator SlideAwayAndDespawn()
    {
        isSlidingAway = true;

        timerImage.gameObject.SetActive(false);
        Destroy(itemImage.gameObject);

        Vector3 startPosition = transform.position;
        yield return new WaitForSeconds(2f);
        Destroy(emojiInstance);
        Vector3 endPosition = new Vector3(transform.position.x - 20, transform.position.y, transform.position.z);
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        Despawn();
    }

    void Despawn()
    {
        spawnPoint.IsOccupied = false;
        if (timerImage != null)
        {
            timerImage.gameObject.SetActive(false);
        }
        if (itemImage != null)
        {
            Destroy(itemImage.gameObject);
        }
        if (emojiInstance != null)
        {
            Destroy(emojiInstance);
        }

        GameSession.Instance.spawnManager.DecreaseItemCount(displayedItem.sprite);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ClickDraggable draggableItem = other.GetComponent<ClickDraggable>();
        if (draggableItem != null)
        {
            SpriteRenderer draggedItemRenderer = draggableItem.GetComponent<SpriteRenderer>();
            if (draggedItemRenderer != null)
            {
                float tipBonus = 0;
                TipBonus tipBonusComponent = draggableItem.GetComponent<TipBonus>();
                if (tipBonusComponent != null)
                {
                    tipBonus = tipBonusComponent.bonus;
                }
                ServeItem(draggedItemRenderer.sprite, tipBonus);
                Destroy(draggableItem.gameObject);
            }
        }
    }

    /*public void ServeItem(Sprite itemSprite, float tipBonus)
    {
        if (timer > 0)
        { 
            if (itemSprite == displayedItem.sprite)
            {
                itemServed = true;

                float tipPercentage = CalculateTipPercentage();
                float tipMultiplier = timer / initialTime;
                float tipAmount = Mathf.Ceil(displayedItem.price * tipPercentage * tipMultiplier * 100f) / 100f;

                // Adjust tips based on quote preference
                var itemComponent = FindObjectOfType<ItemComponent>();
                if (itemComponent != null && itemComponent.quoted)
                {
                    float quoteTipIncrease = itemComponent.quote.tipIncrease;
                    List<QuoteMenu.QuoteType> quoteTypes = itemComponent.quote.types;
                    bool customerLovesQ = false;
                    bool customerLikesQ = false;
                    AdjustTipBasedOnPreference(ref quoteTipIncrease, quoteTypes, itemComponent.quote.text, out customerLovesQ, out customerLikesQ);
                    tipAmount += quoteTipIncrease;

                    if (customerLovesQ)
                    {
                        ShowEmoji(gleefulEmoji); // Show gleeful emoji
                        tipAmount += 0.10f * displayedItem.price; // Increase tip by 10% of item price
                    }
                    else if (customerLikesQ)
                    {
                        ShowEmoji(smileEmoji); // Show smile emoji
                        tipAmount += 0.05f * displayedItem.price; // Increase tip by 5% of item price
                    }
                }

                if (!allowTipsOverride) // <-- ADDED: for first tutorial customer
                {
                    tipAmount = 0f;
                    tipBonus = 0f;
                } 

                float totalAmount = displayedItem.price + tipAmount + tipBonus;

                // Add $0.50 to the final tip amount 
                float finalTipAmount = tipAmount + tipBonus + 0.50f;
                GameSession.Instance.AddMoney(displayedItem.price + finalTipAmount, displayedItem.price, finalTipAmount);

                ShowPriceAndTip(displayedItem.price, finalTipAmount);

                serveTime = initialTime - timer;
                GameSession.Instance.CustomerServed(serveTime);

                OnServed?.Invoke(this); // <-- ADDED: notify tutorial 

                if (timer < initialTime / 3 && !customerLovesQuote)
                {
                    ShowEmoji(whateverEmoji);
                }
                else
                {
                    if (customerLovesQuote)
                    {
                        ShowEmoji(gleefulEmoji); // Show gleeful emoji
                        tipAmount += 0.10f * displayedItem.price; // Increase tip by 10% of item price
                    }
                    else if (customerLikesQuote)
                    {
                        ShowEmoji(smileEmoji); // Show smile emoji
                        tipAmount += 0.05f * displayedItem.price; // Increase tip by 5% of item price
                    }
                }
            }
            else
            {
                GameSession.Instance.CustomerNotServed();
                ShowEmoji(angryEmoji);
                StartCoroutine(SlideAwayAndDespawn());
            }

            ResetTipPercentage(); // Reset tip percentage after serving
        }
    }*/ 

    public void ServeItem(Sprite itemSprite, float tipBonus)
    {
        if (timer <= 0) return;

        if (itemSprite != displayedItem.sprite)
        {
            GameSession.Instance.CustomerNotServed();
            ShowEmoji(angryEmoji);
            StartCoroutine(SlideAwayAndDespawn());
            return;
        }

        itemServed = true;

        float tipAmount = 0f; // default to no tip

        // Only calculate tips if allowed (tutorial’s first customer sets this to false)
        if (allowTipsOverride)
        {
            float tipPct = CalculateTipPercentage();
            float tipMultiplier = timer / initialTime;
            tipAmount = Mathf.Ceil(displayedItem.price * tipPct * tipMultiplier * 100f) / 100f;

            // Quote-based adjustments
            var itemComponent = FindObjectOfType<ItemComponent>();
            if (itemComponent != null && itemComponent.quoted)
            {
                float quoteTipIncrease = itemComponent.quote.tipIncrease;
                List<QuoteMenu.QuoteType> quoteTypes = itemComponent.quote.types;
                bool customerLovesQ, customerLikesQ;
                AdjustTipBasedOnPreference(ref quoteTipIncrease, quoteTypes, itemComponent.quote.text, out customerLovesQ, out customerLikesQ);
                tipAmount += quoteTipIncrease;

                if (customerLovesQ)
                {
                    ShowEmoji(gleefulEmoji);
                    tipAmount += 0.10f * displayedItem.price;
                }
                else if (customerLikesQ)
                {
                    ShowEmoji(smileEmoji);
                    tipAmount += 0.05f * displayedItem.price;
                }
            }

            // Add any per-item tip bonus
            tipAmount += tipBonus;

            // Optional house bump (only when tips are allowed)
            tipAmount += 0.50f;
        }
        else
        {
            // Tips disabled for tutorial: force zero
            tipAmount = 0f;
            tipBonus = 0f;
        }

        // Clamp to 2 decimals
        tipAmount = Mathf.Max(0f, Mathf.Ceil(tipAmount * 100f) / 100f);

        // Money + UI
        GameSession.Instance.AddMoney(displayedItem.price + tipAmount, displayedItem.price, tipAmount);
        ShowPriceAndTip(displayedItem.price, tipAmount);

        serveTime = initialTime - timer;
        GameSession.Instance.CustomerServed(serveTime);

        OnServed?.Invoke(this);

        // Emoji wrap-up (only affects visuals; guard extra tip edits when tips are off)
        if (timer < initialTime / 3f)
            ShowEmoji(whateverEmoji);

        ResetTipPercentage();
    } 

    private void ShowEmoji(Sprite emoji)
    {
        if (emojiInstance != null)
        {
            emojiInstance.GetComponent<Image>().sprite = emoji;
            emojiInstance.SetActive(true);
        }
    }

    private void AdjustTipBasedOnPreference(ref float quoteTipIncrease, List<QuoteMenu.QuoteType> quoteTypes, string quoteText, out bool customerLovesQ, out bool customerLikesQ)
    {
        customerLovesQ = false;
        customerLikesQ = false;
        float tipAdjustment = 0.05f; // Increase by 5%

        // Define specific quotes that customers will love
        Dictionary<CustomerType, List<string>> customerLovedQuotes = new Dictionary<CustomerType, List<string>>
        {
            { CustomerType.Sad, new List<string> { "You are capable of amazing things." } },
            { CustomerType.Athletic, new List<string> { "Wake up with determination. Go to bed with satisfaction." } },
            { CustomerType.BusyProfessional, new List<string> { "Great things never come from comfort zones." } },
            { CustomerType.Elderly, new List<string> { "Espresso yourself with a cup of joy!" } },
            { CustomerType.Emo, new List<string> { "Embrace the glorious mess that you are." } },
            { CustomerType.GenerousTipper, new List<string> { "I like big cups and I cannot lie!" } },
            { CustomerType.Impatient, new List<string> { "The only way to do great work is to love what you do." } },
            { CustomerType.NightOwl, new List<string> { "Happiness is a direction, not a place." } },
            { CustomerType.Regular, new List<string> { "Push yourself, because no one else is going to do it for you." } },
            { CustomerType.SocialButterfly, new List<string> { "Life is short. Smile while you still have teeth." } },
            { CustomerType.Student, new List<string> { "Act as if what you do makes a difference. It does." } },
            { CustomerType.Tourist, new List<string> { "Start each day with a grateful heart." } },
        };

        if (customerLovedQuotes.ContainsKey(customerType) && customerLovedQuotes[customerType].Contains(quoteText))
        {
            customerLovesQ = true;
            quoteTipIncrease += 0.10f; // Increase by 10%
        }
        else
        {
            foreach (var quoteType in quoteTypes)
            {
                switch (customerType)
                {
                    case CustomerType.Sad:
                        if (quoteType == QuoteMenu.QuoteType.Positivity)
                        {
                            customerLikesQ = true;
                            quoteTipIncrease += tipAdjustment;
                        }
                        break;
                    case CustomerType.Athletic:
                        if (quoteType == QuoteMenu.QuoteType.Motivational)
                        {
                            customerLikesQ = true;
                            quoteTipIncrease += tipAdjustment;
                        }
                        break;
                    case CustomerType.BusyProfessional:
                        if (quoteType == QuoteMenu.QuoteType.Inspiring)
                        {
                            customerLikesQ = true;
                            quoteTipIncrease += tipAdjustment;
                        }
                        break;
                    case CustomerType.Elderly:
                        if (quoteType == QuoteMenu.QuoteType.Generic)
                        {
                            customerLikesQ = true;
                            quoteTipIncrease += tipAdjustment;
                        }
                        break;
                    case CustomerType.Emo:
                        if (quoteType == QuoteMenu.QuoteType.Positivity)
                        {
                            customerLikesQ = true;
                            quoteTipIncrease += tipAdjustment;
                        }
                        break;
                    case CustomerType.GenerousTipper:
                        if (quoteType == QuoteMenu.QuoteType.Generic)
                        {
                            customerLikesQ = true;
                            quoteTipIncrease += tipAdjustment;
                        }
                        break;
                    case CustomerType.Impatient:
                        if (quoteType == QuoteMenu.QuoteType.Positivity)
                        {
                            customerLikesQ = true;
                            quoteTipIncrease += tipAdjustment;
                        }
                        break;
                    case CustomerType.NightOwl:
                        if (quoteType == QuoteMenu.QuoteType.Generic)
                        {
                            customerLikesQ = true;
                            quoteTipIncrease += tipAdjustment;
                        }
                        break;
                    case CustomerType.Regular:
                        if (quoteType == QuoteMenu.QuoteType.Generic)
                        {
                            customerLikesQ = true;
                            quoteTipIncrease += tipAdjustment;
                        }
                        break;
                    case CustomerType.SocialButterfly:
                        if (quoteType == QuoteMenu.QuoteType.Generic)
                        {
                            customerLikesQ = true;
                            quoteTipIncrease += tipAdjustment;
                        }
                        break;
                    case CustomerType.Student:
                        if (quoteType == QuoteMenu.QuoteType.Inspiring)
                        {
                            customerLikesQ = true;
                            quoteTipIncrease += tipAdjustment;
                        }
                        break;
                    case CustomerType.Tourist:
                        if (quoteType == QuoteMenu.QuoteType.Positivity)
                        {
                            customerLikesQ = true;
                            quoteTipIncrease += tipAdjustment;
                        }
                        break;
                    default:
                        quoteTipIncrease += tipAdjustment; // Default adjustment for unspecified cases
                        break;
                }
            }
        }

        Debug.Log($"Customer Type: {customerType}, Loves Quote: {customerLovesQ}, Likes Quote Type: {customerLikesQ}");
    }
}
