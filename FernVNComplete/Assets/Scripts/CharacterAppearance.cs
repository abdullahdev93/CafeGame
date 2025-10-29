using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI; 

public class CharacterAppearance : MonoBehaviour
{
    public static CharacterAppearance Instance;
    public CharacterCustomizationData customizationData; // Reference to ScriptableObject

    public Image headRenderer; 
    public Image bodyRenderer;
    public Image hairRenderer;
    public Image eyesRenderer;
    public Image highlightsRenderer;
    public Image eyebrowsRenderer;
    public Image noseRenderer;
    public Image mouthRenderer;
    public Image frecklesRenderer;
    public Image facialRenderer;
    public Image glassesRenderer;
    public Image makeupRenderer;
    public Image scarsRenderer;
    public Image tattoosRenderer;
    public Image earsRenderer;
    public Image tailsRenderer;
    public Image hornsRenderer;
    public Image clothesRenderer;
    public Image hatsRenderer;
    public Image accessoriesRenderer;
    public Image skinToneRenderer;

    private Dictionary<string, Image> renderers;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        renderers = new Dictionary<string, Image>
        {
            { "HEAD", headRenderer}, 
            { "BODY", bodyRenderer },
            { "HAIR", hairRenderer },
            { "EYES", eyesRenderer },
            { "HIGHLIGHTS", highlightsRenderer },
            { "EYEBROWS", eyebrowsRenderer },
            { "NOSE", noseRenderer },
            { "MOUTH", mouthRenderer },
            { "FRECKLES", frecklesRenderer },
            { "FACIAL", facialRenderer },
            { "GLASSES", glassesRenderer },
            { "MAKEUP", makeupRenderer },
            { "SCARS", scarsRenderer },
            { "TATTOOS", tattoosRenderer },
            { "EARS", earsRenderer },
            { "TAILS", tailsRenderer },
            { "HORNS", hornsRenderer },
            { "CLOTHES", clothesRenderer },
            { "HATS", hatsRenderer },
            { "ACCESSORIES", accessoriesRenderer },
            { "SKINTONE", skinToneRenderer }
        };

        LoadCustomization();
    }

    public void SaveCustomization()
    {
        foreach (var category in customizationData.customizationCategories)
        {
            PlayerPrefs.SetInt(category.categoryName, customizationData.GetSelectedIndex(category.categoryName));
        }
        PlayerPrefs.Save();
        Debug.Log("Customization saved.");
    } 

    /*public void LoadCustomization()
    {
        Debug.Log("Loading customization...");

        foreach (var category in customizationData.customizationCategories)
        {
            int savedIndex = PlayerPrefs.GetInt(category.categoryName, 0);
            customizationData.SetAppearance(category.categoryName, savedIndex); 

            if (renderers.ContainsKey(category.categoryName))
            {
                Sprite sprite = customizationData.GetSelectedSprite(category.categoryName);
                renderers[category.categoryName].sprite = sprite;

                if (sprite != null)
                {
                    Debug.Log($"Loaded sprite {sprite.name} for category {category.categoryName}");
                }
                else
                {
                    Debug.LogError($"Sprite is NULL for category {category.categoryName}");
                }
            }
        }
    }*/

    public void LoadCustomization()
    {
        Debug.Log("Loading customization...");

        foreach (var category in customizationData.customizationCategories)
        {
            int savedIndex = PlayerPrefs.GetInt(category.categoryName, 0);
            customizationData.SetAppearance(category.categoryName, savedIndex);

            if (renderers.ContainsKey(category.categoryName))
            {
                Sprite sprite = customizationData.GetSelectedSprite(category.categoryName);
                Color tint = customizationData.GetCategoryColor(category.categoryName);
                Image renderer = renderers[category.categoryName];

                renderer.sprite = sprite;
                renderer.color = tint;

                if (sprite != null)
                {
                    Debug.Log($"Loaded sprite {sprite.name} with color {tint} for category {category.categoryName}");
                }
                else
                {
                    Debug.LogError($"Sprite is NULL for category {category.categoryName}");
                }
            }
        }
    } 

    /*public void SetAppearance(string category, int index)
    {
        Debug.Log($"SetAppearance called for category: {category}, index: {index}");

        customizationData.SetAppearance(category, index);
        SaveCustomization(); 

        if (renderers.ContainsKey(category) && customizationData.GetSelectedSprite(category) != null)
        {
            renderers[category].sprite = customizationData.GetSelectedSprite(category);
            Debug.Log($"Sprite assigned: {customizationData.GetSelectedSprite(category).name} for {category}");
        }
        else
        {
            Debug.LogError($"Renderer or Sprite not found for category: {category}");
        }
    }*/

    public void SetAppearance(string category, int index)
    {
        Debug.Log($"SetAppearance called for category: {category}, index: {index}");

        customizationData.SetAppearance(category, index);
        SaveCustomization();

        if (renderers.ContainsKey(category))
        {
            Image renderer = renderers[category];
            Sprite sprite = customizationData.GetSelectedSprite(category);
            Color tint = customizationData.GetCategoryColor(category);

            if (sprite != null)
            {
                renderer.sprite = sprite;
                renderer.color = tint;
                Debug.Log($"Sprite assigned: {sprite.name} with color {tint} for {category}");
            }
            else
            {
                Debug.LogError($"Sprite is NULL for category: {category}");
            }
        }
        else
        {
            Debug.LogError($"Renderer not found for category: {category}");
        }
    } 

    public void RandomizeAppearance()
    {
        foreach (var category in customizationData.customizationCategories)
        {
            if (category.options.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, category.options.Length);
                SetAppearance(category.categoryName, randomIndex);
            }
        }

        SaveCustomization(); 
    }
} 