using eBayForm.LogicUnits;
using eBayForm.Windows;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace eBayForm
{
    /* Set Font from Code
       myTextBlock.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Poppins"); */
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LogicController lc;
        public MainWindow()
        {
            InitializeComponent();
            Taskbar.Content = new DesignItems.Taskbar(this);
            lc = new LogicController();
        }

        #region MenuEvents
        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnTemplates_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnGetFromWeb_Click(object sender, RoutedEventArgs e)
        {
            //GetFromWeb getFromWebDialog = new GetFromWeb();
            //getFromWebDialog.ShowDialog();
            //string url = "";
            //if (getFromWebDialog.DialogResult == true)
            //{
            //    url = getFromWebDialog.Link.Text;
            //    getFromWebDialog.Close();
            //    string htmlCode = await DownloadPageAsync(url);
            //}
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            string htmlCode = lc.ImportHtml();
            if (htmlCode == null)
            {
                MessageBox.Show("Coudn't open file");
            }
            else
            {
                wbWorkspace.NavigateToString(htmlCode);
                wbWorkspace.Visibility = Visibility.Visible;
                List<HtmlTagElement> htmlTags = lc.GetTags();
                PropertiesToolBox toolBox = new PropertiesToolBox(lc);
                toolBox.Show();
            }
        }
        #endregion

        private async Task DisplayToolBox(string htmlCode)
        {
            
        }

        //private async Task<string> DownloadPageAsync(string url)
        //{
        //    //WebClient webClient = new WebClient();
        //    //string htmlCode = await webClient.DownloadStringTaskAsync(url);

        //    WebRequest request = WebRequest.Create(
        //      url);
        //    // If required by the server, set the credentials.  
        //    request.Credentials = CredentialCache.DefaultCredentials;

        //    // Get the response.  
        //    WebResponse response = request.GetResponse();
        //    // Display the status.  
        //    Console.WriteLine(((HttpWebResponse)response).StatusDescription);

        //    // Get the stream containing content returned by the server. 
        //    // The using block ensures the stream is automatically closed. 
        //    using (Stream dataStream = response.GetResponseStream())
        //    {
        //        // Open the stream using a StreamReader for easy access.  
        //        StreamReader reader = new StreamReader(dataStream);
        //        // Read the content.  
        //        string htmlCode = reader.ReadToEnd();
        //        // Display the content.  
        //        HtmlDocument document = new HtmlDocument();
        //        document.LoadHtml(htmlCode);
        //        foreach (var item in document.DocumentNode.SelectNodes("//div[@id='desc_div']"))
        //        {
        //            var testing = item.InnerHtml;
        //        }
        //    }

        //    // Close the response.  
        //    response.Close();

            
        //    //var Pannel = document.DocumentNode.SelectSingleNode(".//div[@id='BottomPanel']");

        //    //string tmp = Pannel.SelectSingleNode("//body").InnerHtml;
        //    return url;
        //}
    }
}
