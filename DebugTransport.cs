using System;
using System.Diagnostics;
using System.IO;

namespace CsvProcessor
{
    public class DebugTransport : IIntegrationTransport
    {
        private MemoryStream _lastRawData;

        public MemoryStream CollectRawData()
        {
            return new MemoryStream();
        }

        public void SendData(MemoryStream rawData)
        {
            _lastRawData = rawData;
            rawData.Position = 0;
            using (StreamReader sr = new StreamReader(rawData))
            {
                while (sr.Peek() >= 0)
                {
                    Debug.WriteLine(sr.ReadLine() ?? "");
                }
            }
        }

        public Action AfterSend { get; set; }
        public MemoryStream GetLastRawData()
        {
            return _lastRawData;
        }
    }
}
