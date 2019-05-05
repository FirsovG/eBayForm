using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBayForm.LogicUnits.HtmlTags
{
    public interface IHtmlTagElement
    {
        bool IsInList { get; set; }
        string Element { get; set; }
        string Value { get; set; }
    }
}
