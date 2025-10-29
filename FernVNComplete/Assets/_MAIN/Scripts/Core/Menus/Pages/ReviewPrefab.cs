using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ReviewPrefab : MonoBehaviour
{
    [Header("Review Components")]
    [TextArea(3, 10)] // Makes the title field larger in the Inspector
    public string titleText; // The title of the review as a string
    public TextMeshProUGUI titleTextUI; // The UI component that displays the title

    [TextArea(3, 10)] // Makes the description field larger in the Inspector
    public string descriptionText; // The description of the review as a string
    public TextMeshProUGUI descriptionTextUI; // The UI component that displays the description

    public Image[] starImages; // Array of star images for the rating
    //public Color grayStarColor = Color.gray; // Color for gray stars 
    public Color grayStarColor = new Color(0.3f, 0.3f, 0.3f, 1f); // Darker gray color for unused stars 

    public int starCount;

    public void Start()
    {
        SetReview();
    }

    public void SetReview()
    {
        // Set title and description text
        if (titleTextUI != null) titleTextUI.text = titleText;
        if (descriptionTextUI != null) descriptionTextUI.text = descriptionText;

        // Ensure the star count is between 0 and the max number of stars
        starCount = Mathf.Clamp(starCount, 0, starImages.Length);

        // Update star colors
        for (int i = 0; i < starImages.Length; i++)
        {
            if (i < starCount)
            {
                starImages[i].color = Color.white; // Full star remains its default color
            }
            else
            {
                starImages[i].color = grayStarColor; // Gray out unused stars
            }

            starImages[i].enabled = true; // Ensure all stars are visible
        }
    }

    public void ResetReview()
    {
        titleText = string.Empty;
        if (titleTextUI != null) titleTextUI.text = titleText;

        descriptionText = string.Empty;
        if (descriptionTextUI != null) descriptionTextUI.text = descriptionText;

        foreach (var star in starImages)
        {
            star.enabled = false; // Hide all stars
        }
    }
}
