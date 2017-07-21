using System;
using System.Collections.Generic;
using System.IO;


namespace CsvProcessor
{
    public interface IDataIntegrationInputBuilder
    {
        IDataIntegrationInputBuilder SetSerializer(IIntegrationDataSerializer serializer);
        IDataIntegrationInputBuilder SetData(MemoryStream stream);
        IDataIntegrationInputBuilder ReadOnce<TData, TDiscriminator>(Action<TData> assignAction, Func<TDiscriminator, bool> discriminator) where TData : new() where TDiscriminator : new();
        IDataIntegrationInputBuilder ReadMany<TData, TDiscriminator>(IList<TData> destination, Func<TDiscriminator, bool> discriminator) where TData : new() where TDiscriminator : new();
        void Build();
    }
}
