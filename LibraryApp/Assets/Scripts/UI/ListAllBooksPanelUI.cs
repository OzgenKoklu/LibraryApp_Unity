using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListAllBooksPanelUI : MonoBehaviour
{
    public static ListAllBooksPanelUI Instance {  get; private set; }

    [SerializeField] private Transform container;
    [SerializeField] private Transform singleBookListingTemplate;

    [SerializeField] private TextMeshProUGUI ListDetailsText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);
        singleBookListingTemplate.gameObject.SetActive(false);
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        UpdateVisual();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void UpdateListDetailsText(int totalUniqueBookCount, int totalBookCount)
    {
        ListDetailsText.text = $"Total Unique Books: {totalUniqueBookCount}  Total Books: {totalBookCount}";
    }

    private void UpdateVisual()
    {
        foreach (Transform child in container)
        {
            if (child == singleBookListingTemplate) continue;
            Destroy(child.gameObject);
        }

        int totalUniqueBookCount = 0;
        int totalBookCount = 0;

        foreach (BookData bookData in LibraryManager.Instance.GetLibraryData().books)
        {
            totalUniqueBookCount++;
            totalBookCount += bookData.bookCount;
            Transform bookListingTransform = Instantiate(singleBookListingTemplate, container);
            bookListingTransform.gameObject.SetActive(true);
            bookListingTransform.GetComponent<SingleBookListingTemplateUI>().SetBookData(bookData);
        }

        UpdateListDetailsText(totalUniqueBookCount, totalBookCount);

    }
}
