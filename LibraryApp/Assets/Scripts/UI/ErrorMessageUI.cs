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
        ReturnLentBookPanelUI.Instance.OnInvalidReturnCodeEntered += ReturnCodePrompt_OnInvalidReturnCodeEntered;
        Hide();
    }

    private void ReturnCodePrompt_OnInvalidReturnCodeEntered(object sender, System.EventArgs e)
    {
        Show();
        errorMessageText.text = "Your Return Code Must be a 5-Digit Number.";
    }

    private void BorrowerNamePrompt_OnInvalidBorrowerNameEntered(object sender, System.EventArgs e)
    {
        Show();
        errorMessageText.text = "Borrower Name Can't Be Empty.";
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
