using System.Collections.Generic;
using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Data.Commands.XmlFileCommands
{
    public class XmlNodeCommentCommand : BaseCommand
    {
        private readonly IEnumerable<string> _xpaths;
        private readonly IXmlConfigManager _xmlConfigManager;
        private readonly string _filePath;

        public XmlNodeCommentCommand(ILogger logger, string filePath, IEnumerable<string> xpaths)
            : base(logger)
        {
            _filePath = filePath;
            _xpaths = xpaths;

            _xmlConfigManager = new XmlConfigManager(logger);
        }

        public XmlNodeCommentCommand(ILogger logger, string filePath, string xpath)
            : this(logger, filePath, new[] { xpath })
        { }

        public override void Execute()
        {
            foreach (var xpath in _xpaths)
            {
                _xmlConfigManager.CommentNode(_filePath, xpath);
            }
        }
    }
}
