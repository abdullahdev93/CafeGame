using DIALOGUE;
using History;
using System;
using UnityEngine;
using UnityEngine.UI; 

namespace COMMANDS
{
    public class CMD_DatabaseExtension_Audio : CMD_DatabaseExtension
    {
        // --- Track snapshots so we can resume after cutscenes ---
        private struct TrackSnapshot
        {
            public string filePath;   // Resources path used at load time
            public int channel;       // e.g., 0 ambience, 1 song (your defaults)
            public bool loop;
            public float startVolume; // what PlayTrack was given
            public float volumeCap;   // what PlayTrack was given
            public float pitch;       // what PlayTrack was given
        }

        private static TrackSnapshot? _lastSong;
        private static TrackSnapshot? _lastAmbience;

        private static void RememberTrack(int channel, string filePath, bool loop, float startVolume, float volumeCap, float pitch)
        {
            var snap = new TrackSnapshot
            {
                filePath = filePath,
                channel = channel,
                loop = loop,
                startVolume = startVolume,
                volumeCap = volumeCap,
                pitch = pitch
            };

            // Convention: channel 1 = Song (default in your PlaySong), 0 = Ambience (default in your PlayAmbience)
            if (channel == 1) _lastSong = snap;
            else if (channel == 0) _lastAmbience = snap;
        }

        // Hard stop both (you used this earlier)
        public static void StopSongAndAmbienceNow()
        {
            try
            {
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.StopTrack(0); // ambience
                    AudioManager.instance.StopTrack(1); // song
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"StopSongAndAmbienceNow failed: {ex.Message}");
            }
        }

        // Resume both. If startAtFullVolume=true, we ignore original startVolume and start at volumeCap.
        public static void ResumeLastSongAndAmbience(bool startAtFullVolume = true)
        {
            ResumeSnapshot(_lastAmbience, startAtFullVolume);
            ResumeSnapshot(_lastSong, startAtFullVolume);
        }

        public static void ResumeLastSong(bool startAtFullVolume = true) => ResumeSnapshot(_lastSong, startAtFullVolume);
        public static void ResumeLastAmbience(bool startAtFullVolume = true) => ResumeSnapshot(_lastAmbience, startAtFullVolume);

        private static void ResumeSnapshot(TrackSnapshot? snap, bool startAtFullVolume)
        {
            if (!snap.HasValue) { Debug.Log("[AudioResume] No snapshot to resume."); return; }
            if (AudioManager.instance == null) { Debug.LogWarning("[AudioResume] No AudioManager instance."); return; }

            var s = snap.Value;
            var clip = Resources.Load<AudioClip>(s.filePath);
            if (clip == null)
            {
                Debug.LogWarning($"[AudioResume] Could not load '{s.filePath}'");
                return;
            }

            float startVol = startAtFullVolume ? s.volumeCap : s.startVolume;
            Debug.Log($"[AudioResume] Resuming ch={s.channel} clip='{s.filePath}' startVol={startVol} cap={s.volumeCap} loop={s.loop} pitch={s.pitch}");
            AudioManager.instance.PlayTrack(clip, s.channel, s.loop, startVol, s.volumeCap, s.pitch, s.filePath);
        } 

        private static string[] PARAM_SFX = new string[] { "-s", "-sfx" };
        private static string[] PARAM_VOLUME = new string[] { "-v", "-vol", "-volume" };
        private static string[] PARAM_PITCH = new string[] { "-p", "-pitch" };
        private static string[] PARAM_LOOP = new string[] { "-l", "-loop" };

        private static string[] PARAM_CHANNEL = new string[] { "-c", "-channel" };
        private static string[] PARAM_START_VOLUME = new string[] { "-sv", "-startvolume" };
        private static string[] PARAM_SONG = new string[] { "-s", "-song" };
        private static string[] PARAM_AMBIENCE = new string[] { "-a", "-ambience" };

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("playsfx", new Action<string[]>(PlaySFX));
            database.AddCommand("stopsfx", new Action<string>(StopSFX));

            database.AddCommand("playvoice", new Action<string[]>(PlayVoice));
            database.AddCommand("stopvoice", new Action<string>(StopSFX));

            database.AddCommand("playsong", new Action<string[]>(PlaySong));
            database.AddCommand("playambience", new Action<string[]>(PlayAmbience));

