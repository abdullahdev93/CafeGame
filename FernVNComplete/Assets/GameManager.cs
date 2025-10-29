using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject playerScoreContainer;
    public GameObject aiScoreContainer;
    public TMP_Text scorePrefab;
    public TMP_Text theDartNumberText;
    public TMP_Text theWinText;
    public TMP_Text theBustText;

    public GameObject theWinBackground;
    public GameObject theBustBackground; 

    public GameObject turnPanel;
    public TMP_Text turnPanelText;
    public Animator turnPanelAnimator;
    public float holdDuration = 1.5f;

    public int playerScore = 501;
    public int aiScore = 501;
    public int dartNumber = 3;
    public int dartsLeft;
    public bool isPlayerTurn = true;
    public bool gameOver = false;

    private TMP_Text lastPlayerScoreEntry;
    private TMP_Text lastAIScoreEntry;

    private int previousPlayerScore;
    private int previousAIScore;

    public TMP_Text boardPointsText;
    public int accumulatedPoints;

    // List to track instantiated dart prefabs
    private List<GameObject> instantiatedDarts = new List<GameObject>();

    public Button exitGameButton; 

    void Start()
    {
        theWinBackground.SetActive(false); 
        theWinText.enabled = false;
        theBustBackground.SetActive(false); 
        theBustText.enabled = false;
        dartsLeft = dartNumber;

        if (instance == null)
        {
            instance = this;
        }

        lastPlayerScoreEntry = AddScoreEntry(playerScoreContainer, playerScore, false);
        lastAIScoreEntry = AddScoreEntry(aiScoreContainer, aiScore, false);
        previousPlayerScore = playerScore;
        previousAIScore = aiScore;

        turnPanel.SetActive(false);

        exitGameButton.gameObject.SetActive(false); 
        
        exitGameButton.onClick.AddListener(() => BackToBarButtonClicked());  
    }

    private IEnumerator PlayTurnEndAnimations()
    {
        turnPanel.SetActive(true);
        turnPanelAnimator.SetTrigger("TurnPanelEnter");
        yield return new WaitForSeconds(turnPanelAnimator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(holdDuration);
        turnPanelAnimator.SetTrigger("TurnPanelExit");
        yield return new WaitForSeconds(turnPanelAnimator.GetCurrentAnimatorStateInfo(0).length);
        turnPanel.SetActive(false);
        accumulatedPoints = 0;
        UpdateBoardPointsText(0);
        // Destroy all instantiated dart prefabs at the end of the turn
        DestroyAllDartPrefabs();
    }

    private TMP_Text AddScoreEntry(GameObject container, int score, bool isPrevious)
    {
        TMP_Text newScoreEntry = Instantiate(scorePrefab, container.transform);
        if (isPrevious)
        {
            newScoreEntry.text = $"<s>{score}</s>";
        }
        else
        {
            newScoreEntry.text = score.ToString();
        }
        return newScoreEntry;
    }

    public void ChangeDartNumber()
    {
        if (gameOver) return;

        dartsLeft--;
        theDartNumberText.text = "Dart(s): " + dartsLeft.ToString();

        if (dartsLeft <= 0)
        {
            dartsLeft = 0;
            EndTurn();
        }
    }

    public void UpdateBoardPointsText(int points)
    {
        if (boardPointsText != null)
        {
            boardPointsText.text = $"Points Earned: {points}";
        }
    }

    public void EndTurn()
    {
        if (gameOver) return;

        if (isPlayerTurn)
        {
            if (lastPlayerScoreEntry != null)
            {
                lastPlayerScoreEntry.text = $"<s>{previousPlayerScore}</s>";
            }

            if (playerScore == previousPlayerScore) return;

            previousPlayerScore = playerScore;
            lastPlayerScoreEntry = AddScoreEntry(playerScoreContainer, playerScore, false);
        }
        else
        {
            if (lastAIScoreEntry != null)
            {
                lastAIScoreEntry.text = $"<s>{previousAIScore}</s>";
            }

            if (aiScore == previousAIScore) return;

            previousAIScore = aiScore;
            lastAIScoreEntry = AddScoreEntry(aiScoreContainer, aiScore, false);     
        } 

        StartCoroutine(PlayTurnEndAnimations());

        // Destroy all instantiated dart prefabs at the end of the turn
        //DestroyAllDartPrefabs();  

        ResetDarts();

        // Destroy all instantiated dart prefabs at the end of the turn
        //DestroyAllDartPrefabs(); 
    }

    public void BackToBarButtonClicked()
    {
        // Set the starting file based on the location
        PlayerPrefs.SetString("StartingFile", "BarAfterDartsFile");  
        PlayerPrefs.Save(); 

        // Transfer to the VisualNovel scene
        SceneManager.LoadScene("VisualNovel"); 
    }

    public void ResetDarts()
    {
        if (gameOver) return;

        isPlayerTurn = !isPlayerTurn;

        if (!isPlayerTurn)
        {
            dartsLeft = dartNumber;
            DisablePlayerInput();
            StartCoroutine(AITurnRoutine());
            turnPanelText.text = "AI's Turn!";
            // Destroy all instantiated dart prefabs at the end of the turn
            //DestroyAllDartPrefabs();
        }
        else
        {
            dartsLeft = dartNumber;
            EnablePlayerInput();
            turnPanelText.text = "Player's Turn!";
            // Destroy all instantiated dart prefabs at the end of the turn
            //DestroyAllDartPrefabs(); 
        }
    }

    private void EnablePlayerInput()
    {
        if (gameOver) return;
        theDartNumberText.text = "Dart(s): " + dartsLeft.ToString();
        EnableBoardTriggerColliders(true);
    }

    private void DisablePlayerInput()
    {
        theDartNumberText.text = gameOver ? "Game Over" : "AI's Turn";
        EnableBoardTriggerColliders(false);
    }

    private void EnableBoardTriggerColliders(bool enable)
    {
        BoardTriggers[] boardTriggers = FindObjectsOfType<BoardTriggers>();
        foreach (BoardTriggers trigger in boardTriggers)
        {
            Collider2D collider = trigger.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = enable;
            }
        }
    }

    public void changeScore(int boardTriggerPoints, int boardTriggerMultiplier, bool isPlayer)
    {
        if (gameOver) return;

        int boardPoints = boardTriggerPoints * boardTriggerMultiplier;
        int newScore = isPlayer ? playerScore - boardPoints : aiScore - boardPoints;

        UpdateBoardPointsText(accumulatedPoints);

        // Bust if the score goes below 0 or exactly 1
        if (newScore < 0 || newScore == 1)
        {
            // Show the negative result briefly but do NOT save it
            TMP_Text negativeScoreEntry = null;
            if (isPlayer)
            {
                negativeScoreEntry = AddScoreEntry(playerScoreContainer, newScore, false); // Show the negative score briefly
            }
            else
            {
                negativeScoreEntry = AddScoreEntry(aiScoreContainer, newScore, false); // Show the negative score briefly
            }

            // Trigger the bust condition and destroy the negative entry after a short delay
            StartCoroutine(DestroyNegativeScoreEntry(negativeScoreEntry));
            StartCoroutine(WaitToResetFromBust(isPlayer));
            return;
        }

        // Update the player's or AI's score and save it for slashing in the next turn
        if (isPlayer)
        {
            playerScore = newScore;  // Update player score
        }
        else
        {
            aiScore = newScore;  // Update AI score
        }

        if (newScore == 0)
        {
            HandleGameOver(isPlayer);
            return;
        }
    }

    private IEnumerator DestroyNegativeScoreEntry(TMP_Text negativeScoreEntry)
    {
        // Wait for 1.5 seconds before destroying the negative score entry
        yield return new WaitForSeconds(1.5f);
        if (negativeScoreEntry != null)
        {
            Destroy(negativeScoreEntry.gameObject);  // Destroy the negative score entry
        }
    }

    private void HandleGameOver(bool isPlayer)
    {
        if (isPlayer)
        {
            if (lastPlayerScoreEntry != null)
            {
                lastPlayerScoreEntry.text = $"<s>{previousPlayerScore}</s>";
            }
            previousPlayerScore = playerScore;
            lastPlayerScoreEntry = AddScoreEntry(playerScoreContainer, playerScore, false);
        }
        else
        {
            if (lastAIScoreEntry != null)
            {
                lastAIScoreEntry.text = $"<s>{previousAIScore}</s>";
            }
            previousAIScore = aiScore;
            lastAIScoreEntry = AddScoreEntry(aiScoreContainer, aiScore, false);
        }

        gameOver = true;
        theWinText.enabled = true;
        theWinBackground.SetActive(true); 
        theWinText.text = isPlayer ? "Player Wins!" : "AI Wins!";
        DisablePlayerInput();
        EnableBoardTriggerColliders(false);

        exitGameButton.gameObject.SetActive(true);  
    }

    // Add a reference to the instantiated dart and store it in the list
    public void RegisterDartPrefab(GameObject dart)
    {
        instantiatedDarts.Add(dart);  // Add the dart prefab to the list for tracking
    }

    // Destroy all instantiated dart prefabs
    public void DestroyAllDartPrefabs()
    {
        foreach (var dart in instantiatedDarts)
        {
            if (dart != null)
            {
                Destroy(dart);  // Destroy each dart prefab
            }
        }
        instantiatedDarts.Clear();  // Clear the list after destroying all darts
    }

    public IEnumerator WaitToResetFromBust(bool isPlayer)
    {
        if (gameOver) yield break;

        theBustText.enabled = true;
        theBustBackground.SetActive(true); 
        yield return new WaitForSeconds(1.5f);  // Show Bust text and wait
        theBustText.enabled = false;
        theBustBackground.SetActive(false); 
        ResetFromBust(isPlayer);  // Reset to the previous score
    }

    public void ResetFromBust(bool isPlayer)
    {
        if (gameOver) return;  // Prevent resetting if the game is over   

        // Reset the score after a bust
        if (isPlayer)
        {
            playerScore = previousPlayerScore;  // Reset player's score 
            turnPanelText.text = "AI's Turn!";
            // Destroy all instantiated dart prefabs at the end of the turn
            //DestroyAllDartPrefabs();
        }
        else
        {
            aiScore = previousAIScore;  // Reset AI's score 
            turnPanelText.text = "Player's Turn!";
            // Destroy all instantiated dart prefabs at the end of the turn
            //DestroyAllDartPrefabs(); 
        }

        // Reset darts for both player and AI after a bust
        dartsLeft = dartNumber;  // Ensure dartsLeft is reset to the full count (3)

        // Switch turns after a bust
        if (isPlayer)
        {
            isPlayerTurn = false;
            StartCoroutine(AITurnRoutine());
        }
        else
        {
            isPlayerTurn = true;
            EnablePlayerInput();  // Enable player input to start the player's turn
        }
    }

    public IEnumerator AITurnRoutine()
    {
        if (gameOver) yield break;

        dartsLeft = dartNumber;
        yield return StartCoroutine(PlayTurnEndAnimations());
        // Destroy all instantiated dart prefabs at the end of the turn
        DestroyAllDartPrefabs(); 
        yield return new WaitForSeconds(1.0f);

        while (dartsLeft > 0)
        {
            if (isPlayerTurn || gameOver) yield break;

            int boardPoints;
            int multiplier;

            if (aiScore <= 50)
            {
                multiplier = 2;
                boardPoints = aiScore / 2;
                if (aiScore % 2 != 0 || aiScore == 1)
                {
                    multiplier = 1;
                    boardPoints = 1;
                }
            }
            else if (aiScore > 100)
            {
                multiplier = Random.Range(1, 4);
                boardPoints = Random.Range(15, 21);
            }
            else
            {
                multiplier = Random.Range(1, 3);
                boardPoints = Random.Range(10, 20);
            }

            BoardTriggers boardTrigger = FindBoardTrigger(boardPoints, multiplier);
            if (boardTrigger != null)
            {
                boardTrigger.HandleDartThrow(false);
            }

            yield return new WaitForSeconds(1.5f);
        }

        lastAIScoreEntry = AddScoreEntry(aiScoreContainer, aiScore, false);
        EndTurn();
    }

    public BoardTriggers FindBoardTrigger(int points, int multiplier)
    {
        BoardTriggers[] boardTriggers = FindObjectsOfType<BoardTriggers>();

        foreach (BoardTriggers trigger in boardTriggers)
        {
            if (trigger.boardTriggerPoints == points && trigger.boardTriggerMultiplier == multiplier)
            {
                return trigger;
            }
        }

        return null;
    }
}
