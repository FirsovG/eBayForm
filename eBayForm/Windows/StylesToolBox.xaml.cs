using eBayForm.LogicUnits;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace eBayForm.Windows
{
    /// <summary>
    /// Логика взаимодействия для StylesToolBox.xaml
    /// </summary>
    public partial class StylesToolBox : Window
    {

        private List<ToggleButton> toggleButtonList;
        private Button btnShowToolBox;

        public StylesToolBox(LogicController lc, WebBrowser wbWorkspace, Button btnShowToolBox)
        {
            InitializeComponent();

            this.Left = SystemParameters.PrimaryScreenWidth - this.Width - 5;
            this.Top = (SystemParameters.PrimaryScreenHeight / 2) - (this.Height / 2);

            Taskbar.Content = new DesignItems.Taskbar(this);

            List<TagStyleElement> styleElements = lc.GetStyle();
            toggleButtonList = new List<ToggleButton>();

            foreach (TagStyleElement styleElement in styleElements)
            {
                Grid grid = new Grid();
                grid.Margin = new Thickness(0, 10, 0, 10);
                spMain.Children.Add(grid);

                TextBlock textBlock = new TextBlock();
                textBlock.Text = styleElement.Name;
                textBlock.Foreground = (Brush)FindResource("MainColor");
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.FontWeight = FontWeights.Bold;
                textBlock.FontSize = 16;
                textBlock.Margin = new Thickness(15, 0, 15, 0);
                textBlock.HorizontalAlignment = HorizontalAlignment.Left;

                ToggleButton toggleButton = new ToggleButton();
                toggleButton.Name = styleElement.Name.Replace(" ", "");
                toggleButton.IsChecked = true;
                toggleButton.Margin = new Thickness(15, 0, 15, 0);
                toggleButton.HorizontalAlignment = HorizontalAlignment.Right;
                toggleButton.Height = 20;
                toggleButton.Width = 40;

                ColorCanvas colorCanvas = new ColorCanvas();
                string hexcolor = styleElement.Value.Trim();
                if(hexcolor.Length == 7)
                {
                    colorCanvas.UsingAlphaChannel = false;
                    colorCanvas.SelectedColor = (Color)ColorConverter.ConvertFromString(hexcolor);
                }
                else
                {
                    string[] rgbaColorString = hexcolor.Split('(')[1].Split(')')[0].Split(',');
                    byte[] rgbaColorByte = new byte[4];
                    for (int i = 0; i < 3; i++)
                    {
                        rgbaColorByte[i] = Convert.ToByte(rgbaColorString[i]);
                    }
                    float floatA = float.Parse(rgbaColorString[3], CultureInfo.InvariantCulture.NumberFormat);
                    floatA *= 255;
                    rgbaColorByte[3] = Convert.ToByte(floatA);

                    colorCanvas.R = rgbaColorByte[0];
                    colorCanvas.G = rgbaColorByte[1];
                    colorCanvas.B = rgbaColorByte[2];
                    colorCanvas.A = rgbaColorByte[3];
                }
                colorCanvas.Background = (Brush)FindResource("MaterialDesignPaper");
                colorCanvas.BorderThickness = new Thickness(0, 0, 0, 0);
                colorCanvas.Visibility = Visibility.Collapsed;

                toggleButton.Tag = colorCanvas;
                toggleButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorCanvas.HexadecimalString));
                toggleButton.Checked += ToggleButton_Checked;
                toggleButton.Unchecked += ToggleButton_Unchecked;

                toggleButtonList.Add(toggleButton);

                grid.Children.Add(textBlock);
                grid.Children.Add(toggleButton);

                spMain.Children.Add(colorCanvas);
            }
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ColorCanvas colorCanvas = (ColorCanvas)(sender as ToggleButton).Tag;
            colorCanvas.Visibility = Visibility.Visible;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ColorCanvas colorCanvas = (ColorCanvas)(sender as ToggleButton).Tag;
            (sender as ToggleButton).Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorCanvas.HexadecimalString));
            colorCanvas.Visibility = Visibility.Collapsed;
        }

        public List<TagStyleElement> SaveChanges()
        {
            List<TagStyleElement> styleElements = new List<TagStyleElement>();
            foreach (ToggleButton toggleButton in toggleButtonList)
            {
                ColorCanvas colorCanvas = (ColorCanvas)toggleButton.Tag;
                string hexcolor = colorCanvas.HexadecimalString;
                if (hexcolor.Length != 7)
                {
                    string Alpha = (Convert.ToSingle(colorCanvas.A) / 255).ToString().Replace(',', '.');
                    hexcolor = "rgba(" + colorCanvas.R + ", " + colorCanvas.G + ", " + colorCanvas.B + ", " + Alpha + ")";
                }
                styleElements.Add(new TagStyleElement(toggleButton.Name, hexcolor));
            }

            return styleElements;
        }
    }
}
