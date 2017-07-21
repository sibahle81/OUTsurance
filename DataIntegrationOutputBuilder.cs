using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CsvProcessor
{
    public class DataIntegrationOutputBuilder : IDataIntegrationOutputBuilder
    {
        private readonly MemoryStream _memory;
        private IIntegrationDataSerializer _serializer;

        private readonly List<Action> _buildActions = new List<Action>();

        private List<object> _allItems = new List<object>();

        public DataIntegrationOutputBuilder()
        {
            _memory = new MemoryStream();
        }

        /// <summary>
        /// Specify the serializer to use when serializing data.
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        /// <remarks>You can change the serializer at any point</remarks>
        public IDataIntegrationOutputBuilder SetSerializer(IIntegrationDataSerializer serializer)
        {
            _serializer = serializer;
            return this;
        }

        /// <summary>
        /// Add a data set to the output data.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public IDataIntegrationOutputBuilder AddListData<TData>(IEnumerable<TData> data)
        {
            var enumerable = data as IList<TData> ?? data.ToList();
            _allItems.AddRange(enumerable.Cast<object>());
            _buildActions.Add(() =>
            {
                var rawData = _serializer.GenerateRawData(enumerable);
                rawData.Seek(0, SeekOrigin.Begin);
                rawData.Position = 0;
                rawData.CopyTo(_memory);
            });
            return this;
        }

        /// <summary>
        /// Add a data set to the output data.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public IDataIntegrationOutputBuilder AddData<TData>(TData data)
        {
            _allItems.Add(data);
            _buildActions.Add(() =>
            {
                var rawData = _serializer.GenerateRawData(new List<TData> {data});
                rawData.Seek(0, SeekOrigin.Begin);
                rawData.Position = 0;
                rawData.CopyTo(_memory);
            });
            return this;
        }

        public MemoryStream Build()
        {
            _buildActions.ForEach(action => action());
            return _memory;
        }
    }
}