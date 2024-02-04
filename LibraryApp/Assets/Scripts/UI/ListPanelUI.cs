using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListPanelUI : MonoBehaviour
{
    public enum ListType
    {
        AllBooksList,
        AddOrRemovePanelList,
        LendABookList,
        AllLentBooksList,
    }
    public static ListPanelUI Instance { get; private set; }

    private const string AllBooksPanelTitleText = "All Books";
    private const string LendABookPanelTitleText = "Lend A Book";
    private const string AddOrRemoveABookPanelTitleText = "Add or Remove An Existing Book";
    private const string AllLentBooksPanelTitleText = "List of all Lent Books";

    private const string ActionButtonLendText = "Lend";
    private const string ActionButtonReturnText = "Return";
    private const string ActionButtonAddOrRemoveText = "Add or\nRemove";

    public event EventHandler<OnSelectedListItemChangedEventArgs> OnSelectedListItemChanged;
    public class OnSelectedListItemChangedEventArgs : EventArgs { public SingleBookListingTemplateUI selectedListItemTemplate; };

    [SerializeField] private Button _closeButton;
    [SerializeField] private TextMeshProUGUI _panelTitleText;
    [SerializeField] private TextMeshProUGUI _searchResultsDetailsText;

    //Action button will act differently in different states, for example it will work as lend button or return button and so forth, hence the "obscure" name
    [SerializeField] private Button _actionButton;
    [SerializeField] private TextMeshProUGUI _actionButtonText;

    [SerializeField] private Button _searchButton;
    [SerializeField] private TMP_Dropdown _searchTypeDropdown;
    [SerializeField] private TMP_InputField _searchTermInputField;

    //Legend And Vertical Layout Groups that include the listing templates
    [SerializeField] private Transform _generalList;
    [SerializeField] private Transform _lentList;

    //For Generic List
    [SerializeField] private Transform _searchResultsContainer;
    [SerializeField] private Transform _singleBookListingTemplate;


    //For All Lent Books - Return List 
    [SerializeField] private Transform _lentListSearchResultsContainer;
    [SerializeField] private Transform _lentListSingleBookListingTemplate;
    [SerializeField] private Toggle _expiredDueDatesToggle;

    private ListType _currentListType;
    private SingleBookListingTemplateUI _selectedListing;

    private void Awake()
    {
        Instance = this;
        _closeButton.onClick.AddListener(Hide);

        _searchButton.onClick.AddListener(() =>
        {
            OnSearchButtonClicked();
        });

        _singleBookListingTemplate.gameObject.SetActive(false);
        _lentListSingleBookListingTemplate.gameObject.SetActive(false);

        Hide();
    }

    //Main panel setup is done using the state machine. Show can only be called with the ListType Enum and depending on the type, we either set active or deactive some components, set texts,
    //and update the container depending on the context that it was opened. (e.g.:, if Lend a book list is opened, only available books are shown) 
    public void Show(ListType listType)
    {
        gameObject.SetActive(true);

        //sets current list type so that we will unsub from the right events
        _currentListType = listType;
        SetDropdownOptions(listType);

        ChangeLegendAndContainerType(listType);

        //state machine will show and hide necessary components
        switch (listType)
        {
            case ListType.AllBooksList:
                _panelTitleText.text = AllBooksPanelTitleText;
                _actionButton.gameObject.SetActive(false);
                //Load AllBooks Script 
                UpdateSearchResultList(LibraryManager.Instance.GetLibraryData().Books);


                break;
            case ListType.LendABookList:
                _panelTitleText.text = LendABookPanelTitleText;
                _actionButton.gameObject.SetActive(true);

                _actionButtonText.text = ActionButtonLendText;


                List<BookData> availableBookDataList = SearchManager.GetAvailableBooks(LibraryManager.Instance.GetLibraryData().Books);
                UpdateSearchResultList(availableBookDataList);


                LibraryManager.Instance.OnLibraryDataUpdatedForLists += LibraryManager_OnBookLendingSuccessful;
                _actionButton.onClick.AddListener(() => OnLendButtonClick(_selectedListing));

                break;

            //in this list you can return the books so the button is repurposed as a return button
            case ListType.AllLentBooksList:
                _panelTitleText.text = AllLentBooksPanelTitleText;
                _actionButton.gameObject.SetActive(true);

                _actionButtonText.text = ActionButtonReturnText;
                _expiredDueDatesToggle.isOn = false;
                _expiredDueDatesToggle.onValueChanged.AddListener(UpdateBookListForLentBooks);

                LendingInfoPairsSO allLentBooksList = LibraryManager.Instance.GetLendingInfoPairs();
                UpdateBookListForLentBooks(allLentBooksList);
                UpdateBookListForLentBooks(false);

                LibraryManager.Instance.OnLibraryDataUpdatedForLists += LibraryManager_OnReturnFromListSuccessful;

                LendingInfoPairsSO.LendingPair lendingPair = new LendingInfoPairsSO.LendingPair();
                int lendingInfoIndex = 0;
                _actionButton.onClick.AddListener(() => OnReturnButtonClick(lendingPair, lendingInfoIndex));

                break;

            case ListType.AddOrRemovePanelList:
                _panelTitleText.text = AddOrRemoveABookPanelTitleText;
                _actionButton.gameObject.SetActive(true);
                _actionButtonText.text = ActionButtonAddOrRemoveText;

                UpdateSearchResultList(LibraryManager.Instance.GetLibraryData().Books);
                LibraryManager.Instance.OnLibraryDataUpdatedForLists += LibraryManager_OnListDataUpdated;
                _actionButton.onClick.AddListener(() => OnAddOrRemoveButtonClick(_selectedListing));

                break;
        }
    }
    public void Hide()
    {
        //search toggle reset
        _selectedListing = null;

        if (_currentListType == ListType.AllLentBooksList)
        {
            _expiredDueDatesToggle.onValueChanged.RemoveAllListeners();
            _actionButton.onClick.RemoveAllListeners();
            LibraryManager.Instance.OnLibraryDataUpdatedForLists -= LibraryManager_OnReturnFromListSuccessful;
        }
        if (_currentListType == ListType.LendABookList)
        {
            LibraryManager.Instance.OnLibraryDataUpdatedForLists -= LibraryManager_OnBookLendingSuccessful;
            _actionButton.onClick.RemoveAllListeners();
        }
        if (_currentListType == ListType.AddOrRemovePanelList)
        {
            LibraryManager.Instance.OnLibraryDataUpdatedForLists -= LibraryManager_OnListDataUpdated;
            _actionButton.onClick.RemoveAllListeners();
        }


        _searchTermInputField.text = "";
        gameObject.SetActive(false);
    }

    #region General Purpose Methods 
    public void ChangeSelectedListing(SingleBookListingTemplateUI selectedListing)
    {
        this._selectedListing = selectedListing;

        OnSelectedListItemChanged?.Invoke(this, new OnSelectedListItemChangedEventArgs
        {
            selectedListItemTemplate = selectedListing
        });
    }
    private void OnSearchButtonClicked()
    {
        //Only "All Lent books" uses a different container and different search type 
        if (_currentListType == ListType.AllLentBooksList)
        {
            SearchCriteriaSO searchCriteria = ScriptableObject.CreateInstance<SearchCriteriaSO>();
            searchCriteria.SearchTypeLentList = (SearchManager.SearchTypeLentListing)Enum.Parse(typeof(SearchManager.SearchTypeLentListing), _searchTypeDropdown.options[_searchTypeDropdown.value].text.Replace(" ", ""));
            searchCriteria.SearchTerm = _searchTermInputField.text;
            searchCriteria.IsExpiredLent = _expiredDueDatesToggle.isOn;

            //perform Search from Lending info list (Not through book data)
            List<LendingInfoPairsSO.LendingPair> searchResults = SearchManager.PerformLendingInfoPairsSearch(searchCriteria);
            UpdateBookListForLentBooks(searchResults);
            //UpdateSearchResultList(searchResults);
        }
        else
        {
            SearchCriteriaSO searchCriteria = ScriptableObject.CreateInstance<SearchCriteriaSO>();
            //Now we need to remove spaces we added just for the UI.
            searchCriteria.SearchTypeGeneral = (SearchManager.SearchTypeGeneralListing)Enum.Parse(typeof(SearchManager.SearchTypeGeneralListing), _searchTypeDropdown.options[_searchTypeDropdown.value].text.Replace(" ", ""));
            searchCriteria.SearchTerm = _searchTermInputField.text;

            if (_currentListType == ListType.LendABookList)
            {
                searchCriteria.IsAvailable = true;
            }

            // Call the SearchManager to perform the search
            List<BookData> searchResults = SearchManager.PerformBookListingSearch(searchCriteria);
            // Now we have the search results, update & populate the container
            UpdateSearchResultList(searchResults);
        }

    }

    private void LibraryManager_OnBookLendingSuccessful(object sender, System.EventArgs e)
    {
        UpdateSearchResultList(SearchManager.GetAvailableBooks(LibraryManager.Instance.GetLibraryData().Books));
    }



    private string AddSpaceBeforeUpperCase(string input)
    {
        // Add a space before each uppercase letter in the string
        System.Text.StringBuilder modifiedString = new System.Text.StringBuilder();
        foreach (char c in input)
        {
            if (char.IsUpper(c))
            {
                modifiedString.Append(' ');
            }
            modifiedString.Append(c);
        }
        return modifiedString.ToString();
    }


    //two main containers due to 2 lists being based on different data sts
    public void ChangeLegendAndContainerType(ListType listType)
    {
        //AllLentBooks uses different Legend/templates thus this code sets on/off the right template
        if (listType == ListType.AllLentBooksList)
        {
            _lentList.gameObject.SetActive(true);
            _generalList.gameObject.SetActive(false);
        }
        else
        {
            _generalList.gameObject.SetActive(true);
            _lentList.gameObject.SetActive(false);
        }
    }
    private void SetDropdownOptions(ListType listType)
    {
        // Only "All Lent books" list have a different category, can search borrower name
        // Clear existing options
        _searchTypeDropdown.ClearOptions();

        if (listType == ListType.AllLentBooksList)
        {
            // Assuming you have a list of string options
            string[] searchTypeOptions = System.Enum.GetNames(typeof(SearchManager.SearchTypeLentListing));

            // Modify enum names for better UI readability
            for (int i = 0; i < searchTypeOptions.Length; i++)
            {
                searchTypeOptions[i] = AddSpaceBeforeUpperCase(searchTypeOptions[i]);
            }
            // Add new options
            _searchTypeDropdown.AddOptions(new List<string>(searchTypeOptions));
        }
        else
        {
            // Assuming you have a list of string options
            string[] searchTypeOptions = System.Enum.GetNames(typeof(SearchManager.SearchTypeGeneralListing));

            // Modify enum names for better UI readability
            for (int i = 0; i < searchTypeOptions.Length; i++)
            {
                searchTypeOptions[i] = AddSpaceBeforeUpperCase(searchTypeOptions[i]);
            }
            // Add new options
            _searchTypeDropdown.AddOptions(new List<string>(searchTypeOptions));
        }

    }

    #endregion

    #region Book List Methods
    private void UpdateSearchResultList(List<BookData> bookDataList)
    {
        foreach (Transform child in _searchResultsContainer)
        {
            if (child == _singleBookListingTemplate) continue;
            Destroy(child.gameObject);
        }

        int totalUniqueBookCount = 0;
        foreach (BookData result in bookDataList)
        {
            totalUniqueBookCount++;
            Transform bookListingTransform = Instantiate(_singleBookListingTemplate, _searchResultsContainer);
            bookListingTransform.gameObject.SetActive(true);
            bookListingTransform.GetComponent<SingleBookListingTemplateUI>().SetBookDataForBasicListing(result);
        }

        //update this line, more info is better
        _searchResultsDetailsText.text = $"{totalUniqueBookCount} book(s) found.";

    }
 
    private void LibraryManager_OnListDataUpdated(object sender, EventArgs e)
    {
        UpdateSearchResultList(LibraryManager.Instance.GetLibraryData().Books);
    }
    private void OnAddOrRemoveButtonClick(SingleBookListingTemplateUI listing)
    {
        if (listing == null)
        {
            string errorMessage = "Select a book first.";
            PopupPanelUI.Instance.ShowError(errorMessage);
            return;
        }

        BookData tempBook = listing.GetBookData();

        string promptMessage = $"You can increase or decrease the number of copies of '{tempBook.BookTitle}' (ISBN: '{tempBook.BookIsbn}') or remove the data related with it completely including the lending data. To cancel the operation, you can press 'X'.\nCurrent number of copies: '{tempBook.BookCount}'";

        PopupPanelUI.Instance.ShowAddOrRemoveBookPanel(tempBook, promptMessage);
    }

    private void OnLendButtonClick(SingleBookListingTemplateUI listing)
    {
        if (listing == null)
        {
            string errorMessage = "Select a book first.";
            PopupPanelUI.Instance.ShowError(errorMessage);
            return;
        }

        BookData tempBook = listing.GetBookData();

        string promptMessage = $"You are about to borrow the book titled '{tempBook.BookTitle}' by '{tempBook.BookAuthor}' (ISBN: '{tempBook.BookIsbn}'). The borrower is obliged to return the book within 1 month. To cancel the operation, you can press 'X'. To continue, please enter the borrower's name:";

        PopupPanelUI.Instance.ShowBookLendingBorrowerNamePrompt(promptMessage, tempBook);

    }
    #endregion

    //reason why lent list methods differ is bacouse it uses lendinfpair data rahter than BookData list
    #region Lent List Methods
    private void UpdateBookListForLentBooks(List<LendingInfoPairsSO.LendingPair> lendingInfoPairs)
    {
        foreach (Transform child in _lentListSearchResultsContainer)
        {
            if (child == _lentListSingleBookListingTemplate) continue;
            Destroy(child.gameObject);
        }

        int totalLentEntries = 0;

        foreach (LendingInfoPairsSO.LendingPair lendingPair in lendingInfoPairs)
        {
            int lendInfoIndex = 0;
            foreach (LendingInfo lendingInfo in lendingPair.LendingInfoList)
            {
                totalLentEntries++;
                Transform bookListingTransform = Instantiate(_lentListSingleBookListingTemplate, _lentListSearchResultsContainer);
                bookListingTransform.gameObject.SetActive(true);
                bookListingTransform.GetComponent<SingleBookListingTemplateUI>().SetBookDataLentBookList(lendingPair, lendInfoIndex);
                lendInfoIndex++;
            }
        }

        UpdateListDetailsTextForLentBooks(totalLentEntries);

    }
    private void LibraryManager_OnReturnFromListSuccessful(object sender, EventArgs e)
    {
        LendingInfoPairsSO allLentBooksList = LibraryManager.Instance.GetLendingInfoPairs();
        UpdateBookListForLentBooks(allLentBooksList);
        UpdateBookListForLentBooks(_expiredDueDatesToggle.isOn);
    }
    private void OnReturnButtonClick(LendingInfoPairsSO.LendingPair lendingPair, int lendingInfoListIndex)
    {
        if (_selectedListing == null)
        {
            string errorMessage = "Select a book first.";
            PopupPanelUI.Instance.ShowError(errorMessage);
            return;
        }

        lendingPair = _selectedListing.GetLendingPair();
        lendingInfoListIndex = _selectedListing.GetLendingPairLendingListInfoIndex();
        string returnConfirmationMessage = $"You are about to return {lendingPair.Book.BookTitle} borrowed by {lendingPair.LendingInfoList[lendingInfoListIndex].BorrowerName}. Press X to cancel or Confirm to proceed.";


        //passing in function as a delegate for the next UI pop Up to trigger, we can actually use the return code now that we have a function to get the return code. may simplify things
        PopupPanelUI.Instance.ShowResponse(returnConfirmationMessage, () =>

        LibraryManager.Instance.TryReturnLentBookFromTheList(lendingPair.LendingInfoList[lendingInfoListIndex])

        );

    }

    //On why this version is needed: Toggle's OnValueChange event needed a version of this with bool parameter
    private void UpdateBookListForLentBooks(bool isExpired)
    {


        if (isExpired)
        {
            //List only the expired books
            foreach (Transform child in _lentListSearchResultsContainer)
            {
                if (child == _lentListSingleBookListingTemplate) continue;
                Destroy(child.gameObject);
            }

            int totalLentEntries = 0;

            //
            List<LendingInfoPairsSO.LendingPair> allLentBooks = LibraryManager.Instance.GetLendingInfoPairs().LendingPairs;
            List<LendingInfoPairsSO.LendingPair> expiredLendingPairs = SearchManager.GetExpiredBooks(allLentBooks);

            foreach (LendingInfoPairsSO.LendingPair lendingPair in expiredLendingPairs)
            {
                int lendInfoIndex = 0;
                foreach (LendingInfo lendingInfo in lendingPair.LendingInfoList)
                {
                    totalLentEntries++;
                    Transform bookListingTransform = Instantiate(_lentListSingleBookListingTemplate, _lentListSearchResultsContainer);
                    bookListingTransform.gameObject.SetActive(true);
                    bookListingTransform.GetComponent<SingleBookListingTemplateUI>().SetBookDataLentBookList(lendingPair, lendInfoIndex);
                    lendInfoIndex++;
                }
            }

            UpdateListDetailsTextForLentBooks(totalLentEntries);
        }
        else
        {
            //list all the books
            foreach (Transform child in _lentListSearchResultsContainer)
            {
                if (child == _lentListSingleBookListingTemplate) continue;
                Destroy(child.gameObject);
            }

            int totalLentEntries = 0;

            LendingInfoPairsSO LendingInfoPairs = LibraryManager.Instance.GetLendingInfoPairs();

            foreach (LendingInfoPairsSO.LendingPair lendingPair in LendingInfoPairs.LendingPairs)
            {
                int lendInfoIndex = 0;
                foreach (LendingInfo lendingInfo in lendingPair.LendingInfoList)
                {
                    totalLentEntries++;
                    Transform bookListingTransform = Instantiate(_lentListSingleBookListingTemplate, _lentListSearchResultsContainer);
                    bookListingTransform.gameObject.SetActive(true);
                    bookListingTransform.GetComponent<SingleBookListingTemplateUI>().SetBookDataLentBookList(lendingPair, lendInfoIndex);
                    lendInfoIndex++;
                }
            }

            UpdateListDetailsTextForLentBooks(totalLentEntries);

        }
    }

    private void UpdateListDetailsTextForLentBooks(int totalLentEntries)
    {
        _searchResultsDetailsText.text = $"Total Lent Books: {totalLentEntries}";
    }


    #endregion

}
