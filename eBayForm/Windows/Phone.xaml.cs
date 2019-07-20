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
    /// Логика взаимодействия для Phone.xaml
    /// </summary>
    public partial class Phone : Window
    {
        public Phone(Button showButton)
        {
            InitializeComponent();
            Taskbar.Content = new DesignItems.Taskbar(this, showButton, false);
        }
    }
}
