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
        if(ValidInputChecker.IsBookNameValid(bookTitleInputField.text)
         && ValidInputChecker.IsBookAuthorValid(bookAuthorInputField.text)
         && ValidInputChecker.IsBookIsbnValid(bookIsbnInputField.text)){
         
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
                errorMessages.Add("Invalid ISBN. Please enter a valid ISBN with 13-digits. ");
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

