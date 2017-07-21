using System.Collections.Generic;

namespace CsvProcessor
{
    public interface IFileIntegrationTransport : IIntegrationTransport
    {
        string FilePath { get; set; }
        void DeleteFile();
        List<string> ListFiles(string directory = ".", string matchExpression = null);
        void RenameFile(string newPath);

    }
}
