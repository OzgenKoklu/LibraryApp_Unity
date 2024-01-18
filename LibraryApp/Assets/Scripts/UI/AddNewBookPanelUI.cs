using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddNewBookPanelUI : MonoBehaviour
{
    public static AddNewBookPanelUI Instance { get; private set; }

    [SerializeField] private Button closeButton;
    [SerializeField] private Button addBookButton;
    [SerializeField] private TMP_InputField bookTitleInputField;
    [SerializeField] private TMP_InputField bookAuthorInputField;
    [SerializeField] private TMP_InputField bookIsbnInputField;

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);
        addBookButton.onClick.AddListener(AddBook);
        Hide();
    }


    public void AddBook()
    {
        if(ValidInputChecker.IsBookNameValid(bookTitleInputField.text)
         && ValidInputChecker.IsBookAuthorValid(bookAuthorInputField.text)
         && ValidInputChecker.IsBookIsbnValid(bookIsbnInputField.text)){
         
        LibraryManager.Instance.AddBookToLibrary(LibraryManager.Instance.CreateBookData(bookTitleInputField.text, bookAuthorInputField.text, bookIsbnInputField.text));

        CleanInputFields();
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
        gameObject.SetActive(false);
    }
}

