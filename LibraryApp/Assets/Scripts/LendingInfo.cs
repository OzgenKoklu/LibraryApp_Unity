using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[System.Serializable]
public class LendingInfo
    {
    public string borrowerName;
    public string returnCode;

    //normally I held Date's in DateTime but I think it causes issues from serilization/deserialization processes
    //public DateTime bookBorrowDate;
    //public DateTime expectedReturnDate;
    public long expectedReturnDateTicks;
}
