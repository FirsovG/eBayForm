using eBayForm.DesignItems;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace eBayForm.Windows
{
    public partial class ImportTextDialog : Window
    {

        public ImportTextDialog()
        {
            InitializeComponent();
            Taskbar.Content = new DesignItems.Taskbar(this);
            tbText.Style = tbSeparatorHT.Style;

            tbSeparatorHT.LostFocus += TextBox_LostFocus;
            tbSeparatorTT.LostFocus += TextBox_LostFocus;
            tbText.LostFocus += TextBox_LostFocus;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).BorderBrush = (Brush)FindResource("PrimaryHueMidForegroundBrush");
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            bool isUnfill = false;
            if(tbSeparatorHT.Text == "")
            {
                tbSeparatorHT.BorderBrush = (Brush)FindResource("WarningColor");
                isUnfill = true;
            }

            if (tbSeparatorTT.Text == "")
            {
                tbSeparatorTT.BorderBrush = (Brush)FindResource("WarningColor");
                isUnfill = true;
            }

            if (tbText.Text == "")
            {
                tbText.BorderBrush = (Brush)FindResource("WarningColor");
                isUnfill = true;
            }

            if(isUnfill)
            {
                return;
            }

            
            this.DialogResult = true;
        }
    }
}
