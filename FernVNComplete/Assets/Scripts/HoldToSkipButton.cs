using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldToSkipButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Visuals")]
    [Tooltip("Image with Image.Type = Filled; its fillAmount goes 0->1 while holding.")]
    public Image fillImage;

    [Header("Faded UI (GameObjects)")]
    [Tooltip("The faded/tooltip visuals container to show/hide.")]
    public GameObject fadedGroup;     // GameObject, not CanvasGroup

    [Tooltip("Optional: a parent/root GO for the pressable Skip Button to show/hide. Leave null to not toggle the button GO.")]
    public GameObject fadedRootGO;    // GameObject, optional

    [Header("Behavior")]
    [Tooltip("Seconds the player must hold to trigger OnCommit.")]
    public float holdSeconds = 2f;

    [Tooltip("Hide faded UI when released early (no commit).")]
    public bool autoHideWhenReleased = false;

    public event Action OnCommit;

    // NEW: Track & notify visibility
    public bool IsVisible { get; private set; }
    public event Action<bool> OnVisibilityChanged; 

    public bool IsHolding { get; private set; }

    bool holding;
    float timer;

    void Awake()
    {
        ResetFill();
        Show(false);
    }

    public void Attach(Action onCommit)
    {
        OnCommit = onCommit;
        ResetFill();
    }

    /// <summary>Shows/hides the faded visuals and (optionally) the button root.</summary>
    public void Show(bool show)
    {
        if (fadedGroup != null) fadedGroup.SetActive(show);
        if (fadedRootGO != null) fadedRootGO.SetActive(show);

        // NEW: record + notify on change
        if (IsVisible != show)
        {
            IsVisible = show;
            OnVisibilityChanged?.Invoke(show);
        }

        // Note: we do NOT toggle this.gameObject automatically to avoid hiding the component unexpectedly.
        ResetFill();
    }

    void ResetFill()
    {
        holding = false;
        IsHolding = false;
        timer = 0f;
        if (fillImage != null) fillImage.fillAmount = 0f;
    }

    public void OnPointerDown(PointerEventData e)
    {
        holding = true;
        IsHolding = true;

        // Ensure visuals are visible when the hold begins (in case you revealed only the button root).
        if (fadedGroup != null && !fadedGroup.activeSelf) fadedGroup.SetActive(true);
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (!holding) return;
        holding = false;
        IsHolding = false;

        if (autoHideWhenReleased) Show(false);
        ResetFill();
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (!holding) return;
        holding = false;
        IsHolding = false;

        if (autoHideWhenReleased) Show(false);
        ResetFill();
    }

    void Update()
    {
        if (!holding) return;

        timer += Time.unscaledDeltaTime;

        if (fillImage != null)
            fillImage.fillAmount = Mathf.Clamp01(timer / holdSeconds);

        if (timer >= holdSeconds)
        {
            holding = false;
            IsHolding = false;

            var cb = OnCommit;
            ResetFill();

            if (autoHideWhenReleased) Show(false);
            cb?.Invoke();
        }
    }
}
