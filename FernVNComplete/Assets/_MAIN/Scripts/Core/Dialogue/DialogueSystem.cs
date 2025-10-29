using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
//using UnityEngine.UIElements;
using UnityEngine.UI; 

namespace DIALOGUE
{
    public class DialogueSystem : MonoBehaviour
    {
        [SerializeField] private DialogueSystemConfigurationSO _config;
        public DialogueSystemConfigurationSO config => _config;

        public DialogueContainer dialogueContainer = new DialogueContainer();
        public ConversationManager conversationManager { get; private set; }
        private TextArchitect architect;
        public AutoReader autoReader { get; private set; }
        [SerializeField] private CanvasGroup mainCanvas;

        public static DialogueSystem instance { get; private set; }

        public delegate void DialogueSystemEvent();
        public event DialogueSystemEvent onUserPrompt_Next;
        public event DialogueSystemEvent onClear;

        public bool isRunningConversation => conversationManager.isRunning;

        public DialogueContinuePrompt prompt;
        private CanvasGroupController cgController;

        public GameObject loadingScreen;

        public bool loadingShow;

        //public AudioSource dialogueBoxSpeaker; // For storing last played voice clip
        //private Button dialogueBoxSpeakerButton;

        public GameObject dialogueBoxSpeaker; // Drag GameObject with AudioSource + Button in Inspector

        private AudioSource dialogueBoxSpeakerAudio;
        private Button dialogueBoxSpeakerButton;

