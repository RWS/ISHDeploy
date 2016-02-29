using System.Collections.Generic;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data.Actions.XmlFile
{
    public class XmlNodeUncommentAction : SingleXmlFileAction
    {
        private readonly IEnumerable<string> _searchPatterns;

        public XmlNodeUncommentAction(ILogger logger, ISHFilePath filePath, IEnumerable<string> searchPatterns)
			: base(logger, filePath)
        {
            _searchPatterns = searchPatterns;
        }

        public XmlNodeUncommentAction(ILogger logger, ISHFilePath filePath, string searchPattern)
            : this(logger, filePath, new[] { searchPattern })
        { }
        
        public override void Execute()
        {
            foreach (var searchPattern in _searchPatterns)
            {
				XmlConfigManager.UncommentNode(FilePath, searchPattern);
            }
        }
    }
}
