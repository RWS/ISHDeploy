using System.Collections.Generic;
using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Data.Commands.XmlFileCommands
{
    public class XmlBlockCommentCommand : ICommand
    {
        private readonly IEnumerable<string> _searchPatterns;
        private readonly IXmlConfigManager _xmlConfigManager;

        public XmlBlockCommentCommand(ILogger logger, string filePath, IEnumerable<string> searchPatterns)
        {
            _searchPatterns = searchPatterns;

            _xmlConfigManager = new XmlConfigManager(logger, filePath);
        }

        public XmlBlockCommentCommand(ILogger logger, string filePath, string searchPattern)
            : this(logger, filePath, new[] { searchPattern })
        { }

        public void Execute()
        {
            foreach (var pattern in _searchPatterns)
            {
                _xmlConfigManager.CommentBlock(pattern);
            }
        }
    }
}
