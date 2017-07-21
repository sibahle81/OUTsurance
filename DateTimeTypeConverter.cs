using System;
using System.Globalization;
using CsvHelper.TypeConversion;

namespace CsvProcessor
{
    public class DateTimeTypeConverter : ITypeConverter
    {

        public string DateTimeFormat { get; set; }

        public string ConvertToString(TypeConverterOptions options, object value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            return ((DateTime)value).ToString(DateTimeFormat);
        }

        public object ConvertFromString(TypeConverterOptions options, string text)
        {
            try
            {
                return DateTime.ParseExact(text, DateTimeFormat, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return new DateTime();
            }
        }

        public bool CanConvertFrom(Type type)
        {
            if (type == typeof(string))
            {
                return true;
            }
            if (type == typeof(DateTime))
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
            if (type == typeof(DateTime))
            {
                return true;
            }
            return false;
        }
    }
}
