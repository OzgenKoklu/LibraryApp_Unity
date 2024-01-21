using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReturnLentBookPanelUI : MonoBehaviour
{
    public static ReturnLentBookPanelUI Instance { get; private set; }

    public event EventHandler<EventArgs> OnInvalidReturnCodeEntered;

    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_InputField returnCodeInputField;
    [SerializeField] private Button confirmButton;

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);
        Hide();
    }

    private void OnConfirmButtonClick(string returnCode)
    {
        if (returnCode.Length == 5 && int.TryParse(returnCode, out _))
        {
            LibraryManager.Instance.TryReturnLentBookByReturnCode(returnCode);
        }
        else
        {
           OnInvalidReturnCodeEntered?.Invoke(this, new EventArgs());
        }
    }

    public void Show()
    {   
        gameObject.SetActive(true);
        returnCodeInputField.text = "";

        //Holding a reference to the Input field so that lambda expression can work
        TMP_InputField returnCode = returnCodeInputField;
        confirmButton.onClick.AddListener(() => OnConfirmButtonClick(returnCode.text)); ;
    }

    private void Hide()
    {
        confirmButton.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }
}
