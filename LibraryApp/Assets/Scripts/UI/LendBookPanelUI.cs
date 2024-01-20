using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LendBookPanelUI : MonoBehaviour
{
    public static LendBookPanelUI Instance { get; private set; }

    [SerializeField] private Transform allBooksAvailableContainer;
    [SerializeField] private Transform singleBookListingTemplate;

    [SerializeField] private TextMeshProUGUI allBooksAvailableInfoText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        Instance = this;
        singleBookListingTemplate.gameObject.SetActive(false);
        closeButton.onClick.AddListener(Hide);
        Hide();
    }

    //WILL REFACTOR THIS CODE, ITS ALMOST IDENTICAL TO lisAllBooksPanelUI.cs
    //WILL SEE WHATS DUPLICATION AND WHATS UNIQUE AND SHRINKDOWN AND/OR USE INTERFACE IF POSSIBLE
    private void UpdateBookList()
    {
        foreach (Transform child in allBooksAvailableContainer)
        {
            if (child == singleBookListingTemplate) continue;
            Destroy(child.gameObject);
        }

        List<BookData> availableBooks = SearchManager.GetAvailableBooks(LibraryManager.Instance.GetLibraryData().books);
        int totalUniqueBookCount = availableBooks.Count;

        foreach (BookData bookData in availableBooks)
        {          
            Transform bookListingTransform = Instantiate(singleBookListingTemplate, allBooksAvailableContainer);
            bookListingTransform.gameObject.SetActive(true);
            bookListingTransform.GetComponent<SingleBookListingTemplateUI>().SetBookDataForLending(bookData);
        }

        UpdateListDetailsText(totalUniqueBookCount);
    }

    private void UpdateListDetailsText(int totalUniqueBookCount)
    {
        allBooksAvailableInfoText.text = $"Total Number Of Available Unique Books: {totalUniqueBookCount}";
    }

        public void Show()
    {
        gameObject.SetActive(true);
        UpdateBookList();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
