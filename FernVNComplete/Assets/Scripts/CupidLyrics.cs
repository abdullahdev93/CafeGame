using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq; 
using TMPro; 

public class CupidLyrics : MonoBehaviour
{
    public TextAsset lyricsFile;
    public Dictionary<string, float> lyricsDict;
    public float startTime = 0f;
    public float lineDelay = 1f;
    private float nextLyricTime = 0f;
    private int currentLyricIndex = 0;

    public TMP_Text theSongLyrics; 

    void Start()
    {
        lyricsDict = new Dictionary<string, float>();

        if (lyricsFile != null)
        {
            string[] splitText = lyricsFile.text.Split('\n');

            foreach (string line in splitText)
            {
                if (line.Trim() != "")
                {
                    string[] splitLine = line.Trim().Split(';');
                    float timeOffset = 0f;
                    if (splitLine.Length > 2)
                    {
                        float.TryParse(splitLine[2], out timeOffset);
                    }

                    lyricsDict.Add($"{splitLine[0]};{splitLine[1]}", timeOffset); 
                }
            }
        }

        nextLyricTime = startTime;
    }

    void Update()
    {
        if (Time.time >= nextLyricTime && currentLyricIndex < lyricsDict.Count)
        {
            KeyValuePair<string, float> currentLyric = lyricsDict.ElementAt(currentLyricIndex);
            theSongLyrics.text = currentLyric.Key; 
            //Debug.Log(currentLyric.Key); 
            nextLyricTime += lineDelay + currentLyric.Value;
            currentLyricIndex++;
        }
    }
} 
