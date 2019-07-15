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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace eBayForm.DesignItems
{
    /// <summary>
    /// Логика взаимодействия для Taskbar.xaml
    /// </summary>
    public partial class Taskbar : Page
    {
        private Window currentWindow;
        private Button showButton;

        public Taskbar(Window window)
        {
            InitializeComponent();
            currentWindow = window;
            if (currentWindow.Name == "MainWindow")
            {
                this.MouseDown += Taskbar_Fullscreen;
                grMaximizeRestore.Visibility = Visibility.Visible;

                TextBlock logoname = new TextBlock();
                logoname.Text = "eBayForm";
                logoname.FontWeight = FontWeights.Bold;
                logoname.FontSize = 13;
                logoname.VerticalAlignment = VerticalAlignment.Center;
                logoname.Foreground = (Brush)FindResource("SecondColor");
                spLogo.Children.Add(logoname);
            }
            btnClose.Click += Close;
        }

        public Taskbar(Window window, Button showButton)
        {
            InitializeComponent();
            currentWindow = window;
            this.showButton = showButton;
            btnClose.Click += Hidde;
        }

        private void Taskbar_Fullscreen(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (currentWindow.WindowState == System.Windows.WindowState.Maximized)
                {
                    currentWindow.WindowState = System.Windows.WindowState.Normal;
                }
                else
                {
                    currentWindow.WindowState = System.Windows.WindowState.Maximized;
                }
            }
        }

        public void Close(object sender, RoutedEventArgs e)
        {
            currentWindow.Close();
        }

        public void Hidde(object sender, RoutedEventArgs e)
        {
            currentWindow.Hide();
            showButton.Visibility = Visibility.Visible;
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            currentWindow.WindowState = System.Windows.WindowState.Minimized;
        }


        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            currentWindow.WindowState = System.Windows.WindowState.Maximized;
            btnMaximize.Visibility = Visibility.Hidden;
            btnRestore.Visibility = Visibility.Visible;
        }

        private void BtnRestore_Click(object sender, RoutedEventArgs e)
        {
            currentWindow.WindowState = System.Windows.WindowState.Normal;
            btnMaximize.Visibility = Visibility.Visible;
            btnRestore.Visibility = Visibility.Hidden;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                currentWindow.DragMove();
            }
        }
    }
}
