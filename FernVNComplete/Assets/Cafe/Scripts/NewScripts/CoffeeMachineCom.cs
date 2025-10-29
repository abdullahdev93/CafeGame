using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoffeeMachineCom : MonoBehaviour
{
    public static event System.Action OnCoffeePoured;
    public static event System.Action OnMachineReady;   // <-- ADD: machine is full/ready to pour

    [Header("Sprites")]
    public Sprite coffeeMachineFullSprite;
    public Sprite coffeeMachineEmptySprite;

    [Header("UI Elements")]
    public GameObject theCoffeeMachineTimer;
    public TextMeshProUGUI restockText;
    public Button startPouringButton;
    public Image fill;
    public SpriteRenderer coffeeMachineSpriteRenderer; // SpriteRenderer to change the sprite

    [Header("Coffee")]
    public GameObject cupOfCoffee;

    [Header("Timing Parameters")]
    public float pouringCoffeeTime = 5f;
    public float restockTime;

    private float coffeeMachineTimerMax;
    private float restockTimerMax;
    private int useCount = 0;
    private const int maxUseCount = 10;
    private bool coffeeMachineTimerIsRunning = false;
    private bool isRestocking = false;
    public bool allowReturnToMachine = true;  // New variable to manage return behavior 
    private GameObject spawnedFreshCoffee;
    private BoxCollider2D machineCollider;

    private Vector3 initialCoffeePosition;

    private void Start()
    {
        InitializeMachine();
        machineCollider = GetComponent<BoxCollider2D>(); 
    }

    private void Update()
    {
        if (coffeeMachineTimerIsRunning)
        {
            UpdatePouringCoffeeTimer();
        }

        if (spawnedFreshCoffee != null)
        {
            if (Vector3.Distance(spawnedFreshCoffee.transform.position, initialCoffeePosition) > 1f)
            {
                spawnedFreshCoffee = null;
            }
        }
    }

    private void InitializeMachine()
    {
        SetActiveState(theCoffeeMachineTimer, false);
        SetActiveState(restockText.gameObject, false);
        SetActiveState(startPouringButton.gameObject, false);
        coffeeMachineSpriteRenderer.sprite = coffeeMachineEmptySprite; //coffeeMachineFullSprite;
        coffeeMachineTimerMax = pouringCoffeeTime;
        startPouringButton.onClick.AddListener(StartPouringCoffee);
    }

    private void UpdatePouringCoffeeTimer()
    {
        pouringCoffeeTime -= Time.deltaTime;
        fill.fillAmount = pouringCoffeeTime / coffeeMachineTimerMax;

        if (pouringCoffeeTime <= 0f)
        {
            FinishPouringCoffee();
        }
    }

    private void FinishPouringCoffee()
    {
        pouringCoffeeTime = 0f;
        coffeeMachineTimerIsRunning = false;
        spawnedFreshCoffee = Instantiate(cupOfCoffee, transform.position, transform.rotation); 
        initialCoffeePosition = spawnedFreshCoffee.transform.position;
        SetActiveState(theCoffeeMachineTimer, false);

        useCount++;
        if (useCount >= maxUseCount)
        {
            SetToEmptyState();
        }
        else
        {
            coffeeMachineSpriteRenderer.sprite = coffeeMachineFullSprite;
        }

        // Raise the event from inside the declaring type
        OnCoffeePoured?.Invoke(); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CanStartPouringCoffee(other))
        {
            PrepareForPouringCoffee(other.gameObject);
            machineCollider.enabled = false; // Disable the collider  
        }

        if (CanRestockMachine(other))
        {
            RestockMachine(other.gameObject);
        }
    }

    private bool CanStartPouringCoffee(Collider2D other)
    {
        return other.CompareTag("EmptyCup") && !coffeeMachineTimerIsRunning && spawnedFreshCoffee == null && coffeeMachineSpriteRenderer.sprite == coffeeMachineFullSprite;
    }

    private void PrepareForPouringCoffee(GameObject coffeeBean)
    {
        Destroy(coffeeBean);
        SetActiveState(startPouringButton.gameObject, true);
    }

    private void StartPouringCoffee()
    {
        SetActiveState(startPouringButton.gameObject, false);
        StartPouringCoffeeTimer();
    }

    private void StartPouringCoffeeTimer()
    {
        SetActiveState(theCoffeeMachineTimer, true);
        coffeeMachineTimerIsRunning = true;
        pouringCoffeeTime = coffeeMachineTimerMax;
    }

    private void SetToEmptyState()
    {
        coffeeMachineSpriteRenderer.sprite = coffeeMachineEmptySprite;
        SetActiveState(restockText.gameObject, true);
    }

    private bool CanRestockMachine(Collider2D other)
    {
        return other.CompareTag("CoffeeBeans") && coffeeMachineSpriteRenderer.sprite == coffeeMachineEmptySprite && !isRestocking;
    }

    private void RestockMachine(GameObject coffeeBean)
    {
        Destroy(coffeeBean);
        SetActiveState(restockText.gameObject, false);
        StartCoroutine(RestockTimer());
    }

    private IEnumerator RestockTimer()
    {
        restockTime = GetRestockTimeBasedOnEndurance();
        restockTimerMax = restockTime; // Initialize restockTimerMax
        SetActiveState(theCoffeeMachineTimer, true);
        fill.fillAmount = 1f; // Set fill amount to full at start

        while (restockTime > 0)
        {
            restockTime -= Time.deltaTime;
            fill.fillAmount = restockTime / restockTimerMax;
            yield return null;
        }

        CompleteRestocking();
    }

    private float GetRestockTimeBasedOnEndurance()
    {
        int enduranceLevel = GameSession.Instance.playerStats.Endurance.Level;

        if (enduranceLevel == 0) return 20f;
        if (enduranceLevel > 9) return 3f;
        if (enduranceLevel > 8) return 5f;
        if (enduranceLevel > 6) return 10f;
        if (enduranceLevel > 4) return 12f;
        if (enduranceLevel > 2) return 15f;

        return 20f; // Default value for endurance level 1 or 2
    }

    private void CompleteRestocking()
    {
        SetActiveState(theCoffeeMachineTimer, false);
        isRestocking = false;
        useCount = 0;
        coffeeMachineSpriteRenderer.sprite = coffeeMachineFullSprite;

        OnMachineReady?.Invoke();               // <-- FIRE when machine becomes full 
    }

    private void SetActiveState(GameObject obj, bool state)
    {
        obj.SetActive(state);
    }

    public void CoffeeMovedAway()
    {
        spawnedFreshCoffee = null;
        machineCollider.enabled = true; // Re-enable the collider only if the cup is not snapped to a snap point
    } 

    public void CoffeeDestroyed()
    {
        spawnedFreshCoffee = null;
        machineCollider.enabled = true; // Re-enable the collider
    }

    public void CheckForDuplicateCups(GameObject returningCup)
    {
        if (spawnedFreshCoffee != null && returningCup != spawnedFreshCoffee) 
        {
            Destroy(returningCup);
        }
        else
        {
            spawnedFreshCoffee = returningCup;
            machineCollider.enabled = false;
        }
    }

    public void DisableReturnToMachine()
    {
        allowReturnToMachine = false;
    }

    public void EnableCollider()
    {
        machineCollider.enabled = true;
    }
}
