using eBayForm.LogicUnits;
using eBayForm.LogicUnits.Exceptions;
using eBayForm.Windows;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace eBayForm
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PropertiesToolBox toolBox;
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
            tabControll.SelectedIndex = 0;
        }

        private void BtnTemplates_Click(object sender, RoutedEventArgs e)
        {
            tabControll.SelectedIndex = 1;
        }

        //private void BtnGetFromWeb_Click(object sender, RoutedEventArgs e)
        //{
        //    //GetFromWeb getFromWebDialog = new GetFromWeb();
        //    //getFromWebDialog.ShowDialog();
        //    //string url = "";
        //    //if (getFromWebDialog.DialogResult == true)
        //    //{
        //    //    url = getFromWebDialog.Link.Text;
        //    //    getFromWebDialog.Close();
        //    //    string htmlCode = await DownloadPageAsync(url);
        //    //}
        //}

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                // Only html files
                openFileDialog.Filter = "html files (*.html)|*.html";
                bool? result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    wbWorkspace.NavigateToString(lc.ImportHtml(openFileDialog.FileName));
                    wbWorkspace.Visibility = Visibility.Visible;
                    if (toolBox != null)
                    {
                        toolBox.Close();
                    }
                    toolBox = new PropertiesToolBox(lc, wbWorkspace);
                    toolBox.Show();
                    tabControll.SelectedIndex = 0;
                }
            }
            catch (NotATemplateException exception)
            {
                CustomMessageBox.Show(exception.Message);
            }
            catch (UnknownTemplateException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        #endregion

        #region Templates


        private void BtnTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (lc.Document != null)
            {
                bool? result = CustomMessageBox.Dialog("Do you want to export the file?");
                if (result == true)
                {
                    SaveFileDialog fileDialog = new SaveFileDialog();
                    fileDialog.Filter = "html files (*.html)|*.html";
                    bool? dialogResult = fileDialog.ShowDialog();

                    if (dialogResult == true)
                    {
                        lc.Export(fileDialog.FileName);
                    }
                }
                toolBox.Close();
            }

            string templateName = ((Button)sender).Name;

            SetupDialog setupDialog = new SetupDialog();
            setupDialog.ShowDialog();

            if (setupDialog.DialogResult == true)
            {

                Dictionary<string, string> configurationKeyValues = new Dictionary<string, string>();
                foreach (TextBox textBox in SetupDialog.FindVisualChildren<TextBox>(setupDialog))
                {
                    configurationKeyValues.Add(textBox.Name, textBox.Text);
                }

                string htmlCode = lc.LoadTemplate(templateName, configurationKeyValues);
                wbWorkspace.NavigateToString(htmlCode);
                wbWorkspace.Visibility = Visibility.Visible;
                if (toolBox != null)
                {
                    toolBox.Close();
                }
                toolBox = new PropertiesToolBox(lc, wbWorkspace);
                toolBox.Show();
                tabControll.SelectedIndex = 0;
            }
        }

        #endregion

        //private async Task DisplayToolBox(string htmlCode)
        //{

        //}

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

        #region OnClose

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (lc.Document != null)
            {
                bool? result = CustomMessageBox.Dialog("Do you want to export the file?");
                if (result == true)
                {
                    SaveFileDialog fileDialog = new SaveFileDialog();
                    fileDialog.Filter = "html files (*.html)|*.html";
                    bool? dialogResult = fileDialog.ShowDialog();

                    if (dialogResult == true)
                    {
                        lc.Export(fileDialog.FileName);
                    }
                }
                toolBox.Close();
            }

            base.OnClosing(e);
        }

        #endregion
    }
}
