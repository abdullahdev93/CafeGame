using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using CHARACTERS;

public class ConfigMenu : MenuPage
{
    public static ConfigMenu instance { get; private set; }

    //private AutoReader theAutoReader => AutoReader.reader; 

    [SerializeField] private GameObject[] panels;
    private GameObject activePanel;
    //public GameObject skipButton;
    public GameObject skipArrowPressHold;
    public GameObject skipArrowPress; 
    public GameObject skipBehaviorObject; 

    public TextMeshProUGUI dialogueTextSpeedText;
    public TextMeshProUGUI dialogueAutoReadSpeedText;
    public TextMeshProUGUI dialogueSkipSpeedText;
    public TextMeshProUGUI musicText;
    public TextMeshProUGUI sfxText;
    public TextMeshProUGUI voicesText; 


    //public float holdDownSkipTimer; 

    public UI_ITEMS ui;

    private bool skipChoice => config.SkipDisplay; 

    //private bool skipMode => autoReader.skip; 

    //private bool skipPressedAndHeld = false; 

    private VN_Configuration config => VN_Configuration.activeConfig;

    //public bool defaultValue = false; 

    //private CharacterConfigData characterTextSize => CharacterConfigData.activeCharacterConfig;  

    //private DialogueSystemConfigurationSO dialogueSystemConfig;


    //private float theTextSize => dialogueSystemConfig.dialogueFontScale; 

    //private float smallText = 1f;

    //private float largeText = 1.5f; 

    //private AutoReader autoReader => AutoReader.reader; 

    private void Awake()
    {
        instance = this; 
    }

