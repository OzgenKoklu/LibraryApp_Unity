using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddOrRemoveBookPanelUI : MonoBehaviour
{
    public static AddOrRemoveBookPanelUI Instance { get; private set; }

    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _addBookButton;
    [SerializeField] private Button _addOrRemoveFromList;
    [SerializeField] private TMP_InputField _bookTitleInputField;
    [SerializeField] private TMP_InputField _bookAuthorInputField;
    [SerializeField] private TMP_InputField _bookIsbnInputField;

    private void Awake()
    {
        Instance = this;
        _closeButton.onClick.AddListener(Hide);
        _addBookButton.onClick.AddListener(TryAddingBook);
        _addOrRemoveFromList.onClick.AddListener(() =>
        {
            ListPanelUI.Instance.Show(ListPanelUI.ListType.AddOrRemovePanelList);
        });
        Hide();
    }


    public void TryAddingBook()
    {
        //if already listed, addbook increases BookData.bookcount by 1 in LibraryManager, if this check wasnt done, it would show error that ISBN already existed in the library
        if (ValidInputChecker.IsBookAlreadyListed(_bookTitleInputField.text, _bookAuthorInputField.text, _bookIsbnInputField.text))
        {
           
            AddBookToLibraryViaLibraryManager(_bookTitleInputField.text, _bookAuthorInputField.text, _bookIsbnInputField.text);
            return;
        }
        //checks if valid input, if it finds the same ISBN with a different title it would show error. 
        if (IsValidBookInput())
        {         
            AddBookToLibraryViaLibraryManager(_bookTitleInputField.text, _bookAuthorInputField.text, _bookIsbnInputField.text);
        }
        else
        {
            ShowErrorPopup(GenerateErrorMessage());         
        }
    }



    private string GenerateErrorMessage()
    {
        List<string> errorMessages = new List<string>();

        if (!ValidInputChecker.IsBookNameValid(_bookTitleInputField.text))
        {
            errorMessages.Add("Book title can't be empty. Please enter a valid title.");
        }

        if (!ValidInputChecker.IsBookAuthorValid(_bookAuthorInputField.text))
        {
            errorMessages.Add("Author can't be empty. Please enter a valid author name.");
        }

        if (!ValidInputChecker.IsBookIsbnValid(_bookIsbnInputField.text))
        {
            errorMessages.Add("Invalid or repeated ISBN. Please check again and enter a valid ISBN with 10 or 13-digits.");
        }

        if (ValidInputChecker.IsThisBookListedAsADifferentEntry(_bookTitleInputField.text, _bookAuthorInputField.text, _bookIsbnInputField.text))
        {
            errorMessages.Add($"The book '{_bookTitleInputField.text}' by '{_bookAuthorInputField.text}' is already listed with a different ISBN, please check your ISBN entry.");
        }
        string errorMessage = string.Join("\n", errorMessages);

        return errorMessage;
    }

    private void AddBookToLibraryViaLibraryManager(string bookTitle, string bookAuthor, string bookIsbn)
    {
        try
        {
            BookData newBookData = LibraryManager.Instance.CreateBookData(bookTitle, bookAuthor, bookIsbn);
            LibraryManager.Instance.AddBookToLibrary(newBookData);

            ShowResponsePopup($"{bookTitle} by {bookAuthor} is added to the library successfully.");
        }catch(Exception ex)
        {
            ShowErrorPopup($"An error occurred while adding the book: {ex.Message}");
        }
    }

    private void ShowErrorPopup(string errorMessage)
    {
        PopupPanelUI.Instance.ShowError(errorMessage);
    }

    private void ShowResponsePopup(string responseMessage)
    {
        PopupPanelUI.Instance.ShowResponse(responseMessage);
        CleanInputFields();
    }

    private bool IsValidBookInput()
    {
        return ValidInputChecker.IsBookNameValid(_bookTitleInputField.text)
            && ValidInputChecker.IsBookAuthorValid(_bookAuthorInputField.text)
            && ValidInputChecker.IsBookIsbnValid(_bookIsbnInputField.text)
            && !ValidInputChecker.IsThisBookListedAsADifferentEntry(_bookTitleInputField.text, _bookAuthorInputField.text, _bookIsbnInputField.text);
    }

    public void CleanInputFields()
    {
        _bookTitleInputField.text = "";
        _bookAuthorInputField.text = "";
        _bookIsbnInputField.text = "";
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        CleanInputFields();
        gameObject.SetActive(false);
    }
}

