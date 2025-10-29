using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource audioSource; // The AudioSource component on the music player
    public Album[] albums; // Array to hold different albums

    private int currentAlbumIndex = 0;
    private int currentSongIndex = 0;

    void Start()
    {
        if (albums.Length > 0 && albums[currentAlbumIndex].songs.Length > 0 && audioSource != null)
        {
            audioSource.clip = albums[currentAlbumIndex].songs[currentSongIndex];
            audioSource.Play();
        }
    }

    void OnMouseDown()
    {
        SwitchAlbum();
    }

    void SwitchAlbum()
    {
        if (albums.Length == 0 || audioSource == null) return;

        // Switch to the next album
        currentAlbumIndex = (currentAlbumIndex + 1) % albums.Length;
        currentSongIndex = 0; // Reset to the first song of the new album

        if (albums[currentAlbumIndex].songs.Length > 0)
        {
            audioSource.clip = albums[currentAlbumIndex].songs[currentSongIndex];
            audioSource.Play();
        }
    }

    void SwitchSong()
    {
        if (albums.Length == 0 || audioSource == null) return;

        // Switch to the next song in the current album
        currentSongIndex = (currentSongIndex + 1) % albums[currentAlbumIndex].songs.Length;

        audioSource.clip = albums[currentAlbumIndex].songs[currentSongIndex];
        audioSource.Play();
    }
}

[System.Serializable]
public class Album
{
    public string albumName; // Name of the album
    public AudioClip[] songs; // Array of songs in the album
}
