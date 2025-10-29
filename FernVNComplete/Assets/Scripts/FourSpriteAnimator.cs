using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Animation/Sprite Animator with Delay (Match by Sprite)")]
public class FourSpriteAnimator : MonoBehaviour
{
    [System.Serializable]
    public class ExpressionEntry
    {
        [Tooltip("Drag the exact sprite(s) that should trigger this frameset (e.g., Mei_Happy, Mei_Happy_Tilted, etc.)")]
        public List<Sprite> matchSprites = new List<Sprite>();

        [Tooltip("Frames to play when any of the matchSprites is active (order = playback order).")]
        public Sprite[] frames;
    }

    [Header("Expressions (drag sprites)")]
    [SerializeField] private List<ExpressionEntry> expressionSets = new List<ExpressionEntry>();

    //[Header("Fallback Frames (optional)")]
    //[Tooltip("Used when no sprite matches. Leave empty to show whatever the target has without animating.")]
    //[SerializeField] private Sprite[] defaultFrames;

    [Header("Timing")]
    [Tooltip("Seconds to wait before/after each animation loop.")]
    [SerializeField] private float waitTime = 2f;

    [Tooltip("Frames per second for playback.")]
    [SerializeField, Min(0.01f)] private float fps = 8f;

    [Tooltip("Loop animation forever.")]
    [SerializeField] private bool loop = true;

    [Header("Target (choose one)")]
    [Tooltip("UI Image to animate/read. If empty, uses SpriteRenderer on this GameObject.")]
    [SerializeField] private Image targetImage;
    private SpriteRenderer sr;

    // --- Runtime ---
    private Dictionary<Sprite, Sprite[]> spriteMap;   // key: exact Sprite object, value: frames
    private Sprite[] frames;                           // active frameset
    private int frameIndex = 0;
    private bool paused = false;
    private Coroutine animationRoutine;

    // We remember the "anchor" sprite that triggered the current frameset.
    // While the animation plays (and swaps sprites), we don't re-detect unless the target sprite becomes a DIFFERENT sprite not in our current frames.
    private Sprite currentAnchorSprite = null;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null && targetImage == null)
            Debug.LogWarning($"[{nameof(FourSpriteAnimator)}] No SpriteRenderer found and no Image assigned.");

        BuildMap();
        //frames = defaultFrames;
    }

    private void OnEnable()
    {
        DetectAndSwapFrames(force: true);

        if (frames != null && frames.Length > 0)
            animationRoutine = StartCoroutine(PlayAnimationLoop());
    }

    private void OnDisable()
    {
        if (animationRoutine != null)
            StopCoroutine(animationRoutine);
        animationRoutine = null;
    }

    private void Update()
    {
        DetectIfExternalPoseChanged();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        BuildMap();
    }
#endif

    // ---------- Map / Matching ----------

    private void BuildMap()
    {
        spriteMap = new Dictionary<Sprite, Sprite[]>();
        if (expressionSets == null) return;

        foreach (var entry in expressionSets)
        {
            if (entry == null || entry.frames == null || entry.frames.Length == 0) continue;
            if (entry.matchSprites == null) continue;

            foreach (var s in entry.matchSprites)
            {
                if (s == null) continue;
                if (!spriteMap.ContainsKey(s))
                    spriteMap.Add(s, entry.frames);
                else
                    Debug.LogWarning($"[{nameof(FourSpriteAnimator)}] Duplicate sprite key '{s.name}' ignored (same reference already mapped).");
            }
        }
    }

    private Sprite GetCurrentSprite()
    {
        if (targetImage != null) return targetImage.sprite;
        if (sr != null) return sr.sprite;
        return null;
    }

    // Only re-detect when some other system set a DIFFERENT sprite (not one of our active frames).
    private void DetectIfExternalPoseChanged()
    {
        var cur = GetCurrentSprite();
        if (cur == null)
        {
            DetectAndSwapFrames(force: false);
            return;
        }

        // If we're currently animating and the sprite showing is one of our frames,
        // do nothing—this change was made by us.
        if (frames != null && frames.Length > 0)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                if (frames[i] == cur) return; // still our own frame ? ignore
            }
        }

        // Otherwise, a new pose sprite was set externally ? re-resolve.
        DetectAndSwapFrames(force: false);
    }

    private void DetectAndSwapFrames(bool force)
    {
        Sprite current = GetCurrentSprite();

        Sprite[] newSet = null;
        Sprite newAnchor = null;

        if (current != null && spriteMap.TryGetValue(current, out var found))
        {
            newSet = found;
            newAnchor = current;
        }

        if (!force && ReferenceEquals(frames, newSet)) return;

        frames = newSet;
        currentAnchorSprite = newAnchor;
        frameIndex = 0;
        ApplyFrame(SafeFrame(0));

        // Ensure the animator is actually running after a new match appears.
        if (animationRoutine == null && frames != null && frames.Length > 0)
            animationRoutine = StartCoroutine(PlayAnimationLoop());
    } 

    // ---------- Animation ----------

    private IEnumerator PlayAnimationLoop()
    {
        float frameDuration = 1f / fps;

        while (true)
        {
            if (!paused && waitTime > 0f)
                yield return new WaitForSeconds(waitTime);

            int localLen = frames?.Length ?? 0;

            for (frameIndex = 0; frameIndex < localLen; frameIndex++)
            {
                if (paused)
                {
                    ApplyFrame(SafeFrame(frameIndex));
                    while (paused) yield return null;
                }

                ApplyFrame(SafeFrame(frameIndex));
                yield return new WaitForSeconds(frameDuration);

                // If frameset changed mid-loop, restart safely
                if (frames == null) break;
                if (frames.Length != localLen)
                {
                    localLen = frames.Length;
                    frameIndex = -1; // next -> 0
                }
            }

            if (!paused && waitTime > 0f)
                yield return new WaitForSeconds(waitTime);

            if (!loop)
                break;
        }
    }

    private Sprite SafeFrame(int idx)
    {
        if (frames == null || frames.Length == 0) return null;
        idx = Mathf.Clamp(idx, 0, frames.Length - 1);
        return frames[idx];
    }

    private void ApplyFrame(Sprite sprite)
    {
        if (sprite == null) return;

        if (targetImage != null)
            targetImage.sprite = sprite;
        else if (sr != null)
            sr.sprite = sprite;
    }

    // ---------- Public Controls ----------

    public void Pause() => paused = true;
    public void Resume() => paused = false;

    /// <summary>Jump to first frame and pause.</summary>
    public void PauseAndReset()
    {
        paused = true;
        frameIndex = 0;
        ApplyFrame(SafeFrame(0));
    }

    public void SetPaused(bool p) => paused = p;

    /// <summary>Manually override the active frameset.</summary>
    public void SetFrames(Sprite[] newFrames, bool restartToFirst = true)
    {
        if (newFrames == null || newFrames.Length == 0) return;
        frames = newFrames;
        if (restartToFirst)
        {
            frameIndex = 0;
            ApplyFrame(SafeFrame(0));
        }
    }
}
