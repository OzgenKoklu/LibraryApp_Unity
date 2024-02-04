using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScollViewListBoundsLimiter : MonoBehaviour
{
    [SerializeField] private Transform containerGameObject;
    [SerializeField] private RectTransform containerRect;

    private float anchoredPositionLimitForListTop = 0;
    private int activeChildObjectsInContainer = 0;
    private int previousActiveChildObjects = -1; // Initialize to a value that won't match the initial state
    float anchoredPositionLimitForListBottom = 0f;

    private void Awake()
    {
        
    }

    private void Start()
    {
        LibraryManager.Instance.OnLibraryDataUpdatedForLists += LibraryManager_OnLibraryDataUpdatedForLists;
    }

    private void LibraryManager_OnLibraryDataUpdatedForLists(object sender, System.EventArgs e)
    {
        
    }

    private void Update()
    {
        if (containerGameObject != null)
        {
            CorrectContentListRektIfOutOfBounds();
        }    
    }

    private void CorrectContentListRektIfOutOfBounds()
    {
        if (containerRect.anchoredPosition.y < anchoredPositionLimitForListTop)
        {
            Vector2 currentAnchoredPosition = containerRect.anchoredPosition;
            currentAnchoredPosition.y = anchoredPositionLimitForListTop;
            containerRect.anchoredPosition = currentAnchoredPosition;
        }

        activeChildObjectsInContainer = GetNumberOfActiveChildObjects(containerGameObject);
        // Checks if the number of active child objects has changed
        if (activeChildObjectsInContainer != previousActiveChildObjects)
        {
            anchoredPositionLimitForListBottom = GetAnchoredPositionLimitForListBottom(activeChildObjectsInContainer);
            previousActiveChildObjects = activeChildObjectsInContainer;
        }

        if (containerRect.anchoredPosition.y > anchoredPositionLimitForListBottom)
        {
            Vector2 currentAnchoredPosition = containerRect.anchoredPosition;
            currentAnchoredPosition.y = anchoredPositionLimitForListBottom;
            containerRect.anchoredPosition = currentAnchoredPosition;
        }
    }
    private float GetAnchoredPositionLimitForListBottom(int childObjectCount)
    {
        //These numbers are measured during runtime. 56 is pixel count needed when 1920x1080 reference pixel size for UI canvas is selected. Might need a change in case of a canvas update
        int maxNumberOfObjectsThatFitInOnePage = 11;
        float anchoredPositionLimitForListBottom = 0;
        float listingVerticalSize = 56;

        if (childObjectCount < maxNumberOfObjectsThatFitInOnePage)
        {
            return anchoredPositionLimitForListBottom;
        }
        else
        {
            int numberOfChildObjectsThatExceedsTheCapacity = childObjectCount - maxNumberOfObjectsThatFitInOnePage;
            return listingVerticalSize * numberOfChildObjectsThatExceedsTheCapacity;
        }
    }
    int GetNumberOfActiveChildObjects(Transform parentObject)
    {
        int activeChildObjects = 0;
        foreach (Transform child in parentObject)
        {
            if (child.gameObject.activeSelf)
            {
                activeChildObjects++;
            }
        }
        return activeChildObjects;
    }

}
