using System.Collections.Generic;
using System.Linq;

public static class ReturnCodeGeneratorAndChecker 
{
    private const int ReturnCodeLength = 5;

    // Function to generate a random 5-digit return code
    public static string GenerateReturnCode()
    {
        List<LendingInfoPairsSO.LendingPair> lendingPairs = LibraryManager.Instance.GetLendingInfoPairs().lendingPairs;
        string returnCode = "";
        do
        {
            for (int i = 0; i < ReturnCodeLength; i++)
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
        return !lendingPairs.Any(pair => pair.lendingInfoList.Any(info => info.returnCode == returnCode));
    }
    
    //returns the lendingPair that has the matching return code. FirstOrDefault returns the first element that matches the condition. (theres only one anyway)
    public static LendingInfoPairsSO.LendingPair SearchForReturnCodeValidity(string returnCode)
    {
        LendingInfoPairsSO lendingInfoPairs = LibraryManager.Instance.GetLendingInfoPairs();

        return lendingInfoPairs?.lendingPairs
        .FirstOrDefault(pair => pair.lendingInfoList.Any(info => info.returnCode == returnCode));
    }
}
