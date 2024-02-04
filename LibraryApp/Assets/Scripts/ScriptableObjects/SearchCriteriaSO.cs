using UnityEngine;

[CreateAssetMenu(fileName = "SearchCriteria", menuName = "Library/Search Criteria", order = 1)]
public class SearchCriteriaSO : ScriptableObject
{
    // General search criteria for the main library list
    public SearchManager.SearchTypeGeneralListing SearchTypeGeneral;
    // Search criteria specific to the lent books list
    public SearchManager.SearchTypeLentListing SearchTypeLentList;

    // The main search string
    public string SearchTerm;

    //Search option to select if you want to be selective whether the book is available or not (at this moment)
    public bool IsAvailable;

    //search option for lent book search (if its expired or not)
    public bool IsExpiredLent;
   
}
