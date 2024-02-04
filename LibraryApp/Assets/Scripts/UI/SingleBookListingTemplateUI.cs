using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SingleBookListingTemplateUI : MonoBehaviour, IPointerClickHandler
{
    [Header("For general Listings")] //Some General purpose fields, author and count not needed for return panel
    [SerializeField] private TextMeshProUGUI _bookTitleText;
    [SerializeField] private TextMeshProUGUI _bookAuthorText;
    [SerializeField] private TextMeshProUGUI _bookCountText;
    [SerializeField] private TextMeshProUGUI _bookIsbnText;


    [Header("List for all lent books list type")] //Needed to be asigned for panels where returning lent book functionality is available
    [SerializeField] private TextMeshProUGUI _borrowerNameText;
    [SerializeField] private TextMeshProUGUI _dueDateText;

    [SerializeField] private Image _selectedVisual;

    private BookData _bookDataOnSingleTemplate;
    private string _returnCodeOnSingleTemplate;
    private LendingInfoPairsSO.LendingPair _lendingPairOnSingleTemplate;
    private int _lendingPairLendingListIndexSingleTemplate;

    private Color _dueDatePassedColor = Color.red;
    private Color _dueDateNotPassedColor = Color.green;

    private void Awake()
    {
        _bookDataOnSingleTemplate = null;
        _returnCodeOnSingleTemplate = null;
    }

    private void Start()
    {
        ListPanelUI.Instance.OnSelectedListItemChanged += ListPanelUI_OnSelectedListItemChanged;
    }

    public BookData GetBookData()
    {
        return _bookDataOnSingleTemplate;
    }

    public string GetRetrunCode()
    {
        return _returnCodeOnSingleTemplate;
    }

    public LendingInfoPairsSO.LendingPair GetLendingPair()
    {
        return _lendingPairOnSingleTemplate;
    }

    public int GetLendingPairLendingListInfoIndex()
    {
        return _lendingPairLendingListIndexSingleTemplate;
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
            _selectedVisual.gameObject.SetActive(true);
        }
        else
        {
            _selectedVisual.gameObject.SetActive(false);
        }
    }

    public void SetBookDataForBasicListing(BookData bookData)
    {
        _bookDataOnSingleTemplate = bookData;
        _bookTitleText.text = bookData.BookTitle;
        _bookAuthorText.text = bookData.BookAuthor;
        _bookIsbnText.text = bookData.BookIsbn;
        _bookCountText.text = bookData.BookCount.ToString();
    }

    //Index approach is there for avoiding sending 2 classes in this basic function. I didnt want to send lendingInfo as a seperate parameter.
    public void SetBookDataLentBookList(LendingInfoPairsSO.LendingPair lendingPair, int lendingInfoListIndex)
    {
        LendingInfo lendingInfo = lendingPair.LendingInfoList[lendingInfoListIndex];

        _lendingPairOnSingleTemplate = lendingPair;
        _lendingPairLendingListIndexSingleTemplate = lendingInfoListIndex;
        _returnCodeOnSingleTemplate = lendingInfo.ReturnCode;

        _bookTitleText.text = lendingPair.Book.BookTitle;
        _bookAuthorText.text = lendingPair.Book.BookAuthor;
        _borrowerNameText.text = lendingInfo.BorrowerName;

        DateTime expectedReturnDeserialized = new DateTime(lendingInfo.ExpectedReturnDateTicks);

        //change the color of the text according to due date passing or not passing 
        _dueDateText.color = (expectedReturnDeserialized <= DateTime.Now) ? _dueDatePassedColor : _dueDateNotPassedColor;
        _dueDateText.text = expectedReturnDeserialized.ToString("MM/dd/yyyy");

       // returnButton.onClick.AddListener(() => OnReturnButtonClick(lendingPair,lendingInfoListIndex));
    }
    private void OnDestroy()
    {
        ListPanelUI.Instance.OnSelectedListItemChanged -= ListPanelUI_OnSelectedListItemChanged;
    }


}
