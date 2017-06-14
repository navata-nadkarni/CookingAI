using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CookingAI
{
    class MultiplyConverter : IMultiValueConverter
    {
        /*
         IValueConverter
        //value is the quantity
        //parameter is constant 
        */
        /*MultiValueConverter
         * object[] values has 2 values - 
         constant and quantity*/
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Regex check_input = new Regex(@"^[0-9]+$");
            if (values[1].ToString()==string.Empty || values[0].ToString()==string.Empty || !check_input.IsMatch(values[0].ToString()) || !check_input.IsMatch(values[1].ToString()))
            {
                return 0;
            }
            else
            {
                double a = double.Parse(values[0].ToString());
                double b = double.Parse(values[1].ToString());
                double returnvalue = a * b;
                return returnvalue.ToString();
            }
            
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
