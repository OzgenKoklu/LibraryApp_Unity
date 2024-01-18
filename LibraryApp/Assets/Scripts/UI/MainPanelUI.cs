using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainPanelUI : MonoBehaviour
{
    [SerializeField] private Button addNewBookButton;
    [SerializeField] private Button listAllBooksButton;
    [SerializeField] private Button searchBookButton;
    [SerializeField] private Button bookLendingButton;
    [SerializeField] private Button aboutProjectButton;

    private void Awake()
    {
        addNewBookButton.onClick.AddListener(() =>
        {
            AddNewBookPanelUI.Instance.Show();
        });

        listAllBooksButton.onClick.AddListener(() =>
        {
            ListAllBooksPanelUI.Instance.Show();
        });

        searchBookButton.onClick.AddListener(() =>
        {
            SearchBookPanelUI.Instance.Show();
        });

        bookLendingButton.onClick.AddListener(() =>
        {
            BookLendingPanelUI.Instance.Show();
        });

        aboutProjectButton.onClick.AddListener(() =>
        {
            AboutProjectPanelUI.Instance.Show();
        });

    }



}
