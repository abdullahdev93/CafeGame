using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems; 
using COMMANDS; 

public class GalleryMenu : MonoBehaviour
{
    private const int PAGE_BUTTON_LIMIT = 2;

    // Pages 1-5 = Images, 6-10 = Videos
    private const int IMAGE_PAGE_COUNT = 5;
    private const int VIDEO_PAGE_COUNT = 5;
    private const int TOTAL_PAGES = IMAGE_PAGE_COUNT + VIDEO_PAGE_COUNT;

    private int maxPages = TOTAL_PAGES;
    private int selectedPage = 0;

    [Header("Hold-to-Skip UI")]
    [SerializeField] private HoldToSkipButton holdSkip;

    [Header("Root/Preview Panels")]
    [SerializeField] private CanvasGroup root;
    private CanvasGroupController rootCG;

    [SerializeField] private CanvasGroup previewPanel;
    private CanvasGroupController previewPanelCG;

    [SerializeField] private Button previewButton; // RawImage target used for image and video
    private RawImage PreviewSurface => previewButton.targetGraphic as RawImage;

    [Header("Thumbnails Grid")]
    [SerializeField] private Button[] galleryPreviewButtons; // grid of preview buttons (thumbnails)
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    [Header("Image Data (Pages 1-5)")]
    [SerializeField] private Texture[] galleryImages;
    [Tooltip("Resources.LoadAll path for images (Textures)")]
    [SerializeField] private string imagesResourcesPath = FilePaths.resources_gallery;

    [Header("Video Data (Pages 6-10)")]
    [SerializeField] private Texture[] galleryVideoThumbs; // Thumbnails for videos
    [SerializeField] private VideoClip[] galleryVideoClips; // Actual video clips
    [Tooltip("Resources.LoadAll path for video thumbnails (Textures)")]
    [SerializeField] private string videoThumbsResourcesPath = "Gallery/VideoThumbs";
    [Tooltip("Resources.LoadAll path for video clips (VideoClip)")]
    [SerializeField] private string videoClipsResourcesPath = "Gallery/VideoClips";

