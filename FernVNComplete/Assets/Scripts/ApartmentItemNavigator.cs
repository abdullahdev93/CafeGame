/*using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;

public class ApartmentItemNavigator : MonoBehaviour 
{
    [SerializeField] private InventoryData inventoryData; // Reference to InventoryData 
    [SerializeField] private ScrollRect scrollRect; // Reference to the ScrollRect component
    [SerializeField] private RectTransform content; // The content of the Scroll View
    [SerializeField] private List<GameObject> apartmentItems = new List<GameObject>(); 
    [SerializeField] private TextMeshProUGUI displayText; // Display for details
    [SerializeField] private GameObject displayTextBox; // Box for details

    [SerializeField] private GameObject apartmentItemPrefab; // Prefab to represent an item 

    [SerializeField] private TextMeshProUGUI totalMoneyText;

    private float totalMoney;

    private int currentIndex = 0;

    private bool isHoldingUp = false;
    private bool isHoldingDown = false;
    public float holdDelay = 0.3f; // Adjust speed of continuous scrolling 

    private float currentHoldDelayUp;
    private float currentHoldDelayDown;
    private const float maxSpeedCap = 0.05f;

    private void Start()
    {
        totalMoney = PlayerPrefs.GetFloat("TotalMoney", 1000f); // Retrieve money from PlayerPrefs
        UpdateTotalMoneyText();

        if (apartmentItems.Count > 0)
        {
            HighlightItem(currentIndex); // Start by highlighting the first ShopItem 
        }
        else
        {
            Debug.LogError("No apartmentItems assigned!"); 
        }

        displayText.text = "";

        displayTextBox.SetActive(false);

        LoadApartmentItems(); 

        ResetAllShopItems();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentHoldDelayUp = holdDelay;
            NavigateUp();
            isHoldingUp = true;
            InvokeRepeating(nameof(HoldNavigateUp), holdDelay, holdDelay);
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            isHoldingUp = false;
            CancelInvoke(nameof(HoldNavigateUp));
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentHoldDelayDown = holdDelay;
            NavigateDown();
            isHoldingDown = true;
            InvokeRepeating(nameof(HoldNavigateDown), holdDelay, holdDelay);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            isHoldingDown = false;
            CancelInvoke(nameof(HoldNavigateDown));
        }
    }

    private void LoadApartmentItems()
    {
        // Clear previous UI elements
        foreach (Transform child in content)    
        {
            Destroy(child.gameObject);
        }

        // Populate UI with owned items
        foreach (ApartmentItemData itemData in inventoryData.ownedItems)
        {
            GameObject newItem = Instantiate(apartmentItemPrefab, content);    
            newItem.GetComponentInChildren<TextMeshProUGUI>().text = itemData.itemName; // Set name
            newItem.GetComponentInChildren<Image>().sprite = itemData.itemIcon; // Set icon
        }
    }


private void HoldNavigateUp()
    {
        if (isHoldingUp)
        {
            NavigateUp();
            //currentHoldDelayUp = Mathf.Max(0.05f, currentHoldDelayUp * 0.9f);
            currentHoldDelayUp = Mathf.Max(maxSpeedCap, currentHoldDelayUp * 0.9f);
            CancelInvoke(nameof(HoldNavigateUp));
            InvokeRepeating(nameof(HoldNavigateUp), currentHoldDelayUp, currentHoldDelayUp);
        }
    }

    private void HoldNavigateDown()
    {
        if (isHoldingDown)
        {
            NavigateDown();
            //currentHoldDelayDown = Mathf.Max(0.05f, currentHoldDelayDown * 0.9f);
            currentHoldDelayDown = Mathf.Max(maxSpeedCap, currentHoldDelayDown * 0.9f);
            CancelInvoke(nameof(HoldNavigateDown));
            InvokeRepeating(nameof(HoldNavigateDown), currentHoldDelayDown, currentHoldDelayDown);
        }
    }

    public bool DeductMoney(float amount)
    {
        if (totalMoney >= amount)
        {
            totalMoney -= amount;
            PlayerPrefs.SetFloat("TotalMoney", totalMoney); // Save updated money to PlayerPrefs
            PlayerPrefs.Save();
            UpdateTotalMoneyText();
            return true;
        }
        return false; // Not enough funds
    }

    private void UpdateTotalMoneyText()
    {
        if (totalMoneyText != null)
        {
            totalMoneyText.text = $"${totalMoney:N2}";
        }
    }

    public void NavigateUp()
    {
        ResetItemColor(currentIndex); // Reset current item's color
        currentIndex = (currentIndex - 1 + apartmentItems.Count) % apartmentItems.Count; // Loop to last if at first
        HighlightItem(currentIndex);
        ScrollToItem(currentIndex);
    }

    public void NavigateDown()
    {
        ResetItemColor(currentIndex); // Reset current item's color
        currentIndex = (currentIndex + 1) % apartmentItems.Count; // Loop to first if at last
        HighlightItem(currentIndex);
        ScrollToItem(currentIndex);
    }

    public int GetShopItemIndex(GameObject shopItem)
    {
        // Find the index of the ShopItem in the shopItems array
        for (int i = 0; i < apartmentItems.Count; i++) 
        {
            if (apartmentItems[i] == shopItem)
            {
                return i;
            }
        }
        return -1; // Return -1 if the ShopItem is not found
    }

    public void SetCurrentIndex(int index)
    {
        // Update the current index to match the hovered item
        currentIndex = index;
    }

    public void ResetAllShopItems()
    {
        // Reset all ShopItems to blue
        foreach (GameObject shopItem in apartmentItems)
        {
            Image shopItemImage = shopItem.GetComponent<Image>();
            if (shopItemImage != null)
            {
                //shopItemImage.color = Color.purple;
                shopItemImage.color = new Color(0.5f, 0f, 0.5f); // RGB: 128, 0, 128 

            }
        }
    }

    private void HighlightItem(int index)
    {
        // Set the current ShopItem's Image color to Black
        Image shopItemImage = apartmentItems[index].GetComponent<Image>();
        if (shopItemImage != null)
        {
            shopItemImage.color = Color.black;
        }

        // Display the selected ShopItem's details
        BarButton storeButton = apartmentItems[index].GetComponent<BarButton>();
        if (storeButton != null && displayText != null)
        {
            displayText.text = storeButton.GetButtonDetails();
            displayTextBox.SetActive(true);
        }
    }

    private void ResetItemColor(int index)
    {
        // Reset the ShopItem's Image color to Blue
        Image shopItemImage = apartmentItems[index].GetComponent<Image>();
        if (shopItemImage != null)
        {
            //shopItemImage.color = Color.blue;
            shopItemImage.color = new Color(0.5f, 0f, 0.5f); // RGB: 128, 0, 128  
        }
    }

    private void ScrollToItem(int index)
    {
        if (scrollRect != null && content != null)
        {
            // Get the total height of the content
            float contentHeight = content.rect.height;

            // Get the height of a single ShopItem
            float itemHeight = apartmentItems[index].GetComponent<RectTransform>().rect.height;

            // Calculate the normalized position of the selected item
            float normalizedPosition = 1 - ((index * itemHeight) / (contentHeight - scrollRect.viewport.rect.height));

            // Clamp the normalized position between 0 and 1
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalizedPosition);
        }
    }

    public void AddApartmentItem(GameObject item)
    {
        if (!apartmentItems.Contains(item))
        {
            apartmentItems.Add(item);
            Debug.Log("Item added to ApartmentItems: " + item.name);
        }
    }

    public void OnBackClick()
    {
        HandleHangOutClick("BarFile");
    }

    private void HandleHangOutClick(string locationFile)
    {
        // Set the starting file based on the location
        PlayerPrefs.SetString("StartingFile", locationFile);
        PlayerPrefs.Save();

        // Transfer to the VisualNovel scene
        SceneManager.LoadScene("VisualNovel");
    }
}*/

