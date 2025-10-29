using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LibraryItemNavigator : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect; // Reference to the ScrollRect component
    [SerializeField] private RectTransform content; // The content of the Scroll View
    [SerializeField] private GameObject[] libraryItems; // Array of ShopItem buttons
    [SerializeField] private TextMeshProUGUI displayText; // Display for details
    [SerializeField] private GameObject displayTextBox; // Box for details 

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

        if (libraryItems.Length > 0)
        {
            HighlightItem(currentIndex); // Start by highlighting the first ShopItem 
        }
        else
        {
            Debug.LogError("No LibraryItems assigned!");
        }

        displayText.text = "";

        displayTextBox.SetActive(false);

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
        currentIndex = (currentIndex - 1 + libraryItems.Length) % libraryItems.Length; // Loop to last if at first
        HighlightItem(currentIndex);
        ScrollToItem(currentIndex);
    }

    public void NavigateDown()
    {
        ResetItemColor(currentIndex); // Reset current item's color
        currentIndex = (currentIndex + 1) % libraryItems.Length; // Loop to first if at last
        HighlightItem(currentIndex);
        ScrollToItem(currentIndex);
    }

    public int GetShopItemIndex(GameObject shopItem)
    {
        // Find the index of the ShopItem in the shopItems array
        for (int i = 0; i < libraryItems.Length; i++)
        {
            if (libraryItems[i] == shopItem)
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
        foreach (GameObject shopItem in libraryItems)
        {
            Image shopItemImage = shopItem.GetComponent<Image>();
            if (shopItemImage != null)
            {
                //shopItemImage.color = Color.purple;
                shopItemImage.color = new Color(1f, 0.5f, 0f); // RGB: 255, 128, 0 

            }
        }
    }

    private void HighlightItem(int index)
    {
        // Set the current ShopItem's Image color to Black
        Image shopItemImage = libraryItems[index].GetComponent<Image>();
        if (shopItemImage != null)
        {
            shopItemImage.color = Color.black;
        }

        // Display the selected ShopItem's details
        BarButton storeButton = libraryItems[index].GetComponent<BarButton>();
        if (storeButton != null && displayText != null)
        {
            displayText.text = storeButton.GetButtonDetails();
            displayTextBox.SetActive(true);
        }
    }

    private void ResetItemColor(int index)
    {
        // Reset the ShopItem's Image color to Blue
        Image shopItemImage = libraryItems[index].GetComponent<Image>();
        if (shopItemImage != null)
        {
            //shopItemImage.color = Color.blue;
            shopItemImage.color = new Color(1f, 0.5f, 0f); // RGB: 255, 128, 0    
        }
    }

    private void ScrollToItem(int index)
    {
        if (scrollRect != null && content != null)
        {
            // Get the total height of the content
            float contentHeight = content.rect.height;

            // Get the height of a single ShopItem
            float itemHeight = libraryItems[index].GetComponent<RectTransform>().rect.height;

            // Calculate the normalized position of the selected item
            float normalizedPosition = 1 - ((index * itemHeight) / (contentHeight - scrollRect.viewport.rect.height));

            // Clamp the normalized position between 0 and 1
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalizedPosition);
        }
    }

    public void OnBackClick()
    {
        HandleHangOutClick("LibraryFile");  
    }

    private void HandleHangOutClick(string locationFile)
    {
        // Set the starting file based on the location
        PlayerPrefs.SetString("StartingFile", locationFile);
        PlayerPrefs.Save();

        // Transfer to the VisualNovel scene
        SceneManager.LoadScene("VisualNovel");
    }
}
