using System.Collections.Generic;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Actions.XmlFile
{
    public class XmlBlockUncommentAction : BaseAction
    {
        private readonly IEnumerable<string> _searchPatterns;
        private readonly IXmlConfigManager _xmlConfigManager;
        private readonly string _filePath;

        public XmlBlockUncommentAction(ILogger logger, string filePath, IEnumerable<string> searchPatterns)
            : base(logger)
        {
            _filePath = filePath;
            _searchPatterns = searchPatterns;

            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
        }

        public XmlBlockUncommentAction(ILogger logger, string filePath, string searchPattern)
            : this(logger, filePath, new[] { searchPattern })
        { }
        
        public override void Execute()
        {
            foreach (var pattern in _searchPatterns)
            {
                _xmlConfigManager.UncommentBlock(_filePath, pattern);
            }
        }
    }
}
