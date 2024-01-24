using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookLendingPanelUI : MonoBehaviour
{
    public static BookLendingPanelUI Instance { get; private set; }

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
            LendBookPanelUI.Instance.Show();
        });

        returnLentBookButton.onClick.AddListener(() =>
        {
            ReturnLentBookPanelUI.Instance.Show();
        });

        listAllLentBooksButton.onClick.AddListener(() =>
        {
            ListAllLentBooksPanelUI.Instance.Show();
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
