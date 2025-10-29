using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipesMenu : MonoBehaviour
{
    public static RecipesMenu Instance;

    [System.Serializable]
    public class Recipe
    {
        public string name;
        public Sprite image;
        public string description;
        public List<string> ingredients;
    }

    public List<Recipe> recipeGuide;
    public GameObject recipeUIPrefab;
    public Transform recipeContainer;

    public ScrollRect scrollRect;
    public GameObject scrollBar;
    public GameObject upArrow;
    public GameObject downArrow;
    public GameObject recipeDetailsUI;
    public TextMeshProUGUI recipeDetailsName;
    public TextMeshProUGUI recipeDetailsIngredients;

    public Button backToPause;
    public Button exitRecipeDetails; 

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
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        exitRecipeDetails.onClick.AddListener(ExitRecipeDetails);  
        PopulateRecipePanel(recipeGuide);
        UpdateArrowVisibility();
        upArrow.SetActive(false);
        downArrow.SetActive(false);
        recipeDetailsUI.SetActive(false); // Ensure the details UI is initially hidden
    }

    private void PopulateRecipePanel(List<Recipe> recipes)
    {
        foreach (Transform child in recipeContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var recipe in recipes)
        {
            GameObject recipeUI = Instantiate(recipeUIPrefab, recipeContainer);
            Image recipeImage = recipeUI.transform.Find("RecipeImage").GetComponent<Image>();
            TextMeshProUGUI recipeText = recipeUI.transform.Find("RecipeText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI description = recipeUI.transform.Find("RecipeDescription").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI recipeIngredients = recipeUI.transform.Find("RecipeIngredients").GetComponent<TextMeshProUGUI>();
            Button saveButton = recipeUI.transform.Find("SaveButton").GetComponent<Button>();

            recipeImage.sprite = recipe.image;
            recipeText.text = recipe.name;
            description.text = recipe.description;
            recipeIngredients.text = "Ingredients:\n" + string.Join("\n", recipe.ingredients);

            saveButton.onClick.AddListener(() => ViewRecipeDetails(recipe));
        }

        scrollBar.SetActive(true);
        UpdateArrowVisibility();
    }

    private void ViewRecipeDetails(Recipe recipe)
    {
        recipeDetailsName.text = recipe.name;
        recipeDetailsIngredients.text = "Ingredients: " + string.Join(", ", recipe.ingredients); 
        recipeDetailsUI.SetActive(true);
    } 

    private void ExitRecipeDetails()
    {
        recipeDetailsUI.SetActive(false); 
    }

    private void OnScrollValueChanged(Vector2 scrollPosition)
    {
        UpdateArrowVisibility();
    }

    private void UpdateArrowVisibility()
    {
        float scrollValue = scrollRect.verticalNormalizedPosition;

        upArrow.SetActive(scrollValue < 1f);
        downArrow.SetActive(scrollValue > 0f);
    }
}
