using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace eBayForm.DesignItems
{
    public class WatermarkTextBox : TextBox
    {
        #region [ Dependency Properties ]

        public static DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark",
                                                                                         typeof(string),
                                                                                         typeof(WatermarkTextBox));

        public static DependencyProperty WatermarkExtraHideProperty = DependencyProperty.Register("WatermarkExtraHide",
                                                                                                  typeof(string),
                                                                                                  typeof(WatermarkTextBox));

        public static DependencyProperty WatermarkColorProperty = DependencyProperty.Register("WatermarkColor",
                                                                                              typeof(Brush),
                                                                                              typeof(WatermarkTextBox));

        public new static DependencyProperty  ForegroundProperty = DependencyProperty.Register("Foreground",
                                                                                               typeof(Brush),
                                                                                               typeof(WatermarkTextBox));


        #endregion


        #region [ Fields ]

        private bool isWatermarked;

        #endregion


        #region [ Properties ]
        

        public string Watermark
        {
            get => (string)GetValue(WatermarkProperty);
            set => SetValue(WatermarkProperty, value);
        }

        public string WatermarkHideString
        {
            get => (string)GetValue(WatermarkExtraHideProperty);
            set => SetValue(WatermarkExtraHideProperty, value);
        }

        public Brush WatermarkColor
        {
            get => (Brush)GetValue(WatermarkColorProperty);
            set => SetValue(WatermarkColorProperty, value);
        }

        public new Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        public new string Text
        {
            get
            {
                if (isWatermarked)
                {
                    if (WatermarkHideString != null)
                    {
                        return WatermarkHideString;
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return base.Text;
                }
            }
            set
            {
                HideWatermark();
                base.Text = value;
            }
        }

        #endregion


        #region [ Constructors ]

        public WatermarkTextBox()
        {
            Loaded += (s, ea) => ShowWatermark();
        }

        public WatermarkTextBox(string watermark)
        {
            Foreground = (Brush)FindResource("PrimaryHueMidForegroundBrush");
            base.Foreground = Foreground;
            WatermarkColor = (Brush)FindResource("WatermarkColor");
            Loaded += (s, ea) => ShowWatermark();
            Watermark = watermark;
        }

        #endregion


        #region [ Event Handlers ]

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            HideWatermark();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            ShowWatermark();
        }


        #endregion


        #region [ Methods ]

        private void ShowWatermark()
        {
            if (string.IsNullOrEmpty(base.Text) || base.Text == WatermarkHideString)
            {
                isWatermarked = true;
                base.Foreground = WatermarkColor;
                base.Text = Watermark;
            }
        }

        private void HideWatermark()
        {
            if (isWatermarked)
            {
                isWatermarked = false;
                base.Foreground = Foreground;
                base.Text = "";
            }
        }

        #endregion
    }
}
