using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ApartmentItemButton : MonoBehaviour
{
    [SerializeField] private ApartmentItemData apartmentItemData; // ScriptableObject item reference 
    [SerializeField] private InventoryData inventoryData; // Reference to player's inventory
    [SerializeField] private Button useButton;
    private Image itemImage;
    [SerializeField] private TextMeshProUGUI displayText; 
    [SerializeField] private GameObject displayTextBox;

    // Reference to the ShopItemNavigator
    private ApartmentItemNavigator apartmentItemNavigator;

    private int itemIndex;  

    private void Start()
    { 
        apartmentItemNavigator = FindObjectOfType<ApartmentItemNavigator>(); 

        // Find and assign displayText and displayTextBox using Tags
        displayText = GameObject.FindGameObjectWithTag("DisplayText").GetComponent<TextMeshProUGUI>();
        displayTextBox = GameObject.FindGameObjectWithTag("DisplayTextBox"); 

        if (displayText == null)
        {
            Debug.LogError("Display Text is not found in the scene! Make sure there is a GameObject tagged as 'DisplayText' with a TextMeshProUGUI component.");
        }

        if (displayTextBox == null)
        {
            Debug.LogError("Display Text Box is not found in the scene! Make sure there is a GameObject tagged as 'DisplayTextBox'.");
        }
        else
        {
            displayTextBox.GetComponent<Image>().enabled = false; 
        }

        // Get the index of this ShopItem in the ShopItemNavigator
        if (apartmentItemNavigator != null)
        {
            itemIndex = apartmentItemNavigator.GetShopItemIndex(gameObject); 
        }

        itemImage = GetComponent<Image>(); // Get the Image component 
    }

    public void OnUseButtonClick(string toFile)
    {
        HandleHangOutClick(toFile);
    } 

    private void HandleHangOutClick(string locationFile)
    {
        // Set the starting file based on the location
        PlayerPrefs.SetString("StartingFile", locationFile);
        PlayerPrefs.Save(); 

        // Transfer to the VisualNovel scene
        SceneManager.LoadScene("VisualNovel");
    } 

    public void OnHoverEnter()
    {
        apartmentItemNavigator.ResetAllShopItems();    

        if (displayText != null && displayTextBox != null)
        {
            displayText.gameObject.SetActive(true);
            displayTextBox.GetComponent<Image>().enabled = true; 
            displayTextBox.gameObject.SetActive(true);
            displayText.text = apartmentItemData.itemDescription; // Assuming itemDescription exists in ApartmentItemData
        }

        if (itemImage != null)
        {
            itemImage.color = Color.black; // Highlight effect 
        }

        // Reset all other ShopItems to blue
        //if (apartmentItemNavigator != null)
        //{
            //apartmentItemNavigator.ResetAllShopItems();
        //}

        // Update the currentIndex in the navigator
        if (apartmentItemNavigator != null)
        {
            int newIndex = apartmentItemNavigator.GetShopItemIndex(gameObject);
            if (newIndex >= 0)
            {
                apartmentItemNavigator.SetCurrentIndex(newIndex);
            }
        } 
    }

    public void OnHoverExit()
    { 
        //apartmentItemNavigator.ResetAllShopItems(); 

        if (displayText != null && displayTextBox != null)
        {
            displayText.gameObject.SetActive(false);
            displayTextBox.GetComponent<Image>().enabled = false; 
            displayTextBox.gameObject.SetActive(false); 
            displayText.text = "";
        }

        if (itemImage != null)
        {
            itemImage.color = new Color(1f, 0.5f, 0f); // Orange color when hover exits
        }
    }
}
