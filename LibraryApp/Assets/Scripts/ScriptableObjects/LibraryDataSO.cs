using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LibraryData", menuName = "Library/Create Library Data SO", order = 1)]
public class LibraryDataSO : ScriptableObject
{
    //list of book data. 
    public List<BookData> Books = new List<BookData>();
}