using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuzzStopMenu : MenuPage
{
    public static BuzzStopMenu Instance;

    [Header("BuzzStop GameObject")]
    [Tooltip("Assign the BuzzStop GameObject in the Inspector.")]
    public GameObject buzzStop; // Assign this in the Unity Inspector

    [Header("Total Review Score Components")]
    public Image[] totalReviewStars; // Array of star images for the Total Review Score
    public Color grayStarColor = new Color(0.3f, 0.3f, 0.3f, 1f); // Darker gray color for unused stars 

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (buzzStop == null)
        {
            Debug.LogError("BuzzStop GameObject is not assigned in the Inspector!");
            return;
        }

        float score = CalculateBuzzStopScore();
        UpdateTotalReviewScore(score);
    }

    public float CalculateBuzzStopScore()
    {
        if (buzzStop == null)
        {
            Debug.LogError("BuzzStop GameObject is not assigned in the Inspector!");
            return 0f;
        }

        // Get all "Review" GameObjects under "BuzzStop" (include inactive)
        ReviewPrefab[] reviews = buzzStop.GetComponentsInChildren<ReviewPrefab>(true);

        if (reviews == null || reviews.Length == 0)
        {
            Debug.LogWarning("No reviews found under BuzzStop.");
            return 0f;
        }

        // Calculate total points and stars
        int totalPoints = reviews.Length * 5; // Each review is worth 5 points
        int totalStarCount = 0;

        foreach (var review in reviews)
        {
            totalStarCount += review.starCount;
        }

        // Prevent division by zero
        if (totalPoints == 0)
        {
            Debug.LogWarning("Total points is zero, returning score as zero.");
            return 0f;
        }

        // Calculate the score as a percentage (0-100)
        float score = (totalStarCount / (float)totalPoints) * 100f;
        Debug.Log($"BuzzStop Score: {score}");
        return score;
    }

    private void UpdateTotalReviewScore(float score)
    {
        // Determine the number of full stars to display
        int fullStars = 0;
        if (score > 0 && score <= 20)
        {
            fullStars = 0;
        }
        else if (score > 20 && score <= 40)
        {
            fullStars = 1;
        }
        else if (score > 40 && score <= 60)
        {
            fullStars = 2;
        }
        else if (score > 60 && score <= 80)
        {
            fullStars = 3;
        }
        else if (score > 80 && score <= 90)
        {
            fullStars = 4;
        }
        else if (score > 90 && score <= 100)
        {
            fullStars = 5;
        }

        // Update star colors
        for (int i = 0; i < totalReviewStars.Length; i++)
        {
            if (i < fullStars)
            {
                totalReviewStars[i].color = Color.white; // Full star remains its default color
            }
            else
            {
                totalReviewStars[i].color = grayStarColor; // Gray out unused stars
            }

            totalReviewStars[i].enabled = true; // Ensure all stars are visible
        }

        Debug.Log($"Total Review Score: {fullStars} Stars");
    }
}
