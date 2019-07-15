using eBayForm.DesignItems;
using eBayForm.LogicUnits;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
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
        private ComboBox cbIcons;
        public static RoutedCommand cmdSaveChanges = new RoutedCommand();
        public event DataChangedDelegate DataChanged;

        public StylesToolBox(MainWindow mainWindow, LogicController lc, WebBrowser wbWorkspace, Button showButton)
        {
            InitializeComponent();

            this.Owner = mainWindow;
            cmdSaveChanges.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));

            this.Left = SystemParameters.PrimaryScreenWidth - this.Width - 5;
            this.Top = (SystemParameters.PrimaryScreenHeight / 2) - (this.Height / 2);

            Taskbar.Content = new DesignItems.Taskbar(this, showButton);
            
            toggleButtonList = new List<ToggleButton>();

            foreach (StyleTagElement styleElement in lc.GetStyle())
            {
                if (styleElement.Name == "Bulletpoints symbol")
                {
                    GenerateIconControllBox(styleElement);
                }
                else
                {
                    GenerateColorCanvases(styleElement);
                }
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

        private void GenerateIconControllBox(StyleTagElement styleElement)
        {
            Grid grid = new Grid();

            TextBlock textBlock = new TextBlock();
            textBlock.Text = styleElement.Name;

            cbIcons = new ComboBox();
            List<ComboBoxItem> comboBoxItems = new List<ComboBoxItem>();
            StackPanel panel;
            ComboBoxItem comboBoxItem;
            Viewbox iconBox;
            ContentControl icon;
            Label lIcon;
            foreach (string iconName in ((string)FindResource("CurrentIcons")).Split(','))
            {
                panel = new StackPanel();
                panel.Orientation = Orientation.Horizontal;

                icon = new ContentControl();
                icon.Content = (Canvas)FindResource(iconName);

                iconBox = new Viewbox();
                iconBox.Height = 20;
                iconBox.Width = 20;
                iconBox.VerticalAlignment = VerticalAlignment.Center;

                iconBox.Child = icon;

                lIcon = new Label();
                lIcon.Content = iconName.Replace("_", " ");

                panel.Children.Add(iconBox);
                panel.Children.Add(lIcon);

                comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = panel;
                comboBoxItem.Height = 40;

                comboBoxItems.Add(comboBoxItem);
            }
            cbIcons.ItemsSource = comboBoxItems;
            cbIcons.SelectedIndex = 0;

            int counter = 0;
            foreach (string iconClass in ((string)FindResource("CurrentIconsHtmlClasses")).Split(','))
            {
                if (iconClass == styleElement.Value)
                {
                    cbIcons.SelectedIndex = counter;
                }
                counter++;
            }

            grid.Children.Add(textBlock);
            grid.Children.Add(cbIcons);

            spMain.Children.Add(grid);
            cbIcons.LostFocus += StyleChanged;
        }

        private void GenerateColorCanvases(StyleTagElement styleElement)
        {
            Grid grid = new Grid();

            TextBlock textBlock = new TextBlock();
            textBlock.Text = styleElement.Name;

            ToggleButton toggleButton = new ToggleButton();
            toggleButton.IsChecked = true;
            toggleButtonList.Add(toggleButton);

            OwnColorCanvas colorCanvas = new OwnColorCanvas();
            colorCanvas.TextBoxStyle = Application.Current.TryFindResource(typeof(TextBox)) as Style;
            colorCanvas.TextBoxForeground = (Brush)FindResource("SecondColor");
            colorCanvas.ApplyTemplate();
            colorCanvas.LostFocus += StyleChanged;

            colorCanvas.Background = (Brush)FindResource("AccentDark");
            colorCanvas.BorderThickness = new Thickness(0);
            colorCanvas.Visibility = Visibility.Collapsed;

            string hexcolor = styleElement.Value.Trim();
            if (hexcolor.Length == 7)
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

            toggleButton.Tag = colorCanvas;
            toggleButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(colorCanvas.HexadecimalString);
            toggleButton.Checked += ToggleButton_Checked;
            toggleButton.Unchecked += ToggleButton_Unchecked;

            grid.Children.Add(textBlock);
            grid.Children.Add(toggleButton);

            spMain.Children.Add(grid);
            spMain.Children.Add(colorCanvas);
        }

        public List<StyleTagElement> SaveChanges()
        {
            List<StyleTagElement> styleElements = new List<StyleTagElement>();

            styleElements.Add(new StyleTagElement(((string)FindResource("CurrentIconsHtmlClasses")).Split(',')[cbIcons.SelectedIndex]));

            foreach (ToggleButton toggleButton in toggleButtonList)
            {
                ColorCanvas colorCanvas = (ColorCanvas)toggleButton.Tag;
                string hexcolor = colorCanvas.HexadecimalString;
                if (hexcolor.Length != 7)
                {
                    string Alpha = (Convert.ToSingle(colorCanvas.A) / 255).ToString().Replace(',', '.');
                    hexcolor = "rgba(" + colorCanvas.R + ", " + colorCanvas.G + ", " + colorCanvas.B + ", " + Alpha + ")";
                }
                styleElements.Add(new StyleTagElement(hexcolor));
            }

            return styleElements;
        }

        private void StyleChanged(object sender, RoutedEventArgs e)
        {
            DataChanged(sender);
        }

        private void CbSaveChanges_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            (this.Owner as MainWindow).SaveChanges();
        }
    }
}
