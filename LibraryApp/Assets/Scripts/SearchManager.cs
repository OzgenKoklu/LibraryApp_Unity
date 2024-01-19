using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public static class SearchManager
{

    public enum SearchType
    {
        General,
        ByTitle,
        ByIsbn,
        ByAuthor,
    }



    public static List<BookData> PerformSearch(SearchCriteria searchCriteria)
    {
        List<BookData> searchResults = new List<BookData>();
        // Will implement the search logic here
        switch (searchCriteria.searchType)
        {
            case SearchType.General:
                PerformSearchGeneral(searchCriteria, searchResults);
                break;

            case SearchType.ByTitle:
                PerformSearchByTitle(searchCriteria, searchResults);
                break;

            case SearchType.ByIsbn:
                PerformSearchByISBN(searchCriteria, searchResults);
                break;

            case SearchType.ByAuthor:
                PerformSearchByAuthor(searchCriteria, searchResults);
                break;

            default:
                Debug.LogError("Unsupported search type");
                break;
        }

        // Return the list of search results
        return searchResults;

        // You can now use searchCriteria.searchTerm and searchCriteria.searchType to perform the search
    }

    //THIS IS TOO MUCH REPETITON, WILL IMPLEMENT PROPERTY SELECTOR FUNCTION/GENERIC METHOD!
    private static void PerformSearchGeneral(SearchCriteria searchCriteria, List<BookData> searchResults)
    {
        LibraryData libraryData = LibraryManager.Instance.GetLibraryData();
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
    private static bool ContainsSearchTerm(BookData book, string searchTerm)
    {
        // Check if the search term is contained in the title, author, or ISBN
        return ContainsIgnoreCase(book.bookTitle, searchTerm)
            || ContainsIgnoreCase(book.bookAuthor, searchTerm)
            || ContainsIgnoreCase(book.bookIsbn, searchTerm);
    }

    private static bool ContainsIgnoreCase(string source, string searchTerm)
    {
        // Check if the source string contains the search term in a case-insensitive manner
        return source.ToLower().Contains(searchTerm);
    }

    private static void PerformSearchByTitle(SearchCriteria searchCriteria, List<BookData> searchResults)
    {
        LibraryData libraryData = LibraryManager.Instance.GetLibraryData();
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

    private static void PerformSearchByISBN(SearchCriteria searchCriteria, List<BookData> searchResults)
    {
        LibraryData libraryData = LibraryManager.Instance.GetLibraryData();
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

    private static void PerformSearchByAuthor(SearchCriteria searchCriteria, List<BookData> searchResults)
    {
        LibraryData libraryData = LibraryManager.Instance.GetLibraryData();
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


}
