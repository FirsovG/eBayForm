using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBayForm.LogicUnits.Exceptions
{
    class TemplateException : Exception
    {
        public TemplateException(string message) : base (message) { }
    }
}
