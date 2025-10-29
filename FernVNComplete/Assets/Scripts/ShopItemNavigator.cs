using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ShopItemNavigator : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject[] shopItems;
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private GameObject displayTextBox;
    
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
        totalMoney = PlayerStats.GetMoney();

        UpdateTotalMoneyText(); 

        if (shopItems.Length > 0)
        {
            HighlightItem(currentIndex);
        }
        else
        {
            Debug.LogError("No ShopItems assigned!");
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
            PlayerStats.SetMoney(totalMoney); 
            UpdateTotalMoneyText();
            return true;
        }
        return false;
    }

    /*private void UpdateTotalMoneyText()
    {
        if (totalMoneyText != null)
        {
            totalMoneyText.text = "$" + totalMoney.ToString("N2");
        }
    }*/ 

    private void UpdateTotalMoneyText()
    {
        if (totalMoneyText != null)
        {
            float money = PlayerStats.GetMoney();
            totalMoneyText.text = "$" + money.ToString("N2");
        }
    } 

    public void NavigateUp()
    {
        ResetItemColor(currentIndex);
        currentIndex = (currentIndex - 1 + shopItems.Length) % shopItems.Length;
        HighlightItem(currentIndex);
        ScrollToItem(currentIndex);
    }

    public void NavigateDown()
    {
        ResetItemColor(currentIndex);
        currentIndex = (currentIndex + 1) % shopItems.Length;
        HighlightItem(currentIndex);
        ScrollToItem(currentIndex);
    }

    public int GetShopItemIndex(GameObject shopItem)
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            if (shopItems[i] == shopItem)
            {
                return i;
            }
        }
        return -1;
    }

    public void SetCurrentIndex(int index)
    {
        currentIndex = index;
    }

    public void ResetAllShopItems()
    {
        foreach (GameObject shopItem in shopItems)
        {
            Image shopItemImage = shopItem.GetComponent<Image>();
            if (shopItemImage != null)
            {
                shopItemImage.color = Color.blue;
            }
        }
    }

    private void HighlightItem(int index)
    {
        Image shopItemImage = shopItems[index].GetComponent<Image>();
        if (shopItemImage != null)
        {
            shopItemImage.color = Color.black;
        }

        StoreButton storeButton = shopItems[index].GetComponent<StoreButton>();
        if (storeButton != null && displayText != null)
        {
            displayText.text = storeButton.GetButtonDetails();
            displayTextBox.SetActive(true);
        }
    }

    private void ResetItemColor(int index)
    {
        Image shopItemImage = shopItems[index].GetComponent<Image>();
        if (shopItemImage != null)
        {
            shopItemImage.color = Color.blue;
        }
    }

    private void ScrollToItem(int index)
    {
        if (scrollRect != null && content != null)
        {
            float contentHeight = content.rect.height;
            float itemHeight = shopItems[index].GetComponent<RectTransform>().rect.height;
            float normalizedPosition = 1 - ((index * itemHeight) / (contentHeight - scrollRect.viewport.rect.height));
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalizedPosition);
        }
    }

    public void OnBackClick()
    {
        HandleHangOutClick("ConvienceStoreFile");
    }

    private void HandleHangOutClick(string locationFile)
    {
        PlayerPrefs.SetString("StartingFile", locationFile);
        PlayerPrefs.Save();
        //VariableStore.TrySetValue("Default.StartingFile", locationFile);
        //VariableStore.TrySetValue("Default.LastScene", "TownMap"); 
        SceneManager.LoadScene("VisualNovel");
    }
}
