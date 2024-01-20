using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class LibraryManager : MonoBehaviour
{
    public static LibraryManager Instance { get; private set; }


    public event EventHandler<OnErrorEncounteredEventArgs> OnErrorEncountered;
    public class OnErrorEncounteredEventArgs : EventArgs { public string errorMessage; }

    [SerializeField] private LibraryDataSO libraryData;
    [SerializeField] private LendingInfoPairsSO lendingInfoPairsList;

    private BookData selectedBookData;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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

        //CLOSE THE UI WINDOW AND GIVE FEEDBACK LIKE (YOU BORROWED X BY X ON THIS DAY YOU ARE EXPECTED TO RETURN IT BY BLA BLA 
    }



    public void TakeLendedBookBack() { }


    public void ListAllBooksInLibrary() { 
    
    }
    public void SearchBookByTitle() { }

    public void SearchBookByAuthor() { }


    public void ListBooksWithPassedDueDate() { }

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

    private void LoadDataFromPlayerPrefs()
    {

    }

}

