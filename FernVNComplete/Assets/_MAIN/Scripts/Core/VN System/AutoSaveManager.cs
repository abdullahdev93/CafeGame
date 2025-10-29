using UnityEngine;
using UnityEngine.UI; 
using VISUALNOVEL;

public class AutoSaveManager : MonoBehaviour
{
    public static AutoSaveManager instance;

    [Header("Auto Save Settings")]
    public bool autoSaveEnabled = true;

    [Header("UI Elements")]
    public Image autoSaveUI;
    public float displayDuration = 2f;

    private void Start()
    {
        autoSaveUI.gameObject.SetActive(false); 
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void AutoSave()
    {
        if (!autoSaveEnabled || VNGameSave.activeFile == null || VNGameSave.activeFile.slotNumber != 1)
            return; 

        VNGameSave.activeFile.Save();
        Debug.Log("<color=green>[AutoSave]</color> Auto-saved to Slot 1 at: " + System.DateTime.Now.ToString("hh:mm:ss tt"));
        
        ShowAutoSaveUI();
    }

    public void TriggerAutoSave()
    {
        AutoSave(); 
    }

    public void ToggleAutoSave(bool enabled)
    {
        autoSaveEnabled = enabled;
    }

    private void ShowAutoSaveUI()
    {
        if (autoSaveUI != null)
        {
            autoSaveUI.gameObject.SetActive(true);
            CancelInvoke(nameof(HideAutoSaveUI));
            Invoke(nameof(HideAutoSaveUI), displayDuration);
        }
    }

    private void HideAutoSaveUI()
    {
        if (autoSaveUI != null)
            autoSaveUI.gameObject.SetActive(false);
    }
}
