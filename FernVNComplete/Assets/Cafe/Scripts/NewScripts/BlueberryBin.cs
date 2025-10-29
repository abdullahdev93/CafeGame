using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlueberryBin : MonoBehaviour
{
    public GameObject blueberries;
    public AudioClip blueberriesSound;
    public Button restockButton;
    public Image fill;
    public GameObject blueberriesTimer; // UI Image for the timer

    private int blueberriesCount = 0;
    private float timerDuration = 20f;
    private bool isRestocking = false;
    private GameObject spawnedBlueberries;
    private BoxCollider2D jarCollider;

    private void Start()
    {
        jarCollider = GetComponent<BoxCollider2D>();
        restockButton.gameObject.SetActive(false);
        blueberriesTimer.gameObject.SetActive(false);
        restockButton.onClick.AddListener(OnRestockButtonClicked);
    }

    private void Update()
    {
        if (blueberriesCount >= 25 && spawnedBlueberries == null && !isRestocking)
        {
            ShowRestockButton();
        }
    }

    void OnMouseDown()
    {
        if (blueberriesCount < 25 && !isRestocking)
        {
            if (spawnedBlueberries == null)
            {
                // TheAudioManager.Instance.PlaySound(coffeeBeansSound);
                spawnedBlueberries = Instantiate(blueberries, transform.position, transform.rotation);
                blueberriesCount++;
                jarCollider.enabled = false; // Disable the collider
            }
            else
            {
                Debug.Log("Strawberry Slices are already spawned.");
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
        blueberriesTimer.gameObject.SetActive(true);

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

        blueberriesTimer.gameObject.SetActive(false);
        blueberriesCount = 0;
        isRestocking = false;
        jarCollider.enabled = true; // Re-enable the collider
    }

    public void BlueberriesDestroyed()
    {
        spawnedBlueberries = null;
        jarCollider.enabled = true; // Re-enable the collider
    }
}
