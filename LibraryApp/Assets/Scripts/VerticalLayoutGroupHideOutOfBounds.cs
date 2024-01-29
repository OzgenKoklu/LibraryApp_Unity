using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VerticalLayoutGroupHideOutOfBounds : MonoBehaviour
{
    public RectTransform containerRect;
    public SingleBookListingTemplateUI singleBookListingTemplate;

    void Start()
    {
        CheckAndHideOutOfBounds();
    }

    void Update()
    {
        CheckAndHideOutOfBounds();
    }

    void CheckAndHideOutOfBounds()
    {
        foreach (Transform child in transform)
        {
           // Debug.Log(child.gameObject.transform.position);
            //if child is in the rekt transform and not the template, sets on the template. if not sets off
            if (IsInsideContainer(child.GetComponent<Transform>()))
            {
                child.gameObject.SetActive(true); // Show the child
            }
            else
            {
                child.gameObject.SetActive(false); // Hide the child
            }
        }
    }

    bool IsInsideContainer(Transform childTransform)
    {
        float containerTopBound = containerRect.rect.yMax;
        float containerBottomBound = containerRect.rect.yMin;
        float childPosition = childTransform.position.y;
        
        Debug.Log("container top bound : " + containerTopBound + "container bottom bound : " + containerBottomBound + "child pos : " + childPosition);
        return (containerTopBound > childPosition && containerBottomBound < childPosition);
    }
}