            database.AddCommand("stopsong", new Action<string>(StopSong));
            database.AddCommand("stopambience", new Action<string>(StopAmbience));
        }

        private static void PlaySFX(string[] data)
        {
            string filepath;
            float volume, pitch;
            bool loop;

            var parameters = ConvertDataToParameters(data);

            //Try to get the name or path to the sound effect
            parameters.TryGetValue(PARAM_SFX, out filepath);

            //Try to get the volume of the sound
            parameters.TryGetValue(PARAM_VOLUME, out volume, defaultValue: 1f);

            //Try to get the pitch of the sound
            parameters.TryGetValue(PARAM_PITCH, out pitch, defaultValue: 1f);

            //Try to get if this sound loops
            parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: false);

            //Run the logic
            string resourcesPath = FilePaths.GetPathToResource(FilePaths.resources_sfx, filepath);
            AudioClip sound = Resources.Load<AudioClip>(resourcesPath);

            if (sound == null)
            {
                Debug.Log($"Was not able to load sfx '{filepath}'");
                return;
            }

            AudioManager.instance.PlaySoundEffect(sound, volume: volume, pitch: pitch, loop: loop, filePath: resourcesPath);
        }

        private static void StopSFX(string data)
        {
            AudioManager.instance.StopSoundEffect(data);
        }

        private static void PlayVoice(string[] data)
        {
            string filepath;
            float volume, pitch;
            bool loop;

            var parameters = ConvertDataToParameters(data);

            //Try to get the name or path to the sound effect
            parameters.TryGetValue(PARAM_SFX, out filepath);

            //Try to get the volume of the sound
            parameters.TryGetValue(PARAM_VOLUME, out volume, defaultValue: 1f);

            //Try to get the pitch of the sound
            parameters.TryGetValue(PARAM_PITCH, out pitch, defaultValue: 1f);

            //Try to get if this sound loops
            parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: false);

            //Run the logic
            AudioClip sound = Resources.Load<AudioClip>(FilePaths.GetPathToResource(FilePaths.resources_voices, filepath));

            if (sound == null)
            {
                Debug.Log($"Was not able to load voice '{filepath}'");
                return;
            }

            // Play the voice immediately for the current log
            AudioSource currentAudioSource = DialogueSystem.instance.GetCurrentAudioSource();

            if (currentAudioSource != null)
            {
                currentAudioSource.clip = sound;
                currentAudioSource.volume = volume;
                currentAudioSource.pitch = pitch;
                currentAudioSource.loop = loop;
                currentAudioSource.Play();
            }

            // Set the upcoming voice in HistoryManager so the next log can use it
            if (HistoryManager.instance != null)
            {
                HistoryManager.instance.SetUpcomingVoice(sound, volume, pitch, loop);
            }

            if (DialogueSystem.instance != null && DialogueSystem.instance.dialogueBoxSpeaker != null)
            {
                AudioSource speakerAudio = DialogueSystem.instance.dialogueBoxSpeaker.GetComponent<AudioSource>();
                Button speakerButton = DialogueSystem.instance.dialogueBoxSpeaker.GetComponent<Button>();

                if (speakerAudio != null)
                {
                    speakerAudio.clip = sound;
                    speakerAudio.volume = volume;
                    speakerAudio.pitch = pitch;
                    speakerAudio.loop = loop;
                }

                if (speakerButton != null)
                {
                    speakerButton.interactable = true;
                }
            } 

            //AudioManager.instance.PlayVoice(sound, volume: volume, pitch: pitch, loop: loop);
        }

        private static void PlaySong(string[] data)
        {
            string filepath;
            int channel;

            var parameters = ConvertDataToParameters(data);

            //Try to get the name or path to the track
            parameters.TryGetValue(PARAM_SONG, out filepath);
            filepath = FilePaths.GetPathToResource(FilePaths.resources_music, filepath);

            //Try to get the channel for this track
            parameters.TryGetValue(PARAM_CHANNEL, out channel, defaultValue: 1);

            PlayTrack(filepath, channel, parameters);
        }

        private static void PlayAmbience(string[] data)
        {
            string filepath;
            int channel;

            var parameters = ConvertDataToParameters(data);

            //Try to get the name or path to the track
            parameters.TryGetValue(PARAM_AMBIENCE, out filepath);
            filepath = FilePaths.GetPathToResource(FilePaths.resources_ambience, filepath);

            //Try to get the channel for this track
            parameters.TryGetValue(PARAM_CHANNEL, out channel, defaultValue: 0);

            PlayTrack(filepath, channel, parameters);
        }

        private static void PlayTrack(string filepath, int channel, CommandParameters parameters)
        {
            bool loop;
            float volumeCap;
            float startVolume;
            float pitch;

            //Try to get the max volume of the track
            parameters.TryGetValue(PARAM_VOLUME, out volumeCap, defaultValue: 1f);

            //Try to get the start volume of the track
            parameters.TryGetValue(PARAM_START_VOLUME, out startVolume, defaultValue: 0f);

            //Try to get the pitch of the track
            parameters.TryGetValue(PARAM_PITCH, out pitch, defaultValue: 1f);

            //Try to get if this track loops
            parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: true);

            //Run the logic
            AudioClip sound = Resources.Load<AudioClip>(filepath);

            if (sound == null)
            {
                Debug.Log($"Was not able to load voice '{filepath}'");
                return;
            }

            AudioManager.instance.PlayTrack(sound, channel, loop, startVolume, volumeCap, pitch, filepath);

            //AudioManager.instance.PlayTrack(sound, channel, loop, startVolume, volumeCap, pitch, filepath);

            // Remember what we just started so we can restore it later
            RememberTrack(channel, filepath, loop, startVolume, volumeCap, pitch); 
        }

        private static void StopSong(string data)
        {
            if (data == string.Empty)
                StopTrack("1");
            else
                StopTrack(data);
        }

        private static void StopAmbience(string data)
        {
            if (data == string.Empty)
                StopTrack("0");
            else
                StopTrack(data);
        }

        private static void StopTrack(string data)
        {
            if (int.TryParse(data, out int channel))
                AudioManager.instance.StopTrack(channel);
            else
                AudioManager.instance.StopTrack(data);
        }
    }
}