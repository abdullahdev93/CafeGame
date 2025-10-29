using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongSelectManager : MonoBehaviour
{
    public AudioClip[] songs;  // Array of songs to select from
    private int selectedSongIndex = -1; // Index of the selected song 
    public AudioSource audioSource; // Reference to the AudioSource that will play the song

    public Button[] songButtons; // Buttons for each song in the UI
    public GameObject songSelectPanel; // Panel containing the song selection UI 

    public BeatScroller[] beatScrollers; // Add BeatScrollers here for each song 

    // Lyrics for each song (already covered in previous steps)
    public List<List<string>> songLyrics = new List<List<string>>();

    private void Start()
    {
        // Initialize lyrics for each song (example)
        songLyrics.Add(new List<string> { "Lyric 1 for song 1", "Lyric 2 for song 1" }); // Song 1 lyrics
        songLyrics.Add(new List<string> { "Lyric 1 for song 2", "Lyric 2 for song 2" }); // Song 2 lyrics
        songLyrics.Add(new List<string> { "Lyric 1 for song 3", "Lyric 2 for song 3" }); // Song 3 lyrics

        // Attach listeners to each song button
        for (int i = 0; i < songButtons.Length; i++)
        {
            int index = i; // Local copy to avoid closure issue
            songButtons[i].onClick.AddListener(() => SelectSong(index));
        }

        songSelectPanel.SetActive(true); 
    }

    // Function to handle song selection
    public void SelectSong(int songIndex)
    {
        selectedSongIndex = songIndex;
        Debug.Log("Selected Song: " + songs[songIndex].name);

        // Hide the Song Selection Menu (SongSelectPanel)
        songSelectPanel.SetActive(false);

        // Play the selected song
        audioSource.clip = songs[songIndex];

        // Stop all beat scrollers and activate the selected one
        foreach (var scroller in beatScrollers)
        {
            scroller.hasStarted = false; // Stop all scrollers
        }

        beatScrollers[songIndex].hasStarted = true; // Start only the selected one 
        //audioSource.Play();
    }

    // Get the currently selected song index
    public int GetSelectedSongIndex()
    {
        return selectedSongIndex;
    } 

    public AudioClip GetSelectedSong()
    {
        if (selectedSongIndex != -1)
        {
            return songs[selectedSongIndex]; // Return the selected song clip
        }
        return null; // Return null if no song has been selected
    }

    public List<string> GetLyricsForSelectedSong()
    {
        if (selectedSongIndex != -1)
        {
            return songLyrics[selectedSongIndex]; // Return the lyrics for the selected song
        }
        return null; // Return null if no song has been selected
    } 
}
