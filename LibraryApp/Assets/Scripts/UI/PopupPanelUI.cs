using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupPanelUI : MonoBehaviour
{
    public static PopupPanelUI Instance { get; private set; }

    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI mainText;


    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);
        Hide();
    }


    public void ShowResponse(string responseMessage)
    {
        Show();
        //title text might change
        titleText.text = "Success!!!";
        mainText.text = responseMessage;
    }

    public void ShowError(string responseMessage)
    {
        Show();
        //title text might change
        titleText.text = "Error!!!";
        mainText.text = responseMessage;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide() 
    {
        gameObject.SetActive(false);
    }
}
