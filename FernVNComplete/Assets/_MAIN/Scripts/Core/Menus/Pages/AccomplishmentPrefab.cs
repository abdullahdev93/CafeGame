using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AccomplishmentPrefab : MonoBehaviour
{
    [Header("Accomplishment Components")]
    [TextArea(3, 10)]
    public string titleText; // The title of the accomplishment
    public TextMeshProUGUI titleTextUI; // The UI component that displays the title

    [TextArea(3, 10)]
    public string descriptionText; // The description of the accomplishment
    public TextMeshProUGUI descriptionTextUI; // The UI component that displays the description

    [Header("Check Images")]
    public Image highlightedCheckIcon; // Highlighted Check image 

    [Header("Accomplishment State")]
    public bool isAccomplished; // Boolean to determine which image is active

    private void Start()
    {
        UpdateAccomplishmentUI();
    }

    private void Update()
    {
        UpdateAccomplishmentUI(); 
    }

    public void UpdateAccomplishmentUI()
    {
        if (titleTextUI != null)
        {
            titleTextUI.text = titleText;
        }

        if (descriptionTextUI != null)
        {
            descriptionTextUI.text = descriptionText;
        }

        if (highlightedCheckIcon != null)
        {
            highlightedCheckIcon.gameObject.SetActive(isAccomplished);
        }
    }

    public void SetAccomplishmentState(bool accomplished)
    {
        if (isAccomplished != accomplished)
        {
            isAccomplished = accomplished;

            // Notify the KudosMenu to update its state
            KudosMenu.Instance.UpdateAccomplishedTasks();
            UpdateAccomplishmentUI();
        }
    }
}
