using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CsvProcessor
{
    public interface IDataIntegrationService
    {
        void SendAsyncData<TData>(IEnumerable<TData> data, IIntegrationDataSerializer serializer, IIntegrationTransport transport) where TData : class;
        IEnumerable<TData> ReceiveAsyncData<TData>(IIntegrationDataSerializer deserialiser, IIntegrationTransport transport) where TData : class, new();
        void SendAsyncData(IDataIntegrationOutputBuilder outputBuilder, IIntegrationTransport transport);
        void ReceiveAsyncData(IDataIntegrationInputBuilder builder, IIntegrationTransport transport);
    }
}
