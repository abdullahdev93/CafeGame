using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DIALOGUE;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using DIALOGUE.LogicalLines;

public class ChoicePanel : MonoBehaviour
{
    public static ChoicePanel instance { get; private set; }

    private AutoReader skipReader => AutoReader.reader; 

    //private bool skipping => skipReader.skip; 

    private const float BUTTON_MIN_WIDTH = 50;
    private const float BUTTON_MAX_WIDTH = 1000;
    private const float BUTTON_WIDTH_PADDING = 25;

    private const float BUTTON_HEIGHT_PER_LINE = 50;
    private const float BUTTON_HEIGHT_PADDING = 20;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private VerticalLayoutGroup buttonLayoutGroup;

    //[SerializeField] private Slider choiceTimerSlider;       // Drag your Slider here
    //[SerializeField] private GameObject timerContainer;      // (optional) a parent panel for the slider to show/hide
    [SerializeField, Min(0.1f)] private float choiceTimeoutSeconds = 30f; 

    // --- NEW: Double-edge Image timer fields ---
    [SerializeField] private bool useDualEdgeTimer = true;
    [SerializeField] private RectTransform dualTimerContainer; // parent panel/area for the timer
    [SerializeField] private RectTransform dualTimerFill;      // the fill bar that shrinks from both edges
                                                               // (Optional) If you want to color/enable/disable via Image:
    [SerializeField] private Image dualTimerFillImage;

    // --- Timer color + pulse tuning ---
    [SerializeField] private Color timerStartColor = Color.green;
    [SerializeField] private Color timerEndColor = Color.red;
    [SerializeField, Range(0f, 1f)] private float choiceMeterColorThreshold = 0.2f; // rename to colorChangeThreshold if you prefer

    // --- Color timing ---
    [SerializeField, Range(0f, 1f)] private float colorChangeThreshold = 0.20f; // below this: full red
    [SerializeField, Range(0f, 1f)] private float preBlendWindow = 0.08f;  // how far BEFORE threshold to blend

    // --- Colors ---
    [SerializeField] private Color timerSafeColor = Color.white; // most of the time
    [SerializeField] private Color timerBlendStart = Color.green; // start of the pre-red blend
    [SerializeField] private Color timerDangerColor = Color.red;  // final color 

    // ===== Timer Easing (for bar shrink speed) =====
    [SerializeField] private bool easeShrink = true;

    public enum EasingMode { Linear, EaseIn, EaseOut, EaseInOut, SmoothStep, Custom }
    [SerializeField] private EasingMode easingMode = EasingMode.SmoothStep;

