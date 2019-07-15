using eBayForm.LogicUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace eBayForm.Windows
{
    /// <summary>
    /// Логика взаимодействия для TemplateSetupDialog.xaml
    /// </summary>
    public partial class TemplateSetupDialog : Window
    {
        private string templatename;

        public TemplateSetupDialog(string templatename)
        {
            InitializeComponent();
            Taskbar.Content = new DesignItems.Taskbar(this);
            this.templatename = templatename;
            this.Title = templatename + " " + "setup";

            if (templatename == "Tea")
            {
                TeaMarkup();
            }
            else if (templatename == "CoffeeBean")
            {
                CoffeeBeanMarkup();
            }

            foreach (TextBlock textBlock in ElementFinder.FindVisualChildren<TextBlock>(spMain))
            {
                textBlock.FontWeight = FontWeight.FromOpenTypeWeight(700);
            }

            foreach (TextBox textBox in ElementFinder.FindVisualChildren<TextBox>(spMain))
            {
                textBox.KeyDown += TextBox_KeyDown;
            }

            foreach (Label optionalLabel in ElementFinder.FindVisualChildren<Label>(spMain))
            {
                optionalLabel.Foreground = (Brush)FindResource("WatermarkColor");
                optionalLabel.FontSize = 12;
                optionalLabel.Margin = new Thickness(0, 0, 0, 12);
            }
        }

        private void TeaMarkup()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "Amount of similar products";

            TextBox textBox = new TextBox();
            textBox.Name = "NavLinkCount";
            textBox.MaxLength = 1;

            Label optionalLabel = new Label();
            optionalLabel.Content = "max 5";

            spMain.Children.Add(textBlock);
            spMain.Children.Add(textBox);
            spMain.Children.Add(optionalLabel);


            textBlock = new TextBlock();
            textBlock.Text = "Amount of arguments";

            textBox = new TextBox();
            textBox.Name = "ArgumentsCount";
            textBox.MaxLength = 2;

            optionalLabel = new Label();
            optionalLabel.Content = "optional 10";

            spMain.Children.Add(textBlock);
            spMain.Children.Add(textBox);
            spMain.Children.Add(optionalLabel);


            textBlock = new TextBlock();
            textBlock.Text = "Amount of text boxes";

            textBox = new TextBox();
            textBox.Name = "TexboxCount";
            textBox.MaxLength = 2;

            optionalLabel = new Label();
            optionalLabel.Content = "optional 6";

            spMain.Children.Add(textBlock);
            spMain.Children.Add(textBox);
            spMain.Children.Add(optionalLabel);
        }

        private void CoffeeBeanMarkup()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "Amount of gallary images (incl. main image)";
            textBlock.TextWrapping = TextWrapping.Wrap;

            TextBox textBox = new TextBox();
            textBox.Name = "GallaryImageCount";
            textBox.MaxLength = 2;

            Label optionalLabel = new Label();
            optionalLabel.Content = "optonal 6";

            spMain.Children.Add(textBlock);
            spMain.Children.Add(textBox);
            spMain.Children.Add(optionalLabel);


            textBlock = new TextBlock();
            textBlock.Text = "Amount of arguments";

            textBox = new TextBox();
            textBox.Name = "ArgumentsCount";
            textBox.MaxLength = 2;

            optionalLabel = new Label();
            optionalLabel.Content = "optional 10";

            spMain.Children.Add(textBlock);
            spMain.Children.Add(textBox);
            spMain.Children.Add(optionalLabel);


            textBlock = new TextBlock();
            textBlock.Text = "Amount of text boxes";

            textBox = new TextBox();
            textBox.Name = "TexboxCount";
            textBox.MaxLength = 2;

            optionalLabel = new Label();
            optionalLabel.Content = "optional 5";

            spMain.Children.Add(textBlock);
            spMain.Children.Add(textBox);
            spMain.Children.Add(optionalLabel);


            textBlock = new TextBlock();
            textBlock.Text = "Amount of similar products";

            textBox = new TextBox();
            textBox.Name = "NavLinkCount";
            textBox.MaxLength = 1;

            optionalLabel = new Label();
            optionalLabel.Content = "optonal 5";

            spMain.Children.Add(textBlock);
            spMain.Children.Add(textBox);
            spMain.Children.Add(optionalLabel);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.Tab)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            bool isFilled = true;
            foreach (TextBox textBox in ElementFinder.FindVisualChildren<TextBox>(this))
            {
                if (textBox.Text == "" || (templatename == "Tea" && textBox.Name == "NavLinkCount" && Int32.Parse(textBox.Text) > 5))
                {
                    textBox.BorderBrush = (Brush)FindResource("WarningColor");
                    isFilled = false;
                }
            }
            if (isFilled)
            {
                this.DialogResult = true;
            }
        }
    }
}
