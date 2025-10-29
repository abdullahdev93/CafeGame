using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Blender : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite BlenderWithStrawberriesSprite;
    public Sprite BlenderWithBlueberriesSprite;
    public Sprite BlenderWithBananasSprite;
    public Sprite BlenderWithStrawberriesBananasSprite;
    public Sprite BlenderWithStrawberriesBlueberriesSprite;
    public Sprite BlenderWithBlueberriesBananasSprite;
    public Sprite BlenderWithStrawberriesBlueberriesBananasSprite;
    public Sprite BlenderSprite;

    [Header("UI Elements")]
    public GameObject blenderTimer;
    public Button blendButton;
    public Image fill;
    public SpriteRenderer blenderSpriteRenderer;

    [Header("Smoothies")]
    public GameObject bananaSmoothie;
    public GameObject strawberrySmoothie;
    public GameObject blueberrySmoothie;
    public GameObject berrySmoothie;

    [Header("Timing Parameters")]
    public float blendingTime = 10f;

    private float blenderTimerMax;
    private bool blenderTimerIsRunning = false;
    private GameObject spawnedSmoothie;

    private bool hasBananas = false;
    private bool hasStrawberries = false;
    private bool hasBlueberries = false;

    private BoxCollider2D blenderCollider;
    public bool allowReturnToBlender = true;  // New variable to manage return behavior

    private void Start()
    {
        InitializeBlender();
        blenderCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (blenderTimerIsRunning)
        {
            UpdateBlendingTimer();
        }
    }

    private void InitializeBlender()
    {
        SetActiveState(blenderTimer, false);
        SetActiveState(blendButton.gameObject, false);
        blenderTimerMax = blendingTime;
        blendButton.onClick.AddListener(StartBlending);
    }

    private void UpdateBlendingTimer()
    {
        blendingTime -= Time.deltaTime;
        fill.fillAmount = blendingTime / blenderTimerMax;

        if (blendingTime <= 0f)
        {
            FinishBlending();
        }
    }

    private void FinishBlending()
    {
        blendingTime = 0f;
        blenderTimerIsRunning = false;
        SetActiveState(blenderTimer, false);
        blenderSpriteRenderer.sprite = BlenderSprite;
        blenderCollider.enabled = true; // Enable the collider 

        // Determine which smoothie to spawn based on ingredients
        if (hasBananas && hasStrawberries && hasBlueberries)
        {
            spawnedSmoothie = Instantiate(berrySmoothie, new Vector2(4.406f, -0.82f), transform.rotation);
        }
        else if (hasBananas && hasStrawberries)
        {
            spawnedSmoothie = Instantiate(bananaSmoothie, new Vector2(4.406f, -0.82f), transform.rotation);
        }
        else if (hasStrawberries && hasBlueberries)
        {
            spawnedSmoothie = Instantiate(strawberrySmoothie, new Vector2(4.406f, -0.82f), transform.rotation);
        }
        else if (hasBlueberries && hasBananas)
        {
            spawnedSmoothie = Instantiate(blueberrySmoothie, new Vector2(4.406f, -0.82f), transform.rotation);
        }
        else if (hasBananas)
        {
            spawnedSmoothie = Instantiate(bananaSmoothie, new Vector2(4.406f, -0.82f), transform.rotation);
        }
        else if (hasStrawberries)
        {
            spawnedSmoothie = Instantiate(strawberrySmoothie, new Vector2(4.406f, -0.82f), transform.rotation);
        }
        else if (hasBlueberries)
        {
            spawnedSmoothie = Instantiate(blueberrySmoothie, new Vector2(4.406f, -0.82f), transform.rotation); 
        }

        // Reset ingredients after blending
        hasBananas = false;
        hasStrawberries = false;
        hasBlueberries = false;

        SetActiveState(blendButton.gameObject, false);
        UpdateBlenderSprite();  // Reset the sprite to default or empty state if necessary
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bananas"))
        {
            hasBananas = true;
            UpdateBlenderSprite();
            PrepareForBlending(other.gameObject);
        }
        else if (other.CompareTag("Strawberries"))
        {
            hasStrawberries = true;
            UpdateBlenderSprite();
            PrepareForBlending(other.gameObject);
        }
        else if (other.CompareTag("Blueberries"))
        {
            hasBlueberries = true;
            UpdateBlenderSprite();
            PrepareForBlending(other.gameObject);
        }
    }

    private void PrepareForBlending(GameObject ingredient)
    {
        Destroy(ingredient);
        SetActiveState(blendButton.gameObject, true);
    }

    private void StartBlending()
    {
        SetActiveState(blendButton.gameObject, false);
        StartBlendingTimer();
    }

    private void StartBlendingTimer()
    {
        blenderCollider.enabled = false; // Disable the collider 
        SetActiveState(blenderTimer, true);
        blenderTimerIsRunning = true;
        blendingTime = blenderTimerMax;
    }

    private void SetActiveState(GameObject obj, bool state)
    {
        obj.SetActive(state);
    }

    private void UpdateBlenderSprite()
    {
        if (hasBananas && hasStrawberries && hasBlueberries)
        {
            blenderSpriteRenderer.sprite = BlenderWithStrawberriesBlueberriesBananasSprite;
        }
        else if (hasBananas && hasStrawberries)
        {
            blenderSpriteRenderer.sprite = BlenderWithStrawberriesBananasSprite;
        }
        else if (hasStrawberries && hasBlueberries)
        {
            blenderSpriteRenderer.sprite = BlenderWithStrawberriesBlueberriesSprite;
        }
        else if (hasBlueberries && hasBananas)
        {
            blenderSpriteRenderer.sprite = BlenderWithBlueberriesBananasSprite;
        }
        else if (hasBananas)
        {
            blenderSpriteRenderer.sprite = BlenderWithBananasSprite;
        }
        else if (hasStrawberries)
        {
            blenderSpriteRenderer.sprite = BlenderWithStrawberriesSprite;
        }
        else if (hasBlueberries)
        {
            blenderSpriteRenderer.sprite = BlenderWithBlueberriesSprite;
        }
    }

    public void SmoothieMovedAway()
    {
        spawnedSmoothie = null;
        blenderCollider.enabled = true; // Re-enable the collider
    }

    public void SmoothieDestroyed()
    {
        spawnedSmoothie = null;
        blenderCollider.enabled = true; // Re-enable the collider
    }

    public void CheckForDuplicateSmoothies(GameObject returningSmoothie)
    {
        if (spawnedSmoothie != null && returningSmoothie != spawnedSmoothie)
        {
            Destroy(returningSmoothie);
        }
        else
        {
            spawnedSmoothie = returningSmoothie;
            blenderCollider.enabled = false;
        }
    }

    public void DisableReturnToBlender()
    {
        allowReturnToBlender = false;
    }

    public void EnableCollider()
    {
        blenderCollider.enabled = true;
    }
}
