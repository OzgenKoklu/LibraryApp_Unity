using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class SingleBookListingTemplateUI : MonoBehaviour, IPointerClickHandler
{
    [Header("For general Listings")] //Some General purpose fields, author and count not needed for return panel
    [SerializeField] private TextMeshProUGUI bookTitleText;
    [SerializeField] private TextMeshProUGUI bookAuthorText;
    [SerializeField] private TextMeshProUGUI bookCountText;
    [SerializeField] private TextMeshProUGUI bookIsbnText;


    [Header("List for all lent books list type")] //Needed to be asigned for panels where returning lent book functionality is available
    [SerializeField] private TextMeshProUGUI borrowerNameText;
    [SerializeField] private TextMeshProUGUI dueDateText;

    [SerializeField] private Image selectedVisual;

    private BookData bookDataOnSingleTemplate;
    private string returnCodeOnSingleTemplate;
    private LendingInfoPairsSO.LendingPair lendingPairOnSingleTemplate;
    private int lendingPairLendingListIndexSingleTemplate;

    private Color dueDatePassedColor = Color.red;
    private Color dueDateNotPassedColor = Color.green;

    private void Awake()
    {
        bookDataOnSingleTemplate = null;
        returnCodeOnSingleTemplate = null;
    }

    private void Start()
    {
        ListPanelUI.Instance.OnSelectedListItemChanged += ListPanelUI_OnSelectedListItemChanged;
    }

    public BookData GetBookData()
    {
        return bookDataOnSingleTemplate;
    }

    public string GetRetrunCode()
    {
        return returnCodeOnSingleTemplate;
    }

    public LendingInfoPairsSO.LendingPair GetLendingPair()
    {
        return lendingPairOnSingleTemplate;
    }

    public int GetLendingPairLendingListInfoIndex()
    {
        return lendingPairLendingListIndexSingleTemplate;
    }



    private void ListPanelUI_OnSelectedListItemChanged(object sender, ListPanelUI.OnSelectedListItemChangedEventArgs e)
    {
        SetSelectedVisualOnOrOff(e.selectedListItemTemplate == this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Trigger the OnClick method when the UI element is clicked
        OnClick();
    }
    public void OnClick()
    {
        ListPanelUI.Instance.ChangeSelectedListing(this);
    }

    public void SetSelectedVisualOnOrOff(bool isOn)
    {
        if (isOn)
        {
            selectedVisual.gameObject.SetActive(true);
        }
        else
        {
            selectedVisual.gameObject.SetActive(false);
        }
    }

    public void SetBookDataForBasicListing(BookData bookData)
    {
        bookDataOnSingleTemplate = bookData;
        bookTitleText.text = bookData.bookTitle;
        bookAuthorText.text = bookData.bookAuthor;
        bookIsbnText.text = bookData.bookIsbn;
        bookCountText.text = bookData.bookCount.ToString();
    }

    public void SetBookDataForLending(BookData bookData)
    {
        bookDataOnSingleTemplate = bookData;
        bookTitleText.text = bookData.bookTitle;
        bookAuthorText.text = bookData.bookAuthor;
        bookCountText.text = bookData.bookCount.ToString();
       // lendButton.onClick.AddListener(() => OnLendButtonClick(bookData));
    }


    //Index approach is there for avoiding sending 2 classes in this basic function. I didnt want to send lendingInfo as a seperate parameter.
    public void SetBookDataForReturningLentBook(LendingInfoPairsSO.LendingPair lendingPair, int lendingInfoListIndex)
    {
        LendingInfo lendingInfo = lendingPair.lendingInfoList[lendingInfoListIndex];

        lendingPairOnSingleTemplate = lendingPair;
        lendingPairLendingListIndexSingleTemplate = lendingInfoListIndex;
        returnCodeOnSingleTemplate = lendingInfo.returnCode;

        bookTitleText.text = lendingPair.book.bookTitle;
        bookAuthorText.text = lendingPair.book.bookAuthor;
        borrowerNameText.text = lendingInfo.borrowerName;

        DateTime expectedReturnDeserialized = new DateTime(lendingInfo.expectedReturnDateTicks);

        //change the color of the text according to due date passing or not passing 
        dueDateText.color = (expectedReturnDeserialized <= DateTime.Now) ? dueDatePassedColor : dueDateNotPassedColor;
        dueDateText.text = expectedReturnDeserialized.ToString("MM/dd/yyyy");

       // returnButton.onClick.AddListener(() => OnReturnButtonClick(lendingPair,lendingInfoListIndex));
    }



    private void OnDestroy()
    {
        ListPanelUI.Instance.OnSelectedListItemChanged -= ListPanelUI_OnSelectedListItemChanged;
    }


}
