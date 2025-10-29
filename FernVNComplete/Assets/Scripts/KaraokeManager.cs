using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class KaraokeManager : MonoBehaviour
{
    public static KaraokeManager instance;
    public AudioSource theMusic;
    public bool startPlaying;

    //public BeatScroller theBeatScroller;

    public BeatScroller[] beatScrollers; // Array to hold BeatScroller for each song 

    //public GameObject notePrefab; // Note prefab to instantiate

    public bool DeveloperBeatCreation; // Boolean to control beat creation mode 

    // Prefabs for different arrows (Up, Down, Left, Right)
    public GameObject upArrowPrefab;
    public GameObject downArrowPrefab;
    public GameObject leftArrowPrefab;
    public GameObject rightArrowPrefab;

    public Transform upArrowSpawn;
    public Transform downArrowSpawn;
    public Transform leftArrowSpawn;
    public Transform rightArrowSpawn; 

    // Parent object to hold the instantiated arrows
    public GameObject developerScroller; 

    //public Transform[] noteSpawnPositions; // Positions where notes will spawn, one for each activator 

    public int currentScore;
    public int scoreForNote;
    public int scoreForGoodNote;
    public int scoreForPerfectNote;

    public TMP_Text scoreText;
    public TMP_Text multiplierText;

    public SongSelectManager songSelectManager; // Reference to the SongSelectManager

    private BeatScroller developerBeatScroller; // Reference to the BeatScroller component of the developerScroller


    //public TMP_Text songTimerDisplay; // Optional UI element to display the song timer

    public float totalNotes;
    public float OKHits;
    public float goodHits;
    public float perfectHits;
    public float missedHits;

    public GameObject resultsScreen;
    public GameObject failedScreen;
    public TMP_Text percentHitText;
    public TMP_Text OKHitText;
    public TMP_Text goodHitText;
    public TMP_Text perfectHitText;
    public TMP_Text missedHitText;
    public TMP_Text rankText;
    public TMP_Text finalScoreText;

    public int currentMuliplier;

    private float songTime; // Timer to keep track of song time 

    private BeatScroller activeBeatScroller; // The currently active BeatScroller 

    public Slider rhythmMeterSlider; // UI element to display the Rhythm Meter
    public float rhythmMeter = 0.5f; // Start in the middle (0.5) 
    public float maxRhythmMeter = 1.0f;
    public float minRhythmMeter = 0.0f;
    public float rhythmIncreaseAmount = 0.05f;
    public float rhythmDecreaseAmount = 0.2f; 

    public Button exitGameAmazingButton; 
    public Button exitGameGoodButton; 
    public Button exitGameOKButton; 
    public Button exitGameTerribleButton; 
    public Button exitGameHorrendousButton;  

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        scoreText.text = "Score: 0";
        currentMuliplier = 1;
        //totalNotes = FindObjectsOfType<NoteObject>().Length;
        songTime = 0f; // Initialize song time 

        // Get the selected song from SongSelectManager
        AudioClip selectedSong = songSelectManager.GetSelectedSong();
        if (selectedSong != null)
        {
            theMusic.clip = selectedSong; // Assign the selected song to the AudioSource
        }

        exitGameAmazingButton.gameObject.SetActive(false);
        exitGameGoodButton.gameObject.SetActive(false);
        exitGameOKButton.gameObject.SetActive(false);
        exitGameTerribleButton.gameObject.SetActive(false);
        exitGameHorrendousButton.gameObject.SetActive(false);

        exitGameAmazingButton.onClick.AddListener(() => BackToBarButtonAmazingClicked()); 
        exitGameGoodButton.onClick.AddListener(() => BackToBarButtonGoodClicked()); 
        exitGameOKButton.onClick.AddListener(() => BackToBarButtonOKClicked()); 
        exitGameTerribleButton.onClick.AddListener(() => BackToBarButtonTerribleClicked()); 
        exitGameHorrendousButton.onClick.AddListener(() => BackToBarButtonHorrendousClicked()); 

        developerBeatScroller = developerScroller.GetComponent<BeatScroller>();

        UpdateRhythmValues();
        UpdateRhythmMeter(); // Initialize rhythm meter UI based on stats 

        //rhythmMeterSlider.value = rhythmMeter; // Initialize rhythm meter UI 

        //DeveloperBeatCreation = true; // Set this to true in developer mode  
    }

    // New method to update rhythmDecreaseAmount, rhythmIncreaseAmount, and rhythmMeter
    private void UpdateRhythmValues()
    {
        // Update rhythmDecreaseAmount and rhythmIncreaseAmount based on Confidence
        int confidence = PlayerStats.instance.Confidence.Level;
        if (confidence < 2)
        {
            rhythmDecreaseAmount = 0.2f;
            rhythmIncreaseAmount = 0.05f;
        }
        else if (confidence > 8)
        {
            rhythmDecreaseAmount = 0.05f;
            rhythmIncreaseAmount = 0.2f;
        }
        else if (confidence > 5)
        {
            rhythmDecreaseAmount = 0.1f;
            rhythmIncreaseAmount = 0.15f;
        }
        else if (confidence > 2)
        {
            rhythmDecreaseAmount = 0.15f;
            rhythmIncreaseAmount = 0.1f;
        }

        // Update maxRhythmMeter based on Courage
        int courage = PlayerStats.instance.Courage.Level;
        if (courage < 3)
        {
            maxRhythmMeter = 1f;
        }
        else if (courage >= 9)
        {
            maxRhythmMeter = 2f;
        }
        else if (courage >= 7)
        {
            maxRhythmMeter = 1.7f;
        }
        else if (courage >= 5)
        {
            maxRhythmMeter = 1.5f;
        }
        else if (courage >= 3)
        {
            maxRhythmMeter = 1.2f;
        }

        // Update rhythmMeter start value based on Charisma
        int charisma = PlayerStats.instance.Charisma.Level;
        if (charisma <= 2)
        {
            if (courage <= 3)
                rhythmMeter = 1f; 
            else 
                rhythmMeter = 0.5f;
        }
        else if (charisma >= 9)
        {
            if (courage >= 9)
                rhythmMeter = 2f; 
            else 
                rhythmMeter = 1f;
        }
        else if (charisma >= 8)
        {
            if (courage >= 9)
                rhythmMeter = 1.7f; 
            else 
                rhythmMeter = 0.85f;
        }
        else if (charisma >= 6)
        {
            if (courage >= 7)
                rhythmMeter = 1.5f; 
            else
                rhythmMeter = 0.75f;
        }
        else if (charisma >= 4)
        {
            if (courage >= 5)
                rhythmMeter = 1.2f; 
            else 
                rhythmMeter = 0.6f;
        } 
    } 

    // Update is called once per frame
    void Update()
    { 
        UpdateRhythmMeter(); // Call to update the rhythm meter 

        if (DeveloperBeatCreation)
        {
            // Set the developerScroller as the active beat scroller
            if (activeBeatScroller != developerBeatScroller)
            {
                activeBeatScroller = developerBeatScroller;
                activeBeatScroller.hasStarted = true;
            }

            HandleDeveloperBeatCreation(); // Handle note instantiation
        }


        if (!startPlaying)
        {
            // Ensure the game starts only after a song is selected
            AudioClip selectedSong = songSelectManager.GetSelectedSong();
            int selectedSongIndex = songSelectManager.GetSelectedSongIndex(); // Get the selected song index from SongSelectManager 
            if (selectedSong != null && !theMusic.isPlaying)
            {
                startPlaying = true;
                //theBeatScroller.hasStarted = true; 
                // Activate the correct BeatScroller based on the selected song
                if (activeBeatScroller == null)
                {
                    activeBeatScroller = beatScrollers[selectedSongIndex]; // Set the active BeatScroller
                    activeBeatScroller.hasStarted = true; // Start the beat scroller for this song
                } 
                theMusic.Play(); // Start the song only after selection 

                // Set totalNotes to the number of child objects in the active BeatScroller
                totalNotes = CountTotalNotes(activeBeatScroller); 
            }
        } 
        else
        {
            // Check if the music is playing, then update song time
            if (theMusic.isPlaying)
            {
                songTime += Time.deltaTime;

                // Optional: Update song time display (formatted as minutes:seconds)
                int minutes = Mathf.FloorToInt(songTime / 60F);
                int seconds = Mathf.FloorToInt(songTime % 60F);
                //songTimerDisplay.text = string.Format("{0:0}:{1:00}", minutes, seconds);
            }

            //if (missedHits >= 10)
            //{ 
            //startPlaying = false; 
            //activeBeatScroller.hasStarted = false; 
            //theMusic.Stop(); 
            //failedScreen.SetActive(true); 
            //exitGameHorrendousButton.gameObject.SetActive(true); 
            //} 

            // End the game if the Rhythm Meter reaches 0
            if (rhythmMeter <= minRhythmMeter)
            {
                EndGameDueToMeter();
            } 

            if (!theMusic.isPlaying && !resultsScreen.activeInHierarchy)
            {
                resultsScreen.SetActive(true);
                OKHitText.text = OKHits.ToString();
                goodHitText.text = goodHits.ToString();
                perfectHitText.text = perfectHits.ToString();
                missedHitText.text = missedHits.ToString();

                float totalHits = OKHits + goodHits + perfectHits;
                float percentHits = Mathf.Ceil((totalHits / totalNotes) * 100f); 

                //float percentHits = (totalHits / totalNotes) * 100f;

                if (percentHits > 100)
                    percentHits = 100;

                percentHitText.text = percentHits.ToString() + "%";

                string rankVal = GetRank(percentHits);
                rankText.text = rankVal;
                finalScoreText.text = currentScore.ToString(); 

                if (percentHits <= 100f) 
                    exitGameAmazingButton.gameObject.SetActive(true);

                if (percentHits <= 80f)
                    exitGameGoodButton.gameObject.SetActive(true);

                if (percentHits <= 60f)
                    exitGameOKButton.gameObject.SetActive(true);

                if (percentHits <= 40f) 
                    exitGameTerribleButton.gameObject.SetActive(true); 
            }
        }
    }

    // New method to count the total notes (child objects) in the active BeatScroller
    private float CountTotalNotes(BeatScroller beatScroller)
    {
        return beatScroller.transform.childCount;
    }

    private void EndGameDueToMeter()
    {
        startPlaying = false;
        activeBeatScroller.hasStarted = false;
        theMusic.Stop();
        failedScreen.SetActive(true); // Show the "failed" screen
        exitGameHorrendousButton.gameObject.SetActive(true); // Activate the "game over" button
    } 

    void HandleDeveloperBeatCreation()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Up Arrow Pressed"); // Debug statement to verify
            GameObject arrow = Instantiate(upArrowPrefab, upArrowSpawn.position, upArrowPrefab.transform.rotation); // Use prefab's original rotation
            arrow.transform.SetParent(developerScroller.transform);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Down Arrow Pressed"); // Debug statement to verify
            GameObject arrow = Instantiate(downArrowPrefab, downArrowSpawn.position, downArrowPrefab.transform.rotation); // Use prefab's original rotation
            arrow.transform.SetParent(developerScroller.transform);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Left Arrow Pressed"); // Debug statement to verify
            GameObject arrow = Instantiate(leftArrowPrefab, leftArrowSpawn.position, leftArrowPrefab.transform.rotation); // Use prefab's original rotation
            arrow.transform.SetParent(developerScroller.transform);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Right Arrow Pressed"); // Debug statement to verify
            GameObject arrow = Instantiate(rightArrowPrefab, rightArrowSpawn.position, rightArrowPrefab.transform.rotation); // Use prefab's original rotation
            arrow.transform.SetParent(developerScroller.transform);
        }
    } 

    public void BackToBarButtonAmazingClicked()
    { 
        // Set the starting file based on the location
        PlayerPrefs.SetString("StartingFile", "BarAfterAmazingKaraokeFile"); 
        PlayerPrefs.Save();

        // Transfer to the VisualNovel scene
        SceneManager.LoadScene("VisualNovel");
    }

    public void BackToBarButtonGoodClicked()
    {
        // Set the starting file based on the location
        PlayerPrefs.SetString("StartingFile", "BarAfterGoodKaraokeFile");
        PlayerPrefs.Save();

        // Transfer to the VisualNovel scene
        SceneManager.LoadScene("VisualNovel");
    }

    public void BackToBarButtonOKClicked()
    {
        // Set the starting file based on the location
        PlayerPrefs.SetString("StartingFile", "BarAfterOKKaraokeFile");
        PlayerPrefs.Save();

        // Transfer to the VisualNovel scene
        SceneManager.LoadScene("VisualNovel");
    }

    public void BackToBarButtonTerribleClicked()
    {
        // Set the starting file based on the location
        PlayerPrefs.SetString("StartingFile", "BarAfterTerribleKaraokeFile");
        PlayerPrefs.Save();

        // Transfer to the VisualNovel scene
        SceneManager.LoadScene("VisualNovel");
    }

    public void BackToBarButtonHorrendousClicked() 
    {
        // Set the starting file based on the location
        PlayerPrefs.SetString("StartingFile", "BarAfterHorrendousKaraokeFile");
        PlayerPrefs.Save();

        // Transfer to the VisualNovel scene
        SceneManager.LoadScene("VisualNovel");
    }

    // Update the rhythm meter
    private void UpdateRhythmMeter()
    {
        rhythmMeter = Mathf.Clamp(rhythmMeter, minRhythmMeter, maxRhythmMeter);
        rhythmMeterSlider.value = rhythmMeter; // Update the UI
    }

    // Called when a note is hit (OK, Good, Perfect)
    public void IncreaseRhythmMeter()
    {
        rhythmMeter = Mathf.Clamp(rhythmMeter + rhythmIncreaseAmount, minRhythmMeter, maxRhythmMeter);
        rhythmMeterSlider.value = rhythmMeter;
    }

    // Called when a note is missed
    public void DecreaseRhythmMeter()
    {
        rhythmMeter = Mathf.Clamp(rhythmMeter - rhythmDecreaseAmount, minRhythmMeter, maxRhythmMeter);
        rhythmMeterSlider.value = rhythmMeter;
    } 

    public void PerfectHold()
    {
        currentScore += scoreForPerfectNote * currentMuliplier; // Bonus for holding long notes perfectly
        NoteHit();
        perfectHits++;
    }

    public void PerfectRapidPress()
    {
        currentScore += scoreForPerfectNote * currentMuliplier; // Bonus for completing the rapid presses
        NoteAltHit();
        perfectHits++;
    }

    public void GoodRapidPress()
    {
        currentScore += scoreForGoodNote * currentMuliplier; // Bonus for getting Good on rapid presses
        NoteAltHit();
        goodHits++;
    }

    public void OKRapidPress()
    {
        currentScore += scoreForNote * currentMuliplier; // Bonus for getting OK on rapid presses
        NoteAltHit();
        OKHits++;
    }

    private string GetRank(float percentHits)
    {
        string rankVal = "F";
        if (percentHits >= 15) rankVal = "F-";
        if (percentHits >= 20) rankVal = "F";
        if (percentHits >= 25) rankVal = "F+";
        if (percentHits >= 30) rankVal = "D-";
        if (percentHits >= 35) rankVal = "D";
        if (percentHits >= 40) rankVal = "D+";
        if (percentHits >= 45) rankVal = "C-";
        if (percentHits >= 50) rankVal = "C";
        if (percentHits >= 55) rankVal = "C+";
        if (percentHits >= 60) rankVal = "B-";
        if (percentHits >= 65) rankVal = "B";
        if (percentHits >= 70) rankVal = "B+";
        if (percentHits >= 75) rankVal = "A-";
        if (percentHits >= 80) rankVal = "A";
        if (percentHits >= 85) rankVal = "A+";
        if (percentHits >= 90) rankVal = "S-";
        if (percentHits >= 95) rankVal = "S";
        if (percentHits >= 100) rankVal = "S+";

        return rankVal;
    }

    public void NoteHit()
    {
        currentMuliplier++;
        multiplierText.text = "Multiplier: x" + currentMuliplier;
        scoreText.text = "Score: " + currentScore;
    }

    public void NoteAltHit()
    {
        multiplierText.text = "Multiplier: x" + currentMuliplier;
        scoreText.text = "Score: " + currentScore;
    } 

    public void OKHit()
    {
        currentScore += scoreForNote * currentMuliplier;
        NoteHit();
        OKHits++;
    }

    public void GoodHit()
    {
        currentScore += scoreForGoodNote * currentMuliplier;
        NoteHit();
        goodHits++;
    }

    // Good hold feedback for long notes
    public void GoodHold()
    {
        currentScore += scoreForGoodNote * currentMuliplier; // Good hold
        NoteHit();
        goodHits++;
    }

    // OK hold feedback for long notes
    public void OKHold()
    {
        currentScore += scoreForNote * currentMuliplier; // OK hold
        NoteHit();
        OKHits++;
    }

    public void PerfectHit()
    {
        currentScore += scoreForPerfectNote * currentMuliplier;
        NoteHit();
        perfectHits++;
    }

    public void NoteMissed()
    {
        currentMuliplier = 1;
        multiplierText.text = "Multiplier: x" + currentMuliplier;
        missedHits++;
    }
}