    // Start is called before the first frame update
    void Start()
    { 
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == 0);
        }

        activePanel = panels[0];

        SetAvailableResolutions();

        LoadConfig();
    }

    void Update()
    {
        SkipControls(skipChoice);

        /*if (skipPressedAndHeld)
            holdDownSkipTimer -= Time.deltaTime;

        if (holdDownSkipTimer <= 0)  
            holdDownSkipTimer = 0;*/ 

        //HoldSkipButton(); 
    }

    private void LoadConfig()
    {
        if (File.Exists(VN_Configuration.filePath))
            VN_Configuration.activeConfig = FileManager.Load<VN_Configuration>(VN_Configuration.filePath, encrypt: VN_Configuration.ENCRYPT);
        else
            VN_Configuration.activeConfig = new VN_Configuration();

        VN_Configuration.activeConfig.Load();
    }

    private void OnApplicationQuit()
    {
        VN_Configuration.activeConfig.Save();
        VN_Configuration.activeConfig = null;
    }

    public void OpenPanel(string panelName)
    {
        GameObject panel = panels.First(p => p.name.ToLower() == panelName.ToLower());

        if (panel == null)
        {
            Debug.LogWarning($"Did not find panel called '{panelName}' in config menu.");
            return;
        }

        if (activePanel != null && activePanel != panel)
            activePanel.SetActive(false);

        panel.SetActive(true);
        activePanel = panel;
    }

    private void SetAvailableResolutions()
    {
        Resolution[] resolutions = Screen.resolutions;
        List<string> options = new List<string>();  

        for (int i = resolutions.Length - 1; i >= 0; i--)
        {
            options.Add($"{resolutions[i].width}x{resolutions[i].height}");
        }

        ui.resolutions.ClearOptions();
        ui.resolutions.AddOptions(options);
    }

    [System.Serializable]
    public class UI_ITEMS
    {
        private static Color button_selectedColor = new Color(1, 0.35f, 0, 1);
        private static Color button_unselectedColor = new Color(1f, 1f, 1f, 1);
        private static Color text_selectedColor = new Color(1f, 1f, 0.6f, 1f); 
        private static Color text_unselectedColor = new Color(0.25f, 0.25f, 0.25f, 1);
        public static Color musicOnColor = new Color(1, 0.65f, 0, 1);
        public static Color musicOffColor = new Color(0.5f, 0.5f, 0.5f, 1);

        [Header("General")]
        public Button fullscreen;
        public Button windowed;
        public TMP_Dropdown resolutions;
        public Button skippingContinue, skippingStop, textSoundOn, textSoundOff, press, pressAndHold, autoContinue, autoStop, smallText, mediumText, largeText, slowText, normalText, fastText, slowSkipText, normalSkipText, fastSkipText, slowAutoText, normalAutoText, fastAutoText, enableAutoSave, disableAutoSave, fahrenheitButton, celsiusButton, colorBlindOn, colorBlindOff, monoAudio, stereoAudio, dialoguePromtDefault, dialoguePromtALT, subtitlesOn, subtitlesOff;    
        //public Slider /*architectSpeed,*/ autoReaderSpeed/*, skipSpeed, theTextSize*/;    

        [Header("Audio")]
        public Slider musicVolume;
        public Image musicFill;
        public Slider sfxVolume;
        public Image sfxFill;
        public Slider voicesVolume;
        public Image voicesFill;
        public Sprite mutedSymbol;
        public Sprite unmutedSymbol;
        public Image musicMute;
        public Image sfxMute;
        public Image voicesMute;

        public void SetButtonColors(Button A, Button B, bool selectedA)
        {
            A.GetComponent<Image>().color = selectedA ? button_selectedColor : button_unselectedColor;
            B.GetComponent<Image>().color = !selectedA ? button_selectedColor : button_unselectedColor;

            A.GetComponentInChildren<TextMeshProUGUI>().color = selectedA ? text_selectedColor : text_unselectedColor;
            B.GetComponentInChildren<TextMeshProUGUI>().color = !selectedA ? text_selectedColor : text_unselectedColor;
        }

        public void SetButtonColors(Button A, Button B, Button C, int selectedButtonIndex)
        {
            // Set colors for button A
            A.GetComponent<Image>().color = (selectedButtonIndex == 0) ? button_selectedColor : button_unselectedColor;
            A.GetComponentInChildren<TextMeshProUGUI>().color = (selectedButtonIndex == 0) ? text_selectedColor : text_unselectedColor;

            // Set colors for button B
            B.GetComponent<Image>().color = (selectedButtonIndex == 1) ? button_selectedColor : button_unselectedColor;
            B.GetComponentInChildren<TextMeshProUGUI>().color = (selectedButtonIndex == 1) ? text_selectedColor : text_unselectedColor;

            // Set colors for button C
            C.GetComponent<Image>().color = (selectedButtonIndex == 2) ? button_selectedColor : button_unselectedColor;
            C.GetComponentInChildren<TextMeshProUGUI>().color = (selectedButtonIndex == 2) ? text_selectedColor : text_unselectedColor;
        }

    }

    //UI CALLABLE FUNCTIONS
    public void SetDisplayToFullScreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        ui.SetButtonColors(ui.fullscreen, ui.windowed, fullscreen);
    }

    public void SetDisplayResolution()
    {
        string resolution = ui.resolutions.captionText.text;
        string[] values = resolution.Split('x');

        if (int.TryParse(values[0], out int width) && int.TryParse(values[1], out int height))
        {
            Screen.SetResolution(width, height, Screen.fullScreen);
            config.display_resolution = resolution;
        }
        else
            Debug.LogError($"Parsing error for screen resolution! [{resolution}] could not be parsed into WIDTHxHEIGHT");
    }

    public void SetContinueSkippingAfterChoice(bool continueSkipping)
    {
        config.continueSkippingAfterChoice = continueSkipping;
        ui.SetButtonColors(ui.skippingContinue, ui.skippingStop, continueSkipping);
    }

    public void SetContinueAutoAfterChoice(bool continueAuto)
    {
        config.continueAutoAfterChoice = continueAuto;
        ui.SetButtonColors(ui.autoContinue, ui.autoStop, continueAuto); 
    }

    public void SetTemperatureUnit(bool celsius)
    {
        config.useCelsius = celsius;
        ui.SetButtonColors(ui.celsiusButton, ui.fahrenheitButton, celsius);

        if (CalendarSystem.Instance != null)
        {
            CalendarSystem.Instance.UpdateSeasonDayText(); // Refresh temp text
        }
    } 

    public void SetAutoSave(bool autoSave)
    {
        config.controlAutoSave = autoSave;
        ui.SetButtonColors(ui.enableAutoSave, ui.disableAutoSave, autoSave);

        AutoSaveManager.instance.ToggleAutoSave(autoSave);

        //if (autoSave)
            //AutoSaveManager.instance.ToggleAutoSave(autoSave); 
    }

    public void TextSpeed(int textSpeed)
    {
        config.controlTextSpeed = textSpeed;
        ui.SetButtonColors(ui.slowText, ui.normalText, ui.fastText, textSpeed);

        // Set the architect speed based on the selected text speed
        float architectSpeed = 1f; // Default to normal speed 
        switch (textSpeed)
        {
            case 0: // Slow text speed
                architectSpeed = 0.25f;  
                break;
            case 1: // Normal text speed
                architectSpeed = 1f; 
                break;
            case 2: // Fast text speed
                architectSpeed = 4f;  
                break;
            default:
                architectSpeed = 1f; // Default to normal speed if no valid selection
                break;
        }

        // Update the architect speed in the DialogueSystem
        if (DialogueSystem.instance != null && DialogueSystem.instance.conversationManager != null)
        {
            DialogueSystem.instance.conversationManager.architect.speed = architectSpeed;
        }

        // Save the speed to config so it persists across sessions
        config.dialogueTextSpeed = architectSpeed;
    }

    public void TextSkipSpeed(int textSpeed) 
    {
        config.dialogueSkipSpeed = textSpeed;
        ui.SetButtonColors(ui.slowSkipText, ui.normalSkipText, ui.fastSkipText, textSpeed); 

        // Set the architect speed based on the selected text speed
        float dialogueSkipSpeed = 0.25f; // Default to normal speed
        switch (textSpeed)
        {
            case 0: // Slow text speed
                dialogueSkipSpeed = 1.25f; 
                break;
            case 1: // Normal text speed
                dialogueSkipSpeed = 0.25f; 
                break;
            case 2: // Fast text speed
                dialogueSkipSpeed = 0.05f;
                break;
            default:
                dialogueSkipSpeed = 0.25f; // Default to normal speed if no valid selection 
                break;
        }

        // Update the architect speed in the DialogueSystem
        if (DialogueSystem.instance != null && DialogueSystem.instance.autoReader != null)
        {
            DialogueSystem.instance.autoReader.skipSpeed = dialogueSkipSpeed;
        }

        // Save the speed to config so it persists across sessions
        config.dialogueTextSpeed = dialogueSkipSpeed; 
    }

    public void TextAutoSpeed(int textSpeed)
    {
        config.dialogueSkipSpeed = textSpeed; 
        ui.SetButtonColors(ui.slowAutoText, ui.normalAutoText, ui.fastAutoText, textSpeed); 

        // Set the architect speed based on the selected text speed
        float dialogueAutoSpeed = 0.25f; // Default to normal speed
        switch (textSpeed)
        {
            case 0: // Slow text speed
                dialogueAutoSpeed = 0.4f;      
                break;
            case 1: // Normal text speed
                dialogueAutoSpeed = 0.8f;  
                break;
            case 2: // Fast text speed
                dialogueAutoSpeed = 1.6f;        
                break;
            default:
                dialogueAutoSpeed = 0.8f; // Default to normal speed if no valid selection  
                break;
        }

        // Update the architect speed in the DialogueSystem
        if (DialogueSystem.instance != null && DialogueSystem.instance.autoReader != null)
        {
            DialogueSystem.instance.autoReader.speed = dialogueAutoSpeed; 
        }

        // Save the speed to config so it persists across sessions
        config.dialogueAutoReadSpeed = dialogueAutoSpeed; 
    }

    public void TextSoundSetting(bool textSound)
    {
        config.PlayTextSound = textSound;
        ui.SetButtonColors(ui.textSoundOn, ui.textSoundOff, textSound); 
    }

    public void SetTextSize(int textSize)
    {
        config.ShowTextSize = textSize;
        ui.SetButtonColors(ui.smallText, ui.mediumText, ui.largeText, textSize);

        // Set the font size based on the selected option
        float dialogueFontSize = 16f; // Default small size  

        switch (textSize)
        {
            case 0: // Small text
                dialogueFontSize = 16f;
                break;
            case 1: // Medium text
                dialogueFontSize = 18f;
                break;
            case 2: // Large text
                dialogueFontSize = 20f;  
                break;
        }

        // Apply the font size to the dialogue system if available
        if (DialogueSystem.instance != null && DialogueSystem.instance.conversationManager != null)
        {
            DialogueSystem.instance.conversationManager.architect.fontSize = dialogueFontSize;
        }

        // Save the font size to config so it persists across sessions
        config.dialogueFontSize = dialogueFontSize;
    } 

    public void SkipControls(bool skipOption)
    {
        config.SkipDisplay = skipOption;
        ui.SetButtonColors(ui.press, ui.pressAndHold, skipOption); 
        
        if (skipOption)
        {
            ui.skippingContinue.interactable = true;
            ui.skippingStop.interactable = true;
            //Destroy(skipArrowPressHold.GetComponent<EventTrigger>()); 
            //skipButton.SetActive(true); 
            skipArrowPressHold.SetActive(false);
            skipArrowPress.SetActive(true); 
            skipBehaviorObject.GetComponent<Image>().color = new Color32(0, 0, 0, 50); 
        } 

        else
        {
            ui.skippingContinue.interactable = false;
            ui.skippingStop.interactable = false;
            //skipButton.SetActive(false);
            skipArrowPressHold.SetActive(true);
            skipArrowPress.SetActive(false); 
            skipBehaviorObject.GetComponent<Image>().color = new Color32(0, 0, 0, 134); 
        }
    } 

    //public void HoldSkipButton()
    //{
        //skipPressedAndHeld = true; 
    //} 
    
    //public void ResetSkipHoldDownTimer()
    //{
        //skipPressedAndHeld = false;

        //holdDownSkipTimer = 5f; 

        //skipMode = false; 
    //}

    //public void HoldUpAfterSkipTime()
    //{
        //if (holdDownSkipTimer <= 0) 
            //AutoReader.reader.Toggle_Skip(); 
    //}

    /*public void SetTextArchitectSpeed()
    {
        config.dialogueTextSpeed = ui.architectSpeed.value / 5f; 

        //dialogueTextSpeedText.text = Mathf.Round(ui.architectSpeed.value * 10).ToString() + "%";  
        
        //if (ui.architectSpeed.value == 0.025)

        if (DialogueSystem.instance != null)
            DialogueSystem.instance.conversationManager.architect.speed = config.dialogueTextSpeed; 
    }*/ 

    /*public void SetAutoReaderSpeed()
    {
        config.dialogueAutoReadSpeed = ui.autoReaderSpeed.value / 4f;    

        //dialogueAutoReadSpeedText.text = Mathf.Round(ui.autoReaderSpeed.value * 10).ToString() + "%"; 

        if (DialogueSystem.instance == null)
            return;

        AutoReader autoReader = DialogueSystem.instance.autoReader;
        if (autoReader != null) 
            autoReader.speed = config.dialogueAutoReadSpeed;
    }*/ 

    /*public void SetSkipSpeed()
    {
        config.dialogueSkipSpeed = ui.skipSpeed.value / 10f; 

        //dialogueSkipSpeedText.text = Mathf.Round(ui.skipSpeed.value * 10).ToString() + "%";  
        
        /*if (ui.skipSpeed.value <= 1f) 
        {
            dialogueSkipSpeedText.text = "1%"; 
        }

        if (ui.skipSpeed.value <= 0.99f)
        {
            dialogueSkipSpeedText.text = "2%";
        }

        if (ui.skipSpeed.value <= 0.98f)
        {
            dialogueSkipSpeedText.text = "3%";
        }

        if (ui.skipSpeed.value <= 0.97f)
        {
            dialogueSkipSpeedText.text = "4%";
        }

        if (ui.skipSpeed.value <= 0.96f)
        {
            dialogueSkipSpeedText.text = "5%";
        }

        if (ui.skipSpeed.value <= 0.95f)
        {
            dialogueSkipSpeedText.text = "6%";
        }

        if (ui.skipSpeed.value <= 0.94f)
        {
            dialogueSkipSpeedText.text = "7%";
        }

        if (ui.skipSpeed.value <= 0.93f)
        {
            dialogueSkipSpeedText.text = "8%";
        }

        if (ui.skipSpeed.value <= 0.92f)
        {
            dialogueSkipSpeedText.text = "9%";
        }

        if (ui.skipSpeed.value <= 0.91f)
        {
            dialogueSkipSpeedText.text = "10%";
        }

        if (ui.skipSpeed.value <= 0.9f)
        {
            dialogueSkipSpeedText.text = "11%"; 
        }

        if (ui.skipSpeed.value <= 0.89f)
        {
            dialogueSkipSpeedText.text = "12%";
        }

        if (ui.skipSpeed.value <= 0.88f)
        {
            dialogueSkipSpeedText.text = "13%";
        }

        if (ui.skipSpeed.value <= 0.87f)
        {
            dialogueSkipSpeedText.text = "14%";
        }

        if (ui.skipSpeed.value <= 0.86f)
        {
            dialogueSkipSpeedText.text = "15%";
        }

        if (ui.skipSpeed.value <= 0.85f)
        {
            dialogueSkipSpeedText.text = "16%";
        }

        if (ui.skipSpeed.value <= 0.84f)
        {
            dialogueSkipSpeedText.text = "17%";
        }

        if (ui.skipSpeed.value <= 0.83f)
        {
            dialogueSkipSpeedText.text = "18%";
        }

        if (ui.skipSpeed.value <= 0.82f)
        {
            dialogueSkipSpeedText.text = "19%";
        }

        if (ui.skipSpeed.value <= 0.81f)
        {
            dialogueSkipSpeedText.text = "20%";
        }

        if (ui.skipSpeed.value <= 0.8f)
        {
            dialogueSkipSpeedText.text = "21%";
        }

        if (ui.skipSpeed.value <= 0.79f)
        {
            dialogueSkipSpeedText.text = "22%";
        }

        if (ui.skipSpeed.value <= 0.78f)
        {
            dialogueSkipSpeedText.text = "23%";
        }

        if (ui.skipSpeed.value <= 0.77f)
        {
            dialogueSkipSpeedText.text = "24%";
        }

        if (ui.skipSpeed.value <= 0.76f)
        {
            dialogueSkipSpeedText.text = "25%";
        }

        if (ui.skipSpeed.value <= 0.75f)
        {
            dialogueSkipSpeedText.text = "26%";
        }

        if (ui.skipSpeed.value <= 0.74f)
        {
            dialogueSkipSpeedText.text = "27%";
        }

        if (ui.skipSpeed.value <= 0.73f)
        {
            dialogueSkipSpeedText.text = "28%";
        }

        if (ui.skipSpeed.value <= 0.72f)
        {
            dialogueSkipSpeedText.text = "29%";
        }

        if (ui.skipSpeed.value <= 0.71f)
        {
            dialogueSkipSpeedText.text = "30%";
        }

        if (ui.skipSpeed.value <= 0.7f)
        {
            dialogueSkipSpeedText.text = "31%";
        }

        if (ui.skipSpeed.value <= 0.69f)
        {
            dialogueSkipSpeedText.text = "32%";
        }

        if (ui.skipSpeed.value <= 0.68f)
        {
            dialogueSkipSpeedText.text = "33%";
        }

        if (ui.skipSpeed.value <= 0.67f)
        {
            dialogueSkipSpeedText.text = "34%";
        }

        if (ui.skipSpeed.value <= 0.66f)
        {
            dialogueSkipSpeedText.text = "35%";
        }

        if (ui.skipSpeed.value <= 0.65f)
        {
            dialogueSkipSpeedText.text = "36%";
        }

        if (ui.skipSpeed.value <= 0.64f)
        {
            dialogueSkipSpeedText.text = "37%";
        }

        if (ui.skipSpeed.value <= 0.63f)
        {
            dialogueSkipSpeedText.text = "38%";
        }

        if (ui.skipSpeed.value <= 0.62f)
        {
            dialogueSkipSpeedText.text = "39%";
        }

        if (ui.skipSpeed.value <= 0.61f)
        {
            dialogueSkipSpeedText.text = "40%";
        }

        if (ui.skipSpeed.value <= 0.6f)
        {
            dialogueSkipSpeedText.text = "41%";
        }

        if (ui.skipSpeed.value <= 0.59f)
        {
            dialogueSkipSpeedText.text = "42%";
        }

        if (ui.skipSpeed.value <= 0.58f)
        {
            dialogueSkipSpeedText.text = "43%";
        }

        if (ui.skipSpeed.value <= 0.57f)
        {
            dialogueSkipSpeedText.text = "44%";
        }

        if (ui.skipSpeed.value <= 0.56f)
        {
            dialogueSkipSpeedText.text = "45%";
        }

        if (ui.skipSpeed.value <= 0.55f)
        {
            dialogueSkipSpeedText.text = "46%";
        }

        if (ui.skipSpeed.value <= 0.54f)
        {
            dialogueSkipSpeedText.text = "47%";
        }

        if (ui.skipSpeed.value <= 0.53f)
        {
            dialogueSkipSpeedText.text = "48%";
        }

        if (ui.skipSpeed.value <= 0.52f)
        {
            dialogueSkipSpeedText.text = "49%";
        }

        if (ui.skipSpeed.value <= 0.51f)
        {
            dialogueSkipSpeedText.text = "50%";
        }

        if (ui.skipSpeed.value <= 0.5f)
        {
            dialogueSkipSpeedText.text = "51%";
        }

        if (ui.skipSpeed.value <= 0.49f)
        {
            dialogueSkipSpeedText.text = "52%";
        }

        if (ui.skipSpeed.value <= 0.48f)
        {
            dialogueSkipSpeedText.text = "53%";
        }

        if (ui.skipSpeed.value <= 0.47f)
        {
            dialogueSkipSpeedText.text = "54%";
        }

        if (ui.skipSpeed.value <= 0.46f)
        {
            dialogueSkipSpeedText.text = "55%";
        }

        if (ui.skipSpeed.value <= 0.45f)
        {
            dialogueSkipSpeedText.text = "56%";
        }

        if (ui.skipSpeed.value <= 0.44f)
        {
            dialogueSkipSpeedText.text = "57%";
        }

        if (ui.skipSpeed.value <= 0.43f)
        {
            dialogueSkipSpeedText.text = "58%";
        }

        if (ui.skipSpeed.value <= 0.42f)
        {
            dialogueSkipSpeedText.text = "59%";
        }

        if (ui.skipSpeed.value <= 0.41f)
        {
            dialogueSkipSpeedText.text = "60%";
        }

        if (ui.skipSpeed.value <= 0.4f)
        {
            dialogueSkipSpeedText.text = "61%";
        }

        if (ui.skipSpeed.value <= 0.39f)
        {
            dialogueSkipSpeedText.text = "62%";
        }

        if (ui.skipSpeed.value <= 0.38f)
        {
            dialogueSkipSpeedText.text = "63%";
        }

        if (ui.skipSpeed.value <= 0.37f)
        {
            dialogueSkipSpeedText.text = "64%";
        }

        if (ui.skipSpeed.value <= 0.36f)
        {
            dialogueSkipSpeedText.text = "65%";
        }

        if (ui.skipSpeed.value <= 0.35f)
        {
            dialogueSkipSpeedText.text = "66%";
        }

        if (ui.skipSpeed.value <= 0.34f)
        {
            dialogueSkipSpeedText.text = "67%";
        }

        if (ui.skipSpeed.value <= 0.33f) 
        {
            dialogueSkipSpeedText.text = "68%";
        }

        if (ui.skipSpeed.value <= 0.32f)
        {
            dialogueSkipSpeedText.text = "69%";
        }

        if (ui.skipSpeed.value <= 0.31f)
        {
            dialogueSkipSpeedText.text = "70%";
        }

        if (ui.skipSpeed.value <= 0.3f)
        {
            dialogueSkipSpeedText.text = "71%";
        }

        if (ui.skipSpeed.value <= 0.29f)
        {
            dialogueSkipSpeedText.text = "72%";
        }

        if (ui.skipSpeed.value <= 0.28f)
        {
            dialogueSkipSpeedText.text = "73%";
        }

        if (ui.skipSpeed.value <= 0.27f)
        {
            dialogueSkipSpeedText.text = "74%";
        }

        if (ui.skipSpeed.value <= 0.26f)
        {
            dialogueSkipSpeedText.text = "75%";
        }

        if (ui.skipSpeed.value <= 0.25f)
        {
            dialogueSkipSpeedText.text = "76%";
        }

        if (ui.skipSpeed.value <= 0.24f)
        {
            dialogueSkipSpeedText.text = "77%";
        }

        if (ui.skipSpeed.value <= 0.23f)
        {
            dialogueSkipSpeedText.text = "78%";
        }

        if (ui.skipSpeed.value <= 0.22f)
        {
            dialogueSkipSpeedText.text = "79%";
        }

        if (ui.skipSpeed.value <= 0.21f)
        {
            dialogueSkipSpeedText.text = "80%";
        }

        if (ui.skipSpeed.value <= 0.2f)
        {
            dialogueSkipSpeedText.text = "81%";
        }

        if (ui.skipSpeed.value <= 0.19f)
        {
            dialogueSkipSpeedText.text = "82%";
        }

        if (ui.skipSpeed.value <= 0.18f)
        {
            dialogueSkipSpeedText.text = "83%";
        }

        if (ui.skipSpeed.value <= 0.17f)
        {
            dialogueSkipSpeedText.text = "84%";
        }

        if (ui.skipSpeed.value <= 0.16f)
        {
            dialogueSkipSpeedText.text = "85%";
        }

        if (ui.skipSpeed.value <= 0.15f)
        {
            dialogueSkipSpeedText.text = "86%";
        }

        if (ui.skipSpeed.value <= 0.14f)
        {
            dialogueSkipSpeedText.text = "87%";
        }

        if (ui.skipSpeed.value <= 0.13f)
        {
            dialogueSkipSpeedText.text = "88%";
        }

        if (ui.skipSpeed.value <= 0.12f)
        {
            dialogueSkipSpeedText.text = "89%";
        }

        if (ui.skipSpeed.value <= 0.11f)
        {
            dialogueSkipSpeedText.text = "90%";
        }

        if (ui.skipSpeed.value <= 0.1f)
        {
            dialogueSkipSpeedText.text = "91%";
        }

        if (ui.skipSpeed.value <= 0.09f)
        {
            dialogueSkipSpeedText.text = "92%";
        }

        if (ui.skipSpeed.value <= 0.08f)
        {
            dialogueSkipSpeedText.text = "93%";
        }

        if (ui.skipSpeed.value <= 0.07f)
        {
            dialogueSkipSpeedText.text = "94%";
        }

        if (ui.skipSpeed.value <= 0.06f)
        {
            dialogueSkipSpeedText.text = "95%";
        }

        if (ui.skipSpeed.value <= 0.05f)
        {
            dialogueSkipSpeedText.text = "96%";
        }

        if (ui.skipSpeed.value <= 0.04f)
        {
            dialogueSkipSpeedText.text = "97%";
        }

        if (ui.skipSpeed.value <= 0.03f)
        {
            dialogueSkipSpeedText.text = "98%";
        }

        if (ui.skipSpeed.value <= 0.02f)
        {
            dialogueSkipSpeedText.text = "99%"; 
        }

        if (ui.skipSpeed.value <= 0.01f) 
        {
            dialogueSkipSpeedText.text = "100%"; 
        }

        if (DialogueSystem.instance == null)
            return;

        AutoReader autoReader = DialogueSystem.instance.autoReader;
        if (autoReader != null)
            autoReader.skipSpeed = config.dialogueSkipSpeed; 

    }*/ 

    public void SetMusicVolume()
    {
        config.musicVolume = ui.musicVolume.value; 

        musicText.text = Mathf.Round(ui.musicVolume.value * 100).ToString() + "%"; 

        AudioManager.instance.SetMusicVolume(config.musicVolume, config.musicMute);

        ui.musicFill.color = config.musicMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
    }

    public void SetSFXVolume()
    {
        config.sfxVolume = ui.sfxVolume.value; 

        sfxText.text = Mathf.Round(ui.sfxVolume.value * 100).ToString() + "%";

        AudioManager.instance.SetSFXVolume(config.sfxVolume, config.sfxMute);

        ui.sfxFill.color = config.sfxMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
    }

    public void SetVoicesVolume()
    {
        config.voicesVolume = ui.voicesVolume.value; 

        voicesText.text = Mathf.Round(ui.voicesVolume.value * 100).ToString() + "%"; 

        AudioManager.instance.SetVoicesVolume(config.voicesVolume, config.voicesMute);

        ui.voicesFill.color = config.voicesMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
    }

    public void SetMusicMute()
    { 
        if (config.musicMute = !config.musicMute)
        {
            musicText.text = "Muted";

            ui.musicVolume.fillRect.GetComponent<Image>().color = config.musicMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
            ui.musicMute.sprite = config.musicMute ? ui.mutedSymbol : ui.unmutedSymbol;

            AudioManager.instance.SetMusicVolume(config.musicVolume, config.musicMute);
        } 

        else
        {
            ui.musicMute.sprite = config.musicMute ? ui.mutedSymbol : ui.unmutedSymbol; 
            SetMusicVolume(); 
        }
    }

    public void SetSFXMute()
    { 
        if (config.sfxMute = !config.sfxMute)
        { 
            //if (!VN_Configuration.activeConfig.PlayTextSound) 

            sfxText.text = "Muted";

            ui.sfxVolume.fillRect.GetComponent<Image>().color = config.sfxMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
            ui.sfxMute.sprite = config.sfxMute ? ui.mutedSymbol : ui.unmutedSymbol;

            AudioManager.instance.SetSFXVolume(config.sfxVolume, config.sfxMute); 
        } 

        else
        {
            ui.sfxMute.sprite = config.sfxMute ? ui.mutedSymbol : ui.unmutedSymbol;
            SetSFXVolume(); 
        }
    }

    public void SetVoicesMute()
    {
        if (config.voicesMute = !config.voicesMute)
        {
            voicesText.text = "Muted";

            ui.voicesVolume.fillRect.GetComponent<Image>().color = config.voicesMute ? UI_ITEMS.musicOffColor : UI_ITEMS.musicOnColor;
            ui.voicesMute.sprite = config.voicesMute ? ui.mutedSymbol : ui.unmutedSymbol;

            AudioManager.instance.SetVoicesVolume(config.voicesVolume, config.voicesMute);
        } 

        else
        {
            ui.voicesMute.sprite = config.voicesMute ? ui.mutedSymbol : ui.unmutedSymbol;
            SetVoicesVolume(); 
        }
    }

    public void SetColorBlindMode(bool enabled)
    {
        config.colorBlindModeEnabled = enabled;
        ui.SetButtonColors(ui.colorBlindOn, ui.colorBlindOff, enabled); 

        // Optional: Call an accessibility handler here
        // AccessibilityManager.instance.ApplyColorBlindMode(enabled);
    }

    public void SetMonoAudio(bool mono)
    {
        config.useMonoAudio = mono; 
        ui.SetButtonColors(ui.monoAudio, ui.stereoAudio, mono);

        // Optional: Apply audio output behavior
        AudioManager.instance?.SetMonoAudio(mono); 
    } 

    public void SetDialoguePrompt(bool defaultValue)
    {
        //this.defaultValue = defaultValue; 

        config.useDialoguePrompt = defaultValue; 
        ui.SetButtonColors(ui.dialoguePromtDefault, ui.dialoguePromtALT, defaultValue);

        if (defaultValue)
            DialogueContinuePrompt.activeDialoguePrompt.continuePromptOnLastCharacter = false;
        else
            DialogueContinuePrompt.activeDialoguePrompt.continuePromptOnLastCharacter = true; 
    } 

    public void SetSubstitlesInCutscenes(bool sub)
    {
        config.useSubtitles = sub;
        ui.SetButtonColors(ui.subtitlesOn, ui.subtitlesOff, sub); 
    }
}
