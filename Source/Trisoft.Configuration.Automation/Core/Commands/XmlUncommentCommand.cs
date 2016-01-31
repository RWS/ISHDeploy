using Trisoft.Configuration.Automation.Core.Managers;

namespace Trisoft.Configuration.Automation.Core.Commands
{
    public class XmlUncommentCommand : ICommand, IRestorable
    {
        private readonly ILogger _logger;
        private readonly string _commentedLineContains;
        private readonly IXmlConfigManager _xmlConfigManager;
        private readonly string _filePath;
        private string _backupFilePath;

        public XmlUncommentCommand(ILogger logger, string filePath, string commentedLineContains)
        {
            _logger = logger;
            _commentedLineContains = commentedLineContains;
            _filePath = filePath;

            _xmlConfigManager = new XmlConfigManager(logger);
        }

        public void Backup()
        {
            _backupFilePath = _xmlConfigManager.Backup(_filePath);
        }

        public void Execute()
        {
            //if (_xmlConfigManager.IsNodeCommented(_commentedLineContains))
            //{
            //    _xmlConfigManager.UncommentNode(_commentedLineContains);
            //}
            _xmlConfigManager.UncommentNode(_commentedLineContains);
        }

        public void Rollback()
        {
            _xmlConfigManager.RestoreOriginal();
        }
    }
}
