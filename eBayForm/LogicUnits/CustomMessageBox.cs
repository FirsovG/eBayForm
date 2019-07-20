using eBayForm.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace eBayForm.LogicUnits
{
    public static class CustomMessageBox
    {
        public static bool? Show(string message)
        {
            OwnMessageBox messageBox = new OwnMessageBox(message);
            messageBox.ShowDialog();

            return messageBox.DialogResult;
        }

        public static bool? Dialog(string message)
        {
            YNDialog messageBox = new YNDialog(message);
            messageBox.ShowDialog();

            return messageBox.DialogResult;
        }

        public static void Rules()
        {
            EBayRules rules = new EBayRules();
            rules.ShowDialog();
        }

        public static void ShowTipp(string tipp)
        {
            OwnMessageBox messageBox = new OwnMessageBox(tipp, 1900);
            messageBox.Show();
        }
    }
}
