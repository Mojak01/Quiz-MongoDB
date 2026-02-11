using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Labb3.Converters
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
    public class DiffConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = (Difficulty)value;

            if (d == Difficulty.Easy)
            {
                return 0;
            }
            else if (d == Difficulty.Medium)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;

            if (val == 0)
                return Difficulty.Easy;

            if (val == 1)
                return Difficulty.Medium;

            return Difficulty.Hard;
        }
    }
}