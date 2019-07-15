using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace eBayForm.DesignItems
{
    class OwnColorCanvas : ColorCanvas
    {
        public static DependencyProperty TextBoxForegroundProperty = DependencyProperty.Register("TextBoxColor",
                                                                                              typeof(Brush),
                                                                                              typeof(WatermarkTextBox));
        private TextBox tbHexColor;
        private Style textBoxStyle;


        public Brush TextBoxForeground
        {
            get => (Brush)GetValue(TextBoxForegroundProperty);
            set => SetValue(TextBoxForegroundProperty, value);
        }

        public Style TextBoxStyle { get => textBoxStyle; set => textBoxStyle = value; }

        public OwnColorCanvas() : base()
        {
            TextBoxStyle = new Style(typeof(TextBox));
            TextBoxForeground = Brushes.Black;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            tbHexColor = GetTemplateChild("PART_HexadecimalTextBox") as TextBox;
            tbHexColor.Foreground = TextBoxForeground;
            tbHexColor.Style = TextBoxStyle;
        }
    }
}
