using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static void ShowList(ListPanelUI.ListType listType)
    {
        ListPanelUI.Instance.Show(listType);
    }
    public static void ShowResponse(string responseMessage)
    {
        PopupPanelUI.Instance.ShowResponse(responseMessage);

    }

    public static void ShowError(string errorMessage)
    {
        //I tried doing this with delegates but when popup window is set deactive, it does not listen to these so I think this approach is better in the end 
        //Will look for my options.
        PopupPanelUI.Instance.ShowError(errorMessage);
    }


}
