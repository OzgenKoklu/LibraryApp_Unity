using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ListPanelUI;

public class PopupPanelUI : MonoBehaviour
{   
    public enum PopupType
    {
        ShowResponse,
        ShowError,
        ShowPrompt,     
    }
    public static PopupPanelUI Instance { get; private set; }

    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private TMP_InputField popupPanelInputField;
    
    //actionButton's text will change depending on the context. It can write OK or Confirm
    [SerializeField] private Button actionButton; 
    [SerializeField] private TextMeshProUGUI actionButtonText;


    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);
        Hide();
    }


    public void ShowPrompt(string promptMessage)
    {
        Show(PopupType.ShowPrompt);
    }


        public void ShowResponse(string responseMessage)
    {
        Show(PopupType.ShowResponse);
        //title text might change
        titleText.text = "Success!!!";
        mainText.text = responseMessage;
    }

    public void ShowError(string responseMessage)
    {
        Show(PopupType.ShowError);
        //title text might change
        titleText.text = "Error!!!";
        mainText.text = responseMessage;
    }

    public void ShowAboutInfo()
    {
        Show(PopupType.ShowResponse);
        titleText.text = "About";
        mainText.text = "Made by Özgen Köklü";
        actionButtonText.text = "Return";
    }

    public void Show(PopupType popupType)
    {
        gameObject.SetActive(true);

        switch (popupType)
        {
            case PopupType.ShowPrompt:
                popupPanelInputField.gameObject.SetActive(true);
                actionButton.gameObject.SetActive(true);

                break;
            case PopupType.ShowError:
                popupPanelInputField.gameObject.SetActive(false);
                actionButton.gameObject.SetActive(false);

                break;
            case PopupType.ShowResponse:
                popupPanelInputField.gameObject.SetActive(false);
                actionButton.gameObject.SetActive(true);

                break;

        }
    }

    public void Hide() 
    {
        gameObject.SetActive(false);
    }
}
