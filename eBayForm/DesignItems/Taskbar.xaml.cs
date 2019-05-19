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

        public Taskbar(Window window)
        {
            this.currentWindow = window;
            InitializeComponent();
            if (this.currentWindow.GetType().ToString() != "eBayForm.MainWindow")
            {
                btnMinimize.Visibility = Visibility.Hidden;
                btnMaximize.Visibility = Visibility.Hidden;
            }

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentWindow.GetType().ToString().EndsWith("ToolBox"))
            {
                currentWindow.Hide();
            }
            else
            {
                currentWindow.Close();
            }
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
