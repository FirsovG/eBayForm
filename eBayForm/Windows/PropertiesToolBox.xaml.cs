using eBayForm.LogicUnits;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Windows.Input;
using eBayForm.DesignItems;
using System;

namespace eBayForm.Windows
{

    public partial class PropertiesToolBox : Window
    {
        
        private List<WatermarkTextBox> textBoxList;
        private List<WatermarkTextBox> htmlTextBoxList;
        private LogicController lc;
        public static RoutedCommand cmdSaveChanges = new RoutedCommand();
        public event DataChangedDelegate DataChanged;
        public Button btnTextImport;

        public PropertiesToolBox(MainWindow mainWindow, LogicController lc, WebBrowser wbWorkspace, Button showButton)
        {
            InitializeComponent();

            this.lc = lc;
            this.Tag = mainWindow;
            cmdSaveChanges.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));

            this.Left = 5;
            this.Top = (SystemParameters.PrimaryScreenHeight / 2) - (this.Height / 2);

            Taskbar.Content = new Taskbar(this, showButton, true);
            textBoxList = new List<WatermarkTextBox>();
            htmlTextBoxList = new List<WatermarkTextBox>();

            List<HtmlTagElement> elements = lc.GetTags();

            List<StackPanel> spList = new List<StackPanel>();
            foreach (HtmlTagElement element in elements)
            {
                
                StackPanel panel = spMain;

                if (element.IsInList)
                {
                    string menuName = new string(element.Name.Where(c => (c < '0' || c > '9')).ToArray()).Replace(" ", "");
                    if (menuName != "Text")
                    {
                        menuName += "s";
                    }

                    foreach (StackPanel currentPanel in spList)
                    {
                        if (currentPanel.Name == "sp" + menuName)
                        {
                            panel = currentPanel;
                            continue;
                        }
                    }
                    if (panel == spMain)
                    {
                        Button btnForStackPanel = new Button();
                        btnForStackPanel.Content = menuName;
                        btnForStackPanel.Click += BtnOpenMenu_Click;

                        StackPanel elementsStackPanel = new StackPanel();
                        elementsStackPanel.Name = "sp" + menuName;
                        elementsStackPanel.Visibility = Visibility.Collapsed;
                        spList.Add(elementsStackPanel);

                        panel = elementsStackPanel;

                        if (menuName == "Textblocks")
                        {
                            btnTextImport = new Button();
                            btnTextImport.Content = "Import text";
                            btnTextImport.Background = Brushes.Transparent;
                            btnTextImport.Click += BtnTextImport_Click;
                            panel.Children.Add(btnTextImport);
                        }

                        btnForStackPanel.Tag = elementsStackPanel;
                        spMain.Children.Add(btnForStackPanel);
                        spMain.Children.Add(elementsStackPanel);
                    }
                }

                Label label = new Label();
                label.Content = element.Name;
                panel.Children.Add(label);

                WatermarkTextBox tmpTextBox;

                if (element.Type == "Text")
                {
                    WatermarkTextBox textBox = new WatermarkTextBox("Enter text here...");
                    textBox.Text = element.Values[0];
                    textBox.AcceptsReturn = true;
                    textBox.TextWrapping = TextWrapping.Wrap;

                    textBox.LostFocus += TextBox_LostFocus;
                    textBoxList.Add(textBox);
                    panel.Children.Add(textBox);
                }
                else if (element.Type == "Image")
                {
                    WatermarkTextBox textBox = new WatermarkTextBox("Enter source here...");
                    textBox.Text = element.Values[0];
                    textBox.WatermarkHideString = "https://i.imgur.com/ko8F6LC.png";
                    textBox.MouseDoubleClick += DoubleClickOnLink;

                    textBox.LostFocus += TextBox_LostFocus;
                    textBoxList.Add(textBox);
                    panel.Children.Add(textBox);
                }
                else if (element.Type == "Link")
                {
                    WatermarkTextBox textBox = new WatermarkTextBox("Enter text here...");
                    textBox.Text = element.Values[0];
                    textBox.AcceptsReturn = true;
                    textBox.TextWrapping = TextWrapping.Wrap;

                    textBox.LostFocus += TextBox_LostFocus;
                    textBoxList.Add(textBox);
                    panel.Children.Add(textBox);


                    tmpTextBox = textBox;
                    textBox = new WatermarkTextBox("Enter link here...");
                    textBox.Text = element.Values[1];
                    textBox.WatermarkHideString = "#";
                    textBox.MouseDoubleClick += DoubleClickOnLink;

                    textBox.LostFocus += TextBox_LostFocus;
                    tmpTextBox.Tag = textBox;
                    textBoxList.Add(textBox);
                    panel.Children.Add(textBox);
                }
                else if (element.Type == "Textblock")
                {
                    WatermarkTextBox textBox = new WatermarkTextBox("Enter text here...");
                    textBox.Text = element.Values[0];
                    textBox.AcceptsReturn = true;
                    textBox.TextWrapping = TextWrapping.Wrap;

                    textBox.LostFocus += TextBox_LostFocus;
                    textBoxList.Add(textBox);
                    panel.Children.Add(textBox);


                    tmpTextBox = textBox;
                    textBox = new WatermarkTextBox("Enter text here...");
                    textBox.Text = element.Values[1];
                    textBox.AcceptsReturn = true;
                    textBox.TextWrapping = TextWrapping.Wrap;

                    textBox.LostFocus += TextBox_LostFocus;
                    tmpTextBox.Tag = textBox;
                    textBoxList.Add(textBox);
                    panel.Children.Add(textBox);

                    htmlTextBoxList.Add(tmpTextBox);
                    htmlTextBoxList.Add(textBox);
                }
                else if (element.Type == "ImageAndLink")
                {
                    WatermarkTextBox textBox = new WatermarkTextBox("Enter text here...");
                    textBox.Text = element.Values[0];
                    textBox.AcceptsReturn = true;
                    textBox.TextWrapping = TextWrapping.Wrap;

                    textBox.LostFocus += TextBox_LostFocus;
                    textBoxList.Add(textBox);
                    panel.Children.Add(textBox);


                    tmpTextBox = textBox;
                    textBox = new WatermarkTextBox("Enter link here...");
                    textBox.Text = element.Values[1];
                    textBox.WatermarkHideString = "#";
                    textBox.MouseDoubleClick += DoubleClickOnLink;

                    textBox.LostFocus += TextBox_LostFocus;
                    tmpTextBox.Tag = textBox;
                    textBoxList.Add(textBox);
                    panel.Children.Add(textBox);


                    tmpTextBox = textBox;
                    textBox = new WatermarkTextBox("Enter image source here...");
                    textBox.Text = element.Values[2];
                    textBox.WatermarkHideString = "https://i.imgur.com/ko8F6LC.png";
                    textBox.MouseDoubleClick += DoubleClickOnLink;

                    textBox.LostFocus += TextBox_LostFocus;
                    tmpTextBox.Tag = textBox;
                    textBoxList.Add(textBox);
                    panel.Children.Add(textBox);
                }
            }
        }

        private void BtnTextImport_Click(object sender, RoutedEventArgs e)
        {
            ImportTextDialog importTextDialog = new ImportTextDialog();
            importTextDialog.ShowDialog();
            if (importTextDialog.DialogResult == true)
            {
                char textSpliter = importTextDialog.tbSeparatorHT.Text[0];
                char textBoxSpliter = importTextDialog.tbSeparatorTT.Text[0];

                string textToSplit = importTextDialog.tbText.Text.Replace("\r\n", "");

                string[] textBlocks = textToSplit.Split(textBoxSpliter);

                if (string.IsNullOrWhiteSpace(textBlocks[textBlocks.Length - 1]))
                {
                    Array.Resize(ref textBlocks, textBlocks.Length - 1);
                }

                int neededTextBlocksCount = htmlTextBoxList.Count / 2;
                if (textBlocks.Length >= neededTextBlocksCount)
                {
                    int j = 0;
                    string[] splitedTextBlock;
                    for (int i = 0; i < neededTextBlocksCount; i++)
                    {
                        splitedTextBlock = textBlocks[i].Split(textSpliter);
                        htmlTextBoxList[j++].Text = splitedTextBlock[0];
                        htmlTextBoxList[j++].Text = splitedTextBlock[1];
                    }
                }
                else
                {
                    CustomMessageBox.Show("Not enough textblocks");
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            DataChanged(sender);
        }

        private void DoubleClickOnLink(object sender, MouseButtonEventArgs e)
        {
            WatermarkTextBox textBox = (WatermarkTextBox)sender;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }

        public List<HtmlTagElement> SaveChanges()
        {
            List<HtmlTagElement> htmlTags = new List<HtmlTagElement>();
            for (int i = 0; i < textBoxList.Count; i++)
            {
                if (textBoxList[i].Tag == null)
                {
                    htmlTags.Add(new HtmlTagElement(textBoxList[i].Text));
                }
                else
                {
                    if (textBoxList[i + 1].Tag == null)
                    {
                        htmlTags.Add(new HtmlTagElement(textBoxList[i].Text, textBoxList[i + 1].Text));
                        i += 1;
                    }
                    else
                    {
                        htmlTags.Add(new HtmlTagElement(textBoxList[i].Text, textBoxList[i + 1].Text, textBoxList[i + 2].Text));
                        i += 2;
                    }
                }
            }
            return htmlTags;
        }

        private void BtnOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            StackPanel panel = (StackPanel)((Button)sender).Tag;
            if (panel.Visibility == Visibility.Collapsed)
            {
                panel.Visibility = Visibility.Visible;
            }
            else
            {
                panel.Visibility = Visibility.Collapsed;
            }
        }

        private void CbSaveChanges_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (this.Tag as MainWindow).SaveChanges();
        }
    }
}
