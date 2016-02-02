using System.Collections.Generic;
using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Data.Commands.XmlFileCommands
{
    public class XmlCommentCommand : ICommand, IRestorable
    {
        private readonly IEnumerable<string> _commentPatterns;
        private readonly IEnumerable<string> _uncommentPatterns;
        private readonly IXmlConfigManager _xmlConfigManager;

        public XmlCommentCommand(ILogger logger, string filePath, IEnumerable<string> commentPatterns, IEnumerable<string> uncommentPatterns)
        {
            _commentPatterns = commentPatterns;
            _uncommentPatterns = uncommentPatterns;

            _xmlConfigManager = new XmlConfigManager(logger, filePath);
        }

        public void Backup()
        {
            _xmlConfigManager.Backup();
        }

        public void Execute()
        {
            if (_commentPatterns != null)
            {
                foreach (var pattern in _commentPatterns)
                {
                    _xmlConfigManager.CommentNode(pattern);
                }
            }

            if (_uncommentPatterns != null)
            {
                foreach (var pattern in _uncommentPatterns)
                {
                    _xmlConfigManager.UncommentNode(pattern);
                }
            }
        }

        public void Rollback()
        {
            _xmlConfigManager.RestoreOriginal();
        }
    }
}
