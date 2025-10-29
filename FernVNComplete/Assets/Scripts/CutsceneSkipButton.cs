using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class CutsceneSkipButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    public static CutsceneSkipButton Instance;

    // Let external systems (e.g., Gallery) hook custom skip behavior
    public event Action OnSkipRequested; // NEW

    public event Action<bool> OnVisibilityChanged;

    private float skipUITimer = 0f;
    private bool skipUIVisible = false;
    private float skipUIDuration = 5f;

    public Image progressFill;
    public Image progressBackground;
    public TextMeshProUGUI tooltipText;
    public float holdTime = 2f;
    public AudioSource skipSFX;

    private float holdTimer;
    private bool isHeld = false;
    private bool isPointerOver = false;
    private bool skipTriggered = false;

    private CanvasGroup tooltipCanvasGroup;

    public bool IsVisible => skipUIVisible;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        tooltipCanvasGroup = tooltipText.GetComponent<CanvasGroup>();
        if (tooltipCanvasGroup == null)
            tooltipCanvasGroup = tooltipText.gameObject.AddComponent<CanvasGroup>();

        tooltipCanvasGroup.alpha = 0f;
        tooltipCanvasGroup.interactable = false;
        tooltipCanvasGroup.blocksRaycasts = false;

        progressFill.fillAmount = 0f;
    }

    private void Update()
    {
        if (isHeld && !skipTriggered)
        {
            holdTimer += Time.deltaTime;
            progressFill.fillAmount = Mathf.Clamp01(holdTimer / holdTime);

            if (holdTimer >= holdTime)
            {
                skipTriggered = true;
                OnSkip();
            }
        }

        if (skipUIVisible && !isHeld)
        {
            skipUITimer += Time.deltaTime;
            if (skipUITimer >= skipUIDuration)
            {
                HideSkipUI();
            }
        }
    }

    private void OnSkip()
    {
        if (tooltipCanvasGroup != null)
            tooltipCanvasGroup.alpha = 0f;

        tooltipText.gameObject.SetActive(false);

        isHeld = false;
        holdTimer = 0f;
        progressFill.fillAmount = 0f;

        skipSFX?.Play();

        // NEW — if someone subscribed (e.g., Gallery), call them.
        if (OnSkipRequested != null)
        {
            OnSkipRequested.Invoke();
        }
        else
        {
            // Backward-compatible fallback: skip cutscene
            if (CutsceneController.Instance != null)
                CutsceneController.Instance.SkipCutsceneManually();
        }

        progressBackground.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        //progressBackground.gameObject.SetActive(true);
        //tooltipText.gameObject.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHeld = true;
        holdTimer = 0f;
        skipTriggered = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHeld = false;
        holdTimer = 0f;
        progressFill.fillAmount = 0f;
    }

    public void ShowTooltipInstantly()
    {
        skipUITimer = 0f;
        skipUIVisible = true;
        isPointerOver = true;

        tooltipText.gameObject.SetActive(true);
        progressBackground.gameObject.SetActive(true);

        if (tooltipCanvasGroup != null)
        {
            tooltipCanvasGroup.alpha = 1f;
            tooltipCanvasGroup.interactable = true;
            tooltipCanvasGroup.blocksRaycasts = true;
        }

        progressFill.fillAmount = 0f;
        SetSkipUIVisible(true);
    }

    private void HideSkipUI()
    {
        if (tooltipCanvasGroup != null)
            tooltipCanvasGroup.alpha = 0f;

        tooltipText.gameObject.SetActive(false);
        progressBackground.gameObject.SetActive(false);

        SetSkipUIVisible(false);
    }

    private void SetSkipUIVisible(bool visible)
    {
        if (skipUIVisible == visible) return;
        skipUIVisible = visible;
        OnVisibilityChanged?.Invoke(visible);
        if (visible) skipUITimer = 0f;
    }
}
