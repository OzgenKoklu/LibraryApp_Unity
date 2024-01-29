using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LendingOperationsPanelUI : MonoBehaviour
{
    public static LendingOperationsPanelUI Instance { get; private set; }

    [SerializeField] private Button closeButton;
    [SerializeField] private Button lendBookButton;
    [SerializeField] private Button returnLentBookButton;
    [SerializeField] private Button listAllLentBooksButton;

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);

        lendBookButton.onClick.AddListener(() =>
        {
            ListPanelUI.Instance.Show(ListPanelUI.ListType.LendABookList);
        });

        returnLentBookButton.onClick.AddListener(() =>
        {
            PopupPanelUI.Instance.ShowBookReturningReturnCodePrompt();
        });

        listAllLentBooksButton.onClick.AddListener(() =>
        {
            ListPanelUI.Instance.Show(ListPanelUI.ListType.AllLentBooksList);
        });

        Hide();
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
