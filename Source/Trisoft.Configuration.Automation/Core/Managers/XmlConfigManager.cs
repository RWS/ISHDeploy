using System;
using System.IO;

namespace Trisoft.Configuration.Automation.Core.Managers
{
    public class XmlConfigManager : IXmlConfigManager, IDisposable
    {
        public readonly ILogger Logger;
        private readonly string _filePath;
        private string _tempFileName;

        public XmlConfigManager(ILogger logger, string filePath)
        {
            Logger = logger;
            _filePath = filePath;
            _tempFileName = Guid.NewGuid().ToString();
        }

        public void Backup()
        {
            _tempFileName = Guid.NewGuid().ToString();
            //File.Copy(_filePath, _tempFileName);
        }

        public void CommentNode(string commentedLineContains)
        {
        }

        public void UncommentNode(string commentedLineContains)
        {
        }

        public bool IsNodeCommented(string commentedLineContains)
        {
            return false;
        }

        public void RestoreOriginal()
        {
            //File.Copy(_tempFileName, _filePath);
        }

        ~XmlConfigManager()
        {
            Dispose();
        }

        public void Dispose()
        {
            // TODO: Restore file or/and delete temp file?
        }
    }
}
