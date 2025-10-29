using System.Collections;
using UnityEngine;

public class TABuilder_Typewriter : TABuilder
{
    private VN_Configuration config => VN_Configuration.activeConfig; 

    //bool typingSoundEffectisLopping = false; 

    AudioSource typingSound = GameObject.Find("TypingSound").GetComponent<AudioSource>();

    //private TextArchitect textArchitect => TextArchitect.instance; 

    private bool textSound => config.PlayTextSound;   

    public override Coroutine Build()
    {
        Prepare();

        if (textSound)
        {
            typingSound.Play(); 
            typingSound.loop = true;
        } 
        //typingSound.Play();
        //typingSound.loop = true; 
        //PlaySoundOnBuild(typingSoundEffect); 
        return architect.tmpro.StartCoroutine(Building()); 
    }

    //public void PlaySoundOnBuild(AudioSource typeSound, bool isLooping) 
    //{
        //typingSoundEffect = typeSound;
        //if (!isLooping)
            //return; 

        //else 
        //{
            //typeSound.Play();
            //typeSound.loop = true; 
        //} 
    //} 

    public override void ForceComplete()
    {
        architect.tmpro.ForceMeshUpdate();
        architect.tmpro.maxVisibleCharacters = architect.tmpro.textInfo.characterCount;
        typingSound.Stop(); 
        typingSound.loop = false; 
    }

    private void Prepare()
    {
        architect.tmpro.color = architect.tmpro.color;
        architect.tmpro.maxVisibleCharacters = 0;
        architect.tmpro.text = architect.preText;

        if (architect.preText != "")
        {
            architect.tmpro.ForceMeshUpdate();
            architect.tmpro.maxVisibleCharacters = architect.tmpro.textInfo.characterCount;
        }

        architect.tmpro.text += architect.targetText;
        architect.tmpro.ForceMeshUpdate();
    }

    private IEnumerator Building() 
    { 
        while (architect.tmpro.maxVisibleCharacters < architect.tmpro.textInfo.characterCount)
        { 
            //PlaySoundOnBuild(typingSoundEffect, true);   

            architect.tmpro.maxVisibleCharacters += architect.hurryUp ? architect.charactersPerCycle * 2 : architect.charactersPerCycle;

            // Play Typing Sound for each character, ensuring it doesn't overlap too much
            if (textSound && !typingSound.isPlaying)
            {
                typingSound.PlayOneShot(typingSound.clip);
            } 

            yield return new WaitForSeconds(0.015f / (architect.hurryUp ? architect.speed * 5 : architect.speed));
        }

        //if (architect.tmpro.maxVisibleCharacters == architect.tmpro.textInfo.characterCount)
        //{
        //PlaySoundOnBuild(typingSoundEffect, false); 
        //}

        OnComplete(); 

        typingSound.Stop();
        //typingSound.loop = false; 
    } 

    //public void TextSoundSetting(bool textSound)
    //{
        //config.PlayTextSound = textSound;
        //ui.SetButtonColors(ui.textSoundPlay, ui.textSoundStop, textSound); 
        //if (textSound)
        //{
        //typingSound.Play(); 
        //typingSound.loop = true; 
        //} 
    //}
}
