using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DIALOGUE
{
    public class DialogueContinuePrompt : MonoBehaviour
    {
        public static DialogueContinuePrompt activeDialoguePrompt; 

        private RectTransform root;

        [SerializeField] private Animator anim;
        [SerializeField] private TextMeshProUGUI tmpro;

        public bool isShowing => anim.gameObject.activeSelf;

        public bool isAuto => AutoReader.reader.isOn; 

        public bool continuePromptOnLastCharacter;   

        // Start is called before the first frame update
        void Start()
        {
            root = GetComponent<RectTransform>();

            if (activeDialoguePrompt == null)
                activeDialoguePrompt = this; 
        }

        public void Show()
        {
            if (tmpro.text == string.Empty)
            {
                if (isShowing)
                    Hide(); 

                return;
            } 

            tmpro.ForceMeshUpdate(); 

            if (!isAuto)
            {
                anim.gameObject.SetActive(true);
                root.transform.SetParent(tmpro.transform); 
            } 
            //anim.gameObject.SetActive(true);
            //root.transform.SetParent(tmpro.transform);

            if (continuePromptOnLastCharacter)  
            {
                TMP_CharacterInfo finalCharacter = tmpro.textInfo.characterInfo[tmpro.textInfo.characterCount - 1];
                Vector3 targetPos = finalCharacter.bottomRight;
                float characterWidth = finalCharacter.pointSize * 0.5f;
                targetPos = new Vector3(targetPos.x + characterWidth, targetPos.y, 0);

                root.localPosition = targetPos; 
            } 
            
        }

        public void Hide()
        {
            anim.gameObject.SetActive(false);
        } 

        public void ShowPrompt()
        {
            anim.gameObject.SetActive(true); 
        }
    }
}