using eBayForm.LogicUnits.HtmlTags;
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
                textBox.Name = element.Element;
                textBox.Text = element.Value;
                textBoxList.Add(textBox);


                label.FontWeight = FontWeights.Bold;
                label.FontSize = 14;
                label.Foreground = (Brush)FindResource("MainColor");

                textBox.FontSize = 18;
                textBox.BorderThickness = new Thickness(0, 0, 0, 1.5);
                textBox.Margin = new Thickness(7.5, 2.5, 7.5, 10);
                textBox.Foreground = (Brush)FindResource("SecondColor");
                textBox.BorderBrush = (Brush)FindResource("MainColor");

                if (element.IsInList == false)
                {
                    spMain.Children.Add(label);
                    spMain.Children.Add(textBox);
                    
                    if (element.Element.Contains("Link"))
                    {
                        HtmlLinkTagElement linkElement = (HtmlLinkTagElement)element;
                        TextBox linkTextBox = new TextBox();

                        textBox.Tag = linkTextBox;

                        linkTextBox.FontSize = 18;
                        linkTextBox.BorderThickness = new Thickness(0, 0, 0, 1.5);
                        linkTextBox.Margin = new Thickness(7.5, 2.5, 7.5, 10);
                        linkTextBox.Foreground = (Brush)FindResource("SecondColor");
                        linkTextBox.BorderBrush = (Brush)FindResource("MainColor");

                        spMain.Children.Add(linkTextBox);
                    }
                }
                else
                {
                    textBox.Margin = new Thickness(10, 2.5, 10, 8);
                    string menuName = new string(element.Element.Where(c => (c < '0' || c > '9')).ToArray());
                    StackPanel panel;
                    if (lc.IsInList(spList, menuName, out panel))
                    {
                        panel.Children.Add(label);
                        panel.Children.Add(textBox);
                    }
                    else
                    {
                        Button btnForStackPanel = new Button();
                        btnForStackPanel.Content = menuName;
                        btnForStackPanel.Margin = new Thickness(0, 5, 0, 5);
                        btnForStackPanel.BorderBrush = (Brush)FindResource("MainColor");
                        btnForStackPanel.Click += BtnOpenMenu_Click;

                        StackPanel elementsStackPanel = new StackPanel();
                        elementsStackPanel.Name = "sp" + menuName;
                        elementsStackPanel.Margin = new Thickness(0, 0, 0, 10);
                        elementsStackPanel.Background = (Brush)FindResource("AccentDark");
                        elementsStackPanel.Visibility = Visibility.Collapsed;
                        spList.Add(elementsStackPanel);

                        panel = elementsStackPanel;

                        panel.Children.Add(label);
                        panel.Children.Add(textBox);

                        btnForStackPanel.Tag = elementsStackPanel;
                        spMain.Children.Add(btnForStackPanel);
                        spMain.Children.Add(elementsStackPanel);
                    }

                    if (element.Element.Contains("Link"))
                    {
                        HtmlLinkTagElement linkElement = (HtmlLinkTagElement)element;
                        TextBox linkTextBox = new TextBox();
                        linkTextBox.Name = "l" + linkElement.Element;
                        linkTextBox.Text = linkElement.Link;

                        textBox.Tag = linkTextBox;

                        linkTextBox.FontSize = 18;
                        linkTextBox.BorderThickness = new Thickness(0, 0, 0, 1.5);
                        linkTextBox.Margin = new Thickness(10, 2.5, 10, 8);
                        linkTextBox.Foreground = (Brush)FindResource("SecondColor");
                        linkTextBox.BorderBrush = (Brush)FindResource("MainColor");

                        panel.Children.Add(linkTextBox);
                    }
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
                if (textBox.Tag == null)
                {
                    htmlTags.Add(new HtmlTagElement(isInList, textBox.Name, textBox.Text));
                }
                else
                {
                    string link = ((TextBox)textBox.Tag).Text;
                    htmlTags.Add(new HtmlLinkTagElement(isInList, textBox.Name, textBox.Text, link));
                }
            }
            wbWorkspace.NavigateToString(lc.SaveChanges(htmlTags));
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Export not ready");
        }
    }
}
