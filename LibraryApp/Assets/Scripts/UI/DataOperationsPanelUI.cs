using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataOperationsPanelUI : MonoBehaviour
{
    public static DataOperationsPanelUI Instance { get; private set; }

    private const string initialPanelMessage = "Please be advised that the Import operation will overwrite the existing Library Data and Lending Information. \r\n\r\nThe Export feature is primarily intended for backup purposes.";
    [SerializeField] private Button closeButton;
    [SerializeField] private Button importFromJsonButton;
    [SerializeField] private Button exportToJsonButton;
    [SerializeField] private Button deleteLocalLibraryDataButton;
  
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    [SerializeField] private TextMeshProUGUI panelMessage;

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);

        //NEED TO ADD USER PROMPTS AND CONFIRMATION

        importFromJsonButton.onClick.AddListener(() =>
        {
            
            //Set text to you are about to import bla bla
            //makes the other buttons disappear
            //Confirm & cancel buttons appear
            //this might work as a callback?
            panelMessage.text = "LibraryData & Lending List imported to Json";
            ImportExportManager.ImportFromJson();
        });

        exportToJsonButton.onClick.AddListener(() =>
        {
            panelMessage.text = "LibraryData & Lending List exported to Json";

            ImportExportManager.ExportToJson();

            //Set text to "JSON successfully exported to directory" 

        });

        deleteLocalLibraryDataButton.onClick.AddListener(() =>
        {
            panelMessage.text = "You are about to delete local library data that is stored in this application.";

            LibraryManager.Instance.DeleteLocalLibraryData();

            //Library Successfully deleted response

        });

        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        panelMessage.text = initialPanelMessage;
        confirmButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
