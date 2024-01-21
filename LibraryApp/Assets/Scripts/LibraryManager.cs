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


    public event EventHandler<OnErrorEncounteredEventArgs> OnErrorEncountered;
    public class OnErrorEncounteredEventArgs : EventArgs { public string errorMessage; }

    [SerializeField] private LibraryDataSO libraryData;
    [SerializeField] private LendingInfoPairsSO lendingInfoPairsList;

    private BookData selectedBookData;
    private EventArgs sender;

    private void Awake()
    {
        Instance = this;
    }
    
    public LendingInfoPairsSO GetLendingInfoPairs()
    {
        if(lendingInfoPairsList == null)
        {
            Debug.LogWarning("LendingInfoPairsList data is null. Make sure it has been assigned.");
        }
        return lendingInfoPairsList;
    }

    public LibraryDataSO GetLibraryData()
    {
        if (libraryData == null)
        {
            Debug.LogWarning("Library data is null. Make sure it has been assigned.");
            // Optionally, you can create a new instance or return a default value here.
        }
        return libraryData;
    }
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

    //check where you save your data
    private void SaveLibraryData()
    {
        // Saving the ScriptableObject asset
        UnityEditor.EditorUtility.SetDirty(libraryData);
        UnityEditor.EditorUtility.SetDirty(lendingInfoPairsList);
        UnityEditor.AssetDatabase.SaveAssets();
    }


    public void AddBookToLibrary(BookData bookData) {
        if (IsBookListedinLibraryAlready(bookData))
        {
            IncreaseBookCountByOne(bookData);
        }
        else
        {
            libraryData.books.Add(bookData);
            SaveLibraryData();
            Debug.Log(libraryData.books.Count);
        }
    }

    public void IncreaseBookCountByOne(BookData bookData)
    {
        if (IsBookListedinLibraryAlready(bookData))
        {
            //bu yapýnýn sebebi baþta bookData'yý struct olarak tutumuþtum, ref type tutmuyordum ondan birkaç satýr bu tarz iþler yapýyordum class olunca gereksiz kaçtý refactor edilmeli.
            int bookDataIndex = GetIndexOfExistingBook(bookData);
            libraryData.books[bookDataIndex].bookCount++;
        }
    }

    public void IncreaseBookCountInBulk(BookData bookData, int bookCount)
    {
        for(int i = 0; i < bookCount; i++)
        {
            IncreaseBookCountByOne(bookData);
        }
    }

    public void DecreaseBookCountByOne(BookData bookData)
    {
        if (IsBookListedinLibraryAlready(bookData))
        {
            int bookDataIndex = libraryData.books.IndexOf(bookData);
            if (libraryData.books[bookDataIndex].bookCount >= 1)
            {
                libraryData.books[bookDataIndex].bookCount--;

            }
            else
            {
                OnErrorEncountered?.Invoke(this, new OnErrorEncounteredEventArgs
                {
                    errorMessage = "No books remaining."
            });
                
            }
        }
    }

    public void DecreaseBookCountInBulk(BookData bookData, int bookCount)
    {
        for (int i = 0; i < bookCount; i++)
        {
            //Buraya ayar çekilmeli, sýfýrýn altýna inmediði için errorMessage popup'ý fýrlatýp durucak. 
            DecreaseBookCountByOne(bookData);
        }
    }

    public void TryReturnLentBookByReturnCode(string returnCode)
    {
        LendingInfoPairsSO.LendingPair matchingPair = ReturnCodeGeneratorAndChecker.SearchForReturnCodeValidity(returnCode);


        if (matchingPair != null)
        {
            BookData returnedBook = matchingPair.book;
            LendingInfo lendingInfoToRemove = matchingPair.lendingInfoList.Find(info => info.returnCode == returnCode);

            BookData libraryBook = libraryData.books.Find(book => book.Equals(returnedBook));

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

            LendAndReturnResponsePanelUI.Instance.Show(returnSuccessfulResponseMessage);
            
        }
        else
        {
            //Not good practice to have so different panels to give some message...
            OnErrorEncountered?.Invoke(this, new OnErrorEncounteredEventArgs
            {
                errorMessage = "Return Code you provided(" + returnCode + ") not found."
            }) ;
           
        }

    }

    public void LendABook(BookData bookData, string borrowerName)
    {
        //Checks If bookData is already in the lendingInfoPairsSO List
        LendingInfoPairsSO.LendingPair lendingPair = lendingInfoPairsList.lendingPairs.Find(pair => pair.book.Equals(bookData));

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
        string returnCode;
        do
        {
            returnCode = ReturnCodeGeneratorAndChecker.GenerateReturnCode();
        } while (!ReturnCodeGeneratorAndChecker.IsReturnCodeUnused(returnCode, lendingInfoPairsList.lendingPairs));

        // Create a new lending info
        LendingInfo lendingInfo = new LendingInfo
        {
            borrowerName = borrowerName,
            returnCode = returnCode,
            bookBorrowDate = DateTime.Now,
            expectedReturnDate = DateTime.Now.AddDays(30) // Assuming a 30-day lending period
        };

        lendingPair.lendingInfoList.Add(lendingInfo);
        lendingPair.totalLendedBookCount++;

        //this is because we check if this book is available before loading the lendable book list 
        bookData.bookCount--;
        // Save changes to the ScriptableObject
        SaveLibraryData();

        string lendingSuccessfulResponseMessage = $"'{bookData.bookTitle}' (ISBN: '{bookData.bookIsbn}') borrowed by '{borrowerName}' successfully. \n If there are any issues or concerns, please contact the library.\n Return Code: '{lendingInfo.returnCode}'\n Return Due Date: {lendingInfo.expectedReturnDate.ToString("MM/dd/yyyy")}";

        LendAndReturnResponsePanelUI.Instance.Show(lendingSuccessfulResponseMessage);

        //Might change how panel reacts accordingly to events (maybe OnBookLendingUnsuccessful), rather than setting the message here
        OnBookLendingSuccessful?.Invoke(sender, EventArgs.Empty); 
    }

    public int GetIndexOfExistingBook(BookData bookData)
    {
        if (libraryData != null && libraryData.books != null)
        {
            return libraryData.books.FindIndex(existingBook =>
                existingBook.bookTitle == bookData.bookTitle &&
                existingBook.bookAuthor == bookData.bookAuthor &&
                existingBook.bookIsbn == bookData.bookIsbn);
        }

        return -1; 
    }

    public bool IsBookListedinLibraryAlready(BookData bookData)
    {
        if (libraryData != null && libraryData.books != null)
        {
            return libraryData.books.Any(existingBook =>
                existingBook.bookTitle == bookData.bookTitle &&
                existingBook.bookAuthor == bookData.bookAuthor &&
                existingBook.bookIsbn == bookData.bookIsbn);
        }
        return false;
    }


    public bool IsBookAvailable(BookData bookData) {
        if (IsBookListedinLibraryAlready(bookData))
        {
            int bookDataIndex = libraryData.books.IndexOf(bookData);
            if (libraryData.books[bookDataIndex].bookCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }   
        return false;
    }

    public BookData GetBookDataFromInput(string bookName) {

        return selectedBookData;
    }

    public int GetBookDataIndex(BookData bookData)
    {
        return libraryData.books.IndexOf(bookData);
    }

}

