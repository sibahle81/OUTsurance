using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using System.Reflection;

namespace CsvProcessor
{
    public class CsvSerializer : IIntegrationDataSerializer
    {
        public bool HasHeaderRecord { get; set; }
        public string Delimiter { get; set; }
        /// <summary>
        /// This flag tells the reader to ignore white space in the headers when matching the columns to the properties by name.
        /// </summary>
        public bool IgnoreHeaderWhiteSpace { get; set; }
        public bool TrimFields { get; set; }
        public bool TrimHeaders { get; set; }

        public CsvSerializer()
        {

        }

        public IEnumerable<TData> GetData<TData>(MemoryStream rawData) where TData : new()
        {
            rawData.Seek(0, SeekOrigin.Begin);
            using (var sr = new StreamReader(rawData))
            {
                var reader = new CsvReader(sr);
                CsvConfigure<TData>(reader.Configuration);
                IEnumerable<TData> records = reader.GetRecords<TData>().ToList();
                return records;
            }
        }

        public MemoryStream GenerateRawData<TData>(IEnumerable<TData> data)
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            var csv = new CsvWriter(streamWriter);
            CsvConfigure<TData>(csv.Configuration);

            csv.WriteRecords(data);
            streamWriter.Flush();
            return memoryStream;
        }

        private void CsvConfigure<TData>(CsvConfiguration configuration)
        {
            configuration.HasHeaderRecord = HasHeaderRecord;
            if (Delimiter != null)
                configuration.Delimiter = Delimiter;
            configuration.IgnoreHeaderWhiteSpace = IgnoreHeaderWhiteSpace;
            configuration.TrimFields = TrimFields;
            configuration.TrimHeaders = TrimHeaders;
            RegisterMap<TData>(configuration);
        }


        public void ReadSingle<TData, TDiscriminator>(Action<TData> assignAction, Func<TDiscriminator, bool> discriminator, MemoryStream rawData) where TData : new() where TDiscriminator : new()
        {
            rawData.Seek(0, SeekOrigin.Begin);
            var sr = new StreamReader(rawData);
            var reader = new CsvReader(sr);
            CsvConfigure<TData>(reader.Configuration);
            while (reader.Read())
            {
                var discriminatorValue = reader.GetRecord<TDiscriminator>();
                if (discriminator(discriminatorValue))
                {
                    assignAction(reader.GetRecord<TData>());
                    return;
                }
            }
        }

        public void ReadMany<TData, TDiscriminator>(IList<TData> destination, Func<TDiscriminator, bool> discriminator, MemoryStream rawData) where TData : new() where TDiscriminator : new()
        {
            var failedRows = new List<string>();

            rawData.Seek(0, SeekOrigin.Begin);
            var sr = new StreamReader(rawData);
            var reader = new CsvReader(sr);
            CsvConfigure<TData>(reader.Configuration);

            while (reader.Read())
            {
                var discriminatorValue = reader.GetRecord<TDiscriminator>();
                if (discriminator(discriminatorValue))
                {

                    try
                    {
                        var record = reader.GetRecord<TData>();
                        destination.Add(record);
                    }
                    catch (Exception sException)
                    {

                        failedRows.Add($"\n({reader.Row}: {sException.Message})");
                    }
                }
            }

            if (failedRows.Any())
            {
                throw new Exception($"Data Integration Error: Serializer Failure: {string.Join(string.Empty, failedRows)}");
            }
        }


        private void RegisterMap<TData>(CsvConfiguration csv)
        {
            var map = csv.AutoMap<TData>();

            foreach (var prop in map.PropertyMaps)
            {
                var dtFormat = prop.Data.Property.GetCustomAttributes(typeof(DateTimeFormatAttribute), true);
                if (dtFormat.Any())
                {
                    var converter = new DateTimeTypeConverter()
                    {
                        DateTimeFormat = ((DateTimeFormatAttribute)dtFormat.First()).DateTimeFormat
                    };
                    prop.TypeConverter(converter);
                }

                var decFormat = prop.Data.Property.GetCustomAttributes(typeof(DecimalFormatAttribute), true);
                if (decFormat.Any())
                {
                    var converter = new DecimalTypeConverter()
                    {
                        DecimalFormat = ((DecimalFormatAttribute)decFormat.First()).DecimalFormat
                    };

                    prop.TypeConverter(converter);
                }
            }
            csv.RegisterClassMap(map);
        }
    }
}
