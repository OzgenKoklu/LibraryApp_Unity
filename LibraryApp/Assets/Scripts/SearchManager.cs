using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SearchManager
{
    public enum SearchTypeGeneralListing
    {
        General,
        ByTitle,
        ByIsbn,
        ByAuthor,
    }

    public enum SearchTypeLentListing
    {
        General,
        ByTitle,
        ByAuthor,
        ByBorrower,
    }

    public static List<LendingInfoPairsSO.LendingPair> PerformLendingInfoPairsSearch(SearchCriteriaSO searchCriteria)
    {
        List<LendingInfoPairsSO.LendingPair> searchResults = new List<LendingInfoPairsSO.LendingPair>();

        switch (searchCriteria.SearchTypeLentList)
        {
            case SearchTypeLentListing.General:
                PerformGeneralSearchOnInfoPairs(searchCriteria, searchResults);
                break;

            case SearchTypeLentListing.ByBorrower:
                PerformBorrowerNameSearchOnInfoPairs(searchCriteria, searchResults);
                break;

            case SearchTypeLentListing.ByTitle:
                PerformBookTitleSearchOnInfoPairs(searchCriteria, searchResults);
                break;


            case SearchTypeLentListing.ByAuthor:
                PerformAuthorSearchOnInfoPairs(searchCriteria, searchResults);
                break;

            default:
                Debug.LogError("Unsupported search type");
                break;
        }

        //Gets expired books if expired books toggle is enabled
        if (searchCriteria.IsExpiredLent)
        {
            searchResults = GetExpiredBooks(searchResults);
        }

        return searchResults;
    }

    public static List<BookData> PerformBookListingSearch(SearchCriteriaSO searchCriteria)
    {
        List<BookData> searchResults = new List<BookData>();

        switch (searchCriteria.SearchTypeGeneral)
        {
            case SearchTypeGeneralListing.General:
                PerformGeneralSearchOnBookList(searchCriteria, searchResults);
                break;

            case SearchTypeGeneralListing.ByTitle:
                PerformBookTitleSearchOnBookList(searchCriteria, searchResults);
                break;

            case SearchTypeGeneralListing.ByIsbn:
                PerformIsbnSearchOnBookList(searchCriteria, searchResults);
                break;

            case SearchTypeGeneralListing.ByAuthor:
                PerformAuthorSearchOnBookList(searchCriteria, searchResults);
                break;

            default:
                Debug.LogError("Unsupported search type");
                break;
        }

        //mainly for lending a book page where only available books are listed, can be customized to be included in general list searches with the addition of advanced search options
        if (searchCriteria.IsAvailable)
        {
            searchResults = GetAvailableBooks(searchResults);
        }

        // Return the list of search results
        return searchResults;

        // You can now use searchCriteria.searchTerm and searchCriteria.searchType to perform the search
    }

    public static List<BookData> GetAvailableBooks(List<BookData> bookDataList)
    {
        // Use LINQ to filter the list of books based on the book count
        var availableBooks = bookDataList.Where(book => book.BookCount > 0).ToList();

        return availableBooks;
    }

    public static List<LendingInfoPairsSO.LendingPair> GetExpiredBooks(List<LendingInfoPairsSO.LendingPair> lentBooks)
    {
        List<LendingInfoPairsSO.LendingPair> expiredBookLents = new List<LendingInfoPairsSO.LendingPair>();

        foreach (LendingInfoPairsSO.LendingPair lendingPair in lentBooks)
        {
            int totalNumberOfOverDue = 0;
            LendingInfoPairsSO.LendingPair lendingPairInTest = new LendingInfoPairsSO.LendingPair();
            lendingPairInTest.Book = lendingPair.Book;

            foreach (LendingInfo lendingInfo in lendingPair.LendingInfoList)
            {
                // Get the expected return date as a DateTime for the lending Info
                DateTime expectedReturnDate = new DateTime(lendingInfo.ExpectedReturnDateTicks);

                // Compare the expected return date with the current time to check if its exceeded
                if (expectedReturnDate < DateTime.Now)
                {
                    totalNumberOfOverDue++;
                    lendingPairInTest.LendingInfoList.Add(lendingInfo);
                }
            }
            if (totalNumberOfOverDue > 0)
            {
                expiredBookLents.Add(lendingPairInTest);
            }
        }
        return expiredBookLents;
    }

    private static void PerformGeneralSearchOnInfoPairs(SearchCriteriaSO searchCriteria, List<LendingInfoPairsSO.LendingPair> searchResults)
    {
        List<LendingInfoPairsSO.LendingPair> allLentBooks = LibraryManager.Instance.GetLendingInfoPairs().LendingPairs;

        string searchTerm = searchCriteria.SearchTerm.ToLower();

        // Check if the search term is empty, if so, return an empty result list
        if (string.IsNullOrEmpty(searchTerm))
        {
            return;
        }

        foreach (LendingInfoPairsSO.LendingPair lendingPair in allLentBooks)
        {
            int totalNumberOfResults = 0;
            int lendingInfoIndex = 0;
            //since we hold bookdata + lendingInfoList, we want to be sure that this entry is added as a BookData + List<LendingInfo) list. 
            LendingInfoPairsSO.LendingPair lendingPairInTest = new LendingInfoPairsSO.LendingPair();
            lendingPairInTest.Book = lendingPair.Book;

            foreach (LendingInfo lendingInfo in lendingPair.LendingInfoList)
            {

                if (ContainsSearchTermInLendingPairListIndex(lendingPair, lendingInfoIndex, searchTerm))
                {
                    totalNumberOfResults++;
                    lendingPairInTest.LendingInfoList.Add(lendingInfo);
                }
                lendingInfoIndex++;
            }
            if (totalNumberOfResults > 0)
            {
                searchResults.Add(lendingPairInTest);
            }
        }
    }

    private static void PerformBorrowerNameSearchOnInfoPairs(SearchCriteriaSO searchCriteria, List<LendingInfoPairsSO.LendingPair> searchResults)
    {
        List<LendingInfoPairsSO.LendingPair> allLentBooks = LibraryManager.Instance.GetLendingInfoPairs().LendingPairs;

        string searchTerm = searchCriteria.SearchTerm.ToLower();

        // Check if the search term is empty, if so, return an empty result list
        if (string.IsNullOrEmpty(searchTerm))
        {
            return;
        }

        foreach (LendingInfoPairsSO.LendingPair lendingPair in allLentBooks)
        {
            int totalNumberOfResults = 0;
            // Create a new LendingPairInTest to store the matched lending info
            LendingInfoPairsSO.LendingPair lendingPairInTest = new LendingInfoPairsSO.LendingPair();
            lendingPairInTest.Book = lendingPair.Book;

            foreach (LendingInfo lendingInfo in lendingPair.LendingInfoList)
            {
                // Check if the borrower name contains the search term in a case-insensitive manner
                if (ContainsSearchTerm(lendingInfo.BorrowerName, searchTerm))
                {
                    totalNumberOfResults++;
                    lendingPairInTest.LendingInfoList.Add(lendingInfo);
                }
            }

            // If there are matching lending infos, add the lending pair to the search results
            if (totalNumberOfResults > 0)
            {
                searchResults.Add(lendingPairInTest);
            }
        }

    }

    private static void PerformGeneralSearchOnBookList(SearchCriteriaSO searchCriteria, List<BookData> searchResults)
    {
        LibraryDataSO libraryData = LibraryManager.Instance.GetLibraryData();
        List<BookData> allBooks = libraryData.Books;

        string searchTerm = searchCriteria.SearchTerm.ToLower();

        // Check if the search term is empty, if so, return an empty result list
        if (string.IsNullOrEmpty(searchTerm))
        {
            return;
        }

        foreach (BookData book in allBooks)
        {
            if (ContainsSearchTermInBookData(book, searchTerm))
            {
                searchResults.Add(book);
            }
        }
    }

    private static void PerformBookTitleSearchOnBookList(SearchCriteriaSO searchCriteria, List<BookData> searchResults)
    {
        PerformSearchBy(searchCriteria, searchResults, book => book.BookTitle);
    }

    private static void PerformBookTitleSearchOnInfoPairs(SearchCriteriaSO searchCriteria, List<LendingInfoPairsSO.LendingPair> searchResults)
    {
        PerformSearchBy(searchCriteria, searchResults, lendingPair => lendingPair.Book.BookTitle);
    }
    private static void PerformIsbnSearchOnBookList(SearchCriteriaSO searchCriteria, List<BookData> searchResults)
    {
        PerformSearchBy(searchCriteria, searchResults, book => book.BookIsbn);
    }

    private static void PerformAuthorSearchOnBookList(SearchCriteriaSO searchCriteria, List<BookData> searchResults)
    {
        PerformSearchBy(searchCriteria, searchResults, book => book.BookAuthor);
    }

    private static void PerformAuthorSearchOnInfoPairs(SearchCriteriaSO searchCriteria, List<LendingInfoPairsSO.LendingPair> searchResults)
    {
        PerformSearchBy(searchCriteria, searchResults, lendingPair => lendingPair.Book.BookAuthor);
    }

    private static void PerformSearchBy<T>(SearchCriteriaSO searchCriteria, List<T> searchResults, Func<T, string> getProperty)
    {
        // Retrieve the list of items to search from the appropriate source based on the type T
        IEnumerable<T> itemsToSearch = typeof(T) == typeof(BookData) ?
        LibraryManager.Instance.GetLibraryData().Books.Cast<T>() :
        LibraryManager.Instance.GetLendingInfoPairs().LendingPairs.Cast<T>();

        string searchTerm = searchCriteria.SearchTerm.ToLower();

        // Check if the search term is empty, if so, return an empty result list
        if (string.IsNullOrEmpty(searchTerm))
        {
            return;
        }

        foreach (T item in itemsToSearch)
        {
            // Get the property value based on the provided property getter function
            string propertyValue = getProperty(item).ToLower();

            // Check if the property value contains the search term in a case-insensitive manner
            if (ContainsSearchTerm(propertyValue, searchTerm))
            {
                // If the property value matches, add the item to the search results
                searchResults.Add(item);
            }
        }
    }
    private static bool ContainsSearchTermInBookData(BookData book, string searchTerm)
    {
        // Check if the search term is contained in the title, author, or ISBN
        return ContainsSearchTerm(book.BookTitle, searchTerm)
            || ContainsSearchTerm(book.BookAuthor, searchTerm)
            || ContainsSearchTerm(book.BookIsbn, searchTerm);
    }

    private static bool ContainsSearchTermInLendingPairListIndex(LendingInfoPairsSO.LendingPair lendingPair, int lendingInfoIndex, string searchTerm)
    {
        // Check if the search term is contained in the title, author, or ISBN
        return ContainsSearchTerm(lendingPair.Book.BookTitle, searchTerm)
            || ContainsSearchTerm(lendingPair.Book.BookAuthor, searchTerm)
            || ContainsSearchTerm(lendingPair.LendingInfoList[lendingInfoIndex].BorrowerName, searchTerm);
    }
    private static bool ContainsSearchTerm(string source, string searchTerm)
    {
        // Check if the source string contains the search term in a case-insensitive manner
        return source.ToLower().Contains(searchTerm);
    }

    // Method to return the bookData if it exists in the library
    public static BookData FindBookIfItExistsInTheLibrary(string bookTitle, string authorName, string isbn, bool checkDifferentIsbn)
    {
        LibraryDataSO libraryData = LibraryManager.Instance.GetLibraryData();

        if (libraryData != null && libraryData.Books != null)
        {
            return libraryData.Books.FirstOrDefault(existingBook =>
                existingBook.BookTitle == bookTitle &&
                existingBook.BookAuthor == authorName &&
                (checkDifferentIsbn ? !existingBook.BookIsbn.Equals(isbn) : existingBook.BookIsbn.Equals(isbn)));
        }

        return null;
    }

}
