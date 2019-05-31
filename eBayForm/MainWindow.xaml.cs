using eBayForm.LogicUnits;
using eBayForm.LogicUnits.Exceptions;
using eBayForm.Windows;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace eBayForm
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PropertiesToolBox toolBox;
        StylesToolBox stylesToolBox;
        LogicController lc;
        public static RoutedCommand cmdSaveChanges = new RoutedCommand();

        public MainWindow()
        {
            InitializeComponent();
            Taskbar.Content = new DesignItems.Taskbar(this);
            lc = new LogicController();
            wbWorkspace.Navigating += WbWorkspace_Navigating;
        }
        

        #region Workspace

        private void WbWorkspace_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.Uri != null)
            {
                (sender as WebBrowser).NavigateToString(lc.Document);
            }
        }

        public void ShowWorkspace(string htmlCode)
        {
            wbWorkspace.NavigateToString(htmlCode);
            wbWorkspace.Visibility = Visibility.Visible;
            spButtons.Visibility = Visibility.Visible;
            cmdSaveChanges.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            stylesToolBox = new StylesToolBox(lc, wbWorkspace, btnShowToolBox);
            stylesToolBox.Show();
            toolBox = new PropertiesToolBox(lc, wbWorkspace, btnShowToolBox);
            toolBox.Show();
            tabControll.SelectedIndex = 0;
        }

        private void SaveChanges()
        {
            if (lc.Document != null && toolBox != null && stylesToolBox != null)
            {
                List<HtmlTagElement> htmlTags = toolBox.SaveChanges();
                List<TagStyleElement> styleElements = stylesToolBox.SaveChanges();
                wbWorkspace.NavigateToString(lc.SaveChanges(htmlTags, styleElements));
            }
        }

        private void CbSaveChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveChanges();
        }

        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
        }

        public void ExportCheck()
        {
            if (lc.Document != null && toolBox != null && stylesToolBox != null)
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
                stylesToolBox.Close();
            }
        }

        private void BtnExportFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "html files (*.html)|*.html";
            bool? dialogResult = fileDialog.ShowDialog();

            if (dialogResult == true)
            {
                lc.Export(fileDialog.FileName);
            }
        }

        #endregion

        #region MenuEvents

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            tabControll.SelectedIndex = 0;
        }

        private void BtnTemplates_Click(object sender, RoutedEventArgs e)
        {
            tabControll.SelectedIndex = 1;
        }

        private async void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            ExportCheck();
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                // Only html files
                openFileDialog.Filter = "Html files (*.html)|*.html";
                bool? result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    string htmlCode = await lc.ImportHtmlAsync(openFileDialog.FileName);
                    ShowWorkspace(htmlCode);
                }
            }
            catch (TemplateException exception)
            {
                CustomMessageBox.Show(exception.Message);
            }
        }

        private void BtnShowToolBox_Click(object sender, RoutedEventArgs e)
        {
            toolBox.Show();
            ((Button)sender).Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Templates

        private void BtnTemplate_Click(object sender, RoutedEventArgs e)
        {
            ExportCheck();

            string templatename = ((Button)sender).Name;

            SetupDialog setupDialog = new SetupDialog();
            setupDialog.ShowDialog();

            if (setupDialog.DialogResult == true)
            {
                Dictionary<string, string> configurationKeyValues = new Dictionary<string, string>();
                foreach (TextBox textBox in ElementFinder.FindVisualChildren<TextBox>(setupDialog))
                {
                    configurationKeyValues.Add(textBox.Name, textBox.Text);
                }


                TemplateSetupDialog templateSetup = new TemplateSetupDialog(templatename);
                templateSetup.ShowDialog();

                if (templateSetup.DialogResult == true)
                {

                    foreach (TextBox textBox in ElementFinder.FindVisualChildren<TextBox>(templateSetup))
                    {
                        configurationKeyValues.Add(textBox.Name, textBox.Text);
                    }

                    string htmlCode = lc.LoadTemplate(templatename, configurationKeyValues);
                    ShowWorkspace(htmlCode);
                }
            }
        }

        private void BtnEBayRules_Click(object sender, RoutedEventArgs e)
        {
            CustomMessageBox.Rules();
        }

        #endregion

        #region OnClose

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            ExportCheck();
            base.OnClosing(e);
        }

        #endregion
    }
}
