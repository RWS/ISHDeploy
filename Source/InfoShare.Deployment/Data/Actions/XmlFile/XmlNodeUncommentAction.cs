using System.Collections.Generic;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Actions.XmlFile
{
    public class XmlNodeUncommentAction : BaseAction
    {
        private readonly IEnumerable<string> _searchPatterns;
        private readonly IXmlConfigManager _xmlConfigManager;
        private readonly string _filePath;

        public XmlNodeUncommentAction(ILogger logger, string filePath, IEnumerable<string> searchPatterns)
            : base(logger)
        {
            _filePath = filePath;
            _searchPatterns = searchPatterns;

            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
        }

        public XmlNodeUncommentAction(ILogger logger, string filePath, string searchPattern)
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