/*using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ApartmentItemNavigator : MonoBehaviour
{
    [SerializeField] private InventoryData inventoryData; // Reference to InventoryData
    [SerializeField] private Transform contentContainer; // Parent UI container
    [SerializeField] private GameObject apartmentItemPrefab; // Prefab to represent an item

    private void Start()
    {
        LoadApartmentItems();
    }

    private void LoadApartmentItems()
    {
        // Clear previous UI elements
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        // Populate UI with owned items
        foreach (ApartmentItemData itemData in inventoryData.ownedItems)
        {
            GameObject newItem = Instantiate(apartmentItemPrefab, contentContainer);
            newItem.GetComponentInChildren<TextMeshProUGUI>().text = itemData.itemName; // Set name
            newItem.GetComponentInChildren<Image>().sprite = itemData.itemIcon; // Set icon
        }
    }
}*/

/*using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ApartmentItemNavigator : MonoBehaviour
{
    [SerializeField] private InventoryData inventoryData;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private List<GameObject> apartmentItems = new List<GameObject>();
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private GameObject displayTextBox;
    [SerializeField] private GameObject apartmentItemPrefab;
    [SerializeField] private TextMeshProUGUI totalMoneyText; 

    private float totalMoney;
    private int currentIndex = 0;
    private bool isHoldingUp = false;
    private bool isHoldingDown = false;
    public float holdDelay = 0.3f;
    private float currentHoldDelayUp;
    private float currentHoldDelayDown;
    private const float maxSpeedCap = 0.05f;

    private void Start()
    {
        totalMoney = PlayerPrefs.GetFloat("TotalMoney", 1000f);
        UpdateTotalMoneyText();
        LoadApartmentItems();

        if (apartmentItems.Count > 0)
        {
            HighlightItem(currentIndex);
        }
        else
        {
            Debug.LogError("No apartmentItems assigned!");
        }

        displayText.text = "";
        //displayTextBox.SetActive(false);
        //ResetAllShopItems(); 

        ResetAllShopItems(); 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentHoldDelayUp = holdDelay;
            NavigateUp();
            isHoldingUp = true;
            InvokeRepeating(nameof(HoldNavigateUp), holdDelay, holdDelay);
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            isHoldingUp = false;
            CancelInvoke(nameof(HoldNavigateUp));
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentHoldDelayDown = holdDelay;
            NavigateDown();
            isHoldingDown = true;
            InvokeRepeating(nameof(HoldNavigateDown), holdDelay, holdDelay);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            isHoldingDown = false;
            CancelInvoke(nameof(HoldNavigateDown));
        }
    }

    public void ResetAllShopItems()
    {
        // Reset all ShopItems to blue
        foreach (GameObject shopItem in apartmentItems)
        {
            Image shopItemImage = shopItem.GetComponent<Image>();
            if (shopItemImage != null)
            {
                //shopItemImage.color = Color.purple;
                shopItemImage.color = new Color(1f, 0.5f, 0f); // Orange color when hover exits  

            }
        }
    }

    private void HoldNavigateUp()
    {
        if (isHoldingUp)
        {
            NavigateUp();
            currentHoldDelayUp = Mathf.Max(maxSpeedCap, currentHoldDelayUp * 0.9f);
            CancelInvoke(nameof(HoldNavigateUp));
            InvokeRepeating(nameof(HoldNavigateUp), currentHoldDelayUp, currentHoldDelayUp);
        }
    }

    private void HoldNavigateDown()
    {
        if (isHoldingDown)
        {
            NavigateDown();
            currentHoldDelayDown = Mathf.Max(maxSpeedCap, currentHoldDelayDown * 0.9f);
            CancelInvoke(nameof(HoldNavigateDown));
            InvokeRepeating(nameof(HoldNavigateDown), currentHoldDelayDown, currentHoldDelayDown);
        }
    }

    private void LoadApartmentItems()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (ApartmentItemData itemData in inventoryData.ownedItems)
        {
            GameObject newItem = Instantiate(apartmentItemPrefab, content);
            newItem.GetComponentInChildren<TextMeshProUGUI>().text = itemData.itemName;
            newItem.GetComponentInChildren<Image>().sprite = itemData.itemIcon;
            apartmentItems.Add(newItem);
        }
    }

    public void NavigateUp()
    {
        ResetItemColor(currentIndex);
        currentIndex = (currentIndex - 1 + apartmentItems.Count) % apartmentItems.Count;
        HighlightItem(currentIndex);
        ScrollToItem(currentIndex);
    }

    public void NavigateDown()
    {
        ResetItemColor(currentIndex);
        currentIndex = (currentIndex + 1) % apartmentItems.Count;
        HighlightItem(currentIndex);
        ScrollToItem(currentIndex);
    }

    private void ScrollToItem(int index)
    {
        if (scrollRect != null && content != null)
        {
            float contentHeight = content.rect.height;
            float itemHeight = apartmentItems[index].GetComponent<RectTransform>().rect.height;
            float normalizedPosition = 1 - ((index * itemHeight) / (contentHeight - scrollRect.viewport.rect.height));
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalizedPosition);
        }
    }

    private void HighlightItem(int index)
    {
        Image shopItemImage = apartmentItems[index].GetComponent<Image>();
        if (shopItemImage != null)
        {
            shopItemImage.color = Color.black;
        }

        TextMeshProUGUI itemText = apartmentItems[index].GetComponentInChildren<TextMeshProUGUI>();
        if (itemText != null && displayText != null)
        {
            displayText.text = itemText.text;
            displayTextBox.SetActive(true);
        }
    }

    private void ResetItemColor(int index)
    {
        Image shopItemImage = apartmentItems[index].GetComponent<Image>();
        if (shopItemImage != null)
        {
            shopItemImage.color = new Color(1f, 0.5f, 0f); // Orange color when hover exits  
        }
    }

    private void UpdateTotalMoneyText()
    {
        if (totalMoneyText != null)
        {
            totalMoneyText.text = $"${totalMoney:N2}";
        }
    }

    public void OnBackClick()
    {
        HandleHangOutClick("ApartmentStart"); 
    }

    private void HandleHangOutClick(string locationFile)
    {
        PlayerPrefs.SetString("StartingFile", locationFile);
        PlayerPrefs.Save();
        SceneManager.LoadScene("VisualNovel");  
    }
}*/

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ApartmentItemNavigator : MonoBehaviour
{
    [SerializeField] private InventoryData inventoryData;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private GameObject displayTextBox;
    [SerializeField] private TextMeshProUGUI totalMoneyText;

    [SerializeField] private GameObject artOfConversation;
    [SerializeField] private GameObject charmingForDummies;
    [SerializeField] private GameObject magneticPresence;
    [SerializeField] private GameObject masteringTheSocialGame;
    [SerializeField] private GameObject smallTalkBigImpact;
    [SerializeField] private GameObject theJokeWhisperer;
    [SerializeField] private GameObject standUpForDummies;
    [SerializeField] private GameObject wittyComebacksSarcasm;
    [SerializeField] private GameObject whyWeLaugh;
    [SerializeField] private GameObject satireSpotlight;
    [SerializeField] private GameObject scribblesToMasterpieces;
    [SerializeField] private GameObject visionaryWeekly;
    [SerializeField] private GameObject theImaginationReport;
    [SerializeField] private GameObject theCreativeMindset;
    [SerializeField] private GameObject theCreatorsJournal;
    [SerializeField] private GameObject walkingInTheirShoes;
    [SerializeField] private GameObject kindWordsStrongBonds;
    [SerializeField] private GameObject heartfeltStories;
    [SerializeField] private GameObject howToUnderstandOthers;
    [SerializeField] private GameObject theEmotionalCompass; 

    private Dictionary<ApartmentItemData.ItemType, GameObject> itemPrefabs;

    //private Dictionary<string, GameObject> itemPrefabs;
    private List<GameObject> apartmentItems = new List<GameObject>();

    private float totalMoney;
    private int currentIndex = 0;
    private bool isHoldingUp = false;
    private bool isHoldingDown = false;
    public float holdDelay = 0.3f;
    private float currentHoldDelayUp;
    private float currentHoldDelayDown;
    private const float maxSpeedCap = 0.05f;

    private void Start()
    {
        totalMoney = PlayerPrefs.GetFloat("TotalMoney", 1000f);
        UpdateTotalMoneyText(); 

        // Use Enum as Dictionary Key
        itemPrefabs = new Dictionary<ApartmentItemData.ItemType, GameObject>
        {
            { ApartmentItemData.ItemType.ArtOfConversation, artOfConversation },
            { ApartmentItemData.ItemType.CharmingForDummies, charmingForDummies },
            { ApartmentItemData.ItemType.MagneticPresence, magneticPresence },
            { ApartmentItemData.ItemType.MasteringTheSocialGame, masteringTheSocialGame },
            { ApartmentItemData.ItemType.SmallTalkBigImpact, smallTalkBigImpact },
            { ApartmentItemData.ItemType.TheJokeWhisperer, theJokeWhisperer },
            { ApartmentItemData.ItemType.StandUpForDummies, standUpForDummies },
            { ApartmentItemData.ItemType.WittyComebacksSarcasm, wittyComebacksSarcasm },
            { ApartmentItemData.ItemType.WhyWeLaugh, whyWeLaugh },
            { ApartmentItemData.ItemType.SatireSpotlight, satireSpotlight },
            { ApartmentItemData.ItemType.ScribblesToMasterpieces, scribblesToMasterpieces },
            { ApartmentItemData.ItemType.VisionaryWeekly, visionaryWeekly },
            { ApartmentItemData.ItemType.TheImaginationReport, theImaginationReport },
            //{ ApartmentItemData.ItemType.MasteringTheSocialGame, masteringTheSocialGame },
            { ApartmentItemData.ItemType.TheCreativeMindset, theCreativeMindset },
            { ApartmentItemData.ItemType.TheCreatorsJournal, theCreatorsJournal },
            { ApartmentItemData.ItemType.WalkingInTheirShoes, walkingInTheirShoes },
            { ApartmentItemData.ItemType.KindWordsStrongBonds, kindWordsStrongBonds },
            { ApartmentItemData.ItemType.HeartfeltStories, heartfeltStories },
            { ApartmentItemData.ItemType.HowToUnderstandOthers, howToUnderstandOthers },
            { ApartmentItemData.ItemType.TheEmotionalCompass, theEmotionalCompass }

        };

        LoadApartmentItems();

        if (apartmentItems.Count > 0)
        {
            HighlightItem(currentIndex);
        }
        else
        {
            Debug.LogError("No apartmentItems assigned!");
        } 

        displayText.text = "";
        ResetAllShopItems(); 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentHoldDelayUp = holdDelay;
            NavigateUp();
            isHoldingUp = true;
            InvokeRepeating(nameof(HoldNavigateUp), holdDelay, holdDelay);
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            isHoldingUp = false;
            CancelInvoke(nameof(HoldNavigateUp));
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentHoldDelayDown = holdDelay;
            NavigateDown();
            isHoldingDown = true;
            InvokeRepeating(nameof(HoldNavigateDown), holdDelay, holdDelay);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            isHoldingDown = false;
            CancelInvoke(nameof(HoldNavigateDown));
        }
    }

    private void LoadApartmentItems()
    {
        apartmentItems.Clear(); // Ensure list is fresh 

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (ApartmentItemData itemData in inventoryData.ownedItems)
        {
            if (itemPrefabs.TryGetValue(itemData.itemType, out GameObject prefab))
            {
                GameObject newItem = Instantiate(prefab, content);
                newItem.GetComponentInChildren<TextMeshProUGUI>().text = itemData.itemName;
                newItem.GetComponentInChildren<Image>().sprite = itemData.itemIcon;
                apartmentItems.Add(newItem);
            }
            else
            {
                Debug.LogWarning($"No prefab found for item type: {itemData.itemType}");
            }
        }
    }

    private void HighlightItem(int index)
    {
        ResetAllShopItems(); 

        Image shopItemImage = apartmentItems[index].GetComponent<Image>(); 
        if (shopItemImage != null)
        {
            shopItemImage.color = Color.black;
        }

        TextMeshProUGUI itemText = apartmentItems[index].GetComponentInChildren<TextMeshProUGUI>();
        if (itemText != null && displayText != null)
        {
            displayText.text = itemText.text;
            displayTextBox.SetActive(true);
        }
    }

    public int GetShopItemIndex(GameObject shopItem)
    {
        // Find the index of the ShopItem in the shopItems array
        for (int i = 0; i < apartmentItems.Count; i++) 
        {
            if (apartmentItems[i] == shopItem)
            {
                return i;
            }
        }
        return -1; // Return -1 if the ShopItem is not found
    }

    //public void SetCurrentIndex(int index)
    //{
        // Update the current index to match the hovered item
        //currentIndex = index;
    //}

    public void SetCurrentIndex(int index)
    {
        currentIndex = index;
        //HighlightItem(currentIndex);
        //ScrollToItem(currentIndex);
    }


    public void ResetAllShopItems()
    {
        foreach (GameObject shopItem in apartmentItems)
        {
            Image shopItemImage = shopItem.GetComponent<Image>();
            if (shopItemImage != null)
            {
                shopItemImage.color = new Color(1f, 0.5f, 0f);
            }
        }
    }

    private void HoldNavigateUp()
    {
        if (isHoldingUp)
        {
            NavigateUp();
            currentHoldDelayUp = Mathf.Max(maxSpeedCap, currentHoldDelayUp * 0.9f);
            CancelInvoke(nameof(HoldNavigateUp));
            InvokeRepeating(nameof(HoldNavigateUp), currentHoldDelayUp, currentHoldDelayUp);
        }
    }

    private void HoldNavigateDown()
    {
        if (isHoldingDown)
        {
            NavigateDown();
            currentHoldDelayDown = Mathf.Max(maxSpeedCap, currentHoldDelayDown * 0.9f);
            CancelInvoke(nameof(HoldNavigateDown));
            InvokeRepeating(nameof(HoldNavigateDown), currentHoldDelayDown, currentHoldDelayDown);
        }
    }

    public void NavigateUp()
    {
        //ResetAllShopItems(); 
        ResetItemColor(currentIndex); // Reset current item's color
        currentIndex = (currentIndex - 1 + apartmentItems.Count) % apartmentItems.Count;
        HighlightItem(currentIndex);
        ScrollToItem(currentIndex);
    }

    public void NavigateDown()
    {
        //ResetAllShopItems(); 
        ResetItemColor(currentIndex); // Reset current item's color
        currentIndex = (currentIndex + 1) % apartmentItems.Count;
        HighlightItem(currentIndex);
        ScrollToItem(currentIndex);
    }

    private void ScrollToItem(int index)
    {
        if (scrollRect != null && content != null)
        {
            float contentHeight = content.rect.height;
            float itemHeight = apartmentItems[index].GetComponent<RectTransform>().rect.height;
            float normalizedPosition = 1 - ((index * itemHeight) / (contentHeight - scrollRect.viewport.rect.height));
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalizedPosition);
        }
    }

    private void ResetItemColor(int index)
    {
        // Reset the ShopItem's Image color to Blue
        Image shopItemImage = apartmentItems[index].GetComponent<Image>();
        if (shopItemImage != null)
        {
            //shopItemImage.color = Color.blue;
            shopItemImage.color = new Color(1f, 0.5f, 0f); // RGB: 255, 128, 0    
        }
    }

    private void UpdateTotalMoneyText()
    {
        if (totalMoneyText != null)
        {
            totalMoneyText.text = $"${totalMoney:N2}";
        }
    }

    public void OnBackClick()
    {
        HandleHangOutClick("ApartmentStart");
    }

    private void HandleHangOutClick(string locationFile)
    {
        PlayerPrefs.SetString("StartingFile", locationFile);
        PlayerPrefs.Save();
        SceneManager.LoadScene("VisualNovel");
    }
}

