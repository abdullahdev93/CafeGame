using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

[System.Serializable]
public class CutsceneWaitEntry
{
    public string cutsceneName;
    public float waitBefore = 0f;
    public float waitAfter = 0f;
} 

public class CutsceneController : MonoBehaviour
{
    public static CutsceneController Instance; 

    private bool isFadingOutPhase = false; // NEW: true during any end fade 

    //private bool isFadingOutPhase = false; // existing
    private bool hasVideoStarted = false;  // NEW: becomes true after first frame is rendering 

    [Header("UI Display")]
    public RawImage rawImage;
    public RenderTexture renderTexture;
    public float fadeDuration = 0.5f;

    [Header("Video Setup")]
    public VideoPlayer videoPlayer;
    public List<VideoClip> cutsceneClips;

    [Header("Skip Button")]
    public GameObject skipButton;
    public GameObject skipButtonFaded; 

    [Header("Wait Times for Each Cutscene")]
    public List<CutsceneWaitEntry> cutsceneWaitEntries = new List<CutsceneWaitEntry>();

    [Header("Cursor")]
    [Tooltip("Seconds the mouse cursor stays visible after a click/move (when Skip UI is not visible).")]
    public float mouseRevealDuration = 2f; 

    [Tooltip("Minimum mouse movement (in pixels) that counts as movement.")]
    public float mouseMovePixelThreshold = 1.5f;

    private Coroutine cursorHideRoutine;
    private bool forceCursorVisibleFromSkip = false; 

    // NEW: track mouse delta
    private Vector3 _lastMousePos; 

    private Dictionary<string, VideoClip> clipDictionary; 
    private Dictionary<string, CutsceneWaitEntry> cutsceneWaitTimes; 

    private bool videoFinished = false;

    private bool hasShownSkipButton = false;

    private bool isPlayingCutscene = false; 

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        clipDictionary = new Dictionary<string, VideoClip>();
        foreach (var clip in cutsceneClips)
        {
            if (clip != null && !clipDictionary.ContainsKey(clip.name))
            {
                clipDictionary.Add(clip.name, clip);
            }
        }

        cutsceneWaitTimes = new Dictionary<string, CutsceneWaitEntry>();
        foreach (var entry in cutsceneWaitEntries)
        {
            if (!string.IsNullOrEmpty(entry.cutsceneName) && !cutsceneWaitTimes.ContainsKey(entry.cutsceneName))
            {
                cutsceneWaitTimes.Add(entry.cutsceneName, entry);
            }
        } 

        if (videoPlayer != null)
        {
            videoPlayer.targetTexture = renderTexture;
            videoPlayer.loopPointReached += OnVideoEnd;
        }

        if (rawImage != null)
        {
            rawImage.texture = renderTexture;
            rawImage.color = new Color(1, 1, 1, 0);
            rawImage.raycastTarget = false;
            rawImage.enabled = true;
        }

