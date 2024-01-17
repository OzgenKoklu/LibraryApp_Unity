using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI errorMessageText;
    [SerializeField] private Button closeButton; 

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
        Hide();
    }
    private void Start()
    {
        LibraryManager.Instance.OnErrorEncountered += LibraryManager_OnErrorEncountered;
    }

    private void LibraryManager_OnErrorEncountered(object sender, LibraryManager.OnErrorEncounteredEventArgs e)
    {
        errorMessageText.text = e.errorMessage;
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
