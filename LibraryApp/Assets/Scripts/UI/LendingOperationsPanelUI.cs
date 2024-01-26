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
            //LendBookPanelUI.Instance.Show();
            //listPanelWithCustomParameters
        });

        returnLentBookButton.onClick.AddListener(() =>
        {
            //ReturnLentBookPanelUI.Instance.Show();
            //popUpPanelWithCustomParameters
        });

        listAllLentBooksButton.onClick.AddListener(() =>
        {
            //ListAllLentBooksPanelUI.Instance.Show();
            //List panel with custom parameters
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
