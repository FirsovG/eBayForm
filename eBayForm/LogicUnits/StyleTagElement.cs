
namespace eBayForm.LogicUnits
{
    public struct StyleTagElement
    {
        public string Name;
        public string Value;

        // return
        public StyleTagElement(string value)
        {
            Name = null;
            Value = value;
        }

        // create
        public StyleTagElement(string name, string value)
        {
            Name  = name;
            Value = value;
        }
    }
}
