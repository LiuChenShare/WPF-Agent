using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace LinKu.Converters
{
    /// <summary>
    /// 属性转换
    /// </summary>
    public class WatermarkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string txt = (string)value;
            if (txt==null)
            {
                txt = "";
            }
            if (txt!= "")
            {
                //有值
                return new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            }

            Brush result;
            if (parameter.ToString()=="name")
            {
                return result = Application.Current.FindResource("LoginNameBrush") as VisualBrush;
            }
            else if (parameter.ToString() == "pwd")
            {
                return result = Application.Current.FindResource("LoginPwdBrush") as VisualBrush;
            }
            else if (parameter.ToString() == "serach")
            {
                return result = Application.Current.FindResource("LoginSerachBrush") as VisualBrush;
            }
            else
            {
                return result = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
