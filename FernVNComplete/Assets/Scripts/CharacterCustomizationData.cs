using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CharacterCustomizationData", menuName = "Customization/CharacterCustomizationData")]
public class CharacterCustomizationData : ScriptableObject
{
    [Serializable]
    public class CustomizationCategory
    {
        public string categoryName; // Example: "HAIR", "EYES", "CLOTHES"
        public Sprite[] options;    // List of customization sprites for this category 
        public Color selectedColor = Color.white; // NEW 
    }

    public List<CustomizationCategory> customizationCategories = new List<CustomizationCategory>();
    private Dictionary<string, int> selectedOptions = new Dictionary<string, int>();

    public void SetAppearance(string category, int index)
    {
        foreach (var cat in customizationCategories)
        {
            if (cat.categoryName == category && index < cat.options.Length)
            {
                selectedOptions[category] = index;
                return;
            }
        }
    }

    public int GetSelectedIndex(string category)
    {
        return selectedOptions.ContainsKey(category) ? selectedOptions[category] : 0;
    }

    public Sprite GetSelectedSprite(string category)
    {
        foreach (var cat in customizationCategories)
        {
            if (cat.categoryName == category && selectedOptions.ContainsKey(category) && cat.options.Length > 0)
            {
                return cat.options[selectedOptions[category]];
            }
        }
        return null;
    }

    public void SetCategoryColor(string category, Color color)
    {
        foreach (var cat in customizationCategories)
        {
            if (cat.categoryName == category)
            {
                cat.selectedColor = color;
                return;
            }
        }
    }

    public Color GetCategoryColor(string category)
    {
        foreach (var cat in customizationCategories)
        {
            if (cat.categoryName == category)
            {
                return cat.selectedColor;
            }
        }
        return Color.white;
    }

    public void SaveCategoryColor(string category)
    {
        Color color = GetCategoryColor(category);
        PlayerPrefs.SetString(category + "_Color", ColorUtility.ToHtmlStringRGBA(color));
    }

    public void LoadCategoryColor(string category)
    {
        if (PlayerPrefs.HasKey(category + "_Color"))
        {
            string hex = PlayerPrefs.GetString(category + "_Color");
            ColorUtility.TryParseHtmlString("#" + hex, out Color loadedColor);
            SetCategoryColor(category, loadedColor);
        }
    } 
}