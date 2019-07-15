
using System.Collections.Generic;

namespace eBayForm.LogicUnits
{
    public struct HtmlTagElement
    {
        public string Type { get; }
        public bool IsInList { get; }
        public string Name { get; set; }
        public List<string> Values { get; set; }

        // For the GetTags function
        public HtmlTagElement(bool isInList, string name, params string[] values)
        {
            IsInList = isInList;
            Name = name;
            Type = "Unknown";
            Values = new List<string>();

            for (int i = 0; i < values.Length; i++)
            {
                Values.Add(values[i]);
            }

            if (Values.Count == 1)
            {
                if (Values[0].StartsWith("http") || Values[0] == "#")
                {
                    Type = "Image";
                }
                else
                {
                    Type = "Text";
                }
            }
            else if(Values.Count == 2)
            {
                if (Values[1].StartsWith("http") || Values[1] == "#")
                {
                    Type = "Link";
                }
                else
                {
                    Type = "Textblock";
                }
            }
            else if(Values.Count == 3)
            {
                if ((Values[1].StartsWith("http") || Values[1] == "#") && (Values[2].StartsWith("http") || Values[2] == "#"))
                {
                    Type = "ImageAndLink";
                }
            }
        }

        // For the SaveChanges function
        public HtmlTagElement(params string[] values)
        {
            IsInList = false;
            Name = null;
            Type = null;
            Values = new List<string>();

            for (int i = 0; i < values.Length; i++)
            {
                Values.Add(values[i]);
            }
        }
    }
}
