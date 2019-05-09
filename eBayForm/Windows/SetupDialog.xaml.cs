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
    /// Логика взаимодействия для SetupDialog.xaml
    /// </summary>
    public partial class SetupDialog : Window
    {
        public SetupDialog()
        {
            InitializeComponent();
            Taskbar.Content = new DesignItems.Taskbar(this);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key >= Key.D0 && e.Key <= Key.D9))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        // Function to get all Childs from this Window
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            bool isFilled = true;
            foreach (TextBox textBox in FindVisualChildren<TextBox>(this))
            {
                if(textBox.Name.EndsWith("Count"))
                {
                    if (textBox.Text == "")
                    {
                        textBox.BorderBrush = (Brush)FindResource("WarningColor");
                        isFilled = false;
                    }
                }
            }
            if (isFilled)
            {
                this.DialogResult = true;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (((TextBox)sender).BorderBrush == (Brush)FindResource("WarningColor"))
            {
                ((TextBox)sender).BorderBrush = (Brush)FindResource("SecondColor");
            }
        }
    }
}
