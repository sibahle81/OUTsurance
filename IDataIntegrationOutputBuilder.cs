using System.Collections.Generic;
using System.IO;

namespace CsvProcessor
{
    public interface IDataIntegrationOutputBuilder
    {
        IDataIntegrationOutputBuilder SetSerializer(IIntegrationDataSerializer serializer);
        IDataIntegrationOutputBuilder AddListData<TData>(IEnumerable<TData> data);
        IDataIntegrationOutputBuilder AddData<TData>(TData data);

        MemoryStream Build();
    }
}
