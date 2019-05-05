using System;

namespace eBayForm.LogicUnits.Exceptions
{
    class NotATemplateException : Exception
    {
        public NotATemplateException(string message) : base (message) { }
    }
}
