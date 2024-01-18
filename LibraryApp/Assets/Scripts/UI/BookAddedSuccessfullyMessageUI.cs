using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookAddedSuccessfullyMessageUI : MonoBehaviour
{   
    public static BookAddedSuccessfullyMessageUI Instance { get; private set; }

    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI bookAddedMessageText;

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);    
    }

    private void Start()
    {
        AddNewBookPanelUI.Instance.OnSuccessfulBookAddition += AddNewBookPanelUI_OnSuccessfulBookAddition;
        Hide();
    }

    private void AddNewBookPanelUI_OnSuccessfulBookAddition(object sender, AddNewBookPanelUI.OnSuccessfulBookAdditionEventArgs e)
    {
        Show();
        bookAddedMessageText.text = e.bookTitle + " by " + e.bookAuthor + " is added to the library successfully.";
    }

    public void Show()
    {
        gameObject.SetActive(true);

    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }


}
