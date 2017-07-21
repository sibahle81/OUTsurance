using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CsvProcessor
{
    public class DataIntegrationService : IDataIntegrationService
    {

        public void SendAsyncData<TData>(IEnumerable<TData> data, IIntegrationDataSerializer serializer, IIntegrationTransport transport) where TData : class
        {
            MemoryStream rawData = serializer.GenerateRawData(data);
            transport.SendData(rawData);
        }

        public void SendAsyncData(IDataIntegrationOutputBuilder outputBuilder, IIntegrationTransport transport)
        {
            transport.SendData(outputBuilder.Build());
        }

        public IEnumerable<TData> ReceiveAsyncData<TData>(IIntegrationDataSerializer deserialiser, IIntegrationTransport transport) where TData : class, new()
        {
            MemoryStream rawData = transport.CollectRawData();
            var result = deserialiser.GetData<TData>(rawData);
            rawData.Dispose();
            return result;
        }

        public void ReceiveAsyncData(IDataIntegrationInputBuilder builder, IIntegrationTransport transport)
        {
            MemoryStream rawData = transport.CollectRawData();
            builder.SetData(rawData);
            builder.Build();
            rawData.Dispose();
        }
    }
}
