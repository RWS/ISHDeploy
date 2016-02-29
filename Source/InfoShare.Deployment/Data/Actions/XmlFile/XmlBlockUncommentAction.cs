using System.Collections.Generic;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data.Actions.XmlFile
{
    public class XmlBlockUncommentAction : SingleXmlFileAction
    {
        private readonly IEnumerable<string> _searchPatterns;

        public XmlBlockUncommentAction(ILogger logger, ISHFilePath filePath, IEnumerable<string> searchPatterns)
			: base(logger, filePath)
		{
            _searchPatterns = searchPatterns;
        }

        public XmlBlockUncommentAction(ILogger logger, ISHFilePath filePath, string searchPattern)
            : this(logger, filePath, new[] { searchPattern })
        { }
        
        public override void Execute()
        {
            foreach (var pattern in _searchPatterns)
            {
                XmlConfigManager.UncommentBlock(FilePath, pattern);
            }
        }
    }
}
