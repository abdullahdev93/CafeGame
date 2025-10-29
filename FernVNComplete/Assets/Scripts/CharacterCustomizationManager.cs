using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterCustomizationManager : MonoBehaviour
{
    //public static CharacterCustomizationManager instance; 

    public CanvasGroup[] categoryPanels;
    public Button[] panelButtons;
    public Button randomizerButton;
    public Button finishButton;

    public CharacterCustomizationData customizationData;

    private AudioSource customizationClickSound;

    private AudioSource customizationMenuSound;

    private AudioSource customizationCategorySound;

    private CustomizationPanel[] customizationPanels; 

    private int currentPanelIndex = 0;

    private void Start()
    { 
        customizationClickSound = GameObject.Find("CustomizationOptionSound").GetComponent<AudioSource>();

        customizationMenuSound = GameObject.Find("CustomizationMenuSound").GetComponent<AudioSource>();

        customizationCategorySound = GameObject.Find("CustomizationCategorySound").GetComponent<AudioSource>();

        if (customizationData == null)
        {
            Debug.LogError("CharacterCustomizationData is not assigned!");
            return;
        }

        customizationPanels = FindObjectsOfType<CustomizationPanel>(); 

        // Assign buttons to show panels
        for (int i = 0; i < panelButtons.Length; i++)
        {
            int index = i;
            panelButtons[i].onClick.AddListener(() => ShowPanel(index));
        }

        if (randomizerButton != null)
        {
            randomizerButton.onClick.AddListener(RandomizeCharacter);
        }

        if (finishButton != null)
        {
            finishButton.onClick.AddListener(ReturnToApartment);
        }

        ShowPanel(0);
    }

    private void ShowPanel(int panelIndex)
    {
        for (int i = 0; i < categoryPanels.Length; i++)
        {
            categoryPanels[i].alpha = (i == panelIndex) ? 1 : 0;
            categoryPanels[i].interactable = (i == panelIndex);
            categoryPanels[i].blocksRaycasts = (i == panelIndex);
        }

        currentPanelIndex = panelIndex;
    }

    private void RandomizeCharacter()
    {
        Debug.Log("RandomizeCharacter button clicked.");

        foreach (var category in customizationData.customizationCategories)
        {
            if (category.options.Length > 0)
            {
                int randomIndex = Random.Range(0, category.options.Length);
                Debug.Log($"Randomizing {category.categoryName} to index {randomIndex}");

                customizationData.SetAppearance(category.categoryName, randomIndex);

                if (FindObjectOfType<CharacterAppearance>() != null)
                {
                    FindObjectOfType<CharacterAppearance>().SetAppearance(category.categoryName, randomIndex);
                }
            }
            else
            {
                Debug.LogError($"No options found for {category.categoryName}");
            }

            // Now apply a random color for each category
            Color randomColor = new Color(Random.value, Random.value, Random.value);
            customizationData.SetCategoryColor(category.categoryName, randomColor);
            customizationData.SaveCategoryColor(category.categoryName); 

            // Apply the color to the UI ColorPicker and visuals if available
            foreach (CustomizationPanel panel in customizationPanels)
            {
                if (panel.category == category.categoryName && panel.colorPicker != null)
                {
                    panel.colorPicker.SelectedColor = randomColor;
                }
            } 
        }
    } 

    public void PlayCustomizationSelectionSound()
    {
        customizationClickSound.Play(); 
    } 

    public void PlayCustomizationMenuSound()
    {
        customizationMenuSound.Play(); 
    } 

    public void PlayCategorySelectionSound()
    {
        customizationCategorySound.Play(); 
    }

    private void ReturnToApartment()
    {
        PlayerPrefs.SetString("StartingFile", "ApartmentFile");
        PlayerPrefs.Save();
        SceneManager.LoadScene("VisualNovel");
    }
} 