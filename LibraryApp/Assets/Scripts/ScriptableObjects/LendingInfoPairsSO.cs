using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LendingInfoPairs", menuName = "Library/LendingInfoPairsSO", order = 1)]
public class LendingInfoPairsSO : ScriptableObject
{
    [System.Serializable]
    public class LendingPair
    {
        public BookData book;
        public int totalLendedBookCount;
        public List<LendingInfo> lendingInfoList = new List<LendingInfo>();
    }

    [SerializeField] public List<LendingPair> lendingPairs = new List<LendingPair>();
}