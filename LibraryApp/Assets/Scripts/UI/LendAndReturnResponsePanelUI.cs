using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class LendAndReturnResponsePanelUI : MonoBehaviour
{
    public static LendAndReturnResponsePanelUI Instance { get; private set; }

    public delegate void ConfirmationCallback();
    public static ConfirmationCallback OnConfirmReturn;

    [SerializeField] private TextMeshProUGUI responseText;

    [SerializeField] private Button confirmButton;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        Instance = this;
        //closeButton.onClick.AddListener(Hide);
        confirmButton.onClick.AddListener(Hide);
        Hide();
    }

    private void SetResponseText(string responseMessage)
    {
        responseText.text = responseMessage;
    }

    //This show is the version that works with a callback, this is for where confirmation is needed
    public void Show(string responseMessage, LendAndReturnResponsePanelUI.ConfirmationCallback callback)
    {
        gameObject.SetActive(true);
        SetResponseText(responseMessage);

        confirmButton.onClick.AddListener(OnConfirmButtonClickedForApproval); 

        closeButton.gameObject.SetActive(true);

        OnConfirmReturn = callback;
    }

    private void OnConfirmButtonClickedForApproval()
    {
        // Trigger the callback when the confirm button is clicked
        OnConfirmReturn?.Invoke();

        //removes listeners for later usage of the panel
        confirmButton.onClick.RemoveListener(OnConfirmButtonClickedForApproval);
    }

    public void Show(string responseMessage)
    {

        // Remove existing listeners
        // confirmButton.onClick.RemoveAllListeners();

        // Subscribe to the Hide method
        gameObject.SetActive(true);
        SetResponseText(responseMessage);
        //No need for a close button if the task of the window is just to notify the user, only confirm button is active
        closeButton.gameObject.SetActive(false);

    }



    private void Hide()
    {
        SetResponseText("");
        gameObject.SetActive(false);
    }

}