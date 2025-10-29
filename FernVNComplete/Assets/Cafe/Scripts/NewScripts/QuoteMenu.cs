using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuoteMenu : MonoBehaviour
{
    public static QuoteMenu Instance;

    public enum QuoteType
    {
        Generic,
        Positivity,
        Motivational,
        Inspiring,
        Humor
    }

    [System.Serializable]
    public class Quote
    {
        public string text;
        public float tipIncrease;
        public List<QuoteType> types;
        public bool used = false;
    }

    public List<Quote> positivityQuotes;
    public List<Quote> humorQuotes;
    public List<Quote> inspiringQuotes;
    public List<Quote> motivationalQuotes;
    public List<Quote> extraQuotes;

    public GameObject quoteUIPrefab;
    public Transform quoteContainer;
    public GameObject quoteMenuUI;
    public GameObject mainMenu;
    public GameObject listMenu;
    public Button backToMainMenuButton;
    public Button hideQuoteMenuButton;
    public Button hideQuoteAndButtonsMenuButton;

    public Button addQuoteButton1;
    public Button noButton1;
    public Button addQuoteButton2;
    public Button noButton2;
    public Button addQuoteButton3;
    public Button noButton3;

    public Button positivityButton;
    public Button humorButton;
    public Button inspiringButton;
    public Button motivationalButton;

    private GameObject snappedItem;
    private Dictionary<int, Button> addQuoteButtons = new Dictionary<int, Button>();
    private Dictionary<int, Button> noButtons = new Dictionary<int, Button>();
    private HashSet<int> disabledSnapPoints = new HashSet<int>(); // Track disabled snap points
    public int selectedSnapPointIndex = -1;
    private int empathyLevel;

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
        mainMenu.SetActive(false);
        quoteMenuUI.SetActive(false);

        addQuoteButton1.gameObject.SetActive(false);
        noButton1.gameObject.SetActive(false);
        addQuoteButton2.gameObject.SetActive(false);
        noButton2.gameObject.SetActive(false);
        addQuoteButton3.gameObject.SetActive(false);
        noButton3.gameObject.SetActive(false);

        addQuoteButtons.Add(1, addQuoteButton1);
        noButtons.Add(1, noButton1);
        addQuoteButtons.Add(2, addQuoteButton2);
        noButtons.Add(2, noButton2);
        addQuoteButtons.Add(3, addQuoteButton3);
        noButtons.Add(3, noButton3);

        addQuoteButton1.onClick.AddListener(() => ShowQuoteMenu(1));
        noButton1.onClick.AddListener(() => DisableButtons(1)); // Disable buttons on No click
        addQuoteButton2.onClick.AddListener(() => ShowQuoteMenu(2));
        noButton2.onClick.AddListener(() => DisableButtons(2)); // Disable buttons on No click
        addQuoteButton3.onClick.AddListener(() => ShowQuoteMenu(3));
        noButton3.onClick.AddListener(() => DisableButtons(3)); // Disable buttons on No click

        positivityButton.onClick.AddListener(() => ShowQuoteList(positivityQuotes));
        humorButton.onClick.AddListener(() => ShowQuoteList(humorQuotes));
        inspiringButton.onClick.AddListener(() => ShowQuoteList(inspiringQuotes));
        motivationalButton.onClick.AddListener(() => ShowQuoteList(motivationalQuotes));
        backToMainMenuButton.onClick.AddListener(() => BackToMainMenu());
        hideQuoteMenuButton.onClick.AddListener(() => HideQuoteMenu());
        hideQuoteAndButtonsMenuButton.onClick.AddListener(() => HideQuoteAndButtonsMenu());

        // Add extra quotes to their respective lists
        for (int i = 0; i < extraQuotes.Count; i++)
        {
            var extraQuote = extraQuotes[i];
            if (extraQuote.types.Contains(QuoteType.Positivity))
            {
                positivityQuotes.Add(extraQuote);
            }
            if (extraQuote.types.Contains(QuoteType.Humor))
            {
                humorQuotes.Add(extraQuote);
            }
            if (extraQuote.types.Contains(QuoteType.Inspiring))
            {
                inspiringQuotes.Add(extraQuote);
            }
            if (extraQuote.types.Contains(QuoteType.Motivational))
            {
                motivationalQuotes.Add(extraQuote);
            }
        }
    }

    public void SetEmpathyLevel(int level)
    {
        empathyLevel = level;
    }

    public void SnapItem(GameObject item, int snapPointIndex)
    {
        snappedItem = item;
        selectedSnapPointIndex = snapPointIndex;

        var itemComponent = item.GetComponent<ItemComponent>();
        if (empathyLevel > 2 && itemComponent != null && !itemComponent.quoted && !disabledSnapPoints.Contains(snapPointIndex))
        {
            ShowAddQuoteAndNoButtons(snapPointIndex);
        }
    }

    private void ShowAddQuoteAndNoButtons(int snapPointIndex)
    {
        if (snappedItem != null)
        {
            var itemComponent = snappedItem.GetComponent<ItemComponent>();
            if (itemComponent != null && !itemComponent.quoted)
            {
                if (addQuoteButtons.ContainsKey(snapPointIndex))
                {
                    addQuoteButtons[snapPointIndex].gameObject.SetActive(true);
                }
                if (noButtons.ContainsKey(snapPointIndex))
                {
                    noButtons[snapPointIndex].gameObject.SetActive(true);
                }
            }
        }
    }

    public void HideButtons(int snapPointIndex)
    {
        if (addQuoteButtons.ContainsKey(snapPointIndex))
        {
            addQuoteButtons[snapPointIndex].gameObject.SetActive(false);
        }
        if (noButtons.ContainsKey(snapPointIndex))
        {
            noButtons[snapPointIndex].gameObject.SetActive(false);
        }
    }

    private void DisableButtons(int snapPointIndex)
    {
        HideButtons(snapPointIndex);
        disabledSnapPoints.Add(snapPointIndex); // Add to disabled snap points
    }

    private void ShowQuoteMenu(int snapPointIndex)
    {
        mainMenu.SetActive(true);
        PopulateQuotePanel(positivityQuotes); // Default to Positivity quotes
        quoteMenuUI.SetActive(true);
        listMenu.SetActive(false);
        backToMainMenuButton.gameObject.SetActive(true);
    }

    private void ShowQuoteList(List<Quote> quotes)
    {
        mainMenu.SetActive(false);
        PopulateQuotePanel(quotes);
        quoteMenuUI.SetActive(true);
        listMenu.SetActive(true);
        backToMainMenuButton.gameObject.SetActive(true);
        hideQuoteMenuButton.gameObject.SetActive(false);
    }

    private void BackToMainMenu()
    {
        quoteMenuUI.SetActive(true);
        listMenu.SetActive(false);
        backToMainMenuButton.gameObject.SetActive(false);
        hideQuoteMenuButton.gameObject.SetActive(true);
        mainMenu.SetActive(true);
    }

    private void HideQuoteMenu()
    {
        quoteMenuUI.SetActive(false);
    }

    private void HideQuoteAndButtonsMenu()
    {
        quoteMenuUI.SetActive(false);
        HideButtons(selectedSnapPointIndex);
    }

    private void PopulateQuotePanel(List<Quote> quotes)
    {
        foreach (Transform child in quoteContainer)
        {
            child.gameObject.SetActive(false);
        }

        for (int i = 0; i < quotes.Count; i++)
        {
            GameObject quoteUI;
            if (i < quoteContainer.childCount)
            {
                quoteUI = quoteContainer.GetChild(i).gameObject;
                quoteUI.SetActive(true);
            }
            else
            {
                quoteUI = Instantiate(quoteUIPrefab, quoteContainer);
            }
            TextMeshProUGUI quoteText = quoteUI.transform.Find("QuoteText").GetComponent<TextMeshProUGUI>();
            Button selectButton = quoteUI.transform.Find("SelectButton").GetComponent<Button>();

            int capturedIndex = i;  // Capture the current index
            quoteText.text = $"\"{quotes[capturedIndex].text}\"";
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => SelectQuote(quotes[capturedIndex]));
        }
    }

    private void SelectQuote(Quote quote)
    {
        Debug.Log($"Selected Quote: \"{quote.text}\"");
        AdjustTipPercentage(quote.tipIncrease, quote.types);

        if (snappedItem != null)
        {
            var itemComponent = snappedItem.GetComponent<ItemComponent>();
            if (itemComponent != null)
            {
                itemComponent.quoted = true;
                itemComponent.quote = quote; // Store the quote on the item
            }
        }

        quote.used = true; // Mark the quote as used
        quoteMenuUI.SetActive(false);
        backToMainMenuButton.gameObject.SetActive(false);
        mainMenu.SetActive(true);
        HideButtons(selectedSnapPointIndex);
    }


    private void AdjustTipPercentage(float tipIncrease, List<QuoteType> quoteTypes)
    {
        foreach (var customer in FindObjectsOfType<CustomerBehavior>())
        {
            customer.AdjustTipPercentage(tipIncrease, quoteTypes);
        }
    }

    public void UnlockQuotesBasedOnCreativity(int creativityLevel)
    {
        List<string> unlockedQuotes = new List<string>();

        if (creativityLevel > 2 && extraQuotes.Count > 0)
        {
            unlockedQuotes.Add(extraQuotes[0].text);
        }
        if (creativityLevel > 4 && extraQuotes.Count > 1)
        {
            unlockedQuotes.Add(extraQuotes[1].text);
        }
        if (creativityLevel > 6 && extraQuotes.Count > 2)
        {
            unlockedQuotes.Add(extraQuotes[2].text);
        }
        if (creativityLevel > 9 && extraQuotes.Count > 3)
        {
            unlockedQuotes.Add(extraQuotes[3].text);
        }
    }

    public IEnumerator HideButtonsTemporarily(int snapPointIndex, float delay)
    {
        HideButtons(snapPointIndex);
        yield return new WaitForSeconds(delay);
        if (!disabledSnapPoints.Contains(snapPointIndex)) // Do not show buttons if disabled
        {
            ShowAddQuoteAndNoButtons(snapPointIndex);
        }
    }
}
