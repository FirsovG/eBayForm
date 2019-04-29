using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace eBayForm
{
    sealed class LogicController
    {
        public string ImportHtml()
        {
            string htmlCode;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "html files (*.html)|*.html";
            Nullable<bool> result = openFileDialog.ShowDialog();
            if (result == true)
            {

                using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                {
                    return reader.ReadToEnd();
                }
            }
            return null;
        }
    }
}
