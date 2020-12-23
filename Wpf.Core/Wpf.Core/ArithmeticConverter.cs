using System;
using System.Globalization;
using System.Windows.Data;

namespace Wpf.Core
{
    public class ArithmeticConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                double val = (double)value;

                string[] arithmeticOperations = parameter == null ? null : parameter?.ToString().Split('|');

                if (arithmeticOperations == null) return val;

                foreach (var operation in arithmeticOperations)
                {
                    // get the symbol for the operation (supported symbols: +, -, *, /, %) (always the first character)
                    char symbol = operation[0];

                    string arithmeticValue = operation.Substring(1, operation.Length - 1);

                    double arithmeticVal;
                    bool success = double.TryParse(arithmeticValue, out arithmeticVal);

                    // if successful, do arithmetic operation
                    if (success)
                    {
                        switch (symbol)
                        {
                            case '+':
                                val += arithmeticVal;
                                break;
                            case '-':
                                val -= arithmeticVal;
                                break;
                            case '*':
                                val *= arithmeticVal;
                                break;
                            case '/':
                                val /= arithmeticVal;
                                break;
                            case '%':
                                val %= arithmeticVal;
                                break;
                        }
                    }
                }

                return val;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
