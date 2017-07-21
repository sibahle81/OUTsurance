using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CsvProcessor
{
    public class LocalFileTransport : IFileIntegrationTransport
    {
        private MemoryStream _lastRawData;
        public string FilePath { get; set; }

        public MemoryStream CollectRawData()
        {
            MemoryStream mem = new MemoryStream();
            using (var textReader = File.OpenRead(FilePath))
            {
                textReader.CopyTo(mem);
                mem.Seek(0, SeekOrigin.Begin);
            }
            return mem;
        }

        public void SendData(MemoryStream rawData)
        {
            _lastRawData = rawData;

            using (var fileStream = File.Create(FilePath))
            {
                rawData.Seek(0, SeekOrigin.Begin);
                rawData.Position = 0;
                rawData.CopyTo(fileStream);
                rawData.Dispose();
            }
            AfterSend?.Invoke();
        }

        public Action AfterSend { get; set; }
        public MemoryStream GetLastRawData()
        {
            return _lastRawData;
        }

        public void RenameFile(string newFilePath)
        {
            File.Move(FilePath, newFilePath);
        }

        public void DeleteFile()
        {
            File.Delete(FilePath);
        }

        public List<string> ListFiles(string path, string matchPattern = null)
        {
            var files = Directory.GetFiles(path).ToList();
            return files.Where(c => matchPattern == null || Regex.IsMatch(c, matchPattern)).ToList();
        }
    }
}
