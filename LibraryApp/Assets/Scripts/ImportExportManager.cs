using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;


public static class ImportExportManager
{
    public class CombinedData
    {
        public const string FilePathForBackup = "Assets/Resources/CombinedDataBackup.json";
        public const string FilePathForRuntime = "Assets/Resources/CombinedData.json";

        //JSONUTILITY WONT SERIALIZE SCRIPTABLE OBJECTS DIRECTLY. SO WE NEED EXTRA SERIALIZATION/DESERIALIZATION METHODS FOR THEM
        //we need to serialize bookdata, lendingInfo,LendingPairs individually to write them onto the json

        [Serializable]
        public class SerializableBookData
        {
            public string BookTitle;
            public string BookAuthor;
            public string BookIsbn;
            public int BookCount;
        }

        [Serializable]
        public class SerializableLendingInfo
        {
            public string BorrowerName;
            public string ReturnCode;
            public long ExpectedReturnDateTicks;
        }

        [Serializable]
        public class SerializableLendingPair
        {
            public SerializableBookData Book;
            //public int totalLendedBookCount;
            public List<SerializableLendingInfo> LendingInfoList = new List<SerializableLendingInfo>();
        }

        [Serializable]
        public class SerializableLibraryData
        {
            public List<SerializableBookData> Books = new List<SerializableBookData>();
            // Add other fields from LibraryDataSO that you want to include
        }

        [Serializable]
        public class SerializableLendingInfoPairs
        {
            public List<SerializableLendingPair> LendingPairs = new List<SerializableLendingPair>();
            // Add other fields from LendingInfoPairsSO that you want to include
        }

        public SerializableLibraryData LibraryData;
        public SerializableLendingInfoPairs LendingInfoPairs;

        // Convert actual instances to serializable data
        public void ConvertFromActualData(LibraryDataSO libraryData, LendingInfoPairsSO lendingInfoPairs)
        {
            this.LibraryData = new SerializableLibraryData
            {
                Books = libraryData.Books.Select(book => new SerializableBookData
                {
                    BookTitle = book.BookTitle,
                    BookAuthor = book.BookAuthor,
                    BookIsbn = book.BookIsbn,
                    BookCount = book.BookCount
                }).ToList(),
                // Add other data conversion logic
            };

            this.LendingInfoPairs = new SerializableLendingInfoPairs
            {
                LendingPairs = lendingInfoPairs.LendingPairs.Select(pair => new SerializableLendingPair
                {
                    Book = new SerializableBookData
                    {
                        BookTitle = pair.Book.BookTitle,
                        BookAuthor = pair.Book.BookAuthor,
                        BookIsbn = pair.Book.BookIsbn,
                        BookCount = pair.Book.BookCount
                    },
                   // totalLendedBookCount = pair.TotalLentBookCount,
                    LendingInfoList = pair.LendingInfoList.Select(info => new SerializableLendingInfo
                    {
                        BorrowerName = info.BorrowerName,
                        ReturnCode = info.ReturnCode,
                        ExpectedReturnDateTicks = info.ExpectedReturnDateTicks
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

    public static void ExportToJsonForBackup()
    {
        ExportToJson(ImportExportManager.CombinedData.FilePathForBackup);

        string popupResonseMessage = "Json Successfully exported.";

        HandleResponse(popupResonseMessage);
    }

    public static void ExportToJsonForRuntime()
    {
        //this is the version we will use for every single new update that occurs. LibraryManager.cs hase SaveData method for that.
        ExportToJson(ImportExportManager.CombinedData.FilePathForRuntime);
    }

        public static void ExportToJson(string filePath)
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
            File.WriteAllText(filePath, json);

 
        } catch (Exception ex)
        {
            Debug.LogError("An error occurred while exporting to Json: " + ex.Message);
            string errorResponse = "An error occurred while exporting to Json. Please try again.";
            HandleError(errorResponse);
        }
    }

    public static void HandleResponse(string responseMessage)
    {
        UiManager.ShowResponse(responseMessage);  
    }

    public static void HandleError(string errorMessage)
    {
        UiManager.ShowError(errorMessage);
    }

    public static void ImportFromJsonForBackup()
    {
        ImportFromJson(ImportExportManager.CombinedData.FilePathForBackup);

        string popupResonseMessage = "Json Successfully Imported.";
   
        HandleResponse(popupResonseMessage);
    }

    public static void ImportFromJsonForRuntime()
    {
        ImportFromJson(ImportExportManager.CombinedData.FilePathForRuntime);
    }

    public static void ImportFromJson(string filePath)
    {
        try
        { // Read from JSON using the LoadFromJson method
            string json = File.ReadAllText(filePath);
            ImportExportManager.CombinedData combinedData = ImportExportManager.CombinedData.LoadFromJson(json);


            //Deserializing and making scriptable object for library data
            LibraryDataSO libraryData = ScriptableObject.CreateInstance<LibraryDataSO>();
            foreach (CombinedData.SerializableBookData serializableBookData in combinedData.LibraryData.Books)
            {
                BookData bookData = new BookData
                {
                    BookTitle = serializableBookData.BookTitle,
                    BookAuthor = serializableBookData.BookAuthor,
                    BookIsbn = serializableBookData.BookIsbn,
                    BookCount = serializableBookData.BookCount
                };
                libraryData.Books.Add(bookData);
            }

            //Deserializing and making scriptable object for lendingInfoPairs
            LendingInfoPairsSO lendingInfoPairs = ScriptableObject.CreateInstance<LendingInfoPairsSO>();
            foreach (ImportExportManager.CombinedData.SerializableLendingPair serializableLendingPair in combinedData.LendingInfoPairs.LendingPairs)
            {
                LendingInfoPairsSO.LendingPair lendingPair = new LendingInfoPairsSO.LendingPair
                {
                    Book = new BookData
                    {
                        BookTitle = serializableLendingPair.Book.BookTitle,
                        BookAuthor = serializableLendingPair.Book.BookAuthor,
                        BookIsbn = serializableLendingPair.Book.BookIsbn,
                        BookCount = serializableLendingPair.Book.BookCount
                    },
                   // TotalLentBookCount = serializableLendingPair.totalLendedBookCount
                };
                lendingPair.LendingInfoList.Clear();
                foreach (ImportExportManager.CombinedData.SerializableLendingInfo serializableLendingInfo in serializableLendingPair.LendingInfoList)
                {
                    LendingInfo lendingInfo = new LendingInfo
                    {
                        BorrowerName = serializableLendingInfo.BorrowerName,
                        ReturnCode = serializableLendingInfo.ReturnCode,
                        ExpectedReturnDateTicks = serializableLendingInfo.ExpectedReturnDateTicks
                    };
                    lendingPair.LendingInfoList.Add(lendingInfo);
                }
                lendingInfoPairs.LendingPairs.Add(lendingPair);
            }
            //sending them to libraryManager to set the library data accordingly and save it

            LibraryManager.Instance.UpdateLibraryDataFromJsonData(libraryData, lendingInfoPairs);

        }
        catch (Exception ex)
        {
            Debug.LogError("An error occurred while importing from Json: " + ex.Message);
            string errorResponse = "An error occurred while importing from Json. Please try again.";
            HandleError(errorResponse);
        }
    }
}
