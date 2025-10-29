using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlanGenius : MenuPage
{
    public static PlanGenius Instance;

    [SerializeField] private GameObject[] panels; // Array of panels
    [SerializeField] private TMP_InputField[,] inputFields; // 2D array for input fields [panelIndex, fieldIndex]
    [SerializeField] private TextMeshProUGUI[,] charCounts; // 2D array for character counts
    [SerializeField] private Button backButton;
    [SerializeField] private Button forwardButton;

    private const int CharacterLimit = 50;
    private int currentPanelIndex = 0;

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

        // Initialize the arrays
        inputFields = new TMP_InputField[7, 3];
        charCounts = new TextMeshProUGUI[7, 3];
    }

    void Start()
    {
        for (int panelIndex = 0; panelIndex < panels.Length; panelIndex++)
        {
            // Get Input Fields and Character Counts for each panel
            inputFields[panelIndex, 0] = panels[panelIndex].transform.Find("InputFieldMorning").GetComponent<TMP_InputField>();
            inputFields[panelIndex, 1] = panels[panelIndex].transform.Find("InputFieldAfternoon").GetComponent<TMP_InputField>();
            inputFields[panelIndex, 2] = panels[panelIndex].transform.Find("InputFieldEvening").GetComponent<TMP_InputField>();

            charCounts[panelIndex, 0] = panels[panelIndex].transform.Find("CharCountMorning").GetComponent<TextMeshProUGUI>();
            charCounts[panelIndex, 1] = panels[panelIndex].transform.Find("CharCountAfternoon").GetComponent<TextMeshProUGUI>();
            charCounts[panelIndex, 2] = panels[panelIndex].transform.Find("CharCountEvening").GetComponent<TextMeshProUGUI>();

            // Set character limits and load saved values
            for (int fieldIndex = 0; fieldIndex < 3; fieldIndex++)
            {
                if (inputFields[panelIndex, fieldIndex] != null)
                {
                    int panel = panelIndex; // Capture index for lambda
                    int field = fieldIndex;

                    inputFields[panelIndex, fieldIndex].characterLimit = CharacterLimit;
                    inputFields[panelIndex, fieldIndex].onValueChanged.AddListener(text => UpdateCharCount(panel, field, text));

                    // Load saved value
                    string savedValue = PlayerPrefs.GetString($"Panel{panel}Field{field}", "");
                    inputFields[panelIndex, fieldIndex].text = savedValue;
                    UpdateCharCount(panel, field, savedValue);
                }
            }
        }

        // Initialize buttons
        backButton.onClick.AddListener(SwitchToPreviousPanel);
        forwardButton.onClick.AddListener(SwitchToNextPanel);

        UpdatePanelVisibility();
    }

    private void UpdateCharCount(int panelIndex, int fieldIndex, string text)
    {
        if (charCounts[panelIndex, fieldIndex] != null)
        {
            int currentCount = text?.Length ?? 0;
            charCounts[panelIndex, fieldIndex].text = $"{currentCount} / {CharacterLimit}";

            // Save input to PlayerPrefs
            PlayerPrefs.SetString($"Panel{panelIndex}Field{fieldIndex}", text);
        }
    }

    /*private void SwitchToPreviousPanel()
    {
        if (currentPanelIndex > 0)
        {
            currentPanelIndex--;
            UpdatePanelVisibility();
        }
    }*/

    /*private void SwitchToNextPanel()
    {
        if (currentPanelIndex < panels.Length - 1)
        {
            currentPanelIndex++;
            UpdatePanelVisibility();
        }
    }*/

    private void SwitchToPreviousPanel()
    {
        currentPanelIndex = (currentPanelIndex - 1 + panels.Length) % panels.Length;
        UpdatePanelVisibility();
    }

    private void SwitchToNextPanel()
    {
        currentPanelIndex = (currentPanelIndex + 1) % panels.Length;
        UpdatePanelVisibility();
    } 

    private void UpdatePanelVisibility()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == currentPanelIndex);
        }

        //backButton.interactable = currentPanelIndex > 0;
        //forwardButton.interactable = currentPanelIndex < panels.Length - 1;
    }

    private void ResetInputFields()
    {
        for (int panelIndex = 0; panelIndex < panels.Length; panelIndex++)
        {
            for (int fieldIndex = 0; fieldIndex < 3; fieldIndex++)
            {
                string key = $"Panel{panelIndex}Field{fieldIndex}";
                PlayerPrefs.DeleteKey(key);
                if (inputFields[panelIndex, fieldIndex] != null)
                {
                    inputFields[panelIndex, fieldIndex].text = string.Empty;
                    UpdateCharCount(panelIndex, fieldIndex, string.Empty);
                }
            }
        }
        PlayerPrefs.Save();
    }

    private void OnDayChangedHandler()
    {
        // Check if today is Sunday
        if (PlayerPrefs.GetInt("IsSunday", 0) == 1)
        {
            ResetInputFields(); // Reset input fields only on Sunday
        }
    } 

    private void OnEnable()
    {
        if (CalendarSystem.Instance != null)
        {
            CalendarSystem.Instance.onDayChanged += OnDayChangedHandler;
        }
    }

    private void OnDisable()
    {
        if (CalendarSystem.Instance != null)
        {
            CalendarSystem.Instance.onDayChanged -= OnDayChangedHandler;
        }
    }

    public void PrintAllValues()
    {
        for (int panelIndex = 0; panelIndex < panels.Length; panelIndex++)
        {
            for (int fieldIndex = 0; fieldIndex < 3; fieldIndex++)
            {
                string value = inputFields[panelIndex, fieldIndex]?.text ?? "Not assigned";
                Debug.Log($"Panel {panelIndex} Field {fieldIndex}: {value}");
            }
        }
    }
}
