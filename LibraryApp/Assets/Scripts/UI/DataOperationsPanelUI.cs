using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LendingInfoPairsSO;

public class DataOperationsPanelUI : MonoBehaviour
{
    public static DataOperationsPanelUI Instance { get; private set; }

    [SerializeField] private Button closeButton;
    [SerializeField] private Button importFromJsonButton;
    [SerializeField] private Button exportToJsonButton;
    [SerializeField] private Button deleteLibraryDataButton;

    private void Awake()
    {
        Instance = this;

        closeButton.onClick.AddListener(Hide);

        importFromJsonButton.onClick.AddListener(OnImportFromJsonButtonClick);

        exportToJsonButton.onClick.AddListener(OnExportToJsonButtonClick);

        deleteLibraryDataButton.onClick.AddListener(OnDeleteLocalDataButtonClick);

        Hide();
    }

    private void OnImportFromJsonButtonClick()
    {
        string responseMessage = "You are about to import from Json data, your local data will be re-writen. Continue?";
        PopupPanelUI.Instance.ShowResponse(responseMessage, () => ImportExportManager.ImportFromJsonForBackup()
        );
    }

    private void OnExportToJsonButtonClick() {
        string responseMessage = "You are about to export to a Json file, Json file located in the directory will be  re-writen. Continue?";
        PopupPanelUI.Instance.ShowResponse(responseMessage, () => ImportExportManager.ExportToJsonForBackup()
        );
    }

    private void OnDeleteLocalDataButtonClick() {
        string responseMessage = "You are about to delete the local Library Data. Continue?";
        PopupPanelUI.Instance.ShowResponse(responseMessage, () => LibraryManager.Instance.DeleteLocalLibraryDataFromUserPrompt()
        );
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
