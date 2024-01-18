using System.Collections;
using System.Collections.Generic;
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

    public static bool IsBookIsbnValid(string isbn)
    {
        // Discards hyphens from the ISBN string
        string cleanedIsbn = isbn.Replace("-", "");

        // Check if the cleaned ISBN is a valid 13-digit number
        return !string.IsNullOrEmpty(cleanedIsbn) &&
               cleanedIsbn.Length == 13 &&
               long.TryParse(cleanedIsbn, out _);
    }
}
