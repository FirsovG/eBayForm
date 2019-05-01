using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBayForm.LogicUnits
{
    public struct HtmlTagElement
    {
        public string Tag { get; set; }
        public string Text { get; set; }
        public HtmlTagElement(string tag, string text)
        {
            Tag = tag;
            Text = text;
        }
    }
}
