using System.Collections.Generic;
using Trisoft.Configuration.Automation.Core.Managers;
using Trisoft.Configuration.Automation.Core.Models;

namespace Trisoft.Configuration.Automation.Core.Commands
{
    public class XmlCommentCommand : ICommand, IRestorable
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<string> _commentPatterns;
        private readonly IXmlConfigManager _xmlConfigManager;

        public XmlCommentCommand(ILogger logger, string filePath, IEnumerable<string> commentPatterns)
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
                _xmlConfigManager.CommentNode(pattern);
            }
        }

        public void Rollback()
        {
            _xmlConfigManager.RestoreOriginal();
        }
    }
}
