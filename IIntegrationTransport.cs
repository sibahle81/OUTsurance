using System;
using System.IO;

namespace CsvProcessor
{
    public interface IIntegrationTransport
    {
        MemoryStream CollectRawData();

        void SendData(MemoryStream rawData);

        Action AfterSend { get; set; }

        MemoryStream GetLastRawData();
    }
}
