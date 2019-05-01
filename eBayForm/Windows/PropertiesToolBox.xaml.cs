using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            for (int i = 0; i < 10; i++)
            {
                Label label1 = new Label();
                label1.Content = "H1";
                label1.FontWeight = FontWeights.Bold;
                label1.FontSize = 14;
                label1.Foreground = (Brush)FindResource("MainForeground");
                TextBox textBox1 = new TextBox();
                textBox1.FontSize = 18;
                textBox1.BorderThickness = new Thickness(0, 0, 0, 1.5);
                textBox1.Margin = new Thickness(2.5, 2.5, 2.5, 10);
                textBox1.BorderBrush = (Brush)FindResource("SecondBackground");
                textBox1.Text = "Hello";
                spList.Children.Add(label1);
                spList.Children.Add(textBox1);
            }
        }
    }
}
