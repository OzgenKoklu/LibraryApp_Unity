using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

public static class ImportExportManager
{
    public class CombinedData
    {
        public const string filePath = "Assets/Resources/CombinedData.json";

        //JSONUTILITY WONT SERIALIZE SCRIPTABLE OBJECTS DIRECTLY. SO WE NEED EXTRA SERIALIZATION/DESERIALIZATION METHODS FOR THEM
        //we need to serialize bookdata, lendingInfo,LendingPairs individually to write them onto the json

        [Serializable]
        public class SerializableBookData
        {
            public string bookTitle;
            public string bookAuthor;
            public string bookIsbn;
            public int bookCount;
        }

        [Serializable]
        public class SerializableLendingInfo
        {
            public string borrowerName;
            public string returnCode;
            public long expectedReturnDateTicks;
        }

        [Serializable]
        public class SerializableLendingPair
        {
            public SerializableBookData book;
            public int totalLendedBookCount;
            public List<SerializableLendingInfo> lendingInfoList = new List<SerializableLendingInfo>();
        }

        [Serializable]
        public class SerializableLibraryData
        {
            public List<SerializableBookData> books = new List<SerializableBookData>();
            // Add other fields from LibraryDataSO that you want to include
        }

        [Serializable]
        public class SerializableLendingInfoPairs
        {
            public List<SerializableLendingPair> lendingPairs = new List<SerializableLendingPair>();
            // Add other fields from LendingInfoPairsSO that you want to include
        }

        public SerializableLibraryData libraryData;
        public SerializableLendingInfoPairs lendingInfoPairs;

        // Convert actual instances to serializable data
        public void ConvertFromActualData(LibraryDataSO libraryData, LendingInfoPairsSO lendingInfoPairs)
        {
            this.libraryData = new SerializableLibraryData
            {
                books = libraryData.books.Select(book => new SerializableBookData
                {
                    bookTitle = book.bookTitle,
                    bookAuthor = book.bookAuthor,
                    bookIsbn = book.bookIsbn,
                    bookCount = book.bookCount
                }).ToList(),
                // Add other data conversion logic
            };

            this.lendingInfoPairs = new SerializableLendingInfoPairs
            {
                lendingPairs = lendingInfoPairs.lendingPairs.Select(pair => new SerializableLendingPair
                {
                    book = new SerializableBookData
                    {
                        bookTitle = pair.book.bookTitle,
                        bookAuthor = pair.book.bookAuthor,
                        bookIsbn = pair.book.bookIsbn,
                        bookCount = pair.book.bookCount
                    },
                    totalLendedBookCount = pair.totalLendedBookCount,
                    lendingInfoList = pair.lendingInfoList.Select(info => new SerializableLendingInfo
                    {
                        borrowerName = info.borrowerName,
                        returnCode = info.returnCode,
                        expectedReturnDateTicks = info.expectedReturnDateTicks
                    }).ToList()
                }).ToList(),
                // Add other data conversion logic
            };
        }

        public string SaveToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        public static CombinedData LoadFromJson(string json)
        {
            return JsonUtility.FromJson<CombinedData>(json);
        }
    }


    public static void ExportToJson()
    {
        try
        {
            ImportExportManager.CombinedData combinedData = new ImportExportManager.CombinedData();
            combinedData.ConvertFromActualData(
                LibraryManager.Instance.GetLibraryData(),
                LibraryManager.Instance.GetLendingInfoPairs()
            );

            // Save to JSON using the SaveToJson method
            string json = combinedData.SaveToJson();
            File.WriteAllText(ImportExportManager.CombinedData.filePath, json);

            string popupResonseMessage = "Json Successfully exported.";
            PopupPanelUI.Instance.ShowResponse(popupResonseMessage);
        } catch (Exception ex)
        {
            Debug.LogError("An error occurred while exporting to Json: " + ex.Message);
            string errorResponse = "An error occurred while exporting to Json. Please try again.";
            PopupPanelUI.Instance.ShowError(errorResponse);
        }
    }

    public static void ImportFromJson()
    {
        try
        { // Read from JSON using the LoadFromJson method
            string json = File.ReadAllText(ImportExportManager.CombinedData.filePath);
            ImportExportManager.CombinedData combinedData = ImportExportManager.CombinedData.LoadFromJson(json);


            //Deserializing and making scriptable object for library data
            LibraryDataSO libraryData = ScriptableObject.CreateInstance<LibraryDataSO>();
            foreach (CombinedData.SerializableBookData serializableBookData in combinedData.libraryData.books)
            {
                BookData bookData = new BookData
                {
                    bookTitle = serializableBookData.bookTitle,
                    bookAuthor = serializableBookData.bookAuthor,
                    bookIsbn = serializableBookData.bookIsbn,
                    bookCount = serializableBookData.bookCount
                };
                libraryData.books.Add(bookData);
            }

            //Deserializing and making scriptable object for lendingInfoPairs
            LendingInfoPairsSO lendingInfoPairs = ScriptableObject.CreateInstance<LendingInfoPairsSO>();
            foreach (ImportExportManager.CombinedData.SerializableLendingPair serializableLendingPair in combinedData.lendingInfoPairs.lendingPairs)
            {
                LendingInfoPairsSO.LendingPair lendingPair = new LendingInfoPairsSO.LendingPair
                {
                    book = new BookData
                    {
                        bookTitle = serializableLendingPair.book.bookTitle,
                        bookAuthor = serializableLendingPair.book.bookAuthor,
                        bookIsbn = serializableLendingPair.book.bookIsbn,
                        bookCount = serializableLendingPair.book.bookCount
                    },
                    totalLendedBookCount = serializableLendingPair.totalLendedBookCount
                };
                lendingPair.lendingInfoList.Clear();
                foreach (ImportExportManager.CombinedData.SerializableLendingInfo serializableLendingInfo in serializableLendingPair.lendingInfoList)
                {
                    LendingInfo lendingInfo = new LendingInfo
                    {
                        borrowerName = serializableLendingInfo.borrowerName,
                        returnCode = serializableLendingInfo.returnCode,
                        expectedReturnDateTicks = serializableLendingInfo.expectedReturnDateTicks
                    };
                    lendingPair.lendingInfoList.Add(lendingInfo);
                }
                lendingInfoPairs.lendingPairs.Add(lendingPair);
            }
            //sending them to libraryManager to set the library data accordingly and save it

            LibraryManager.Instance.UpdateLibraryDataFromJsonData(libraryData, lendingInfoPairs);

            string popupResonseMessage = "Json Successfully Imported.";
            PopupPanelUI.Instance.ShowResponse(popupResonseMessage);
        }
        catch (Exception ex)
        {
            Debug.LogError("An error occurred while importing from Json: " + ex.Message);
            string errorResponse = "An error occurred while importing from Json. Please try again.";
            PopupPanelUI.Instance.ShowError(errorResponse);
        }
    }




}
