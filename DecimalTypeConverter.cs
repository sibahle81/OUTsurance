using System;
using System.Globalization;
using CsvHelper.TypeConversion;

namespace CsvProcessor
{
    public class DecimalTypeConverter : ITypeConverter
    {

        public string DecimalFormat { get; set; }

        public string ConvertToString(TypeConverterOptions options, object value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            return ((Decimal)value).ToString(DecimalFormat);
        }

        public object ConvertFromString(TypeConverterOptions options, string text)
        {
            decimal outValue = 0;
            try
            {
                Decimal.TryParse(text, out outValue);
                return outValue;
            }
            catch (Exception)
            {
                return outValue;
            }
        }

        public bool CanConvertFrom(Type type)
        {
            if (type == typeof(string))
            {
                return true;
            }
            if (type == typeof(Decimal))
            {
                return true;
            }
            return false;
        }

        public bool CanConvertTo(Type type)
        {
            if (type == typeof(string))
            {
                return true;
            }
            if (type == typeof(Decimal))
            {
                return true;
            }
            return false;
        }
    }
}
