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
    /// Логика взаимодействия для PropertiesToolBox.xaml
    /// </summary>
    public partial class PropertiesToolBox : Window
    {
        private LogicController lc;
        public PropertiesToolBox(LogicController lc)
        {
            InitializeComponent();
            Taskbar.Content = new DesignItems.Taskbar(this);
            this.lc = lc;
        }
    }
}
