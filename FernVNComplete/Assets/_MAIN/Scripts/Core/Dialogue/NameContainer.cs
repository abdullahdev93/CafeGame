using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

namespace DIALOGUE
{
    [System.Serializable] 
    public class NameContainer
    {
        [SerializeField] private GameObject root;
        [field:SerializeField] public TextMeshProUGUI nameText { get; private set; }
        [SerializeField] private RectTransform containerRect; // RectTransform for dynamic resizing

        private const float MIN_WIDTH = 50f;
        private const float MAX_WIDTH = 400f;
        private const float WIDTH_PADDING = 20f; 

        public void Show(string nameToShow = "")
        {
            root.SetActive(true);

            if (!string.IsNullOrEmpty(nameToShow))
            {
                nameText.text = nameToShow;
                AdjustContainerSize();
            } 

            //if (nameToShow != string.Empty)
            //nameText.text = nameToShow;
        }

        public void Hide()
        {
            root.SetActive(false);
        }

        public void SetNameColor(Color color) => nameText.color = color;
        public void SetNameFont(TMP_FontAsset font) => nameText.font = font;
        public void SetNameFontSize(float size) => nameText.fontSize = size;

        private void AdjustContainerSize()
        {
            if (containerRect == null || nameText == null) return;

            // Force update of text to get the correct dimensions
            nameText.ForceMeshUpdate();

            // Calculate the preferred width of the name text
            float preferredWidth = nameText.preferredWidth + WIDTH_PADDING;

            // Clamp the width to ensure it stays within reasonable bounds
            float finalWidth = Mathf.Clamp(preferredWidth, MIN_WIDTH, MAX_WIDTH);

            // Apply the calculated width to the container
            containerRect.sizeDelta = new Vector2(finalWidth, containerRect.sizeDelta.y);
        }
    }
}