using System.Collections.Generic;

namespace PDFHistoryGeneration.RsSerializer
{
    interface IDataList<T>
    {
        /*
         * Create HTML/JSON/XML from list of data
         */
        string SerializeData(List<T> items);

        /*
         * Load data into internal list
         */
        IList<T> DeserializeData(string html);    //SHould be this List<T>  as return type

        /*
         * Load Data from local drive
         */
        IList<T> LoadData(string fileLocation);

        /*
         * Save Data in local drive
         */
        string SaveData(List<T> items, string storeFileName, string pdfFileLocation);
    }
}
