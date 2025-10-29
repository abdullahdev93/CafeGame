using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InputPanel : MonoBehaviour
{
    public static InputPanel instance { get; private set; } = null;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private TMP_InputField firstNameField;
    [SerializeField] private TMP_InputField lastNameField;
    public TMP_Dropdown pronounDropdown;
    [SerializeField] private TMP_FontAsset customFont; // Reference to the custom font asset

    private CanvasGroupController cg;

    public string firstName { get; set; } = string.Empty;
    public string lastName { get; set; } = string.Empty;
    public string pronounSubject { get; private set; } = string.Empty; // "he", "she", "they"
    public string pronounObject { get; private set; } = string.Empty;  // "him", "her", "them"
    public string lastInput => $"{firstName} {lastName}";

    public bool isWaitingOnUserInput { get; private set; }

    private void Awake()
    {
        instance = this;

        firstName = PlayerPrefs.GetString("FirstName", "");
        lastName = PlayerPrefs.GetString("LastName", ""); 
    }

    void Start()
    {
        cg = new CanvasGroupController(this, canvasGroup);

        cg.alpha = 0;
        cg.SetInteractableState(active: false);
        acceptButton.interactable = false;

        // Add options to the pronoun dropdown
        pronounDropdown.ClearOptions();
        List<string> pronounOptions = new List<string> { "Select Pronoun", "HE/HIM", "SHE/HER", "THEY/THEM" };
        pronounDropdown.AddOptions(pronounOptions);

        // Apply the custom bold font to the dropdown options
        ApplyBoldFontToDropdown();

        // Listen for changes in input fields and dropdown
        firstNameField.onValueChanged.AddListener(delegate { OnInputChanged(); });
        lastNameField.onValueChanged.AddListener(delegate { OnInputChanged(); });
        pronounDropdown.onValueChanged.AddListener(delegate { OnInputChanged(); UpdatePlaceholders(); });

        acceptButton.onClick.AddListener(OnAcceptInput);

        // Load saved name color
        if (PlayerPrefs.HasKey("NameColor_FirstName"))
        {
            Color loadedColor;
            if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString("NameColor_FirstName"), out loadedColor))
            {
                titleText.color = loadedColor;
            }
        } 
    }

    private void ApplyBoldFontToDropdown()
    {
        if (customFont == null)
        {
            Debug.LogWarning("Custom Font is not assigned.");
            return;
        }

        // Set the font for the dropdown's label
        pronounDropdown.captionText.font = customFont;
        pronounDropdown.captionText.fontStyle = FontStyles.Bold;

        // Access the dropdown's template and modify its font
        Transform dropdownTemplate = pronounDropdown.template;
        if (dropdownTemplate)
        {
            TMP_Text itemText = dropdownTemplate.GetComponentInChildren<TMP_Text>();
            if (itemText)
            {
                itemText.font = customFont;
                itemText.fontStyle = FontStyles.Bold;
            }
        }

        // Update the font for each option in the dropdown list
        foreach (TMP_Dropdown.OptionData option in pronounDropdown.options)
        {
            option.text = $"<b>{option.text}</b>"; // Apply bold tags directly
        }

        // Refresh the dropdown to apply changes
        pronounDropdown.RefreshShownValue();
    }

    private void UpdatePlaceholders()
    {
        switch (pronounDropdown.value)
        {
            case 1: // HE/HIM
                firstNameField.placeholder.GetComponent<TMP_Text>().text = "Lucas";
                lastNameField.placeholder.GetComponent<TMP_Text>().text = "Reed";
                break;
            case 2: // SHE/HER
                firstNameField.placeholder.GetComponent<TMP_Text>().text = "Aurora";
                lastNameField.placeholder.GetComponent<TMP_Text>().text = "Reed";
                break;
            case 3: // THEY/THEM
                firstNameField.placeholder.GetComponent<TMP_Text>().text = "Sage";
                lastNameField.placeholder.GetComponent<TMP_Text>().text = "Reed";
                break;
            default:
                firstNameField.placeholder.GetComponent<TMP_Text>().text = "First Name...";
                lastNameField.placeholder.GetComponent<TMP_Text>().text = "Last Name...";
                break;
        }
    }

    public void Show(string title)
    {
        titleText.text = title;

        // Apply saved color again when the panel becomes visible
        if (PlayerPrefs.HasKey("NameColor_FirstName"))
        {
            if (ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString("NameColor_FirstName"), out Color loadedColor))
            {
                titleText.color = loadedColor;
            }
        } 

        firstNameField.text = string.Empty;
        lastNameField.text = string.Empty;
        pronounDropdown.value = 0;
        cg.Show();
        cg.SetInteractableState(active: true);
        isWaitingOnUserInput = true;

        // ShowColorPickerName()  
        //VNMenuManager.instance.colorPicker.gameObject.SetActive(true); 
    }

    public void Hide()
    {
        cg.Hide();
        cg.SetInteractableState(active: false);
        isWaitingOnUserInput = false;
    }

    public void OnAcceptInput()
    {
        if (!HasValidInput()) return;

        string firstInput = firstNameField.text;
        string lastInput = lastNameField.text;

        if (CensorManager.Censor(ref firstInput) || CensorManager.Censor(ref lastInput))
        {
            UIConfirmationMenu.instance.Show("Name Not Accepted!", new UIConfirmationMenu.ConfirmationButton("Okay", () => {
                firstNameField.text = string.Empty;
                lastNameField.text = string.Empty;
            }));
        }
        else
        {
            firstName = firstInput;
            lastName = lastInput;

            VariableStore.TrySetValue("PlayerStats.FirstName", firstName);
            VariableStore.TrySetValue("PlayerStats.LastName", lastName);

            PlayerPrefs.SetString("FirstName", firstName);
            PlayerPrefs.SetString("LastName", lastName);
            PlayerPrefs.Save(); 

            Debug.Log($"[InputPanel] Saved FirstName = {firstName}, LastName = {lastName}"); 

            // Assign pronouns based on dropdown selection
            switch (pronounDropdown.value)
            {
                case 1: // HE/HIM
                    pronounSubject = "he";
                    pronounObject = "him";
                    SetPronouns("he", "him", "his", "his", "himself");
                    //VNMenuManager.instance.UpdateMCImage("HE/HIM");
                    VNMenuManager.instance.UpdateMCImage("HE/HIM");
                    // Add debug line here
                    Debug.Log($"Subject: {PlayerStats.instance.Pronouns.Subject}, Object: {PlayerStats.instance.Pronouns.Object}");
                    break;
                case 2: // SHE/HER
                    pronounSubject = "she";
                    pronounObject = "her";
                    SetPronouns("she", "her", "her", "hers", "herself"); 
                    VNMenuManager.instance.UpdateMCImage("SHE/HER"); 
                    break;
                case 3: // THEY/THEM
                    pronounSubject = "they";
                    pronounObject = "them";
                    SetPronouns("they", "them", "their", "theirs", "themself"); 
                    VNMenuManager.instance.UpdateMCImage("THEY/THEM"); 
                    break;
                default:
                    pronounSubject = string.Empty;
                    pronounObject = string.Empty;
                    break;
            }

            Hide();
        }
    }

    private void SetPronouns(string subject, string obj, string possAdj, string possPro, string reflexive)
    {
        PlayerStats.instance.SetPronoun("Subject", subject);
        PlayerStats.instance.SetPronoun("Object", obj);
        PlayerStats.instance.SetPronoun("PossessiveAdjective", possAdj);
        PlayerStats.instance.SetPronoun("PossessivePronoun", possPro);
        PlayerStats.instance.SetPronoun("Reflexive", reflexive);

        // Save to PlayerPrefs as backup (optional)
        PlayerPrefs.SetString("Pronoun_Subject", subject);
        PlayerPrefs.SetString("Pronoun_Object", obj);
        PlayerPrefs.SetString("Pronoun_PossessiveAdjective", possAdj);
        PlayerPrefs.SetString("Pronoun_PossessivePronoun", possPro);
        PlayerPrefs.SetString("Pronoun_Reflexive", reflexive);
        PlayerPrefs.Save();
    }

    public void SetTitleTextColor(Color color)
    {
        titleText.color = color;

        // Force a UI refresh
        titleText.ForceMeshUpdate();

        // Save the selected color
        PlayerPrefs.SetString("NameColor_FirstName", ColorUtility.ToHtmlStringRGBA(color));
        PlayerPrefs.Save();

        Debug.Log("[InputPanel] Saved Title Color to PlayerPrefs: " + color); 
    } 

    private void OnInputChanged()
    {
        acceptButton.interactable = HasValidInput();
    }

    private bool HasValidInput()
    {
        return !string.IsNullOrEmpty(firstNameField.text) &&
               !string.IsNullOrEmpty(lastNameField.text) &&
               pronounDropdown.value != 0;
    } 
}
