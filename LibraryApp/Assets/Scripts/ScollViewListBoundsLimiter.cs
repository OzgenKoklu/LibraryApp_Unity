using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScollViewListBoundsLimiter : MonoBehaviour
{
    [SerializeField] private Transform containerGameObject;
    [SerializeField] private RectTransform containerRect;

    private void Awake()
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
        float anchoredPositionLimitForListTop = 0;

        if (containerRect.anchoredPosition.y < anchoredPositionLimitForListTop)
        {
            Vector2 currentAnchoredPosition = containerRect.anchoredPosition;
            currentAnchoredPosition.y = anchoredPositionLimitForListTop;
            containerRect.anchoredPosition = currentAnchoredPosition;
        }

        //not logicat to do it every single update. Only logical to do once list updates. Can Also change Scrollbar size depending on child object count 

        int activeChildObjectsInContainer = GetNumberOfActiveChildObjects(containerGameObject);
        float anchoredPositionLimitForListBottom = GetAnchoredPositionLimitForListBottom(activeChildObjectsInContainer);

        if (containerRect.anchoredPosition.y > anchoredPositionLimitForListBottom)
        {
            Vector2 currentAnchoredPosition = containerRect.anchoredPosition;
            currentAnchoredPosition.y = anchoredPositionLimitForListBottom;
            containerRect.anchoredPosition = currentAnchoredPosition;
        }
    }
    private float GetAnchoredPositionLimitForListBottom(int childObjectCount)
    {
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
