using CHARACTERS;
using TMPro;
using UnityEngine;

namespace DIALOGUE
{
    [CreateAssetMenu(fileName = "Dialogue System Configuration", menuName = "Dialogue System/Dialogue Configuration Asset")]
    public class DialogueSystemConfigurationSO : ScriptableObject
    {
        //public static DialogueSystemConfigurationSO activeDialogueConfig; 

        public const float DEFAULT_FONTSIZE_DIALOGUE = 18;
        public const float DEFAULT_FONTSIZE_NAME = 22;
        //public float DEFAULT_FONTSCALE_DIALOGUE = 1f; 

        public CharacterConfigSO characterConfigurationAsset;

        public Color defaultTextColor = Color.white;
        public TMP_FontAsset defaultFont;

        public float dialogueFontScale; //DEFAULT_FONTSIZE_DIALOGUE; 
        public float defaultNameFontSize = DEFAULT_FONTSIZE_NAME;
        public float defaultDialogueFontSize = DEFAULT_FONTSIZE_DIALOGUE;

        //private void Awake()
        //{
            //if (activeDialogueConfig == null)
            //{
                //activeDialogueConfig = this; 
                
            //}
        //}
    }
}