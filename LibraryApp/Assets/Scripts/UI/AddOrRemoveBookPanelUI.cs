using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddOrRemoveBookPanelUI : MonoBehaviour
{
    public static AddOrRemoveBookPanelUI Instance { get; private set; }

    [SerializeField] private Button closeButton;
    [SerializeField] private Button addBookButton;
    [SerializeField] private Button addOrRemoveFromList;
    [SerializeField] private TMP_InputField bookTitleInputField;
    [SerializeField] private TMP_InputField bookAuthorInputField;
    [SerializeField] private TMP_InputField bookIsbnInputField;

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);
        addBookButton.onClick.AddListener(TryAddBook);
        addOrRemoveFromList.onClick.AddListener(() =>
        {
            ListPanelUI.Instance.Show(ListPanelUI.ListType.AddOrRemovePanelList);
        });
        Hide();
    }


    public void TryAddBook()
    {
        //if already listed, addbook increases BookData.bookcount by 1 in LibraryManager, if this check wasnt done, it would show error that ISBN already existed in the library
        if (ValidInputChecker.IsBookAlreadyListed(bookTitleInputField.text, bookAuthorInputField.text, bookIsbnInputField.text))
        {
           
            AddBookToLibraryViaLibraryManager(bookTitleInputField.text, bookAuthorInputField.text, bookIsbnInputField.text);
            return;
        }
        //checks if valid input, if it finds the same ISBN with a different title it would show error. 
        if (IsValidBookInput())
        {         
            AddBookToLibraryViaLibraryManager(bookTitleInputField.text, bookAuthorInputField.text, bookIsbnInputField.text);
        }
        else
        {
            string errorMessage = GenerateErrorMessage();
            PopupPanelUI.Instance.ShowError(errorMessage);
        }
    }

    private string GenerateErrorMessage()
    {
        List<string> errorMessages = new List<string>();

        if (!ValidInputChecker.IsBookNameValid(bookTitleInputField.text))
        {
            errorMessages.Add("Book title can't be empty. Please enter a valid title.");
        }

        if (!ValidInputChecker.IsBookAuthorValid(bookAuthorInputField.text))
        {
            errorMessages.Add("Author can't be empty. Please enter a valid author name.");
        }

        if (!ValidInputChecker.IsBookIsbnValid(bookIsbnInputField.text))
        {
            errorMessages.Add("Invalid or repeated ISBN. Please check again and enter a valid ISBN with 10 or 13-digits.");
        }

        if (ValidInputChecker.IsThisBookListedAsADifferentEntry(bookTitleInputField.text, bookAuthorInputField.text, bookIsbnInputField.text))
        {
            errorMessages.Add($"The book '{bookTitleInputField.text}' by '{bookAuthorInputField.text}' is already listed with a different ISBN, please check your ISBN entry.");
        }
        string errorMessage = string.Join("\n", errorMessages);

        return errorMessage;
    }

    private void AddBookToLibraryViaLibraryManager(string bookTitle, string bookAuthor, string bookIsbn)
    {
        BookData newBookData = LibraryManager.Instance.CreateBookData(bookTitle, bookAuthor, bookIsbn);
        LibraryManager.Instance.AddBookToLibrary(newBookData);

        string responseMessage = $"{bookTitle} by {bookAuthor} is added to the library successfully.";
        PopupPanelUI.Instance.ShowResponse(responseMessage);
        CleanInputFields();
    }

    private bool IsValidBookInput()
    {
        return ValidInputChecker.IsBookNameValid(bookTitleInputField.text)
            && ValidInputChecker.IsBookAuthorValid(bookAuthorInputField.text)
            && ValidInputChecker.IsBookIsbnValid(bookIsbnInputField.text)
            && !ValidInputChecker.IsThisBookListedAsADifferentEntry(bookTitleInputField.text, bookAuthorInputField.text, bookIsbnInputField.text);
    }

    public void CleanInputFields()
    {
        bookTitleInputField.text = "";
        bookAuthorInputField.text = "";
        bookIsbnInputField.text = "";
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

