using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct BookData 
{
    public FixedString512Bytes bookTitle;
    public FixedString512Bytes bookAuthor;
    public FixedString512Bytes bookIsbn;
    public int bookCount;

    //Gereksinimler:
    //Kitap S�n�f�: Ba�l�k, Yazar, ISBN, Kopya Say�s� ve �d�n� Al�nan Kopyalar gibi niteliklere sahip bir Kitap s�n�f� olu�turun.
}

