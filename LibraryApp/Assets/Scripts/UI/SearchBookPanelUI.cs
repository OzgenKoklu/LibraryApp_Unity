using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class SearchBookPanelUI : MonoBehaviour
{
    public static SearchBookPanelUI Instance { get; private set; }

    [SerializeField] private Transform searchResultsContainer;
    [SerializeField] private Transform singleBookListingTemplate;

    [SerializeField] private TextMeshProUGUI searchResultsDetailsText;
    [SerializeField] private Button searchButton;
    [SerializeField] private TMP_Dropdown searchTypeDropdown;
    [SerializeField] private TMP_InputField searchTermInputField;

    [SerializeField] private Button closeButton;
    private SearchManager.SearchType selectedSearchType;

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);
        searchButton.onClick.AddListener(() =>
        {
            OnSearchButtonClicked();
        });
        singleBookListingTemplate.gameObject.SetActive(false);
        Hide();
    }

    private void OnSearchButtonClicked() {
        SearchCriteria searchCriteria = ScriptableObject.CreateInstance<SearchCriteria>();
        //Now we need to remove spaces we added just for the UI.
        searchCriteria.searchType = (SearchManager.SearchType)Enum.Parse(typeof(SearchManager.SearchType), searchTypeDropdown.options[searchTypeDropdown.value].text.Replace(" ", ""));
        searchCriteria.searchTerm = searchTermInputField.text;

        // Call the SearchManager to perform the search
        List<BookData> searchResults = SearchManager.PerformSearch(searchCriteria);
        // Now we have the search results, update & populate the container
        UpdateSearchResultList(searchResults);
    }

    private void UpdateSearchResultList(List<BookData> bookDataList)
    {
        foreach (Transform child in searchResultsContainer)
        {
            if (child == singleBookListingTemplate) continue;
            Destroy(child.gameObject);
        }

        int totalUniqueBookCount = 0;
        foreach (BookData result in bookDataList)
        {
            totalUniqueBookCount++;
            Transform bookListingTransform = Instantiate(singleBookListingTemplate, searchResultsContainer);
            bookListingTransform.gameObject.SetActive(true);
            bookListingTransform.GetComponent<SingleBookListingTemplateUI>().SetBookData(result);
        }
        searchResultsDetailsText.text = $"{totalUniqueBookCount} book(s) found.";

    }

    private void SetDropdownOptions()
    {
        // Assuming you have a list of string options
        string[] searchTypeOptions = System.Enum.GetNames(typeof(SearchManager.SearchType));

        // Modify enum names for better UI readability
        for (int i = 0; i < searchTypeOptions.Length; i++)
        {
            searchTypeOptions[i] = AddSpaceBeforeUpperCase(searchTypeOptions[i]);
        }

        // Clear existing options
        searchTypeDropdown.ClearOptions();

        // Add new options
        searchTypeDropdown.AddOptions(new List<string>(searchTypeOptions));
    }

    string AddSpaceBeforeUpperCase(string input)
    {
        // Add a space before each uppercase letter in the string
        System.Text.StringBuilder modifiedString = new System.Text.StringBuilder();
        foreach (char c in input)
        {
            if (char.IsUpper(c))
            {
                modifiedString.Append(' ');
            }
            modifiedString.Append(c);
        }
        return modifiedString.ToString();
    }

    private void ResetTextFieldsAndContainer()
    {
        searchResultsDetailsText.text = "";
        searchTermInputField.text = "";

        //cleans up the container
        foreach (Transform child in searchResultsContainer)
        {
            if (child == singleBookListingTemplate) continue;
            Destroy(child.gameObject);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        SetDropdownOptions();
    }

    private void Hide()
    {
        ResetTextFieldsAndContainer();
        gameObject.SetActive(false);
    }

}
