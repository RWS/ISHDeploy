using System.Collections.Generic;
using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Data.Commands.XmlFileCommands
{
    public class XmlUncommentCommand : BaseCommand, IRestorable
    {
        private readonly IEnumerable<string> _commentPatterns;
        private readonly IXmlConfigManager _xmlConfigManager;
        private readonly string _filePath;

        public XmlUncommentCommand(ILogger logger, string filePath, IEnumerable<string> commentPatterns)
            : base(logger)
        {
            _commentPatterns = commentPatterns;
            _filePath = filePath;

            _xmlConfigManager = new XmlConfigManager(logger);
        }

        public XmlUncommentCommand(ILogger logger, string filePath, string commentPattern)
            : this(logger, filePath, new[] { commentPattern })
        { }

        public void Backup()
        {
        }

        public override void Execute()
        {
            foreach (var pattern in _commentPatterns)
            {
                _xmlConfigManager.UncommentNode(_filePath, pattern);
            }
        }

        public void Rollback()
        {
        }
    }
}
