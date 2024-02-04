using UnityEngine;
using UnityEngine.UI;


public class MainPanelUI : MonoBehaviour
{
    [SerializeField] private Button _addOrRemoveBookButton;
    [SerializeField] private Button _listAndSearchBooksButton;
    [SerializeField] private Button _lendingOperationsButton;
    [SerializeField] private Button _dataOperationsButton;
    [SerializeField] private Button _aboutProjectButton;

    private void Awake()
    {
        _addOrRemoveBookButton.onClick.AddListener(ShowAddOrRemoveBookPanel);
        _listAndSearchBooksButton.onClick.AddListener(ShowAllBooksList);
        _lendingOperationsButton.onClick.AddListener(ShowLendingOperationsPanel);
        _dataOperationsButton.onClick.AddListener(ShowDataOperationsPanel);
        _aboutProjectButton.onClick.AddListener(ShowAboutInfo);
    }

    private void ShowAddOrRemoveBookPanel()
    {
        AddOrRemoveBookPanelUI.Instance.Show();
    }

    private void ShowAllBooksList()
    {
        ListPanelUI.Instance.Show(ListPanelUI.ListType.AllBooksList);
    }

    private void ShowLendingOperationsPanel()
    {
        LendingOperationsPanelUI.Instance.Show();
    }

    private void ShowDataOperationsPanel()
    {
        DataOperationsPanelUI.Instance.Show();
    }

    private void ShowAboutInfo()
    {
        PopupPanelUI.Instance.ShowAboutInfo();
    }
}
