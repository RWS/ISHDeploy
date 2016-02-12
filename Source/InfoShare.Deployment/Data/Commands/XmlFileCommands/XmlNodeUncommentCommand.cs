using System.Collections.Generic;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Commands.XmlFileCommands
{
    public class XmlNodeUncommentCommand : BaseCommand
    {
        private readonly IEnumerable<string> _searchPatterns;
        private readonly IXmlConfigManager _xmlConfigManager;
        private readonly string _filePath;

        public XmlNodeUncommentCommand(ILogger logger, string filePath, IEnumerable<string> searchPatterns)
            : base(logger)
        {
            _filePath = filePath;
            _searchPatterns = searchPatterns;

            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
        }

        public XmlNodeUncommentCommand(ILogger logger, string filePath, string searchPattern)
            : this(logger, filePath, new[] { searchPattern })
        { }
        
        public override void Execute()
        {
            foreach (var searchPattern in _searchPatterns)
            {
                _xmlConfigManager.UncommentNode(_filePath, searchPattern);
            }
        }
    }
}
