using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ListPanelUI;

public class PopupPanelUI : MonoBehaviour
{   
    public enum PopupType
    {
        ShowResponse,
        ShowError,
        ShowPrompt,     
    }
    public static PopupPanelUI Instance { get; private set; }


    public event EventHandler<OnInvalidInputEnteredEventArgs> OnInvalidInputEntered;
    public class OnInvalidInputEnteredEventArgs : EventArgs { public string errorMessage; }

    public delegate void ConfirmationCallback();
    public static ConfirmationCallback OnConfirmReturn;

    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private TMP_InputField popupPanelInputField;
    
    //actionButton's text will change depending on the context. It can write OK or Confirm
    [SerializeField] private Button actionButton; 
    [SerializeField] private TextMeshProUGUI actionButtonText;

    private PopupType currentPopupType;


    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);
        Hide();
        OnInvalidInputEntered += PopupPanelUI_OnInvalidInputEntered;
    }

    private void PopupPanelUI_OnInvalidInputEntered(object sender, OnInvalidInputEnteredEventArgs e)
    {
        ShowError(e.errorMessage);
    }

    public void ShowBookLendingBorrowerNamePrompt(string promptMessage, BookData bookData)
    {
        SetPopupComponents(PopupType.ShowPrompt);
        titleText.text = "Enter Borrower Name";
        mainText.text = $"You are about to borrow the book titled '{bookData.bookTitle}' by '{bookData.bookAuthor}' (ISBN: '{bookData.bookIsbn}'). The borrower is obliged to return the book within 1 month. To cancel the operation, you can press 'X'. To continue, please enter the borrower's name:";

        TMP_InputField borrowerNameInput = popupPanelInputField;
        actionButton.onClick.AddListener(() => OnConfirmButtonClick(bookData, borrowerNameInput.text));
        
    }

    public void ShowBookReturningReturnCodePrompt()
    {
        SetPopupComponents(PopupType.ShowPrompt);
        titleText.text = "Return Book";
        mainText.text = "Enter the 5-Digit Return Code\n Or Select Your Book From The 'All Lent Books' List";
        TMP_InputField returnCodeInput = popupPanelInputField;
        actionButton.onClick.AddListener(() => OnRetunrCodeEntered(returnCodeInput.text));
    }

    private void OnRetunrCodeEntered(string returnCode)
    {
        if (returnCode.Length == 5 && int.TryParse(returnCode, out _))
        {
            actionButton.onClick.RemoveAllListeners();
            LibraryManager.Instance.TryReturnLentBookByReturnCode(returnCode);
        }
        else
        {
            actionButton.onClick.RemoveAllListeners();
            OnInvalidInputEntered?.Invoke(this, new OnInvalidInputEnteredEventArgs { errorMessage = "Your Return Code Must be a 5-Digit Number." });
        }
    }


    private void OnConfirmButtonClick(BookData bookData, string borrowerName)
    {
        if (!string.IsNullOrEmpty(borrowerName))
        {
            actionButton.onClick.RemoveAllListeners();
            LibraryManager.Instance.LendABook(bookData, borrowerName);
            popupPanelInputField.text = "";
           
        }
        else
        {
            actionButton.onClick.RemoveAllListeners();
            OnInvalidInputEntered?.Invoke(this, new OnInvalidInputEnteredEventArgs { errorMessage = "Borrower Name Can't Be Empty."});
        }
    }

    //response with a confirmationCallback
    public void ShowResponse(string responseMessage, PopupPanelUI.ConfirmationCallback callback)
    {
        SetPopupComponents(PopupType.ShowResponse);
        //title text might change
        titleText.text = "Warning!!!";
        mainText.text = responseMessage;

        actionButton.onClick.AddListener(OnConfirmButtonClickedForApproval);

        OnConfirmReturn = callback;
    }

    private void OnConfirmButtonClickedForApproval()
    {
        // Trigger the callback when the confirm button is clicked
        OnConfirmReturn?.Invoke();

        //removes listeners for later usage of the panel
        actionButton.onClick.RemoveListener(OnConfirmButtonClickedForApproval);
    }


    public void ShowResponse(string responseMessage)
    {
        SetPopupComponents(PopupType.ShowResponse);
        //title text might change
        titleText.text = "Success!!!";
        mainText.text = responseMessage;
    }

    public void ShowError(string responseMessage)
    {
        SetPopupComponents(PopupType.ShowError);
        //title text might change
        titleText.text = "Error!!!";
        mainText.text = responseMessage;
    }

    public void ShowAboutInfo()
    {
        SetPopupComponents(PopupType.ShowResponse);
        titleText.text = "About";
        mainText.text = "Made by Özgen Köklü\n\nFor Velo Games\n\nAs a Task Project\n\n2024 All Rights Reserved";
        actionButtonText.text = "Return";
    }

    public void SetPopupComponents(PopupType popupType)
    {
        gameObject.SetActive(true);

        currentPopupType = popupType;

        switch (popupType)
        {
            case PopupType.ShowPrompt:
                popupPanelInputField.gameObject.SetActive(true);
                actionButton.gameObject.SetActive(true);

                break;
            case PopupType.ShowError:
                popupPanelInputField.gameObject.SetActive(false);
                actionButton.gameObject.SetActive(false);

                break;
            case PopupType.ShowResponse:
                popupPanelInputField.gameObject.SetActive(false);
                actionButton.gameObject.SetActive(true);
                actionButton.onClick.AddListener(Hide);
                break;

        }
    }

    public void Hide() 
    {
        if(currentPopupType == PopupType.ShowResponse)
        {
            actionButton.onClick.RemoveAllListeners();
        }
        popupPanelInputField.text = "";
        gameObject.SetActive(false);
    }
}
