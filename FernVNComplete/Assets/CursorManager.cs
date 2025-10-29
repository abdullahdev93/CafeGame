using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorImage;
    public Texture2D cursorImageALT; 
    public float shakeAmount = 100f; // Adjust this value to control the shakiness
    public float shakeSpeed = 10f; // Adjust this value to control the speed of the shaking 
    public float stabilizationDuration = 3.0f;  // How long the cursor stays stable   
    public float idleThreshold = 1f; // Time in seconds before random travel starts 
    public float travelSpeed = 3f;  // Speed of random travel  

    private Vector2 cursorHotspot;
    private float noiseOffsetX;
    private float noiseOffsetY;

    private float stabilizationEndTime;  // Time when stabilization will end 
    private float lastMouseMovementTime; // Time of the last mouse movement

    private bool isStabilized = false;   // Tracks if the cursor is stabilized 
    private bool isRandomTraveling = false;  // Whether the cursor is traveling randomly

    public PlayerStats playerStats; // Reference to player stats

    void Start()
    {
        cursorHotspot = new Vector2(cursorImage.width / 2, cursorImage.height / 2);
        Cursor.SetCursor(cursorImage, cursorHotspot, CursorMode.Auto);

        noiseOffsetX = Random.Range(0f, 100f);
        noiseOffsetY = Random.Range(0f, 100f);
        lastMouseMovementTime = Time.time;

        //playerStats = PlayerStats.instance; // Access the player stats

        ApplyStatEffects(); // Apply the stat effects at the start
    }

    void Update()
    {
        if (GameManager.instance.gameOver)
        {
            cursorHotspot = new Vector2(cursorImageALT.width / 4, cursorImageALT.height / 4);
            Cursor.SetCursor(cursorImageALT, cursorHotspot, CursorMode.Auto);
            return;
        } 

        // Check for mouse movement or click input
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0 || Input.anyKeyDown)
        {
            lastMouseMovementTime = Time.time;
            isRandomTraveling = false;
        }

        // If idle for too long, enable random travel mode
        if (Time.time - lastMouseMovementTime > idleThreshold && !isRandomTraveling)
        {
            isRandomTraveling = true;
        }

        if (isStabilized && Time.time < stabilizationEndTime)  
        { 
            Cursor.SetCursor(cursorImage, cursorHotspot, CursorMode.Auto);
        }
        else if (isRandomTraveling) 
        {
            RandomlyMoveCursor();
        }
        else
        { 
            float time = Time.time * shakeSpeed;
            float offsetX = (Mathf.PerlinNoise(noiseOffsetX, time) - 0.5f) * shakeAmount * 2f;
            float offsetY = (Mathf.PerlinNoise(noiseOffsetY, time) - 0.5f) * shakeAmount * 2f;
            Vector2 shakyHotspot = cursorHotspot + new Vector2(offsetX, offsetY);
            Cursor.SetCursor(cursorImage, shakyHotspot, CursorMode.Auto);
        }

        if (Input.GetMouseButtonDown(1))
        {
            StabilizeCursor();
        } 
    }

    private void ApplyStatEffects()
    {
        // Adjust stabilization duration based on Endurance
        int endurance = playerStats.Endurance.Level;
        if (endurance >= 9) stabilizationDuration = 6.0f;
        else if (endurance >= 8) stabilizationDuration = 5.5f;
        else if (endurance >= 6) stabilizationDuration = 4.5f;
        else if (endurance >= 4) stabilizationDuration = 4.0f;
        else if (endurance >= 2) stabilizationDuration = 3.5f;

        // Adjust shake amount based on Dexterity
        int confidence = playerStats.Confidence.Level; // Assuming Dexterity is added 
        if (confidence >= 8) shakeAmount = 50f;
        else if (confidence >= 6) shakeAmount = 60f;
        else if (confidence >= 4) shakeAmount = 70f;
        else if (confidence >= 2) shakeAmount = 80f;

        // Adjust travel speed based on Patience
        int patience = playerStats.Patience.Level;
        if (patience >= 8) travelSpeed = 1f;
        else if (confidence >= 6) travelSpeed = 1.5f;
        else if (confidence >= 4) travelSpeed = 2f;
        else if (confidence >= 2) travelSpeed = 2.5f; 
    }

    private void StabilizeCursor()
    {
        isStabilized = true;
        stabilizationEndTime = Time.time + stabilizationDuration;
    }

    private void RandomlyMoveCursor()
    {
        float randomX = Mathf.PerlinNoise(Time.time * travelSpeed, noiseOffsetX) * Screen.width;
        float randomY = Mathf.PerlinNoise(Time.time * travelSpeed, noiseOffsetY) * Screen.height;
        Vector2 randomPosition = new Vector2(randomX, randomY);
        Cursor.SetCursor(cursorImage, randomPosition, CursorMode.Auto);
    }
}
