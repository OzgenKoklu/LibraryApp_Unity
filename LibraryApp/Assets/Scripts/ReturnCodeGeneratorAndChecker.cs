using System.Collections.Generic;
using System.Linq;

public static class ReturnCodeGeneratorAndChecker 
{
    private const int _returnCodeLength = 5;

    // Function to generate a random 5-digit return code
    public static string GenerateReturnCode()
    {
        LendingInfoPairsSO lendingInfoPairs = LibraryManager.Instance.GetLendingInfoPairs();

        if (lendingInfoPairs != null)
        {
            List<LendingInfoPairsSO.LendingPair> lendingPairs = lendingInfoPairs.LendingPairs;
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
        else
        {
            //Debug.LogError("LendingInfoPairs is null. Unable to generate return code.");
            // Error Handing will Pop up the Popup window but not through events or delegates specific to this class, maybe will write additional UI manager script to have gemeric  events
            return ""; // Returning an empty string as a fallback value
        }
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

        if (lendingInfoPairs != null)
        {
            return lendingInfoPairs?.LendingPairs
        .FirstOrDefault(pair => pair.LendingInfoList.Any(info => info.ReturnCode == returnCode));
        }
        else
        {
            // Debug.LogError("LendingInfoPairs is null. Unable to search for return code validity.");
            // Error Handing will Pop up the Popup window but not through events or delegates specific to this class, maybe will write additional UI manager script to have gemeric  events
            return null;
        }
    }
}
