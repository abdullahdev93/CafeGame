using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CoffeeBeanJar : MonoBehaviour
{
    public static event System.Action OnBeansSpawned; 

    public GameObject coffeeBeans;
    public AudioClip coffeeBeansSound;
    public Button restockButton;
    public Image fill;
    public GameObject coffeeBeansTimer; // UI Image for the timer

    private int coffeeBeanCount = 0;
    public float timerDuration = 20f; 
    private bool isRestocking = false;
    private GameObject spawnedCoffeeBeans;
    private BoxCollider2D jarCollider;

    private void Start()
    {
        jarCollider = GetComponent<BoxCollider2D>();
        restockButton.gameObject.SetActive(false);
        coffeeBeansTimer.gameObject.SetActive(false);
        restockButton.onClick.AddListener(OnRestockButtonClicked);
    }

    private void Update()
    {
        if (coffeeBeanCount >= 25 && spawnedCoffeeBeans == null && !isRestocking)
        {
            ShowRestockButton();
        }
    }

    void OnMouseDown()
    {
        if (coffeeBeanCount < 25 && !isRestocking)
        {
            if (spawnedCoffeeBeans == null)
            {
                // TheAudioManager.Instance.PlaySound(coffeeBeansSound);
                spawnedCoffeeBeans = Instantiate(coffeeBeans, transform.position, transform.rotation);
                coffeeBeanCount++;
                jarCollider.enabled = false; // Disable the collider 

                OnBeansSpawned?.Invoke(); // <-- ADDED: notify tutorial 
            }
            else
            {
                Debug.Log("Coffee beans are already spawned.");
            }
        }
    }

    private void ShowRestockButton()
    {
        restockButton.gameObject.SetActive(true);
    }

    private void OnRestockButtonClicked()
    {
        StartRestockTimer();
    }

    private void StartRestockTimer()
    {
        restockButton.gameObject.SetActive(false);
        isRestocking = true;
        coffeeBeansTimer.gameObject.SetActive(true);

        int enduranceLevel = GameSession.Instance.playerStats.Endurance.Level;
        if (enduranceLevel > 9)
        {
            timerDuration = 5f;
        }
        else if (enduranceLevel > 6)
        {
            timerDuration = 10f;
        }
        else if (enduranceLevel > 3)
        {
            timerDuration = 15f;
        }

        StartCoroutine(RestockTimer());
    }

    private IEnumerator RestockTimer()
    {
        float timeLeft = timerDuration;
        while (timeLeft > 0)
        {
            fill.fillAmount = timeLeft / timerDuration;
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        coffeeBeansTimer.gameObject.SetActive(false);
        coffeeBeanCount = 0;
        isRestocking = false;
        jarCollider.enabled = true; // Re-enable the collider
    }

    public void CoffeeBeansDestroyed()
    {
        spawnedCoffeeBeans = null;
        jarCollider.enabled = true; // Re-enable the collider
    }
}
