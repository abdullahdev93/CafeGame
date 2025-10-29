using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BananaBin : MonoBehaviour
{
    public GameObject bananaSlices;
    public AudioClip bananaSlicesSound;
    public Button restockButton;
    public Image fill;
    public GameObject bananaSlicesTimer; // UI Image for the timer

    private int bananaSlicesCount = 0;
    private float timerDuration = 20f;
    private bool isRestocking = false;
    private GameObject spawnedBananaSlices;
    private BoxCollider2D jarCollider;

    private void Start()
    {
        jarCollider = GetComponent<BoxCollider2D>();
        restockButton.gameObject.SetActive(false);
        bananaSlicesTimer.gameObject.SetActive(false);
        restockButton.onClick.AddListener(OnRestockButtonClicked);
    }

    private void Update()
    {
        if (bananaSlicesCount >= 25 && spawnedBananaSlices == null && !isRestocking)
        {
            ShowRestockButton();
        }
    }

    void OnMouseDown()
    {
        if (bananaSlicesCount < 25 && !isRestocking)
        {
            if (spawnedBananaSlices == null)
            {
                // TheAudioManager.Instance.PlaySound(coffeeBeansSound);
                spawnedBananaSlices = Instantiate(bananaSlices, transform.position, transform.rotation);
                bananaSlicesCount++;
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
        bananaSlicesTimer.gameObject.SetActive(true);

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

        bananaSlicesTimer.gameObject.SetActive(false);
        bananaSlicesCount = 0;
        isRestocking = false;
        jarCollider.enabled = true; // Re-enable the collider
    }

    public void BananaSlicesDestroyed()
    {
        spawnedBananaSlices = null;
        jarCollider.enabled = true; // Re-enable the collider
    }
}
