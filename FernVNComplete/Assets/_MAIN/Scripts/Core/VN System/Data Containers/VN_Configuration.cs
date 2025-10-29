using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class VN_Configuration
{
    public static VN_Configuration activeConfig; 

    public static string filePath => $"{FilePaths.root}vnconfig.cfg";

    public const bool ENCRYPT = false;

    //General Settings
    public bool display_fullscreen = true;  
    public string display_resolution = "1920x1080";
    public bool continueSkippingAfterChoice = false;
    public bool continueAutoAfterChoice = false;
    public bool controlAutoSave = false; 
    public int controlTextSpeed = 0;
    public int controlSkipSpeed = 0;
    public int controlAutoSpeed = 0; 
    public bool PlayTextSound = true;  
    public bool SkipDisplay = true;
    public int ShowTextSize = 0;    
    public float dialogueTextSpeed = 1f;
    public float dialogueAutoReadSpeed = 1f;
    public float dialogueSkipSpeed;
    public float dialogueFontSize = 18f;
    public bool useCelsius = false; // Default to Fahrenheit
    public bool colorBlindModeEnabled = false;
    public bool useMonoAudio = false;
    public bool useDialoguePrompt = false;
    public bool useSubtitles = false; 

    //Audio Settings
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public float voicesVolume = 1f;
    public bool musicMute = false;
    public bool sfxMute = false;
    public bool voicesMute = false;

    //Other Settings
    public float historyLogScale = 1f;

    public int bondLevelsOne, bondLevelsTwo, bondLevelsThree, bondLevelsFour, bondLevelsFive; 

    public void Load()
    {
        var ui = ConfigMenu.instance.ui;

        /*// Restore dialogue font size
        if (DialogueSystem.instance != null && DialogueSystem.instance.conversationManager != null)
        {
            DialogueSystem.instance.conversationManager.architect.fontSize = dialogueFontSize;
        } 

        // Restore skip speed when loading the config
        if (DialogueSystem.instance != null && DialogueSystem.instance.autoReader != null)
        {
            DialogueSystem.instance.autoReader.skipSpeed = dialogueSkipSpeed;
        } 

        // Restore architect speed when loading the config
        if (DialogueSystem.instance != null && DialogueSystem.instance.conversationManager != null)
        {
            DialogueSystem.instance.conversationManager.architect.speed = dialogueTextSpeed;
        }*/ 

        //ui.architectSpeed.value = dialogueTextSpeed;  

        //GENERAL SETTINGS
        //Set Window Size
        ConfigMenu.instance.SetDisplayToFullScreen(display_fullscreen);
        ui.SetButtonColors(ui.fullscreen, ui.windowed, display_fullscreen);

        //Set the screen resolution
        int res_index = 0;
        for (int i = 0; i < ui.resolutions.options.Count; i++)
        {
            string resolution = ui.resolutions.options[i].text;
            if (resolution == display_resolution)
            {
                res_index = i;
                break;
            }  
        }
        ui.resolutions.value = res_index;

        //Set continue after skipping option
        ui.SetButtonColors(ui.skippingContinue, ui.skippingStop, continueSkippingAfterChoice);

        ui.SetButtonColors(ui.textSoundOn, ui.textSoundOff, PlayTextSound);

        ui.SetButtonColors(ui.press, ui.pressAndHold, SkipDisplay);

        ui.SetButtonColors(ui.autoContinue, ui.autoStop, continueAutoAfterChoice);

        ui.SetButtonColors(ui.enableAutoSave, ui.disableAutoSave, controlAutoSave); 

        ui.SetButtonColors(ui.slowText, ui.normalText, ui.fastText, controlTextSpeed);

        ui.SetButtonColors(ui.slowSkipText, ui.normalSkipText, ui.fastSkipText, controlSkipSpeed); 

        ui.SetButtonColors(ui.slowAutoText, ui.normalAutoText, ui.fastAutoText, controlAutoSpeed);   

        ui.SetButtonColors(ui.smallText, ui.mediumText, ui.largeText, ShowTextSize);

        ui.SetButtonColors(ui.celsiusButton, ui.fahrenheitButton, useCelsius);

        ui.SetButtonColors(ui.colorBlindOn, ui.colorBlindOff, colorBlindModeEnabled);

        ui.SetButtonColors(ui.monoAudio, ui.stereoAudio, useMonoAudio);
        AudioManager.instance.SetMonoAudio(useMonoAudio);

        ui.SetButtonColors(ui.dialoguePromtDefault, ui.dialoguePromtALT, useDialoguePrompt);

        ui.SetButtonColors(ui.subtitlesOn, ui.subtitlesOff, useSubtitles); 

        //Set the value of the architect and auto reader speed
        //ui.architectSpeed.value = dialogueTextSpeed;
        //ui.autoReaderSpeed.value = dialogueAutoReadSpeed;
        //ui.theTextSize.value = dialogueTextSize; 
        //ui.skipSpeed.value = dialogueSkipSpeed; 

        //dialogueTextSpeedText.text = (ui.architectSpeed.value).ToString();
        //dialogueAutoReadSpeedText.text = (ui.autoReaderSpeed.value).ToString();
        //dialogueSkipSpeedText.text = (ui.skipSpeed.value).ToString(); 


        //Set the audio mixer volumes
        ui.musicVolume.value = musicVolume;
        ui.sfxVolume.value = sfxVolume;
        ui.voicesVolume.value = voicesVolume;
        ui.musicMute.sprite = musicMute ? ui.mutedSymbol : ui.unmutedSymbol;
        ui.sfxMute.sprite = sfxMute ? ui.mutedSymbol : ui.unmutedSymbol;
        ui.voicesMute.sprite = voicesMute ? ui.mutedSymbol : ui.unmutedSymbol;
    }

    public void Save()
    {
        FileManager.Save(filePath, JsonUtility.ToJson(this), encrypt: ENCRYPT);
    }
}
