using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SearchCriteria", menuName = "Library/Search Criteria", order = 1)]
public class SearchCriteria : ScriptableObject
{
    public SearchManager.SearchType searchType;
    public string searchTerm;
    // Add other relevant search parameters as needed Like Book availability, parsing options etc. 
}
