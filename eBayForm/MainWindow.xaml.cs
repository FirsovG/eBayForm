using eBayForm.LogicUnits;
using eBayForm.LogicUnits.Exceptions;
using eBayForm.Windows;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfScreenHelper;

namespace eBayForm
{

    public delegate void DataChangedDelegate(object sender);

    public partial class MainWindow : Window
    {
        PropertiesToolBox toolBox;
        StylesToolBox stylesToolBox;
        LogicController lc;
        Phone phone;

        List<Screen> screens;

        public static RoutedCommand cmdSaveChanges = new RoutedCommand();

        bool documentIsChanged = false;
        // TODO: export file
        //bool wasExported = false;
        public MainWindow()
        {
            screens = new List<Screen>();
            foreach (Screen screen in Screen.AllScreens)
            {
                screens.Add(screen);
            }

            InitializeComponent();

            this.Name = "MainWindow";

            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.LocationChanged += MainWindow_LocationChanged;
            this.StateChanged += MainWindow_StateChanged;

            Taskbar.Content = new DesignItems.Taskbar(this);
            lc = new LogicController();
            wbWorkspace.Navigating += WbWorkspace_Navigating;
            

            if (lc.Templates != null)
            {
                foreach (Button button in ElementFinder.FindVisualChildren<Button>(wpTemplates))
                {
                    for (int i = 0; i < lc.Templates.Length; i++)
                    {
                        if (button.Name == lc.Templates[i])
                        {
                            button.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }

        private void MainWindow_StateChanged(object sender, System.EventArgs e)
        {

        }

        private void MainWindow_LocationChanged(object sender, System.EventArgs e)
        {
            if (screens.Count > 1)
            {
                MainWindow window = (MainWindow)sender;

                int windowMiddleState = (int)(window.Left + window.Width / 2);
                for (int i = 0; i < screens.Count; i++)
                {
                    if ((screens[i].Bounds.Left < windowMiddleState) && (windowMiddleState < (screens[i].Bounds.Left + screens[i].Bounds.Width)))
                    {
                        window.MaxHeight = screens[i].WorkingArea.Height + 16;
                        return;
                    }
                }
            }
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

            spPhoneButtons.Visibility = Visibility.Visible;
            spButtons.Visibility = Visibility.Visible;
            wbWorkspace.Visibility = Visibility.Visible;

            cmdSaveChanges.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));

            phone = new Phone(btnShowPhone);
            phone.wbWorkspace.NavigateToString(htmlCode);

            stylesToolBox = new StylesToolBox(this, lc, wbWorkspace, btnShowStyleBox);
            stylesToolBox.DataChanged += DataChangedHandler; 

            toolBox = new PropertiesToolBox(this, lc, wbWorkspace, btnShowToolBox);
            toolBox.DataChanged += DataChangedHandler;

            if (screens.Count > 1)
            {
                toolBox.Top = screens[1].Bounds.Top;
                stylesToolBox.Top = screens[1].Bounds.Top;

                toolBox.Height = screens[1].WorkingArea.Height;
                stylesToolBox.Height = screens[1].WorkingArea.Height;

                toolBox.Width = screens[1].Bounds.Width / 2;
                stylesToolBox.Width = screens[1].Bounds.Width / 2;

                toolBox.Left = screens[1].Bounds.Left;
                stylesToolBox.Left = screens[1].Bounds.Left + screens[1].Bounds.Width / 2;

                this.WindowState = WindowState.Maximized;
            }

            CustomMessageBox.ShowTipp("Ctrl+S to save changes");

            tabControll.SelectedIndex = 0;
        }

        private void DataChangedHandler(object sender)
        {
            // Log with sender :: Later
            if (!documentIsChanged)
            {
                documentIsChanged = true;
            }
        }

        public void SaveChanges()
        {
            if (lc.Document != null && toolBox != null && stylesToolBox != null)
            {
                List<HtmlTagElement> htmlTags = toolBox.SaveChanges();
                List<StyleTagElement> styleElements = stylesToolBox.SaveChanges();
                string htmlCode = lc.SaveChanges(htmlTags, styleElements);
                wbWorkspace.NavigateToString(htmlCode);
                phone.wbWorkspace.NavigateToString(htmlCode);
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
            if(documentIsChanged)
            {
                if (lc.Document != null && toolBox != null && stylesToolBox != null)
                {
                    bool? result = CustomMessageBox.Dialog("Do you want to export the file?");
                    if (result == true)
                    {
                        SaveFileDialog fileDialog = new SaveFileDialog();
                        fileDialog.Filter = "Html-Files (*.html)|*.html";
                        bool? dialogResult = fileDialog.ShowDialog();

                        if (dialogResult == true)
                        {
                            lc.Export(fileDialog.FileName);
                        }
                    }
                    
                    wbWorkspace.Visibility = Visibility.Hidden;
                    spButtons.Visibility = Visibility.Hidden;
                    spPhoneButtons.Visibility = Visibility.Hidden;

                    phone.Hide();
                    toolBox.Close();
                    stylesToolBox.Close();
                    documentIsChanged = false;
                }
            }
        }

        private void BtnExportFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "Html-Files (*.html)|*.html";
            bool? dialogResult = fileDialog.ShowDialog();

            if (dialogResult == true)
            {
                lc.Export(fileDialog.FileName);
                documentIsChanged = false;
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
                    this.Title = lc.Templatename + " : " + "eBayForm";
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
            (sender as Button).Visibility = Visibility.Collapsed;
        }

        private void BtnShowStyleBox_Click(object sender, RoutedEventArgs e)
        {
            stylesToolBox.Show();
            (sender as Button).Visibility = Visibility.Collapsed;
        }

        private void BtnShowPhone_Click(object sender, RoutedEventArgs e)
        {
            phone.Show();
            (sender as Button).Visibility = Visibility.Collapsed;
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
                    this.Title = lc.Templatename + " : " + "eBayForm";
                    documentIsChanged = true;
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
