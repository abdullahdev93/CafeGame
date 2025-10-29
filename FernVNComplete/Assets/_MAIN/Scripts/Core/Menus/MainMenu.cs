using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using VISUALNOVEL;

public class MainMenu : MonoBehaviour
{
    public const string MAIN_MENU_SCENE = "MainMenu";

    public static MainMenu instance { get; private set; }

    //public AudioClip menuMusic;
    //public CanvasGroup mainPanel;
    public CanvasGroup saveLoadPanel; 
    //private CanvasGroupController mainCG;
    private CanvasGroupController saveLoadRootCG;

    //public GameObject saveLoadRoot; 

    //[SerializeField] private GalleryMenu gallery;

    private UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //mainCG = new CanvasGroupController(this, mainPanel);
        saveLoadRootCG = new CanvasGroupController(this, saveLoadPanel);  

        //AudioManager.instance.StopAllSoundEffects();
        //AudioManager.instance.StopAllTracks();

        //AudioManager.instance.PlayTrack(menuMusic, channel: 0, startingVolume: 1);
    }

    public void Click_StartNewGame()
    {
        uiChoiceMenu.Show("Start a new game?", new UIConfirmationMenu.ConfirmationButton("Yes", StartNewGame), new UIConfirmationMenu.ConfirmationButton("No", null));
    } 

    public void Click_LoadCurrentGame()
    {
        uiChoiceMenu.Show("Load current game?", new UIConfirmationMenu.ConfirmationButton("Yes", StartingGame), new UIConfirmationMenu.ConfirmationButton("No", null));
    } 

    public void LoadGame(VNGameSave file)
    {
        VNGameSave.activeFile = file;
        //StartCoroutine(StartingGame());
        StartingGame();
    }

    private void StartNewGame()
    {
        HandleStartingFile("Introduction");  
        //SceneManager.LoadScene("VisualNovel");
        //VNGameSave.activeFile = new VNGameSave();
        //StartCoroutine(StartingGame());
        //StartingGame(); 
    }

    private void HandleStartingFile(string locationFile)
    {
        // Set the starting file based on the location
        PlayerPrefs.SetString("StartingFile", locationFile);
        PlayerPrefs.Save();

        // Transfer to the VisualNovel scene
        SceneManager.LoadScene("VisualNovel");
    }

    public void Click_LoadSaveFiles()
    {
        saveLoadRootCG.alpha = 1; 

        saveLoadRootCG.SetInteractableState(true);  

        VNMenuManager.instance.OpenLoadPage();  
          
    } 

    public void CloseSaveFiles()
    {
        saveLoadRootCG.Hide();  

        //saveLoadCG.alpha = 0; 

        saveLoadRootCG.SetInteractableState(false); 

        //VNMenuManager.instance.CloseRoot();   
    }

    public void /*IEnumerator*/ StartingGame() 
    {
        //mainCG.Hide(speed: 0.3f);
        AudioManager.instance.StopTrack(0);

        //while (mainCG.isVisible)
            //yield return null;

        //VN_Configuration.activeConfig.Save();

        SceneManager.LoadScene("VisualNovel");
    }
}
