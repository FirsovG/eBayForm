﻿using eBayForm.LogicUnits;
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

    public delegate void DataChangedDelegate(object sender);

    public partial class MainWindow : Window
    {
        PropertiesToolBox toolBox;
        StylesToolBox stylesToolBox;
        LogicController lc;
        Phone phone;
        public static RoutedCommand cmdSaveChanges = new RoutedCommand();

        bool documentIsChanged = false;
        // TODO: export file
        //bool wasExported = false;

        public MainWindow()
        {
            InitializeComponent();
            this.Name = "MainWindow";
            Taskbar.Content = new DesignItems.Taskbar(this);
            lc = new LogicController();
            wbWorkspace.Navigating += WbWorkspace_Navigating;

            if(lc.Templates != null)
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
            
            if (phone == null)
            {
                phone = new Phone(btnShowPhone);
            }
            phone.wbWorkspace.NavigateToString(htmlCode);
            spPhoneButtons.Visibility = Visibility.Visible;

            wbWorkspace.Visibility = Visibility.Visible;
            spButtons.Visibility = Visibility.Visible;

            cmdSaveChanges.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            
            stylesToolBox = new StylesToolBox(this, lc, wbWorkspace, btnShowStyleBox);
            stylesToolBox.Show();
            stylesToolBox.DataChanged += DataChangedHandler; 
            toolBox = new PropertiesToolBox(this, lc, wbWorkspace, btnShowToolBox);
            toolBox.Show();
            toolBox.DataChanged += DataChangedHandler;

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
