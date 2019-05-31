using eBayForm.LogicUnits;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace eBayForm.Windows
{
    /// <summary>
    /// Логика взаимодействия для PropertiesToolBox.xaml
    /// </summary>
    /// 

    public partial class PropertiesToolBox : Window
    {
        
        private List<TextBox> textBoxList;
        private Button btnShowToolBox;

        public PropertiesToolBox(LogicController lc, WebBrowser wbWorkspace, Button btnShowToolBox)
        {
            InitializeComponent();

            this.Left = 5;
            this.Top = (SystemParameters.PrimaryScreenHeight / 2) - (this.Height / 2);
            
            IsVisibleChanged += OnClose;

            Taskbar.Content = new DesignItems.Taskbar(this);
            textBoxList = new List<TextBox>();
            this.btnShowToolBox = btnShowToolBox;

            List<HtmlTagElement> elements = lc.GetTags();

            List<StackPanel> spList = new List<StackPanel>();

            int imageBoxCount = 0;
            int textBoxCount = 0;
            int linkBoxCount = 0;
            foreach (HtmlTagElement element in elements)
            {
                Label label = new Label();
                label.Content = element.Element;

                TextBox textBox = new TextBox();
                textBoxList.Add(textBox);


                label.FontWeight = FontWeights.Bold;
                label.FontSize = 14;
                label.Foreground = (Brush)FindResource("MainColor");
                
                if (element.Element.Contains("image") || element.Element.Contains("logo"))
                {
                    textBox.Name = "tbImage" + imageBoxCount++;
                    textBox.Text = element.Value == "" ? "Enter source here..." : Regex.Replace(element.Value, @"\s+", " ");
                }
                else
                {
                    textBox.Name = "tbText" + textBoxCount++;
                    textBox.Text = element.Value == "" ? "Enter text here..." : Regex.Replace(element.Value, @"\s+", " ");
                    textBox.AcceptsReturn = true;
                    textBox.TextWrapping = TextWrapping.Wrap;
                }
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
                    string menuName = new string(element.Element.Where(c => (c < '0' || c > '9')).ToArray()).Replace(" ", "");
                    if (menuName != "Text")
                    {
                        menuName += "s";
                    }
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
                if (element.Link != null)
                {
                    TextBox linkTextBox = new TextBox();
                    linkTextBox.Text = element.Link == "" ? "Enter link here..." : element.Link;
                    linkTextBox.Name = "tbLink" + linkBoxCount++;

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
        }

        public List<HtmlTagElement> SaveChanges()
        {
            List<HtmlTagElement> htmlTags = new List<HtmlTagElement>();
            foreach (TextBox textBox in textBoxList)
            {
                bool isInList = textBox.Parent == spMain ? false : true;
                string text = textBox.Text == "Enter text here..." ? "" : textBox.Text == "Enter source here..." ? "" : textBox.Text ;
                if (textBox.Tag == null)
                {
                    htmlTags.Add(new HtmlTagElement(isInList, textBox.Name, text));
                }
                else
                {
                    TextBox linkTextBox = (TextBox)textBox.Tag;
                    string link = linkTextBox.Text == "Enter link here..." ? "" : linkTextBox.Text;
                    htmlTags.Add(new HtmlTagElement(isInList, textBox.Name, text, link));
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

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Enter text here..." || textBox.Text == "Enter source here..." || textBox.Text == "Enter link here...")
            {
                textBox.Text = "";
            }
        }

        private void AddText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox.Name.StartsWith("tbLink"))
                {
                    textBox.Text = "Enter link here...";
                }
                else if (textBox.Name.StartsWith("tbImage"))
                {
                    textBox.Text = "Enter source here...";
                }
                else if (textBox.Name.StartsWith("tbText"))
                {
                    textBox.Text = "Enter text here...";
                }
            }
        }

        private void OnClose(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((Window)sender).Visibility == Visibility.Hidden)
            {
                btnShowToolBox.Visibility = Visibility.Visible;
            }
        }
    }
}
