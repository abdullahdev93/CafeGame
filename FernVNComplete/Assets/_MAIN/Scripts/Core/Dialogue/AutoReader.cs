using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using History;

namespace DIALOGUE
{
    public class AutoReader : MonoBehaviour
    {
        private DialogueContinuePrompt dialoguePrompt => DialogueContinuePrompt.activeDialoguePrompt;  

        public static AutoReader reader; 
         
        private const int DEFAULT_CHARACTERS_READ_PER_SECOND = 18;
        private const float READ_TIME_PADDING = 0.5f;
        private const float MAX_READ_TIME = 99f;
        private const float MIN_READ_TIME = 1f; 
        public float skipSpeed; //= 0.05f; 
        private const string STATUS_TEXT_AUTO = "Auto";
        private const string STATUS_TEXT_SKIP = "Skipping";
        public GameObject fastForward;
        public Animator skipEffectAnimator;
        public GameObject skipEffect; 

        private ConversationManager conversationManager;
        private TextArchitect architect => conversationManager.architect;

        private UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.instance; 

        public bool skip { get; set; } = false;
        public bool auto { get; set; } = false; 

        private bool buttonHasBeenHeld = false;
        public float speed { get; set; } = 1f; 

        //public float holdTime; 

        public bool isOn => co_running != null;
        private Coroutine co_running = null;

        [SerializeField] private TextMeshProUGUI statusText;
        [HideInInspector] public bool allowToggle = true; 

        void Awake()
        {
            if (reader == null)
            {
                reader = this; 
            } 
        }

        /*void Update()
        { 
            if (buttonHasBeenHeld) 
                SkipHoldButton(); 
        }*/

        public void Initialize(ConversationManager conversationManager)
        {
            this.conversationManager = conversationManager;

            statusText.text = string.Empty;
        }

        public void Enable()
        {
            if (isOn)
                return;

            //dialoguePrompt.Hide(); 

            co_running = StartCoroutine(AutoRead()); 
        }

        public void Disable()
        {
            if (!isOn) 
                return;

            dialoguePrompt.ShowPrompt(); 

            StopCoroutine(co_running); 
            skip = false;
            auto = false; 
            co_running = null;
            statusText.text = string.Empty;
        }

        private IEnumerator AutoRead()
        {
            //Do nothing if there is no conversation to monitor.
            if (!conversationManager.isRunning)
            {
                Disable();
                yield break;
            }

            if (!architect.isBuilding && architect.currentText != string.Empty)
                DialogueSystem.instance.OnSystemPrompt_Next();

            while (conversationManager.isRunning)
            {
                //Read and wait
                if (!skip)
                {
                    while (!architect.isBuilding && !conversationManager.isWaitingOnAutoTimer)
                        yield return null;

                    yield return new WaitForSeconds(0.02f);

                    float timeStarted = Time.time;

                    while (architect.isBuilding || conversationManager.isWaitingOnAutoTimer)
                        yield return null;

                    float timeToRead = Mathf.Clamp(((float)architect.tmpro.textInfo.characterCount / DEFAULT_CHARACTERS_READ_PER_SECOND), MIN_READ_TIME, MAX_READ_TIME);
                    timeToRead = Mathf.Clamp((timeToRead - (Time.time - timeStarted)), MIN_READ_TIME, MAX_READ_TIME);
                    timeToRead = (timeToRead / speed) + READ_TIME_PADDING;

                    Debug.Log($"wait [{timeToRead}s] for '{architect.currentText}'");

                    yield return new WaitForSeconds(timeToRead);
                }
                //Skip
                else
                {
                    //StartCoroutine(DialogueSkipSpeed(skipSpeed)); 
                    architect.ForceComplete();
                    yield return new WaitForSeconds(skipSpeed/*0.05f*/); 
                }

                DialogueSystem.instance.OnSystemPrompt_Next();
            }

            Disable();
        } 

        //public IEnumerator DialogueSkipSpeed(float skipSpeed)
        //{
            //architect.ForceComplete();
            //yield return new WaitForSeconds(skipSpeed/*0.05f*/); 
        //}

        public void Toggle_Auto()
        {
            if (HistoryManager.instance.isViewingHistory)
            {
                UIConfirmationMenu.instance.Show("You cannot auto while viewing history.", new UIConfirmationMenu.ConfirmationButton("Okay", null));
                return;
            }

            if (!allowToggle)
                return; 


            bool prevState = skip;
            skip = false;

            if (prevState)
                Enable();

            else
            {
                if (!isOn)
                    Enable();
                else
                    Disable();
            }

            if (isOn)
                statusText.text = STATUS_TEXT_AUTO;
        }

        public void Toggle_Skip()
        {
            if (HistoryManager.instance.isViewingHistory)
            {
                UIConfirmationMenu.instance.Show("You cannot skip while viewing history.", new UIConfirmationMenu.ConfirmationButton("Okay", null));
                return;
            } 

            if (!allowToggle)
                return;

            bool prevState = skip;
            skip = true;

            if (!prevState)
                Enable();

            else
            {
                if (!isOn)
                    Enable();
                else
                    Disable(); 

                    //Disable();
                //fastForward.SetActive(false); 
            }

            if (isOn)
            {
                skipEffect.SetActive(true); 
                //fastForward.SetActive(true);
                skipEffectAnimator.SetTrigger("SkipAnimation"); 
                statusText.text = STATUS_TEXT_SKIP; 
            } 

            else
            {
                //fastForward.SetActive(false); 
                skipEffect.SetActive(false);  
            }
                //fastForward.SetActive(true); 
                //statusText.text = STATUS_TEXT_SKIP;
        } 

        /*public void SkipHoldButton()
        {
            buttonHasBeenHeld = true; 

            holdTime -= Time.deltaTime;

            if (holdTime <= 0)
                holdTime = 0;

            if (holdTime <= 0)
                //Toggle_Skip(); 
                //skip = true; 
        }*/
    }
}