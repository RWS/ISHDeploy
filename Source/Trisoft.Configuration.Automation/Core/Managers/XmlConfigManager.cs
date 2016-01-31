using System;
using System.IO;

namespace Trisoft.Configuration.Automation.Core.Managers
{
    public class XmlConfigManager : IXmlConfigManager
    {
        private readonly ILogger _logger;

        public XmlConfigManager(ILogger logger)
        {
            _logger = logger;
        }

        public string Backup(string filePath)
        {
            var backupFilePath;

            File.Copy(filePath, backupFilePath);

            return backupFilePath;
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
    }
}
