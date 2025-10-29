using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrawberryBin : MonoBehaviour
{
    public GameObject strawberrySlices;
    public AudioClip strawberrySlicesSound;
    public Button restockButton;
    public Image fill;
    public GameObject strawberrySlicesTimer; // UI Image for the timer

    private int strawberrySlicesCount = 0;
    private float timerDuration = 20f;
    private bool isRestocking = false;
    private GameObject spawnedStrawberrySlices;
    private BoxCollider2D jarCollider;

    private void Start()
    {
        jarCollider = GetComponent<BoxCollider2D>();
        restockButton.gameObject.SetActive(false);
        strawberrySlicesTimer.gameObject.SetActive(false);
        restockButton.onClick.AddListener(OnRestockButtonClicked);
    }

    private void Update()
    {
        if (strawberrySlicesCount >= 25 && spawnedStrawberrySlices == null && !isRestocking)
        {
            ShowRestockButton();
        }
    }

    void OnMouseDown()
    {
        if (strawberrySlicesCount < 25 && !isRestocking)
        {
            if (spawnedStrawberrySlices == null)
            {
                // TheAudioManager.Instance.PlaySound(coffeeBeansSound);
                spawnedStrawberrySlices = Instantiate(strawberrySlices, transform.position, transform.rotation); 
                strawberrySlicesCount++;
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
        strawberrySlicesTimer.gameObject.SetActive(true);

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

        strawberrySlicesTimer.gameObject.SetActive(false);
        strawberrySlicesCount = 0;
        isRestocking = false;
        jarCollider.enabled = true; // Re-enable the collider
    }

    public void StrawberrySlicesDestroyed()
    {
        spawnedStrawberrySlices = null;
        jarCollider.enabled = true; // Re-enable the collider
    }
}
