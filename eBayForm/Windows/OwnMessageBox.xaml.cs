using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
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
            btnOk.IsCancel = true;

            Taskbar.Content = new DesignItems.Taskbar(this);
            if (message.Length > 70)
            {
                tbMessage.TextAlignment = TextAlignment.Left;
            }
            tbMessage.Text = message;
        }

        public OwnMessageBox(string message, int time)
        {
            InitializeComponent();
            btnOk.Click += BtnOk_Click;

            Taskbar.Content = new DesignItems.Taskbar(this);
            if (message.Length > 70)
            {
                tbMessage.TextAlignment = TextAlignment.Left;
            }
            tbMessage.Text = message;

            Timer timer = new Timer();
            timer.Interval = time;
            timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            timer.Start();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Close();
            }), null);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Close();
            }), null);
        }
    }
}
