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
    //Kitap Sýnýfý: Baþlýk, Yazar, ISBN, Kopya Sayýsý ve Ödünç Alýnan Kopyalar gibi niteliklere sahip bir Kitap sýnýfý oluþturun.
}