        skipButton.SetActive(false); 
    }

    private void Update()
    {
        if (!isPlayingCutscene) return;

        // NEW: During fade-out, always hide the cursor (ignore skip UI & movement)
        if (isFadingOutPhase)
        {
            SetCursorVisible(false);
            _lastMousePos = Input.mousePosition;
            return;
        }

        // NEW: Before the cutscene actually starts, force-hide the cursor no matter what.
        if (!hasVideoStarted)
        {
            SetCursorVisible(false);
            _lastMousePos = Input.mousePosition;
            return;
        } 

        // If Skip UI is holding the cursor visible, nothing else to do
        if (forceCursorVisibleFromSkip || (CutsceneSkipButton.Instance != null && CutsceneSkipButton.Instance.IsVisible))
        {
            SetCursorVisible(true);
            _lastMousePos = Input.mousePosition; // keep baseline fresh
            return;
        }

        // Detect movement in screen pixels
        Vector3 now = Input.mousePosition;
        if ((now - _lastMousePos).sqrMagnitude >= mouseMovePixelThreshold * mouseMovePixelThreshold)
        {
            // Any movement reveals the cursor for a short time
            RevealCursorTemporarily();
        }
        _lastMousePos = now;
    } 

    private void SetCursorVisible(bool visible)
    {
        Cursor.visible = visible;
        if (visible && Cursor.lockState != CursorLockMode.None)
            Cursor.lockState = CursorLockMode.None;
    }

    private void RevealCursorTemporarily()
    {
        // NEW: Don’t reveal before the cutscene has started
        if (!hasVideoStarted)
        {
            SetCursorVisible(false);
            return;
        } 

        // If Skip UI is visible, keep cursor visible indefinitely
        if (forceCursorVisibleFromSkip || (CutsceneSkipButton.Instance != null && CutsceneSkipButton.Instance.IsVisible))
        {
            SetCursorVisible(true);
            return;
        }

        SetCursorVisible(true);
        if (cursorHideRoutine != null) StopCoroutine(cursorHideRoutine);
        cursorHideRoutine = StartCoroutine(HideCursorAfterDelay(mouseRevealDuration));
    }

    private IEnumerator HideCursorAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (!forceCursorVisibleFromSkip) SetCursorVisible(false);
        cursorHideRoutine = null;
    } 

    // NEW — subscribe once Skip UI exists
    private void SubscribeSkipVisibility()
    {
        if (CutsceneSkipButton.Instance == null) return;

        // avoid double subscription
        CutsceneSkipButton.Instance.OnVisibilityChanged -= HandleSkipUIVisibilityChanged;
        CutsceneSkipButton.Instance.OnVisibilityChanged += HandleSkipUIVisibilityChanged;
    }

    // NEW — keep cursor visible while Skip UI is visible
    private void HandleSkipUIVisibilityChanged(bool visible)
    {
        // NEW: ignore skip UI during fade-out; keep cursor hidden
        if (isFadingOutPhase)
        {
            forceCursorVisibleFromSkip = false;
            SetCursorVisible(false);
            return;
        }

        // NEW: Before video starts, ignore Skip UI’s attempt to show cursor
        if (!hasVideoStarted)
        {
            forceCursorVisibleFromSkip = false;
            SetCursorVisible(false);
            return;
        }

        forceCursorVisibleFromSkip = visible;

        if (visible)
        {
            if (cursorHideRoutine != null) StopCoroutine(cursorHideRoutine);
            SetCursorVisible(true);
        }
        else
        {
            // Skip UI just faded away—if still in cutscene, hide cursor now
            if (isPlayingCutscene) SetCursorVisible(false);
        }
    } 

    /*void Update()
    {
        // If cutscene is playing and skip button hasn't appeared yet
        if (Input.GetMouseButtonDown(0) && isPlayingCutscene && !hasShownSkipButton)
        {
            skipButton.SetActive(true);
            CutsceneSkipButton.Instance.progressBackground.gameObject.SetActive(true);
            CutsceneSkipButton.Instance.tooltipText.gameObject.SetActive(true); 
            hasShownSkipButton = true;
        }
    }*/

    public void HandleCutsceneClick()
    {
        if (isPlayingCutscene) 
        {
            skipButton.SetActive(true);
            CutsceneSkipButton.Instance.ShowTooltipInstantly(); 
            hasShownSkipButton = true;

            // NEW: reveal the cursor for a moment (or indefinitely if Skip UI is up)
            RevealCursorTemporarily(); 
        }
    }

    public IEnumerator PlayCutscene(string cutsceneName)
    {
        isPlayingCutscene = true;
        hasVideoStarted = false; // NEW: reset start gate 

        // Track initial mouse position for movement detection
        //_lastMousePos = Input.mousePosition;

        // Subscribe to Skip UI visibility
        SubscribeSkipVisibility(); 

        if (!clipDictionary.TryGetValue(cutsceneName, out VideoClip selectedClip))
        {
            Debug.LogWarning($"No cutscene found with name: {cutsceneName}");
            yield break;
        }

        if (skipButton != null)
            skipButton.SetActive(true);

        // NEW — hide the cursor as the cutscene begins
        forceCursorVisibleFromSkip = false;
        if (cursorHideRoutine != null) { StopCoroutine(cursorHideRoutine); cursorHideRoutine = null; }
        SetCursorVisible(false); 

        videoFinished = false;
        renderTexture.Release();

        rawImage.color = new Color(1, 1, 1, 0);       // Keep cutscene hidden
        rawImage.raycastTarget = false;              // Prevent early clicks

        //Step 1: Fade TO black
        yield return StartCoroutine(FadeDarkenPanel(true, 1.0f));  // Fade in (to black)

        //Step 2: Pause on black
        yield return new WaitForSeconds(3.0f);

        //Step 3: Play video (while screen is still black)
        videoPlayer.clip = selectedClip;
        videoPlayer.Play();

        // NEW: flip 'hasVideoStarted' as soon as the first frame is presented
        StartCoroutine(MarkStartedWhenFirstFrame()); 

        //Step 4: Fade rawImage to visible (cutscene is now running, but hidden)
        rawImage.raycastTarget = true;

        yield return StartCoroutine(FadeRawImage(0f, 1f, 1.0f)); // Fade in cutscene visuals

        //Step 5: Fade FROM black to transparent (cutscene is now visible)
        yield return StartCoroutine(FadeDarkenPanel(false, 1.0f)); 

        // Wait for the video to end
        while (!videoFinished)
            yield return null;

        //Step 6: Fade TO black at end 
        //Step 6: Fade TO black at end
        BeginFadeOutPhase(); // NEW: start fade-out phase (cursor hidden, skip ignored) 
        yield return StartCoroutine(FadeDarkenPanel(true, 1.0f));

        //Optional pause if you want a breath before continuing
        yield return new WaitForSeconds(1.0f); 

        // Cleanup
        if (skipButton != null)
            skipButton.SetActive(false);

        rawImage.raycastTarget = false;

        // Now fade out cutscene visuals
        yield return StartCoroutine(FadeRawImage(1f, 0f, 1.0f));

        //Step 7: Fade FROM black to transparent (if needed before next scene/dialogue)
        yield return StartCoroutine(FadeDarkenPanel(false, 1.0f));

        // End of fade-out sequence: allow normal cursor behavior again
        EndFadeOutPhase(); // NEW 

        renderTexture.Release(); 

        if (cutsceneWaitTimes.TryGetValue(cutsceneName, out CutsceneWaitEntry waitEntry) && waitEntry.waitAfter > 0f)
            yield return new WaitForSeconds(waitEntry.waitAfter);

        // NEW — cutscene done, ensure cursor is visible for normal gameplay
        isPlayingCutscene = false;
        hasVideoStarted = false; // NEW: reset 
        forceCursorVisibleFromSkip = false;
        if (cursorHideRoutine != null) { StopCoroutine(cursorHideRoutine); cursorHideRoutine = null; }
        SetCursorVisible(true); 
    }

    /*public IEnumerator PlayCutscene(string cutsceneName)
    {
        if (videoPlayer == null || rawImage == null || renderTexture == null)
        {
            Debug.LogWarning("Missing VideoPlayer, RawImage, or RenderTexture.");
            yield break;
        }

        if (!clipDictionary.TryGetValue(cutsceneName, out VideoClip selectedClip))
        {
            Debug.LogWarning($"No cutscene found with name: {cutsceneName}");
            yield break;
        }

        // Wait BEFORE cutscene
        if (cutsceneWaitTimes.TryGetValue(cutsceneName, out CutsceneWaitEntry waitEntry) && waitEntry.waitBefore > 0f)
        {
            Debug.Log($"[CutsceneController] Waiting {waitEntry.waitBefore} seconds before '{cutsceneName}'...");
            yield return new WaitForSeconds(waitEntry.waitBefore);
        }

        renderTexture.Release();
        rawImage.raycastTarget = true;
        videoFinished = false;

        videoPlayer.clip = selectedClip;
        videoPlayer.Play();

        yield return StartCoroutine(FadeRawImage(0f, 1f, fadeDuration));

        while (!videoFinished)
        {
            yield return null;
        }

        rawImage.raycastTarget = false;
        yield return StartCoroutine(FadeRawImage(1f, 0f, fadeDuration));
        renderTexture.Release();

        // Wait AFTER cutscene
        if (waitEntry != null && waitEntry.waitAfter > 0f)
        {
            Debug.Log($"[CutsceneController] Waiting {waitEntry.waitAfter} seconds after '{cutsceneName}'...");
            yield return new WaitForSeconds(waitEntry.waitAfter);
        } 

        // Optional Wait AFTER cutscene (if needed, optional):
        // yield return new WaitForSeconds(waitTimeAfter);
    }*/

    public void StopCutscene()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }

        rawImage.raycastTarget = false;
        renderTexture.Release();

        // NEW: treat this like an end fade — keep cursor hidden until fade is done
        BeginFadeOutPhase();
        StartCoroutine(FadeRawImage(1f, 0f, fadeDuration));
        StartCoroutine(EndFadeOutPhaseAfter(fadeDuration + 0.05f)); // small buffer

        // Restore normal flags AFTER fade control above
        isPlayingCutscene = false;
        hasVideoStarted = false; // NEW: reset here too 
        forceCursorVisibleFromSkip = false;
        if (cursorHideRoutine != null) { StopCoroutine(cursorHideRoutine); cursorHideRoutine = null; }
        // NOTE: Don't force SetCursorVisible(true) here; the EndFadeOutPhaseAfter coroutine does that once the fade is complete.
    } 

    private void OnVideoEnd(VideoPlayer vp)
    {
        videoFinished = true;

        CutsceneSkipButton.Instance.tooltipText.gameObject.SetActive(false); 

        skipButtonFaded.SetActive(false);  
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }

    private IEnumerator FadeRawImage(float startAlpha, float endAlpha, float duration) 
    {
        float timer = 0f;
        Color currentColor = rawImage.color;

        while (timer < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            rawImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        rawImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, endAlpha);
    }

    public void SkipCutsceneManually()
    {
        if (!isPlayingCutscene) return; 

        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            Debug.Log("[CutsceneController] Cutscene manually skipped.");
            videoPlayer.Stop();
            videoFinished = true; // Triggers coroutine to continue 
            isPlayingCutscene = false; // block multiple triggers 
        }
    }

    private IEnumerator FadeDarkenPanel(bool fadeIn, float duration)
    {
        Image panel = VNMenuManager.instance.darkenPanel.GetComponent<Image>();
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        float time = 0f;
        Color color = panel.color;

        while (time < duration)
        {
            float t = time / duration;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            panel.color = color;
            time += Time.deltaTime;
            yield return null;
        }

        color.a = endAlpha;
        panel.color = color;
    }

    private void BeginFadeOutPhase()
    {
        isFadingOutPhase = true;
        forceCursorVisibleFromSkip = false;
        if (cursorHideRoutine != null) { StopCoroutine(cursorHideRoutine); cursorHideRoutine = null; }

        // Hide Skip UI immediately so it can't force the cursor visible
        if (skipButton != null) skipButton.SetActive(false);
        if (CutsceneSkipButton.Instance != null)
        {
            // If your Skip script has a Hide method, call it; otherwise disabling the GO above is enough.
            // CutsceneSkipButton.Instance.Hide(); // (optional)
        }

        SetCursorVisible(false);
    }

    private void EndFadeOutPhase()
    {
        isFadingOutPhase = false;
        // If we're no longer in a cutscene, restore normal cursor visibility
        if (!isPlayingCutscene) SetCursorVisible(true);
    }

    private IEnumerator EndFadeOutPhaseAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        EndFadeOutPhase();
    }

    private IEnumerator MarkStartedWhenFirstFrame()
    {
        // Wait until VideoPlayer reports it’s playing
        while (videoPlayer != null && !videoPlayer.isPlaying)
            yield return null;

        // Then wait until at least one frame has been presented
        // (videoPlayer.frame can start at -1; wait for > 0)
        while (videoPlayer != null && videoPlayer.isPlaying && videoPlayer.frame <= 0)
            yield return null;

        hasVideoStarted = true; // NOW it’s allowed for the cursor logic to reveal
    } 
}
