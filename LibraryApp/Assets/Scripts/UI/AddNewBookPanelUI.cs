using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LibraryManager;

public class AddNewBookPanelUI : MonoBehaviour
{
    public static AddNewBookPanelUI Instance { get; private set; }

    public event EventHandler<OnSuccessfulBookAdditionEventArgs> OnSuccessfulBookAddition;
    public class OnSuccessfulBookAdditionEventArgs : EventArgs { public string bookTitle; public string bookAuthor;  }

    public event EventHandler<OnInvalidInputEventArgs> OnInvalidInput;
    public class OnInvalidInputEventArgs : EventArgs { public string invalidInputErrorMessage;}

    [SerializeField] private Button closeButton;
    [SerializeField] private Button addBookButton;
    [SerializeField] private TMP_InputField bookTitleInputField;
    [SerializeField] private TMP_InputField bookAuthorInputField;
    [SerializeField] private TMP_InputField bookIsbnInputField;

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);
        addBookButton.onClick.AddListener(TryAddBook);
        Hide();
    }


    public void TryAddBook()
    {
        //if the same entry is already listed, add book to the library without further test and break from method
        if (ValidInputChecker.IsBookAlreadyListed(bookTitleInputField.text, bookAuthorInputField.text, bookIsbnInputField.text))
        {
            LibraryManager.Instance.AddBookToLibrary(LibraryManager.Instance.CreateBookData(bookTitleInputField.text, bookAuthorInputField.text, bookIsbnInputField.text));

            OnSuccessfulBookAddition?.Invoke(this, new OnSuccessfulBookAdditionEventArgs
            {
                bookTitle = bookTitleInputField.text,
                bookAuthor = bookAuthorInputField.text
            });

            CleanInputFields();

            return;
        }

        //tests for valid input and also whether the bookTitle+bookAuthor is listed with a different ISBN
        if (ValidInputChecker.IsBookNameValid(bookTitleInputField.text)
         && ValidInputChecker.IsBookAuthorValid(bookAuthorInputField.text)
         && ValidInputChecker.IsBookIsbnValid(bookIsbnInputField.text)
        && !ValidInputChecker.IsThisBookListedAsADifferentEntry(bookTitleInputField.text, bookAuthorInputField.text, bookIsbnInputField.text)){

            LibraryManager.Instance.AddBookToLibrary(LibraryManager.Instance.CreateBookData(bookTitleInputField.text, bookAuthorInputField.text, bookIsbnInputField.text));

            OnSuccessfulBookAddition?.Invoke(this, new OnSuccessfulBookAdditionEventArgs
            {
                bookTitle = bookTitleInputField.text,
                bookAuthor = bookAuthorInputField.text
            });

            CleanInputFields();
        }
        else
        {
            List<string> errorMessages = new List<string>();

            if (!ValidInputChecker.IsBookNameValid(bookTitleInputField.text))
            {
                errorMessages.Add("Book title can't be empty. Please enter a valid title. ");
            }

            if (!ValidInputChecker.IsBookAuthorValid(bookAuthorInputField.text))
            {
                errorMessages.Add("Author can't be empty. Please enter a valid author name. ");
            }

            if (!ValidInputChecker.IsBookIsbnValid(bookIsbnInputField.text))
            {
                errorMessages.Add("Invalid or repeated ISBN. Please check again and enter a valid ISBN with 13-digits.");
            }
            if (ValidInputChecker.IsThisBookListedAsADifferentEntry(bookTitleInputField.text, bookAuthorInputField.text, bookIsbnInputField.text))
            {
                errorMessages.Add($"The book '{bookTitleInputField.text}' by '{bookAuthorInputField.text}' is already listed with a different ISBN, please check your ISBN entry.");
            }

            // Combine error messages into a single string
            string errorMessage = string.Join("\n", errorMessages);

            // Trigger the OnInvalidInput event with the error message
            OnInvalidInput?.Invoke(this, new OnInvalidInputEventArgs
            {
                invalidInputErrorMessage = errorMessage
            });
        }
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

