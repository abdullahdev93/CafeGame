using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoteDetailPrefab : MonoBehaviour
{
    [Header("Note Components")]
    [TextArea(3, 10)]
    public string descriptionText; // The description of the accomplishment
    public TextMeshProUGUI descriptionTextUI; // The UI component that displays the description

    [Header("Button Component")]
    public Button noteButton; // The button to interact with the prefab

    private void Start()
    {
        noteButton = GetComponent<Button>();
        if (noteButton != null)
        {
            noteButton.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogWarning("Note Button is not assigned in the inspector.");
        }

        UpdateNotesUI();
    }

    public void UpdateNotesUI()
    {
        if (descriptionTextUI != null)
        {
            descriptionTextUI.text = descriptionText;
        }
    }

    public void OnButtonClick()
    {
        if (Notes.Instance != null)
        {
            Notes.Instance.NavigateToNote(this);
        }
        else
        {
            Debug.LogWarning("Notes instance is null. Cannot navigate.");
        }
    } 
}
