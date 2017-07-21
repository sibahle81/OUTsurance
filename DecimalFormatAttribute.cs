using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CsvProcessor
{
    public class DecimalFormatAttribute : Attribute
    {
        public string DecimalFormat { get; set; }

        public DecimalFormatAttribute(string decimalFormat)
        {
            DecimalFormat = decimalFormat;
        }
    }
}
