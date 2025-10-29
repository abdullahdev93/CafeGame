using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; 
    public GameObject panelBackground;
    public GameObject recipesMenuUI; 
    public Button pauseButton;
    public Button continueButton;
    public Button recipesButton; 
    public Button exitButton;

    private bool isPaused = false;

    void Start()
    {
        panelBackground.SetActive(false);
        pauseMenuUI.SetActive(false); // Ensure the pause menu is hidden at the start

        // Add event listeners to the buttons
        continueButton.onClick.AddListener(ResumeGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        recipesMenuUI.SetActive(false); 
        panelBackground.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        SetUIInteractable(false); // Make other UI elements non-interactable
        SetSpriteColliders(false); // Make sprites non-interactable
        isPaused = true;
    } 

    public void ShowRecipes()
    {
        pauseMenuUI.SetActive(false); 
        recipesMenuUI.SetActive(true);
        panelBackground.SetActive(true);
        Time.timeScale = 0f;
        SetUIInteractable(false); // Make other UI elements non-interactable
        SetSpriteColliders(false); // Make sprites non-interactable
        isPaused = true; 
    } 

    public void BackToPauseMenu()
    {
        pauseMenuUI.SetActive(true);
        recipesMenuUI.SetActive(false);
        panelBackground.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        SetUIInteractable(false); // Make other UI elements non-interactable
        SetSpriteColliders(false); // Make sprites non-interactable
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        recipesMenuUI.SetActive(false); 
        panelBackground.SetActive(false);
        Time.timeScale = 1f; // Resume the game
        SetUIInteractable(true); // Make other UI elements interactable
        SetSpriteColliders(true); // Make sprites interactable
        isPaused = false;
    }

    public void ExitGame()
    {
        Time.timeScale = 1f; // Ensure the game is not paused when exiting
        SceneManager.LoadScene("MainMenu"); // Replace with your main menu scene name
    } 

    private void SetUIInteractable(bool state)
    {
        // Find all GraphicRaycaster components in the scene
        GraphicRaycaster[] raycasters = FindObjectsOfType<GraphicRaycaster>();

        foreach (GraphicRaycaster raycaster in raycasters)
        {
            // Disable/enable all raycasters except for the ones in the pause menu
            if (!pauseMenuUI.transform.IsChildOf(raycaster.transform))
            {
                raycaster.enabled = state;
            }
        }
    }

    private void SetSpriteColliders(bool state)
    {
        // Find all Collider2D components in the scene
        Collider2D[] colliders = FindObjectsOfType<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            // Disable/enable all colliders
            collider.enabled = state;
        }
    }
}
