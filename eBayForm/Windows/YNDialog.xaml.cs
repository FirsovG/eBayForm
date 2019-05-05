using System.Windows;

namespace eBayForm.Windows
{
    public partial class YNDialog : Window
    {
        public YNDialog(string message)
        {
            InitializeComponent();
            Taskbar.Content = new DesignItems.Taskbar(this);
            tbMessage.Text = message;
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
