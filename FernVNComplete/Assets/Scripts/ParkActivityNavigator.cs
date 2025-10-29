using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ParkActivityNavigator : MonoBehaviour 
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject[] parkActivities;
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
        totalMoney = PlayerPrefs.GetFloat("TotalMoney", 1000f);
        UpdateTotalMoneyText();
        //FilterActivitiesBySeasonAndPhase();
        //FilterActivitiesBySeasonPhaseAndWeather();
        FilterActivitiesBySeasonPhaseWeatherAndDay();

        if (parkActivities.Length > 0)
        {
            HighlightItem(currentIndex);
        }
        else
        {
            Debug.LogError("No ShopItems assigned!");
        }

        displayText.text = "";
        displayTextBox.SetActive(false);
        ResetAllParkActivities();
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

    private void FilterActivitiesBySeasonPhaseWeatherAndDay() 
    {
        CalendarSystem.Season currentSeason = CalendarSystem.Instance.GetCurrentSeason();
        string currentPhase = CalendarSystem.Instance.activityPhases[CalendarSystem.Instance.activityCounter];
        CalendarSystem.Weather currentWeather = CalendarSystem.Instance.GetCurrentWeather();
        string currentDayOfWeek = CalendarSystem.Instance.GetCurrentDayOfWeek(); 

        foreach (GameObject activity in parkActivities)
        {
            ParkButton parkButton = activity.GetComponent<ParkButton>();
            if (parkButton != null)
            {
                //bool isAvailable = parkButton.IsAvailable(currentSeason, currentPhase);
                //bool isAvailable = parkButton.IsAvailable(currentSeason, currentPhase, currentWeather);
                bool isAvailable = parkButton.IsAvailable(currentSeason, currentPhase, currentWeather, currentDayOfWeek);
                activity.SetActive(isAvailable);
            }
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
            PlayerPrefs.SetFloat("TotalMoney", totalMoney);
            PlayerPrefs.Save();
            UpdateTotalMoneyText();
            return true;
        }
        return false;
    }

    private void UpdateTotalMoneyText()
    {
        if (totalMoneyText != null)
        {
            totalMoneyText.text = "$" + totalMoney.ToString("N2");
        }
    }

    public void NavigateUp()
    {
        ResetItemColor(currentIndex);
        do
        {
            currentIndex = (currentIndex - 1 + parkActivities.Length) % parkActivities.Length;
        } while (!parkActivities[currentIndex].activeSelf);
        HighlightItem(currentIndex);
        ScrollToItem(currentIndex);
    }

    public void NavigateDown()
    {
        ResetItemColor(currentIndex);
        do
        {
            currentIndex = (currentIndex + 1) % parkActivities.Length;
        } while (!parkActivities[currentIndex].activeSelf);
        HighlightItem(currentIndex);
        ScrollToItem(currentIndex);
    }

    public int GetShopItemIndex(GameObject shopItem)
    {
        for (int i = 0; i < parkActivities.Length; i++)
        {
            if (parkActivities[i] == shopItem)
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

    public void ResetAllParkActivities()
    {
        foreach (GameObject shopItem in parkActivities)
        {
            Image shopItemImage = shopItem.GetComponent<Image>();
            if (shopItemImage != null)
            {
                shopItemImage.color = new Color(1f, 0.41f, 0.71f); // RGB: 255, 105, 180 (Hot Pink)   
            }
        }
    }

    private void HighlightItem(int index)
    {
        Image shopItemImage = parkActivities[index].GetComponent<Image>();
        if (shopItemImage != null)
        {
            shopItemImage.color = Color.black;
        }

        StoreButton storeButton = parkActivities[index].GetComponent<StoreButton>();
        if (storeButton != null && displayText != null)
        {
            displayText.text = storeButton.GetButtonDetails();
            displayTextBox.SetActive(true);
        }
    }

    private void ResetItemColor(int index)
    {
        Image shopItemImage = parkActivities[index].GetComponent<Image>();
        if (shopItemImage != null)
        {
            shopItemImage.color = new Color(1f, 0.41f, 0.71f); // RGB: 255, 105, 180 (Hot Pink)   
        }
    }

    private void ScrollToItem(int index)
    {
        if (scrollRect != null && content != null)
        {
            float contentHeight = content.rect.height;
            float itemHeight = parkActivities[index].GetComponent<RectTransform>().rect.height;
            float normalizedPosition = 1 - ((index * itemHeight) / (contentHeight - scrollRect.viewport.rect.height));
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalizedPosition);
        }
    }

    public void OnBackClick()
    {
        HandleHangOutClick("ParkFile"); 
    }

    private void HandleHangOutClick(string locationFile)
    {
        PlayerPrefs.SetString("StartingFile", locationFile);
        PlayerPrefs.Save();
        SceneManager.LoadScene("VisualNovel");
    }
}
