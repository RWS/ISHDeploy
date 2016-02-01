using System.Collections.Generic;
using InfoShare.Deployment.Core.Managers;

namespace InfoShare.Deployment.Core.Commands
{
    public class XmlUncommentCommand : ICommand, IRestorable
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<string> _commentPatterns;
        private readonly IXmlConfigManager _xmlConfigManager;

        public XmlUncommentCommand(ILogger logger, string filePath, IEnumerable<string> commentPatterns)
        {
            _logger = logger;
            _commentPatterns = commentPatterns;

            _xmlConfigManager = new XmlConfigManager(logger, filePath);
        }

        public void Backup()
        {
            _xmlConfigManager.Backup();
        }

        public void Execute()
        {
            foreach (var pattern in _commentPatterns)
            {
                _xmlConfigManager.UncommentNode(pattern);
            }
        }

        public void Rollback()
        {
            _xmlConfigManager.RestoreOriginal();
        }
    }
}
