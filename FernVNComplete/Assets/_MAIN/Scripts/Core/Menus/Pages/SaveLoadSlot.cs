using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VISUALNOVEL;
using System.IO;
using History;
using System;

public class SaveLoadSlot : MonoBehaviour
{
    public GameObject root;
    public RawImage previewImage;
    public TextMeshProUGUI titleText;
    public Button deleteButton;
    public Button saveButton;
    public Button loadButton;

    [HideInInspector] public int fileNumber = 0;
    [HideInInspector] public string filePath = "";

    private UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.instance;

    public void PopulateDetails(SaveAndLoadMenu.MenuFunction function)
    {
        if (File.Exists(filePath))
        {
            VNGameSave file = VNGameSave.Load(filePath);
            PopulateDetailsFromFile(function, file);
        }
        else
        {
            PopulateDetailsFromFile(function, null);
        }
    }

    private void PopulateDetailsFromFile(SaveAndLoadMenu.MenuFunction function, VNGameSave file)
    {
        bool isAutoSave = fileNumber == 1;

        if (file == null)
        {
            titleText.text = isAutoSave ? "Auto Save Slot" : $"{fileNumber}. Empty File";
            deleteButton.gameObject.SetActive(!isAutoSave);
            loadButton.gameObject.SetActive(false);
            saveButton.gameObject.SetActive(function == SaveAndLoadMenu.MenuFunction.save && !isAutoSave);
            previewImage.texture = SaveAndLoadMenu.Instance.emptyFileImage;
        }
        else
        {
            titleText.text = isAutoSave ?
                $"1. Auto Save - {file.savedSeason} {file.savedDay:D2}, {file.savedYear} - {file.savedTimeOfDay} - {file.savedDayOfWeek} - {file.timestamp}" :
                $"{fileNumber}. {file.savedSeason} {file.savedDay:D2}, {file.savedYear} - {file.savedTimeOfDay} - {file.savedDayOfWeek} - {file.timestamp}";

            deleteButton.gameObject.SetActive(!isAutoSave);
            loadButton.gameObject.SetActive(function == SaveAndLoadMenu.MenuFunction.load);
            saveButton.gameObject.SetActive(function == SaveAndLoadMenu.MenuFunction.save && !isAutoSave);

            byte[] data = File.ReadAllBytes(file.screenshotPath);
            Texture2D screenshotPreview = new Texture2D(1, 1);
            ImageConversion.LoadImage(screenshotPreview, data);
            previewImage.texture = screenshotPreview;
        }
    }

    public void Delete()
    {
        if (fileNumber == 1) return;

        uiChoiceMenu.Show(
            "Delete this file? (<i>This cannot be undone!</i>)",
            new UIConfirmationMenu.ConfirmationButton("Yes", () =>
            {
                uiChoiceMenu.Show(
                    "Are you sure?",
                    new UIConfirmationMenu.ConfirmationButton("I am sure", OnConfirmDelete),
                    new UIConfirmationMenu.ConfirmationButton("Never Mind", null));
            },
            autoCloseOnClick: false),
            new UIConfirmationMenu.ConfirmationButton("No", null));
    }

    private void OnConfirmDelete()
    {
        File.Delete(filePath);
        PopulateDetails(SaveAndLoadMenu.Instance.menuFunction);
    }

    public void Load()
    {
        VNGameSave file = VNGameSave.Load(filePath, false);
        SaveAndLoadMenu.Instance.Close(closeAllMenus: true);

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == MainMenu.MAIN_MENU_SCENE)
        {
            MainMenu.instance.LoadGame(file);
        }
        else
        {
            file.Activate();
        }
    }

    public void Save()
    {
        if (fileNumber == 1)
        {
            Debug.LogWarning("Manual saving to Auto Save Slot (Slot 1) is disabled.");
            return;
        }

        if (HistoryManager.instance.isViewingHistory)
        {
            UIConfirmationMenu.instance.Show("You cannot save while viewing history.", new UIConfirmationMenu.ConfirmationButton("Okay", null));
            return;
        }

        var activeSave = VNGameSave.activeFile;
        activeSave.slotNumber = fileNumber;
        activeSave.Save();

        PopulateDetailsFromFile(SaveAndLoadMenu.Instance.menuFunction, activeSave);
    }
}

