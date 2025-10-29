using UnityEngine;
using TMPro;
using System.Collections;

namespace DIALOGUE
{
    [System.Serializable]
    public class DialogueContainer
    {
        public GameObject root;
        public NameContainer nameContainer;
        public TextMeshProUGUI dialogueText;
        public AudioSource dialogueAudioSource;  // Added AudioSource for voice playback

        private CanvasGroupController cgController;

        public void SetDialogueColor(Color color) => dialogueText.color = color;
        public void SetDialogueFont(TMP_FontAsset font) => dialogueText.font = font;
        public void SetDialogueFontSize(float size) => dialogueText.fontSize = size;

        private bool initialized = false;
        public void Initialize()
        {
            if (initialized)
                return;

            cgController = new CanvasGroupController(DialogueSystem.instance, root.GetComponent<CanvasGroup>());

            // Ensure the dialogueAudioSource is properly initialized or referenced
            if (dialogueAudioSource == null)
            {
                dialogueAudioSource = root.GetComponent<AudioSource>();
                if (dialogueAudioSource == null)
                {
                    // If there is no AudioSource on the root, add one
                    dialogueAudioSource = root.AddComponent<AudioSource>();
                }
            }
        }

        public bool isVisible => cgController.isVisible;
        public Coroutine Show(float speed = 1f, bool immediate = false) => cgController.Show(speed, immediate);
        public Coroutine Hide(float speed = 1f, bool immediate = false) => cgController.Hide(speed, immediate);
    }
}