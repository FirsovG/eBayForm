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
    /// Логика взаимодействия для YNCDialog.xaml
    /// </summary>
    public partial class YNCDialog : Window
    {
        public bool isYes { get; set; }
        public YNCDialog(string message)
        {
            InitializeComponent();
            Taskbar.Content = new DesignItems.Taskbar(this);
            tbMessage.Text = message;
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            isYes = true;
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            isYes = false;
        }
    }
}
