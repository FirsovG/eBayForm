using eBayForm.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
