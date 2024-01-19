using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingleBookListingTemplateUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bookTitleText;
    [SerializeField] private TextMeshProUGUI bookAuthorText;
    [SerializeField] private TextMeshProUGUI bookIsbnText;
    [SerializeField] private TextMeshProUGUI bookCountText;

    public void SetBookData(BookData bookData)
    {
        bookTitleText.text = bookData.bookTitle;
        bookAuthorText.text = bookData.bookAuthor;
        bookIsbnText.text = bookData.bookIsbn;
        bookCountText.text = bookData.bookCount.ToString();

    }
}
