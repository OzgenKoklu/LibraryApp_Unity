using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SearchCriteria", menuName = "Library/Search Criteria", order = 1)]
public class SearchCriteriaSO : ScriptableObject
{
    public SearchManager.SearchTypeGeneralListing searchTypeGeneral;
    public SearchManager.SearchTypeLentListing searchTypeLentList;
    public string searchTerm;
    public bool isAvailable;
    public bool isExpiredLent;
    // Add other relevant search parameters as needed Like Book availability, parsing options etc. 
}
