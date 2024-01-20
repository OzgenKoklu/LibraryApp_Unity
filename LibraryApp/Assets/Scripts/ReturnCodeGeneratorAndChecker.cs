using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReturnCodeGeneratorAndChecker 
{
    private const int ReturnCodeLength = 5;

    // Function to generate a random 5-digit return code
    public static string GenerateReturnCode()
    {
        string returnCode = "";
        for (int i = 0; i < ReturnCodeLength; i++)
        {
            returnCode += UnityEngine.Random.Range(0, 10).ToString();
        }
        return returnCode;
    }

    // Function to check if a return code is unused
    public static bool IsReturnCodeUnused(string returnCode, List<LendingInfoPairsSO.LendingPair> lendingPairs)
    {
        foreach (var lendingPair in lendingPairs)
        {
            foreach (var lendingInfo in lendingPair.lendingInfoList)
            {
                if (lendingInfo.returnCode == returnCode)
                {
                    return false; // Return code is already used
                }
            }
        }
        return true; // Return code is unused
    }
}
