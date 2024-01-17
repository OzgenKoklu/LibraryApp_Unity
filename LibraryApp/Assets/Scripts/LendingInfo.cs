using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

    public struct LendingInfo
    {
    public FixedString512Bytes bookIsbn;
    public FixedString512Bytes borrowerName;
    public DateTime bookBorrowDate;
    public DateTime expectedReturnDate;
    }
