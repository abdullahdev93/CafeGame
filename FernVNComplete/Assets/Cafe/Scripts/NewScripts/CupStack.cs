using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CupStack : MonoBehaviour
{
    public static event System.Action OnCupSpawned; 

    public GameObject cup;
    public AudioClip cupSound;
    public Button restockButton;
    public Image fill;
    public GameObject cupTimer; // UI Image for the timer

    private int coffeeBeanCount = 0;
    public float timerDuration = 20f; 
    private bool isRestocking = false;
    private GameObject spawnedCup;
    private BoxCollider2D jarCollider;

    private void Start()
    {
        jarCollider = GetComponent<BoxCollider2D>();
        restockButton.gameObject.SetActive(false);
        cupTimer.gameObject.SetActive(false);
        restockButton.onClick.AddListener(OnRestockButtonClicked);
    }

    private void Update()
    {
        if (coffeeBeanCount >= 25 && spawnedCup == null && !isRestocking)
        {
            ShowRestockButton();
        }
    }

    void OnMouseDown()
    {
        if (coffeeBeanCount < 25 && !isRestocking)
        {
            if (spawnedCup == null)
            {
                SpawnCup();
            }
            else
            {
                Debug.Log("A cup is already spawned.");
            }
        }
    }

    private void SpawnCup()
    {
        // TheAudioManager.Instance.PlaySound(cupSound);
        spawnedCup = Instantiate(cup, transform.position, transform.rotation);
        coffeeBeanCount++;
        jarCollider.enabled = false; // Disable the collider 

        OnCupSpawned?.Invoke(); // <-- ADDED 
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
        cupTimer.gameObject.SetActive(true);

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

        cupTimer.gameObject.SetActive(false);
        coffeeBeanCount = 0;
        isRestocking = false;
        jarCollider.enabled = true; // Re-enable the collider
    }

    public void CupMovedAway()
    {
        spawnedCup = null;
        jarCollider.enabled = true; // Re-enable the collider
    }

    public void CupDestroyed()
    {
        spawnedCup = null;
        jarCollider.enabled = true; // Re-enable the collider
    }

    public void CheckForDuplicateCups(GameObject returningCup)
    {
        if (spawnedCup != null && returningCup != spawnedCup)
        {
            Destroy(returningCup);
        }
        else
        {
            spawnedCup = returningCup;
            jarCollider.enabled = false;
        }
    }

    public void EnableCollider()
    {
        jarCollider.enabled = true;
    }
}
