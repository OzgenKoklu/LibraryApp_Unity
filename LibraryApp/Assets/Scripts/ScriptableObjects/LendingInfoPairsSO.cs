using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LendingInfoPairs", menuName = "Library/LendingInfoPairsSO", order = 1)]
public class LendingInfoPairsSO : ScriptableObject
{
    [System.Serializable]
    public class LendingPair
    {
        public BookData Book;
        //public int TotalLentBookCount;
        public List<LendingInfo> LendingInfoList = new List<LendingInfo>();
    }

    [SerializeField] public List<LendingPair> LendingPairs = new List<LendingPair>();
}