using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LibraryButton : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string buttonDetails;

    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private GameObject displayTextBox;
    [SerializeField] private float barItemPrice;
    [SerializeField] private InventoryData inventoryData;

    [SerializeField] private Button purchaseButton; 

    private Image barItemImage;

    // Reference to the ShopItemNavigator
    private LibraryItemNavigator libraryItemNavigator;

    [SerializeField] private ApartmentItemData apartmentItemData; // ScriptableObject item reference 

    private int itemIndex;

    private void Start()
    {
        if (displayText == null)
        {
            Debug.LogError("Display Text is not assigned! Please assign a Text object in the Inspector.");
        }

        displayTextBox.SetActive(false);
        barItemImage = GetComponent<Image>(); // Get the Image component

        // Find the ShopItemNavigator in the scene
        libraryItemNavigator = FindObjectOfType<LibraryItemNavigator>(); 

        // Get the index of this ShopItem in the ShopItemNavigator
        if (libraryItemNavigator != null)
        {
            itemIndex = libraryItemNavigator.GetShopItemIndex(gameObject);
        }

        libraryItemNavigator.ResetAllShopItems();
    }

    public void OnHoverEnter()
    {
        if (displayText != null)
        {
            displayText.text = buttonDetails;
            displayTextBox.SetActive(true);

            // Reset all other ShopItems to blue
            if (libraryItemNavigator != null)
            {
                libraryItemNavigator.ResetAllShopItems();
            }

            // Set this ShopItem's color to black
            if (barItemImage != null)
            {
                barItemImage.color = Color.black;
            }

            // Update the currentIndex in the ShopItemNavigator
            if (libraryItemNavigator != null)
            {
                libraryItemNavigator.SetCurrentIndex(itemIndex);
            }
        }
    }

    public void OnHoverExit()
    {
        if (displayText != null)
        {
            displayText.text = ""; // Clear the text
            displayTextBox.SetActive(false);

            if (barItemImage != null)
            {
                //barItemImage.color = Color.blue; // Reset color to blue on hover exit 
                barItemImage.color = new Color(1f, 0.5f, 0f); // RGB: 255, 128, 0      
            }
        }
    }

    public void OnPurchaseButtonClick() 
    {
        if (libraryItemNavigator != null)
        {
            if (libraryItemNavigator.DeductMoney(barItemPrice)) // Deduct money if sufficient funds 
            {
                Debug.Log($"Item purchased for ${barItemPrice}.");
                //SceneManager.LoadScene(ToScene);
                inventoryData.AddItem(apartmentItemData);   
                //HandleHangOutClick(ToFile);
            }
            else
            {
                Debug.Log("Insufficient funds!");
            }
        }

        purchaseButton.interactable = false; 
    }

    public string GetButtonDetails()
    {
        return buttonDetails;
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

/*using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LibraryButton : MonoBehaviour
{
    [SerializeField] private ApartmentItemData apartmentItemData; // ScriptableObject item reference
    [SerializeField] private InventoryData inventoryData; // Reference to player's inventory
    [SerializeField] private float itemPrice;
    [SerializeField] private Button purchaseButton;

    [TextArea(3, 10)]
    [SerializeField] private string buttonDetails;

    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private GameObject displayTextBox;
    [SerializeField] private float barItemPrice;
    //[SerializeField] private InventoryData inventoryData;

    //[SerializeField] private Button purchaseButton;

    private Image libraryItemImage;

    private int itemIndex; 

    // Reference to the ShopItemNavigator
    private LibraryItemNavigator libraryItemNavigator; 

    private void Start()
    {
        // Get the index of this ShopItem in the ShopItemNavigator
        if (libraryItemNavigator != null)
        {
            itemIndex = libraryItemNavigator.GetShopItemIndex(gameObject);
        }

        //libraryItemNavigator.ResetAllShopItems(); 

        libraryItemImage = GetComponent<Image>(); // Get the Image component 
    }

    public void OnPurchaseButtonClick()
    {
        if (inventoryData == null || apartmentItemData == null)
        {
            Debug.LogError("InventoryData or ApartmentItemData is not assigned!");
            return;
        }

        if (inventoryData.ownedItems.Contains(apartmentItemData))
        {
            Debug.Log("Item already purchased!");
            return;
        }

        inventoryData.AddItem(apartmentItemData);
        Debug.Log($"Purchased: {apartmentItemData.itemName}");

        purchaseButton.interactable = false; // Disable button after purchase
    }

    public void OnHoverEnter()
    {
        if (displayText != null)
        {
            displayText.text = buttonDetails;
            displayTextBox.SetActive(true);

            // Reset all other ShopItems to blue
            if (libraryItemNavigator != null)
            {
                libraryItemNavigator.ResetAllShopItems();
            }

            // Set this ShopItem's color to black
            if (libraryItemImage != null)
            {
                libraryItemImage.color = Color.black;
            }

            // Update the currentIndex in the ShopItemNavigator
            if (libraryItemNavigator != null)
            {
                libraryItemNavigator.SetCurrentIndex(itemIndex);
            }
        }
    }

    public void OnHoverExit()
    { 
        if (displayText != null)
        {
            displayText.text = ""; // Clear the text
            displayTextBox.SetActive(false);

            if (libraryItemImage != null)
            {
                //barItemImage.color = Color.blue; // Reset color to blue on hover exit 
                libraryItemImage.color = new Color(1f, 0.5f, 0f); // Orange color when hover exits     
            }
        }
    }
}*/