        public Button continueButton; 


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Initialize();
            }
            else
                DestroyImmediate(gameObject);
        }

        bool _initialized = false;
        private void Initialize()
        {
            if (_initialized)
                return;

            // Ensure the architect uses the font size from the config
            architect = new TextArchitect(dialogueContainer.dialogueText, TABuilder.BuilderTypes.Typewriter)
            {
                fontSize = _config.defaultDialogueFontSize * _config.dialogueFontScale
            };

            //architect = new TextArchitect(dialogueContainer.dialogueText, TABuilder.BuilderTypes.Typewriter);
            conversationManager = new ConversationManager(architect);

            cgController = new CanvasGroupController(this, mainCanvas);
            dialogueContainer.Initialize();

            autoReader = GetComponent<AutoReader>();
            if (autoReader != null)
                autoReader.Initialize(conversationManager);

            if (dialogueBoxSpeaker != null)
            {
                dialogueBoxSpeakerAudio = dialogueBoxSpeaker.GetComponent<AudioSource>();
                //if (dialogueBoxSpeakerAudio == null)
                    //dialogueBoxSpeakerAudio = dialogueBoxSpeaker.AddComponent<AudioSource>();

                //dialogueBoxSpeakerAudio.playOnAwake = false;

                dialogueBoxSpeakerButton = dialogueBoxSpeaker.GetComponent<Button>();
                //if (dialogueBoxSpeakerButton == null)
                    //dialogueBoxSpeakerButton = dialogueBoxSpeaker.AddComponent<Button>();

                // Start with button disabled
                dialogueBoxSpeakerButton.interactable = false;
            }
            else
            {
                Debug.Log("DialogueBoxSpeaker GameObject is not assigned in the DialogueSystem.");
            } 

            //_initialized = true; 
        }

        public void OnUserPrompt_Next()
        {
            onUserPrompt_Next?.Invoke();

            // Clear saved voice clip
            if (dialogueBoxSpeakerAudio != null && dialogueBoxSpeakerAudio.clip != null)
            {
                dialogueBoxSpeakerAudio.Stop();
                dialogueBoxSpeakerAudio.clip = null;

                if (dialogueBoxSpeakerButton != null)
                    dialogueBoxSpeakerButton.interactable = false;

                Debug.Log("[DialogueSystem] Cleared saved voice clip and disabled replay button.");
            } 

            if (autoReader != null && autoReader.isOn)
                autoReader.Disable();
        }

        public void OnSystemPrompt_Next()
        {
            onUserPrompt_Next?.Invoke();
        }

        public void OnSystemPrompt_Clear()
        {
            onClear?.Invoke();
        }

        public void OnStartViewingHistory()
        {
            prompt.Hide();
            autoReader.allowToggle = false;
            conversationManager.allowUserPrompts = false;

            if (autoReader.isOn)
                autoReader.Disable();
        }

        public void OnStopViewingHistory()
        {
            prompt.Show();
            autoReader.allowToggle = true;
            conversationManager.allowUserPrompts = true;
        }

        public void ApplySpeakerDataToDialogueContainer(string speakerName)
        {
            Character character = CharacterManager.instance.GetCharacter(speakerName);
            CharacterConfigData config = character != null ? character.config : CharacterManager.instance.GetCharacterConfig(speakerName);

            ApplySpeakerDataToDialogueContainer(config);
        }

        public void ApplySpeakerDataToDialogueContainer(CharacterConfigData config)
        {
            //Set Dialogue details
            dialogueContainer.SetDialogueColor(config.dialogueColor);
            dialogueContainer.SetDialogueFont(config.dialogueFont);
            float fontSize = this.config.defaultDialogueFontSize * this.config.dialogueFontScale * config.dialogueFontScale;
            dialogueContainer.SetDialogueFontSize(fontSize);
            architect.fontSize = fontSize; // Ensure architect uses the same font size 

            //Set name details
            dialogueContainer.nameContainer.SetNameColor(config.nameColor); 
            dialogueContainer.nameContainer.SetNameFont(config.nameFont);
            float nameFontSize = this.config.defaultNameFontSize * config.nameFontScale;
            dialogueContainer.nameContainer.SetNameFontSize(nameFontSize); 
        }

        /*public void ShowSpeakerName(string speakerName = "")
        {
            if (speakerName.ToLower() != "narrator")
                dialogueContainer.nameContainer.Show(speakerName);

            else if (speakerName.ToLower() == "firstName")
                dialogueContainer.nameContainer.Show(InputPanel.instance.firstName); 

            else
            {
                HideSpeakerName();
                dialogueContainer.nameContainer.nameText.text = "";
            }

        }*/

        public void ShowSpeakerName(string speakerName = "")
        {
            if (string.IsNullOrEmpty(speakerName))
            {
                HideSpeakerName();
                return;
            }

            string resolvedName = speakerName;

            // Resolve variable from VariableStore if prefixed with $
            if (speakerName.StartsWith("$"))
            {
                string variableKey = speakerName.Substring(1); // Remove $
                if (VariableStore.TryGetValue(variableKey, out object value))
                {
                    resolvedName = value.ToString();
                }
                else
                {
                    Debug.LogWarning($"[DialogueSystem] Variable '{variableKey}' not found."); 
                    resolvedName = ""; // fallback
                }
            }

            if (resolvedName.ToLower() == "narrator")
            {
                HideSpeakerName();
                dialogueContainer.nameContainer.nameText.text = "";
            }
            else if (resolvedName.ToLower() == "firstname")
            {
                resolvedName = InputPanel.instance.firstName;
                dialogueContainer.nameContainer.Show(resolvedName);
                Debug.Log($"[DialogueSystem] Showing name: {resolvedName}"); 
            }
            else
            {
                dialogueContainer.nameContainer.Show(resolvedName);
            }
        } 

        public void HideSpeakerName() => dialogueContainer.nameContainer.Hide();

        public Coroutine Say(string speaker, string dialogue)
        {
            List<string> conversation = new List<string>() { $"{speaker} \"{dialogue}\"" };
            return Say(conversation);
        }

        public Coroutine Say(List<string> lines, string filePath = "")
        {
            Conversation conversation = new Conversation(lines, file: filePath);
            return conversationManager.StartConversation(conversation);
        }

        public Coroutine Say(Conversation conversation)
        {
            return conversationManager.StartConversation(conversation);
        }

        public AudioSource GetCurrentAudioSource()
        {
            // Assuming DialogueSystem or the current scene has an AudioSource for dialogue
            // You need to ensure this returns the appropriate AudioSource for playing the current dialogue voice clip
            return dialogueContainer.dialogueAudioSource;  // Or wherever the current AudioSource is stored
        } 

        public void ShowLoading()
        {
            loadingScreen.SetActive(true);
            loadingShow = true;
        }

        public void SetCharacterNameColor(string characterName, Color newColor)
        {
            // Apply it immediately
            if (characterName.ToLower() == "firstname" && dialogueContainer.nameContainer != null)
            {
                dialogueContainer.nameContainer.SetNameColor(newColor);
            }

            // 2. Save to config
            var config = CharacterManager.instance.GetCharacterConfig(characterName);
            if (config != null)
            {
                config.nameColor = newColor;
            }

            // 3. Save persistently to PlayerPrefs 
            string key = $"NameColor_{characterName.ToLower()}";
            PlayerPrefs.SetString(key, ColorUtility.ToHtmlStringRGBA(newColor));
            PlayerPrefs.Save();
        }

        public void PlaySavedVoiceClip()
        {
            if (dialogueBoxSpeakerAudio != null && dialogueBoxSpeakerAudio.clip != null)
            {
                dialogueBoxSpeakerAudio.Play();
                Debug.Log("[DialogueSystem] Replaying saved voice clip.");
            }
            else
            {
                Debug.Log("[DialogueSystem] No voice clip saved.");
            }
        } 



        public bool isVisible => cgController.isVisible;
        public Coroutine Show(float speed = 1f, bool immediate = false) => cgController.Show(speed, immediate);

        public Coroutine Hide(float speed = 1f, bool immediate = false) => cgController.Hide(speed, immediate);
    }

}