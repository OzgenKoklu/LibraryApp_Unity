using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static LendingInfoPairsSO;

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

    public static List<BookData> GetAvailableBooks(List<BookData> bookDataList)
    {
        List<BookData> availableBooks = new List<BookData>();
        
        foreach (BookData book in bookDataList)
        {
            if (book.bookCount > 0)
            {
                availableBooks.Add(book);
            }
        }
        return availableBooks;
    }

    public static List<LendingInfoPairsSO.LendingPair> GetExpiredBooks()
    {
        List<LendingInfoPairsSO.LendingPair> expiredBookLents = new List<LendingInfoPairsSO.LendingPair>();

        List<LendingInfoPairsSO.LendingPair> allLentBooks = LibraryManager.Instance.GetLendingInfoPairs().lendingPairs;

        foreach (LendingInfoPairsSO.LendingPair lendingPair in allLentBooks)
        {
            int totalNumberOfOverDue = 0;
            LendingInfoPairsSO.LendingPair lendingPairInTest = new LendingInfoPairsSO.LendingPair();
            lendingPairInTest.book = lendingPair.book;

            foreach (LendingInfo lendingInfo in lendingPair.lendingInfoList)
            {
                // Get the expected return date as a DateTime for the lending Info
                DateTime expectedReturnDate = new DateTime(lendingInfo.expectedReturnDateTicks);
                
                // Compare the expected return date with the current time to check if its exceeded
                if (expectedReturnDate < DateTime.Now)
                {
                    totalNumberOfOverDue++;
                    lendingPairInTest.lendingInfoList.Add(lendingInfo);  
                }
            }
            if (totalNumberOfOverDue > 0)
            {
                expiredBookLents.Add(lendingPairInTest);
            }
        }
        return expiredBookLents;
    }

    public static List<LendingInfoPairsSO.LendingPair> PerformLendingInfoPairsSearch(SearchCriteriaSO searchCriteria)
    {
        List<LendingInfoPairsSO.LendingPair> searchResults = new List<LendingInfoPairsSO.LendingPair>();

        switch (searchCriteria.searchTypeLentList)
        {
            case SearchTypeLentListing.General:
                PerformSearchGeneral(searchCriteria, searchResults);
                break;

            case SearchTypeLentListing.ByTitle:
                PerformSearchByTitle(searchCriteria, searchResults);
                break;

            case SearchTypeLentListing.ByBorrower:
                PerformSearchByBorrowerName(searchCriteria, searchResults);
                break;

            case SearchTypeLentListing.ByAuthor:
                PerformSearchByAuthor(searchCriteria, searchResults);
                break;

            default:
                Debug.LogError("Unsupported search type");
                break;
        }

  

        return searchResults;
    }


    public static List<BookData> PerformBookListingSearch(SearchCriteriaSO searchCriteria)
    {

        List<BookData> searchResults = new List<BookData>();
       
        switch (searchCriteria.searchTypeGeneral)
        {
            case SearchTypeGeneralListing.General:
                PerformSearchGeneral(searchCriteria, searchResults);
                break;

            case SearchTypeGeneralListing.ByTitle:
                PerformSearchByTitle(searchCriteria, searchResults);
                break;

            case SearchTypeGeneralListing.ByIsbn:
                 PerformSearchByISBN(searchCriteria, searchResults);
                break;

            case SearchTypeGeneralListing.ByAuthor:
               PerformSearchByAuthor(searchCriteria, searchResults);
                break;

            default:
                Debug.LogError("Unsupported search type");
                break;
        }

        if (searchCriteria.isAvailable)
        {
            GetAvailableBooks(searchResults);
            
        }

        // Return the list of search results
        return searchResults;

        // You can now use searchCriteria.searchTerm and searchCriteria.searchType to perform the search
    }

    //THIS IS TOO MUCH REPETITON, WILL IMPLEMENT PROPERTY SELECTOR FUNCTION/GENERIC METHOD!

    private static void PerformSearchByBorrowerName(SearchCriteriaSO searchCriteria, List<LendingInfoPairsSO.LendingPair> searchResults)
    {
        List<LendingInfoPairsSO.LendingPair> allLentBooks = LibraryManager.Instance.GetLendingInfoPairs().lendingPairs;

        string searchTerm = searchCriteria.searchTerm.ToLower();

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
            lendingPairInTest.book = lendingPair.book;

            foreach (LendingInfo lendingInfo in lendingPair.lendingInfoList)
            {
                // Check if the borrower name contains the search term in a case-insensitive manner
                if (ContainsIgnoreCase(lendingInfo.borrowerName, searchTerm))
                {
                    totalNumberOfResults++;
                    lendingPairInTest.lendingInfoList.Add(lendingInfo);
                }
            }

            // If there are matching lending infos, add the lending pair to the search results
            if (totalNumberOfResults > 0)
            {
                searchResults.Add(lendingPairInTest);
            }
        }

    }

    private static void PerformSearchGeneral(SearchCriteriaSO searchCriteria, List<BookData> searchResults)
    {
        LibraryDataSO libraryData = LibraryManager.Instance.GetLibraryData();
        List<BookData> allBooks = libraryData.books;

        string searchTerm = searchCriteria.searchTerm.ToLower();

        // Check if the search term is empty, if so, return an empty result list
        if (string.IsNullOrEmpty(searchTerm))
        {
            return;
        }

        foreach (BookData book in allBooks)
        {
            if (ContainsSearchTerm(book, searchTerm))
            {
                searchResults.Add(book);
            }
        }
    }

    private static void PerformSearchGeneral(SearchCriteriaSO searchCriteria, List<LendingInfoPairsSO.LendingPair> searchResults)
    {
        List<LendingInfoPairsSO.LendingPair> allLentBooks = LibraryManager.Instance.GetLendingInfoPairs().lendingPairs;

        string searchTerm = searchCriteria.searchTerm.ToLower();

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
            lendingPairInTest.book = lendingPair.book;
         
            foreach (LendingInfo lendingInfo in lendingPair.lendingInfoList)
            { 
          
                if (ContainsSearchTerm(lendingPair, lendingInfoIndex, searchTerm))
                {
                    totalNumberOfResults++;
                    lendingPairInTest.lendingInfoList.Add(lendingInfo);
                }
                lendingInfoIndex++;
            }
            if (totalNumberOfResults > 0)
            {
                searchResults.Add(lendingPairInTest);
            }
        }
    }
    private static bool ContainsSearchTerm(BookData book, string searchTerm)
    {
        // Check if the search term is contained in the title, author, or ISBN
        return ContainsIgnoreCase(book.bookTitle, searchTerm)
            || ContainsIgnoreCase(book.bookAuthor, searchTerm)
            || ContainsIgnoreCase(book.bookIsbn, searchTerm);
    }

    private static bool ContainsSearchTerm(LendingInfoPairsSO.LendingPair lendingPair, int lendingInfoIndex, string searchTerm)
    {
        // Check if the search term is contained in the title, author, or ISBN
        return ContainsIgnoreCase(lendingPair.book.bookTitle, searchTerm)
            || ContainsIgnoreCase(lendingPair.book.bookAuthor, searchTerm)
            || ContainsIgnoreCase(lendingPair.lendingInfoList[lendingInfoIndex].borrowerName, searchTerm);
    }

    private static bool ContainsIgnoreCase(string source, string searchTerm)
    {
        // Check if the source string contains the search term in a case-insensitive manner
        return source.ToLower().Contains(searchTerm);
    }

    private static void PerformSearchByTitle(SearchCriteriaSO searchCriteria, List<BookData> searchResults)
    {
        LibraryDataSO libraryData = LibraryManager.Instance.GetLibraryData();
        List<BookData> allBooks = libraryData.books;

        string searchTerm = searchCriteria.searchTerm.ToLower();

        // Check if the search term is empty, if so, return an empty result list
        if (string.IsNullOrEmpty(searchTerm))
        {
            return;
        }

        foreach (BookData book in allBooks)
        {
            if (ContainsIgnoreCase(book.bookTitle, searchTerm))
            {
                searchResults.Add(book);
            }
        }
    }

    private static void PerformSearchByTitle(SearchCriteriaSO searchCriteria, List<LendingInfoPairsSO.LendingPair> searchResults)
    {
        List<LendingInfoPairsSO.LendingPair> allLentBooks = LibraryManager.Instance.GetLendingInfoPairs().lendingPairs;

        string searchTerm = searchCriteria.searchTerm.ToLower();

        // Check if the search term is empty, if so, return an empty result list
        if (string.IsNullOrEmpty(searchTerm))
        {
            return;
        }

        foreach (LendingInfoPairsSO.LendingPair lendingPair in allLentBooks)
        {
            // Check if the book title contains the search term in a case-insensitive manner
            if (ContainsIgnoreCase(lendingPair.book.bookTitle, searchTerm))
            {
                // If the book title matches, add the entire lending pair to the search results
                searchResults.Add(lendingPair);
            }
        }
    }

    //This one does not have an ISBN search, actiually it would be easy to add since LendingInfoPair.book already has ISBN. I dont think the function is needed for ease of use though
    private static void PerformSearchByISBN(SearchCriteriaSO searchCriteria, List<BookData> searchResults)
    {
        LibraryDataSO libraryData = LibraryManager.Instance.GetLibraryData();
        List<BookData> allBooks = libraryData.books;

        string searchTerm = searchCriteria.searchTerm.ToLower();

        // Check if the search term is empty, if so, return an empty result list
        if (string.IsNullOrEmpty(searchTerm))
        {
            return;
        }

        foreach (BookData book in allBooks)
        {
            if (ContainsIgnoreCase(book.bookIsbn, searchTerm))
            {
                searchResults.Add(book);
            }
        }
    }

    private static void PerformSearchByAuthor(SearchCriteriaSO searchCriteria, List<BookData> searchResults)
    {
        LibraryDataSO libraryData = LibraryManager.Instance.GetLibraryData();
        List<BookData> allBooks = libraryData.books;

        string searchTerm = searchCriteria.searchTerm.ToLower();

        // Check if the search term is empty, if so, return an empty result list
        if (string.IsNullOrEmpty(searchTerm))
        {
            return;
        }

        foreach (BookData book in allBooks)
        {
            if (ContainsIgnoreCase(book.bookAuthor, searchTerm))
            {
                searchResults.Add(book);
            }
        }
    }

    private static void PerformSearchByAuthor(SearchCriteriaSO searchCriteria, List<LendingInfoPairsSO.LendingPair> searchResults)
    {
        List<LendingInfoPairsSO.LendingPair> allLentBooks = LibraryManager.Instance.GetLendingInfoPairs().lendingPairs;

        string searchTerm = searchCriteria.searchTerm.ToLower();

        // Check if the search term is empty, if so, return an empty result list
        if (string.IsNullOrEmpty(searchTerm))
        {
            return;
        }

        foreach (LendingInfoPairsSO.LendingPair lendingPair in allLentBooks)
        {
            // Check if the book title contains the search term in a case-insensitive manner
            if (ContainsIgnoreCase(lendingPair.book.bookAuthor, searchTerm))
            {
                // If the book title matches, add the entire lending pair to the search results
                searchResults.Add(lendingPair);
            }
        }
    }


}