    [Header("Video Playback")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RenderTexture videoRenderTexture;
    [SerializeField] private Button simpleSkipButton;
    [SerializeField] private GameObject simpleSkipButtonFaded;

    [Header("Misc UI")]
    public GameObject exitButton;
    public TextMeshProUGUI pageText;
    public Button downloadButton; // Images only

    [Header("Cursor Auto-Hide")]
    [SerializeField] private bool autoHideCursor = true;
    [SerializeField] private float mouseMoveRevealPixels = 1.5f;

    [Header("Auto-hide Rules (While Video Playing)")]
    [SerializeField] private float bothVisibleHideSeconds = 5f;  // cursor + Skip -> hide both after 5s
    [SerializeField] private float cursorOnlyHideSeconds = 2f;   // cursor alone -> hide after 2s

    // NEW — post-cutscene auto-hide
    [Header("Post-Video Cursor Hide (NEW)")]
    [SerializeField] private bool hideCursorAfterCutscene = true;
    [SerializeField] private float hideCursorDelayAfterCutscene = 3f; // “a few seconds” after end

    // Runtime
    private bool cursorSessionActive = false;
    private Vector3 lastMousePos;
    private bool originalCursorVisible = true;

    private float visibilityTimer = 0f;
    private Coroutine skipAutoHideRoutine;     // unused but kept for safety
    private Coroutine postCutsceneHideRoutine; // NEW

    private bool initialized = false;
    private int previewsPerPage => galleryPreviewButtons.Length;

    private Texture currentPreviewImage; // Image currently shown
    private bool isVideoPreview = false;
    private int currentVideoIndex = -1;

    // Behavior toggles
    [SerializeField] private bool imageClickToClose = true;
    [SerializeField] private bool videoClickDisabled = true;

    [Header("Audio (when videos/cutscenes start)")]
    [SerializeField] private bool stopSongAndAmbienceOnVideoPlay = true;

    [Header("Audio Resume After Video")]
    [SerializeField] private bool resumeSongAndAmbienceAfterVideo = true;
    [SerializeField] private bool resumeAtFullVolume = true; // if false, use the original startVolume snapshot
                                                             
                                                             // --- Add under [Header("Video Data (Pages 6-10)")]
    [Header("Video Download")]
    [Tooltip("Folder where the ORIGINAL video files live, e.g. StreamingAssets/Graphics/Cutscenes")]
    [SerializeField] private string streamingVideosSubfolder = "Graphics/Cutscenes";

    // Try these extensions in order (case-insensitive). Put the ones you actually ship.
    [SerializeField] private string[] videoFileExtensions = new[] { ".mp4" };

    [Header("Download Button Visual Reset")]
    [SerializeField] private float downloadButtonResetDelay = 1.5f; // seconds 

    // Optional override mapping (ClipName -> FileNameWithExt) when import names differ from disk files
    //[SerializeField] private TextAsset videoFilenameMapJson;
    // JSON shape: { "entries":[ {"clip":"Cutscene01","file":"cutscene_01_final.mp4"}, ... ] }

    [System.Serializable] private class VideoFilenameMap { public Entry[] entries; [System.Serializable] public class Entry { public string clip; public string file; } }
    private Dictionary<string, string> clipToFileName; 

    public float fadeDuration = 0.5f;

    private void Start()
    {
        rootCG = new CanvasGroupController(this, root);
        previewPanelCG = new CanvasGroupController(this, previewPanel);

        GalleryConfig.Load();
        LoadAllResources();
        //LoadVideoFilenameMap();   // <-- NEW 

        if (downloadButton != null)
        {
            downloadButton.onClick.RemoveAllListeners();
            downloadButton.onClick.AddListener(DownloadCurrent);   // <-- unified handle
            //downloadButton.onClick.AddListener(DownloadCurrentImage);
            downloadButton.gameObject.SetActive(false);
        }

        if (holdSkip != null) holdSkip.Show(false); // start hidden
        StartCoroutine(WatchForEscapeSkip());
    }

    /*private void LoadVideoFilenameMap()
    {
        clipToFileName = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
        if (videoFilenameMapJson == null || string.IsNullOrWhiteSpace(videoFilenameMapJson.text)) return;
        try
        {
            var map = JsonUtility.FromJson<VideoFilenameMap>(videoFilenameMapJson.text);
            if (map?.entries != null)
            {
                foreach (var e in map.entries)
                {
                    if (!string.IsNullOrEmpty(e.clip) && !string.IsNullOrEmpty(e.file) && !clipToFileName.ContainsKey(e.clip))
                        clipToFileName.Add(e.clip, e.file);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[Gallery] Failed parsing videoFilenameMapJson: {ex.Message}");
        }
    }*/

    private IEnumerator WatchForEscapeSkip()
    {
        while (true)
        {
            if (isVideoPreview && Input.GetKeyDown(KeyCode.Escape))
            {
                SkipVideoNow();
            }
            yield return null;
        }
    }

    private void StopBgmForVideoIfNeeded()
    {
        if (!stopSongAndAmbienceOnVideoPlay) return;

        try
        {
            // 0 = ambience (per your command DB default), 1 = song
            if (AudioManager.instance != null)
            {
                AudioManager.instance.StopTrack(0);
                AudioManager.instance.StopTrack(1);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"StopBgmForVideoIfNeeded failed: {ex.Message}");
        }
    } 

    public void Open()
    {
        if (!initialized)
            Initialize();

        rootCG.Show();
        rootCG.SetInteractableState(true);
    }

    public void Close()
    {
        rootCG?.Hide();
        rootCG.SetInteractableState(false);
    }

    private void Initialize()
    {
        initialized = true;
        ConstructNavBar();
        LoadPage(1);
        UpdatePageText();
    }

    private void LoadAllResources()
    {
        // Images
        galleryImages = Resources.LoadAll<Texture>(imagesResourcesPath);

        // Videos
        galleryVideoThumbs = Resources.LoadAll<Texture>(videoThumbsResourcesPath);
        galleryVideoClips = Resources.LoadAll<VideoClip>(videoClipsResourcesPath);
    }

    private void ConstructNavBar()
    {
        int pagelimit = PAGE_BUTTON_LIMIT < maxPages ? PAGE_BUTTON_LIMIT : maxPages;

        prevButton.gameObject.SetActive(pagelimit < maxPages);
        nextButton.gameObject.SetActive(pagelimit < maxPages);

        nextButton.transform.SetAsLastSibling();
    }

    private void Update()
    {
        nextButton.interactable = selectedPage < TOTAL_PAGES;
        prevButton.interactable = selectedPage > 1;

        // Unified auto-hide logic while a video is actively previewing
        if (!cursorSessionActive || !isVideoPreview || !autoHideCursor) return;

        bool skipIsVisible = (holdSkip != null && holdSkip.IsVisible);

        Vector3 now = Input.mousePosition;
        float moved = Vector3.Distance(now, lastMousePos);
        bool anyInput = moved >= mouseMoveRevealPixels || Input.anyKeyDown;

        if (anyInput)
        {
            ShowCursor();
            visibilityTimer = 0f;
        }

        lastMousePos = now;

        if (!Cursor.visible)
        {
            visibilityTimer = 0f;
            return;
        }

        visibilityTimer += Time.unscaledDeltaTime;

        if (skipIsVisible)
        {
            if (visibilityTimer >= bothVisibleHideSeconds)
            {
                HideCursor();
                if (holdSkip != null) holdSkip.Show(false);
                visibilityTimer = 0f;
            }
        }
        else
        {
            if (visibilityTimer >= cursorOnlyHideSeconds)
            {
                HideCursor();
                visibilityTimer = 0f;
            }
        }
    }

    private bool IsImagePage(int page) => page >= 1 && page <= IMAGE_PAGE_COUNT;
    private bool IsVideoPage(int page) => page >= (IMAGE_PAGE_COUNT + 1) && page <= TOTAL_PAGES;

    private int GetVideoPageIndex(int pageNumber) => pageNumber - (IMAGE_PAGE_COUNT + 1);

    private void LoadPage(int pageNumber)
    {
        selectedPage = Mathf.Clamp(pageNumber, 1, TOTAL_PAGES);

        // Clear all button listeners each time
        for (int i = 0; i < previewsPerPage; i++)
        {
            Button b = galleryPreviewButtons[i];
            b.onClick.RemoveAllListeners();
            b.transform.parent.gameObject.SetActive(false);
        }

        if (IsImagePage(selectedPage))
        {
            int startingIndex = (selectedPage - 1) * previewsPerPage;

            for (int i = 0; i < previewsPerPage; i++)
            {
                int index = startingIndex + i;
                Button button = galleryPreviewButtons[i];

                if (index >= galleryImages.Length)
                {
                    button.transform.parent.gameObject.SetActive(false);
                    continue;
                }

                Texture previewImage = galleryImages[index];
                var ri = button.targetGraphic as RawImage;

                button.transform.parent.gameObject.SetActive(true);
                button.onClick.RemoveAllListeners();

                if (GalleryConfig.ImageIsUnlocked(previewImage.name))
                {
                    ri.color = Color.white;
                    ri.texture = previewImage;
                    button.onClick.AddListener(() => ShowPreviewImage(previewImage));
                }
                else
                {
                    ri.color = Color.black;
                    ri.texture = null;
                }
            }
        }
        else if (IsVideoPage(selectedPage))
        {
            int pageZeroBased = GetVideoPageIndex(selectedPage); // 0..4
            int startingIndex = pageZeroBased * previewsPerPage;

            for (int i = 0; i < previewsPerPage; i++)
            {
                int index = startingIndex + i;
                Button button = galleryPreviewButtons[i];

                if (index >= galleryVideoThumbs.Length || index >= galleryVideoClips.Length)
                {
                    button.transform.parent.gameObject.SetActive(false);
                    continue;
                }

                Texture thumb = galleryVideoThumbs[index];
                VideoClip clip = galleryVideoClips[index];
                var ri = button.targetGraphic as RawImage;

                button.transform.parent.gameObject.SetActive(true);
                button.onClick.RemoveAllListeners();

                bool unlocked = GalleryConfig.VideoIsUnlocked(clip.name);
                if (unlocked)
                {
                    ri.color = Color.white;
                    ri.texture = thumb;
                    int captured = index;
                    button.onClick.AddListener(() => ShowPreviewVideo(captured));
                }
                else
                {
                    ri.color = Color.black;
                    ri.texture = null;
                }
            }
        }

        UpdatePageText();
    }

    private void ShowPreviewImage(Texture image)
    {
        CancelPostCutsceneHide(); // NEW: ensure no lingering post-cutscene hide

        // Images: make sure we’re not running the cursor session
        EndCursorAutoHideSession(restoreVisibility: true);

        isVideoPreview = false;
        currentVideoIndex = -1;

        StopVideoPlaybackIfAny();

        if (PreviewSurface != null)
        {
            var c = PreviewSurface.color;
            PreviewSurface.color = new Color(c.r, c.g, c.b, 1f);
            PreviewSurface.texture = image;
        } 

        currentPreviewImage = image;

        StartCoroutine(DownloadButtonDelay()); 
        //if (downloadButton != null) downloadButton.gameObject.SetActive(true); 

        ConfigurePreviewButtonForImage();

        pageText.gameObject.SetActive(false);
        previewPanelCG.Show();
        previewPanelCG.SetInteractableState(true);
        //StartCoroutine(DownloadButtonDelay()); 
        exitButton.SetActive(false);

        if (simpleSkipButton != null) simpleSkipButton.gameObject.SetActive(false);
        if (simpleSkipButtonFaded != null) simpleSkipButtonFaded.SetActive(false);
    }

    private void ShowPreviewVideo(int videoIndex)
    {
        CancelPostCutsceneHide(); // NEW: ensure clean state

        if (videoIndex < 0 || videoIndex >= galleryVideoClips.Length) return;

        isVideoPreview = true;
        currentVideoIndex = videoIndex;
        currentPreviewImage = null; 

        if (PreviewSurface != null)
        {
            var c = PreviewSurface.color;
            PreviewSurface.color = new Color(c.r, c.g, c.b, 1f);
            PreviewSurface.texture = videoRenderTexture;
        }

        ConfigurePreviewButtonForVideo();

        StopVideoPlaybackIfAny();

        // NEW: kill any BGM/ambience before cutscene audio starts
        //StopBgmForVideoIfNeeded();

        // NEW: centralized hard-stop for ambience & song
        CMD_DatabaseExtension_Audio.StopSongAndAmbienceNow(); 

        if (videoPlayer != null)
        {
            videoPlayer.targetTexture = videoRenderTexture;
            videoPlayer.isLooping = false;
            videoPlayer.clip = galleryVideoClips[videoIndex];
            videoPlayer.Play();
            videoPlayer.loopPointReached -= OnVideoEnded;
            videoPlayer.loopPointReached += OnVideoEnded;
        }

        pageText.gameObject.SetActive(false);
        previewPanelCG.Show();
        previewPanelCG.SetInteractableState(true);
        //StartCoroutine(DownloadButtonDelay()); 
        exitButton.SetActive(false);

        StartCoroutine(DownloadButtonDelay()); 
        // <--- enable Download for videos as well
        //if (downloadButton != null) downloadButton.gameObject.SetActive(true); 

        // Start of cutscene-like playback: start hidden; user movement reveals
        BeginCursorAutoHideSession(hideImmediately: true);   // keep cursor hidden until user moves

        // Hook Skip visibility to our timer rules
        if (holdSkip != null)
        {
            holdSkip.Attach(SkipVideoNow);
            holdSkip.Show(false);
            holdSkip.OnVisibilityChanged -= OnSkipVisibilityChanged;
            holdSkip.OnVisibilityChanged += OnSkipVisibilityChanged;
        }

        if (simpleSkipButton != null) simpleSkipButton.gameObject.SetActive(false);
        if (simpleSkipButtonFaded != null) simpleSkipButtonFaded.SetActive(false);
    }

    private void OnVideoEnded(VideoPlayer vp)
    {
        StartCoroutine(FadeOutAndHidePreview(0.5f));

        // <--- enable Download for videos as well
        //if (downloadButton != null) downloadButton.gameObject.SetActive(false); 
    }

    private void SkipVideoNow()
    {
        if (!isVideoPreview) return;
        StartCoroutine(FadeOutAndHidePreview(0.5f));

        // <--- enable Download for videos as well
        if (downloadButton != null) downloadButton.gameObject.SetActive(false);
    }

    private void StopVideoPlaybackIfAny()
    {
        if (videoPlayer != null)
        {
            if (videoPlayer.isPlaying) videoPlayer.Stop();
            videoPlayer.loopPointReached -= OnVideoEnded;
            videoPlayer.targetTexture = null;
        }

        if (videoRenderTexture != null)
        {
            videoRenderTexture.Release();
        }
    }

    // Images only
    private void DownloadCurrentImage()
    {
        if (currentPreviewImage == null)
        {
            if (downloadButton != null) downloadButton.gameObject.SetActive(true);
            return;
        }

        RenderTexture renderTexture = RenderTexture.GetTemporary(
            currentPreviewImage.width,
            currentPreviewImage.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        Graphics.Blit(currentPreviewImage, renderTexture);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D texture2D = new Texture2D(currentPreviewImage.width, currentPreviewImage.height, TextureFormat.ARGB32, false);
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);

        byte[] bytes = texture2D.EncodeToPNG();
        Destroy(texture2D);

        string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

        string outPath = GetUniqueSavePath(downloadsPath, currentPreviewImage.name, ".png");

        File.WriteAllBytes(outPath, bytes);
        Debug.Log($"Image saved to {outPath}");

        // In DownloadCurrentImage() — after File.WriteAllBytes(...)
        Debug.Log($"Image saved to {outPath}");
        StartCoroutine(ResetSelectableVisual(downloadButton, downloadButtonResetDelay)); 
    }

    public void HidePreviewImage() => HidePreview();

    private void HidePreview(bool force = false)
    {
        if (isVideoPreview && !force) return;

        if (downloadButton != null) downloadButton.gameObject.SetActive(false);   // ensure hidden 

        if (skipAutoHideRoutine != null) { StopCoroutine(skipAutoHideRoutine); skipAutoHideRoutine = null; }
        if (holdSkip != null)
        {
            holdSkip.Show(false);
            holdSkip.OnVisibilityChanged -= OnSkipVisibilityChanged;
        }

        EndCursorAutoHideSession(restoreVisibility: true);

        previewPanelCG.Hide();
        previewPanelCG.SetInteractableState(false);
        pageText.gameObject.SetActive(true);
        exitButton.SetActive(true); 
        downloadButton.gameObject.SetActive(false);

        if (isVideoPreview)
        {
            if (PreviewSurface != null) PreviewSurface.texture = null;

            if (videoPlayer != null)
            {
                if (videoPlayer.isPlaying) videoPlayer.Stop();
                videoPlayer.loopPointReached -= OnVideoEnded;
                videoPlayer.targetTexture = null;
            }

            if (videoRenderTexture != null)
            {
                var prev = RenderTexture.active;
                RenderTexture.active = videoRenderTexture;
                GL.Clear(true, true, Color.clear);
                RenderTexture.active = prev;
            }

            isVideoPreview = false;
            currentVideoIndex = -1;
        }

        if (simpleSkipButton != null) simpleSkipButton.gameObject.SetActive(false);
        if (simpleSkipButtonFaded != null) simpleSkipButtonFaded.SetActive(false);
    }

    // NEW: Unified entry point
    private void DownloadCurrent()
    {
        if (isVideoPreview)
        {
            StartCoroutine(DownloadCurrentVideo());
        }
        else
        {
            DownloadCurrentImage();
        }
    } 

    public void ToNextPage()
    {
        if (selectedPage < maxPages)
            LoadPage(selectedPage + 1);
    }

    public void ToPreviousPage()
    {
        if (selectedPage > 1)
            LoadPage(selectedPage - 1);
    }

    private void UpdatePageText()
    {
        pageText.text = $"{selectedPage} / {maxPages}";
    }

    private void ConfigurePreviewButtonForImage()
    {
        if (previewButton == null) return;
        previewButton.onClick.RemoveAllListeners();
        previewButton.interactable = imageClickToClose;
        if (imageClickToClose)
            previewButton.onClick.AddListener(HidePreviewImage);
    }

    private void ConfigurePreviewButtonForVideo()
    {
        if (previewButton == null) return;

        previewButton.onClick.RemoveAllListeners();
        previewButton.interactable = true;

        // Clicking the video reveals Skip and resets the 5s timer
        previewButton.onClick.AddListener(() =>
        {
            if (holdSkip == null) return;
            holdSkip.Show(true);
            ShowCursor();          // ensure cursor is visible alongside Skip
            visibilityTimer = 0f;  // start 5s countdown for "both visible" rule
        });
    }

    private IEnumerator FadeRawImage(float startAlpha, float endAlpha, float duration)
    {
        float timer = 0f;
        Color currentColor = PreviewSurface.color;

        while (timer < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            PreviewSurface.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        PreviewSurface.color = new Color(currentColor.r, currentColor.g, currentColor.b, endAlpha);
    }

    private IEnumerator FadeOutAndHidePreview(float fade = 0.5f)
    {
        // Ensure Skip visuals are hidden and detached
        if (skipAutoHideRoutine != null) { StopCoroutine(skipAutoHideRoutine); skipAutoHideRoutine = null; }
        if (holdSkip != null)
        {
            holdSkip.Show(false);
            holdSkip.OnVisibilityChanged -= OnSkipVisibilityChanged;
        }

        // Make cursor NOT visible during fade-out (per your request)
        HideCursor();

        if (PreviewSurface != null)
        {
            float startA = PreviewSurface.color.a;
            yield return StartCoroutine(FadeRawImage(startA, 0f, fade));
        }

        // End the in-video cursor auto-hide session but DO NOT restore original visibility here
        EndCursorAutoHideSession(restoreVisibility: false);

        if (PreviewSurface != null) PreviewSurface.texture = null;
        StopVideoPlaybackIfAny();

        // Small hold to feel like a proper outro
        yield return new WaitForSeconds(2f);

        previewPanelCG.Hide();
        previewPanelCG.SetInteractableState(false);
        pageText.gameObject.SetActive(true);
        exitButton.SetActive(true);

        if (simpleSkipButton != null) simpleSkipButton.gameObject.SetActive(false);
        if (simpleSkipButtonFaded != null) simpleSkipButtonFaded.SetActive(false);

        isVideoPreview = false;
        currentVideoIndex = -1;

        // Resume previous ambience/song after the cutscene ends
        if (resumeSongAndAmbienceAfterVideo)
        {
            // true = start immediately at volumeCap (no fade); false = use original startVolume
            CMD_DatabaseExtension_Audio.ResumeLastSongAndAmbience(resumeAtFullVolume);
        } 

        // Now that the cutscene is over and the gallery grid is back,
        // briefly show the cursor again but auto-hide it a few seconds later.
        if (hideCursorAfterCutscene)
        {
            if (postCutsceneHideRoutine != null) StopCoroutine(postCutsceneHideRoutine);
            postCutsceneHideRoutine = StartCoroutine(PostCutsceneCursorHide());
        }
        else
        {
            // If you prefer it hidden immediately after the end, uncomment next line:
            // HideCursor();
            // Or if you prefer it restored immediately, uncomment:
            // Cursor.visible = true;
        }

        // <--- enable Download for videos as well
        //if (downloadButton != null) downloadButton.gameObject.SetActive(false); 
    }

    // Waits for user idle "a few seconds" after a cutscene, then hides the cursor.
    private IEnumerator PostCutsceneCursorHide()
    {
        // Show cursor first so the player can see it on the grid
        Cursor.visible = true;

        float t = 0f;
        Vector3 last = Input.mousePosition;

        while (t < hideCursorDelayAfterCutscene)
        {
            // If the user moves or presses a key, reset the idle timer
            if (Vector3.Distance(Input.mousePosition, last) >= mouseMoveRevealPixels || Input.anyKeyDown)
            {
                t = 0f;
                last = Input.mousePosition;
            }

            t += Time.unscaledDeltaTime;
            yield return null;
        }

        HideCursor();
        postCutsceneHideRoutine = null;
    }

    private void BeginCursorAutoHideSession(bool hideImmediately = true)
    {
        if (!autoHideCursor) return;

        cursorSessionActive = true;
        visibilityTimer = 0f;
        lastMousePos = Input.mousePosition;

        originalCursorVisible = Cursor.visible;
        if (hideImmediately) HideCursor();
    }

    private void EndCursorAutoHideSession(bool restoreVisibility = true)
    {
        if (!cursorSessionActive) return;
        cursorSessionActive = false;
        visibilityTimer = 0f;

        if (restoreVisibility) Cursor.visible = originalCursorVisible;
    }

    private void ShowCursor()
    {
        Cursor.visible = true;
        visibilityTimer = 0f;
        lastMousePos = Input.mousePosition;
    }

    private void HideCursor()
    {
        Cursor.visible = false;
    }

    private void CancelPostCutsceneHide()
    {
        if (postCutsceneHideRoutine != null)
        {
            StopCoroutine(postCutsceneHideRoutine);
            postCutsceneHideRoutine = null;
        }
    }

    // When Skip shows, keep cursor visible and reset timer.
    // When Skip hides, do not instantly hide the cursor; let the 2s cursor-only rule take over.
    private void OnSkipVisibilityChanged(bool visible)
    {
        if (!autoHideCursor) return;

        if (visible)
        {
            ShowCursor();
            visibilityTimer = 0f;  // begin 5s window (cursor + skip)
        }
        else
        {
            visibilityTimer = 0f;  // start fresh 2s countdown in Update (cursor-only)
        }
    }

    private static string SanitizeFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "Untitled";
        foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name.Trim();
    }

    private static string GetUniqueSavePath(string directory, string baseName, string extensionWithDot)
    {
        baseName = SanitizeFileName(baseName);
        if (string.IsNullOrWhiteSpace(extensionWithDot) || !extensionWithDot.StartsWith("."))
            extensionWithDot = ".dat";

        System.IO.Directory.CreateDirectory(directory);

        string first = System.IO.Path.Combine(directory, baseName + extensionWithDot);
        if (!System.IO.File.Exists(first)) return first;

        int i = 1;
        while (true)
        {
            string candidate = System.IO.Path.Combine(directory, $"{baseName} ({i}){extensionWithDot}");
            if (!System.IO.File.Exists(candidate)) return candidate;
            i++;
        }
    }

    private IEnumerator ResetSelectableVisual(Selectable s, float delaySeconds)
    {
        if (s == null) yield break;

        yield return new WaitForSeconds(delaySeconds);

        // If the button is still the selected object, clear selection so it won't stay tinted
        var es = EventSystem.current;
        if (es != null && es.currentSelectedGameObject == s.gameObject)
            es.SetSelectedGameObject(null);

        // Force the targetGraphic to its Normal tint (works for Color Tint transition)
        if (s.targetGraphic != null)
        {
            var cb = s.colors;
            s.targetGraphic.CrossFadeColor(cb.normalColor, 0.12f, true, true);
        }

        // Fallback nudge: briefly toggle interactable to force a visual refresh
        // (helps if something else held it in a weird state)
        s.interactable = false;
        yield return null;               // wait one frame
        s.interactable = true;
    } 
    
    private IEnumerator DownloadButtonDelay()
    { 
        yield return new WaitForSeconds(0.25f); 
        if (downloadButton != null) downloadButton.gameObject.SetActive(true); 
    }

    private IEnumerator DownloadCurrentVideo()
    {
        if (!isVideoPreview || currentVideoIndex < 0 || currentVideoIndex >= galleryVideoClips.Length)
        {
            Debug.LogWarning("[Gallery] No active video to download.");
            yield break;
        }

        var clip = galleryVideoClips[currentVideoIndex];
        if (clip == null)
        {
            Debug.LogWarning("[Gallery] VideoClip is null.");
            yield break;
        }

        // Determine the source filename to copy
        string baseFileName;
        if (clipToFileName != null && clipToFileName.TryGetValue(clip.name, out var mapped))
        {
            baseFileName = mapped; // Already includes extension
        }
        else
        {
            // Try clip.name + each extension
            baseFileName = null;
        }

        // Build StreamingAssets path
        string streamingRoot = Application.streamingAssetsPath;
        string folderPath = string.IsNullOrEmpty(streamingVideosSubfolder)
            ? streamingRoot
            : System.IO.Path.Combine(streamingRoot, streamingVideosSubfolder.Replace("\\", "/"));

        string srcPath = null;

        if (!string.IsNullOrEmpty(baseFileName))
        {
            srcPath = System.IO.Path.Combine(folderPath, baseFileName);
            srcPath = srcPath.Replace("\\", "/");
        }
        else
        {
            // Probe possible extensions
            foreach (var ext in videoFileExtensions)
            {
                string tryPath = System.IO.Path.Combine(folderPath, clip.name + ext).Replace("\\", "/");
                if (CanFileBeReadFromStreamingAssets(tryPath))
                {
                    srcPath = tryPath;
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(srcPath))
        {
            Debug.LogWarning($"[Gallery] Could not resolve a source file in StreamingAssets for clip '{clip.name}'. " +
                             $"Provide a filename map or place the original file in StreamingAssets/{streamingVideosSubfolder}.");
            yield break;
        }

        // Read bytes from StreamingAssets (note: Android needs UnityWebRequest)
        byte[] bytes = null;

#if UNITY_ANDROID && !UNITY_EDITOR
    var request = UnityEngine.Networking.UnityWebRequest.Get(srcPath);
    yield return request.SendWebRequest();
    if (request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
    {
        Debug.LogWarning($"[Gallery] Failed to read video from StreamingAssets (Android): {request.error}");
        yield break;
    }
    bytes = request.downloadHandler.data;
#else
        try
        {
            // On desktop/console, StreamingAssets is a normal folder
            bytes = System.IO.File.ReadAllBytes(srcPath);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[Gallery] Failed to read video from StreamingAssets: {ex.Message}");
            yield break;
        }
#endif

        if (bytes == null || bytes.Length == 0)
        {
            Debug.LogWarning("[Gallery] No data read for video file.");
            yield break;
        }

        // Destination: user's Downloads
        string downloadsPath = System.IO.Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads");

        // Keep original filename if we had it; otherwise generate from clip name + detected extension
        string fileNameOut;
        if (clipToFileName != null && clipToFileName.TryGetValue(clip.name, out var mappedOut))
        {
            fileNameOut = mappedOut;
        }
        else
        {
            string ext = System.IO.Path.GetExtension(srcPath);
            if (string.IsNullOrEmpty(ext))
            {
                // Fallback
                ext = ".mp4";
            }
            fileNameOut = $"{clip.name}{ext}";
        }

        // Split into base + ext and get a unique path
        string baseName = System.IO.Path.GetFileNameWithoutExtension(fileNameOut);
        string extWithDot = System.IO.Path.GetExtension(fileNameOut);
        string outPath = GetUniqueSavePath(downloadsPath, baseName, string.IsNullOrEmpty(extWithDot) ? ".mp4" : extWithDot);

        //string outPath = System.IO.Path.Combine(downloadsPath, fileNameOut);

        try
        {
            System.IO.File.WriteAllBytes(outPath, bytes);
            Debug.Log($"[Gallery] Video saved to {outPath}");

            // In DownloadCurrentVideo() — after File.WriteAllBytes(...)
            Debug.Log($"[Gallery] Video saved to {outPath}");
            StartCoroutine(ResetSelectableVisual(downloadButton, downloadButtonResetDelay)); 
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[Gallery] Failed saving video: {ex.Message}");
        }
    }

    // Returns true on platforms where we can read directly from StreamingAssets with File APIs.
    // On Android, StreamingAssets is inside the APK -> must use UnityWebRequest.
    private bool CanFileBeReadFromStreamingAssets(string path)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    return true; // We can 'read' via UnityWebRequest; existence will be checked when we request it
#else
        return System.IO.File.Exists(path);
#endif
    } 
}
