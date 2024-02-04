using System;
using System.Collections.Generic;
using UnityEngine;

public class LibraryManager : MonoBehaviour
{
    public static LibraryManager Instance { get; private set; }

    public event EventHandler<EventArgs> OnLibraryDataUpdatedForLists;

    [SerializeField] private LibraryDataSO _libraryData;
    [SerializeField] private LendingInfoPairsSO _lendingInfoPairsList;

    private void Awake()
    {
        Instance = this;  
    }

    private void Start()
    {
        ImportExportManager.ImportFromJsonForRuntime();
    }


    #region MainOperationMethods
    public BookData CreateBookData(string bookTitle, string bookAuthor, string bookIsbn)
    {
        BookData newBookData = new BookData
        {
            BookTitle = bookTitle,
            BookAuthor = bookAuthor,
            BookIsbn = bookIsbn,
            BookCount = 1
        };
        return newBookData;
    }
    public void AddBookToLibrary(BookData bookData)
    {
        if (IsBookListedInLibraryAlready(bookData))
        {
            IncreaseBookCountByOne(bookData);
        }
        else
        {
            AddNewBookToLibrary(bookData);
        }
    }

    private void AddNewBookToLibrary(BookData bookData)
    {
        _libraryData.Books.Add(bookData);
        SaveLibraryData();
    }

    public void IncreaseBookCountByOne(BookData bookData)
    {
        int bookCountToIncrease = 1;
        IncreaseBookCountByAmount(bookData, bookCountToIncrease);
    }

    public void IncreaseBookCountByAmount(BookData bookData, int amountToIncrease)
    {
        BookData existingBook = GetExistingBook(bookData);

        if (existingBook != null)
        {
            for (int i = 0; i < amountToIncrease; i++)
            {
                existingBook.BookCount++;
            }

            SaveLibraryData();
        }
    }

    public void DecreaseBookCountByAmount(BookData bookData, int amountToDecrease, out string actualDecreaseAmount)
    {
        BookData existingBook = GetExistingBook(bookData);
        actualDecreaseAmount = "";

        if (existingBook != null)
        {
            int decreaseAmount = 0;
            for (int i = 0; i < amountToDecrease; i++)
            {
                if (existingBook.BookCount > 0)
                {
                    existingBook.BookCount--;
                    decreaseAmount++;
                }
                else
                {
                    break; // If no more books are available to decrease, exit the loop
                }
            }

            actualDecreaseAmount = decreaseAmount.ToString();
            SaveLibraryData();
        }
    }

    public void IncreaseTheNumberOfBooks(BookData bookData, string numberOfBooksToIncrease)
    {
        int increaseAmount = int.Parse(numberOfBooksToIncrease);

        IncreaseBookCountByAmount(bookData, increaseAmount);

        OnLibraryDataUpdatedForLists?.Invoke(this, EventArgs.Empty);
        string responseMessage = $"Number of copies of '{bookData.BookTitle}' (ISBN: '{bookData.BookIsbn}') increased by '{numberOfBooksToIncrease}'.";
        PopupPanelUI.Instance.ShowResponse(responseMessage);

    }

    public void DecreaseTheNumberOfBooks(BookData bookData, string numberOfBooksToDecrease)
    {
        int decreaseAmount = int.Parse(numberOfBooksToDecrease);
        string actualDecreaseAmount;
        DecreaseBookCountByAmount(bookData, decreaseAmount, out actualDecreaseAmount);

        //Update the list and show response message
        OnLibraryDataUpdatedForLists?.Invoke(this, EventArgs.Empty);
        string responseMessage = $"Number of copies of '{bookData.BookTitle}' (ISBN: '{bookData.BookIsbn}') decreased by '{actualDecreaseAmount}'.";
        PopupPanelUI.Instance.ShowResponse(responseMessage);
    }

