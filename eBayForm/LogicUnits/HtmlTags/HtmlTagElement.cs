using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBayForm.LogicUnits.HtmlTags
{
    public struct HtmlTagElement : IHtmlTagElement
    {
        public bool IsInList { get; set; }
        public string Element { get; set; }
        public string Value { get; set; }

        public HtmlTagElement(bool isInList, string element, string value)
        {
            IsInList = isInList;
            Element = element;
            Value = value;
        }
    }
}
