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

    public event EventHandler<EventArgs> OnInvalidBorrowerNameEntered;

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
        OnInvalidBorrowerNameEntered += PopupPanel_OnInvalidBorrowerNameEntered;
    }

    private void PopupPanel_OnInvalidBorrowerNameEntered(object sender, EventArgs e)
    {
        //not good. We need to do better error handling with this new setup.
        ShowError("INVALID NAME");
    }

    public void ShowPrompt(string promptMessage, BookData bookData)
    {
        //this is not ideal yet. return code will also use Show prompt, need better naming and/or better usage
        SetPopupComponents(PopupType.ShowPrompt);
        mainText.text = $"You are about to borrow the book titled '{bookData.bookTitle}' by '{bookData.bookAuthor}' (ISBN: '{bookData.bookIsbn}'). The borrower is obliged to return the book within 1 month. To cancel the operation, you can press 'X'. To continue, please enter the borrower's name:";
        
        //this allows input field to be tracked as a parameter for the onconfirmButtonClick 
        TMP_InputField borrowerNameInput = popupPanelInputField;

        actionButton.onClick.AddListener(()=> OnConfirmButtonClick(bookData, borrowerNameInput.text)); ;
    }


    private void OnConfirmButtonClick(BookData bookData, string borrowerName)
    {
        if (!string.IsNullOrEmpty(borrowerName))
        {
            //we already lend available books so no need to check availability
            LibraryManager.Instance.LendABook(bookData, borrowerName);
            Hide();
        }
        else
        {
            OnInvalidBorrowerNameEntered?.Invoke(this, new EventArgs());
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
        mainText.text = "Made by Özgen Köklü";
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
        gameObject.SetActive(false);
    }
}
