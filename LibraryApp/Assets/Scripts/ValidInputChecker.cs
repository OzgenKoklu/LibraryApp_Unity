using System.Linq;

public static class ValidInputChecker 
{
    public static bool IsBookNameValid(string inputText)
    {
        return IsInputEmptyOrNull(inputText);
        //Can add aditional logi if needed.
    }
    public static bool IsBookAuthorValid(string inputText)
    {
        return IsInputEmptyOrNull(inputText);
    }

    // Checks if the input text is valid (not null or empty)
    private static bool IsInputEmptyOrNull(string inputText)
    {
        return !string.IsNullOrEmpty(inputText);
    }

    // Checks if a book with the same title, author, and ISBN already exists
    public static bool IsBookAlreadyListed(string bookTitle, string authorName, string isbn)
    {
        return SearchManager.FindBookIfItExistsInTheLibrary(bookTitle, authorName, isbn, checkDifferentIsbn: false) != null;
    }

    // Checks if a book with the same title and author but a different ISBN exists
    public static bool IsThisBookListedAsADifferentEntry(string bookTitle, string authorName, string isbn)
    {
        return SearchManager.FindBookIfItExistsInTheLibrary(bookTitle, authorName, isbn, checkDifferentIsbn: true) != null;
    }

    //This code uses ISBN as book identifiers so getting a valid and unique ISBN is important
    //LibraryManager's many validation processes depend on bookData.bookIsbn
    public static bool IsBookIsbnValid(string isbn)
    {
        // Clean the ISBN by removing hyphens
        string cleanedIsbn = isbn.Replace("-", "");

        // Check if the cleaned ISBN is a valid 10 or 13-digit number
        if (string.IsNullOrEmpty(cleanedIsbn) || (cleanedIsbn.Length != 13 && cleanedIsbn.Length != 10) || !long.TryParse(cleanedIsbn, out _))
        {
            // If the ISBN is not a valid 13-digit number, return false
            return false;
        }

        // Retrieve the library data
        LibraryDataSO libraryData = LibraryManager.Instance.GetLibraryData();

        // Check if the ISBN already exists in the library
        if (libraryData.Books.Any(book => book.BookIsbn == cleanedIsbn))
        {
            // If the ISBN already exists, return false
            return false;
        }

        // If the ISBN is valid and not found in the library, return true
        return true;
    }

}
