using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI errorMessageText;
    [SerializeField] private Button closeButton; 

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
       
    }
    private void Start()
    {
        LibraryManager.Instance.OnErrorEncountered += LibraryManager_OnErrorEncountered;
        AddNewBookPanelUI.Instance.OnInvalidInput += AddNewBookPanelUI_OnInvalidInput;
        LendABookBorrowerNamePromptPanelUI.Instance.OnInvalidBorrowerNameEntered += BorrowerNamePrompt_OnInvalidBorrowerNameEntered;
        Hide();
    }

    private void BorrowerNamePrompt_OnInvalidBorrowerNameEntered(object sender, System.EventArgs e)
    {
        Show();
        errorMessageText.text = "Invalid User Name";
    }

    private void AddNewBookPanelUI_OnInvalidInput(object sender, AddNewBookPanelUI.OnInvalidInputEventArgs e)
    {
        Show();
        errorMessageText.text = e.invalidInputErrorMessage;
    }

    private void LibraryManager_OnErrorEncountered(object sender, LibraryManager.OnErrorEncounteredEventArgs e)
    {
        Show();
        errorMessageText.text = e.errorMessage;
      
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
