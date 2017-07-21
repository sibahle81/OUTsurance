using System;

namespace CsvProcessor
{
    public class DateTimeFormatAttribute : Attribute
    {
        public string DateTimeFormat { get; set; }

        public DateTimeFormatAttribute(string dateTimeFormat)
        {
            DateTimeFormat = dateTimeFormat;
        }
    }
}
