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
    public DateTime bookBorrowDate;
    public DateTime expectedReturnDate;
    }
