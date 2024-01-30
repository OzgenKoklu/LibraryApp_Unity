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
        ShowBookAddOrRemovePanel,
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

    //For Add & Remove book Subpanel
    [SerializeField] Transform addOrRemoveBookSubpanel;
    [SerializeField] private Button removeListingButton;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private TMP_InputField bookCountInputField;

    private PopupType currentPopupType;


    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);
        Hide();
        OnInvalidInputEntered += PopupPanelUI_OnInvalidInputEntered;
    }

    //State Machine for object activation
    public void SetPopupComponents(PopupType popupType)
    {
        gameObject.SetActive(true);

        currentPopupType = popupType;

        switch (popupType)
        {
            case PopupType.ShowPrompt:
                popupPanelInputField.gameObject.SetActive(true);
                actionButton.gameObject.SetActive(true);
                addOrRemoveBookSubpanel.gameObject.SetActive(false);

                break;
            case PopupType.ShowError:
                popupPanelInputField.gameObject.SetActive(false);
                actionButton.gameObject.SetActive(false);
                addOrRemoveBookSubpanel.gameObject.SetActive(false);

                break;
            case PopupType.ShowResponse:
                popupPanelInputField.gameObject.SetActive(false);
                actionButton.gameObject.SetActive(true);
                addOrRemoveBookSubpanel.gameObject.SetActive(false);
                actionButton.onClick.AddListener(Hide);
                break;
            case PopupType.ShowBookAddOrRemovePanel:
                addOrRemoveBookSubpanel.gameObject.SetActive(true);
                bookCountInputField.onValueChanged.AddListener(LimitToInteger);
                actionButton.gameObject.SetActive(false);
                popupPanelInputField.gameObject.SetActive(false);
                bookCountInputField.text = "";
                break;

        }
    }

    public void Hide()
    {
        if (currentPopupType == PopupType.ShowResponse)
        {
            actionButton.onClick.RemoveAllListeners();
        }

        if (currentPopupType == PopupType.ShowBookAddOrRemovePanel)
        {
            bookCountInputField.onValueChanged.RemoveAllListeners();
            RemoveListenerForAddorRemoveBookPanel();
        }

        popupPanelInputField.text = "";

        gameObject.SetActive(false);
    }

    #region All Purpose Methods

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

    //this is truely under utilized. Should actually use this for error handling (not directly using Popuppanel.Instance.ShowError like I do a looot
    private void PopupPanelUI_OnInvalidInputEntered(object sender, OnInvalidInputEnteredEventArgs e)
    {
        ShowError(e.errorMessage);
    }
    public void ShowError(string responseMessage)
    {
        SetPopupComponents(PopupType.ShowError);
        //title text might change
        titleText.text = "Error!!!";
        mainText.text = responseMessage;
    }
    void LimitToInteger(string newValue)
    {
        int intValue;
        //if input does not parse to an integer, delete it
        if (!int.TryParse(newValue, out intValue))
        {
            // If parsing fails, set the input field text to empty
            bookCountInputField.text = "";
        }
    }

    #endregion

    #region Lending PopUp Methods
    public void ShowBookLendingBorrowerNamePrompt(string promptMessage, BookData bookData)
    {
        SetPopupComponents(PopupType.ShowPrompt);
        titleText.text = "Enter Borrower Name";
        mainText.text = $"You are about to borrow the book titled '{bookData.bookTitle}' by '{bookData.bookAuthor}' (ISBN: '{bookData.bookIsbn}'). The borrower is obliged to return the book within 1 month. To cancel the operation, you can press 'X'. To continue, please enter the borrower's name:";

        TMP_InputField borrowerNameInput = popupPanelInputField;
        actionButton.onClick.AddListener(() => OnConfirmBorrowerNameButtonClick(bookData, borrowerNameInput.text));

    }
    private void OnConfirmBorrowerNameButtonClick(BookData bookData, string borrowerName)
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
            OnInvalidInputEntered?.Invoke(this, new OnInvalidInputEnteredEventArgs { errorMessage = "Borrower Name Can't Be Empty." });
        }
    }

    #endregion

    #region Return Code Popup methods
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
        actionButton.onClick.RemoveAllListeners();
        if (returnCode.Length == 5 && int.TryParse(returnCode, out _))
        {
            LibraryManager.Instance.TryReturnLentBookByReturnCode(returnCode);
        }
        else
        {
            OnInvalidInputEntered?.Invoke(this, new OnInvalidInputEnteredEventArgs { errorMessage = "Your Return Code Must be a 5-Digit Number." });
        }
    }

    #endregion

    #region Add or Remove Book Panel Popup Methods
    public void ShowAddOrRemoveBookPanel(BookData bookData, string infoMessage)
    {
        SetPopupComponents(PopupType.ShowBookAddOrRemovePanel);
        titleText.text = "Add or Remove Book";
        mainText.text = infoMessage;

        TMP_InputField bookCountInput = bookCountInputField;

        removeListingButton.onClick.AddListener(() => OnRemoveButtonClicked(bookData));
        minusButton.onClick.AddListener(() => OnMinusButtonClicked(bookData, bookCountInput.text));
        plusButton.onClick.AddListener(() => OnPlusButtonClicked(bookData, bookCountInput.text));
    }
    private void OnMinusButtonClicked(BookData bookData, string bookCountToDerease)
    {
        if (bookData.bookCount == 0)
        {
            string errorMessage = "Can't decrease any number of coppies";
            PopupPanelUI.Instance.ShowError(errorMessage);
            return;
        }
        //already accepting int only input so checking if its empty is enough
        if (bookCountToDerease != "")
        {
            RemoveListenerForAddorRemoveBookPanel();
            string responseMessage = $"You are about to decrease the number of copies of '{bookData.bookTitle}' (ISBN: '{bookData.bookIsbn}') by '{bookCountToDerease}'. Please Confirm: ";
            PopupPanelUI.Instance.ShowResponse(responseMessage, () => LibraryManager.Instance.DecreaseTheNumberOfBooks(bookData, bookCountToDerease));
        }
    }
    private void OnPlusButtonClicked(BookData bookData, string bookCountToIncrease)
    {
        if (bookCountToIncrease != "")
        {
            RemoveListenerForAddorRemoveBookPanel();
            string responseMessage = $"You are about to increase the number of copies of '{bookData.bookTitle}' (ISBN: '{bookData.bookIsbn}') by '{bookCountToIncrease}'. Please Confirm: ";
            PopupPanelUI.Instance.ShowResponse(responseMessage, () => LibraryManager.Instance.IncreaseTheNumberOfBooks(bookData, bookCountToIncrease));
        }
    }

    private void OnRemoveButtonClicked(BookData bookData)
    {
        RemoveListenerForAddorRemoveBookPanel();
        string responseMessage = $"You are about to remove any data related with '{bookData.bookTitle}' (ISBN: '{bookData.bookIsbn}') including the lending information stored about this book. Continue?";
        PopupPanelUI.Instance.ShowResponse(responseMessage, () => LibraryManager.Instance.DeleteSingleBookDataInformation(bookData));
    }

    public void RemoveListenerForAddorRemoveBookPanel()
    {
        removeListingButton.onClick.RemoveAllListeners();
        minusButton.onClick.RemoveAllListeners();
        plusButton.onClick.RemoveAllListeners();
    }


    #endregion

    public void ShowAboutInfo()
    {
        SetPopupComponents(PopupType.ShowResponse);
        titleText.text = "About";
        mainText.text = "Made by Özgen Köklü\n\nFor Velo Games\n\nAs a Task Project\n\n2024 All Rights Reserved";
        actionButtonText.text = "Return";
    }
}