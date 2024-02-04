using UnityEngine;

public class ScrollViewListBoundsLimiter : MonoBehaviour
{
    [SerializeField] private Transform _containerGameObject;
    [SerializeField] private RectTransform _containerRect;

    private float _anchoredPositionLimitForListTop = 0;
    private int _activeChildObjectsInContainer = 0;
    private int _previousActiveChildObjects = -1; // Initialize to a value that won't match the initial state
    private float _anchoredPositionLimitForListBottom = 0f;

    private void Update()
    {
        if (_containerGameObject != null)
        {
            CorrectContentListRektIfOutOfBounds();
        }    
    }

    private void CorrectContentListRektIfOutOfBounds()
    {
        if (_containerRect.anchoredPosition.y < _anchoredPositionLimitForListTop)
        {
            Vector2 currentAnchoredPosition = _containerRect.anchoredPosition;
            currentAnchoredPosition.y = _anchoredPositionLimitForListTop;
            _containerRect.anchoredPosition = currentAnchoredPosition;
        }

        _activeChildObjectsInContainer = GetNumberOfActiveChildObjects(_containerGameObject);
        // Checks if the number of active child objects has changed
        if (_activeChildObjectsInContainer != _previousActiveChildObjects)
        {
            _anchoredPositionLimitForListBottom = GetAnchoredPositionLimitForListBottom(_activeChildObjectsInContainer);
            _previousActiveChildObjects = _activeChildObjectsInContainer;
        }

        if (_containerRect.anchoredPosition.y > _anchoredPositionLimitForListBottom)
        {
            Vector2 currentAnchoredPosition = _containerRect.anchoredPosition;
            currentAnchoredPosition.y = _anchoredPositionLimitForListBottom;
            _containerRect.anchoredPosition = currentAnchoredPosition;
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