    public void DeleteSingleBookDataInformation(BookData bookData)
    {
        BookData existingBook = GetExistingBook(bookData);

        if (_libraryData.Books.Contains(bookData))
        {
            _libraryData.Books.Remove(bookData);
        }

        //now remove the book data from lending pair list, first we need to find the index

        for (int i = 0; i < _lendingInfoPairsList.LendingPairs.Count; i++)
        {
            LendingInfoPairsSO.LendingPair lendingPair = _lendingInfoPairsList.LendingPairs[i];

            //if I did book = bookData it might not find it due to how count is stored and if it doesnt match it skips it
            if (lendingPair.Book.BookIsbn == bookData.BookIsbn)
            {
                _lendingInfoPairsList.LendingPairs.RemoveAt(i);
                i--;
            }
            else
            {
                //no lent book exists on this listing 
            }
        }
        SaveLibraryData();

        OnLibraryDataUpdatedForLists?.Invoke(this, EventArgs.Empty);
        string responseMessage = $"Book Data about '{bookData.BookTitle}' (ISBN: '{bookData.BookIsbn}') and all the related info is deleted from the library.";
        PopupPanelUI.Instance.ShowResponse(responseMessage);
    }
    public void LendABook(BookData bookData, string borrowerName)
    {
        //Checks If bookData is already in the lendingInfoPairsSO List, checks from book ISBN to avoid addition of multiple book-lendingInfoPairs
        LendingInfoPairsSO.LendingPair lendingPair = _lendingInfoPairsList.LendingPairs.Find(pair => pair.Book.BookIsbn.Equals(bookData.BookIsbn));

        if (lendingPair == null)
        {
            // If the bookData doesn't exist, create a new lending pair
            lendingPair = new LendingInfoPairsSO.LendingPair
            {
                Book = bookData,
               // TotalLentBookCount = 0,
                LendingInfoList = new List<LendingInfo>()
            };

            _lendingInfoPairsList.LendingPairs.Add(lendingPair);
        }

        // Generate a unique return code from the static class 
        string returnCode = ReturnCodeGeneratorAndChecker.GenerateReturnCode();

        // Create a new lending info
        LendingInfo lendingInfo = new LendingInfo
        {
            BorrowerName = borrowerName,
            ReturnCode = returnCode,
            ExpectedReturnDateTicks = DateTime.Now.AddDays(30).Ticks // Assuming a 30-day lending period
        };

        lendingPair.LendingInfoList.Add(lendingInfo);
        //lendingPair.TotalLentBookCount++;

        //this is because we check if this book is available before loading the lendable book list 
        bookData.BookCount--;
        // Save changes to the ScriptableObject
        SaveLibraryData();

        DateTime deserializeDate = new DateTime(lendingInfo.ExpectedReturnDateTicks);

        string lendingSuccessfulResponseMessage = $"'{bookData.BookTitle}' (ISBN: '{bookData.BookIsbn}') borrowed by '{borrowerName}' successfully. \n If there are any issues or concerns, please contact the library.\n Return Code: '{lendingInfo.ReturnCode}'\n Return Due Date: {deserializeDate.ToString("MM/dd/yyyy")}";
        

        PopupPanelUI.Instance.ShowResponse(lendingSuccessfulResponseMessage);
        OnLibraryDataUpdatedForLists?.Invoke(this, EventArgs.Empty);   
    }
    public void TryReturnLentBookFromTheList(LendingInfo lendingInfo)
    {

        TryReturnLentBookByReturnCode(lendingInfo.ReturnCode);

        OnLibraryDataUpdatedForLists?.Invoke(this, EventArgs.Empty);
    }
    public void TryReturnLentBookByReturnCode(string returnCode)
    {
        LendingInfoPairsSO.LendingPair matchingPair = ReturnCodeGeneratorAndChecker.SearchForReturnCodeValidity(returnCode);


        if (matchingPair != null)
        {
            BookData returnedBook = matchingPair.Book;
            LendingInfo lendingInfoToRemove = matchingPair.LendingInfoList.Find(info => info.ReturnCode == returnCode);

            BookData libraryBook = _libraryData.Books.Find(book => book.BookIsbn.Equals(returnedBook.BookIsbn));

            if (libraryBook != null)
            {
                libraryBook.BookCount++;
                // Remove lending info
                matchingPair.LendingInfoList.Remove(lendingInfoToRemove);

                // If no lending info remains for the book, remove the entire LendingPair
                if (matchingPair.LendingInfoList.Count == 0)
                {
                    _lendingInfoPairsList.LendingPairs.Remove(matchingPair);
                }

                SaveLibraryData();
            }
            //Should add if due date has passed, penalty fee maybe?
            string returnSuccessfulResponseMessage = $"'{returnedBook.BookTitle}' (ISBN: '{returnedBook.BookIsbn}') borrowed by '{lendingInfoToRemove.BorrowerName}' returned successfully.";

            PopupPanelUI.Instance.ShowResponse(returnSuccessfulResponseMessage);


        }
        else
        {
            string errorMessage = "Return Code you provided(" + returnCode + ") not found.";

            PopupPanelUI.Instance.ShowError(errorMessage);
        }

    }
    #endregion

