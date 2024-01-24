using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ValidInputChecker 
{
    public static bool IsBookNameValid(string inputText)
    {
        //checks if the input is empty
        return !string.IsNullOrEmpty(inputText);
    }
    public static bool IsBookAuthorValid(string inputText)
    {
        //checks if the input is empty
        return !string.IsNullOrEmpty(inputText);
    }

    public static bool IsBookAlreadyListed(string bookTitle, string authorName, string isbn)
    {
        LibraryDataSO libraryData = LibraryManager.Instance.GetLibraryData();

        // Check if any book in the library has the same author and title but a different ISBN
        bool bookAlreadyListed = libraryData.books.Any(existingBook =>
            existingBook.bookTitle.Equals(bookTitle) &&
            existingBook.bookAuthor.Equals(authorName) &&
            existingBook.bookIsbn.Equals(isbn));

        return bookAlreadyListed;
    }

    //Checks if AuthorName + BookTitle combo is listed with a different ISBN.
    public static bool IsThisBookListedAsADifferentEntry(string bookTitle, string authorName, string isbn)
    {
        LibraryDataSO libraryData = LibraryManager.Instance.GetLibraryData();

        // Check if any book in the library has the same author and title but a different ISBN
        bool similarEntryExists = libraryData.books.Any(existingBook =>
            existingBook.bookTitle.Equals(bookTitle) &&
            existingBook.bookAuthor.Equals(authorName) &&
            !existingBook.bookIsbn.Equals(isbn));

        return similarEntryExists;
    }

    //This code uses ISBN as book identifiers so getting a valid and unique ISBN is important
    //LibraryManager's many validation processes depend on bookData.bookIsbn
    public static bool IsBookIsbnValid(string isbn)
    {
        // Clean the ISBN by removing hyphens
        string cleanedIsbn = isbn.Replace("-", "");

        // Check if the cleaned ISBN is a valid 13-digit number
        if (string.IsNullOrEmpty(cleanedIsbn) || cleanedIsbn.Length != 13 || !long.TryParse(cleanedIsbn, out _))
        {
            // If the ISBN is not a valid 13-digit number, return false
            return false;
        }

        // Retrieve the library data
        LibraryDataSO libraryData = LibraryManager.Instance.GetLibraryData();

        // Check if the ISBN already exists in the library
        if (libraryData.books.Any(book => book.bookIsbn == cleanedIsbn))
        {
            // If the ISBN already exists, return false
            return false;
        }

        // If the ISBN is valid and not found in the library, return true
        return true;
    }





}
