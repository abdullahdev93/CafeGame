using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StoreButton : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string buttonDetails;

    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private GameObject displayTextBox;
    [SerializeField] private float shopItemPrice;

    private Image shopItemImage;

    // Reference to the ShopItemNavigator
    private ShopItemNavigator shopItemNavigator;

    private int itemIndex;

    private void Start()
    {
        if (displayText == null)
        {
            Debug.LogError("Display Text is not assigned! Please assign a Text object in the Inspector.");
        }

        displayTextBox.SetActive(false);
        shopItemImage = GetComponent<Image>(); // Get the Image component

        // Find the ShopItemNavigator in the scene
        shopItemNavigator = FindObjectOfType<ShopItemNavigator>();

        // Get the index of this ShopItem in the ShopItemNavigator
        if (shopItemNavigator != null)
        {
            itemIndex = shopItemNavigator.GetShopItemIndex(gameObject);
        }

        shopItemNavigator.ResetAllShopItems(); 
    }

    public void OnHoverEnter()
    {
        if (displayText != null)
        {
            displayText.text = buttonDetails;
            displayTextBox.SetActive(true);

            // Reset all other ShopItems to blue
            if (shopItemNavigator != null)
            {
                shopItemNavigator.ResetAllShopItems();
            }

            // Set this ShopItem's color to black
            if (shopItemImage != null)
            {
                shopItemImage.color = Color.black;
            }

            // Update the currentIndex in the ShopItemNavigator
            if (shopItemNavigator != null)
            {
                shopItemNavigator.SetCurrentIndex(itemIndex);
            }
        }
    }

    public void OnHoverExit()
    {
        if (displayText != null)
        {
            displayText.text = ""; // Clear the text
            displayTextBox.SetActive(false);

            if (shopItemImage != null)
            {
                shopItemImage.color = Color.blue; // Reset color to blue on hover exit
            }
        }
    }

    public void OnPurchaseButtonClick(string ToScene)
    {
        if (shopItemNavigator != null)
        {
            if (shopItemNavigator.DeductMoney(shopItemPrice)) // Deduct money if sufficient funds
            {
                Debug.Log($"Item purchased for ${shopItemPrice}.");
                SceneManager.LoadScene(ToScene);
            }
            else
            {
                Debug.Log("Insufficient funds!");
            }
        }
    } 

    public string GetButtonDetails()
    {
        return buttonDetails;
    }
}
