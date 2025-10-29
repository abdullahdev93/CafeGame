using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace History
{
    [RequireComponent(typeof(HistoryLogManager))]
    [RequireComponent(typeof(HistoryNavigation))]
    public class HistoryManager : MonoBehaviour
    {
        public const int HISTORY_CACHE_LIMIT = 100;
        public static HistoryManager instance { get; private set; }
        public List<HistoryState> history = new List<HistoryState>();

        private AudioClip upcomingVoiceClip;
        private float upcomingVoiceVolume;
        private float upcomingVoicePitch;
        private bool upcomingVoiceLoop; 

        private HistoryNavigation navigation;

        //private AudioManager theAudioManager; 
        public bool isViewingHistory => navigation.isViewingHistory;

        //private AudioSource[] sfx => theAudioManager.allSFX; 

        public HistoryLogManager logManager { get; private set; }

        private void Awake()
        {
            instance = this;
            navigation = GetComponent<HistoryNavigation>();
            logManager = GetComponent<HistoryLogManager>();
        }

        // Start is called before the first frame update
        void Start()
        {
            DialogueSystem.instance.onClear += LogCurrentState;
        }

        // Method to set the upcoming voice
        public void SetUpcomingVoice(AudioClip clip, float volume, float pitch, bool loop)
        {
            upcomingVoiceClip = clip;
            upcomingVoiceVolume = volume;
            upcomingVoicePitch = pitch;
            upcomingVoiceLoop = loop;
        }

        // Method to assign the upcoming voice to the next log
        public void AssignUpcomingVoiceToLog(HistoryLog log)
        {
            if (upcomingVoiceClip != null && log.dialogueAudio != null)
            {
                log.dialogueAudio.clip = upcomingVoiceClip;
                log.dialogueAudio.volume = upcomingVoiceVolume;
                log.dialogueAudio.pitch = upcomingVoicePitch;
                log.dialogueAudio.loop = upcomingVoiceLoop;

                // Play the audio if needed
                //log.dialogueAudio.Play();

                // Clear upcoming voice data after assigning
                ClearUpcomingVoice();
            }
        }

        private void ClearUpcomingVoice()
        {
            upcomingVoiceClip = null;
            upcomingVoiceVolume = 1f;
            upcomingVoicePitch = 1f;
            upcomingVoiceLoop = false;
        } 

    public void LogCurrentState()
        {
            HistoryState state = HistoryState.Capture();
            history.Add(state);
            logManager.AddLog(state);

            if (history.Count > HISTORY_CACHE_LIMIT)
                history.RemoveAt(0);
        }

        public void LoadState(HistoryState state)
        {
            state.Load();
        }

        public void GoForward() => navigation.GoForward();
        public void GoBack() => navigation.GoBack();
    }
}