using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeMenu : MonoBehaviour
{
    public static UpgradeMenu Instance;

    [System.Serializable]
    public class Upgrade
    {
        public string name;
        public float cost; // Changed from int to float to handle dollars and cents
        public Sprite image;
        public GameObject item; // Reference to the item to be upgraded or purchased
        public int maxPurchaseCount; // Maximum number of this item that can be purchased
        [HideInInspector]
        public int currentPurchaseCount = 0; // Current number of this item that has been purchased
    }

    public List<Upgrade> decorations;
    public List<Upgrade> extraEquipment;
    public List<Upgrade> albums;

    public TextMeshProUGUI totalMoneyText;
    //public TextMeshProUGUI totalMoneyUpgradeText;
    public TextMeshProUGUI upgradeMenuText;
    public GameObject upgradeUIPrefab;
    public Transform upgradeContainer; // Container to hold the upgrade UI elements

    public ScrollRect scrollRect; // ScrollRect component to handle scrolling 
    public GameObject scrollBar;
    public GameObject upArrow; // UI element indicating more items above
    public GameObject downArrow; // UI element indicating more items below
    public GameObject mainMenu;
    public GameObject mainUpgradeMenu;
    public GameObject upgradeList;
    //public GameObject equipmentMenu;
    //public GameObject decorMenu;
    //public GameObject albumsMenu; 
    public GameObject pauseMenuUI;

    public Button backToPause;
    public Button backToMain; 

    private float totalMoney;

    public GameObject coffeeMachinePrefab; // Prefab for the Coffee Machine
    public GameObject coffeeMachineTimerCloneO; // Reference to the Coffee Machine Timer
    public GameObject timerFilledO; // Reference to the Timer Filled
    public GameObject startBrewingButtonCloneO; // Reference to the Start Brewing Button 
    public Button backToMapButton;
    public Button upgradeMenuButton; 

    public float TotalMoney
    {
        get { return totalMoney; }
        set
        {
            totalMoney = Mathf.Round(value * 100f) / 100f;
            UpdateTotalMoneyText();
        }
    }

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        /*if (Instance == null) 
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject); 
        }
        else 
        {
            Destroy(gameObject); 
        }*/

        //upgradeMenuButton.onClick.AddListener(ShowMainMenu); 
    }

    // Add method to manually destroy this instance if needed
    /*public static void DestroyInstance()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Instance = null;
        }
    }*/ 

    private void Start()
    {
        InitializeTotalMoney();
        UpdateTotalMoneyText();

        // Make the objects initially inactive
        coffeeMachineTimerCloneO.SetActive(false);
        timerFilledO.SetActive(false);
        startBrewingButtonCloneO.SetActive(false);

        // Load total money from PlayerPrefs
        totalMoney = PlayerPrefs.GetFloat("TotalMoney", 0); // Default to 0 if not found
        UpdateTotalMoneyText(); 

        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        UpdateArrowVisibility();
        upArrow.SetActive(false);
        downArrow.SetActive(false); 

        upgradeMenuButton.onClick.AddListener(ShowMainMenu);
        Debug.Log($"mainUpgradeMenu active: {mainUpgradeMenu.activeSelf}");
        Debug.Log($"upgradeList active: {upgradeList.activeSelf}");

        StartCoroutine(EnableUpgradeMenuButtonWithDelay()); 

    }

    private IEnumerator EnableUpgradeMenuButtonWithDelay()
    {
        yield return new WaitForSeconds(0.5f); // Adjust delay as needed
        upgradeMenuButton.interactable = true;
    } 

    private void InitializeTotalMoney()
    {
        totalMoney = 0f; // Start with 0 money in the upgrade menu
        UpdateTotalMoneyText();
    }

    public void AddMoney(float amount)
    {
        totalMoney += Mathf.Round(amount * 100f) / 100f;
        UpdateTotalMoneyText();
    }

    public void UnlockUpgradesBasedOnCreativity(int creativityLevel)
    {
        // Clear the extra equipment list of special upgrades
        extraEquipment.RemoveAll(upgrade => upgrade.name == "Improved Lighting" ||
                                             upgrade.name == "Latte Art" ||
                                             upgrade.name == "Bean Bliss Brand");

        decorations.RemoveAll(upgrade => upgrade.name == "Framed Art" ||
                                             upgrade.name == "Sweet Decor");

        //albums.RemoveAll(upgrade => upgrade.name == "Midnight Ghosts"); 

        if (creativityLevel > 2)
        {
            extraEquipment.Add(new Upgrade { name = "Latte Art", cost = 200, image = null, item = null, maxPurchaseCount = 1 });
        }
        if (creativityLevel > 4)
        {
            extraEquipment.Add(new Upgrade { name = "Improved Lighting", cost = 300, image = null, item = null, maxPurchaseCount = 1 });
        }
        if (creativityLevel > 6)
        {
            decorations.Add(new Upgrade { name = "Framed Art", cost = 400, image = null, item = null, maxPurchaseCount = 1 });
        }
        if (creativityLevel > 8)
        {
            decorations.Add(new Upgrade { name = "Sweet Decor", cost = 500, image = null, item = null, maxPurchaseCount = 1 });
        }
        if (creativityLevel > 9)
        {
            extraEquipment.Add(new Upgrade { name = "Bean Bliss Brand", cost = 1000, image = null, item = null, maxPurchaseCount = 1 });
        }
    }

    private void PopulateUpgradePanel(List<Upgrade> upgrades)
    {
        // Clear previous upgrade UI elements
        foreach (Transform child in upgradeContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var upgrade in upgrades)
        {
            GameObject upgradeUI = Instantiate(upgradeUIPrefab, upgradeContainer);
            Image upgradeImage = upgradeUI.transform.Find("UpgradeImage").GetComponent<Image>();
            TextMeshProUGUI upgradeText = upgradeUI.transform.Find("UpgradeText").GetComponent<TextMeshProUGUI>();
            Button purchaseButton = upgradeUI.transform.Find("PurchaseButton").GetComponent<Button>();

            upgradeImage.sprite = upgrade.image;
            upgradeText.text = $"{upgrade.name} - ${FormatMoney(upgrade.cost)} ({upgrade.currentPurchaseCount}/{upgrade.maxPurchaseCount})";

            if (upgrade.currentPurchaseCount >= upgrade.maxPurchaseCount)
            {
                purchaseButton.interactable = false;
                purchaseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Maxed Out";
            }
            else
            {
                purchaseButton.onClick.AddListener(() => BuyUpgrade(upgrade, purchaseButton));
            }
        }

        scrollBar.SetActive(true);
        UpdateArrowVisibility();
    }

    private void BuyUpgrade(Upgrade upgrade, Button purchaseButton)
    {
        if (totalMoney >= upgrade.cost)
        {
            totalMoney -= upgrade.cost;
            totalMoney = Mathf.Round(totalMoney * 100f) / 100f;
            UpdateTotalMoneyText();

            // Handle Coffee Machine specific logic
            if (upgrade.name == "Coffee Machine")
            {
                GameObject coffeeMachineInstance = Instantiate(coffeeMachinePrefab, new Vector3(-1.647f, -1.149f, 0), Quaternion.identity);
                CoffeeMachineCom coffeeMachineCom = coffeeMachineInstance.GetComponent<CoffeeMachineCom>();
                coffeeMachineCom.theCoffeeMachineTimer = coffeeMachineTimerCloneO;
                coffeeMachineCom.fill = timerFilledO.GetComponent<Image>();
                coffeeMachineCom.startPouringButton = startBrewingButtonCloneO.GetComponent<Button>();

                // Activate the objects
                coffeeMachineTimerCloneO.SetActive(true);
                timerFilledO.SetActive(true);
                startBrewingButtonCloneO.SetActive(true);
            }

            // Activate or enhance the item (e.g., show the decoration, add the equipment, etc.)
            if (upgrade.item != null)
            {
                upgrade.item.SetActive(true);
                // Apply any additional logic for the upgrade here
            }
            Debug.Log($"{upgrade.name} purchased!");

            upgrade.currentPurchaseCount++;
            if (upgrade.currentPurchaseCount >= upgrade.maxPurchaseCount)
            {
                purchaseButton.interactable = false;
                purchaseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Maxed Out";
            }

            // Optionally: Update UI to reflect purchase count
            UpdateUpgradeText(purchaseButton.transform.parent, upgrade);

            PlayerPrefs.SetFloat("TotalMoney", totalMoney);
            PlayerPrefs.Save(); 
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    private void UpdateUpgradeText(Transform upgradeUI, Upgrade upgrade)
    {
        TextMeshProUGUI upgradeText = upgradeUI.Find("UpgradeText").GetComponent<TextMeshProUGUI>();
        upgradeText.text = $"{upgrade.name} - ${FormatMoney(upgrade.cost)} ({upgrade.currentPurchaseCount}/{upgrade.maxPurchaseCount})";
    }

    private void UpdateTotalMoneyText()
    {
        totalMoneyText.text = $"Money: ${FormatMoney(totalMoney)}";
        //totalMoneyUpgradeText.text = $"Money: ${FormatMoney(totalMoney)}";
    }

    private string FormatMoney(float amount)
    {
        return amount.ToString("F2");
    }

    // Menu navigation methods
    public void ShowMainMenu()
    {
        upgradeMenuText.text = "Upgrade Menu";
        PopulateUpgradePanel(new List<Upgrade>()); // Clear the upgrade container
        mainUpgradeMenu.SetActive(true);
        mainMenu.SetActive(true);
        upgradeList.SetActive(false);
        //downArrow.SetActive(false);
        //upArrow.SetActive(false);
        backToMain.gameObject.SetActive(false);
        backToPause.gameObject.SetActive(true);
        backToMapButton.gameObject.SetActive(false);   
        //equipmentMenu.SetActive(false);
        //decorMenu.SetActive(false); 
        //albumsMenu.SetActive(false); 
    }

    public void ReturnToPauseMenu()
    {
        pauseMenuUI.SetActive(true);
        mainUpgradeMenu.SetActive(false);
        upgradeList.SetActive(true);
        //downArrow.SetActive(true);
        //upArrow.SetActive(true);
        backToMain.gameObject.SetActive(false);
        //backToPause.gameObject.SetActive(true);
        backToMapButton.gameObject.SetActive(true); 
        //equipmentMenu.SetActive(false);
        //decorMenu.SetActive(false);  
        //albumsMenu.SetActive(false); 
    }

    public void ShowEquipmentMenu()
    {
        upgradeMenuText.text = "Equipment Menu";
        PopulateUpgradePanel(extraEquipment);
        mainMenu.SetActive(false);
        upgradeList.SetActive(true);
        downArrow.SetActive(true);
        upArrow.SetActive(true);
        backToMain.gameObject.SetActive(true);
        backToPause.gameObject.SetActive(false);
        //equipmentMenu.SetActive(true); 
        //decorMenu.SetActive(false); 
        //albumsMenu.SetActive(false); 
    }

    public void ShowDecorMenu()
    {
        upgradeMenuText.text = "Decor Menu";
        PopulateUpgradePanel(decorations);
        mainMenu.SetActive(false);
        upgradeList.SetActive(true);
        downArrow.SetActive(true);
        upArrow.SetActive(true);
        backToMain.gameObject.SetActive(true);
        backToPause.gameObject.SetActive(false);
        //equipmentMenu.SetActive(false);
        //decorMenu.SetActive(true);
        //albumsMenu.SetActive(false); 
    }

    public void ShowAlbumsMenu()
    {
        upgradeMenuText.text = "Albums Menu";
        PopulateUpgradePanel(albums);
        mainMenu.SetActive(false);
        upgradeList.SetActive(true);
        downArrow.SetActive(true);
        upArrow.SetActive(true);
        backToMain.gameObject.SetActive(true);
        backToPause.gameObject.SetActive(false);
        //equipmentMenu.SetActive(false);
        //decorMenu.SetActive(false);
        //albumsMenu.SetActive(true); 
    }

    private void OnScrollValueChanged(Vector2 scrollPosition)
    {
        UpdateArrowVisibility();
    }

    private void UpdateArrowVisibility()
    {
        float scrollValue = scrollRect.verticalNormalizedPosition;

        upArrow.SetActive(scrollValue < 1f);
        downArrow.SetActive(scrollValue > 0f);
    }
}
