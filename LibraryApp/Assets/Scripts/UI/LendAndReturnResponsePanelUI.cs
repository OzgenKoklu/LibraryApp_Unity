using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LendAndReturnResponsePanelUI : MonoBehaviour
{
    public static LendAndReturnResponsePanelUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI responseText;

    [SerializeField] private Button confirmButton;

    private void Awake()
    {
        Instance = this;
        confirmButton.onClick.AddListener(Hide);
        Hide();
    }

    private void SetResponseText(string responseMessage)
    {
        responseText.text = responseMessage;
    }

    public void Show(string responseMessage)
    {
        gameObject.SetActive(true);
        SetResponseText(responseMessage);
    }

    private void Hide()
    {
        SetResponseText("");
        gameObject.SetActive(false);
    }

}


