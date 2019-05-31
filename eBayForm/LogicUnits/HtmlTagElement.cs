
namespace eBayForm.LogicUnits
{
    public struct HtmlTagElement
    {
        public bool IsInList { get; set; }
        public string Element { get; set; }
        public string Value { get; set; }
        public string Link { get; set; }

        public HtmlTagElement(bool isInList, string element, string value)
        {
            IsInList = isInList;
            Element = element;
            Value = value.Trim();
            Link = null;
        }

        public HtmlTagElement(bool isInList, string element, string value, string link)
        {
            IsInList = isInList;
            Element = element;
            Value = value.Trim();
            Link = link;
        }
    }
}
