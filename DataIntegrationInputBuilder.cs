using System;
using System.Collections.Generic;
using System.IO;

namespace CsvProcessor
{
    public class DataIntegrationInputBuilder : IDataIntegrationInputBuilder
    {
        private IIntegrationDataSerializer _serializer;
        private MemoryStream _stream;

        private readonly List<Action> _buildActions = new List<Action>();

        public IDataIntegrationInputBuilder SetSerializer(IIntegrationDataSerializer serializer)
        {
            _buildActions.Add(() => _serializer = serializer);
            return this;
        }

        public IDataIntegrationInputBuilder SetData(MemoryStream stream)
        {
            _stream = stream;
            return this;
        }

        public IDataIntegrationInputBuilder ReadOnce<TData, TDiscriminator>(Action<TData> assignAction, Func<TDiscriminator, bool> discriminator) where TDiscriminator : new() where TData : new()
        {
            _buildActions.Add(() => _serializer.ReadSingle(assignAction, discriminator, _stream));
            return this;
        }

        public IDataIntegrationInputBuilder ReadMany<TData, TDiscriminator>(IList<TData> destination, Func<TDiscriminator, bool> discriminator) where TData : new() where TDiscriminator : new()
        {
            _buildActions.Add(() => _serializer.ReadMany(destination, discriminator, _stream));
            return this;
        }

        public void Build()
        {
            _buildActions.ForEach(action =>
            {
                action();
            }
                );
        }
    }
}
