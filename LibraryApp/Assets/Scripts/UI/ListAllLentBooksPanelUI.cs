using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListAllLentBooksPanelUI : MonoBehaviour
{
    public static ListAllLentBooksPanelUI Instance { get; private set; }

    [SerializeField] private Transform lentBooksListContainer;
    [SerializeField] private Transform singleBookListingTemplate;

    [SerializeField] private TextMeshProUGUI lentBooksListDetailsText;

    [SerializeField] private Button closeButton;

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);
        singleBookListingTemplate.gameObject.SetActive(false);
        Hide();
    }

    private void UpdateBookList()
    {
        foreach (Transform child in lentBooksListContainer)
        {
            if (child == singleBookListingTemplate) continue;
            Destroy(child.gameObject);
        }

        int totalLentEntries = 0;

        foreach (LendingInfoPairsSO.LendingPair lendingPair in LibraryManager.Instance.GetLendingInfoPairs().lendingPairs)
        {
            int lendInfoIndex = 0;
            foreach(LendingInfo lendingInfo in lendingPair.lendingInfoList)
            {
                totalLentEntries++;
                Transform bookListingTransform = Instantiate(singleBookListingTemplate, lentBooksListContainer);
                bookListingTransform.gameObject.SetActive(true);
                bookListingTransform.GetComponent<SingleBookListingTemplateUI>().SetBookDataForReturningLentBook(lendingPair, lendInfoIndex);
                lendInfoIndex++;
            }
        }

        UpdateListDetailsText(totalLentEntries);

    }

    private void UpdateListDetailsText(int totalLentEntries)
    {
        lentBooksListDetailsText.text = $"Total Lent Books: {totalLentEntries}";
    }

    public void Show()
    {
        LibraryManager.Instance.OnReturnFromListSuccessful += LibraryManager_OnReturnFromListSuccessful;
        gameObject.SetActive(true);
        UpdateBookList();
    }

    private void LibraryManager_OnReturnFromListSuccessful(object sender, System.EventArgs e)
    {
        UpdateBookList();
    }

    private void Hide()
    {
        LibraryManager.Instance.OnReturnFromListSuccessful -= LibraryManager_OnReturnFromListSuccessful;
        gameObject.SetActive(false);
    }
}
