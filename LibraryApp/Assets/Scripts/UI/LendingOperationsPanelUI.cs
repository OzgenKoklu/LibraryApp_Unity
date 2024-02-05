using UnityEngine;
using UnityEngine.UI;

public class LendingOperationsPanelUI : MonoBehaviour
{
    public static LendingOperationsPanelUI Instance { get; private set; }

    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _lendBookButton;
    [SerializeField] private Button _returnLentBookButton;
    [SerializeField] private Button _listAllLentBooksButton;

    private void Awake()
    {
        Instance = this;
        _closeButton.onClick.AddListener(() =>
        {
            PlayMouseClickSoundOnWindow();
            Hide();
        });

        _lendBookButton.onClick.AddListener(ShowLendABookList);

        _returnLentBookButton.onClick.AddListener(ShowBookReturningReturnCodePrompt);

        _listAllLentBooksButton.onClick.AddListener(ShowAllLentBooksList);

        Hide();
    }

    private void ShowLendABookList()
    {
        ListPanelUI.Instance.Show(ListPanelUI.ListType.LendABookList);
    }

    private void ShowBookReturningReturnCodePrompt()
    {
        PopupPanelUI.Instance.ShowBookReturningReturnCodePrompt();
    }

    private void ShowAllLentBooksList()
    {
        ListPanelUI.Instance.Show(ListPanelUI.ListType.AllLentBooksList);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        PlayMouseClickSoundOnWindow();
    }

    private void PlayMouseClickSoundOnWindow()
    {
        SoundManager.Instance.PlayMouseClick();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