    #region FindingAndExposingLibraryData

    public LendingInfoPairsSO GetLendingInfoPairs()
    {
        if (_lendingInfoPairsList == null)
        {
            Debug.LogWarning("LendingInfoPairsList data is null. Make sure it has been assigned.");
            string errorMessage = "Lending Pairs data not found.";
            PopupPanelUI.Instance.ShowError(errorMessage);
        }
        return _lendingInfoPairsList;
    }

    public LibraryDataSO GetLibraryData()
    {
        if (_libraryData == null)
        {
            Debug.LogWarning("Library data is null. Make sure it has been assigned.");
            string errorMessage = "Library data not found.";
            PopupPanelUI.Instance.ShowError(errorMessage);
        }
        return _libraryData;
    }
    public BookData GetExistingBook(BookData bookData)
    {
        //return libraryData.books.FirstOrDefault(existingBook => existingBook.Equals(bookData));
        //this does not work because bookData.bookcount is different for the newly created book, thus it does not return the same book
        return FindBookInLibrary(bookData);
    }

    public bool IsBookListedInLibraryAlready(BookData bookData)
    {
        return FindBookInLibrary(bookData) != null;
    }

    private BookData FindBookInLibrary(BookData bookData)
    {
        return SearchManager.FindBookIfItExistsInTheLibrary(bookData.BookTitle, bookData.BookAuthor, bookData.BookIsbn, checkDifferentIsbn: false);
    }
    #endregion

    #region SavingDeletingAndRelatedOperationMethods
    private void SaveLibraryData()
    {
        try
        {
            ImportExportManager.ExportToJsonForRuntime();
        }catch (Exception ex)
        {
            Debug.LogError("An error occurred while trying to save: " + ex.Message);
            string errorResponse = "An error occurred while trying to save: " + ex.Message;
            PopupPanelUI.Instance.ShowError(errorResponse);
        }
        // Saving the ScriptableObject asset(only available for Unity Editor version) 
        // UnityEditor.EditorUtility.SetDirty(libraryData);
        // UnityEditor.EditorUtility.SetDirty(lendingInfoPairsList);
        // UnityEditor.AssetDatabase.SaveAssets();
        // UnityEditor.AssetDatabase.Refresh();
    }

    public void UpdateLibraryDataFromJsonData(LibraryDataSO libraryData, LendingInfoPairsSO lendingInfoPairs)
    {
        ClearLocalLibraryData();

        //addrange method to avoid changing the reference this.libraryData = libraryData would change the reference
        this._libraryData.Books.AddRange(libraryData.Books);
        this._lendingInfoPairsList.LendingPairs.AddRange(lendingInfoPairs.LendingPairs);

        SaveLibraryData();
    }
    //Deletes the data stored on Scriptable objects
    public void DeleteLocalLibraryDataFromUserPrompt()
    {
        try
        {
            ClearLocalLibraryData();

            string popupResonseMessage = "Library Data successfully deleted.";
            PopupPanelUI.Instance.ShowResponse(popupResonseMessage);

        }
        catch (Exception ex)
        {
            Debug.LogError("An error occurred while deleting library data: " + ex.Message);
            string errorResponse = "An error occurred while deleting library data. Please try again.";
            PopupPanelUI.Instance.ShowError(errorResponse);
        }
    }

    private void ClearLocalLibraryData()
    {
        _libraryData.Books.Clear();
        _lendingInfoPairsList.LendingPairs.Clear();
        SaveLibraryData();
    }

    #endregion
}

