using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


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

    [SerializeField] private Button _closeButton;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private TMP_InputField _popupPanelInputField;
    
    //actionButton's text will change depending on the context. It can write OK or Confirm
    [SerializeField] private Button _actionButton; 
    [SerializeField] private TextMeshProUGUI _actionButtonText;

    //For Add & Remove book Subpanel
    [SerializeField] Transform _addOrRemoveBookSubpanel;
    [SerializeField] private Button _removeListingButton;
    [SerializeField] private Button _plusButton;
    [SerializeField] private Button _minusButton;
    [SerializeField] private TMP_InputField _bookCountInputField;

    //For setting Window Tint for different contexts
    [SerializeField] private Transform _windowTint;
    [SerializeField] private Color _defaultWindowTintColor;
    [SerializeField] private Color _errorWindowTintColor;
    [SerializeField] private Color _successWindowTintColor;
    [SerializeField] private Color _warningWindowTintColor;


    private PopupType _currentPopupType;


    private void Awake()
    {
        Instance = this;
        _closeButton.onClick.AddListener(Hide);
        _windowTint.GetComponent<Image>().color = _defaultWindowTintColor;
        OnInvalidInputEntered += PopupPanelUI_OnInvalidInputEntered;
        Hide();
    }

    //State Machine for object activation
    public void SetPopupComponents(PopupType popupType)
    {
        gameObject.SetActive(true);

        _currentPopupType = popupType;

        switch (popupType)
        {
            case PopupType.ShowPrompt:
                _popupPanelInputField.gameObject.SetActive(true);
                _actionButton.gameObject.SetActive(true);
                _actionButtonText.text = "Enter";
                _addOrRemoveBookSubpanel.gameObject.SetActive(false);
                _windowTint.GetComponent<Image>().color = _defaultWindowTintColor;

                break;
            case PopupType.ShowError:
                _popupPanelInputField.gameObject.SetActive(false);
                _actionButton.gameObject.SetActive(false);
                _addOrRemoveBookSubpanel.gameObject.SetActive(false);
                _windowTint.GetComponent<Image>().color = _errorWindowTintColor;

                break;
            case PopupType.ShowResponse:
                _popupPanelInputField.gameObject.SetActive(false);
                _actionButton.gameObject.SetActive(true);
                _actionButtonText.text = "Confirm";
                _addOrRemoveBookSubpanel.gameObject.SetActive(false);
                _windowTint.GetComponent<Image>().color = _defaultWindowTintColor;
                _actionButton.onClick.AddListener(Hide);
                break;
            case PopupType.ShowBookAddOrRemovePanel:
                _addOrRemoveBookSubpanel.gameObject.SetActive(true);
                _bookCountInputField.onValueChanged.AddListener(LimitToInteger);
                _windowTint.GetComponent<Image>().color = _defaultWindowTintColor;
                _actionButton.gameObject.SetActive(false);
                _popupPanelInputField.gameObject.SetActive(false);
                _bookCountInputField.text = "";
                break;

        }
    }

    public void Hide()
    {
        if (_currentPopupType == PopupType.ShowResponse)
        {
            _actionButton.onClick.RemoveAllListeners();
        }

        if (_currentPopupType == PopupType.ShowBookAddOrRemovePanel)
        {
            _bookCountInputField.onValueChanged.RemoveAllListeners();
            RemoveListenerForAddorRemoveBookPanel();
        }

        _popupPanelInputField.text = "";

        gameObject.SetActive(false);
    }

    #region All Purpose Methods

    //response with a confirmationCallback
    public void ShowResponse(string responseMessage, PopupPanelUI.ConfirmationCallback callback)
    {
        SetPopupComponents(PopupType.ShowResponse);
        //title text might change
        _titleText.text = "Warning!!!";
        _mainText.text = responseMessage;
        _windowTint.GetComponent<Image>().color = _warningWindowTintColor;

        _actionButton.onClick.AddListener(OnConfirmButtonClickedForApproval);

        OnConfirmReturn = callback;
    }
    private void OnConfirmButtonClickedForApproval()
    {
        // Trigger the callback when the confirm button is clicked
        OnConfirmReturn?.Invoke();

        //removes listeners for later usage of the panel
        _actionButton.onClick.RemoveListener(OnConfirmButtonClickedForApproval);
    }

    public void ShowResponse(string responseMessage)
    {
        SetPopupComponents(PopupType.ShowResponse);
        //title text might change
        _titleText.text = "Success!!!";
        _windowTint.GetComponent<Image>().color = _successWindowTintColor;
        _mainText.text = responseMessage;
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
        _titleText.text = "Error!!!";
        _mainText.text = responseMessage;
    }
    void LimitToInteger(string newValue)
    {
        int intValue;
        //if input does not parse to an integer, delete it
        if (!int.TryParse(newValue, out intValue))
        {
            // If parsing fails, set the input field text to empty
            _bookCountInputField.text = "";
        }
    }

    #endregion

    #region Lending PopUp Methods
    public void ShowBookLendingBorrowerNamePrompt(string promptMessage, BookData bookData)
    {
        SetPopupComponents(PopupType.ShowPrompt);
        _titleText.text = "Enter Borrower Name";
        _mainText.text = $"You are about to borrow the book titled '{bookData.BookTitle}' by '{bookData.BookAuthor}' (ISBN: '{bookData.BookIsbn}'). The borrower is obliged to return the book within 1 month. To cancel the operation, you can press 'X'. To continue, please enter the borrower's name:";

        TMP_InputField borrowerNameInput = _popupPanelInputField;
        _actionButton.onClick.AddListener(() => OnConfirmBorrowerNameButtonClick(bookData, borrowerNameInput.text));

    }
    private void OnConfirmBorrowerNameButtonClick(BookData bookData, string borrowerName)
    {
        if (!string.IsNullOrEmpty(borrowerName))
        {
            _actionButton.onClick.RemoveAllListeners();
            LibraryManager.Instance.LendABook(bookData, borrowerName);
            _popupPanelInputField.text = "";

        }
        else
        {
            _actionButton.onClick.RemoveAllListeners();
            OnInvalidInputEntered?.Invoke(this, new OnInvalidInputEnteredEventArgs { errorMessage = "Borrower Name Can't Be Empty." });
        }
    }

    #endregion

    #region Return Code Popup methods
    public void ShowBookReturningReturnCodePrompt()
    {
        SetPopupComponents(PopupType.ShowPrompt);
        _titleText.text = "Return Book";
        _mainText.text = "Enter the 5-Digit Return Code\n Or Select Your Book From The 'All Lent Books' List";
        TMP_InputField returnCodeInput = _popupPanelInputField;
        _actionButton.onClick.AddListener(() => OnRetunrCodeEntered(returnCodeInput.text));
    }

    private void OnRetunrCodeEntered(string returnCode)
    {
        _actionButton.onClick.RemoveAllListeners();
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
        _titleText.text = "Add or Remove Book";
        _mainText.text = infoMessage;

        TMP_InputField bookCountInput = _bookCountInputField;

        _removeListingButton.onClick.AddListener(() => OnRemoveButtonClicked(bookData));
        _minusButton.onClick.AddListener(() => OnMinusButtonClicked(bookData, bookCountInput.text));
        _plusButton.onClick.AddListener(() => OnPlusButtonClicked(bookData, bookCountInput.text));
    }
    private void OnMinusButtonClicked(BookData bookData, string bookCountToDerease)
    {
        if (bookData.BookCount == 0)
        {
            string errorMessage = "Can't decrease any number of coppies";
            PopupPanelUI.Instance.ShowError(errorMessage);
            return;
        }
        //already accepting int only input so checking if its empty is enough
        if (bookCountToDerease != "")
        {
            RemoveListenerForAddorRemoveBookPanel();
            string responseMessage = $"You are about to decrease the number of copies of '{bookData.BookTitle}' (ISBN: '{bookData.BookIsbn}') by '{bookCountToDerease}'. Please Confirm: ";
            PopupPanelUI.Instance.ShowResponse(responseMessage, () => LibraryManager.Instance.DecreaseTheNumberOfBooks(bookData, bookCountToDerease));
        }
    }
    private void OnPlusButtonClicked(BookData bookData, string bookCountToIncrease)
    {
        if (bookCountToIncrease != "")
        {
            RemoveListenerForAddorRemoveBookPanel();
            string responseMessage = $"You are about to increase the number of copies of '{bookData.BookTitle}' (ISBN: '{bookData.BookIsbn}') by '{bookCountToIncrease}'. Please Confirm: ";
            PopupPanelUI.Instance.ShowResponse(responseMessage, () => LibraryManager.Instance.IncreaseTheNumberOfBooks(bookData, bookCountToIncrease));
        }
    }

    private void OnRemoveButtonClicked(BookData bookData)
    {
        RemoveListenerForAddorRemoveBookPanel();
        string responseMessage = $"You are about to remove any data related with '{bookData.BookTitle}' (ISBN: '{bookData.BookIsbn}') including the lending information stored about this book. Continue?";
        PopupPanelUI.Instance.ShowResponse(responseMessage, () => LibraryManager.Instance.DeleteSingleBookDataInformation(bookData));
    }

    public void RemoveListenerForAddorRemoveBookPanel()
    {
        _removeListingButton.onClick.RemoveAllListeners();
        _minusButton.onClick.RemoveAllListeners();
        _plusButton.onClick.RemoveAllListeners();
    }


    #endregion

    public void ShowAboutInfo()
    {
        SetPopupComponents(PopupType.ShowResponse);
        _titleText.text = "About";
        _mainText.text = "Made by Özgen Köklü\n\nFor Velo Games\n\nAs a Task Project\n\n2024 All Rights Reserved";
        _actionButtonText.text = "Return";
    }
}