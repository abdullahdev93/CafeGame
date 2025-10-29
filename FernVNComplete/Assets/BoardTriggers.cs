using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardTriggers : MonoBehaviour
{
    public int boardTriggerPoints;      // The points for this specific board area (e.g., 20, 1, 5)
    public int boardTriggerMultiplier;  // The multiplier for the hit (e.g., single, double, triple)

    public GameObject floatingTextPrefab;  // Prefab for showing points in text 
    public GameObject dartPrefab;          // Prefab for spawning a dart after a throw 
    //public TMP_Text boardPointsText;       // Text component to display total accumulated board points

    private int boardPoints;
    //private int accumulatedPoints;     // To store the accumulated points
    public bool dartThrown = false;        // Ensure a dart can only be thrown once per click

    void Start()
    {
        boardPoints = boardTriggerPoints * boardTriggerMultiplier;  // Calculate the total points for this area
        //UpdateBoardPointsText();  // Initialize the board points text
    } 

    //void Update()
    //{
        //UpdateBoardPointsText(accumulatedPoints); 
    //}

    // Method to handle the player's click on the dartboard
    void OnMouseDown() 
    {
        if (dartThrown == true || !GameManager.instance.isPlayerTurn)
            return;  // Prevent multiple darts being thrown in the same click or if it's not the player's turn

        HandleDartThrow(true);  // Handle dart throw for player
    }

    // Method to handle dart throws, allowing AI and Player to use it
    public void HandleDartThrow(bool isPlayer)
    {
        if (dartThrown) return;  // Prevents multiple throws on the same board area

        // Increment the accumulated points
        GameManager.instance.accumulatedPoints += boardPoints; 
        ShowPoints(boardPoints.ToString());  // Display points hit 

        // Instantiate the Dart Prefab at the same location as the floatingTextPrefab
        if (dartPrefab != null)
        {
            GameObject dartInstance =Instantiate(dartPrefab, transform.position, Quaternion.identity);  // Spawn dart at the same location 
            GameManager.instance.RegisterDartPrefab(dartInstance);  // Register the dart with GameManager 
        } 

        // Update the score by calling GameManager's changeScore method
        GameManager.instance.changeScore(boardTriggerPoints, boardTriggerMultiplier, isPlayer);

        // Decrease the dart count by calling ChangeDartNumber from GameManager
        GameManager.instance.ChangeDartNumber();  // Decrease the dart count

        dartThrown = true;  // Prevent further interaction until reset
        StartCoroutine(DartToFalse());  // Reset after a delay
    }

    // Method to show floating text and update the accumulated points display
    void ShowPoints(string text)
    {
        if (floatingTextPrefab) 
        {
            GameObject thePrefab = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            thePrefab.GetComponentInChildren<TextMeshPro>().text = text;  // Set the text to show board points
        }

        //UpdateBoardPointsText();  // Update the displayed accumulated points
    } 

    // Method to update the displayed accumulated points
    //void UpdateBoardPointsText()
    //{
        //if (GameManager.instance.boardPointsText != null) 
        //{
            //GameManager.instance.boardPointsText.text = $"Points Earned: {accumulatedPoints}";  // Display total accumulated points
        //}
    //}

    // Coroutine to reset dartThrown after a delay
    public IEnumerator DartToFalse()
    {
        yield return new WaitForSeconds(1.5f);
        dartThrown = false;  // Allow the next dart to be thrown after a delay
    }
}
