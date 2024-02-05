using UnityEngine;
using UnityEngine.UI;


public class DataOperationsPanelUI : MonoBehaviour
{
    public static DataOperationsPanelUI Instance { get; private set; }

    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _importFromJsonButton;
    [SerializeField] private Button _exportToJsonButton;
    [SerializeField] private Button _deleteLibraryDataButton;

    private void Awake()
    {
        Instance = this;

        _closeButton.onClick.AddListener(() =>
        {
            PlayMouseClickSoundOnWindow();
            Hide();
        });

        _importFromJsonButton.onClick.AddListener(OnImportFromJsonButtonClick);

        _exportToJsonButton.onClick.AddListener(OnExportToJsonButtonClick);

        _deleteLibraryDataButton.onClick.AddListener(OnDeleteLocalDataButtonClick);

        Hide();
    }

    private void OnImportFromJsonButtonClick()
    {
        string confirmationMessage = "You are about to import from Json data, your local data will be re-writen. Continue?";
        PopupPanelUI.Instance.ShowResponse(confirmationMessage,  ImportExportManager.ImportFromJsonForBackup);
    }

    private void OnExportToJsonButtonClick() 
    {
        string confirmationMessage = "You are about to export to a Json file, Json file located in the directory will be  re-writen. Continue?";
        PopupPanelUI.Instance.ShowResponse(confirmationMessage, ImportExportManager.ExportToJsonForBackup);
    }

    private void OnDeleteLocalDataButtonClick()
    {
        string confirmationMessage = "You are about to delete the local Library Data. Continue?";
        PopupPanelUI.Instance.ShowResponse(confirmationMessage, ConfirmDeleteLocalData);
    }

    private void ConfirmDeleteLocalData()
    {
        LibraryManager.Instance.DeleteLocalLibraryDataFromUserPrompt();
        //This way, additional logic can start here (like another response)
    }

    private void PlayMouseClickSoundOnWindow()
    {
        SoundManager.Instance.PlayMouseClick();
    }
    public void Show()
    {
        gameObject.SetActive(true);
        PlayMouseClickSoundOnWindow();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
