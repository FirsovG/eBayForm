using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для OwnMessageBox.xaml
    /// </summary>
    public partial class OwnMessageBox : Window
    {
        public OwnMessageBox(string message)
        {
            InitializeComponent();
            Taskbar.Content = new DesignItems.Taskbar(this);
            tbMessage.Text = message;
        }
    }
}
