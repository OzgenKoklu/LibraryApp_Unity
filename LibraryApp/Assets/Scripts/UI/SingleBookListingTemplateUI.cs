using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleBookListingTemplateUI : MonoBehaviour
{
    [Header("General Purpose Fields")] //Some General purpose fields, author and count not needed for return panel
    [SerializeField] private TextMeshProUGUI bookTitleText;
    [SerializeField] private TextMeshProUGUI bookAuthorText;
    [SerializeField] private TextMeshProUGUI bookCountText;

    [Header("Listing and Searching Panels")] //Needed for listing and searching panels

    [SerializeField] private TextMeshProUGUI bookIsbnText;

    [Header("Lending Panel")] //Needed to be asigned for panels where lending functionality is available
    [SerializeField] private Button lendButton;

    [Header("Returning Panel")] //Needed to be asigned for panels where returning lent book functionality is available
    [SerializeField] private TextMeshProUGUI borrowerNameText;
    [SerializeField] private TextMeshProUGUI dueDateText;
    [SerializeField] private Button returnButton;

    public void SetBookDataForBasicListing(BookData bookData)
    {
        bookTitleText.text = bookData.bookTitle;
        bookAuthorText.text = bookData.bookAuthor;
        bookIsbnText.text = bookData.bookIsbn;
        bookCountText.text = bookData.bookCount.ToString();
    }

    public void SetBookDataForLending(BookData bookData)
    {
        bookTitleText.text = bookData.bookTitle;
        bookAuthorText.text = bookData.bookAuthor;
        bookCountText.text = bookData.bookCount.ToString();
        lendButton.onClick.AddListener(() => OnLendButtonClick(bookData));
    }

    private void OnLendButtonClick(BookData bookData)
    {
        LendABookBorrowerNamePromptPanelUI.Instance.Show(bookData);
    }
    public void SetBookDataForReturningLentBook(BookData bookData)
    {
        
    }

    
}
