using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;
using static AddNewBookPanelUI;

public class LendABookBorrowerNamePromptPanelUI : MonoBehaviour
{
    public static LendABookBorrowerNamePromptPanelUI Instance { get; private set; }

    public event EventHandler<EventArgs> OnInvalidBorrowerNameEntered;

    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_InputField borrowerNameInputField;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private Button confirmButton;
    

    private void Awake()
    {
        Instance = this;

        closeButton.onClick.AddListener(Hide);
        Hide();
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
    private void SetPromptText(BookData bookData)
    {
        promptText.text = $"You are about to borrow the book titled '{bookData.bookTitle}' by '{bookData.bookAuthor}' (ISBN: '{bookData.bookIsbn}'). The borrower is obliged to return the book within 1 month. To cancel the operation, you can press 'X'. To continue, please enter the borrower's name:";
    }
    public void Show(BookData bookData)
    {
        gameObject.SetActive(true);
        borrowerNameInputField.text = "";
        SetPromptText(bookData);

        //Holding a reference to the Input field so that lambda expression can work
        TMP_InputField borrowerNameInput = borrowerNameInputField;
        confirmButton.onClick.AddListener(() => OnConfirmButtonClick(bookData, borrowerNameInput.text)); ;
    }
    private void Hide()
    {
        //removes listeners
        confirmButton.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }

}
