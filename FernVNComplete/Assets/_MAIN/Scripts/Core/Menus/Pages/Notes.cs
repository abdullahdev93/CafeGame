using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class Notes : MenuPage
{
    public static Notes Instance;

    [Header("Note Tracking")]
    public List<List<GameObject>> notesParents = new List<List<GameObject>>();
    public int totalNotes => notes.Count;
    public int notesNumber = 0;

    [SerializeField] private GameObject[] panels;
    [SerializeField] private Button backButton;
    [SerializeField] private Button forwardButton;
    [SerializeField] private ScrollRect[] scrollRects; // ScrollRects for each panel
    [SerializeField] private Button resetScrollButton; // New button to reset the scroll position

    [System.Serializable]
    public class PanelSections
    {
        public GameObject personalityTraitsTitle;
        public GameObject likesTitle;
        public GameObject dislikesTitle;
        public GameObject hobbiesTitle;
        public GameObject insecuritiesTitle;
        public GameObject randomObservationsTitle;
        public GameObject guiltyPleasuresTitle;
    }

    [SerializeField] private PanelSections[] panelSections;

    [Header("Search Feature")]
    [SerializeField] private TMP_InputField searchBar;

    private int currentPanelIndex = 0;
    private List<NoteDetailPrefab> notes = new List<NoteDetailPrefab>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeNotesParents();
        UpdateNoteListForCurrentPanel();
        UpdatePanelVisibility();

        backButton.onClick.AddListener(SwitchToPreviousPanel);
        forwardButton.onClick.AddListener(SwitchToNextPanel);

        if (searchBar != null)
        {
            searchBar.onValueChanged.AddListener(OnSearchValueChanged);
        }

        if (resetScrollButton != null)
        {
            resetScrollButton.onClick.AddListener(ResetScrollPosition);
        }
    }

    private void InitializeNotesParents()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            var panel = panels[i];
            var sections = panelSections[i];

            List<GameObject> parentGroup = new List<GameObject>
            {
                sections.personalityTraitsTitle,
                sections.likesTitle,
                sections.dislikesTitle,
                sections.hobbiesTitle,
                sections.insecuritiesTitle,
                sections.guiltyPleasuresTitle,
                sections.randomObservationsTitle
            };

            notesParents.Add(parentGroup);
        }
    }

    public void UpdateNoteListForCurrentPanel()
    {
        notes.Clear();

        var activePanel = panels[currentPanelIndex];
        if (activePanel == null)
        {
            Debug.LogWarning("Active panel is null!");
            notesNumber = 0;
            return;
        }

        AssignSectionTitles();

        var noteComponents = activePanel.GetComponentsInChildren<NoteDetailPrefab>(true);
        notes.AddRange(noteComponents);

        notesNumber = notes.Count;
    }

    private void AssignSectionTitles()
    {
        var sections = panelSections[currentPanelIndex];
        Debug.Log("Section titles assigned for the active panel using predefined section references.");
    }

    private void UpdatePanelVisibility()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == currentPanelIndex);
        }

        UpdateNoteListForCurrentPanel();
    }

    private void SwitchToPreviousPanel()
    {
        currentPanelIndex = (currentPanelIndex - 1 + panels.Length) % panels.Length;
        UpdatePanelVisibility();
    }

    private void SwitchToNextPanel()
    {
        currentPanelIndex = (currentPanelIndex + 1) % panels.Length;
        UpdatePanelVisibility();
    }

    private void OnSearchValueChanged(string searchText)
    {
        FilterNotes(searchText);

        if (!string.IsNullOrEmpty(searchText))
        {
            SetPanelSectionsActive(false);
        }
        else
        {
            SetPanelSectionsActive(true);
        }
    }

    private void SetPanelSectionsActive(bool isActive)
    {
        var sections = panelSections[currentPanelIndex];
        sections.personalityTraitsTitle?.SetActive(isActive);
        sections.likesTitle?.SetActive(isActive);
        sections.dislikesTitle?.SetActive(isActive);
        sections.hobbiesTitle?.SetActive(isActive);
        sections.insecuritiesTitle?.SetActive(isActive);
        sections.guiltyPleasuresTitle?.SetActive(isActive);
        sections.randomObservationsTitle?.SetActive(isActive);
    }

    private void FilterNotes(string searchText)
    {
        foreach (var note in notes)
        {
            bool isMatch = note.descriptionText.Contains(searchText, System.StringComparison.OrdinalIgnoreCase);
            note.gameObject.SetActive(isMatch);
        }
    }

    public void NavigateToNote(NoteDetailPrefab note)
    {
        if (note == null)
        {
            Debug.LogWarning("Note is null. Cannot navigate.");
            return;
        }

        if (searchBar == null || string.IsNullOrWhiteSpace(searchBar.text))
        {
            Debug.Log("Search bar is empty. Navigation aborted.");
            return;
        }

        // Clear the search bar after clicking the note
        searchBar.text = string.Empty;

        // Find and activate the correct panel
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i].GetComponentsInChildren<NoteDetailPrefab>(true).Contains(note))
            {
                currentPanelIndex = i;
                UpdatePanelVisibility();
                break;
            }
        }

        // Reset scroll position to the top
        ScrollRect currentScrollRect = scrollRects[currentPanelIndex];
        if (currentScrollRect != null)
        {
            currentScrollRect.verticalNormalizedPosition = 1f; // Reset to the top 
            Canvas.ForceUpdateCanvases(); // Ensure layout updates before scrolling
        }
        else
        {
            Debug.LogWarning("Current ScrollRect is null. Cannot reset scroll position.");
            return;
        }

        // Scroll to the note within the active panel
        RectTransform noteRect = note.GetComponent<RectTransform>();
        RectTransform contentRect = currentScrollRect.content;

        if (noteRect != null && contentRect != null)
        {
            Canvas.ForceUpdateCanvases();

            var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(contentRect, noteRect);
            float targetY = -bounds.center.y; // Adjust for the content's Y-direction
            float normalizedPosition = Mathf.Clamp01(1.05f - (targetY / contentRect.rect.height)); 

            // Smoothly scroll to the note
            StartCoroutine(SmoothScrollTo(currentScrollRect, normalizedPosition));

            Debug.Log($"Scrolled to note: {note.descriptionText}");
        }
        else
        {
            Debug.LogWarning("Unable to scroll to the note. Ensure ScrollRect and Note RectTransforms are properly configured.");
        }
    }

    private IEnumerator SmoothScrollTo(ScrollRect scrollRect, float targetPosition)
    {
        float currentPosition = scrollRect.verticalNormalizedPosition;
        float timeElapsed = 0f;
        float duration = 0.5f; // Adjust duration for smoother or faster scrolling

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(currentPosition, targetPosition, timeElapsed / duration);
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = targetPosition;
    } 

    private void ResetScrollPosition()
    {
        if (scrollRects != null)
        {
            foreach (var scrollRect in scrollRects)
            {
                if (scrollRect != null)
                {
                    scrollRect.verticalNormalizedPosition = 1f; // Reset to the top for each panel
                }
            }
            Debug.Log("Scroll positions reset to the top for all panels.");
        }
        else
        {
            Debug.LogWarning("ScrollRects are not properly assigned. Cannot reset scroll positions.");
        }
    }
} 
