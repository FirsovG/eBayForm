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
    /// Логика взаимодействия для GetFromWeb.xaml
    /// </summary>
    public partial class GetFromWeb : Window
    {
        public GetFromWeb()
        {
            InitializeComponent();
            Taskbar.Content = new DesignItems.Taskbar(this);
        }

        private void SumbitLink_Click(object sender, RoutedEventArgs e)
        {
            if (Link.Text.StartsWith("http://") || Link.Text.StartsWith("https://") || 
                Link.Text.StartsWith("www") || Link.Text.StartsWith("ebay"))
            {
                if (Link.Text.StartsWith("www"))
                {
                    Link.Text = "https://" + Link.Text;
                }
                else if (Link.Text.StartsWith("ebay"))
                {
                    Link.Text = "https://www." + Link.Text;
                }
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please enter an correct Link");
            }
        }
    }
}
