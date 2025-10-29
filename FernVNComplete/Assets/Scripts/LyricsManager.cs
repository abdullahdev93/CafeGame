using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LyricsManager : MonoBehaviour
{
    [System.Serializable]
    public class LyricLine
    {
        public float time; // Time in the song when this line should appear
        public string lyricText; // The full lyric text to display
    }

    public float songTime; // Timer to keep track of song play time
    public TMP_Text currentLyricsText; // Text component for displaying current lyrics
    public TMP_Text nextLyricsText; // Text component for displaying next lyrics

    public Color currentLyricsColor = Color.red; // Color for the current lyric (red)
    public Color nextLyricsColor = Color.gray; // Color for the next lyric (gray)

    public List<LyricLine> songCupidLyrics = new List<LyricLine>(); // Lyrics for song Cupid
    public List<LyricLine> songFiveMLyrics = new List<LyricLine>(); // Lyrics for song 5M

    private int currentLyricIndex = 0; // To keep track of the current lyric being displayed
    public List<LyricLine> activeLyrics; // Holds reference to the currently active song's lyrics 

    public SongSelectManager songSelectManager; // Reference to SongSelectManager to get the selected song
    public KaraokeManager karaokeManager; // Reference to KaraokeManager to sync with the song 

    private bool songSelected = false; // Flag to check if a song has been selected 

    void Start()
    {
        songTime = 0f; // Initialize songTime to 0
        LoadLyricsForSelectedSong(); // Load the appropriate lyrics for the selected song 

        // Display the first lyric at the start of the song if lyrics are available
        if (activeLyrics != null && activeLyrics.Count > 0)
        {
            DisplayFirstLyric();
        }
    }

    void Update()
    {
        // Check if a song has been selected and we haven't loaded its lyrics yet
        if (!songSelected && songSelectManager.GetSelectedSongIndex() != -1)
        {
            LoadLyricsForSelectedSong(); // Load lyrics for the newly selected song
            songSelected = true; // Mark song as selected to avoid reloading lyrics every frame

            // Display the first lyric at the start of the song if lyrics are available
            if (activeLyrics != null && activeLyrics.Count > 0)
            {
                DisplayFirstLyric();
            }
        }

        if (activeLyrics != null && activeLyrics.Count > 0 && karaokeManager.theMusic.isPlaying)
        {
            songTime = karaokeManager.theMusic.time;
            HandleLyrics(); // Handle lyrics progression
        }
    }

    // Load the correct lyrics based on the selected song
    void LoadLyricsForSelectedSong()
    {
        // Get the selected song index from the SongSelectManager
        int selectedSongIndex = songSelectManager.GetSelectedSongIndex();

        // Select the appropriate lyrics list based on the selected song
        if (selectedSongIndex == 0) // Assuming 0 is for Cupid song
        {
            activeLyrics = songCupidLyrics;
        }
        else if (selectedSongIndex == 1) // Assuming 1 is for 5M song
        {
            activeLyrics = songFiveMLyrics;
        }
        else
        {
            activeLyrics = null; // No song selected, set activeLyrics to null
        }

        // Reset lyric index for new song
        currentLyricIndex = 0;
    } 

    // Handle when to update and switch the current and next lyrics based on song time
    void HandleLyrics()
    {
        // Ensure that there are lyrics and a valid index to check
        if (activeLyrics != null && currentLyricIndex < activeLyrics.Count - 1)
        {
            // Check if it's time to switch to the next lyric
            if (songTime >= activeLyrics[currentLyricIndex + 1].time)
            {
                // Move to the next lyric
                currentLyricIndex++;
                DisplayNextLyrics();
            }
        }
    }

    // Display the first lyric and next lyric at the start
    void DisplayFirstLyric()
    {
        if (activeLyrics != null && activeLyrics.Count > 0)
        {
            // Set the first lyric as current
            currentLyricsText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(currentLyricsColor)}>" + activeLyrics[0].lyricText + "</color>";

            // If there is a next lyric, display it in gray
            if (activeLyrics.Count > 1)
            {
                nextLyricsText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(nextLyricsColor)}>" + activeLyrics[1].lyricText + "</color>";
            }
            else
            {
                nextLyricsText.text = ""; // No more lyrics after the first one
            }
        }
    }

    // Display the next lyrics in gray and update the current lyrics
    void DisplayNextLyrics()
    {
        if (activeLyrics != null && currentLyricIndex < activeLyrics.Count)
        {
            // Move the current lyric to become the red "current lyric"
            currentLyricsText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(currentLyricsColor)}>" + activeLyrics[currentLyricIndex].lyricText + "</color>";

            // Set the next lyric (if there is one) to gray
            if (currentLyricIndex + 1 < activeLyrics.Count)
            {
                nextLyricsText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(nextLyricsColor)}>" + activeLyrics[currentLyricIndex + 1].lyricText + "</color>";
            }
            else
            {
                nextLyricsText.text = ""; // No more upcoming lyrics
            }
        }
    }
}
