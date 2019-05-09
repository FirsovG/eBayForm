using eBayForm.LogicUnits.HtmlTags;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace eBayForm.Windows
{
    /// <summary>
    /// Логика взаимодействия для PropertiesToolBox.xaml
    /// </summary>
    public partial class PropertiesToolBox : Window
    {
        private LogicController lc;
        private WebBrowser wbWorkspace;
        private List<TextBox> textBoxList;
        public PropertiesToolBox(LogicController lc, WebBrowser wbWorkspace)
        {
            InitializeComponent();
            Taskbar.Content = new DesignItems.Taskbar(this);
            this.lc = lc;
            this.wbWorkspace = wbWorkspace;
            textBoxList = new List<TextBox>();

            List<IHtmlTagElement> elements = lc.GetTags();

            List<StackPanel> spList = new List<StackPanel>();

            foreach (IHtmlTagElement element in elements)
            {

                Label label = new Label();
                label.Content = element.Element;

                TextBox textBox = new TextBox();
                textBox.TextWrapping = TextWrapping.Wrap;
                textBox.AcceptsReturn = true;
                textBox.Name = element.Element;
                textBox.Text = element.Value == "" ? "Enter text here..." : element.Value;
                textBoxList.Add(textBox);


                label.FontWeight = FontWeights.Bold;
                label.FontSize = 14;
                label.Foreground = (Brush)FindResource("MainColor");

                textBox.FontSize = 18;
                textBox.BorderThickness = new Thickness(0, 0, 0, 1.5);
                textBox.Margin = new Thickness(10, 2.5, 12.5, 10);
                textBox.Foreground = (Brush)FindResource("SecondColor");
                textBox.BorderBrush = (Brush)FindResource("SecondColor");
                textBox.GotFocus += RemoveText;
                textBox.LostFocus += AddText;

                StackPanel panel = spMain;

                if (!element.IsInList)
                {
                    panel.Children.Add(label);
                    panel.Children.Add(textBox);
                }
                else
                {
                    textBox.Margin = new Thickness(10, 2.5, 15, 8);
                    string menuName = new string(element.Element.Where(c => (c < '0' || c > '9')).ToArray()).Replace("Link", "");
                    if (lc.IsInList(spList, menuName, out panel))
                    {
                        panel.Children.Add(label);
                        panel.Children.Add(textBox);
                    }
                    else
                    {
                        Button btnForStackPanel = new Button();
                        btnForStackPanel.Content = menuName;
                        btnForStackPanel.Margin = new Thickness(5, 5, 5, 5);
                        btnForStackPanel.BorderBrush = (Brush)FindResource("MainColor");
                        btnForStackPanel.Click += BtnOpenMenu_Click;

                        StackPanel elementsStackPanel = new StackPanel();
                        elementsStackPanel.Name = "sp" + menuName;
                        elementsStackPanel.Margin = new Thickness(0, 0, 0, 10);
                        elementsStackPanel.Background = (Brush)FindResource("MaterialDesignPaper");
                        elementsStackPanel.Visibility = Visibility.Collapsed;
                        spList.Add(elementsStackPanel);

                        panel = elementsStackPanel;

                        panel.Children.Add(label);
                        panel.Children.Add(textBox);

                        btnForStackPanel.Tag = elementsStackPanel;
                        spMain.Children.Add(btnForStackPanel);
                        spMain.Children.Add(elementsStackPanel);
                    }
                }
                if (element.Element.Contains("Link"))
                {
                    HtmlLinkTagElement linkElement = (HtmlLinkTagElement)element;
                    TextBox linkTextBox = new TextBox();
                    linkTextBox.Name = "l" + linkElement.Element;
                    linkTextBox.Text = linkElement.Link == "" ? "Enter link here..." : linkElement.Link;

                    textBox.Tag = linkTextBox;

                    linkTextBox.FontSize = 18;
                    linkTextBox.BorderThickness = new Thickness(0, 0, 0, 1.5);
                    linkTextBox.Margin = new Thickness(10, 2.5, 15, 8);
                    linkTextBox.Foreground = (Brush)FindResource("SecondColor");
                    linkTextBox.BorderBrush = (Brush)FindResource("SecondColor");
                    linkTextBox.GotFocus += RemoveText;
                    linkTextBox.LostFocus += AddText;

                    panel.Children.Add(linkTextBox);
                }
            }

            Button btnSave = new Button();
            btnSave.Content = "Save Changes";
            btnSave.Margin = new Thickness(25, 5, 25, 5);
            btnSave.BorderBrush = (Brush)FindResource("MainColor");
            btnSave.Click += BtnSaveChanges_Click;
            spMain.Children.Add(btnSave);

            Button btnExport = new Button();
            btnExport.Content = "Export";
            btnExport.Margin = new Thickness(25, 5, 25, 5);
            btnExport.BorderBrush = (Brush)FindResource("MainColor");
            btnExport.Click += BtnExport_Click;
            spMain.Children.Add(btnExport);
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

        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            List<IHtmlTagElement> htmlTags = new List<IHtmlTagElement>();
            foreach (TextBox textBox in textBoxList)
            {
                bool isInList = textBox.Parent == spMain ? false : true;
                string text = textBox.Text == "Enter text here..." ? "" : textBox.Text;
                if (textBox.Tag == null)
                {
                    htmlTags.Add(new HtmlTagElement(isInList, textBox.Name, text));
                }
                else
                {
                    TextBox linkTextBox = (TextBox)textBox.Tag;
                    string link = linkTextBox.Text == "Enter link here..." ? "" : linkTextBox.Text;
                    htmlTags.Add(new HtmlLinkTagElement(isInList, textBox.Name, text, link));
                }
            }
            wbWorkspace.NavigateToString(lc.SaveChanges(htmlTags));
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "html files (*.html)|*.html";
            bool? dialogResult = fileDialog.ShowDialog();

            if (dialogResult == true)
            {
                lc.Export(fileDialog.FileName);
            }
        }

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Enter text here..." || textBox.Text == "Enter link here...")
            {
                textBox.Text = "";
            }
        }

        private void AddText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox.Name.StartsWith("l"))
                {
                    textBox.Text = "Enter link here...";
                }
                else
                {
                    textBox.Text = "Enter text here...";
                }
            }
        }
    }
}
