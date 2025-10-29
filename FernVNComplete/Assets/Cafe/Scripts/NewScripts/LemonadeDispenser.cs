using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LemonadeDispenser : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite lemonadeDispenserFullSprite;
    public Sprite lemonadeDispenserEmptySprite;

    [Header("UI Elements")]
    public GameObject theLemonadeDispenserTimer;
    public Button startPouringButton;
    public Button restockButton; // Reference to the restock button
    public Image fill;
    public SpriteRenderer lemonadeDispenserSpriteRenderer; // SpriteRenderer to change the sprite

    [Header("Lemonade")]
    public GameObject lemonade;

    [Header("Timing Parameters")]
    public float pouringTime = 5f;
    public float restockTime;

    private float lemonadeDispenserTimerMax;
    private float restockTimerMax;
    private int useCount = 0;
    private const int maxUseCount = 10;
    private bool lemonadeDispenserTimerIsRunning = false;
    private bool isRestocking = false;
    private GameObject spawnedFreshLemonade;

    private Vector3 initialLemonadePosition;

    private BoxCollider2D dispenserCollider;

    private void Start()
    {
        InitializeDispenser();
        dispenserCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (lemonadeDispenserTimerIsRunning)
        {
            UpdatePouringTimer();
        }

        if (spawnedFreshLemonade != null)
        {
            if (Vector3.Distance(spawnedFreshLemonade.transform.position, initialLemonadePosition) > 1f)
            {
                spawnedFreshLemonade = null;
            }
        }
    }

    private void InitializeDispenser()
    {
        SetActiveState(theLemonadeDispenserTimer, false);
        SetActiveState(startPouringButton.gameObject, false);
        SetActiveState(restockButton.gameObject, false);
        lemonadeDispenserSpriteRenderer.sprite = lemonadeDispenserFullSprite;
        lemonadeDispenserTimerMax = pouringTime;
        startPouringButton.onClick.AddListener(StartPouring);
        restockButton.onClick.AddListener(OnRestockButtonClicked); // Add listener to the restock button
    }

    private void UpdatePouringTimer()
    {
        pouringTime -= Time.deltaTime;
        fill.fillAmount = pouringTime / lemonadeDispenserTimerMax;

        if (pouringTime <= 0f)
        {
            FinishPouring();
        }
    }

    private void FinishPouring()
    {
        pouringTime = 0f;
        lemonadeDispenserTimerIsRunning = false;
        spawnedFreshLemonade = Instantiate(lemonade, new Vector2(2.254f, -1.26f), transform.rotation);  
        initialLemonadePosition = spawnedFreshLemonade.transform.position;
        SetActiveState(theLemonadeDispenserTimer, false);
        dispenserCollider.enabled = true; // Enable the collider   

        useCount++;
        if (useCount >= maxUseCount)
        {
            SetToEmptyState();
        }
        else
        {
            lemonadeDispenserSpriteRenderer.sprite = lemonadeDispenserFullSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CanStartPouring(other))
        {
            PrepareForPouring(other.gameObject);
            dispenserCollider.enabled = false; // Disable the collider  
        }
    }

    private bool CanStartPouring(Collider2D other)
    {
        return other.CompareTag("EmptyCup") && !lemonadeDispenserTimerIsRunning && spawnedFreshLemonade == null && lemonadeDispenserSpriteRenderer.sprite == lemonadeDispenserFullSprite;
    }

    private void PrepareForPouring(GameObject emptyCup)
    {
        Destroy(emptyCup);
        SetActiveState(startPouringButton.gameObject, true);
    }

    private void StartPouring()
    {
        SetActiveState(startPouringButton.gameObject, false);
        StartPouringTimer();
    }

    private void StartPouringTimer()
    {
        dispenserCollider.enabled = false; // Disable the collider  
        SetActiveState(theLemonadeDispenserTimer, true);
        lemonadeDispenserTimerIsRunning = true;
        pouringTime = lemonadeDispenserTimerMax;
    }

    private void SetToEmptyState()
    {
        lemonadeDispenserSpriteRenderer.sprite = lemonadeDispenserEmptySprite;
        SetActiveState(restockButton.gameObject, true); // Show the restock button
    }

    private void OnRestockButtonClicked()
    {
        SetActiveState(restockButton.gameObject, false); // Hide the restock button
        StartCoroutine(RestockTimer());
    }

    private IEnumerator RestockTimer()
    {
        restockTime = GetRestockTimeBasedOnEndurance();
        restockTimerMax = restockTime; // Initialize restockTimerMax
        SetActiveState(theLemonadeDispenserTimer, true);
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
        SetActiveState(theLemonadeDispenserTimer, false);
        isRestocking = false;
        useCount = 0;
        lemonadeDispenserSpriteRenderer.sprite = lemonadeDispenserFullSprite;
    }

    private void SetActiveState(GameObject obj, bool state)
    {
        obj.SetActive(state);
    }

    public void LemonadeMovedAway()
    {
        spawnedFreshLemonade = null;
        dispenserCollider.enabled = true; // Re-enable the collider
    }

    public void LemonadeDestroyed()
    {
        spawnedFreshLemonade = null;
        dispenserCollider.enabled = true; // Re-enable the collider
    }

    public void CheckForDuplicateCups(GameObject returningCup)
    {
        if (spawnedFreshLemonade != null && returningCup != spawnedFreshLemonade)
        {
            Destroy(returningCup);
        }
        else
        {
            spawnedFreshLemonade = returningCup;
            dispenserCollider.enabled = false;
        }
    }

    public void EnableCollider()
    {
        dispenserCollider.enabled = true;
    }
}
