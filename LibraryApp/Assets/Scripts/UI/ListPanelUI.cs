using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private const string ALL_BOOKS_TITLE_TEXT = "All Books";
    private const string LEND_A_BOOK_TITLE_TEXT = "Lend A Book";
    private const string ADD_OR_REMOVE_A_BOOK_TITLE_TEXT = "Add or Remove A Book";
    private const string ALL_LENT_BOOKS_TITLE_TEXT = "List of all Lent Books";

    private const string ACTION_BUTTON_LEND_TEXT = "Lend";
    private const string ACTION_BUTTON_RETURN_TEXT = "Return";

    public event EventHandler<OnSelectedListItemChangedEventArgs> OnSelectedListItemChanged;
    public class OnSelectedListItemChangedEventArgs : EventArgs { public SingleBookListingTemplateUI selectedListItemTemplate; };

    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI panelTitleText;
    [SerializeField] private TextMeshProUGUI searchResultsDetailsText;

    //Action button will act differently in different states, for example it will work as lend button or return button and so forth, hence the "obscure" name
    [SerializeField] private Button actionButton;
    [SerializeField] private TextMeshProUGUI actionButtonText;

  

    [SerializeField] private Button searchButton;
    [SerializeField] private TMP_Dropdown searchTypeDropdown;
    [SerializeField] private TMP_InputField searchTermInputField;

    //Legend And Vertical Layout Groups that include the listing templates
    [SerializeField] private Transform GeneralList;
    [SerializeField] private Transform LentList;

    //For Generic List
    [SerializeField] private Transform searchResultsContainer;
    [SerializeField] private Transform singleBookListingTemplate;


    //For All Lent Books - Return List 
    [SerializeField] private Transform lentListSearchResultsContainer;
    [SerializeField] private Transform lentListSingleBookListingTemplate;
    [SerializeField] private Toggle expiredDueDatesToggle;

    private ListType currentListType;
    private SingleBookListingTemplateUI selectedListing;

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);

        searchButton.onClick.AddListener(() =>
        {
            OnSearchButtonClicked();
        });

        singleBookListingTemplate.gameObject.SetActive(false);
        lentListSingleBookListingTemplate.gameObject.SetActive(false);

        Hide();
    }

    public void ChangeSelectedListing(SingleBookListingTemplateUI selectedListing)
    {
        this.selectedListing = selectedListing;

        OnSelectedListItemChanged?.Invoke(this, new OnSelectedListItemChangedEventArgs
        {
            selectedListItemTemplate = selectedListing
        });
    }

    private void OnSearchButtonClicked()
    {
        //Only "All Lent books" uses a different container and different search type 
        if(currentListType == ListType.AllLentBooksList)
        {
            SearchCriteriaSO searchCriteria = ScriptableObject.CreateInstance<SearchCriteriaSO>();
            searchCriteria.searchTypeLentList = (SearchManager.SearchTypeLentListing)Enum.Parse(typeof(SearchManager.SearchTypeLentListing), searchTypeDropdown.options[searchTypeDropdown.value].text.Replace(" ", ""));
            searchCriteria.searchTerm = searchTermInputField.text;
            searchCriteria.isExpiredLent = expiredDueDatesToggle.isOn;

            //perform Search from Lending info list (Not through book data)
            List<LendingInfoPairsSO.LendingPair> searchResults = SearchManager.PerformLendingInfoPairsSearch(searchCriteria);
            UpdateBookListForLentBooks(searchResults);
            //UpdateSearchResultList(searchResults);
        }
        else
        {
            SearchCriteriaSO searchCriteria = ScriptableObject.CreateInstance<SearchCriteriaSO>();
            //Now we need to remove spaces we added just for the UI.
            searchCriteria.searchTypeGeneral = (SearchManager.SearchTypeGeneralListing)Enum.Parse(typeof(SearchManager.SearchTypeGeneralListing), searchTypeDropdown.options[searchTypeDropdown.value].text.Replace(" ", ""));
            searchCriteria.searchTerm = searchTermInputField.text;

            if(currentListType == ListType.LendABookList)
            {
                searchCriteria.isAvailable = true;
            }

            // Call the SearchManager to perform the search
            List<BookData> searchResults = SearchManager.PerformBookListingSearch(searchCriteria);
            // Now we have the search results, update & populate the container
            UpdateSearchResultList(searchResults);
        }
       
    }

    private void UpdateSearchResultList(List<BookData> bookDataList)
    {
        foreach (Transform child in searchResultsContainer)
        {
            if (child == singleBookListingTemplate) continue;
            Destroy(child.gameObject);
        }

        int totalUniqueBookCount = 0;
        foreach (BookData result in bookDataList)
        {
            totalUniqueBookCount++;
            Transform bookListingTransform = Instantiate(singleBookListingTemplate, searchResultsContainer);
            bookListingTransform.gameObject.SetActive(true);
            bookListingTransform.GetComponent<SingleBookListingTemplateUI>().SetBookDataForBasicListing(result);
        }

        //update this line, more info is better
        searchResultsDetailsText.text = $"{totalUniqueBookCount} book(s) found.";

    }

    public void Show(ListType listType)
    {
        gameObject.SetActive(true);
        
        //sets current list type so that we will unsub from the right events
        currentListType = listType;
        SetDropdownOptions(listType);

        ChangeLegendAndContainerType(listType);

        //state machine will show and hide necessary components
        switch (listType)
        {
            case ListType.AllBooksList:
                panelTitleText.text = ALL_BOOKS_TITLE_TEXT;
                actionButton.gameObject.SetActive(false);
                //Load AllBooks Script 
                UpdateSearchResultList(LibraryManager.Instance.GetLibraryData().books);
                

                break;
            case ListType.LendABookList:
                panelTitleText.text = LEND_A_BOOK_TITLE_TEXT;
                actionButton.gameObject.SetActive(true);

                actionButtonText.text = ACTION_BUTTON_LEND_TEXT;


                List<BookData> availableBookDataList = SearchManager.GetAvailableBooks(LibraryManager.Instance.GetLibraryData().books);
                UpdateSearchResultList(availableBookDataList); 

                
                LibraryManager.Instance.OnBookLendingSuccessful += LibraryManager_OnBookLendingSuccessful;
                actionButton.onClick.AddListener(() => OnLendButtonClick(selectedListing));

                break;

            //in this list you can return the books so the button is repurposed as a return button
            case ListType.AllLentBooksList:
                panelTitleText.text = ALL_LENT_BOOKS_TITLE_TEXT;
                actionButton.gameObject.SetActive(true);
               
                actionButtonText.text = ACTION_BUTTON_RETURN_TEXT;
                expiredDueDatesToggle.onValueChanged.AddListener(UpdateBookListForLentBooks);
                expiredDueDatesToggle.isOn = false;
                LendingInfoPairsSO allLentBooksList = LibraryManager.Instance.GetLendingInfoPairs();
                UpdateBookListForLentBooks(allLentBooksList);

                LibraryManager.Instance.OnReturnFromListSuccessful += LibraryManager_OnReturnFromListSuccessful;

                LendingInfoPairsSO.LendingPair lendingPair = new LendingInfoPairsSO.LendingPair();
                int lendingInfoIndex = 0;
                actionButton.onClick.AddListener(() => OnReturnButtonClick(lendingPair,lendingInfoIndex));

                break;

            case ListType.AddOrRemovePanelList:
                panelTitleText.text = ADD_OR_REMOVE_A_BOOK_TITLE_TEXT;
                actionButton.gameObject.SetActive(false);


                break;
        }
    }

    private void LibraryManager_OnReturnFromListSuccessful(object sender, EventArgs e)
    {
        LendingInfoPairsSO allLentBooksList = LibraryManager.Instance.GetLendingInfoPairs();
        UpdateBookListForLentBooks(allLentBooksList);
    }

    private void OnReturnButtonClick(LendingInfoPairsSO.LendingPair lendingPair, int lendingInfoListIndex)
    {
        if(selectedListing == null)  return; 

        lendingPair = selectedListing.GetLendingPair();
        lendingInfoListIndex = selectedListing.GetLendingPairLendingListInfoIndex();
        string returnConfirmationMessage = $"You are about to return {lendingPair.book.bookTitle} borrowed by {lendingPair.lendingInfoList[lendingInfoListIndex].borrowerName}. Press X to cancel or Confirm to proceed.";
       
        
        //passing in function as a delegate for the next UI pop Up to trigger, we can actually use the return code now that we have a function to get the return code. may simplify things
        PopupPanelUI.Instance.ShowResponse(returnConfirmationMessage, () =>

        LibraryManager.Instance.TryReturnLentBookFromTheList(lendingPair.lendingInfoList[lendingInfoListIndex])

        );

    }

    private void OnLendButtonClick(SingleBookListingTemplateUI listing)
    {
        if (listing == null) return;

        BookData tempBook = listing.GetBookData();
        PopupPanelUI.Instance.ShowPrompt("Enter Your Name", tempBook);
    }


    private void UpdateBookListForLentBooks(List<LendingInfoPairsSO.LendingPair> lendingInfoPairs)
    {
        foreach (Transform child in lentListSearchResultsContainer)
        {
            if (child == lentListSingleBookListingTemplate) continue;
            Destroy(child.gameObject);
        }

        int totalLentEntries = 0;

        foreach (LendingInfoPairsSO.LendingPair lendingPair in lendingInfoPairs)
        {
            int lendInfoIndex = 0;
            foreach (LendingInfo lendingInfo in lendingPair.lendingInfoList)
            {
                totalLentEntries++;
                Transform bookListingTransform = Instantiate(lentListSingleBookListingTemplate, lentListSearchResultsContainer);
                bookListingTransform.gameObject.SetActive(true);
                bookListingTransform.GetComponent<SingleBookListingTemplateUI>().SetBookDataForReturningLentBook(lendingPair, lendInfoIndex);
                lendInfoIndex++;
            }
        }

        UpdateListDetailsText(totalLentEntries);

    }

    private void UpdateBookListForLentBooks(bool isExpired)
    {


        if (isExpired)
        {
            //List only the expired books
            foreach (Transform child in lentListSearchResultsContainer)
            {
                if (child == lentListSingleBookListingTemplate) continue;
                Destroy(child.gameObject);
            }

            int totalLentEntries = 0;

            //
            List<LendingInfoPairsSO.LendingPair> allLentBooks = LibraryManager.Instance.GetLendingInfoPairs().lendingPairs;
            List<LendingInfoPairsSO.LendingPair> expiredLendingPairs = SearchManager.GetExpiredBooks(allLentBooks);

            foreach (LendingInfoPairsSO.LendingPair lendingPair in expiredLendingPairs)
            {
                int lendInfoIndex = 0;
                foreach (LendingInfo lendingInfo in lendingPair.lendingInfoList)
                {
                    totalLentEntries++;
                    Transform bookListingTransform = Instantiate(lentListSingleBookListingTemplate, lentListSearchResultsContainer);
                    bookListingTransform.gameObject.SetActive(true);
                    bookListingTransform.GetComponent<SingleBookListingTemplateUI>().SetBookDataForReturningLentBook(lendingPair, lendInfoIndex);
                    lendInfoIndex++;
                }
            }

            UpdateListDetailsText(totalLentEntries);
        }
        else
        {
            //list all the books
            foreach (Transform child in lentListSearchResultsContainer)
            {
                if (child == lentListSingleBookListingTemplate) continue;
                Destroy(child.gameObject);
            }

            int totalLentEntries = 0;

            LendingInfoPairsSO LendingInfoPairs = LibraryManager.Instance.GetLendingInfoPairs();

            foreach (LendingInfoPairsSO.LendingPair lendingPair in LendingInfoPairs.lendingPairs)
            {
                int lendInfoIndex = 0;
                foreach (LendingInfo lendingInfo in lendingPair.lendingInfoList)
                {
                    totalLentEntries++;
                    Transform bookListingTransform = Instantiate(lentListSingleBookListingTemplate, lentListSearchResultsContainer);
                    bookListingTransform.gameObject.SetActive(true);
                    bookListingTransform.GetComponent<SingleBookListingTemplateUI>().SetBookDataForReturningLentBook(lendingPair, lendInfoIndex);
                    lendInfoIndex++;
                }
            }

            UpdateListDetailsText(totalLentEntries);

        }
    }

    private void UpdateListDetailsText(int totalLentEntries)
    {
        searchResultsDetailsText.text = $"Total Lent Books: {totalLentEntries}";
    }




    private void LibraryManager_OnBookLendingSuccessful(object sender, System.EventArgs e)
    {
        UpdateSearchResultList(SearchManager.GetAvailableBooks(LibraryManager.Instance.GetLibraryData().books));
    }



    private void SetDropdownOptions(ListType listType)
    {
        // Only "All Lent books" list have a different category, can search borrower name
        // Clear existing options
        searchTypeDropdown.ClearOptions();

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
            searchTypeDropdown.AddOptions(new List<string>(searchTypeOptions));
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
            searchTypeDropdown.AddOptions(new List<string>(searchTypeOptions));
        }
       
    }

    string AddSpaceBeforeUpperCase(string input)
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

    public void ChangeLegendAndContainerType(ListType listType)
    {
        //AllLentBooks uses different Legend/templates thus this code sets on/off the right template
        if(listType == ListType.AllLentBooksList)
        {
            LentList.gameObject.SetActive(true);
            GeneralList.gameObject.SetActive(false);
        }
        else
        {
            GeneralList.gameObject.SetActive(true);
            LentList.gameObject.SetActive(false);
        }
    }

    public void Hide()
    {
        //search toggle reset
        selectedListing = null;

        if (currentListType == ListType.AllLentBooksList)
        {
            expiredDueDatesToggle.onValueChanged.RemoveAllListeners();
            actionButton.onClick.RemoveAllListeners();
            LibraryManager.Instance.OnReturnFromListSuccessful -= LibraryManager_OnReturnFromListSuccessful;
        }
        if (currentListType == ListType.LendABookList)
        {
            LibraryManager.Instance.OnBookLendingSuccessful -= LibraryManager_OnBookLendingSuccessful;
            actionButton.onClick.RemoveAllListeners();
        }


        searchTermInputField.text = "";
        gameObject.SetActive(false);
    }
}
