using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public CafeContinuePrompt continuePrompt;

    [Header("UI")]
    public CanvasGroup overlay;                     // dim background (optional)
    public TextMeshProUGUI tutorialText;            // message box text
    public Button nextButton;                       // "Next/Continue" button (used when we want manual advance)
    public float fadeTime = 0.25f;

    [Header("Targets to Pulse")]
    //public Transform orderBubble;                   // assign the bubble on the first spawn point placeholder (or a prefab ref)
    public Transform coffeeBeanJar;                 // assign your CoffeeBeanJar transform
    public Transform coffeeMachine;                 // assign your CoffeeMachine transform
    public Transform cupStack;                      // assign your CupStack transform

    [Header("References")]
    public SpawnManager spawnManager;
    public GameSession gameSession;

    [Header("Settings")]
    public bool runTutorialOnStart = true;

    private bool stepSatisfied;
    private Coroutine currentStepRoutine;
    private int tutorialCustomerServedCount;

    private TextArchitect architect;

    // TutorialManager.cs (fields)
    private CustomerBehavior impatientForGate;

    // Track which step we’re on and (optionally) which customer type must be served
    private Step currentStep;
    private CustomerBehavior.CustomerType? expectedServeType = null;

    // Gates Step 9 until the impatient coffee is brewed
    private bool waitingForImpatientPour = false;

    // Keep a handle to the impatient customer we spawned
    //private CustomerBehavior impatientForGate; 

    private enum Step
    {
        Welcome,
        ReadOrder,
        SpawnBeans,
        BrewCoffee,
        SpawnCup,
        CombineCupAndCoffee,
        ServeFirstCustomer_NoTip,
        PatienceExplainer,
        ServeImpatient,
        TipExplainer,
        ServeGenerousTipper,
        Finish
    }

    private void Awake()
    {
        if (Instance == null) Instance = this; else { Destroy(gameObject); return; }
        if (spawnManager == null) spawnManager = FindObjectOfType<SpawnManager>();
        if (gameSession == null) gameSession = FindObjectOfType<GameSession>();

        // Typewriter setup
        architect = new TextArchitect(tutorialText, TABuilder.BuilderTypes.Typewriter);

        // Attach the prompt to our tutorial TMP
        if (continuePrompt != null)
            continuePrompt.AttachTo(tutorialText);

        // Subscribe to tutorial hooks
        CoffeeBeanJar.OnBeansSpawned += OnBeansSpawned;
        CoffeeMachineCom.OnMachineReady += OnMachineReady;   // <-- ADD 
        CoffeeMachineCom.OnCoffeePoured += OnCoffeePoured;
        CupStack.OnCupSpawned += OnCupSpawned;
        FreshCoffee.OnCupOfCoffeeSpawned += OnCupOfCoffeeSpawned;   // <— ADD 
        CustomerBehavior.OnServed += OnCustomerServed;
        CoffeeMachineCom.OnCoffeePoured += OnCoffeePouredImpatient;   // NEW 
    }

    private void OnDestroy()
    {
        CoffeeBeanJar.OnBeansSpawned -= OnBeansSpawned;
        CoffeeMachineCom.OnMachineReady -= OnMachineReady;   // <-- ADD 
        CoffeeMachineCom.OnCoffeePoured -= OnCoffeePoured;
        CupStack.OnCupSpawned -= OnCupSpawned;
        FreshCoffee.OnCupOfCoffeeSpawned -= OnCupOfCoffeeSpawned;   // <— ADD 
        CustomerBehavior.OnServed -= OnCustomerServed;
        CoffeeMachineCom.OnCoffeePoured -= OnCoffeePouredImpatient;   // NEW 
    }

    private void Start()
    {
        // Pause random spawns until tutorial finishes
        spawnManager.SetRandomSpawningEnabled(false);

        if (runTutorialOnStart)
        {
            // Hide overall earnings UI text if needed etc. (optional)
            StartTutorial();
        }
        else
        {
            // If skipping tutorial, just enable normal play
            spawnManager.SetRandomSpawningEnabled(true);
            gameSession.StartCafeGame();
        }
    }

    public void StartTutorial()
    {
        if (currentStepRoutine != null) StopCoroutine(currentStepRoutine);
        currentStepRoutine = StartCoroutine(TutorialFlow());
    } 

    private IEnumerator TutorialFlow()
    {
        // Step 1 — Welcome
        yield return ShowStep(
            Step.Welcome,
            "Welcome to the cafe! Let’s learn how to serve customers quickly and accurately."
        ); // :contentReference[oaicite:1]{index=1}

        // Spawn Regular (no patience meter, same Cup of Coffee order, tips disabled)
        var regular = spawnManager.SpawnTutorialCustomer(
            CustomerBehavior.CustomerType.Regular,
            forceCupOfCoffee: true,
            showPatienceMeter: false,
            allowTips: false
        );

        // Step 2 — Reading the order (pulse the bubble)
        //PulseHighlighter.Attach(orderBubble, true);
        yield return ShowStep(
            Step.ReadOrder,
            "This customer wants a Cup of Coffee. Their request appears in the bubble above their head."
        ); // :contentReference[oaicite:2]{index=2}
        //PulseHighlighter.Attach(orderBubble, false);

        // Step 3 — Beans (pulse jar; wait until beans are spawned)
        PulseHighlighter.Attach(coffeeBeanJar, true);
        yield return WaitForStep(Step.SpawnBeans,
            "Click the Coffee Bean Jar to spawn Coffee Beans.");
        PulseHighlighter.Attach(coffeeBeanJar, false); // :contentReference[oaicite:3]{index=3}

        // Step 4 — Brew (pulse machine; wait for Coffee poured)
        PulseHighlighter.Attach(coffeeMachine, true);
        yield return WaitForStep(Step.BrewCoffee,
            "Now drag the Coffee Beans to the Coffee Machine to brew fresh coffee.");
        PulseHighlighter.Attach(coffeeMachine, false); // :contentReference[oaicite:4]{index=4}

        // Step 5 — Cup (pulse cup stack; wait for cup spawned)
        PulseHighlighter.Attach(cupStack, true);
        yield return WaitForStep(Step.SpawnCup,
            "Great! Now let’s serve it in a cup. Click the Cup Stack to spawn a cup.");
        PulseHighlighter.Attach(cupStack, false); // :contentReference[oaicite:5]{index=5}

        // Step 6 — Combine
        yield return WaitForStep(Step.CombineCupAndCoffee,
            "Drag the Cup onto the Coffee to make a Cup of Coffee.");

        // Step 7 — Serve (order bubble pulse; wait for serve; no tip)
        //PulseHighlighter.Attach(orderBubble, true);
        yield return WaitForStep(Step.ServeFirstCustomer_NoTip,
            "Now drag the Cup of Coffee to the customer’s bubble.");
        //PulseHighlighter.Attach(orderBubble, false); // :contentReference[oaicite:6]{index=6}

        // Step 8 — Patience explainer (spawn Impatient with meter)
        yield return ShowStep(Step.PatienceExplainer,
            "Customers have different patience levels. Serve them before their patience meter runs out!");
        // Spawn Impatient
        var impatient = spawnManager.SpawnTutorialCustomer(
            CustomerBehavior.CustomerType.Impatient,
            forceCupOfCoffee: true,
            showPatienceMeter: true,
            allowTips: false
        ); // :contentReference[oaicite:7]{index=7} 
        impatientForGate = impatient;                  // NEW: remember them 
        // NEW: pause this customer's patience countdown until we finish the next line + 1s
        if (impatient != null) impatient.SetTimerPaused(true);

        // Arm the gate — don’t show Step 9 line until the impatient coffee is brewed
        //waitingForImpatientPour = true;                // NEW  

        // Wait until the impatient coffee is brewed
        //yield return new WaitUntil(() => waitingForImpatientPour == false);   // NEW
        // Step 9 — Serve Impatient (specialized wait that resumes timer 1s after the line finishes)
        yield return WaitForServeImpatientStep(
            impatientForGate,
            "Serve the Impatient Customer (same Cup of Coffee).",
            1f
        );

        // Arm the gate — don’t show Step 9 line until the impatient coffee is brewed
        //waitingForImpatientPour = true;                // NEW   

        //yield return new WaitUntil(() => waitingForImpatientPour == false);   // NEW 
        // Step 10 — Tips explainer (spawn GenerousTipper)
        yield return ShowStep(Step.TipExplainer,
            "The faster you serve an accurate order, the bigger your tip!");
        var generous = spawnManager.SpawnTutorialCustomer(
            CustomerBehavior.CustomerType.GenerousTipper,
            forceCupOfCoffee: true,
            showPatienceMeter: true,
            allowTips: true
        ); // :contentReference[oaicite:8]{index=8}

        // Step 11 — Serve GenerousTipper
        yield return WaitForStep(Step.ServeGenerousTipper,
            "Serve the Generous Tipper (Cup of Coffee).");

        // Step 12 — Finish
        yield return ShowStep(Step.Finish,
            "You’re ready! Keep customers happy, earn money, and build your reputation.\n\n• Full mode unlocked\n• You can replay this tutorial anytime from the menu.");

        // Resume normal spawning + start game clock
        spawnManager.SetRandomSpawningEnabled(true);
        gameSession.StartCafeGame();
    }

    // ==== Step Utilities ====

    private IEnumerator ShowStep(Step step, string message)
    {
        currentStep = step;                  // <-- ADD
        expectedServeType = null;            // <-- ADD 

        stepSatisfied = false;

        yield return FadeOverlay(1f);

        // Prep the button
        bool clicked = false;
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() => clicked = true);
        }

        // Start typewriter and hide the prompt while text is building
        if (continuePrompt != null) continuePrompt.Hide();
        architect.Build(message);

        // While typing, a click should force-complete (but NOT advance)
        while (architect.isBuilding)
        {
            if (clicked)
            {
                clicked = false;           // consume the click
                architect.ForceComplete(); // finish the line
                break;
            }
            yield return null;
        }

        // Text finished: show the prompt at the end of the line
        if (continuePrompt != null) continuePrompt.Show();

        // Now wait for a click to advance
        while (!clicked) yield return null;

        // Hide prompt and button, then fade out
        if (continuePrompt != null) continuePrompt.Hide();
        if (nextButton != null) nextButton.gameObject.SetActive(false);

        yield return FadeOverlay(0f);
        stepSatisfied = true;
    }

    private IEnumerator WaitForStep(Step step, string message)
    {
        currentStep = step;                  // <-- ADD

        // If this step requires serving a specific customer, set the expected type
        switch (step)                        // <-- ADD
        {
            case Step.ServeFirstCustomer_NoTip:
                expectedServeType = CustomerBehavior.CustomerType.Regular;
                break;
            case Step.ServeImpatient:
                expectedServeType = CustomerBehavior.CustomerType.Impatient;
                break;
            case Step.ServeGenerousTipper:
                expectedServeType = CustomerBehavior.CustomerType.GenerousTipper;
                break;
            default:
                expectedServeType = null;
                break;
        }

        stepSatisfied = false;

        yield return FadeOverlay(1f);

        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.gameObject.SetActive(false);
        }

        if (continuePrompt != null) continuePrompt.Hide();
        architect.Build(message);

        // Let the typewriter finish naturally
        while (architect.isBuilding) yield return null;

        yield return FadeOverlay(0f); 

        while (!stepSatisfied) yield return null;
    }

    private IEnumerator FadeOverlay(float targetAlpha)
    {
        if (overlay == null) yield break;
        float start = overlay.alpha;
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            overlay.alpha = Mathf.Lerp(start, targetAlpha, t / fadeTime);
            yield return null;
        }
        overlay.alpha = targetAlpha;
    }

    private void MarkSatisfied()
    {
        stepSatisfied = true;
    }

    // ==== Event Handlers from hooks ====

    private void OnBeansSpawned()
    {
        if (currentStep == Step.SpawnBeans)
            MarkSatisfied();
    }

    private void OnMachineReady()
    {
        if (currentStep == Step.BrewCoffee)
            MarkSatisfied();
    }

    private void OnCoffeePoured()
    {
        //if (currentStep == Step.BrewCoffee)
            MarkSatisfied(); 
    }

    // Fired alongside your normal OnCoffeePoured, but only releases Step 9 when armed
    private void OnCoffeePouredImpatient()
    {
        // Only care if we’re specifically waiting to start the Impatient step
        if (!waitingForImpatientPour) return;

        if (currentStep != Step.PatienceExplainer && currentStep != Step.ServeImpatient) return; 

        // Optional: if you want to be extra strict, also check we’re post-PatienceExplainer:
        // if (currentStep != Step.PatienceExplainer && currentStep != Step.ServeImpatient) return;

        waitingForImpatientPour = false;  // release the gate so the flow continues into Step 9
    } 

    private void OnCupSpawned()
    {
        if (currentStep == Step.SpawnCup)
            MarkSatisfied();
    }

    // NEW: Cup of Coffee created by combining FreshCoffee + EmptyCup
    private void OnCupOfCoffeeSpawned()
    {
        if (currentStep == Step.CombineCupAndCoffee)
            MarkSatisfied();
    }

    //private void OnCupOfCoffeeSpawned() => MarkSatisfied();       // <— NEW: Cup of Coffee made; next text shows: "Now drag the Cup of Coffee..."// SpawnCup
    private void OnCustomerServed(CustomerBehavior cb)
    {
        tutorialCustomerServedCount++;

        // Only satisfy serve-steps when the served customer's type matches
        if (expectedServeType.HasValue)
        {
            if (cb.customerType == expectedServeType.Value)
            {
                expectedServeType = null;    // clear once satisfied
                MarkSatisfied();
            }
        }

        else if (cb.customerType == CustomerBehavior.CustomerType.Impatient)
        {
            MarkSatisfied(); 
        } 

        else
        {
            // For non-serve steps that (rarely) use this hook, still allow satisfaction
            MarkSatisfied();
        }
    } 

    private IEnumerator WaitForServeImpatientStep(CustomerBehavior impatient, string message, float resumeDelaySeconds = 1f)
    {
        currentStep = Step.ServeImpatient;                                 // <-- ADD
        expectedServeType = CustomerBehavior.CustomerType.Impatient;       // <-- ADD 

        stepSatisfied = false;

        // Same pre-step UI as WaitForStep
        yield return FadeOverlay(1f);

        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.gameObject.SetActive(false);
        }

        if (continuePrompt != null) continuePrompt.Hide();
        architect.Build(message);

        // Let the typewriter finish naturally
        while (architect.isBuilding) yield return null;

        // Fade the overlay out so player can act
        yield return FadeOverlay(0f);

        // NEW: resume the impatient customer's timer 1 second after the line finishes
        if (impatient != null)
        {
            yield return new WaitForSeconds(resumeDelaySeconds); 
            //yield return new WaitForSecondsRealtime(resumeDelaySeconds); 
            impatient.SetTimerPaused(false);
        }

        // Now wait for the step to actually be completed (customer served)
        while (!stepSatisfied) yield return null;
        expectedServeType = null;  // <-- optional cleanup 
    } 
}