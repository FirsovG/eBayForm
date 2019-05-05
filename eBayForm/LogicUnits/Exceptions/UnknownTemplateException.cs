using System;

namespace eBayForm.LogicUnits.Exceptions
{
    class UnknownTemplateException : Exception
    {
        public UnknownTemplateException(string message) : base(message) { }
    }
}
