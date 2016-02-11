using System.Collections.Generic;
using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Data.Commands.XmlFileCommands
{
    public class XmlNodeCommentCommand : ICommand
    {
        private readonly IEnumerable<string> _xpaths;
        private readonly IXmlConfigManager _xmlConfigManager;

        public XmlNodeCommentCommand(ILogger logger, string filePath, IEnumerable<string> xpaths)
        {
            _xpaths = xpaths;

            _xmlConfigManager = new XmlConfigManager(logger, filePath);
        }

        public XmlNodeCommentCommand(ILogger logger, string filePath, string xpath)
            : this(logger, filePath, new[] { xpath })
        { }

        public void Execute()
        {
            foreach (var xpath in _xpaths)
            {
                _xmlConfigManager.CommentNode(xpath);
            }
        }
    }
}
