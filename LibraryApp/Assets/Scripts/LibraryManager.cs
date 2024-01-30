using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class LibraryManager : MonoBehaviour
{
    public static LibraryManager Instance { get; private set; }


    public event EventHandler<EventArgs> OnBookLendingSuccessful;
    public event EventHandler<EventArgs> OnReturnFromListSuccessful;
    public event EventHandler<EventArgs> OnListDataUpdated;

    [SerializeField] private LibraryDataSO libraryData;
    [SerializeField] private LendingInfoPairsSO lendingInfoPairsList;

    private void Awake()
    {
        Instance = this;
    }
    
    #region MainOperationMethods
    public BookData CreateBookData(string bookTitle, string bookAuthor, string bookIsbn)
    {
        BookData newBookData = new BookData
        {
            bookTitle = bookTitle,
            bookAuthor = bookAuthor,
            bookIsbn = bookIsbn,
            bookCount = 1
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
        libraryData.books.Add(bookData);
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
                existingBook.bookCount++;
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
                if (existingBook.bookCount > 0)
                {
                    existingBook.bookCount--;
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

        OnListDataUpdated?.Invoke(this, EventArgs.Empty);
        string responseMessage = $"Number of copies of '{bookData.bookTitle}' (ISBN: '{bookData.bookIsbn}') increased by '{numberOfBooksToIncrease}'.";
        PopupPanelUI.Instance.ShowResponse(responseMessage);

    }

    public void DecreaseTheNumberOfBooks(BookData bookData, string numberOfBooksToDecrease)
    {
        int decreaseAmount = int.Parse(numberOfBooksToDecrease);
        string actualDecreaseAmount;
        DecreaseBookCountByAmount(bookData, decreaseAmount, out actualDecreaseAmount);

        //Update the list and show response message
        OnListDataUpdated?.Invoke(this, EventArgs.Empty);
        string responseMessage = $"Number of copies of '{bookData.bookTitle}' (ISBN: '{bookData.bookIsbn}') decreased by '{actualDecreaseAmount}'.";
        PopupPanelUI.Instance.ShowResponse(responseMessage);
    }

    public void DeleteSingleBookDataInformation(BookData bookData)
    {
        BookData existingBook = GetExistingBook(bookData);

        if (libraryData.books.Contains(bookData))
        {
            libraryData.books.Remove(bookData);
        }

        //now remove the book data from lending pair list, first we need to find the index

        for (int i = 0; i < lendingInfoPairsList.lendingPairs.Count; i++)
        {
            LendingInfoPairsSO.LendingPair lendingPair = lendingInfoPairsList.lendingPairs[i];

            //if I did book = bookData it might not find it due to how count is stored and if it doesnt match it skips it
            if (lendingPair.book.bookIsbn == bookData.bookIsbn)
            {
                lendingInfoPairsList.lendingPairs.RemoveAt(i);
                i--;
            }
            else
            {
                //no lent book exists on this listing 
            }
        }
        SaveLibraryData();

        OnListDataUpdated?.Invoke(this, EventArgs.Empty);
        string responseMessage = $"Book Data about '{bookData.bookTitle}' (ISBN: '{bookData.bookIsbn}') and all the related info is deleted from the library.";
        PopupPanelUI.Instance.ShowResponse(responseMessage);
    }
    public void LendABook(BookData bookData, string borrowerName)
    {
        //Checks If bookData is already in the lendingInfoPairsSO List, checks from book ISBN to avoid addition of multiple book-lendingInfoPairs
        LendingInfoPairsSO.LendingPair lendingPair = lendingInfoPairsList.lendingPairs.Find(pair => pair.book.bookIsbn.Equals(bookData.bookIsbn));

        if (lendingPair == null)
        {
            // If the bookData doesn't exist, create a new lending pair
            lendingPair = new LendingInfoPairsSO.LendingPair
            {
                book = bookData,
                totalLendedBookCount = 0,
                lendingInfoList = new List<LendingInfo>()
            };

            lendingInfoPairsList.lendingPairs.Add(lendingPair);
        }

        // Generate a unique return code from the static class 
        string returnCode = ReturnCodeGeneratorAndChecker.GenerateReturnCode();

        // Create a new lending info
        LendingInfo lendingInfo = new LendingInfo
        {
            borrowerName = borrowerName,
            returnCode = returnCode,
            expectedReturnDateTicks = DateTime.Now.AddDays(30).Ticks // Assuming a 30-day lending period
        };

        lendingPair.lendingInfoList.Add(lendingInfo);
        lendingPair.totalLendedBookCount++;

        //this is because we check if this book is available before loading the lendable book list 
        bookData.bookCount--;
        // Save changes to the ScriptableObject
        SaveLibraryData();

        DateTime deserializeDate = new DateTime(lendingInfo.expectedReturnDateTicks);

        string lendingSuccessfulResponseMessage = $"'{bookData.bookTitle}' (ISBN: '{bookData.bookIsbn}') borrowed by '{borrowerName}' successfully. \n If there are any issues or concerns, please contact the library.\n Return Code: '{lendingInfo.returnCode}'\n Return Due Date: {deserializeDate.ToString("MM/dd/yyyy")}";
        

        PopupPanelUI.Instance.ShowResponse(lendingSuccessfulResponseMessage);
        OnBookLendingSuccessful?.Invoke(this, EventArgs.Empty);   
    }
    public void TryReturnLentBookFromTheList(LendingInfo lendingInfo)
    {

        TryReturnLentBookByReturnCode(lendingInfo.returnCode);

        OnReturnFromListSuccessful?.Invoke(this, EventArgs.Empty);
    }
    public void TryReturnLentBookByReturnCode(string returnCode)
    {
        LendingInfoPairsSO.LendingPair matchingPair = ReturnCodeGeneratorAndChecker.SearchForReturnCodeValidity(returnCode);


        if (matchingPair != null)
        {
            BookData returnedBook = matchingPair.book;
            LendingInfo lendingInfoToRemove = matchingPair.lendingInfoList.Find(info => info.returnCode == returnCode);

            BookData libraryBook = libraryData.books.Find(book => book.bookIsbn.Equals(returnedBook.bookIsbn));

            if (libraryBook != null)
            {
                libraryBook.bookCount++;
                // Remove lending info
                matchingPair.lendingInfoList.Remove(lendingInfoToRemove);

                // If no lending info remains for the book, remove the entire LendingPair
                if (matchingPair.lendingInfoList.Count == 0)
                {
                    lendingInfoPairsList.lendingPairs.Remove(matchingPair);
                }

                SaveLibraryData();
            }
            //Should add if due date has passed, penalty fee maybe?
            string returnSuccessfulResponseMessage = $"'{returnedBook.bookTitle}' (ISBN: '{returnedBook.bookIsbn}') borrowed by '{lendingInfoToRemove.borrowerName}' returned successfully.";

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
        if (lendingInfoPairsList == null)
        {
            Debug.LogWarning("LendingInfoPairsList data is null. Make sure it has been assigned.");
            string errorMessage = "Lending Pairs data not found.";
            PopupPanelUI.Instance.ShowError(errorMessage);
        }
        return lendingInfoPairsList;
    }

    public LibraryDataSO GetLibraryData()
    {
        if (libraryData == null)
        {
            Debug.LogWarning("Library data is null. Make sure it has been assigned.");
            string errorMessage = "Library data not found.";
            PopupPanelUI.Instance.ShowError(errorMessage);
        }
        return libraryData;
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
        return SearchManager.FindBookIfItExistsInTheLibrary(bookData.bookTitle, bookData.bookAuthor, bookData.bookIsbn, checkDifferentIsbn: false);
    }
    #endregion

    #region SavingDeletingAndRelatedOperationMethods
    private void SaveLibraryData()
    {
        // Saving the ScriptableObject asset
        UnityEditor.EditorUtility.SetDirty(libraryData);
        UnityEditor.EditorUtility.SetDirty(lendingInfoPairsList);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }

    public void UpdateLibraryDataFromJsonData(LibraryDataSO libraryData, LendingInfoPairsSO lendingInfoPairs)
    {
        DeleteLocalLibraryData();

        //addrange method to avoid changing the reference this.libraryData = libraryData would change the reference
        this.libraryData.books.AddRange(libraryData.books);
        this.lendingInfoPairsList.lendingPairs.AddRange(lendingInfoPairs.lendingPairs);

        SaveLibraryData();
    }
    //Deletes the data stored on Scriptable objects
    public void DeleteLocalLibraryData()
    {
        try
        {
            libraryData.books.Clear();
            lendingInfoPairsList.lendingPairs.Clear();
            SaveLibraryData();

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
    #endregion
}

