using COMMANDS;
using DIALOGUE;
using Reko.ColorPicker;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VNMenuManager : MonoBehaviour
{
    public static VNMenuManager instance;

    private MenuPage activePage = null;
    private bool isOpen = false;

    public bool developerMode = false; 

    [SerializeField] private CanvasGroup root;
    [SerializeField] private MenuPage[] pages;

    private CanvasGroupController rootCG;

    private UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.instance; 

    // Add this for displaying earnings
    public TextMeshProUGUI earningsText;

    public Button PhoneIcon;

    public Button autoButton;
    public Button logButton;
    public Button speakerIcon; 

    public Button PicstagramButton;
    public Button TownMapButton;
    public Button GalleryButton;
    public Button statsButton;
    public Button CalendarButton; 

    public Image weatherImage; // Image component for displaying the weather icon
    public Sprite sunnySprite;
    public Sprite cloudySprite;
    public Sprite sunnyCloudySprite;
    public Sprite rainySprite;
    public Sprite windySprite;
    public Sprite snowySprite;
    public Sprite foggySprite;
    public Sprite thunderstormSprite;

    public Sprite nightSprite; // Add this sprite via Inspector    

    public Animator loveResponseAnim;
    public Animator likeResponseAnim;
    public Animator straightResponseAnim;
    public Animator awkwardResponseAnim;
    public Animator moneyUpValueAnim;
    public Animator moneyDownValueAnim;

    public GameObject loveHeart;
    public GameObject likeFace;
    public GameObject straightFace;
    public GameObject awkwardFace;
    public GameObject moneyUpValue;
    public GameObject moneyDownValue;
    public GameObject infoDisplayBox;
    public GameObject statRequirementBox;
    public GameObject darkenPanel;

    public Animator statIncreaseAnimator; // Reference to Animator 
    public GameObject statIncreaseGameObject; // Pre-existing animated GameObject 
    public Animator statRequirementMetAnimator;
    public GameObject statRequirementMetGameObject;
    public Animator statRequirementImageAnimator;
    public GameObject statRequirementImageGameObject; 
    public TextMeshProUGUI statIncreaseText; // Text component to update stat name
    public TextMeshProUGUI statRequirementMetText; // Text component to update stat name
    public TextMeshProUGUI moneyAddedText; // Text component to update stat name  
    public TextMeshProUGUI moneyDeductedText; // Text component to update stat name 

    public Animator MCAnimator; // Reference to Animator 
    public GameObject MCGameObject; // Pre-existing animated GameObject
    public Image MCImage;
    public Sprite mcGuySprite;
    public Sprite mcGirlSprite;
    public Sprite mcNonBinarySprite;

    public TextMeshProUGUI infoDisplayText; // Text component to update stat name

    public TextMeshProUGUI statRequirementText;

    private float earnings;

    public Texture2D cursorImage;

    private Vector2 cursorHotspot;

    private CanvasGroup infoDisplayCanvasGroup; // CanvasGroup for fading
    private Image darkenPanelImage; // Reference to Image for darkenPanel opacity 

    private CanvasGroup statRequirementCanvasGroup;

    public GameObject fakeCalendarPanel;

    private bool isSaveAndLoadMenuOpen = false;

    //private bool meiHangOutTwoCom;

    private AudioSource menuButtonSound;

    private AudioSource menuClickSound;

    private AudioSource messagingClickSound;

    private AudioSource phoneClickSound; 

    public Slider karmaMeter;
    public CanvasGroup karmaMeterCanvasGroup; // CanvasGroup for the Karma Meter 
    public Image karmaHandle; // Reference to the Karma Meter's Handle

    public Sprite lowKarmaSprite;
    public Sprite neutralKarmaSprite;
    public Sprite highKarmaSprite;

    public Sprite charismaSprite;
    public Sprite creativitySprite; 
    public Sprite empathySprite;
    public Sprite humorSprite;
    public Sprite enduranceSprite;
    public Sprite patienceSprite; 
    public Sprite intelligenceSprite;
    public Sprite courageSprite;
    public Sprite kindnessSprite;
    public Sprite confidenceSprite;

    [Header("Color Customization")]
    public Color defaultNameColor = Color.white; // Or any default you want 
    public ColorPicker colorPicker; // Assign this in Inspector

    public bool checkedPhone = false;

    public bool meiFirstTime = false;
    public bool alexFirstTime = false;
    public bool ninaFirstTime = false;
    public bool simonFirstTime = false; 

    //private Coroutine karmaFadeCoroutine;

    /*public Image bodyRenderer;
    public Image hairRenderer;
    public Image eyesRenderer;
    public Image highlightsRenderer;
    public Image eyebrowsRenderer;
    public Image noseRenderer;
    public Image mouthRenderer;
    public Image frecklesRenderer;
    public Image facialRenderer;
    public Image glassesRenderer;
    public Image makeupRenderer;
    public Image scarsRenderer;
    public Image tattoosRenderer;
    public Image earsRenderer;
    public Image tailsRenderer;
    public Image hornsRenderer;
    public Image clothesRenderer;
    public Image hatsRenderer;
    public Image accessoriesRenderer;
    public Image skinToneRenderer;*/

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (loveHeart == null)
            return;
        if (likeFace == null)
            return;
        if (straightFace == null)
            return;
        if (awkwardFace == null)
            return;
        if (infoDisplayBox == null)
            return;
        if (statRequirementBox == null)
            return;

        if (colorPicker != null)
        {
            colorPicker.OnColorChanged.AddListener(OnFirstNameColorChanged);
        }
        else
        {
            Debug.LogWarning("ColorPicker reference not assigned in VNMenuManager.");
        }

        colorPicker.gameObject.SetActive(false); 

        loveHeart.SetActive(false);
        likeFace.SetActive(false);
        straightFace.SetActive(false);
        awkwardFace.SetActive(false);
        infoDisplayBox.SetActive(false);
        statRequirementBox.SetActive(false);

        rootCG = new CanvasGroupController(this, root);

        if (infoDisplayBox == null)
            return;
        if (statRequirementBox == null)
            return;

        infoDisplayCanvasGroup = infoDisplayBox.GetComponent<CanvasGroup>();
        statRequirementCanvasGroup = statRequirementBox.GetComponent<CanvasGroup>();
        infoDisplayCanvasGroup.alpha = 0;
        statRequirementCanvasGroup.alpha = 0;

        darkenPanelImage = darkenPanel.GetComponent<Image>();
        SetImageAlpha(darkenPanelImage, 0f); // Initially set darkenPanel to transparent

        rootCG.alpha = 0;



        // Check if we are coming from the TownMap scene
        if (PlayerPrefs.GetString("LastScene", "") == "TownMap")
        // Use VariableStore to check LastScene
        //VariableStore.TryGetValue("Scene.LastScene", out object lastScene);
        //if ((string)lastScene == "TownMap")
        {
            // Ensure the rootCG is immediately active and visible
            rootCG.alpha = 1;
            rootCG.SetInteractableState(true);
            isOpen = true;

            // Open the PhoneHome page immediately
            OpenPhoneHomePage();

            PlayerPrefs.DeleteKey("LastScene");
            PlayerPrefs.Save();
            //VariableStore.RemoveVariable("Scene.LastScene"); 
        }
        else
        {
            // Hide the rootCG if starting the game in the VisualNovel scene
            rootCG.alpha = 0;
            rootCG.SetInteractableState(false);
            isOpen = false;

            PlayerPrefs.DeleteKey("LastScene");
            PlayerPrefs.Save();
            //VariableStore.RemoveVariable("Scene.LastScene"); 
        }

        UpdateWeatherIcon(); // Update the weather icon when the TownMap scene is loaded 

        // Retrieve and display the earnings in the Visual Novel scene
        //earnings = PlayerPrefs.GetFloat("TotalMoney", 1000);

        earnings = PlayerStats.GetMoney();

        UpdateEarningsText(earnings);

        cursorHotspot = new Vector2(cursorImage.width / 4, cursorImage.height / 4);
        Cursor.SetCursor(cursorImage, cursorHotspot, CursorMode.Auto);

        menuButtonSound = GameObject.Find("MenuButtonSound").GetComponent<AudioSource>();

        menuClickSound = GameObject.Find("MenuClickSound").GetComponent<AudioSource>();

        messagingClickSound = GameObject.Find("MessagingClickSound").GetComponent<AudioSource>();

        phoneClickSound = GameObject.Find("PhoneClickSound").GetComponent<AudioSource>(); 

        UpdateMCImage(PlayerPrefs.GetString("SelectedPronouns", "HE/HIM"));
        //LoadCustomization(); // Load the saved customization for the MC

        karmaMeter.gameObject.SetActive(false);

        karmaMeterCanvasGroup.alpha = 0; // Initially hide the Karma Meter  

        UpdateKarmaMeter();

        UpdateEarningsText(PlayerStats.GetMoney());

        VariableStore.CreateVariable("PlayerCheckedPhone", checkedPhone); 
    } 

    public void UpdateMCImage(string pronouns)
    {
        if (MCImage == null) return;

        switch (pronouns)
        {
            case "HE/HIM":
                MCImage.sprite = mcGuySprite;
                break;
            case "SHE/HER":
                MCImage.sprite = mcGirlSprite;
                break;
            case "THEY/THEM":
                MCImage.sprite = mcNonBinarySprite;
                break;
        }

        PlayerPrefs.SetString("SelectedPronouns", pronouns);
        PlayerPrefs.Save();
    }

    /*private void LoadCustomization()
    {
        ApplyCustomization("BODY", bodyRenderer);
        ApplyCustomization("HAIR", hairRenderer);
        ApplyCustomization("EYES", eyesRenderer);
        ApplyCustomization("HIGHLIGHTS", highlightsRenderer);
        ApplyCustomization("EYEBROWS", eyebrowsRenderer);
        ApplyCustomization("NOSE", noseRenderer);
        ApplyCustomization("MOUTH", mouthRenderer);
        ApplyCustomization("FRECKLES", frecklesRenderer);
        ApplyCustomization("FACIAL", facialRenderer);
        ApplyCustomization("GLASSES", glassesRenderer);
        ApplyCustomization("MAKEUP", makeupRenderer);
        ApplyCustomization("SCARS", scarsRenderer);
        ApplyCustomization("TATTOOS", tattoosRenderer);
        ApplyCustomization("EARS", earsRenderer);
        ApplyCustomization("TAILS", tailsRenderer);
        ApplyCustomization("HORNS", hornsRenderer);
        ApplyCustomization("CLOTHES", clothesRenderer);
        ApplyCustomization("HATS", hatsRenderer);
        ApplyCustomization("ACCESSORIES", accessoriesRenderer);
        ApplyCustomization("SKINTONE", skinToneRenderer);
    }*/

    /*private void ApplyCustomization(string category, Image renderer)
    {
        int index = PlayerPrefs.GetInt(category, 0);
        string spritePath = PlayerPrefs.GetString(category + "_Sprite", "");

        if (!string.IsNullOrEmpty(spritePath))
        {
            Sprite savedSprite = Resources.Load<Sprite>(spritePath);
            if (savedSprite != null)
            {
                renderer.sprite = savedSprite;
            }
        }
    }*/

    // Method to update the earnings display
    public void UpdateEarningsText(float earnings)
    {
        if (earningsText != null)
        {
            earningsText.text = $"${earnings:N2}";
        }
    }

    public void PlayMoneyUpValue(string animation)
    {
        if (animation == "MoneyAdded")
        {
            moneyUpValue.SetActive(true);
            moneyUpValueAnim.SetTrigger(animation);
            StartCoroutine(DisableAfterAnimation(moneyUpValueAnim, moneyUpValue));
        }
    }

    public void PlayMoneyDownValue(string animation)
    {
        if (animation == "MoneyDeducted")
        {
            moneyDownValue.SetActive(true);
            moneyDownValueAnim.SetTrigger(animation);
            StartCoroutine(DisableAfterAnimation(moneyDownValueAnim, moneyDownValue));
        }
    }

    public void DisplayTipUI(string tip)
    {
        infoDisplayBox.SetActive(true);
        infoDisplayText.text = tip;

        StartCoroutine(FadeCanvasGroup(infoDisplayCanvasGroup, 0f, 1f, 1f)); // Fade in over 0.5 seconds
        StartCoroutine(FadeImageAlpha(darkenPanelImage, 0f, 0.6f, 0.5f)); // Fade darkenPanel to 60% opacity  
    }

    public void StatRequirementUI(string requirement)
    {
        statRequirementBox.SetActive(true);
        statRequirementText.text = requirement;

        StartCoroutine(FadeCanvasGroup(statRequirementCanvasGroup, 0f, 1f, 1f)); // Fade in over 0.5 seconds
        //StartCoroutine(FadeImageAlpha(darkenPanelImage, 0f, 0.6f, 0.5f)); // Fade darkenPanel to 60% opacity
    }

    public void HideTipUI()
    {
        StartCoroutine(FadeOutAndDisable(infoDisplayCanvasGroup, 1f, 0.5f)); // Fade out over 0.5 seconds 
        StartCoroutine(FadeImageAlpha(darkenPanelImage, 0.5f, 0f, 0.5f)); // Fade darkenPanel back to transparent 
    }

    public void HideRequirementUI()
    {
        StartCoroutine(FadeOutAndDisable(statRequirementCanvasGroup, 1f, 0.5f)); // Fade out over 0.5 seconds 
        //StartCoroutine(FadeImageAlpha(darkenPanelImage, 0.5f, 0f, 0.5f)); // Fade darkenPanel back to transparent
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = end;
    }

    private IEnumerator FadeImageAlpha(Image image, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = image.color;

        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            image.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }
        color.a = endAlpha;
        image.color = color;
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    private IEnumerator FadeOutAndDisable(CanvasGroup canvasGroup, float startAlpha, float duration)
    {
        yield return FadeCanvasGroup(canvasGroup, startAlpha, 0f, duration);
        infoDisplayBox.SetActive(false); // Disable after fade-out completes
        infoDisplayText.text = null;
        statRequirementBox.SetActive(false);
        statRequirementText.text = null;
    }

    public void PlayCharacterResponse(string animation)
    {
        if (animation == "LovedResponse")
        {
            loveHeart.SetActive(true);
            loveResponseAnim.SetTrigger(animation);
            StartCoroutine(DisableAfterAnimation(loveResponseAnim, loveHeart));
        }

        if (animation == "LikedResponse")
        {
            likeFace.SetActive(true);
            likeResponseAnim.SetTrigger(animation);
            StartCoroutine(DisableAfterAnimation(likeResponseAnim, likeFace));
        }

        if (animation == "StraightResponse")
        {
            straightFace.SetActive(true);
            straightResponseAnim.SetTrigger(animation);
            StartCoroutine(DisableAfterAnimation(straightResponseAnim, straightFace));
        }

        if (animation == "AwkwardResponse")
        {
            awkwardFace.SetActive(true);
            awkwardResponseAnim.SetTrigger(animation);
            StartCoroutine(DisableAfterAnimation(awkwardResponseAnim, awkwardFace));
        }
    }

    public void PlayStatIncreaseAnimation(string statName, int addedPoints)
    { 
        // Set the stat name on the UI text
        statIncreaseText.text = "\u00A0" + $" {statName} + {addedPoints}!";

        // Enable the GameObject before playing the animation
        statIncreaseGameObject.SetActive(true);

        // Set trigger for the Animator to play the animation
        statIncreaseAnimator.SetTrigger("DisplayStat");

        // Start a coroutine to disable the GameObject after the animation ends
        StartCoroutine(DisableStatAnimation());
    }

    public void PlayFriendIncreaseAnimation(string friendName, int addedPoints)
    {
        if (meiFirstTime)
            friendName = "Familar Person"; 

        if (alexFirstTime)
            friendName = "Pretty Girl"; 

        if (ninaFirstTime)
            friendName = "Young Woman"; 

        if (simonFirstTime)
            friendName = "Shy Person"; 

        // Set the stat name on the UI text
        statIncreaseText.text = "\u00A0" + $" {friendName} + {addedPoints}!";

        // Enable the GameObject before playing the animation
        statIncreaseGameObject.SetActive(true);

        // Set trigger for the Animator to play the animation
        statIncreaseAnimator.SetTrigger("DisplayStat");

        // Start a coroutine to disable the GameObject after the animation ends
        StartCoroutine(DisableStatAnimation());
    }

    public void PlayStatRequirementMetAnimation(string statMet)
    {
        statRequirementMetText.text = "\u00A0" + $" {statMet} - Special Dialogue Unlocked!";

        // Change sprite based on statMet
        Image statImage = statRequirementImageGameObject.GetComponent<Image>();
        if (statImage != null)
        {
            switch (statMet.ToLower())
            {
                case "charisma":
                    statImage.sprite = charismaSprite;
                    break;
                case "creativity":
                    statImage.sprite = creativitySprite;
                    break; 
                case "empathy":
                    statImage.sprite = empathySprite;
                    break;
                case "humor":
                    statImage.sprite = humorSprite;
                    break;
                case "endurance":
                    statImage.sprite = enduranceSprite;
                    break;
                case "patience":
                    statImage.sprite = patienceSprite;
                    break; 
                case "intelligence":
                    statImage.sprite = intelligenceSprite;
                    break;
                case "courage":
                    statImage.sprite = courageSprite;
                    break;
                case "kindness":
                    statImage.sprite = kindnessSprite;
                    break;
                case "confidence":
                    statImage.sprite = confidenceSprite; 
                    break; 
                // Add more cases for other stats
                default:
                    statImage.sprite = null; // Or assign a default sprite
                    break;
            }
        }

        statRequirementMetGameObject.SetActive(true);
        statRequirementImageGameObject.SetActive(true);

        statRequirementMetAnimator.SetTrigger("DisplayStatRequirementMet");
        statRequirementImageAnimator.SetTrigger("ImageStatRequirementMet");

        StartCoroutine(DisableStatRequirementAnimation());
    } 

    // Coroutine to disable the GameObject after the animation ends
    private IEnumerator DisableAfterAnimation(Animator animator, GameObject obj)
    {
        // Wait until the animation has finished playing
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(0.25f);
        obj.SetActive(false); // Disable the GameObject
    }

    private IEnumerator DisableStatAnimation()
    {
        // Wait until the animation has finished playing
        yield return new WaitForSeconds(statIncreaseAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Optionally, add a small delay before disabling
        yield return new WaitForSeconds(0.5f);

        // Disable the GameObject after the animation completes
        statIncreaseGameObject.SetActive(false);
    }

    private IEnumerator DisableStatRequirementAnimation()
    {
        // Wait until the animation has finished playing
        yield return new WaitForSeconds(statRequirementMetAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Wait until the animation has finished playing
        yield return new WaitForSeconds(statRequirementImageAnimator.GetCurrentAnimatorStateInfo(0).length);


        // Optionally, add a small delay before disabling
        yield return new WaitForSeconds(0.5f);

        // Disable the GameObject after the animation completes
        statRequirementMetGameObject.SetActive(false);
        statRequirementImageGameObject.SetActive(false); 

    }

    public void PlayMCAnimation()
    {
        // Enable the GameObject before playing the animation
        MCGameObject.SetActive(true);

        // Set trigger for the Animator to play the animation
        MCAnimator.SetTrigger("DisplayMC");
    }

    public void HideMC()
    {
        MCAnimator.SetTrigger("SlideAwayMC");

        /*Image mcImage = MCGameObject.GetComponent<Image>();

        if (mcImage != null)
        {
            StartCoroutine(FadeOutAndHide(mcImage)); 
        }*/

        //MCGameObject.SetActive(false); 
    }

    private IEnumerator FadeOutAndHide(Image image)
    {
        float duration = 0.5f; // Fade-out duration  
        float elapsedTime = 0f;
        Color startColor = image.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < duration)
        {
            image.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        image.color = endColor;
        MCGameObject.SetActive(false);
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            rootCG.Hide();
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            rootCG.Show();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            rootCG.alpha = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            rootCG.alpha = 1;

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!developerMode)
                return; 

            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save(); // Ensure changes are written immediately

            ResetPlayerNameColor(); 
        }

        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //PlayerPrefs.DeleteKey("LastScene");
        //PlayerPrefs.Save(); 
        //}
    }

    /*public void HandleDayAdvance()
    {
        // Set rootCG to 0, make it non-interactable, and ensure it is not opened
        rootCG.alpha = 0;
        rootCG.SetInteractableState(false);
        isOpen = false;
    } */

    public void UpdateWeatherIcon()
    {
        // If it's Evening, override with Night Sprite
        if (CalendarSystem.Instance != null && CalendarSystem.Instance.activityCounter == 2 && nightSprite != null)
        {
            weatherImage.sprite = nightSprite;
            return;
        } 

        CalendarSystem.Weather currentWeather = CalendarSystem.Instance.GetCurrentWeather();

        switch (currentWeather)
        {
            case CalendarSystem.Weather.Sunny:
                weatherImage.sprite = sunnySprite;
                break;
            case CalendarSystem.Weather.Cloudy:
                weatherImage.sprite = cloudySprite;
                break;
            case CalendarSystem.Weather.SunnyAndCloudy:
                weatherImage.sprite = sunnyCloudySprite;
                break;
            case CalendarSystem.Weather.Rainy:
                weatherImage.sprite = rainySprite;
                break;
            case CalendarSystem.Weather.Windy:
                weatherImage.sprite = sunnySprite;
                break;
            case CalendarSystem.Weather.Snowy:
                weatherImage.sprite = cloudySprite;
                break;
            case CalendarSystem.Weather.Foggy:
                weatherImage.sprite = foggySprite;
                break;
            case CalendarSystem.Weather.Thunderstorm:
                weatherImage.sprite = thunderstormSprite;
                break;
        }
    }

    public void AddMoney(float amount)
    {
        earnings += amount;
        //PlayerPrefs.SetFloat("TotalMoney", earnings);
        //PlayerPrefs.Save();

        VariableStore.TrySetValue("PlayerStats.Money", earnings);

        UpdateEarningsText(earnings);  // Update the display in the VN scene
    }

    public void DeductMoney(float amount)
    {
        earnings -= amount;
        if (earnings < 0)
        {
            earnings = 0;  // Prevent negative earnings
        }

        PlayerStats.SetMoney(earnings);

        UpdateEarningsText(earnings);  // Update the display in the VN scene 
    }

    public void PlayMessagesSound()
    {
        messagingClickSound.PlayOneShot(messagingClickSound.clip);
    } 

    public void PlayPhoneSound()
    {
        phoneClickSound.PlayOneShot(phoneClickSound.clip); 
    }

    public void PhoneButtonInteractive(bool phoneEnabled)
    {
        if (!phoneEnabled && CalendarSystem.Instance.currentSeason == CalendarSystem.Season.Spring && CalendarSystem.Instance.currentDay == 15)
        {
            PicstagramButton.interactable = false;
            TownMapButton.interactable = false;
            statsButton.interactable = false; 
            //GalleryButton.interactable = false;
            //CalendarButton.interactable = false; 
        }

        else if (!phoneEnabled)
        {
            PicstagramButton.interactable = false;
            TownMapButton.interactable = false;
            //GalleryButton.interactable = false;
            CalendarButton.interactable = false;
        }

        else
        {
            PicstagramButton.interactable = true;
            TownMapButton.interactable = true;
            //GalleryButton.interactable = true; 
            CalendarButton.interactable = true;
        }
        //PhoneIcon.interactable = true;   

    }

    private void HandleTransition(string locationFile)
    {
        // Set the starting file based on the location
        PlayerPrefs.SetString("StartingFile", locationFile);
        PlayerPrefs.Save();
        //VariableStore.TrySetValue("Scene.LastScene", "TownMap");
        //VariableStore.TrySetValue("Scene.StartingFile", locationFile); 

        // Transfer to the VisualNovel scene
        SceneManager.LoadScene("VisualNovel");
    }

    private void OnFirstNameColorChanged(Color selectedColor)
    {
        if (DialogueSystem.instance != null)
        {
            DialogueSystem.instance.SetCharacterNameColor("FirstName", selectedColor);
        }
        if (InputPanel.instance != null)
        {
            InputPanel.instance.SetTitleTextColor(selectedColor); // Update title text + Save
        } 
    } 

    //public void MeiHangOutTwo(bool hangOutComplete)
    //{
    //DirectMessageManager.Instance.alreadyAcceptedHangOutTwo = hangOutComplete;
    //meiHangOutTwoCom = hangOutComplete; 
    //}

    private MenuPage GetPage(MenuPage.PageType pageType)
    {
        return pages.FirstOrDefault(page => page.pageType == pageType);
    }

    public void OpenSavePage()
    {
        //menuButtonSound.Play();

        //if (!isOpen)
        //{
        //OpenRoot(); // Ensure the root opens if it's closed
        //} 

        //menuClickSound.PlayOneShot(menuClickSound.clip); 

        //StartCoroutine(PlaySoundAndSaveLoad()); 

        SaveAndLoadMenu.Instance.titleText.text = "Save Data";

        var page = GetPage(MenuPage.PageType.SaveAndLoad);
        var slm = page.anim.GetComponentInParent<SaveAndLoadMenu>();

        if (isSaveAndLoadMenuOpen && slm.menuFunction == SaveAndLoadMenu.MenuFunction.save)
            return; // Already in Save mode, no need to animate 

        slm.menuFunction = SaveAndLoadMenu.MenuFunction.save;
        isSaveAndLoadMenuOpen = true;
        OpenPage(page);
    }

    public void OpenLoadPage()
    {
        //menuButtonSound.Play();

        //menuClickSound.PlayOneShot(menuClickSound.clip); 

        SaveAndLoadMenu.Instance.titleText.text = "Load Data";

        var page = GetPage(MenuPage.PageType.SaveAndLoad);
        var slm = page.anim.GetComponentInParent<SaveAndLoadMenu>();

        if (isSaveAndLoadMenuOpen && slm.menuFunction == SaveAndLoadMenu.MenuFunction.load)
            return; // Already in Load mode, no need to animate 

        slm.menuFunction = SaveAndLoadMenu.MenuFunction.load;
        isSaveAndLoadMenuOpen = true;
        OpenPage(page);
    }

    public void OpenConfigPage()
    {
        //menuButtonSound.Play();
        menuButtonSound.PlayOneShot(menuButtonSound.clip);
        var page = GetPage(MenuPage.PageType.Config);
        OpenPage(page);
    }

    public void OpenBuzzStopPage()
    {
        //menuButtonSound.Play();
        menuButtonSound.PlayOneShot(menuButtonSound.clip);
        var page = GetPage(MenuPage.PageType.BuzzStop);
        OpenPage(page);
    }

    public void OpenKudosPage()
    {
        //menuButtonSound.Play();
        menuButtonSound.PlayOneShot(menuButtonSound.clip);
        var page = GetPage(MenuPage.PageType.Kudos);
        OpenPage(page);
    }

    public void OpenPlanGeniusPage()
    {
        //menuButtonSound.Play();
        menuButtonSound.PlayOneShot(menuButtonSound.clip);
        var page = GetPage(MenuPage.PageType.PlanGenius);
        OpenPage(page);
    }

    public void OpenNotesPage()
    {
        //menuButtonSound.Play();
        menuButtonSound.PlayOneShot(menuButtonSound.clip);
        var page = GetPage(MenuPage.PageType.Notes);
        OpenPage(page);
    }

    public void OpenStatsPage()
    {
        //menuButtonSound.Play();
        menuButtonSound.PlayOneShot(menuButtonSound.clip);
        var page = GetPage(MenuPage.PageType.Stats);
        OpenPage(page);
    }

    public void OpenCalendarPage()
    {
        //menuButtonSound.Play();
        menuButtonSound.PlayOneShot(menuButtonSound.clip);
        var page = GetPage(MenuPage.PageType.Calendar);
        CalendarMenu.instance.OpenCalendarMenu(); // Ensure it resets to the current season 
        OpenPage(page);
    }

    public void OpenMapPage()
    {
        StartCoroutine(PlaySoundAndTransition());
    }

    public void OpenPhoneHomePage()
    {
        //menuButtonSound.Play();
        //menuButtonSound.PlayOneShot(menuButtonSound.clip);
        var page = GetPage(MenuPage.PageType.PhoneHome);
        OpenPage(page);
    }

    public void OpenHelpPage()
    {
        //menuButtonSound.Play(); 
        menuButtonSound.PlayOneShot(menuButtonSound.clip);

        var page = GetPage(MenuPage.PageType.Help);
        OpenPage(page);
    }

    public void PlayMenuSoundEffect()
    {
        menuButtonSound.PlayOneShot(menuButtonSound.clip);
    }

    public void PlayMenuClickEffect()
    {
        menuClickSound.PlayOneShot(menuClickSound.clip);
    } 

    private void OpenPage(MenuPage page)
    {
        //menuButtonSound.Play();

        //menuButtonSound.PlayOneShot(menuButtonSound.clip); 

        if (page == null)
            return;

        if (activePage != null && activePage != page)
            activePage.Close();

        page.Open();
        activePage = page;

        if (!isOpen)
            OpenRoot();
    }

    public void OpenRoot()
    {
        rootCG.Show();
        rootCG.SetInteractableState(true);
        isOpen = true;
    }

    public void CloseRoot()
    {
        //if (CalendarSystem.Instance.currentSeason == CalendarSystem.Season.Spring && CalendarSystem.Instance.currentDay == 15)
        //{
        //CalendarSystem.Instance.activityCounter = 2;
        //HandleTransition("ApartmentStart");  
        //CalendarSystem.Instance.AdvanceActivityOrDay(); 
        //} 

        rootCG.Hide();
        rootCG.SetInteractableState(false);
        isOpen = false;
        isSaveAndLoadMenuOpen = false; // Reset the flag when the menu is closed 
        //checkedPhone = true; 

        if (CalendarSystem.Instance.currentSeason == CalendarSystem.Season.Spring && CalendarSystem.Instance.currentDay == 15)
        {
            var cmdGeneral = new CMD_DatabaseExtension_General();
            cmdGeneral.LoadDialogueFile("WelcomeToBaywoodPARTTwo"); 
            //checkedPhone = true; 
            //PlayerPrefs.SetString("StartingFile", "WelcomeToBaywood"); 
            //PlayerPrefs.Save(); 

            //SceneManager.LoadScene("VisualNovel"); 

            //Debug.Log("Checking your phone."); 
            //PicstagramButton.interactable = false;
            //TownMapButton.interactable = false;
            //statsButton.interactable = false;
            //GalleryButton.interactable = false;
            //CalendarButton.interactable = false; 
        }
    }

    public void CloseSaveAndLoadMenu()
    {
        var page = GetPage(MenuPage.PageType.SaveAndLoad);
        if (page != null)
        {
            page.Close(); // Close the Save/Load menu
            isSaveAndLoadMenuOpen = false; // Reset flag
            Debug.Log("Save/Load Menu Closed. isSaveAndLoadMenuOpen set to false.");
        }
    }

    public void OnClick_CloseSaveLoadMenu()
    {
        //CloseSaveAndLoadMenu();
        isSaveAndLoadMenuOpen = false; // Reset flag 
    }

    public void Click_Home()
    {
        StartCoroutine(PlaySoundAndMenu());

        // Reset the LastScene PlayerPrefs when going back to the main menu
        /*PlayerPrefs.DeleteKey("LastScene"); 
        PlayerPrefs.Save();

        VN_Configuration.activeConfig.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene(MainMenu.MAIN_MENU_SCENE);*/
    }

    public void Click_Quit()
    {
        // Reset the LastScene PlayerPrefs when quitting the game
        PlayerPrefs.DeleteKey("LastScene");
        PlayerPrefs.Save();

        // Reset the LastScene variable when going back to the main menu
        //VariableStore.RemoveVariable("Default.LastScene"); 

        uiChoiceMenu.Show("Quit to desktop?", new UIConfirmationMenu.ConfirmationButton("Yes", () => Application.Quit()), new UIConfirmationMenu.ConfirmationButton("No", null));
    }

    private IEnumerator PlaySoundAndTransition()
    {
        if (menuButtonSound != null && menuButtonSound.clip != null)
        {
            menuButtonSound.Play(); // Play the sound effect
            yield return new WaitForSeconds(menuButtonSound.clip.length); // Wait for the sound to finish
        }

        SceneManager.LoadScene("TownMap"); // Transition to the Map Menu
    }

    private IEnumerator PlaySoundAndMenu()
    {
        if (menuButtonSound != null && menuButtonSound.clip != null)
        {
            menuButtonSound.Play(); // Play the sound effect
            yield return new WaitForSeconds(menuButtonSound.clip.length); // Wait for the sound to finish
        }

        // Reset the LastScene PlayerPrefs when going back to the main menu
        PlayerPrefs.DeleteKey("LastScene");
        PlayerPrefs.Save();

        // Reset the LastScene variable when going back to the main menu
        //VariableStore.RemoveVariable("Default.LastScene"); 

        VN_Configuration.activeConfig.Save();
        //UnityEngine.SceneManagement.SceneManager.LoadScene(MainMenu.MAIN_MENU_SCENE);

        SceneManager.LoadScene("Main Menu"); // Transition to the Main Menu  
    }

    public void PlaySoundEffect()
    {
        menuButtonSound.Play(); // Play the sound effect 
    }

    private IEnumerator PlaySoundAndSaveLoad()
    {
        if (menuButtonSound != null && menuButtonSound.clip != null)
        {
            menuButtonSound.Play(); // Play the sound effect
            yield return new WaitForSeconds(menuButtonSound.clip.length); // Wait for the sound to finish
        }

        //SceneManager.LoadScene("TownMap"); // Transition to the Map Menu
    }

    public void UpdateKarmaMeter()
    {
        // Set Karma Slider value based on current Karma Points (0 - 300)
        karmaMeter.value = PlayerStats.instance.Karma.Points;

        Debug.Log($"Karma Slider Updated: {karmaMeter.value}");

        ChangeKarmaHandle(); // Update handle appearance 

        // Show and fade out Karma Meter
        //if (karmaFadeCoroutine != null)
        //{
        //StopCoroutine(karmaFadeCoroutine);
        //}

        /*karmaFadeCoroutine = */
        StartCoroutine(ShowAndFadeOutKarmaMeter());
    }

    private void ChangeKarmaHandle()
    {
        if (karmaHandle == null) return;

        if (karmaMeter.value <= 40)
        {
            karmaHandle.sprite = lowKarmaSprite;
        }
        else if (karmaMeter.value > 40)
        {
            karmaHandle.sprite = neutralKarmaSprite;
        }
        else if (karmaMeter.value >= 60)
        {
            karmaHandle.sprite = highKarmaSprite;
        }
    }

    /*public void FadeInPanel()
    {
        // Instantly set the panel to fully visible
        SetImageAlpha(darkenPanelImage, 1f);

        // Start fading out after a short delay
        StartCoroutine(FadeOutDarkenPanelAfterDelay());
    }

    private IEnumerator FadeOutDarkenPanelAfterDelay()
    {
        // Wait while it's fully visible
        yield return new WaitForSeconds(5f);

        // Then fade out to transparent
        yield return FadeImageAlpha(darkenPanelImage, 1f, 0f, 0.5f);
    }*/

    private IEnumerator ShowAndFadeOutKarmaMeter()
    {
        // Fade In
        yield return FadeCanvasGroup(karmaMeterCanvasGroup, 0f, 1f, 0.5f);

        // Wait for a couple of seconds
        yield return new WaitForSeconds(2f);

        // Fade Out
        yield return FadeCanvasGroup(karmaMeterCanvasGroup, 1f, 0f, 0.5f);

        karmaMeter.gameObject.SetActive(false);
    } 

    public void CloseColorPicker()
    {
        colorPicker.OnColorChanged.AddListener(InputPanel.instance.SetTitleTextColor); 
        colorPicker.gameObject.SetActive(false); 
    }

    private void ResetPlayerNameColor()
    {
        if (DialogueSystem.instance != null)
        {
            DialogueSystem.instance.SetCharacterNameColor("FirstName", defaultNameColor);
        }

        if (InputPanel.instance != null)
        {
            InputPanel.instance.SetTitleTextColor(defaultNameColor);
        }

        if (colorPicker != null)
        {
            //colorPicker.CurrentColor = defaultNameColor;
            colorPicker.SelectedColor = defaultNameColor; 
        }

        PlayerPrefs.DeleteKey("NameColor_FirstName");
        PlayerPrefs.Save();

        Debug.Log("[VNMenuManager] Reset Player Name Color to default.");
    } 

}