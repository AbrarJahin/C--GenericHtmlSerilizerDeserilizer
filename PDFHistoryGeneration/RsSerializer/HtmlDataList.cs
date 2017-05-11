using EO.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PDFHistoryGeneration.RsSerializer
{
    class HtmlDataList<T> : IDataList<T> where T: new()
    {
        // Html Configurations
        string htmlTitle = "<!DOCTYPE html><html><head><title>Generated PDF</title>";
        string htmlStyle = "<style>table, th, td{border: 1px solid black;}</style>";
        string htmlHeader = "</head><body>";
        string htmlFooter = "</body></html>";

        /*
         * Create HTML/JSON/XML from list of data
         */
        public string SerializeData(List<T> items)
        {
            var properties = typeof(T).GetProperties();
            StringBuilder generatedHtmlTable = new StringBuilder();
            generatedHtmlTable.Append("<table> ");
            //Table Header
            generatedHtmlTable.Append("<tr>");
            foreach (var prop in properties)
            {
                generatedHtmlTable.Append("<th> "+prop.Name+ " </th>");
            }
            generatedHtmlTable.Append("</tr> ");

            //Table Contents
            foreach (var item in items)
            {
                generatedHtmlTable.Append("<tr> ");
                foreach (var prop in properties)
                {
                    generatedHtmlTable.Append("<td> "+ prop.GetValue(item) + " </td>");
                }
                Console.WriteLine();
                generatedHtmlTable.Append(" </tr>");
            }
            generatedHtmlTable.Append(" </table>");

            Console.Write(generatedHtmlTable.ToString());

            return htmlTitle + htmlStyle + htmlHeader + generatedHtmlTable.ToString() + htmlFooter;
            //return generatedHtmlTable.ToString();
        }

        /*
         * Load data into internal list
         */
        public IList<T> DeserializeData(string html)
        {
            IList<T> nodeData = new List<T>();

            List<string> columnNames = new List<string>();
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(html);
            IEnumerable<HtmlAgilityPack.HtmlNode> tableRows = htmlDoc.DocumentNode.Descendants("tr");

            bool ifNotName = false;
            foreach (HtmlAgilityPack.HtmlNode input in tableRows)
            {
                int elementNo = 0;
                T instance = new T();

                foreach (HtmlAgilityPack.HtmlNode node in input.ChildNodes)
                {
                    if (node.Name.Equals("th"))
                    {
                        columnNames.Add(node.InnerHtml.Trim());
                    }
                    else if (node.Name.Equals("td"))
                    {
                        try
                        {
                            typeof(T).GetProperty(columnNames[elementNo]).SetValue(instance, node.InnerHtml, null);
                        }
                        catch(Exception ex)
                        {
                            //throw ex;
                            Console.WriteLine(ex.ToString());
                        }
                        elementNo++;
                    }
                }
                if(ifNotName)            //First element is not data, so we don't need to insert it
                {
                    nodeData.Add(instance);
                }
                ifNotName = true;
            }
            return nodeData;
        }

        /*
         * Load Data from local drive
         */
        public IList<T> LoadData(string fileLocation)
        {
            try
            {
                string html = File.ReadAllText(fileLocation);
                return DeserializeData(html);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*
         * Save Data in local drive
         */
        public string SaveData(List<T> items, string storeFileName, string pdfFileLocation)
        {
            try
            {
                string html = SerializeData(items);
                //Store HTML
                File.WriteAllText(storeFileName+".html", html);
                //Create PDF from HTML
                HtmlToPdf.ConvertUrl(storeFileName + ".html", storeFileName + ".pdf");

                //Merge with showed PDF
                PdfDocument doc1 = new PdfDocument(pdfFileLocation);
                PdfDocument doc2 = new PdfDocument(storeFileName + ".pdf");
                PdfDocument mergedDoc = PdfDocument.Merge(doc1, doc2);
                mergedDoc.Save(storeFileName+"_tot.pdf");
                return storeFileName + "_tot.pdf";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
