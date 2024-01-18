using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LibraryData", menuName = "Library/Create Library Data", order = 1)]
public class LibraryData : ScriptableObject
{
    public List<BookData> books = new List<BookData>();
}