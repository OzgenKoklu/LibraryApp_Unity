using System.Collections.Generic;
using System.Linq;

public static class ReturnCodeGeneratorAndChecker 
{
    private const int _returnCodeLength = 5;

    // Function to generate a random 5-digit return code
    public static string GenerateReturnCode()
    {
        List<LendingInfoPairsSO.LendingPair> lendingPairs = LibraryManager.Instance.GetLendingInfoPairs().LendingPairs;
        string returnCode = "";
        do
        {
            for (int i = 0; i < _returnCodeLength; i++)
            {
                returnCode += UnityEngine.Random.Range(0, 10).ToString();
            }
        } while (!IsReturnCodeUnused(returnCode, lendingPairs));
        return returnCode;
    }

// Function to check if a return code is unused,  method chaining/fluent syntax + lambda expression
//checks if any of the returncodes inside the lendingInfoList matches the returnCode we use as a parameter
public static bool IsReturnCodeUnused(string returnCode, List<LendingInfoPairsSO.LendingPair> lendingPairs)
    {
        return !lendingPairs.Any(pair => pair.LendingInfoList.Any(info => info.ReturnCode == returnCode));
    }
    
    //returns the lendingPair that has the matching return code. FirstOrDefault returns the first element that matches the condition. (theres only one anyway)
    public static LendingInfoPairsSO.LendingPair SearchForReturnCodeValidity(string returnCode)
    {
        LendingInfoPairsSO lendingInfoPairs = LibraryManager.Instance.GetLendingInfoPairs();

        return lendingInfoPairs?.LendingPairs
        .FirstOrDefault(pair => pair.LendingInfoList.Any(info => info.ReturnCode == returnCode));
    }
}
