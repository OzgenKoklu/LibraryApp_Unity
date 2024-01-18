using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class LibraryManager : MonoBehaviour
{
    public static LibraryManager Instance { get; private set; }


    public event EventHandler<OnErrorEncounteredEventArgs> OnErrorEncountered;
    public class OnErrorEncounteredEventArgs : EventArgs { public string errorMessage; }

    private List<BookData> bookDataList;
    private Dictionary<BookData, List<LendingInfo>> bookLendingInfoPairs;
    private BookData selectedBookData;

    private void Awake()
    {
        Instance = this;
        bookDataList = new List<BookData>();

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddBookToLibrary(BookData bookData) {
        if (IsBookListedinLibraryAlready(bookData))
        {
            IncreaseBookCountByOne(bookData);
        }
        else
        {
            bookDataList.Add(bookData);
            IncreaseBookCountByOne(bookData);
        }
    }

    public void IncreaseBookCountByOne(BookData bookData)
    {
        if (IsBookListedinLibraryAlready(bookData))
        {
            int bookDataIndex = bookDataList.IndexOf(bookData);
            BookData updatedBookData = bookDataList[bookDataIndex];
            updatedBookData.bookCount++;
            bookDataList[bookDataIndex] = updatedBookData;
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
            int bookDataIndex = bookDataList.IndexOf(bookData);
            BookData updatedBookData = bookDataList[bookDataIndex];
            if (updatedBookData.bookCount >= 1)
            {
                updatedBookData.bookCount--;
                bookDataList[bookDataIndex] = updatedBookData;
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

    public void LendABook(BookData bookData)
    {
        if (IsBookAvailable(bookData))
        {

        }
    }

    public void TakeLendedBookBack() { }


    public void ListAllBooksInLibrary() { 
    
    }
    public void SearchBookByTitle() { }

    public void SearchBookByAuthor() { }


    public void ListBooksWithPassedDueDate() { } 

    public bool IsBookListedinLibraryAlready(BookData bookData)
    {
        return bookDataList.Contains(bookData);
    }


    public bool IsBookAvailable(BookData bookData) {
        if (IsBookListedinLibraryAlready(bookData))
        {
            int bookDataIndex = bookDataList.IndexOf(bookData);
            selectedBookData = bookDataList[bookDataIndex];
            if(selectedBookData.bookCount > 0)
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
        return bookDataList.IndexOf(bookData);
    }

    private void LoadDataFromPlayerPrefs()
    {

    }

}