    // Customize the curve if you pick Custom (time 0..1 -> value 0..1)
    [SerializeField] private AnimationCurve customEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f); 

    // Master toggle for the choice timer
    [SerializeField] private bool choiceTimerEnabled = true; 

    // When remaining fraction is below this, start pulsing
    //[SerializeField, Range(0f, 1f)] private float pulseThreshold = 0.2f;

    // How fast the pulse animates
    //[SerializeField, Min(0.1f)] private float pulseSpeed = 6f;

    // How strong the pulse is (applied to alpha + bar thickness)
    //[SerializeField, Range(0f, 1.0f)] private float pulseAmount = 0.08f; 

    private Coroutine timerRoutine;
    private List<bool> cachedLockStates = new List<bool>();  // cache locks for fallback on timeout

    [SerializeField] private CanvasGroup choiceTimerGroup;     // CanvasGroup on the slider (or its parent)
    [SerializeField, Min(0.05f)] private float timerFadeDuration = 0.3f; 

    //private Coroutine timerRoutine;
    private Coroutine timerFadeRoutine;

    private CanvasGroupController cg = null;
    private List<ChoiceButton> buttons = new List<ChoiceButton>();
    public ChoicePanelDecision lastDecision { get; private set; } = null;

    public bool isWaitingOnUserChoice { get; private set; } = false;

    public Button theSkipButton;

    public Button mockSkipButton;

    public GameObject fastForwardEffect; 

    public bool showingChoices;

    public bool skipDisabled;

    private Dictionary<string, string> cachedAvailableFriends;

    private List<string> cachedChoiceConditions = new List<string>();

    private static bool isShowingStatRequirement = false; 

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //if (choiceTimerSlider != null) choiceTimerSlider.value = 0f;
        if (choiceTimerGroup != null)
        {
            //choiceTimerGroup.alpha = 0f; 
            // Ensure timer UI is hidden by default OR when disabled
            choiceTimerGroup.alpha = choiceTimerEnabled ? 0f : 0f;
            choiceTimerGroup.interactable = false;
            choiceTimerGroup.blocksRaycasts = false;
        }

        if (useDualEdgeTimer && dualTimerFill != null && dualTimerContainer != null)
            EnsureDualTimerCentered(); 

        cg = new CanvasGroupController(this, canvasGroup);

        cg.alpha = 0;
        cg.SetInteractableState(false);

        //if (timerContainer != null) timerContainer.SetActive(false);
        //if (choiceTimerSlider != null) choiceTimerSlider.value = 0f;  
    }

    public void Show(string question, string[] choices, List<bool> lockStates = null, List<string> conditions = null) 
    {
        showingChoices = true; 

        lastDecision = new ChoicePanelDecision(question, choices);
        isWaitingOnUserChoice = true;

        titleText.text = question;

        // Save conditions
        cachedChoiceConditions = conditions ?? new List<string>();
        cachedLockStates = lockStates ?? new List<bool>();

        StartCoroutine(GenerateChoices(choices, lockStates)); 

        cg.Show();
        cg.SetInteractableState(active: true);

        // Start the countdown timer
        //StartChoiceTimer();

        // Only start the timer if enabled
        if (choiceTimerEnabled)
            StartChoiceTimer();
        else
            HideTimerUIImmediately();  // keep the bar hidden when disabled 
    }

    private void StopChoiceTimer()
    {
        if (!choiceTimerEnabled)
        {
            // If disabled, ensure UI is hidden and no coroutines are dangling
            if (timerRoutine != null)
            {
                StopCoroutine(timerRoutine);
                timerRoutine = null;
            }
            HideTimerUIImmediately();
            return;
        } 

        if (timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
            timerRoutine = null;
        }

        if (choiceTimerGroup != null)
        {
            if (timerFadeRoutine != null)
                StopCoroutine(timerFadeRoutine);
            timerFadeRoutine = StartCoroutine(FadeOutChoiceTimer());
        }
    } 

    private void EnsureDualTimerCentered()
    {
        if (dualTimerFill == null || dualTimerContainer == null) return;

        // Ensure pivot is dead center for true double-edge scaling
        dualTimerFill.pivot = new Vector2(0.5f, 0.5f);

        // Anchor to middle-stretch horizontally (optional but nice)
        dualTimerFill.anchorMin = new Vector2(0.5f, 0.5f);
        dualTimerFill.anchorMax = new Vector2(0.5f, 0.5f);
        dualTimerFill.anchoredPosition = Vector2.zero;

        // Make the fill as wide as the container
        var w = ((RectTransform)dualTimerContainer).rect.width;
        dualTimerFill.sizeDelta = new Vector2(w, dualTimerFill.sizeDelta.y);

        // Start fully filled
        dualTimerFill.localScale = Vector3.one;
    }

    // Optional: expose a runtime setter
    public void SetChoiceTimerEnabled(bool enabled)
    {
        choiceTimerEnabled = enabled;

        // Immediately reflect UI state if we’re currently showing choices
        if (!enabled)
        {
            // Stop any running countdown and hide the UI cleanly
            StopChoiceTimer();
            if (choiceTimerGroup != null)
            {
                choiceTimerGroup.alpha = 0f;
                choiceTimerGroup.interactable = false;
                choiceTimerGroup.blocksRaycasts = false;
            }
        }
    }

    private void HideTimerUIImmediately()
    {
        if (choiceTimerGroup != null)
        {
            choiceTimerGroup.alpha = 0f;
            choiceTimerGroup.interactable = false;
            choiceTimerGroup.blocksRaycasts = false;
        }
    } 

    public void ShowInviteMenu()
    {
        CalendarSystem calendar = CalendarSystem.Instance;

        cachedAvailableFriends = FriendAvailabilityManager.Instance.GetAvailableFriends( 
            calendar.GetCurrentSeason(),
            calendar.GetCurrentWeekDay(),
            calendar.activityPhases[calendar.activityCounter]
        );

        // Retrieve the Current Activity
        string currentActivity = ActivityTracker.Instance.GetSelectedActivity();

        // Determine the Correct Solo Text
        string soloText = "Go Alone"; // Default

        if (currentActivity.Contains("Jog"))
        {
            soloText = "Run Alone"; 
        }
        else if (currentActivity.Contains("EspressoMartini") || currentActivity.Contains("OldFashioned") || currentActivity.Contains("Margarita") || currentActivity.Contains("ShotOfTequila") || currentActivity.Contains("RedWine"))
        {
            soloText = "Drink Alone";
        } 

        if (cachedAvailableFriends.Count == 0)
        {
            Show("No one is available to join you.", new string[] { soloText }); 
        }
        else
        {
            List<string> choices = new List<string>();
            foreach (var friend in cachedAvailableFriends.Keys)
            {
                choices.Add(friend);
            }
            choices.Add(soloText);

            Show("Who do you want to invite?", choices.ToArray());
        }
    }

    /*public void ShowBarInvitesMenu()
    {
        CalendarSystem calendar = CalendarSystem.Instance;
        Dictionary<string, string> availableFriends = FriendAvailabilityManager.Instance.GetAvailableFriends(
            calendar.GetCurrentSeason(),
            calendar.GetCurrentWeekDay(),
            calendar.activityPhases[calendar.activityCounter]
        );

        if (availableFriends.Count == 0)
        {
            Show("No one is available to join you.", new string[] { "Drink Alone" });
        }
        else
        {
            List<string> choices = new List<string>();
            foreach (var friend in availableFriends.Keys)
            {
                choices.Add(friend);
            }
            choices.Add("Drink Alone");

            Show("Who do you want to invite?", choices.ToArray());
        }
    }*/

    private IEnumerator GenerateChoices(string[] choices, List<bool> lockStates = null)
    {
        float maxWidth = 0;

        // Get the currently selected activity from ActivityTracker
        string currentActivity = ActivityTracker.Instance.GetSelectedActivity();

        Dictionary<string, string> availableFriends = FriendAvailabilityManager.Instance.GetAvailableFriends(
            CalendarSystem.Instance.GetCurrentSeason(),
            CalendarSystem.Instance.GetCurrentWeekDay(),
            CalendarSystem.Instance.activityPhases[CalendarSystem.Instance.activityCounter]
        );

        for (int i = 0; i < choices.Length; i++)
        {
            ChoiceButton choiceButton;
            if (i < buttons.Count)
            {
                choiceButton = buttons[i];
            }
            else
            {
                GameObject newButtonObject = Instantiate(choiceButtonPrefab, buttonLayoutGroup.transform);
                newButtonObject.SetActive(true);

                Button newButton = newButtonObject.GetComponent<Button>();
                TextMeshProUGUI newTitle = newButton.GetComponentInChildren<TextMeshProUGUI>();
                LayoutElement newLayout = newButton.GetComponent<LayoutElement>();
                GameObject lockedIcon = newButtonObject.transform.Find("LockedIcon")?.gameObject;

                choiceButton = new ChoiceButton { button = newButton, layout = newLayout, title = newTitle, lockedIcon = lockedIcon };
                buttons.Add(choiceButton);
            }

            choiceButton.button.onClick.RemoveAllListeners();
            int buttonIndex = i;

            // ---------- NEW: detect ellipsis and hide ----------
            string trimmed = (choices[i] ?? string.Empty).Trim();
            bool isEllipsis = trimmed == "...";
            if (isEllipsis)
            {
                // Make sure it is hidden and not interactable
                choiceButton.button.gameObject.SetActive(false);

                // Also clear listeners and locked icon for safety
                if (choiceButton.lockedIcon != null)
                    choiceButton.lockedIcon.SetActive(false);

                // Skip sizing and listeners for this entry
                continue;
            }
            // ---------------------------------------------------

            // Normal path for non-ellipsis choices
            if (availableFriends.ContainsKey(choices[buttonIndex]))
            {
                choiceButton.button.onClick.AddListener(() => HandleFriendSelection(choices[buttonIndex]));
            }
            else
            {
                choiceButton.button.onClick.AddListener(() => AcceptAnswer(buttonIndex));
            }

            choiceButton.title.text = choices[i];
            choiceButton.title.margin = new Vector4(10, 0, 10, 0);

            // Respect lock state
            choiceButton.button.interactable = lockStates == null || i >= lockStates.Count ? true : !lockStates[i];

            if (choiceButton.lockedIcon != null)
            {
                bool isLocked = (lockStates != null && i < lockStates.Count && lockStates[i]);
                choiceButton.lockedIcon.SetActive(isLocked);

                if (isLocked)
                {
                    Button lockedButton = choiceButton.lockedIcon.GetComponent<Button>();
                    if (lockedButton != null)
                    {
                        lockedButton.onClick.RemoveAllListeners();

                        string condition = (cachedChoiceConditions != null && i < cachedChoiceConditions.Count) ? cachedChoiceConditions[i] : "";
                        lockedButton.onClick.AddListener(() =>
                        {
                            if (isShowingStatRequirement)
                                return;

                            isShowingStatRequirement = true;
                            LogicalLineUtils.Conditions.ShowConditionFailureMessage(condition);
                            StartCoroutine(AutoHideStatRequirement());
                        });
                    }
                }
            }

            float buttonWidth = Mathf.Clamp(BUTTON_WIDTH_PADDING + choiceButton.title.preferredWidth, BUTTON_MIN_WIDTH, BUTTON_MAX_WIDTH);
            maxWidth = Mathf.Max(maxWidth, buttonWidth);

            // Ensure visible for non-ellipsis
            choiceButton.button.gameObject.SetActive(true);
        }

        // Apply uniform width only to active buttons
        foreach (var btn in buttons)
        {
            if (btn.button != null && btn.button.gameObject.activeSelf)
                btn.layout.preferredWidth = maxWidth + 50f;
        }

        // Keep original visibility rule for non-created slots,
        // but ellipsis ones are already deactivated above.
        for (int i = 0; i < buttons.Count; i++)
        {
            bool show = i < choices.Length && buttons[i].button.gameObject.activeSelf;
            // If a button was hidden due to "...", leave it hidden.
            // Otherwise, ensure it matches range.
            if (i < choices.Length && buttons[i].button != null)
            {
                // Do nothing if already hidden (ellipsis); otherwise ensure active.
                // (No change needed here; kept for clarity)
            }
            else if (i >= choices.Length && buttons[i].button != null)
            {
                buttons[i].button.gameObject.SetActive(false);
            }
        }

        yield return new WaitForEndOfFrame();

        // Set height only for active buttons
        foreach (var btn in buttons)
        {
            if (btn.button != null && btn.button.gameObject.activeSelf)
            {
                int lines = btn.title.textInfo.lineCount;
                btn.layout.preferredHeight = BUTTON_HEIGHT_PADDING + (BUTTON_HEIGHT_PER_LINE * lines);
            }
        }
    } 

    public void Hide()
    {
        showingChoices = false;
        StopChoiceTimer();           // NEW

        cg.Hide();
        cg.SetInteractableState(false); 
    }

    private void HandleHangOutClick(string locationFile)
    {
        PlayerPrefs.SetString("StartingFile", locationFile);
        PlayerPrefs.Save();
        SceneManager.LoadScene("VisualNovel");
    }


    private void HandleFriendSelection(string friendName)
    {
        if (cachedAvailableFriends != null && cachedAvailableFriends.ContainsKey(friendName))
        {
            Debug.Log($"[FriendSelection] {friendName} was selected. Assigned File: {cachedAvailableFriends[friendName]}");
            HandleHangOutClick(cachedAvailableFriends[friendName]);
        }
        else
        {
            Debug.LogWarning($"[FriendSelection] No cached entry for {friendName}. Falling back to solo activity.");
            string currentActivity = ActivityTracker.Instance.GetSelectedActivity();
            HandleHangOutClick(currentActivity + CalendarSystem.Instance.activityPhases[CalendarSystem.Instance.activityCounter] + CalendarSystem.Instance.GetCurrentSeason());
        }
    } 

    public void Update()
    { 
        if (skipDisabled)
        { 
            theSkipButton.interactable = false; 
            mockSkipButton.interactable = false;

            theSkipButton.GetComponent<CanvasGroup>().interactable = false; 

            theSkipButton.gameObject.SetActive(false);
            mockSkipButton.gameObject.SetActive(true);

            skipReader.Disable();

            fastForwardEffect.SetActive(false);
        } 

        if (showingChoices)
        {
            mockSkipButton.interactable = false; 

            theSkipButton.gameObject.SetActive(false);
            mockSkipButton.gameObject.SetActive(true); 

            skipReader.Disable();

            fastForwardEffect.SetActive(false); 
        } 

        else
        {
            theSkipButton.interactable = true; 
            mockSkipButton.interactable = true;

            //theSkipButton.GetComponent<CanvasGroup>().interactable = true; 

            theSkipButton.gameObject.SetActive(true); 
            mockSkipButton.gameObject.SetActive(false);
        } 
    }

    private void AcceptAnswer(int index)
    {
        if (index < 0 || index > lastDecision.choices.Length - 1)
            return;

        StopChoiceTimer();           // NEW 

        lastDecision.answerIndex = index;
        isWaitingOnUserChoice = false;
        Hide();
    }

    private void StartChoiceTimer()
    {
        if (!choiceTimerEnabled) return; // <- master guard 

        // Cancel any pending fade-out
        if (timerFadeRoutine != null)
        {
            StopCoroutine(timerFadeRoutine);
            timerFadeRoutine = null;
        }

        // Reset bar to full
        if (useDualEdgeTimer && dualTimerFill != null && dualTimerContainer != null)
            EnsureDualTimerCentered();

        // Make the timer visible (fade group on; bar is already full)
        if (choiceTimerGroup != null)
        {
            choiceTimerGroup.alpha = 1f;
            choiceTimerGroup.interactable = false;
            choiceTimerGroup.blocksRaycasts = false;
        }

        if (timerRoutine != null) StopCoroutine(timerRoutine);
        timerRoutine = StartCoroutine(CountdownRoutine(choiceTimeoutSeconds));
    }

    private IEnumerator CountdownRoutine(float seconds)
    {
        float remaining = seconds;

        while (remaining > 0f && isWaitingOnUserChoice && showingChoices)
        {
            remaining -= Time.unscaledDeltaTime;

            // raw fraction of time remaining (1 -> 0)
            float frac = Mathf.Clamp01(remaining / seconds);

            // apply easing for the *visual* shrink only
            float easedFrac = easeShrink ? ApplyEase01(frac) : frac;

            // --- Double-edge shrink (use easedFrac for X scale) ---
            if (useDualEdgeTimer && dualTimerFill != null)
            {
                dualTimerFill.localScale = new Vector3(easedFrac, 1f, 1f);
                dualTimerFill.anchoredPosition = Vector2.zero;
            }

            // --- Color section (keep using 'frac' so timing is exact) ---
            if (dualTimerFillImage != null)
            {
                float f = frac; // use raw time-based fraction

                Color c;
                if (f > colorChangeThreshold + preBlendWindow)
                    c = Color.white;
                else if (f > colorChangeThreshold)
                {
                    float t = Mathf.InverseLerp(colorChangeThreshold + preBlendWindow, colorChangeThreshold, f);
                    c = Color.Lerp(Color.white, Color.red, t);
                }
                else
                    c = Color.red;

                dualTimerFillImage.color = c;
            }

            yield return null;
        }

        timerRoutine = null;
        if (isWaitingOnUserChoice && showingChoices)
            AutoSelectEllipsisOrFallback();
    }


    private float ApplyEase01(float x)
    {
        x = Mathf.Clamp01(x);

        switch (easingMode)
        {
            case EasingMode.Linear:
                return x;

            case EasingMode.EaseIn:
                // quadratic ease-in (slow -> fast)
                return x * x;

            case EasingMode.EaseOut:
                // quadratic ease-out (fast -> slow)
                return 1f - (1f - x) * (1f - x);

            case EasingMode.EaseInOut:
                // cubic ease-in-out
                return (x < 0.5f)
                    ? 4f * x * x * x
                    : 1f - Mathf.Pow(-2f * x + 2f, 3f) * 0.5f;

            case EasingMode.SmoothStep:
                // classic smoothstep (cubic) between 0 and 1
                return Mathf.SmoothStep(0f, 1f, x);

            case EasingMode.Custom:
                return Mathf.Clamp01(customEase.Evaluate(x));

            default:
                return x;
        }
    } 

    private void AutoSelectEllipsisOrFallback()
    {
        // First preference: hidden "..." (The Hidden Option)
        int index = GetIndexOfEllipsisChoice();

        // On timeout, ALWAYS choose "..." if it exists—ignore locks/visibility.
        if (index >= 0)
        {
            AcceptAnswer(index);
            return;
        }

        // Otherwise: fallback to first unlocked/visible, then any visible/interactable
        int firstUnlocked = GetFirstUnlockedIndex();
        if (firstUnlocked >= 0)
        {
            AcceptAnswer(firstUnlocked);
            return;
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].button.gameObject.activeInHierarchy && buttons[i].button.interactable)
            {
                AcceptAnswer(i);
                return;
            }
        }

        // Extreme edge case: nothing selectable
        isWaitingOnUserChoice = false;
        Hide();
    } 

    private int GetIndexOfEllipsisChoice()
    {
        if (lastDecision == null || lastDecision.choices == null)
            return -1;

        for (int i = 0; i < lastDecision.choices.Length; i++)
        {
            if (lastDecision.choices[i].Trim() == "...")
                return i;
        }
        return -1;
    }

    private bool IsIndexSelectable(int i)
    {
        // Respect lock state if we have it
        bool locked = (cachedLockStates != null && i < cachedLockStates.Count && cachedLockStates[i]);
        if (locked) return false;

        // Also verify the actual button is interactable
        if (i >= 0 && i < buttons.Count)
            return buttons[i].button != null && buttons[i].button.gameObject.activeInHierarchy && buttons[i].button.interactable;

        return true; // assume selectable if no button yet
    }

    private int GetFirstUnlockedIndex()
    {
        // Prefer an unlocked, interactable, visible button
        for (int i = 0; i < buttons.Count && i < lastDecision.choices.Length; i++)
        {
            bool locked = (cachedLockStates != null && i < cachedLockStates.Count && cachedLockStates[i]);
            if (!locked && buttons[i].button.gameObject.activeInHierarchy && buttons[i].button.interactable)
                return i;
        }

        // If cachedLockStates shorter than choices, try any interactable visible
        for (int i = 0; i < buttons.Count && i < lastDecision.choices.Length; i++)
        {
            if (buttons[i].button.gameObject.activeInHierarchy && buttons[i].button.interactable)
                return i;
        }

        return -1;
    } 

    private static IEnumerator AutoHideStatRequirement()
    {
        yield return new WaitForSeconds(3f); // Show for 3 seconds
        VNMenuManager.instance.HideRequirementUI();
        yield return new WaitForSeconds(1f);  
        isShowingStatRequirement = false; 
    }

    private IEnumerator FadeOutChoiceTimer()
    {
        float startA = choiceTimerGroup.alpha;
        float t = 0f;

        while (t < timerFadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / timerFadeDuration);
            choiceTimerGroup.alpha = Mathf.Lerp(startA, 0f, p);
            yield return null;
        }

        choiceTimerGroup.alpha = 0f;

        // Optional: snap to empty after hidden so no visual jump
        if (useDualEdgeTimer && dualTimerFill != null)
            dualTimerFill.localScale = new Vector3(0f, 1f, 1f);

        choiceTimerGroup.interactable = false;
        choiceTimerGroup.blocksRaycasts = false;

        timerFadeRoutine = null;
    } 

    public class ChoicePanelDecision
    {
        public string question = string.Empty;
        public int answerIndex = -1;
        public string[] choices = new string[0];

        public ChoicePanelDecision(string question, string[] choices)
        { 
            this.question = question;
            this.choices = choices;
            answerIndex = -1;
        }
    }

    private struct ChoiceButton
    {
        public Button button;
        public TextMeshProUGUI title;
        public LayoutElement layout;
        public GameObject lockedIcon; // Add this!
    }
}
