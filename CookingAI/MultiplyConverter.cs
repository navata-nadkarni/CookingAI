using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
            
            double a = double.Parse(values[0].ToString());
            double b = double.Parse(values[1].ToString());
            double returnvalue = a * b;
            return returnvalue.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
