using UnityEngine;
using UnityEngine.UI;

public class CustomizationPanel : MonoBehaviour
{
    public string category;
    public Button[] optionButtons;
    public CharacterCustomizationData customizationData;

    public Reko.ColorPicker.ColorPicker colorPicker; // Reference to ColorPicker
    public Image[] coloredOptionImages; // These should match the optionButtons visually 

    private void Start()
    {
        if (customizationData == null)
        {
            Debug.LogError("CharacterCustomizationData is missing!");
            return;
        }

        // Load and apply saved color for this category
        customizationData.LoadCategoryColor(category);
        Color savedColor = customizationData.GetCategoryColor(category);

        // Set color picker to saved color (this will invoke the listener too)
        if (colorPicker != null)
        {
            colorPicker.SelectedColor = savedColor;
            colorPicker.OnColorChanged.AddListener(OnColorChanged); // Listen for color changes
        }

        // Apply color visually to all customization options in this panel
        OnColorChanged(savedColor); 

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            optionButtons[i].onClick.AddListener(() => ApplyCustomization(index));
        }

        int savedIndex = customizationData.GetSelectedIndex(category);
        ApplyCustomization(savedIndex);

        //colorPicker.OnColorChanged.AddListener(OnColorChanged);

        // Apply saved color on load
        //Color savedColor = customizationData.GetCategoryColor(category);
        //colorPicker.SelectedColor = savedColor;
        //OnColorChanged(savedColor); // Apply color to options 
    }

    public void ApplyCustomization(int index)
    {
        Debug.Log($"ApplyCustomization called for category: {category}, index: {index}");

        customizationData.SetAppearance(category, index);
        if (FindObjectOfType<CharacterAppearance>() != null)
        {
            FindObjectOfType<CharacterAppearance>().SetAppearance(category, index);
        }
        else
        {
            Debug.LogError("CharacterAppearance script not found in scene!");
        }
    }

    private void OnColorChanged(Color newColor)
    {
        customizationData.SetCategoryColor(category, newColor);
        customizationData.SaveCategoryColor(category); 

        foreach (Image img in coloredOptionImages)
        {
            img.color = newColor; // Apply the selected color to all customization images
        }
    } 

}