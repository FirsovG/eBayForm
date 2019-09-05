using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// 


    public partial class Taskbar : Page
    {
        private Window currentWindow;
        private Button showButton;

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

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
                logoname.Foreground = (Brush)FindResource("PrimaryHueMidForegroundBrush");
                spLogo.Children.Add(logoname);
            }
            btnClose.Click += Close;
        }

        public Taskbar(Window window, Button showButton, bool showWindowFirst)
        {
            InitializeComponent();
            currentWindow = window;
            this.showButton = showButton;
            if (showWindowFirst)
            {
                currentWindow.Show();
                showButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                showButton.Visibility = Visibility.Visible;
            }
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
                if (currentWindow.WindowState == System.Windows.WindowState.Maximized)
                {
                    Point mousePoint = PointToScreen(Mouse.GetPosition(currentWindow));
                    currentWindow.Top = mousePoint.Y - 15;
                    currentWindow.Left = mousePoint.X - currentWindow.Width / 2;
                    currentWindow.WindowState = System.Windows.WindowState.Normal;
                }
                currentWindow.DragMove();
            }
        }
    }
}
