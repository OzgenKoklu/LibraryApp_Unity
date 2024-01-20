using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LibraryData", menuName = "Library/Create Library Data SO", order = 1)]
public class LibraryDataSO : ScriptableObject
{
    public List<BookData> books = new List<BookData>();
}