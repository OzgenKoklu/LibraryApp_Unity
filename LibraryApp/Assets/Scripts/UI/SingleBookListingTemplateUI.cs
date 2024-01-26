using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class SingleBookListingTemplateUI : MonoBehaviour
{
    [Header("For general Listings")] //Some General purpose fields, author and count not needed for return panel
    [SerializeField] private TextMeshProUGUI bookTitleText;
    [SerializeField] private TextMeshProUGUI bookAuthorText;
    [SerializeField] private TextMeshProUGUI bookCountText;
    [SerializeField] private TextMeshProUGUI bookIsbnText;


    [Header("List for all lent books list type")] //Needed to be asigned for panels where returning lent book functionality is available
    [SerializeField] private TextMeshProUGUI borrowerNameText;
    [SerializeField] private TextMeshProUGUI dueDateText;

    private BookData bookDataOnSingleTemplate;

    private Color dueDatePassedColor = Color.red;
    private Color dueDateNotPassedColor = Color.green;


    public void SetBookDataForBasicListing(BookData bookData)
    {
        bookTitleText.text = bookData.bookTitle;
        bookAuthorText.text = bookData.bookAuthor;
        bookIsbnText.text = bookData.bookIsbn;
        bookCountText.text = bookData.bookCount.ToString();
    }

    public void SetBookDataForLending(BookData bookData)
    {
        bookTitleText.text = bookData.bookTitle;
        bookAuthorText.text = bookData.bookAuthor;
        bookCountText.text = bookData.bookCount.ToString();
       // lendButton.onClick.AddListener(() => OnLendButtonClick(bookData));
    }

    private void OnLendButtonClick(BookData bookData)
    {
       // LendABookBorrowerNamePromptPanelUI.Instance.Show(bookData);
    }

    //Index approach is there for avoiding sending 2 classes in this basic function. I didnt want to send lendingInfo as a seperate parameter.
    public void SetBookDataForReturningLentBook(LendingInfoPairsSO.LendingPair lendingPair, int lendingInfoListIndex)
    {
        LendingInfo lendingInfo = lendingPair.lendingInfoList[lendingInfoListIndex];
        bookTitleText.text = lendingPair.book.bookTitle;
        borrowerNameText.text = lendingInfo.borrowerName;

        DateTime expectedReturnDeserialized = new DateTime(lendingInfo.expectedReturnDateTicks);

        //change the color of the text according to due date passing or not passing 
        dueDateText.color = (expectedReturnDeserialized <= DateTime.Now) ? dueDatePassedColor : dueDateNotPassedColor;
        dueDateText.text = expectedReturnDeserialized.ToString("MM/dd/yyyy");

       // returnButton.onClick.AddListener(() => OnReturnButtonClick(lendingPair,lendingInfoListIndex));
    }

    private void OnReturnButtonClick(LendingInfoPairsSO.LendingPair lendingPair, int lendingInfoListIndex)
    {
        string returnConfirmationMessage = $"You are about to return {lendingPair.book.bookTitle} borrowed by {lendingPair.lendingInfoList[lendingInfoListIndex].borrowerName}. Press X to cancel or Confirm to proceed.";
      
        //passing in function as a delegate for the next UI pop Up to trigger

       /* LendAndReturnResponsePanelUI.Instance.Show(returnConfirmationMessage, () =>
        {
            LibraryManager.Instance.TryReturnLentBookFromTheList(lendingPair.lendingInfoList[lendingInfoListIndex]);
        } );
         */       
    }
}
