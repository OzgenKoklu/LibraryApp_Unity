using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainPanelUI : MonoBehaviour
{
    [SerializeField] private Button addOrRemoveBookButton;
    [SerializeField] private Button listAndSearchBooksButton;
    [SerializeField] private Button lendingOperationsButton;
    [SerializeField] private Button dataOperationsButton;
    [SerializeField] private Button aboutProjectButton;


    private void Awake()
    {
        addOrRemoveBookButton.onClick.AddListener(() =>
        {
            AddOrRemoveBookPanelUI.Instance.Show();
        });

        listAndSearchBooksButton.onClick.AddListener(() =>
        {
            ListPanelUI.Instance.Show(ListPanelUI.ListType.AllBooksList);
        });

        lendingOperationsButton.onClick.AddListener(() =>
        {
          LendingOperationsPanelUI.Instance.Show();
        });

        dataOperationsButton.onClick.AddListener(() =>
        {
          DataOperationsPanelUI.Instance.Show();    
        });

        aboutProjectButton.onClick.AddListener(() =>
        {
            PopupPanelUI.Instance.ShowAboutInfo();
        });

    }

}
