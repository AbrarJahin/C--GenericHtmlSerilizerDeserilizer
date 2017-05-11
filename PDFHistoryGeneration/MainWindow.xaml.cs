using Microsoft.Win32;
using PDFHistoryGeneration.RsSerializer;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PDFHistoryGeneration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<SingleDataRow> items = new List<SingleDataRow>();
        HtmlDataList<SingleDataRow> serializer = new HtmlDataList<SingleDataRow>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "PDF files (*.pdf)|*.pdf| txt files (*.txt)|*.txt| DOC files (*.doc)|*.doc";
            fileDialog.FilterIndex = 1;
            fileDialog.Multiselect = false;

            if (fileDialog.ShowDialog() == true)
            {
                //string sFileName = fileDialog.FileName;
                //string[] arrAllFiles = fileDialog.FileNames; //used when Multiselect = true
                chosenFileLocationTxtbox.Text = fileDialog.FileName;

                items.Add(new SingleDataRow() { Name = "Data 1", Detail = "Detail 1" });
                items.Add(new SingleDataRow() { Name = "Data 2", Detail = "Detail 2" });

                serializer.SerializeData(items);
            }
        }

        private void generateHTML_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Serilizing Data
                string savedLocation = serializer.SaveData(items, generateFileLocationTxtbox.Text, chosenFileLocationTxtbox.Text);
                System.Diagnostics.Process.Start(savedLocation);

                //Deserilizing Data
                IList<SingleDataRow> deSerilizedData = new List<SingleDataRow>();
                deSerilizedData = serializer.LoadData(generateFileLocationTxtbox.Text);

                System.Diagnostics.Process.Start(generateFileLocationTxtbox.Text + ".html");
                System.Diagnostics.Process.Start(generateFileLocationTxtbox.Text + ".pdf");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
